using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace GSPTestProject.GameData.GlobalGame.Fixtures
{
    public sealed class SkyrimSEFixture : BaseFixture<ISkyrimMod, ISkyrimModGetter>
    {
        public override GameRelease GameRelease => GameRelease.SkyrimSE;
    }
}