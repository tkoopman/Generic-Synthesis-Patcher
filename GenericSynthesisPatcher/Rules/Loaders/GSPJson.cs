using System.Collections;
using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Rules.Loaders
{
    public class GSPJson : IGSPConfigs
    {
        private const int ClassLogCode = 0x05;
        private const string JsonExtension = ".json";
        private readonly List<GSPBase> Rules = [];

        private GSPJson ()
        { }

        /// <param name="gspConfigsPath">Directory to load JSON files from.</param>
        /// <inheritdoc cref="IGSPConfigs.TryLoadRules(int, out IGSPConfigs?)" />
        public static bool TryLoadRules (int fileCount, string gspConfigsPath, [NotNullWhen(true)] out IGSPConfigs? rules)
        {
            if (!Directory.Exists(gspConfigsPath))
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.GeneralConfigFailure, $"Missing data folder: {gspConfigsPath}", ClassLogCode);
                rules = new GSPJson();
                return true;
            }

            var files = Directory.GetFiles(gspConfigsPath).Where(x => Path.GetExtension(x).Equals(JsonExtension, StringComparison.OrdinalIgnoreCase));

            // Filter out Synthesis' settings.json file if gspConfigsPath is the ExtraSettingsDataPath.
            if (Global.Game?.State is not null && Global.Game.State.ExtraSettingsDataPath.Equals(new Noggog.DirectoryPath(gspConfigsPath)))
                files = files.Where(x => !Path.GetFileName(x).Equals("settings.json", StringComparison.OrdinalIgnoreCase));

            return TryLoadRules(fileCount, files, out rules);
        }

        /// <inheritdoc />
        public static bool TryLoadRules (int fileCount, [NotNullWhen(true)] out IGSPConfigs? rules)
        {
            string gspConfigsPath = Global.Settings.Folder;
            gspConfigsPath = gspConfigsPath.Replace("{SkyrimData}", Global.Game.State.DataFolderPath);
            gspConfigsPath = gspConfigsPath.Replace("{GameData}", Global.Game.State.DataFolderPath);
            gspConfigsPath = gspConfigsPath.Replace("{SynthesisData}", Global.Game.State.ExtraSettingsDataPath);

            return TryLoadRules(fileCount, gspConfigsPath, out rules);
        }

        /// <param name="files">List of JSON files to load.</param>
        /// <inheritdoc cref="IGSPConfigs.TryLoadRules(int, out IGSPConfigs?)" />
        public static bool TryLoadRules (int fileCount, IEnumerable<string> files, [NotNullWhen(true)] out IGSPConfigs? rules)
        {
            GSPJson loadedRules = new();
            rules = loadedRules;

            bool allPassed = true;

            int countFile = fileCount - 1;
            foreach (string? f in files)
            {
                Global.Logger.WriteLog(LogLevel.Information, LogType.GeneralConfig, $"Loading config file #{++countFile}: {f}", ClassLogCode);
                List<GSPBase>? fileRules = null;
                using (var jsonFile = File.OpenText(f))
                {
                    using var jsonReader = new JsonTextReader(jsonFile);
                    fileRules = JsonSerializer.Create(Global.Game.SerializerSettings).Deserialize<List<GSPBase>>(jsonReader);
                }

                int countRule = 1;
                foreach (var rule in fileRules ?? [])
                {
                    rule.ConfigFile = countFile;
                    rule.ConfigRule = countRule++;

                    allPassed = rule.Validate() && allPassed;

                    loadedRules.Rules.Add(rule);
                }
            }

            return allPassed;
        }

        public IEnumerator<GSPBase> GetEnumerator () => ((IEnumerable<GSPBase>)Rules).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator () => ((IEnumerable)Rules).GetEnumerator();
    }
}