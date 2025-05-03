using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;

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
        private int HashCode;

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
        ///     Add Fill action(s) to this rule. This is only used for adding to joint Fill/Forward
        ///     store.
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
        ///     Add Forward action(s) to this rule. This is only used for adding to joint
        ///     Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Forward", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DictionaryConverter<FilterOperation, JToken>))]
        public Dictionary<FilterOperation, JToken> Forward { get; set; } = [];

        /// <summary>
        ///     Changes JSON format for Forward from being the default "mod.esp" : [ "field1",
        ///     "field2" ] to be "field": [ "Mod1.esp", "Mod2.esp" ]
        /// </summary>
        [JsonProperty(PropertyName = "ForwardIndexedByField", NullValueHandling = NullValueHandling.Ignore)]
        [Obsolete("""ForwardIndexedByField is deprecated. Should use "ForwardFlags": ["IndexedByField"]""")]
        public bool ForwardIndexedByField
        {
            get => ForwardOptions.HasFlag(ForwardOptions.IndexedByField);
            set
            {
                Global.Logger.Log(0, """ForwardIndexedByField is deprecated. Should use "ForwardOptions": ["IndexedByField"]""", logLevel: LogLevel.Warning);

                ForwardOptions = ForwardOptions.SetFlag(ForwardOptions.IndexedByField, value);
            }
        }

        /// <summary>
        ///     ForwardType can change how Forwarding actions work.
        /// </summary>
        [JsonProperty(PropertyName = "ForwardOptions", NullValueHandling = NullValueHandling.Ignore)]
        public ForwardOptions ForwardOptions { get; set; }

        /// <summary>
        ///     ForwardType can change how Forwarding actions work.
        /// </summary>
        [JsonProperty(PropertyName = "ForwardType", NullValueHandling = NullValueHandling.Ignore)]
        [Obsolete("""ForwardIndexedByField is deprecated. Should use "ForwardFlags": ["..."]""")]
        public ForwardOptions ForwardType
        {
            get => ForwardOptions;
            set
            {
                Global.Logger.Log(0, """ForwardType is deprecated. Should use "ForwardOptions": ["..."]""", logLevel: LogLevel.Warning);

                ForwardOptions |= value;
            }
        }

        [JsonIgnore]
        public GSPGroup? Group { get; private set; } = null;

        /// <summary>
        ///     Add Forward action(s) to this rule. This is only used for adding to joint
        ///     Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Matches", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DictionaryConverter<FilterOperation, JToken>))]
        public Dictionary<FilterOperation, JToken> Match { get; set; } = [];

        /// <summary>
        ///     List of merge actions to perform.
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
                Types = Global.RecordTypeMappings.All;

            return valid;
        }

        #region GetValues

        private readonly Dictionary<FilterOperation, object?> fillCache = [];
        private readonly Dictionary<FilterOperation, (IEnumerable<ModKey>, string[])?> forwardCache = [];
        private readonly Dictionary<FilterOperation, object?> matchCache = [];

        /// <summary>
        ///     Get the value data for a selected rule's action value key parsed to selected class
        ///     type.
        /// </summary>
        public bool TryGetFillValueAs<T> (FilterOperation key, out T? valueAs) => tryGetValueAs(Fill, fillCache, key, out _, out valueAs);

        /// <summary>
        ///     Gets forward data for current key. If indexed by field, will always have 1 field,
        ///     but 1 or more mods outputted. If indexed by mod, will always have 1 mod, but 1 or
        ///     more fields outputted.
        /// </summary>
        /// <param name="proKeys">Used to see what order mods should be returned in.</param>
        /// <param name="key"></param>
        /// <param name="mods">
        ///     Output of listed mods. List will be in order entered. If no mods were entered when
        ///     indexed by field, then will populate with all mods.
        /// </param>
        /// <param name="fields">Output of field names from config for current key</param>
        /// <returns>
        ///     If valid combination of fields and mods found in config will return true, else
        ///     false.
        /// </returns>
        public bool TryGetForward (ProcessingKeys proKeys, FilterOperation key, [NotNullWhen(true)] out IEnumerable<ModKey>? mods, [NotNullWhen(true)] out string[]? fields)
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

            bool sortMods = proKeys.Rule.HasForwardType(ForwardOptions._sortMods);
            List<ModKey> buildMods = [];
            List<string> buildFields = [];

            if (ForwardOptions.HasFlag(ForwardOptions.IndexedByField))
            {
                buildFields.Add(key.Value);
                if (tryGetValueAs(Forward, [], key, out _, out List<ModKeyListOperation>? values))
                {
                    bool hasIncludeMods = values?.Any(m => m.Operation == ListLogic.Default) ?? false;
                    bool hasExcludeMods = values?.Any(m => m.Operation == ListLogic.NOT) ?? false;
                    sortMods |= hasExcludeMods || !values.SafeAny();

                    if (hasIncludeMods && hasExcludeMods)
                    {
                        Global.Logger.Log(ClassLogCode, $"When indexed by field, array of mods must not include both include and excluded mods.");
                    }
                    else
                    {
                        if (hasExcludeMods || !values.SafeAny())
                        {
                            // Include all mods except excluded
                            foreach (var addMod in Global.LoadOrder)
                            {
                                if (addMod.Enabled // Mod must be enabled
                                    && (!hasExcludeMods || !values.SafeAny(v => v.Value.Equals(addMod.ModKey)))) // Exclude anything excluded in config
                                {
                                    buildMods.Add(addMod.ModKey);
                                }
                            }
                        }
                        else
                        {
                            // Just include mods listed against field
                            foreach (var mod in values ?? [])
                                buildMods.Add(mod.Value);
                        }
                    }
                }
            }
            else
            {
                if (ModKey.TryFromFileName(new FileName(key.Value), out var mod))
                    buildMods.Add(mod);

                if (tryGetValueAs(Forward, [], key, out _, out List<string>? values))
                    buildFields = values ?? buildFields;
            }

            if (buildMods.Count != 0 && buildFields.Count != 0)
            {
                mods = sortMods
                     ? proKeys.Rule.HasForwardType(ForwardOptions._randomMod)
                         ? buildMods.OrderBy(_ => proKeys.GetRandom().Next())
                         : buildMods.OrderByDescending(Global.LoadOrder.IndexOf)
                     : buildMods;

                fields = [.. buildFields];

                forwardCache.Add(key, (mods, fields));

                return true;
            }

            forwardCache.Add(key, null);
            mods = null;
            fields = null;
            return false;
        }

        public bool TryGetMatchValueAs<T> (FilterOperation key, out bool fromCache, out T? valueAs) => tryGetValueAs(Match, matchCache, key, out fromCache, out valueAs);

        /// <summary>
        ///     Get the value data for a selected rule's action value key parsed to selected class
        ///     type.
        /// </summary>
        private static bool tryGetValueAs<T> (Dictionary<FilterOperation, JToken> values, Dictionary<FilterOperation, object?> cache, FilterOperation key, out bool fromCache, out T? valueAs)
        {
            if (cache.TryGetValue(key, out object? cachedValue))
            {
                fromCache = true;
                if (Global.Settings.Value.Logging.NoisyLogs.Cache)
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

                hash.Add(ForwardOptions);
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

        public bool HasForwardType (ForwardOptions flag) => ForwardOptions.HasFlag(flag);

        /// <summary>
        ///     Checks if rule filter(s) match current context's record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        public override bool Matches (ProcessingKeys proKeys)
        {
            if (!base.Matches(proKeys))
                return false;

            if (FormID != null && !MatchesHelper.Matches(proKeys.Record.FormKey, FormID, "Matched FormID: ", Global.Settings.Value.Logging.NoisyLogs.FormIDMatchSuccessful, Global.Settings.Value.Logging.NoisyLogs.FormIDMatchFailed))
                return false;

            if (EditorID != null && !MatchesHelper.Matches(proKeys.Record.EditorID, EditorID, "Matched EditorID: ", Global.Settings.Value.Logging.NoisyLogs.EditorIDMatchSuccessful, Global.Settings.Value.Logging.NoisyLogs.EditorIDMatchFailed))
                return false;

            foreach (var x in Match)
            {
                if (!proKeys.SetProperty(x.Key, x.Key.Value) || !proKeys.Property.Action.CanMatch())
                {
                    Global.TraceLogger?.Log(ClassLogCode, $"Matched {x.Key}: No match enabled RPM for field.");
                    return false;
                }

                Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.MatchesRule)}", propertyName: proKeys.Property.PropertyName);
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