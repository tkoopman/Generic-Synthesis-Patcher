using GSPTestProject.GameData.GlobalGame.Fixtures;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;

using Xunit.Abstractions;

using Global = GenericSynthesisPatcher.Global;

namespace GSPTestProject
{
#pragma warning disable CS9113 // Parameter is unread.

    public sealed class GeneralTests_Skyrim (SkyrimSEFixture skyrimFixture, ITestOutputHelper output) : GeneralTests_Base(output), IClassFixture<SkyrimSEFixture>
#pragma warning restore CS9113 // Parameter is unread.
    {
        protected override GameRelease GameRelease => GameRelease.SkyrimSE;

        protected override Type ModGetterType => typeof(IEnumerable<IModListing<ISkyrimModGetter>>);

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

        [Fact]
        public void LoadOrder ()
        {
            Output.WriteLine($"Load Order:");
            foreach (var mod in Global.Game.LoadOrder)
                Output.WriteLine($"  {mod.FileName}");
        }
    }
}