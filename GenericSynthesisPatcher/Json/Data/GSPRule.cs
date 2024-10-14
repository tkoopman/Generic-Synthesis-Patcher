using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Noggog;

namespace GenericSynthesisPatcher.Json.Data
{
    public partial class GSPRule : IComparable<GSPRule>
    {
        internal Dictionary<ValueKey, JToken> fillValues = [];
        internal Dictionary<ValueKey, JToken> forwardValues = [];
        internal Dictionary<ValueKey, JToken> matchValues = [];
        private const int ClassLogPrefix = 0xA00;
        private bool forwardIndexedByField;
        private ForwardTypes forwardType;
        private int HashCode = 0;

        [JsonProperty(PropertyName = "EditorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string>? EditorID { get; set; }

        /// <summary>
        /// Add Fill action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Fill", NullValueHandling = NullValueHandling.Ignore)]
        public JObject? Fill
        {
            set
            {
                foreach (var x in value ?? [])
                {
                    if (x.Value != null)
                        fillValues.Add(new ValueKey(x.Key), x.Value);
                }
            }
        }

        [JsonProperty(PropertyName = "FormID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKey>))]
        public List<FormKey>? FormID { get; set; }

        /// <summary>
        /// Add Forward action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Forward", NullValueHandling = NullValueHandling.Ignore)]
        public JObject? Forward
        {
            set
            {
                foreach (var x in value ?? [])
                {
                    if (x.Value != null)
                        forwardValues.Add(new ValueKey(x.Key), x.Value);
                }
            }
        }

        /// <summary>
        /// Changes JSON format for Forward from being the default "mod.esp" : [ "field1", "field2" ]
        /// to be "field": [ "Mod1.esp", "Mod2.esp" ]
        /// </summary>
        [JsonProperty(PropertyName = "ForwardIndexedByField", NullValueHandling = NullValueHandling.Ignore)]
        public bool ForwardIndexedByField
        {
            get => forwardIndexedByField;
            set
            {
                if (HasForwardType(ForwardTypeFlags.IndexedByField) && !value)
                    throw new ArgumentException("", "ForwardIndexedByField");
                forwardIndexedByField = value;
            }
        }

        /// <summary>
        /// ForwardType can change how Forwarding actions work.
        /// </summary>
        [JsonProperty(PropertyName = "ForwardType", NullValueHandling = NullValueHandling.Ignore)]
        public ForwardTypes ForwardType
        {
            get => forwardType;
            set
            {
                forwardType = value;
                if (HasForwardType(ForwardTypeFlags.IndexedByField))
                    forwardIndexedByField = true;
            }
        }

        [JsonProperty(PropertyName = "Masters", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKey>))]
        public List<ModKey>? Masters { get; set; }

        /// <summary>
        /// Add Forward action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Matches", NullValueHandling = NullValueHandling.Ignore)]
        public JObject? Match
        {
            set
            {
                foreach (var x in value ?? [])
                {
                    if (x.Value != null)
                        matchValues.Add(new ValueKey(x.Key), x.Value);
                }
            }
        }

        [JsonProperty(PropertyName = "-EditorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string>? NotEditorID { get; set; }

        [JsonProperty(PropertyName = "-FormID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKey>))]
        public List<FormKey>? NotFormID { get; set; }

        [JsonProperty(PropertyName = "OnlyIfDefault", NullValueHandling = NullValueHandling.Ignore)]
        public bool OnlyIfDefault { get; set; }

        [DefaultValue(0)]
        [JsonProperty(PropertyName = "Priority", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Priority { get; set; }

        [JsonProperty(PropertyName = "Types", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<GSPRule.Type>))]
        public List<GSPRule.Type>? Types { get; set; }

        /// <summary>
        /// Create new rule, and validates it meets basic needs.
        /// </summary>
        public GSPRule ( int priority, List<GSPRule.Type>? types, List<FormKey>? formID, List<string>? editorID, List<FormKey>? notFormID, List<string>? notEditorID, JObject? match, JObject? fill, JObject? forward, List<ModKey>? masters, ForwardTypes forwardType )
        {
            Priority = priority;
            Types = ValidateList(types);
            FormID = ValidateList(formID);
            EditorID = ValidateList(editorID);
            NotFormID = ValidateList(notFormID);
            NotEditorID = ValidateList(notEditorID);
            Masters = ValidateList(masters);
            Match = match;
            Fill = fill;
            Forward = forward;
            ForwardType = forwardType;

            if (types == null && formID == null && editorID == null)
                throw new JsonSerializationException("Each Json record must contain at least one basic filter (types, editorID or formID)");
        }

        public int CompareTo ( GSPRule? other )
        {
            if (other == null)
                return 1;
            if (other == this)
                return 0;

            if (Priority != other.Priority)
                return Priority.CompareTo(other.Priority);

            int left
                = ((Types?.Count ?? 0) << 8)
                + ((FormID?.Count ?? 0) << 4)
                + EditorID?.Count ?? 0;
            int right
                = ((other.Types?.Count ?? 0) << 8)
                + ((other.FormID?.Count ?? 0) << 4)
                + other.EditorID?.Count ?? 0;

            return left.CompareTo(right);
        }

        #region GetValues

        private Dictionary<ValueKey, object?> fillCache = [];
        private Dictionary<ValueKey, (IReadOnlyDictionary<ModKey, IModListing<ISkyrimModGetter>>, string[])?> forwardCache = [];
        private Dictionary<ValueKey, object?> matchCache = [];

        public T? GetFillValueAs<T> ( ValueKey key ) => GetValueAs<T>(fillValues, fillCache, key);

        public T? GetMatchValueAs<T> ( ValueKey key ) => GetValueAs<T>(matchValues, matchCache, key);

        public bool TryGetForward ( ValueKey key, [NotNullWhen(true)] out IReadOnlyDictionary<ModKey, IModListing<ISkyrimModGetter>>? mods, [NotNullWhen(true)] out string[]? fields )
        {
            if (forwardCache.TryGetValue(key, out var value))
            {
                if (value == null || !value.HasValue)
                {
                    mods = null;
                    fields = null;
                    return false;
                }

                mods = value.Value.Item1;
                fields = value.Value.Item2;

                return true;
            }

            Dictionary<ModKey, IModListing<ISkyrimModGetter>> buildMods = [];
            List<string> buildFields = [];

            if (ForwardIndexedByField)
            {
                buildFields.Add(key.Key);
                foreach (var mod in GetValueAs<List<ModKey>>(forwardValues, [], key) ?? [])
                    buildMods.Add(mod, Global.State.LoadOrder[mod]);
            }
            else
            {
                if (ModKey.TryFromFileName(new FileName(key.Key), out var mod))
                    buildMods.Add(mod, Global.State.LoadOrder[mod]);

                buildFields = GetValueAs<List<string>>(forwardValues, [], key) ?? buildFields;
            }

            if (buildMods.Count != 0 && buildFields.Count != 0)
            {
                mods = new ReadOnlyDictionary<ModKey, IModListing<ISkyrimModGetter>>(buildMods);
                fields = [.. buildFields];

                forwardCache.Add(key, (mods, fields));

                return true;
            }

            forwardCache.Add(key, null);
            mods = null;
            fields = null;
            return false;
        }

        /// <summary>
        /// Get the value data for a selected rule's action value key parsed to selected class type.
        /// </summary>
        private static T? GetValueAs<T> ( Dictionary<ValueKey, JToken> values, Dictionary<ValueKey, object?> cache, ValueKey key )
        {
            if (cache.TryGetValue(key, out object? value))
            {
                return value is T v ? v : throw new InvalidOperationException($"Invalid value type returned for {key.Key}");
            }

            if (values.TryGetValue(key, out var jsonValue))
            {
                T? o = default;
                if (jsonValue.Type != JTokenType.Null)
                {
                    if (typeof(T) == typeof(string))
                    {
                        o = (T)(object)(jsonValue.Type == JTokenType.String ? jsonValue.ToString()
                        : throw new InvalidOperationException($"Invalid value type returned for {key.Key}"));
                    }
                    else if (JsonConvert.DeserializeObject<T>(jsonValue.ToString(), Global.SerializerSettings) is T value2)
                    {
                        o = value2;
                    }
                }

                cache.Add(key, o);
                return o;
            }

            return default;
        }

        #endregion GetValues

        public override int GetHashCode ()
        {
            if (HashCode == 0)
            {
                var hash = new HashCode();

                hash.Add(ForwardIndexedByField);
                hash.Add(ForwardType);
                hash.Add(OnlyIfDefault);
                hash.Add(Priority);
                if (EditorID != null)
                    hash.AddEnumerable(EditorID);
                if (FormID != null)
                    hash.AddEnumerable(FormID);
                if (Masters != null)
                    hash.AddEnumerable(Masters);
                if (NotEditorID != null)
                    hash.AddEnumerable(NotEditorID);
                if (NotFormID != null)
                    hash.AddEnumerable(NotFormID);
                if (Types != null)
                    hash.AddEnumerable(Types);
                if (matchValues != null)
                    hash.AddDictionary(matchValues);
                if (fillValues != null)
                    hash.AddDictionary(fillValues);
                if (forwardValues != null)
                    hash.AddDictionary(forwardValues);

                HashCode = hash.ToHashCode();
            }

            return HashCode;
        }

        public bool HasForwardType ( ForwardTypeFlags flag ) => ((ForwardTypeFlags)ForwardType).HasFlag(flag);

        /// <summary>
        /// Checks if rule filter(s) match current context's record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        public bool Matches ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            bool matches = MatchesBasicFilters(context)
                        && MatchesExtraFilters(context);

            if (Global.Settings.Value.TraceFormKey != null && context.Record.FormKey.Equals(Global.Settings.Value.TraceFormKey))
            {
                LogHelper.Log(LogLevel.Trace, context, $"MatchesBasicFilters: {MatchesBasicFilters(context)} HasTypes: {Types != null} HasEditorID: {EditorID != null} HasFormID: {FormID != null}", ClassLogPrefix | 0x11);
                LogHelper.Log(LogLevel.Trace, context, $"MatchesExtraFilters: {MatchesExtraFilters(context)} HasNotEditorID: {NotEditorID != null} HasNotFormID: {NotFormID != null} HasMasters: {Masters != null}", ClassLogPrefix | 0x12);
            }

            return matches;
        }

        /// <summary>
        /// Validates Lists to make sure it is null if list contains 0 entries and no duplicates.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>Cleaned List.</returns>
        private static List<T>? ValidateList<T> ( List<T>? list ) => list.SafeAny() ? list.Distinct().ToList() : null;

        /// <summary>
        /// Checks basic filters.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns>Returns true if context matches basic filters.</returns>
        private bool MatchesBasicFilters ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            var recordType = GetGSPRuleType(context.Record);
            if (recordType == GSPRule.Type.UNKNOWN)
                return false;

            if (Types != null && !Types.Contains(recordType))
                return false;

            if (FormID != null && !FormID.Contains(context.Record.FormKey))
                return false;

            // Can assume true if no EditorID filter as GSPRule MUST contain 1 filter
            // Which means one of the above checks must of have contained a matching value
            if (EditorID == null)
                return true;

            foreach (string editorID in EditorID)
            {
                if (editorID.StartsWith('/') && editorID.EndsWith('/'))
                {
                    var matchedRegex = new Regex(editorID.Trim('/'));
                    if (matchedRegex.IsMatch(context.Record.EditorID ?? ""))
                        return true;
                }
                else if (editorID.Equals(context.Record.EditorID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks Extra filters.
        /// Excludes OnlyIfDefault as that must be checked by the appropriate Actions if all other filters match.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Return true if context matches any defined extra filters.</returns>
        private bool MatchesExtraFilters ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            if (Masters != null && !Masters.Contains(context.Record.FormKey.ModKey))
                return false;

            if (NotFormID != null && NotFormID.Contains(context.Record.FormKey))
                return false;

            if (NotEditorID != null)
            {
                foreach (string editorID in NotEditorID)
                {
                    if (editorID.StartsWith('/') && editorID.EndsWith('/'))
                    {
                        var matchedRegex = new Regex(editorID.Trim('/'));
                        if (matchedRegex.IsMatch(context.Record.EditorID ?? ""))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (editorID.Equals(context.Record.EditorID))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}