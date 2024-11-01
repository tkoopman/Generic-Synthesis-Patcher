using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
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

                editorIDs = value;
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

                editorIDs = [];
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

                formIDs = value;
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

                formIDs = [];
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

        public bool ClaimAndValidate (GSPGroup group)
        {
            if (Group != null)
                throw new Exception("Rule already claimed.");

            Group = group;

            if (Types.Count == 0)
                Types = Group.Types;  // This may also be NONE but we do extra check for that after validation.
            else if (Group.Types.Count > 0 && Types.Any(t => !Group.Types.Contains(t)))
                throw new Exception($"Record under group tries to filter for Type(s) that are excluded at by group.");

            if (Priority != 0)
                LogHelper.WriteLog(LogLevel.Information, ClassLogCode, "You have defined a rule priority, for a rule in a group. Group member priorities are ignored. Order in file is processing order.", rule: this);

            bool valid = Validate();
            if (Types.Count == 0)
                Types = RecordTypeMappings.All;

            return valid;
        }

        #region GetValues

        private readonly Dictionary<FilterOperation, object?> fillCache = [];
        private readonly Dictionary<FilterOperation, (IReadOnlyDictionary<ModKey, IModListing<ISkyrimModGetter>>, string[])?> forwardCache = [];
        private readonly Dictionary<FilterOperation, object?> matchCache = [];

        public bool TryGetFillValueAs<T> (FilterOperation key, out T? valueAs) => TryGetValueAs(Fill, fillCache, key, out _, out valueAs);

        public bool TryGetForward (FilterOperation key, [NotNullWhen(true)] out IReadOnlyDictionary<ModKey, IModListing<ISkyrimModGetter>>? mods, [NotNullWhen(true)] out string[]? fields)
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
                if (TryGetValueAs(Forward, [], key, out _, out List<ModKey>? values))
                {
                    foreach (var mod in values ?? [])
                        buildMods.Add(mod, Global.State.LoadOrder[mod]);
                }
            }
            else
            {
                if (ModKey.TryFromFileName(new FileName(key.Value), out var mod))
                    buildMods.Add(mod, Global.State.LoadOrder[mod]);

                if (TryGetValueAs(Forward, [], key, out _, out List<string>? values))
                    buildFields = values ?? buildFields;
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

        public bool TryGetMatchValueAs<T> (FilterOperation key, out bool fromCache, out T? valueAs) => TryGetValueAs(Match, matchCache, key, out fromCache, out valueAs);

        /// <summary>
        /// Get the value data for a selected rule's action value key parsed to selected class type.
        /// </summary>
        private static bool TryGetValueAs<T> (Dictionary<FilterOperation, JToken> values, Dictionary<FilterOperation, object?> cache, FilterOperation key, out bool fromCache, out T? valueAs)
        {
            if (cache.TryGetValue(key, out object? cachedValue))
            {
                fromCache = true;
                Global.TraceLogger?.Log(ClassLogCode, $"Got value for {key.Value} from cache.");
                if (cachedValue is T v)
                {
                    valueAs = v;
                    return true;
                }
                else
                {
                    valueAs = default;
                    // If value != null then failed to cast to correct type
                    return cachedValue == null;
                }
            }

            fromCache = false;
            valueAs = default;
            bool valid = values.TryGetValue(key, out var jsonValue);

            if (valid && jsonValue != null)
                valueAs = jsonValue.Deserialize<T>();

            cache.Add(key, valueAs);
            return valid;
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

        public bool HasForwardType (ForwardTypeFlags flag) => ((ForwardTypeFlags)ForwardType).HasFlag(flag);

        /// <summary>
        /// Checks if rule filter(s) match current context's record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        public override bool Matches (ProcessingKeys proKeys)
        {
            if (!base.Matches(proKeys))
                return false;

            if (FormID != null && !MatchesHelper.Matches(proKeys.Record.FormKey, FormID, "Matched FormID: "))
                return false;

            if (EditorID != null && !MatchesHelper.Matches(proKeys.Record.EditorID, EditorID, "Matched FormID: "))
                return false;

            foreach (var x in Match)
            {
                if (!proKeys.SetProperty(x.Key, x.Key.Value) || !proKeys.Property.Action.CanMatch())
                {
                    Global.TraceLogger?.Log(ClassLogCode, $"Matched {x.Key}: No match enabled RPM for field.");
                    return false;
                }

                Global.TraceLogger?.Log(ClassLogCode, $"Action: {proKeys.Property.Action.GetType().GetClassName()}.MatchesRule", propertyName: proKeys.Property.PropertyName);
                if (!proKeys.Property.Action.MatchesRule(proKeys))
                    return false;
            }

            return true;
        }

        public override bool Validate ()
        {
            if (!base.Validate())
                return false;

            if (Types.Count == 0)
            {
                LogHelper.WriteLog(LogLevel.Critical, ClassLogCode, "Rules must specify record type(s) either directly in the rule or via type(s) added at a group level.", rule: this);
                return false;
            }

            return true;
        }
    }
}