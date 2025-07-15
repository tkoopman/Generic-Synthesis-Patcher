using System.Diagnostics.CodeAnalysis;

using Common;

using DynamicData;

using GenericSynthesisPatcher.Games.Universal.Action;
using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Rules.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Noggog;

namespace GenericSynthesisPatcher.Rules
{
    public class GSPRule : GSPBase
    {
        private const int ClassLogCode = 0x1C;
        private List<ListOperation>? editorIDs;
        private List<FormKeyListOperation>? formIDs;
        private int HashCode;

        /// <summary>
        ///     Details for DeepCopIn Action if required by this rule.
        /// </summary>
        [JsonProperty(PropertyName = "Copy", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<GSPDeepCopyIn>))]
        public List<GSPDeepCopyIn> DeepCopyIn { get; set; } = [];

        /// <summary>
        ///     List of EditorIDs to match against that will be included or excluded. If not set,
        ///     not filter any records out by EditorID.
        /// </summary>
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
        ///     Add Fill action(s) to this rule. This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Fill", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DictionaryConverter<FilterOperation, JToken>))]
        public Dictionary<FilterOperation, JToken> Fill { get; set; } = [];

        /// <summary>
        ///     List of FormIDs to match against that will be included or excluded. If not set, not
        ///     filter any records out by FormID.
        /// </summary>
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
                Global.Logger.WriteLog(LogLevel.Warning, LogType.GeneralConfigFailure, """ForwardIndexedByField is deprecated. Should use "ForwardOptions": ["IndexedByField"]""", ClassLogCode);

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
        [Obsolete("""ForwardType is deprecated. Should use "ForwardOptions": ["..."]""")]
        public ForwardOptions ForwardType
        {
            get => ForwardOptions;
            set
            {
                Global.Logger.WriteLog(LogLevel.Warning, LogType.GeneralConfigFailure, """ForwardType is deprecated. Should use "ForwardOptions": ["..."]""", ClassLogCode);

                ForwardOptions |= value;
            }
        }

        /// <summary>
        ///     If rule belongs to a group, this will point to the group that it belongs to.
        /// </summary>
        [JsonIgnore]
        public GSPGroup? Group { get; private set; } = null;

        public virtual bool HasActions => Fill.SafeAny() || Forward.SafeAny() || Merge.SafeAny() || DeepCopyIn.SafeAny();

        /// <summary>
        ///     Add filters to this rule that can filter on supported properties that are not
        ///     EditorID or FormID.
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

        /// <summary>
        ///     If set to true, will only apply this rule if the winning record is set to the
        ///     default value in the master record.
        /// </summary>
        [JsonProperty(PropertyName = "OnlyIfDefault", NullValueHandling = NullValueHandling.Ignore)]
        public bool OnlyIfDefault { get; set; }

        /// <summary>
        ///     When a rule is in a group this is the method called to tell the rule the group it
        ///     belongs to. Will also then validate this rule.
        /// </summary>
        /// <returns>True if rule validation passes, else false.</returns>
        /// <exception cref="Exception">
        ///     If rule already claimed by a group, or if if filters for types the parent don't include.
        /// </exception>
        public bool ClaimAndValidate (GSPGroup group)
        {
            if (Group is not null)
                throw new Exception("Rule already claimed.");

            Group = group;

            if (!Types.Any())
                Types = Group.Types;  // This may also be NONE but we do extra check for that after validation.
            else if (Group.Types.Any() && Types.Any(t => !Group.Types.Contains(t)))
                throw new Exception("Record under group tries to filter for Type(s) that are excluded at by group.");

            if (Priority != 0)
                Global.Logger.WriteLog(LogLevel.Error, LogType.GeneralConfigFailure, "You have defined a rule priority, for a rule in a group. Group member priorities are ignored. Order in file is processing order.", ClassLogCode, includePrefix: GetLogRuleID());

            bool valid = Validate();
            if (!Types.Any())
                Types = Global.Game.AllRecordTypes();

            return valid;
        }

        #region GetValues

        private readonly Dictionary<FilterOperation, object?> fillCache = [];
        private readonly Dictionary<FilterOperation, (IEnumerable<ModKey>, string[])?> forwardCache = [];
        private readonly Dictionary<FilterOperation, object?> matchCache = [];

        /// <summary>
        ///     Get the value data for a selected rule's action value key parsed to selected class type.
        /// </summary>
        public bool TryGetFillValueAs<T> (FilterOperation key, out T? valueAs) => tryGetValueAs(Fill, fillCache, key, out _, out valueAs);

        public bool TryGetMatchValueAs<T> (FilterOperation key, out bool fromCache, out T? valueAs) => tryGetValueAs(Match, matchCache, key, out fromCache, out valueAs);

        /// <summary>
        ///     Get the value data for a selected rule's action value key parsed to selected class type.
        /// </summary>
        private static bool tryGetValueAs<T> (Dictionary<FilterOperation, JToken> values, Dictionary<FilterOperation, object?> cache, FilterOperation key, out bool fromCache, out T? valueAs)
        {
            if (cache.TryGetValue(key, out object? cachedValue))
            {
                fromCache = true;
                Global.Logger.WriteLog(LogLevel.Trace, LogType.Cache, $"Got value for {key.Value} from cache.", ClassLogCode);

                if (cachedValue is T v)
                {
                    valueAs = v;
                    return true;
                }
                else
                {
                    valueAs = default;

                    // If value is not null then failed to cast to correct type
                    return cachedValue is null;
                }
            }

            fromCache = false;
            valueAs = default;
            bool valid = values.TryGetValue(key, out var jsonValue);

            if (valid && jsonValue is not null)
                valueAs = jsonValue.Deserialize<T>();

            cache.Add(key, valueAs);
            return valid;
        }

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
        ///     If valid combination of fields and mods found in config will return true, else false.
        /// </returns>
        private bool tryGetForward (ProcessingKeys proKeys, FilterOperation key, [NotNullWhen(true)] out IEnumerable<ModKey>? mods, [NotNullWhen(true)] out string[]? fields)
        {
            if (forwardCache.TryGetValue(key, out var value))
            {
                if (value is null || !value.HasValue)
                {
                    mods = null;
                    fields = null;
                    return false;
                }

                (mods, fields) = value.Value;

                return true;
            }

            bool sortMods = HasForwardOption(ForwardOptions._sortMods);
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
                        Global.Logger.WriteLog(LogLevel.Error, LogType.GeneralConfigFailure, "When indexed by field, array of mods must not include both include and excluded mods.", ClassLogCode);
                    }
                    else
                    {
                        if (hasExcludeMods || !values.SafeAny())
                        {
                            // Include all mods except excluded
                            foreach (var addMod in Global.Game.LoadOrder)
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
                     ? HasForwardOption(ForwardOptions._randomMod)
                         ? buildMods.OrderBy(_ => proKeys.GetRandom().Next())
                         : buildMods.OrderByDescending(Global.Game.LoadOrder.IndexOf) // Descending so same highest to lowest priority as manually entered mod list
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

        #endregion GetValues

        /// <inheritdoc />
        public override int GetHashCode ()
        {
            if (HashCode == 0)
            {
                var hash = new HashCode();
                hash.Add(base.GetHashCode());

                hash.Add(ForwardOptions);
                hash.Add(OnlyIfDefault);
                if (EditorID is not null)
                    hash.AddEnumerable(EditorID);
                if (FormID is not null)
                    hash.AddEnumerable(FormID);
                if (Match is not null)
                    hash.AddDictionary(Match);
                if (Fill is not null)
                    hash.AddDictionary(Fill);
                if (Forward is not null)
                    hash.AddDictionary(Forward);

                HashCode = hash.ToHashCode();
            }

            return HashCode;
        }

        public override string GetLogRuleID () => Group is null ? base.GetLogRuleID() : $"{Group.GetLogRuleID()}.{ConfigRule}";

        /// <summary>
        ///     Checks if rule has a specific Forward Option flag set.
        /// </summary>
        /// <param name="flag">Flag you want to check for.</param>
        /// <returns>True if flag is set on this rule.</returns>
        public bool HasForwardOption (ForwardOptions flag) => ForwardOptions.HasFlag(flag);

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

            if (FormID is not null && !MatchesHelper.Matches(proKeys.Record.FormKey, FormID, Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeFormID))
                return false;

            if (EditorID is not null && !MatchesHelper.Matches(proKeys.Record.EditorID, EditorID, Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeEditorID))
                return false;

            foreach (var x in Match)
            {
                if (!proKeys.SetProperty(x.Key, x.Key.Value, ClassLogCode))
                    return false;

                if (!proKeys.Property.Action.CanMatch())
                {
                    Global.Logger.WriteLog(LogLevel.Error, LogType.MatchFailure, $"Matched {x.Key}: No match enabled action for field.", ClassLogCode);
                    return false;
                }

                Global.Logger.LogAction($"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.MatchesRule)}", ClassLogCode);
                if (!proKeys.Property.Action.MatchesRule(proKeys))
                    return false;
            }

            Global.Logger.CurrentPropertyName = null; // Clear property name after loop just to be safe.

            return true;
        }

        public override int RunActions (ProcessingKeys proKeys)
        {
            // We want result to be 0 if no actions so it works with SingleMatch
            if (!HasActions)
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.SkippingRule, "Rule contains no actions.", ClassLogCode);
                return 0;
            }

            int changes = -1;

            foreach (var dci in DeepCopyIn)
            {
                int changed = processDeepCopyInAction(proKeys, dci);
                if (changed >= 0)
                    changes = (changes == -1) ? changed : changes + changed;
            }

            if (Merge.Count > 0)
            {
                int versions = Global.Game.State.LinkCache.ResolveAllSimpleContexts(proKeys.Record.FormKey, proKeys.Record.Registration.GetterType).Count();
                switch (versions)
                {
                    case < 2:
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.NoOverwrites, "Doesn't have any overwrites to merge with.", ClassLogCode);
                        break;

                    default:
                        foreach (var x in Merge)
                        {
                            if (!proKeys.SetProperty(x.Key, x.Key.Value, ClassLogCode))
                                continue;

                            int changed = processMerge(proKeys);

                            if (changed >= 0)
                                changes = (changes == -1) ? changed : changes + changed;
                        }

                        break;
                }
            }

            foreach (var x in Forward)
            {
                int changed = processForwardAction(proKeys, x.Key);

                if (changed >= 0)
                    changes = (changes == -1) ? changed : changes + changed;
            }

            foreach (var x in Fill)
            {
                if (!proKeys.SetProperty(x.Key, x.Key.Value, ClassLogCode))
                    continue;

                int changed = processFillAction(proKeys);

                if (changed >= 0)
                    changes = (changes == -1) ? changed : changes + changed;
            }

            Global.Logger.CurrentPropertyName = null; // Clear property name after loop just to be safe.

            return changes;
        }

        /// <inheritdoc />
        public override bool Validate ()
        {
            if (!base.Validate())
                return false;

            if (!Types.Any())
            {
                Global.Logger.WriteLog(LogLevel.Critical, LogType.GeneralConfigFailure, "Rules must specify record type(s) either directly in the rule or via type(s) added at a group level.", ClassLogCode, includePrefix: GetLogRuleID());
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Returns list of all valid record contexts for the given form key and mods. Does not
        ///     fully filter out <see cref="ForwardOptions.NonDefault" /> or
        ///     <see cref="ForwardOptions.NonNull" /> mods.
        /// </summary>
        /// <param name="proKeys">Used to get ForwardOptions</param>
        /// <param name="mods">List of valid mods to select from, in highest to lowest priority.</param>
        /// <param name="formKey">FormKey to get contexts for</param>
        /// <returns>
        ///     Ordered based on forward options list of valid records. (Randomized or ordered in
        ///     same order as <see cref="mods" />, which should already be in highest to lowest priority.
        /// </returns>
        protected IEnumerable<IModContext<IMajorRecordGetter>> getAvailableMods (ProcessingKeys proKeys, IEnumerable<ModKey> mods, FormKey formKey)
        {
            if (mods is null || !mods.Any())
                return [];

            bool nonDefault = HasForwardOption(ForwardOptions._nonDefaultMod);
            bool randomize = HasForwardOption(ForwardOptions._randomMod) && !HasForwardOption(ForwardOptions._sortMods);

            var AllRecordMods = nonDefault
                ? Global.Game.State.LinkCache.ResolveAllSimpleContexts(formKey, proKeys.Record.Registration.GetterType).Where(m => !m.ModKey.Equals(formKey.ModKey) && mods.Contains(m.ModKey))
                : Global.Game.State.LinkCache.ResolveAllSimpleContexts(formKey, proKeys.Record.Registration.GetterType).Where(m => mods.Contains(m.ModKey));

            if (AllRecordMods.Count() > 1)
            { // Sort if more than 1 record
                if (randomize)
                {
                    var r = proKeys.GetRandom();
                    AllRecordMods = AllRecordMods.OrderBy(_ => r.Next());
                }
                else
                {
                    AllRecordMods = AllRecordMods.OrderBy(m => mods.IndexOf(m.ModKey));
                }
            }

            return AllRecordMods;
        }

        /// <summary>
        ///     Process a Deep Copy In action against current record
        /// </summary>
        /// <param name="proKeys">
        ///     Current processing keys. <see cref="ProcessingKeys.Record" /> is the record to run
        ///     action over.
        /// </param>
        /// <param name="action">Copy action to process.</param>
        /// <returns>
        ///     Number of updates made to current record. -1 if record didn't meet requirements for
        ///     this rule
        /// </returns>
        private int processDeepCopyInAction (ProcessingKeys proKeys, GSPDeepCopyIn action)
        {
            var from = action.FromID == FormKey.Null ? proKeys.Record.FormKey : action.FromID;
            bool fromSameID = from == proKeys.Record.FormKey;

            // If not coping over from another record then skip if already master record
            if (fromSameID && proKeys.Context.IsMaster())
                return -1;

            if (!proKeys.Record.Registration.TryGetTranslationMaskType(out var maskType))
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, "No valid mask type found for DeepCopyIn action.", ClassLogCode);
                return -1;
            }

            var mask = action.GetMask(maskType);
            if (mask is null)
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, "No valid mask found for DeepCopyIn action.", ClassLogCode);
                return -1;
            }

            if (OnlyIfDefault)
            {
                if (!proKeys.Record.Equals(proKeys.GetOriginRecord(), mask))
                {
                    Global.Logger.WriteLog(LogLevel.Trace, LogType.OriginNotMatch, "DeepCopyIn OnlyIfDefault: Did not match - skipping", ClassLogCode);
                    return -1;
                }

                Global.Logger.WriteLog(LogLevel.Trace, LogType.OriginMatch, "DeepCopyIn OnlyIfDefault: Matched", ClassLogCode);
            }

            var AllRecordMods =
                fromSameID || action.FromMod.SafeAny()
                ? getAvailableMods(proKeys, action.FromMod, from)
                : [Global.Game.State.LinkCache.ResolveSimpleContext(from, proKeys.Record.Registration.GetterType, ResolveTarget.Winner)];

            var fromContext = AllRecordMods.FirstOrDefault();
            if (fromContext is null)
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessSkipped, "No valid record found for DeepCopyIn action.", ClassLogCode);
                return -1;
            }

            if (proKeys.Record.Equals(fromContext.Record, mask))
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.NoUpdateAlreadyMatches, LogWriter.PropertyIsEqual, ClassLogCode);
                return 0;
            }

            if (proKeys.GetPatchRecord() is not IMajorRecordInternal patchRecord)
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, $"No changes to {proKeys.Property.PropertyName} invalid record type for DeepCopyIn", ClassLogCode);
                return 0;
            }

            Global.Logger.LogAction("Calling DeepCopyIn to update property.", ClassLogCode);

            patchRecord.DeepCopyIn(fromContext.Record, out var errorMask, mask);
            if (errorMask.IsInError())
            {
                Global.Logger.WriteLog(LogLevel.Critical, LogType.RecordUpdateFailure, $"DeepCopyIn returned with errors.{Environment.NewLine}{errorMask}", ClassLogCode);
                return 0;
            }

            foreach (string fieldName in mask.GetEnabled())
                Program.RecordUpdates.Add((proKeys.Record.Registration, proKeys.Record.FormKey, this, fieldName, 1));

            if (Global.Logger.CurrentLogLevel <= LogLevel.Debug)
            {
                string fromStr = fromSameID ? fromContext.ModKey.FileName
                    : fromContext.IsMaster() ? from.ToString() : $"{from} in {fromContext.ModKey}";

                Global.Logger.WriteLog(LogLevel.Debug, LogType.RecordUpdated, $"{LogWriter.RecordUpdated} - DeepCopyIn performed from {fromStr}", ClassLogCode);
            }

            return 1;
        }

        /// <summary>
        ///     Process a single fill action against current record. Fill action to run is stored in <see cref="ProcessingKeys.RuleKey" />
        /// </summary>
        /// <param name="proKeys">Current processing keys</param>
        /// <param name="ruleKey">Current key of current rule</param>
        /// <returns>
        ///     Number of updates made to current record. -1 if record didn't meet requirements for
        ///     this rule (CheckOnlyIfDefault)
        /// </returns>
        private int processFillAction (ProcessingKeys proKeys)
        {
            if (!proKeys.Property.Action.CanFill())
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.MatchFailure, $"Matched {proKeys.RuleKey.Value}: No fill enabled action for field.", ClassLogCode);
                return -1;
            }

            if (proKeys.CheckOnlyIfDefault())
                return -1;

            Global.Logger.LogAction($"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.Fill)}", ClassLogCode);
            int changed = proKeys.Property.Action.Fill(proKeys);

            if (changed > 0) // TODO: Change this so that not using static field in Program class - This way for now as this code use to be in that class
                Program.RecordUpdates.Add((proKeys.Type, proKeys.Record.FormKey, this, proKeys.Property.PropertyName, changed));

            return changed;
        }

        /// <summary>
        ///     Process a Forward rule against current record. Unlike Fill
        ///     <see cref="ProcessingKeys.SetProperty(FilterOperation, string, int, int)" /> has not
        ///     been called yet as Forward could be keyed by property or mod. So this method must
        ///     call it prior to forwarding each property.
        /// </summary>
        /// <param name="proKeys">Current processing keys</param>
        /// <param name="ruleKey">Current key of current rule</param>
        /// <returns>
        ///     Number of updates made to current record. -1 if record didn't meet requirements for
        ///     this rule (CheckOnlyIfDefault)
        /// </returns>
        private int processForwardAction (ProcessingKeys proKeys, FilterOperation ruleKey)
        {
            // Don't waste time if record is master with no overwrites
            if (proKeys.Context.IsMaster())
                return -1;

            if (!tryGetForward(proKeys, ruleKey, out var mods, out string[]? fields))
                return -1;

            var AllRecordMods = getAvailableMods(proKeys, mods, proKeys.Record.FormKey);
            if (!AllRecordMods.Any())
                return -1;

            bool nonDefault = HasForwardOption(ForwardOptions._nonDefaultMod);
            bool nonNull = HasForwardOption(ForwardOptions._nonNullMod);
            bool selfMasterOnly = HasForwardOption(ForwardOptions.SelfMasterOnly);

            int changed = 0;
            foreach (string field in fields)
            {
                if (!proKeys.SetProperty(ruleKey, field, ClassLogCode))
                    continue;

                if (!proKeys.Property.Action.CanForward() || (selfMasterOnly && !proKeys.Property.Action.CanForwardSelfOnly()))
                {
                    Global.Logger.WriteLog(LogLevel.Error, LogType.MatchFailure, $"Matched {field}: No forward enabled action for field.", ClassLogCode);
                    continue;
                }

                if (HasForwardOption(ForwardOptions._hpu))
                {
                    var graph = ForwardRecordGraph.Create(proKeys);
                    var endNodes = graph?.GetEndNodes(mods);
                    if (endNodes is null)
                        continue;

                    Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"End nodes: {string.Join(',', endNodes)}", ClassLogCode);

                    var mc = proKeys.Property.Action.FindHPUIndex(proKeys, AllRecordMods, endNodes);
                    if (mc is not null)
                    {
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Forwarding Type: {nameof(ForwardOptions.HPU)}. From Mod: {mc.ModKey.FileName}.", ClassLogCode);
                        Global.Logger.LogAction($"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.Forward)}", ClassLogCode);
                        changed += proKeys.Property.Action.Forward(proKeys, mc);
                    }
                    else
                    {
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessSkipped, $"Forwarding Type: {nameof(ForwardOptions.HPU)}. Skipping as no valid mod found.", ClassLogCode);
                    }
                }
                else if (selfMasterOnly)
                {
                    changed = processForwardSelfMasterOnly(proKeys, mods, AllRecordMods);
                }
                else
                {   // Default Forward Type
                    if (proKeys.CheckOnlyIfDefault())
                        continue;

                    // Only forward single record but can loop until valid record found if
                    // nonDefault and or nonNull used
                    foreach (var modContext in AllRecordMods)
                    {
                        if (nonNull && proKeys.Property.Action.IsNullOrEmpty(proKeys, modContext))
                        {
                            Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessSkipped, $"Forwarding Type: {Enum.GetName(ForwardOptions)}. Skipping from mod {modContext.ModKey.FileName} as has null/empty value and NonNull option used.", ClassLogCode);
                            continue;
                        }

                        if (nonDefault && proKeys.Property.Action.MatchesOrigin(proKeys, modContext))
                        {
                            Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessSkipped, $"Forwarding Type: {Enum.GetName(ForwardOptions)}. Skipping from mod {modContext.ModKey.FileName} as matches origin and NonDefault option used.", ClassLogCode);
                            continue;
                        }

                        Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Forwarding Type: {Enum.GetName(ForwardOptions)} From Mod: {modContext.ModKey.FileName}.", ClassLogCode);
                        Global.Logger.LogAction($"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.Forward)}", ClassLogCode);
                        int changes = proKeys.Property.Action.Forward(proKeys, modContext);
                        if (changes > 0)
                            changed += changes;

                        // If we got to here then we have found a valid mod to forward from so can
                        // stop processing
                        break;
                    }
                }

                if (changed > 0) // TODO: Change this so that not using static field in Program class - This way for now as this code use to be in that class
                    Program.RecordUpdates.Add((proKeys.Type, proKeys.Record.FormKey, this, proKeys.Property.PropertyName, changed));
            }

            return changed;
        }

        /// <summary>
        ///     Process a Forward of type SelfMasterOnly rule against current record
        /// </summary>
        /// <param name="proKeys">Current processing keys</param>
        /// <param name="ruleKey">Current key of current rule</param>
        /// <returns>
        ///     Number of updates made to current record. -1 if record didn't meet requirements for
        ///     this rule (CheckOnlyIfDefault)
        /// </returns>
        private int processForwardSelfMasterOnly (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods)
        {
            bool firstMod = true;
            int changed = 0;

            if (mods.First() != AllRecordMods.First().ModKey)
            {   // If first mod is not the same as the first mod in AllRecordMods then we don't forward
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessSkipped, $"Forwarding Type: {nameof(ForwardOptions.DefaultThenSelfMasterOnly)}. Skipping as doesn't contain record in {mods.First()}.", ClassLogCode);
                return 0;
            }

            foreach (var mod in AllRecordMods)
            {
                if (firstMod && proKeys.CheckOnlyIfDefault())
                    return 0;

                if (firstMod && ForwardOptions.HasFlag(ForwardOptions.DefaultThenSelfMasterOnly))
                {  // First mod of DefaultThenSelfMasterOnly
                    if (proKeys.CheckOnlyIfDefault())
                        break;

                    Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Forwarding Type: {nameof(ForwardOptions.DefaultThenSelfMasterOnly)} From: {mod.ModKey.FileName}.", ClassLogCode);

                    int changes = proKeys.Property.Action.Forward(proKeys, mod);
                    if (changes < 0)
                    {   // If default forward fails we do not continue with the SelfMasterOnly forwards
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessSkipped, $"Forwarding Type: {nameof(ForwardOptions.DefaultThenSelfMasterOnly)}. Skipping as default forward from {mods.First()} failed.", ClassLogCode);
                        return 0;
                    }

                    changed += changes;
                }
                else
                {   //  SelfMasterOnly
                    Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Forwarding Type: {nameof(ForwardOptions.DefaultThenSelfMasterOnly)} From: {mod.ModKey.FileName}.", ClassLogCode);

                    int changes = proKeys.Property.Action.ForwardSelfOnly(proKeys, mod);
                    if (changes > 0)
                        changed += changes;
                }

                firstMod = false;
            }

            return changed;
        }

        /// <summary>
        ///     Process a Merge rule against current record ( <see cref="ProcessingKeys.Record" />)
        ///     and property ( <see cref="ProcessingKeys.Property" />).
        /// </summary>
        /// <param name="proKeys">Current processing keys</param>
        /// <param name="ruleKey">Current key of current rule</param>
        /// <returns>
        ///     Number of updates made to current record. -1 if record didn't meet requirements for
        ///     this rule/property. (CheckOnlyIfDefault)
        /// </returns>
        private int processMerge (ProcessingKeys proKeys)
        {
            if (!proKeys.Property.Action.CanMerge())
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordActionInvalid, "No merge action found", ClassLogCode);
                return -1;
            }

            if (proKeys.CheckOnlyIfDefault())
                return -1;

            Global.Logger.LogAction($"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.Merge)}", ClassLogCode);
            int changed = proKeys.Property.Action.Merge(proKeys);

            if (changed > 0) // TODO: Change this so that not using static field in Program class - This way for now as this code use to be in that class
                Program.RecordUpdates.Add((proKeys.Type, proKeys.Record.FormKey, this, proKeys.Property.PropertyName, changed));

            return changed;
        }
    }
}