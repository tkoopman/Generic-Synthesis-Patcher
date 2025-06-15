using GSPTestProject.GameData.Universal;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Oblivion;

namespace GSPTestProject.GameData.Oblivion
{
    public sealed class OblivionFixture : BaseFixture<IOblivionMod, IOblivionModGetter>
    {
        public override GameRelease GameRelease => GameRelease.Oblivion;
    }
}