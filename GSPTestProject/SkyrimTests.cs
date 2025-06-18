using GSPTestProject.GameData.GlobalGame.Fixtures;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

using Xunit.Abstractions;

using Global = GenericSynthesisPatcher.Global;

namespace GSPTestProject
{
#pragma warning disable CS9113 // Parameter is unread.

    public sealed class SkyrimTests (SkyrimSEFixture skyrimFixture, ITestOutputHelper output) : GameTestsBase(output), IClassFixture<SkyrimSEFixture>
#pragma warning restore CS9113 // Parameter is unread.
    {
        protected override GameRelease GameRelease => GameRelease.SkyrimSE;

        [Fact]
        public void ConfirmAllRecordTypes ()
        {
            foreach (var method in typeof(TypeOptionSolidifierMixIns).GetMethods())
            {
                if (!method.ReturnType.IsGenericType || method.ReturnType.GenericTypeArguments.Length == 0)
                    continue;

                var returnType = method.ReturnType.GenericTypeArguments[^1];

                var regoProperty = returnType.GetProperty("StaticRegistration");
                if (regoProperty?.GetValue(null) is ILoquiRegistration rego)
                    _ = Global.Game.GetRecords(rego);
            }

            Assert.NotEmpty(Global.Game.AllRecordTypes());
        }
    }
}