using GenericSynthesisPatcher;
using GenericSynthesisPatcher.Rules;
using GenericSynthesisPatcher.Rules.Loaders;

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

        [Fact]
        public void CheckExampleRules ()
        {
            string ExampleDirectory = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "../../../../Examples/"));

            Assert.True(GSPJson.TryLoadRules(1, ExampleDirectory, out var rules));

            int maxConfigFile = rules.Max(x => x.ConfigFile);
            int actualCount = Directory.GetFiles(ExampleDirectory).Count(x => Path.GetExtension(x).Equals(".json", StringComparison.OrdinalIgnoreCase));

            Assert.Equal(actualCount, maxConfigFile);

            var missingConfigFiles = Enumerable.Range(1, maxConfigFile).Except(rules.Select(x => x.ConfigFile).Distinct());
            Assert.Empty(missingConfigFiles);
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