using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace GenericSynthesisPatcher
{
    public class GSPSettings
    {
        [Tooltip("JSON config files location. {SkyrimData} or {SynthesisData} valid dynamic values to start folder with. {SynthesisData} can be used alone, but for {SkyrimData} you should add a subfolder.")]
        public string Folder { get; set; } = "{SkyrimData}\\GSP";

        [Tooltip("Logging Options")]
        public GSPLogging Logging { get; set; } = new();

        public class GSPLogging
        {
            [Tooltip("WARNING: This will generate a lot of logs and slow the process down.\nRecommend using either FormKey above or Logging option on a rule instead of this.")]
            [SettingName("Debug / Trace All !WARNING!")]
            [MaintainOrder(3)]
            public bool All { get; set; }

            [Tooltip("Disable noisy logs you not interested in.")]
            [SettingName("Disable Noisy Logs")]
            [MaintainOrder(3)]
            public GSPLogNoise DisabledLogs { get; set; } = new();

            [Tooltip("Use in conjunction with Log Level of Debug or Trace. Must include any leading 0s.")]
            [SettingName("Debug / Trace Form Key")]
            [MaintainOrder(2)]
            public FormKey FormKey { get; set; }

            [Tooltip("Controls the amount of logs generated. Information is recommended for normal operations.\nDebug and Trace will only output extra logs that either match Debug / Trace Form Key or for Rules with Debug option set true.")]
            [MaintainOrder(1)]
            public LogLevel LogLevel { get; set; }
        }

        public class GSPLogNoise
        {
            [MaintainOrder(1)]
            public bool FailedTypeMatch { get; set; } = true;

            [MaintainOrder(2)]
            public bool SuccessfulTypeMatch { get; set; } = false;
        }
    }
}