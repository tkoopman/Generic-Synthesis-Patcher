using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    [JsonConverter(typeof(GSPBaseConverter))]
    public class GSPGroup : GSPBase
    {
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
            var AllTypes = RecordTypes.NONE;

            foreach (var rule in Rules)
            {
                if (!rule.ClaimAndValidate(this))
                    return false;

                AllTypes |= rule.Types;
            }

            if (Types != RecordTypes.NONE && Types != AllTypes)
                LogHelper.Log(LogLevel.Information, $"Reducing group's Types to {AllTypes} from {Types} as extra types not used.", 0xFFD);
            Types = AllTypes;

            return true;
        }
    }
}