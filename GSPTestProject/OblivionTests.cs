using GSPTestProject.GameData.Oblivion;

using Loqui;

using Mutagen.Bethesda.Oblivion;

using Xunit.Abstractions;

using Global = GenericSynthesisPatcher.Global;

namespace GSPTestProject
{
#pragma warning disable CS9113 // Parameter is unread.

    public sealed class OblivionTests (OblivionFixture oblivionFixture, ITestOutputHelper output) : GameTestsBase(output), IClassFixture<OblivionFixture>
#pragma warning restore CS9113 // Parameter is unread.
    {
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