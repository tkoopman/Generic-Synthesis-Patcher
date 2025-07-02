using GSPTestProject.GameData.GlobalGame.Fixtures;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Order;

using Xunit.Abstractions;

using Global = GenericSynthesisPatcher.Global;

namespace GSPTestProject
{
#pragma warning disable CS9113 // Parameter is unread.

    public sealed class OblivionTests (OblivionFixture oblivionFixture, ITestOutputHelper output) : GameTestsBase(output), IClassFixture<OblivionFixture>
#pragma warning restore CS9113 // Parameter is unread.
    {
        protected override GameRelease GameRelease => GameRelease.OblivionRE;

        protected override Type ModGetterType => typeof(IEnumerable<IModListing<IOblivionModGetter>>);

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