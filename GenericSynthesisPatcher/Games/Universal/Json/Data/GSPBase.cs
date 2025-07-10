using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Common;

using DynamicData;

using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Games.Universal.Json.Operations;
using GenericSynthesisPatcher.Helpers;

using Loqui;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Json.Data
{
    [JsonConverter(typeof(GSPBaseConverter))]
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "Just used for sorting")]
    public abstract class GSPBase : IComparable<GSPBase>
    {
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
        [JsonConverter(typeof(SingleOrArrayConverter<ILoquiRegistration>))]
        public IEnumerable<ILoquiRegistration> Types { get; internal set; } = [];

        #region Masters

        private List<ModKeyListOperation>? masters;

        /// <summary>
        ///     List of masters that should be either included or excluded from matching
        /// </summary>
        [JsonProperty(PropertyName = "Masters")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? Masters
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
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? MastersDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                masters = [];
                foreach (var v in value ?? [])
                    masters.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!Masters")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? MastersNot { set => MastersDel = value; }

        #endregion Masters

        #region PatchedBy

        private List<ModKeyListOperation>? patchedBy;

        private FilterLogic patchedByLogic = FilterLogic.Default;

        /// <summary>
        ///     List of mods that if this record was patched by will either include or exclude it
        ///     from matching.
        ///     Note: Record will never matched PatchedBy of it's master mod. Use Masters for that.
        /// </summary>
        [JsonProperty(PropertyName = "PatchedBy")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedBy
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
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedByAnd
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
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedByDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.Default;
                patchedBy = [];
                foreach (var v in value ?? [])
                    patchedBy.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!PatchedBy")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedByNot { set => PatchedByDel = value; }

        [JsonProperty(PropertyName = "^PatchedBy")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedXOR
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

        public virtual string GetLogRuleID () => $"{ConfigFile}.{ConfigRule}";

        /// <summary>
        ///     Checks if current context matches filters. When overridden should always check base
        ///     matches first, and only do extra checks if base returned true, for best performance.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns true if context matches filters.</returns>
        public virtual bool Matches (ProcessingKeys proKeys)
        {
            if (!Types.Contains(proKeys.Type))
            {
                if (Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeType && Global.Settings.Logging.NoisyLogs.MatchLogs.NotMatched)
                    Global.Logger.WriteLog(LogLevel.Trace, LogType.MatchFailure, "Record Type: Matched: False", ClassLogCode);
                return false;
            }

            if (Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeType && Global.Settings.Logging.NoisyLogs.MatchLogs.Matched)
                Global.Logger.WriteLog(LogLevel.Trace, LogType.MatchSuccess, "Record Type: Matched: True", ClassLogCode);

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