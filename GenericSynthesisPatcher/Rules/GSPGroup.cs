using Common.JsonConverters;

using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Helpers;

using Loqui;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Rules
{
    [JsonConverter(typeof(GSPBaseConverter))]
    public class GSPGroup : GSPBase
    {
        private const int ClassLogCode = 0x1B;

        /// <summary>
        ///     Rules contained in this group.
        ///     NOTE: Rule Priority is ignored when in a group. Only the Group's Priority is used.
        /// </summary>
        [JsonProperty(PropertyName = "Rules", Required = Required.Always)]
        [JsonConverter(typeof(ListConverter<GSPRule>))]
        public List<GSPRule> Rules { get; set; } = [];

        /// <summary>
        ///     If true processing will stop after finding a single matched rule for a record.
        /// </summary>
        [JsonProperty(PropertyName = "SingleMatch")]
        public bool SingleMatch { get; set; } = false;

        /// <summary>
        ///     Will check all rules in group for matches and run actions on matching rules.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        ///     proKeys parameter is not pointing to this rule.
        /// </exception>
        public override int RunActions (ProcessingKeys proKeys)
        {
            if (Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeGroup)
                Global.Logger.WriteLog(LogLevel.Trace, LogType.MatchSuccess, "Matched group. Processing Rules.", ClassLogCode);

            var gProKeys = new ProcessingKeys(proKeys.Context, proKeys);
            int ruleCount = 0;
            int changesTotal = -1;
            foreach (var groupRule in Rules)
            {
                _ = gProKeys.SetRule(groupRule);
                Global.Logger.UpdateCurrentProcess(groupRule, proKeys.Context, ClassLogCode);

                ruleCount++;
                if (groupRule.Matches(gProKeys))
                {
                    int changed = groupRule.RunActions(gProKeys);
                    if (changed >= 0) // -1 would mean failed OnlyIfDefault check
                    {
                        changesTotal = (changesTotal == -1) ? changed : changesTotal + changed;

                        if (SingleMatch)
                        {
                            if (ruleCount != Rules.Count)
                                Global.Logger.WriteLog(LogLevel.Trace, LogType.SkippingRule, $"Skipping remaining rules in group due to SingleMatch. Checked {ruleCount}/{Rules.Count}", ClassLogCode);

                            break;
                        }
                    }
                }
            }

            return changesTotal;
        }

        public override bool Validate ()
        {
            if (!base.Validate())
                return false;

            HashSet<ILoquiRegistration> AllTypes = [];

            int ruleCount = 1;
            foreach (var rule in Rules)
            {
                rule.ConfigFile = ConfigFile;
                rule.ConfigRule = ruleCount++;

                if (!rule.ClaimAndValidate(this))
                    return false;

                // Claiming rule will also Rule type if current None to either match Group Types or
                // All if group types is None So AllTypes will be All if a single rule and group
                // were None So can overwrite Group Types once all rules claimed safely.
                AllTypes.Add(rule.Types);
            }

            // Output message if groups types defined and all rule types defined but combined to
            // less than current group types.
            if (Types.Count != 0 && AllTypes.Count < Types.Count)
                Global.Logger.WriteLog(LogLevel.Information, LogType.GeneralConfig, $"Reducing group's Types to {AllTypes.Count} from {Types.Count} as extra types not used.", ClassLogCode, includePrefix: GetLogRuleID());

            Types = [.. AllTypes];

            return true;
        }
    }
}