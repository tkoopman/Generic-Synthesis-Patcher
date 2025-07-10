using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace GenericSynthesisPatcher
{
    public class GSPSettings
    {
        [Tooltip("Add any other Dynamic Patch Mods like SynthEBD to this list if they exist prior to this mod in load order. Causes them to be treated slightly differently in merges and similar actions.")]
        public HashSet<ModKey> DynamicMods { get; set; } = [];

        [Tooltip("JSON config files location. {GameData} or {SynthesisData} valid dynamic values to start folder with. {SynthesisData} can be used alone, but for {GameData} you should add a sub-folder.")]
        public string Folder { get; set; } = "{GameData}\\GSP";

        [Tooltip("Logging Options")]
        public GSPLogging Logging { get; set; } = new();

        public class GSPLogging
        {
            [Tooltip("WARNING: This will generate a lot of logs.\nRecommend using either FormKey above or Logging option on a rule instead of this.")]
            [SettingName("Debug / Trace All !WARNING!")]
            [MaintainOrder(3)]
            public bool All { get; set; }

            [Tooltip("Use in conjunction with Log Level of Debug or Trace. Must include any leading 0s.")]
            [SettingName("Enable Debug & Trace on Form Key")]
            [MaintainOrder(2)]
            public FormKey FormKey { get; set; }

            [Tooltip("Controls the amount of logs generated. Information is recommended for normal operations.\nDebug and Trace will only output extra logs that either match Debug / Trace Form Key or for Rules with Debug option set true.")]
            [MaintainOrder(1)]
            public LogLevel LogLevel { get; set; } = LogLevel.Information;

            [Tooltip("Disable noisy logs you not interested in.")]
            [SettingName("Noisy Debug & Trace Logs")]
            [MaintainOrder(3)]
            public GSPLogNoise NoisyLogs { get; set; } = new();
        }

        public class GSPLogNoise
        {
            [MaintainOrder(2)]
            public ActionLogNoise ActionLogs { get; set; } = new();

            [MaintainOrder(100)]
            public bool Cache { get; set; } = true;

            [MaintainOrder(1)]
            public MatchLogNoise MatchLogs { get; set; } = new();

            public class ActionLogNoise
            {
                [Tooltip("Include logging when an action is called.")]
                [MaintainOrder(1)]
                public bool ActionCalled { get; set; } = false;

                [Tooltip("Include all other logging when an action is processing.")]
                [MaintainOrder(4)]
                public bool ActionOther { get; set; } = false;

                [Tooltip("Include logging when an action is skipped as nothing to do on the current record (ie already match value or no record found to forward from).")]
                [MaintainOrder(2)]
                public bool ActionSkipped { get; set; } = false;

                [Tooltip("Include logging when an action made a change to a record.")]
                [MaintainOrder(3)]
                public bool RecordUpdated { get; set; } = true;
            }

            public class MatchLogNoise
            {
                [Tooltip("Include logging matches against EditorID if logging matched or not matched.")]
                [MaintainOrder(6)]
                public bool IncludeEditorID { get; set; } = false;

                [Tooltip("Include logging matches against FormID if logging matched or not matched.")]
                [MaintainOrder(5)]
                public bool IncludeFormID { get; set; } = false;

                [Tooltip("Include logging matches against a group.")]
                [MaintainOrder(4)]
                public bool IncludeGroup { get; set; } = false;

                [Tooltip("Include logging matches against Masters if logging matched or not matched.")]
                [MaintainOrder(7)]
                public bool IncludeMasters { get; set; } = false;

                [Tooltip("Include logging OnlyIfDefault option results.")]
                [MaintainOrder(8)]
                public bool IncludeOnlyIfDefault { get; set; } = false;

                [Tooltip("Include logging PatchedBy option results.")]
                [MaintainOrder(8)]
                public bool IncludePatchedBy { get; set; } = false;

                [Tooltip("Include logging matches against Regex values if logging matched or not matched.")]
                [MaintainOrder(9)]
                public bool IncludeRegex { get; set; } = false;

                [Tooltip("Include logging matches against record type if logging matched or not matched.")]
                [MaintainOrder(3)]
                public bool IncludeType { get; set; } = false;

                [Tooltip("Log when a match is found for a rule.")]
                [MaintainOrder(1)]
                public bool Matched { get; set; } = true;

                [Tooltip("Log when a match is not found for a rule.")]
                [MaintainOrder(2)]
                public bool NotMatched { get; set; } = true;
            }
        }
    }
}