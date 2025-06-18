using Mutagen.Bethesda;
using Mutagen.Bethesda.Oblivion;

namespace GSPTestProject.GameData.GlobalGame.Fixtures
{
    public sealed class OblivionFixture : BaseFixture<IOblivionMod, IOblivionModGetter>
    {
        public override GameRelease GameRelease => GameRelease.Oblivion;
    }
}