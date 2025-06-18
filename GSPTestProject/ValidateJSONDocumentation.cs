using Common;

using GenericSynthesisPatcher.Helpers;

using GSPTestProject.GameData;
using GSPTestProject.GameData.Stateless;
using GSPTestProject.JsonData;

using Mutagen.Bethesda.Oblivion;

using Newtonsoft.Json;

using Xunit.Abstractions;

namespace GSPTestProject
{
    public class ValidateJSONDocumentation
    {
        public readonly string DocumentationPath = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "../../../../docs/data/"));

        /// <summary>
        ///     If set to <c>true</c>, the test will update the JSON files with the expected content
        ///     if they do not match, then re-compare the files to ensure they now match.
        ///
        ///     If set to <c>false</c>, the test will fail if the JSON files do not match the
        ///     expected content.
        /// </summary>
        public readonly bool InPlaceUpdate = true;

        public ValidateJSONDocumentation (ITestOutputHelper output)
        {
            Output = output;
            if (!Directory.Exists(DocumentationPath))
                throw new DirectoryNotFoundException($"Documentation directory not found: {DocumentationPath}");
        }

        public ITestOutputHelper Output { get; }

        [Theory]
        [ClassData(typeof(AllGames))]
        public void Check4ExtraJsonFiles (Game gameData)
        {
            string directory = Path.Join(DocumentationPath, gameData.GameName);
            var files = Directory.GetFiles(directory, "*.json", SearchOption.TopDirectoryOnly)
                .Where(f => !f.EndsWith("types.json"))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();

            foreach (var recordType in gameData.RecordTypes)
            {
                if (recordType.TryGetRecordType(out var type))
                {
                    string filename = type.Type.ToLowerInvariant();
                    _ = files.Remove(filename);
                }
            }

            Assert.Empty(files);
        }

        [Theory]
        [ClassData(typeof(AllGames_AllRecordTypes))]
        public void CheckType_JsonFiles (GameRecordType gameRecordType)
        {
            Assert.True(gameRecordType.RecordType.TryGetRecordType(out var recordID));

            string filename = Path.Join(DocumentationPath, gameRecordType.GameName, $"{recordID.Type.ToLowerInvariant()}.json");
            List<Property> properties = [];
            var aliases = gameRecordType.BaseGame.AliasMappings.Where(a => a.Type is null || a.Type == gameRecordType.RecordType.ClassType);

            foreach (var property in gameRecordType.Properties)
            {
                var propertyAction = gameRecordType.BaseGame.GetAction(gameRecordType.RecordType, property.Name);

                var alias = aliases.Where(a => string.Equals(a.RealPropertyName, property.Name, StringComparison.Ordinal));
                if (alias.Any(a => a.Type is not null))
                    alias = alias.Where(a => a.Type is not null);

                string propertyAliases = alias.Any()
                    ? string.Join(";", alias.Select(a => a.PropertyName))
                    : string.Empty;

                if (!propertyAction.Action.TryGetDocumentation(property.PropertyType, property.Name, out string? description, out string? example))
                    Output.WriteLine($"Failed to get documentation. Property {property.Name} Action: {propertyAction.Action.GetType().GetClassName()} Type: {property.PropertyType.GetClassName()}");

                var flags = PropertyFlags.None;

                if (propertyAction.Action.CanMatch())
                    flags |= PropertyFlags.Match;

                if (propertyAction.Action.CanFill())
                    flags |= PropertyFlags.Fill;

                if (propertyAction.Action.CanForward())
                    flags |= PropertyFlags.Forward;

                if (propertyAction.Action.CanForwardSelfOnly())
                    flags |= PropertyFlags.ForwardSelfOnly;

                if (propertyAction.Action.CanMerge())
                    flags |= PropertyFlags.Merge;

                if (propertyAction.Action.GetType() == typeof(GenericSynthesisPatcher.Games.Universal.Action.DeepCopyInAction))
                    flags |= PropertyFlags.DeepCopyIn;

                properties.Add(new Property(property.Name, propertyAliases, flags, description ?? "", example ?? ""));
            }

            using var writer = new StringWriter();
            using var jsonWriter = new JsonTextWriter(writer);

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
            };

            serializer.Serialize(writer, properties);
            string expect = writer.ToString();
            bool match = compareFile(expect, filename);

            if (!match)
            {
                Output.WriteLine($"Expected content does not match for {filename}");
                if (InPlaceUpdate)
                {
                    File.WriteAllText(filename, expect);
                    Output.WriteLine($"Updated {Path.GetFileName(filename)} with expected content.");

                    // Confirm it now matches
                    match = compareFile(expect, filename);
                }
                else
                {
                    Output.WriteLine(expect);
                }
            }

            Assert.True(match);
        }

        [Theory]
        [ClassData(typeof(AllGames))]
        public void CheckTypes_Json (Game gameData)
        {
            string filename = Path.Join(DocumentationPath, gameData.GameName, "types.json");

            List<Types> allTypes = [];
            foreach (var recordType in gameData.RecordTypes)
            {
                if (recordType.TryGetRecordType(out var type))
                    allTypes.Add(new Types(recordType.Name.SeparateWords(), type.Type));
            }

            allTypes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            using var writer = new StringWriter();
            using var jsonWriter = new JsonTextWriter(writer);

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
            };

            serializer.Serialize(writer, allTypes);
            string expect = writer.ToString();
            bool match = compareFile(expect, filename);

            if (!match)
            {
                Output.WriteLine($"Expected content does not match for {filename}");
                if (InPlaceUpdate)
                {
                    File.WriteAllText(filename, expect);
                    Output.WriteLine("Updated types.json with expected content.");

                    // Confirm it now matches
                    match = compareFile(expect, filename);
                }
                else
                {
                    Output.WriteLine(expect);
                }
            }

            Assert.True(match);
        }

        private bool compareFile (string expect, string filename)
        {
            string filePath = Path.Join(filename);
            if (!File.Exists(filePath))
            {
                Output.WriteLine($"File not found: {filePath}");
                return false;
            }

            string content = File.ReadAllText(filePath);
            return content == expect;
        }
    }
}