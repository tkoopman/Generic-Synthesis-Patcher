using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    [JsonConverter(typeof(GSPBaseConverter))]
    public class GSPGroup : GSPBase
    {
        private const int ClassLogCode = 0x05;

        /// <summary>
        /// Rules contained in this group.
        /// NOTE: Rule Priority is ignored when in a group.
        ///       Only the Group's Priority is used.
        /// </summary>
        [JsonProperty(PropertyName = "Rules", Required = Required.Always)]
        [JsonConverter(typeof(SingleOrArrayConverter<GSPRule>))]
        public List<GSPRule> Rules { get; set; } = [];

        /// <summary>
        /// If true processing will stop after finding a single matched rule for a record.
        /// </summary>
        [JsonProperty(PropertyName = "SingleMatch")]
        public bool SingleMatch { get; set; } = false;

        public override bool Validate ()
        {
            if (!base.Validate())
                return false;

            var AllTypes = RecordTypes.NONE;

            int ruleCount = 1;
            foreach (var rule in Rules)
            {
                rule.ConfigFile = ConfigFile;
                rule.ConfigRule = ruleCount++;

                if (!rule.ClaimAndValidate(this))
                    return false;

                // Claiming rule will also Rule type if current None to either match Group Types or All if group types is None
                // So AllTypes will be All if a single rule and group were None
                // So can overwrite Group Types once all rules claimed safely.
                AllTypes |= rule.Types;
            }

            // Output message if groups types defined and all rule types defined but combined to less than current group types.
            if (Types != RecordTypes.NONE && Types != AllTypes)
                LogHelper.WriteLog(LogLevel.Information, ClassLogCode, $"Reducing group's Types to {AllTypes} from {Types} as extra types not used.", rule: this);

            Types = AllTypes;

            return true;
        }
    }
}