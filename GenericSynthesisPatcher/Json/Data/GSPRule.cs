using System.Collections.ObjectModel;
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
    public class GSPRule : GSPBase
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

        [JsonIgnore]
        public GSPGroup? Group { get; private set; } = null;

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

        public bool ClaimAndValidate ( GSPGroup group )
        {
            if (Group != null)
                throw new Exception("Rule already claimed.");

            Group = group;

            if (Types == RecordTypes.NONE)
                Types = Group.Types;  // This may also be NONE but we do extra check for that after validation.
            else if (Group.Types != RecordTypes.NONE && (Types & ~Group.Types) != RecordTypes.NONE)
                throw new Exception($"Record under group tries to filter for Type(s) [{Types & ~Group.Types}] that are excluded at by group.");

            if (Priority != 0)
                LogHelper.Log(LogLevel.Information, "You have defined a rule priority, for a rule in a group. Group member priorities are ignored. Order in file is processing order.", 0x00);

            bool valid = Validate();
            if (Types == RecordTypes.NONE)
                Types = RecordTypes.All;

            return valid;
        }

        #region GetValues

        private readonly Dictionary<ValueKey, object?> fillCache = [];
        private readonly Dictionary<ValueKey, (IReadOnlyDictionary<ModKey, IModListing<ISkyrimModGetter>>, string[])?> forwardCache = [];
        private readonly Dictionary<ValueKey, object?> matchCache = [];

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
                hash.Add(base.GetHashCode());

                hash.Add(ForwardIndexedByField);
                hash.Add(ForwardType);
                hash.Add(OnlyIfDefault);
                if (EditorID != null)
                    hash.AddEnumerable(EditorID);
                if (FormID != null)
                    hash.AddEnumerable(FormID);
                if (NotEditorID != null)
                    hash.AddEnumerable(NotEditorID);
                if (NotFormID != null)
                    hash.AddEnumerable(NotFormID);
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
        public override bool Matches ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            if (!base.Matches(context))
                return false;

            bool trace = Global.Settings.Value.TraceFormKey != null && context.Record.FormKey.Equals(Global.Settings.Value.TraceFormKey);

            if (trace && FormID != null)
                LogHelper.Log(LogLevel.Trace, context, $"Check FormID: {FormID != null && !FormID.Contains(context.Record.FormKey)}", ClassLogPrefix | 0x10);

            if (FormID != null && !FormID.Contains(context.Record.FormKey))
                return false;

            if (trace && NotFormID != null)
                LogHelper.Log(LogLevel.Trace, context, $"Check FormID: {NotFormID != null && !NotFormID.Contains(context.Record.FormKey)}", ClassLogPrefix | 0x10);

            if (NotFormID != null && NotFormID.Contains(context.Record.FormKey))
                return false;

            if (trace && EditorID != null)
                LogHelper.Log(LogLevel.Trace, context, $"Check EditorID: {MatchesEditorID(context, EditorID)}", ClassLogPrefix | 0x10);

            if (!MatchesEditorID(context, EditorID))
                return false;

            if (trace && NotEditorID != null)
                LogHelper.Log(LogLevel.Trace, context, $"Check NotEditorID: {!MatchesEditorID(context, NotEditorID)}", ClassLogPrefix | 0x10);

            if (NotEditorID != null && MatchesEditorID(context, NotEditorID))
                return false;

            foreach (var x in matchValues)
            {
                var rcd = RCDMapping.FindRecordCallData(context, x.Key.Key);

                if (rcd != null && !rcd.Matches(context.Record, this, x.Key, rcd))
                {
                    LogHelper.Log(LogLevel.Trace, context, $"Failed on match. Field: {x.Key}", ClassLogPrefix | 0x32);
                    return false;
                }
            }

            return true;
        }

        public override bool Validate ()
        {
            if (FormID == null && EditorID == null && Types == RecordTypes.NONE)
            {
                LogHelper.Log(LogLevel.Critical, "Each rule in config must contain at least one basic filter (types, editorID or formID)", 0xFE);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks basic filters.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns>Returns true if context matches basic filters.</returns>
        private static bool MatchesEditorID ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IReadOnlyList<string>? ids )
        {
            // Can assume true if no EditorID filter as GSPRule MUST contain 1 filter
            // Which means one of the above checks must of have contained a matching value
            if (ids == null)
                return true;

            foreach (string editorID in ids)
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
    }
}