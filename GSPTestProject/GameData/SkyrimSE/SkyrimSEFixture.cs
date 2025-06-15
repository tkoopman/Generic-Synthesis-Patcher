using GSPTestProject.GameData.Universal;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace GSPTestProject.GameData.SkyrimSE
{
    public sealed class SkyrimSEFixture : BaseFixture<ISkyrimMod, ISkyrimModGetter>
    {
        public override GameRelease GameRelease => GameRelease.SkyrimSE;
    }
}