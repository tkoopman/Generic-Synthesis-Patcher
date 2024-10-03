using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace GenericSynthesisPatcher
{
    public class GSPSettings
    {
        [Tooltip("JSON config files location. {SkyrimData} or {SynthesisData} valid dynamic values to start folder with. {SynthesisData} can be used alone, but for {SkyrimData} you should add a subfolder.")]
        public string Folder { get; set; } = "{SkyrimData}\\GSP";
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}
