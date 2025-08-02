using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

namespace GenericSynthesisPatcher.Rules.Loaders.KID
{
    /// <summary>
    ///     Converts Keyword Item Distribution (KID) ini files into GSP Rules. <br /><see href="https://www.nexusmods.com/skyrimspecialedition/mods/55728" />
    /// </summary>
    public partial class KidLoader : IGSPConfigs
    {
        private const int ClassLogCode = 0x1F;

        public KidLoader ()
        {
        }

        public int Count => Rules?.Count ?? 0;
        public Dictionary<string, KidIniFile>? IniFiles { get; private set; }
        public List<KidRule>? Rules { get; private set; }

        IEnumerable<GSPBase>? IGSPConfigs.Rules => Rules;

        public void Close (bool successful)
        {
            if (!successful || IniFiles is null)
                return;

            int countDel = 0;
            int countNew = 0;
            int count = 0;

            foreach (var (path, file) in IniFiles)
            {
                foreach (var line in file.lines)
                {
                    if (line.HandledByGsp)
                    {
                        count++;
                        if (!line.WasHandledByGsp)
                            countNew++;

                        continue;
                    }

                    if (line.WasHandledByGsp)
                        countDel++;
                }

                bool save = Global.Settings.KIDComments && (countNew != 0 || countDel != 0);
                Global.Logger.WriteLine($"{Path.GetFileName(path)}. GSP Processed: {count} Added: {countNew} Removed: {countDel}{(save ? " - Saving file." : string.Empty)}");

                if (save)
                {
                    string contents = file.ToString();
                    File.WriteAllText(path, contents);
                }
            }
        }

        [MemberNotNullWhen(true, nameof(Rules))]
        public bool LoadRules (int fileCount) => LoadRules(fileCount, Global.Game.State.DataFolderPath);

        public bool LoadRules (int fileCount, string path)
        {
            IniFiles = KidIniFile.Load(path);
            Rules = [];

            fileCount--;

            foreach (var file in IniFiles)
            {
                Global.Logger.WriteLog(LogLevel.Information, Helpers.LogType.CONFIG, $"Loading config file #{++fileCount}: {file.Key}", ClassLogCode);
                int lineNum = 0;
                int loaded = 0;
                int validLines = 0;
                foreach (var line in file.Value)
                {
                    lineNum++;
                    if (!line.SeemsValid)
                        continue;

                    validLines++;

                    var kidRule = KidRule.Convert(line);
                    if (kidRule is not null)
                    {
                        kidRule.ConfigFile = fileCount;
                        kidRule.ConfigRule = line.LineNumber;
                        line.HandledByGsp = true;
                        loaded++;
                        Rules.Add(kidRule);
                    }
                    else if (line.WasHandledByGsp)
                    {
                        Global.Logger.WriteLog(LogLevel.Warning, Helpers.LogType.CONFIG, $"KID config entry in {file.Key} line {lineNum}, was marked as previously being loaded by GSP, however failed to load this time.", ClassLogCode);
                    }
                }

                Global.Logger.WriteLog(LogLevel.Debug, Helpers.LogType.CONFIG, $"Loaded {loaded} of {validLines} rules from {Path.GetFileName(file.Key)}", ClassLogCode);
            }

            return true;
        }
    }
}