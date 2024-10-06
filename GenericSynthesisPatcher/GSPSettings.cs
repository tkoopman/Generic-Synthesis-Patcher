using GenericSynthesisPatcher.Json.Converters;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

using Noggog;

namespace GenericSynthesisPatcher
{
    public class GSPSettings
    {
        [Tooltip("JSON config files location. {SkyrimData} or {SynthesisData} valid dynamic values to start folder with. {SynthesisData} can be used alone, but for {SkyrimData} you should add a subfolder.")]
        public string Folder { get; set; } = "{SkyrimData}\\GSP";
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        [Tooltip("Use in conjunction with Log Level of Trace to get even more logs for this one FormID. Must include any leading 0s.")]
        [JsonDiskName("TraceFormKey")]
        [SettingName("Trace Form Key")]
        public string? TraceFormKeyString { get => TraceFormKey?.ToString(); set => TraceFormKey = value.IsNullOrEmpty() ? null : FormKey.Factory(value); }

        [Ignore]
        public FormKey? TraceFormKey { get; set; } = null;
    }
}
