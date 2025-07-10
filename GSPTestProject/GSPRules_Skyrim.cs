using GenericSynthesisPatcher;
using GenericSynthesisPatcher.Games.Universal.Json.Data;

using GSPTestProject.GameData;
using GSPTestProject.GameData.GlobalGame.Fixtures;
using GSPTestProject.Helpers;
using GSPTestProject.StaticData;

using Loqui;

using Newtonsoft.Json;

using Noggog;

using Xunit.Abstractions;

namespace GSPTestProject
{
    [Collection("Sequential")]
    public class GSPRules_Skyrim : IClassFixture<SkyrimSEFixture>
    {
        private readonly SkyrimSEFixture _skyrimSEFixture;

        public GSPRules_Skyrim (SkyrimSEFixture skyrimSEFixture, ITestOutputHelper output)
        {
            _skyrimSEFixture = skyrimSEFixture;
            Output = output;
            Global.Logger.Out = new TestOutputTextWritter(output);
        }

        public ITestOutputHelper Output { get; }

        [Theory]
        [ClassData(typeof(ExampleRules))]
        public void CheckExamples (string json)
        {
            using var jsonFile = new StringReader(json);
            using var jsonReader = new JsonTextReader(jsonFile);
            var rules = JsonSerializer.Create(Global.Game.SerializerSettings).Deserialize<List<GSPBase>>(jsonReader);

            foreach (var rule in rules ?? [])
                Assert.True(rule.Validate());
        }

        [Theory]
        [ClassData(typeof(GSPRules_SkyrimData))]
        public void LoadRules (string json, bool expectedValidate, Func<List<GSPBase>, bool>? customValidate)
        {
            HashSet <ILoquiRegistration> enabledTypes = [];

            using var jsonFile = new StringReader(json);
            using var jsonReader = new JsonTextReader(jsonFile);
            var rules = JsonSerializer.Create(Global.Game.SerializerSettings).Deserialize<List<GSPBase>>(jsonReader);

            Assert.NotNull(rules);

            int countRule = 1;
            int count = 0;

            foreach (var rule in rules)
            {
                rule.ConfigFile = 0;
                rule.ConfigRule = countRule++;

                Assert.Equal(expectedValidate, rule.Validate());

                if (rule is GSPGroup group)
                    count += group.Rules.Count;
                else
                    count++;

                enabledTypes.Add(rule.Types);
            }

            if (customValidate is not null)
                Assert.True(customValidate(rules));
        }
    }
}