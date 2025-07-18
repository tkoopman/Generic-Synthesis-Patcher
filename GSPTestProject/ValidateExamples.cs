using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using Noggog;

using Xunit.Abstractions;

namespace GSPTestProject
{
    public class ValidateExamples
    {
        private readonly ITestOutputHelper Output;
        private readonly string rawSchema;
        private readonly JSchema schema;

        public ValidateExamples (ITestOutputHelper output)
        {
            Output = output;

            string schemaPath = Path.Combine(ValidateJSONDocumentation.SchemaPath, "gsp-config-skyrim.schema.json");
            rawSchema = File.ReadAllText(schemaPath);

            schema = JSchema.Parse(rawSchema);
        }

        [Theory]
        [ClassData(typeof(GameData.SkyrimExamples))]
        public void ValidateExamples_Test (string json)
        {
            var config = JArray.Parse(json);
            Assert.NotNull(config);

            bool valid = config.IsValid(schema, out IList<string> errors);

            errors.ForEach(Output.WriteLine);

            Assert.True(valid);
        }
    }
}