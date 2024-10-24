using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using DynamicData;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

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
        private const int ClassLogCode = 0x04;
        private List<ListOperation>? editorIDs;
        private List<FormKeyListOperation>? formIDs;
        private bool forwardIndexedByField;
        private ForwardTypes forwardType;
        private int HashCode = 0;

        [JsonProperty(PropertyName = "EditorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<ListOperation>))]
        public List<ListOperation>? EditorID
        {
            get => editorIDs;
            set
            {
                if (!value.SafeAny())
                    return;

                editorIDs ??= [];
                editorIDs.Add(value);
            }
        }

        [JsonProperty(PropertyName = "-EditorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<ListOperation>))]
        public List<ListOperation>? EditorIDDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                editorIDs ??= [];
                foreach (var v in value)
                    editorIDs.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!EditorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<ListOperation>))]
        public List<ListOperation>? EditorIDNot { set => EditorIDDel = value; }

        /// <summary>
        /// Add Fill action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Fill", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DictionaryConverter<FilterOperation, JToken>))]
        public Dictionary<FilterOperation, JToken> Fill { get; set; } = [];

        [JsonProperty(PropertyName = "FormID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKeyListOperation>))]
        public List<FormKeyListOperation>? FormID
        {
            get => formIDs;
            set
            {
                if (!value.SafeAny())
                    return;

                formIDs ??= [];
                formIDs.Add(value);
            }
        }

        [JsonProperty(PropertyName = "-FormID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKeyListOperation>))]
        public List<FormKeyListOperation>? FormIDDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                formIDs ??= [];
                foreach (var v in value)
                    formIDs.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!FormID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKeyListOperation>))]
        public List<FormKeyListOperation>? FormIDNot { set => FormIDDel = value; }

        /// <summary>
        /// Add Forward action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Forward", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DictionaryConverter<FilterOperation, JToken>))]
        public Dictionary<FilterOperation, JToken> Forward { get; set; } = [];

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
        [JsonConverter(typeof(DictionaryConverter<FilterOperation, JToken>))]
        public Dictionary<FilterOperation, JToken> Match { get; set; } = [];

        /// <summary>
        /// List of merge actions to perform.
        /// </summary>
        [JsonProperty(PropertyName = "Merge", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DictionaryConverter<FilterOperation, List<ModKeyListOperation>>))]
        public Dictionary<FilterOperation, List<ModKeyListOperation>?> Merge { get; set; } = [];

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
                LogHelper.WriteLog(LogLevel.Information, ClassLogCode, "You have defined a rule priority, for a rule in a group. Group member priorities are ignored. Order in file is processing order.", rule: this);

            bool valid = Validate();
            if (Types == RecordTypes.NONE)
                Types = RecordTypes.All;

            return valid;
        }

        #region GetValues

        private readonly Dictionary<FilterOperation, object?> fillCache = [];
        private readonly Dictionary<FilterOperation, (IReadOnlyDictionary<ModKey, IModListing<ISkyrimModGetter>>, string[])?> forwardCache = [];
        private readonly Dictionary<FilterOperation, object?> matchCache = [];

        public T? GetFillValueAs<T> ( FilterOperation key ) => GetValueAs<T>(Fill, fillCache, key);

        public T? GetMatchValueAs<T> ( FilterOperation key ) => GetValueAs<T>(Match, matchCache, key);

        public bool TryGetForward ( FilterOperation key, [NotNullWhen(true)] out IReadOnlyDictionary<ModKey, IModListing<ISkyrimModGetter>>? mods, [NotNullWhen(true)] out string[]? fields )
        {
            if (forwardCache.TryGetValue(key, out var value))
            {
                if (value == null || !value.HasValue)
                {
                    mods = null;
                    fields = null;
                    return false;
                }

                (mods, fields) = value.Value;

                return true;
            }

            Dictionary<ModKey, IModListing<ISkyrimModGetter>> buildMods = [];
            List<string> buildFields = [];

            if (ForwardIndexedByField)
            {
                buildFields.Add(key.Value);
                foreach (var mod in GetValueAs<List<ModKey>>(Forward, [], key) ?? [])
                    buildMods.Add(mod, Global.State.LoadOrder[mod]);
            }
            else
            {
                if (ModKey.TryFromFileName(new FileName(key.Value), out var mod))
                    buildMods.Add(mod, Global.State.LoadOrder[mod]);

                buildFields = GetValueAs<List<string>>(Forward, [], key) ?? buildFields;
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
        private static T? GetValueAs<T> ( Dictionary<FilterOperation, JToken> values, Dictionary<FilterOperation, object?> cache, FilterOperation key )
        {
            if (cache.TryGetValue(key, out object? value))
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Got value for {key.Value} from cache.");
                return value == null ? default
                     : value is T v ? v
                     : throw new InvalidOperationException($"Invalid value type returned for {key.Value} - {value?.GetType().FullName ?? "null"}");
            }

            if (values.TryGetValue(key, out var jsonValue))
            {
                var o = jsonValue.Deserialize<T>();

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
                if (Match != null)
                    hash.AddDictionary(Match);
                if (Fill != null)
                    hash.AddDictionary(Fill);
                if (Forward != null)
                    hash.AddDictionary(Forward);

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

            if (FormID != null)
            {
                bool hasEntry = FormID?.Any(id => id.Value.Equals(context.Record.FormKey)) ?? false;
                bool isNeg = FormID != null && FormID.First().Operation == ListLogic.NOT;
                bool result = isNeg ? !hasEntry : (FormID == null || hasEntry);
                Global.TraceLogger?.Log(ClassLogCode, $"Check FormID: {result} Has FormID: {FormID != null} Found: {hasEntry} Not: {isNeg}");

                if (!result)
                    return false;
            }

            if (EditorID != null)
            {
                bool hasEntry = !context.Record.EditorID.IsNullOrEmpty() && EditorID.Any(id => id.MatchesValue(context.Record.EditorID));
                bool isNeg = EditorID != null && EditorID.First().Operation == ListLogic.NOT;
                bool result = isNeg ? !hasEntry : (EditorID == null || hasEntry);

                Global.TraceLogger?.Log(ClassLogCode, $"Check EditorID: {result} Has EditorID: {EditorID != null} Record Found: {hasEntry} Not: {isNeg}");

                if (!result)
                    return false;
            }

            foreach (var x in Match)
            {
                var rcd = RCDMapping.FindRecordCallData(context, x.Key.Value);

                if (rcd == null || !rcd.Matches(context.Record, this, x.Key, rcd))
                {
                    Global.TraceLogger?.Log(ClassLogCode, $"Failed on match. Field: {x.Key.Value} RCD Class: {rcd?.GetType().GenericTypeArguments[0].Name ?? "None found."}");
                    return false;
                }
            }

            return true;
        }

        public override bool Validate ()
        {
            if (!base.Validate())
                return false;

            if (FormID == null && EditorID == null && Types == RecordTypes.NONE)
            {
                LogHelper.WriteLog(LogLevel.Critical, ClassLogCode, "Each rule in config must contain at least one basic filter (types, editorID or formID)", rule: this);
                return false;
            }

            return true;
        }
    }
}