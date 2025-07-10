using GenericSynthesisPatcher;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;

using Noggog;

using Global = GenericSynthesisPatcher.Global;

namespace GSPTestProject.GameData.GlobalGame.Fixtures
{
    public abstract class BaseFixture<TModSetter, TModGetter>
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
    {
        public BaseFixture ()
        {
            var env = Synthesis.Bethesda.UnitTests.Common.Utility.SetupEnvironment(GameRelease);
            var modPath = patchModPath(env.DataFolder);

            var stateFactory = env.GetStateFactory();

            using var state = stateFactory.ToState<TModSetter, TModGetter>(
            new RunSynthesisMutagenPatcher()
            {
                DataFolderPath = env.DataFolder,
                GameRelease = GameRelease,
                OutputPath = modPath,
                SourcePath = null,
                LoadOrderFilePath = env.PluginPath,
            },
            new PatcherPreferences(),
            Synthesis.Bethesda.Constants.SynthesisModKey);

            Global.Initialize(state, new GSPSettings());
        }

        public abstract Mutagen.Bethesda.GameRelease GameRelease { get; }
        private static ModKey PatchModKey => new("Patch", ModType.Plugin);

        private static ModPath patchModPath (DirectoryPath dataFolder) => new(PatchModKey, Path.Combine(dataFolder.Path, PatchModKey.ToString()));
    }
}