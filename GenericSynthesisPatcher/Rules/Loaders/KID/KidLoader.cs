using System.Collections;
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
        private readonly Dictionary<string, KidIniFile> KidFiles;
        private readonly List<GSPBase> Rules = [];

        private KidLoader (int fileCount, Dictionary<string, KidIniFile> kidFiles)
        {
            KidFiles = kidFiles;
            fileCount = fileCount - 1;

            foreach (var file in kidFiles)
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

            foreach (var b in Rules.GroupBy(a => a.Types.Single().Name))
            { // Need to pre filter rules by Type in main program
                Console.WriteLine($"{b.Key}: {b.Count()}");
            }
        }

        public int Count => Rules.Count;

        public static bool TryLoadRules (int fileCount, [NotNullWhen(true)] out IGSPConfigs? rules)
        {
            KidLoader loadedRules = new(fileCount, KidIniFile.Load(Global.Game.State.DataFolderPath));
            rules = loadedRules;

            return rules.Count > 0;
        }

        public IEnumerator<GSPBase> GetEnumerator () => ((IEnumerable<GSPBase>)Rules).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

        internal static bool TryLoadRules (int fileCount, string path, [NotNullWhen(true)] out IGSPConfigs? rules)
        {
            KidLoader loadedRules = new(fileCount, KidIniFile.Load(path));
            rules = loadedRules;

            return rules.Count > 0;
        }
    }
}