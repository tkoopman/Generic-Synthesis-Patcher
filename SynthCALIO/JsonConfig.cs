using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

namespace SynthCALIO
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonConfig
    {
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<JsonLeveledItem> LeveledItems { get; } = [];

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<JsonOutfit> Outfits { get; } = [];

        internal static string? CurrentFile { get; set; }

        public void LoadConfigurationFiles (IRunnabilityState state)
        {
            string dataFolder = Program.Settings.Value.Folder;
            dataFolder = dataFolder.Replace("{SkyrimData}", state.DataFolderPath);
            dataFolder = dataFolder.Replace("{SynthesisData}", state.ExtraSettingsDataPath);

            if (!Directory.Exists(dataFolder))
                throw new DirectoryNotFoundException($"Could not find data folder: {dataFolder}");

            var files = Directory.GetFiles(dataFolder).Where(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
            foreach (string file in files)
            {
                if (file.Equals(Path.Combine(state.ExtraSettingsDataPath ?? "", "settings.json"), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Skipping: {file}");
                    continue;
                }

                Console.WriteLine($"Loading configuration file: {file}");
                CurrentFile = file;
                JsonConvert.PopulateObject(File.ReadAllText(file), this);
            }

            basicChecks();

            Console.WriteLine($"Loaded {Outfits.Count} outfits from configuration files");
        }

        /// <summary>
        ///     Basic checks to ensure the configuration files look valid. Also checks if any
        ///     duplicate EditorIDs are found. Should throw exception if any issues are found.
        /// </summary>
        private void basicChecks ()
        {
            var editorIDs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in LeveledItems)
            {
                entry.BasicChecks();

                if (!editorIDs.Add(entry.EditorID))
                    throw new InvalidDataException($"Duplicate EditorID found {entry.EditorID}");
            }

            foreach (var outfit in Outfits)
            {
                outfit.BasicChecks();
                if (!editorIDs.Add(outfit.EditorID))
                    throw new InvalidDataException($"Duplicate EditorID found {outfit.EditorID}");
            }
        }
    }
}