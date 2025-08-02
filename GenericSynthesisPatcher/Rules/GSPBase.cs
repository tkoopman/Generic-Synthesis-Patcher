using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Common;
using Common.JsonConverters;

using DynamicData;

using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Rules.Operations;

using Loqui;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Rules
{
    [JsonConverter(typeof(GSPBaseConverter))]
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "Just used for sorting")]
    public abstract class GSPBase : IComparable<GSPBase>
    {
        internal bool _foundViaIndex;
        private const int ClassLogCode = 0x1A;

        /// <summary>
        ///     Unique ID for the file this rule exists in.
        /// </summary>
        [JsonIgnore]
        public int ConfigFile { get; internal set; }

        /// <summary>
        ///     Unique ID for the rule in the file this rule exists in.
        /// </summary>
        [JsonIgnore]
        public int ConfigRule { get; internal set; }

        /// <summary>
        ///     Set to true to enable Debug/Trace logging for this rule
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty(PropertyName = "Debug")]
        public bool Debug { get; set; }

        public bool FullyIndexed { get; protected set; }

        /// <summary>
        ///     If set will check if record has already been patched by another GSP rule or not. If
        ///     you don't care exclude or set to null
        /// </summary>
        [JsonProperty(PropertyName = "Patched")]
        public bool? Patched { get; set; }

        /// <summary> Sets order of processing rules. Rules with matching priority can be executed
        /// in any order. Processed lowest to highest, so if rules with priority of 1 & 100 both
        /// touch the same field, while both still run the 100 priority will run last so may
        /// overwrite changes the other made. </summary>
        [DefaultValue(0)]
        [JsonProperty(PropertyName = "Priority")]
        public int Priority { get; set; }

        /// <summary>
        ///     List of record types this rule should match
        /// </summary>
        [JsonProperty(PropertyName = "Types")]
        [JsonConverter(typeof(ListConverter<ILoquiRegistration>))]
        public HashSet<ILoquiRegistration> Types { get; internal set; } = [];

        protected bool FoundViaIndex => _foundViaIndex;

        /// <summary>
        ///     Should only be used for sorting by Priority
        /// </summary>
        public int CompareTo (GSPBase? other)
            => other is null ? 1
             : other == this ? 0
             : Priority.CompareTo(other.Priority);

        // <inheritdoc />
        public override int GetHashCode ()
        {
            var hash = new HashCode();

            hash.Add(Priority);
            hash.Add(Types);
            if (Masters is not null)
                hash.AddEnumerable(Masters);

            return hash.ToHashCode();
        }

        public virtual bool GetIndexableData (out List<RecordID> include, out List<RecordID> exclude)
        {
            include = [];
            exclude = [];
            FullyIndexed = PatchedBy is null || PatchedBy.Count == 0;

            if (Masters is not null && Masters.Count != 0)
            {
                foreach (var m in Masters)
                {
                    var recordID = new RecordID(m.Value, RecordID.EqualsOptions.ModKey);
                    switch (m.Operation)
                    {
                        case ListLogic.NOT:
                            exclude.Add(recordID);
                            break;

                        case ListLogic.ADD:
                            include.Add(recordID);
                            break;

                        default:
                            FullyIndexed = false;
                            break;
                    }
                }
            }

            return include.Count != 0 || exclude.Count != 0;
        }

        #region Masters

        private HashSet<ModKeyListOperation>? masters;

        /// <summary>
        ///     List of masters that should be either included or excluded from matching
        /// </summary>
        [JsonProperty(PropertyName = "Masters")]
        [JsonConverter(typeof(ListConverter<ModKeyListOperation>))]
        public HashSet<ModKeyListOperation>? Masters
        {
            get => masters;
            set
            {
                if (!value.SafeAny())
                    return;

                masters = value;
            }
        }

        [JsonProperty(PropertyName = "-Masters")]
        [JsonConverter(typeof(ListConverter<ModKeyListOperation>))]
        public HashSet<ModKeyListOperation>? MastersDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                masters = [];
                foreach (var v in value ?? [])
                    _ = masters.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!Masters")]
        [JsonConverter(typeof(ListConverter<ModKeyListOperation>))]
        public HashSet<ModKeyListOperation>? MastersNot { set => MastersDel = value; }

        #endregion Masters

        #region PatchedBy

        private HashSet<ModKeyListOperation>? patchedBy;

        private FilterLogic patchedByLogic = FilterLogic.Default;

        /// <summary>
        ///     List of mods that if this record was patched by will either include or exclude it
        ///     from matching.
        ///     Note: Record will never matched PatchedBy of it's master mod. Use Masters for that.
        /// </summary>
        [JsonProperty(PropertyName = "PatchedBy")]
        [JsonConverter(typeof(ListConverter<ModKeyListOperation>))]
        public HashSet<ModKeyListOperation>? PatchedBy
        {
            get => patchedBy;
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.Default;
                patchedBy = value;
            }
        }

        [JsonProperty(PropertyName = "&PatchedBy")]
        [JsonConverter(typeof(ListConverter<ModKeyListOperation>))]
        public HashSet<ModKeyListOperation>? PatchedByAnd
        {
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.AND;
                patchedBy ??= [];
                patchedBy.Add(value);
            }
        }

        [JsonProperty(PropertyName = "-PatchedBy")]
        [JsonConverter(typeof(ListConverter<ModKeyListOperation>))]
        public HashSet<ModKeyListOperation>? PatchedByDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.Default;
                patchedBy = [];
                foreach (var v in value ?? [])
                    _ = patchedBy.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!PatchedBy")]
        [JsonConverter(typeof(ListConverter<ModKeyListOperation>))]
        public HashSet<ModKeyListOperation>? PatchedByNot { set => PatchedByDel = value; }

        [JsonProperty(PropertyName = "^PatchedBy")]
        [JsonConverter(typeof(ListConverter<ModKeyListOperation>))]
        public HashSet<ModKeyListOperation>? PatchedXOR
        {
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.XOR;
                patchedBy = value;
            }
        }

        #endregion PatchedBy

        public virtual string GetLogRuleID () => $"{ConfigFile}.{ConfigRule}";

        /// <summary>
        ///     Checks if current context matches filters. When overridden should always check base
        ///     matches first, and only do extra checks if base returned true, for best performance.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns true if context matches filters.</returns>
        public virtual bool Matches (ProcessingKeys proKeys)
        {
            if (this is GSPRule rule && rule.Group is not null)
            {
                // No longer check type on main rules as pre-filtered. However still need to check
                // on rules that belong to a group.
                if (!Types.Contains(proKeys.Type))
                {
                    if (Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeType && Global.Settings.Logging.NoisyLogs.MatchLogs.NotMatched)
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.MatchFailure, "Record Type: Matched: False", ClassLogCode);
                    return false;
                }

                if (Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeType && Global.Settings.Logging.NoisyLogs.MatchLogs.Matched)
                    Global.Logger.WriteLog(LogLevel.Trace, LogType.MatchSuccess, "Record Type: Matched: True", ClassLogCode);
            }

            if (Masters is not null && !MatchesHelper.Matches(proKeys.Record.FormKey.ModKey, Masters, Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeMasters))
                return false;

            if (PatchedBy is not null)
            {
                var all = Global.Game.State.LinkCache.ResolveAllSimpleContexts(proKeys.Record.FormKey, proKeys.Record.Registration.GetterType).Select(m => m.ModKey);
                if (!MatchesHelper.Matches(all, patchedByLogic, PatchedBy, Global.Settings.Logging.NoisyLogs.MatchLogs.IncludePatchedBy))
                    return false;
            }

            if (Patched.HasValue)
            {
                bool result = Patched.Value ? proKeys.HasPatchRecord : !proKeys.HasPatchRecord;
                Global.Logger.WriteLog(LogLevel.Trace, result ? LogType.MatchSuccess : LogType.MatchFailure, $"Patched: Matched: {result} Has patch record: {proKeys.HasPatchRecord}", ClassLogCode);
                return result;
            }

            return true;
        }

        /// <summary>
        ///     Run the actions for this rule.
        /// </summary>
        /// <param name="proKeys">
        ///     Keys of record to run actions against. <see cref="ProcessingKeys.Context" /> will
        ///     already be set to the context of the record to process actions against.
        ///     <see cref="ProcessingKeys.RuleBase" /> will already be set to this instance.
        ///
        ///     You should call
        ///     <see cref="ProcessingKeys.SetProperty(FilterOperation, string, int, int)" /> before
        ///     processing each property of an action (if appropriate)
        /// </param>
        /// <returns>
        ///     Number of changes made by the actions to this record. Where possible this should be
        ///     as detailed of a count as possible. So if multiple simple fields are changed then
        ///     could be counted per field, but if a list was edited then each add / remove should
        ///     be counted as a single change.
        ///
        ///     Return -1 if all actions were skipped due to OnlyIfDefault check failing. Otherwise
        ///     should return 0 if no changes made, or count of changes made.
        /// </returns>
        public abstract int RunActions (ProcessingKeys proKeys);

        /// <summary>
        ///     Checks for anything that may make this rule invalid and possibly cause issues. Any
        ///     rule failing validation will mean the patcher will never start any patching, and
        ///     just end with an error.
        /// </summary>
        /// <returns>True if rule passes validation</returns>
        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public virtual bool Validate ()
        {
            if (Debug) // Not using Global.DebugLogger as needs to output even if debugging only enabled for certain rules / records.
                Global.Logger.WriteLog(LogLevel.Debug, LogType.GeneralConfig, "Debug / Trace logging enabled for this rule.", ClassLogCode, includePrefix: GetLogRuleID());

            if (Masters.SafeAny() && !MatchesHelper.Validate(Masters))
                return false;

            if (patchedByLogic != FilterLogic.AND && PatchedBy.SafeAny() && !MatchesHelper.Validate(patchedByLogic, PatchedBy))
                return false;

            return true;
        }
    }
}