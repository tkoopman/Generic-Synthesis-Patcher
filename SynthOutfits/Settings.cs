using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SynthOutfits
{
    public class Settings
    {
        [Tooltip("JSON config files location. {SkyrimData} or {SynthesisData} valid dynamic values to start folder with. {SynthesisData} can be used alone, but for {SkyrimData} you should add a sub-folder.")]
        public string Folder { get; set; } = "{SkyrimData}\\SynthOutfits";

        [Tooltip("Output folder for the SPID ini. Leave empty for Skyrim data folder.")]
        public string Output { get; set; } = "";

        [Tooltip("Updates cache from existing version of this mod, to make sure any existing outfits keep the same FormID.\nThis happens after loading the outfit's cache from the Synthesis data folder.")]
        [SettingName("Update cache from existing mod")]
        public bool UpdateCache { get; set; } = true;
    }
}