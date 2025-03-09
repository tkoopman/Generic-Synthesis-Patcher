using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace GenericSynthesisPatcher
{
    public class GSPSettings
    {
        [Tooltip("Add any other Dynamic Patch Mods like SynthEBD to this list if they exist prior to this mod in load order. Causes them to be treated slightly differently in merges and similar actions.")]
        public HashSet<ModKey> DynamicMods { get; set; } = [];

        [Tooltip("JSON config files location. {SkyrimData} or {SynthesisData} valid dynamic values to start folder with. {SynthesisData} can be used alone, but for {SkyrimData} you should add a sub-folder.")]
        public string Folder { get; set; } = "{SkyrimData}\\GSP";

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
            [MaintainOrder(100)]
            public bool Cache { get; set; } = true;

            [MaintainOrder(100)]
            public bool CallingAction { get; set; } = true;

            [MaintainOrder(4)]
            public bool EditorIDMatchFailed { get; set; } = true;

            [MaintainOrder(4)]
            public bool EditorIDMatchSuccessful { get; set; } = true;

            [MaintainOrder(3)]
            public bool FormIDMatchFailed { get; set; } = true;

            [MaintainOrder(3)]
            public bool FormIDMatchSuccessful { get; set; } = true;

            [MaintainOrder(100)]
            public bool GroupMatched { get; set; } = true;

            [MaintainOrder(2)]
            public bool MastersMatchFailed { get; set; } = true;

            [MaintainOrder(2)]
            public bool MastersMatchSuccessful { get; set; } = true;

            [MaintainOrder(100)]
            public bool MergeNoOverwrites { get; set; } = true;

            [MaintainOrder(5)]
            public bool RegexMatchFailed { get; set; } = true;

            [MaintainOrder(5)]
            public bool RegexMatchSuccessful { get; set; } = true;

            [MaintainOrder(1)]
            public bool TypeMatchFailed { get; set; } = true;

            [MaintainOrder(1)]
            public bool TypeMatchSuccessful { get; set; } = true;
        }
    }
}