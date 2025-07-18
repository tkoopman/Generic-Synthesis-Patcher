using System.Text.RegularExpressions;

using Common;

using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Helpers;

using GSPTestProject.GameData;
using GSPTestProject.GameData.Stateless;
using GSPTestProject.JsonData;

using Loqui;

using Mutagen.Bethesda.Oblivion;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit.Abstractions;

namespace GSPTestProject
{
    public partial class ValidateJSONDocumentation
    {
        public static readonly string DocumentationPath = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "../../../../docs/data/"));

        public static readonly string SchemaPath = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "../../../../GenericSynthesisPatcher/"));

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

            foreach (var recordType in gameData.BaseGame.AllRecordTypes())
            {
                if (recordType.TryGetRecordType(out var type))
                {
                    string filename = gameData.BaseGame.GetRecordType(type.Type) == recordType
                        ? type.Type.ToLowerInvariant()
                        : recordType.Name.ToLowerInvariant();

                    _ = files.Remove(filename);
                }
            }

            Assert.Empty(files);
        }

        [Theory]
        [ClassData(typeof(AllGames))]
        public void CheckJsonSchema (Game gameData)
        {
            string gameFileName = $"gsp-config-{gameData.GameCategory.ToString().ToLower()}.schema.json";
            string gameFilePath = Path.Join(SchemaPath, gameFileName);

            List<string> allTypes = [];
            foreach (var rt in gameData.BaseGame.AllRecordTypes())
            {
                if (rt.TryGetRecordType(out var _))
                    allTypes.Add(char.ToLowerInvariant(rt.Name[0]) + rt.Name[1..]);
            }

            allTypes.Sort((a, b) => string.Compare(a, b, StringComparison.OrdinalIgnoreCase));

            using var writer = new StringWriter();
            using var jsonWriter = new JsonTextWriter(writer);

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.None,
            };

            var assembly = typeof(GenericSynthesisPatcher.Global).Assembly;
            using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.gsp-config.schema.json") ?? throw new Exception();

            TextReader reader = new StreamReader(stream);
            string rawSchema = reader.ReadToEnd();

            var schema = JObject.Parse(rawSchema);
            var def = schema["definitions"];
            Assert.NotNull(def);
            var recordType = def["recordType"] as JObject;
            Assert.NotNull(recordType);

            // Create json array of types and add to recordTypes definition
            var jsonTypes = JArray.FromObject(allTypes);
            recordType.Add("enum", jsonTypes);

            // Update $id to game specific URL
            string? id = (schema["$id"] as JValue)?.Value as string;
            Assert.NotNull(id);
            var uri = new Uri(id);
            uri = new Uri(uri, $"./{gameFileName}");
            schema["$id"] = JValue.FromObject(uri.ToString());

            serializer.Serialize(writer, schema);
            string expect = writer.ToString();
            bool match = compareFile(expect, gameFilePath);

            if (!match)
            {
                Output.WriteLine($"Expected content does not match for {gameFilePath}");
                if (InPlaceUpdate)
                {
                    File.WriteAllText(gameFilePath, expect);
                    Output.WriteLine($"Updated {gameFileName} with expected content.");

                    // Confirm it now matches
                    match = compareFile(expect, gameFilePath);
                }
                else
                {
                    Output.WriteLine(expect);
                }
            }

            Assert.True(match);
        }

        [Theory]
        [ClassData(typeof(AllGames_AllRecordTypes))]
        public void CheckType_JsonFiles (GameRecordType gameRecordType)
        {
            Assert.True(gameRecordType.RecordType.TryGetRecordType(out var recordID));
            string filename = gameRecordType.BaseGame.GetRecordType(recordID.Type) == gameRecordType.RecordType
                ? Path.Join(DocumentationPath, gameRecordType.GameName, $"{recordID.Type.ToLowerInvariant()}.json")
                : Path.Join(DocumentationPath, gameRecordType.GameName, $"{gameRecordType.RecordType.Name.ToLowerInvariant()}.json");

            List<Property> properties = [];
            var aliases = gameRecordType.BaseGame.AliasMappings.Where(a => a.Type is null || a.Type == gameRecordType.RecordType.ClassType);

            addProperties(gameRecordType.BaseGame, gameRecordType.RecordType, aliases, properties, gameRecordType.RecordType.ClassType);
            foreach (var subType in gameRecordType.SubTypes)
            {
                Assert.True(gameRecordType.RecordType.TryGetRecordType(out var subRecordID));
                string subClassID = subType.ClassType.Name.SeparateWords();
                Output.WriteLine($"Checking sub-type: {subClassID} for {gameRecordType.RecordType.Name}");
                addProperties(gameRecordType.BaseGame, subType, aliases, properties, subType.ClassType, subClassID: subClassID);
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
            foreach (var recordType in gameData.BaseGame.AllRecordTypes())
            {
                if (recordType.TryGetRecordType(out var type))
                {
                    // In Oblivion found 2 different recordTypes with the same 4 letter type, so
                    // added this so only first will add alias
                    if (gameData.BaseGame.GetRecordType(type.Type) != recordType)
                        allTypes.Add(new Types(recordType.Name.SeparateWords(), recordType.Name));
                    else
                        allTypes.Add(new Types(recordType.Name.SeparateWords(), type.Type));
                }
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

        [GeneratedRegex(@"^un(?:used|known)\w*$")]
        private static partial Regex UnusedUnknown ();

        private void addProperties (BaseGame baseGame, ILoquiRegistration recordType, IEnumerable<PropertyAliasMapping>? aliases, List<Property> properties, Type parent, string? parentName = null, IEnumerable<string>? parentAliases = null, string? subClassID = null)
        {
            var checkProperties = GameRecordType.GetProperties(parent);

            foreach (var property in checkProperties)
            {
                string name = parentName is null
                    ? property.Name
                    : $"{parentName}.{property.Name}";

                if (subClassID is not null && properties.Any(p => p.Name.Equals(name)))
                    continue; // Skip if property already exists in the list

                var alias = aliases?.Where(a => string.Equals(a.RealPropertyName, property.Name, StringComparison.Ordinal));
                if (alias.SafeAny(a => a.Type is not null))
                    alias = alias.Where(a => a.Type is not null);

                var propertyAliases = alias.SafeAny()
                    ? alias.Select(a => a.PropertyName)
                    : parentName is not null && parentAliases.SafeAny()
                    ? parentAliases.Select(a => $"{a}.{property.Name}")
                    : [];

                var propertyAction = baseGame.GetAction(recordType, name);
                if (!propertyAction.IsValid)
                {
                    Output.WriteLine($"Invalid property action for {recordType.ClassType.GetClassName()}.{name}. Type of: {property.PropertyType.GetClassName()}");

                    checkForSubProperties(baseGame, recordType, properties, property.PropertyType, name, propertyAction, propertyAliases, subClassID);
                    continue;
                }

                if (!propertyAction.Action.TryGetDocumentation(property.PropertyType, name, out string? description, out string? example) || (propertyAction.Action.CanFill() && (string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(example))))
                    Output.WriteLine($"Failed to get documentation. Property {property.Name} Action: {propertyAction.Action.GetType().GetClassName()} Type: {property.PropertyType.GetClassName()}");

                if (subClassID is not null)
                {
                    name += " *"; // Indicate this is a sub-property
                    description = $"* Only valid for '{subClassID}' record types. " + (description ?? string.Empty);
                }

                var flags = PropertyFlags.None;

                if (parentName is not null)
                    flags |= PropertyFlags.SubProperty;

                if (UnusedUnknown().IsMatch(property.Name) || baseGame.HiddenProperties.Contains(property.Name))
                    flags |= PropertyFlags.Hidden;

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

                properties.Add(new Property(name, string.Join(';', propertyAliases), flags, description ?? string.Empty, example ?? string.Empty));

                // Check to see if we should search for sub-properties
                checkForSubProperties(baseGame, recordType, properties, property.PropertyType, name, propertyAction, propertyAliases, subClassID);
            }
        }

        private void checkForSubProperties (BaseGame baseGame, ILoquiRegistration recordType, List<Property> properties, Type propertyType, string name, PropertyAction propertyAction, IEnumerable<string> propertyAliases, string? subClassID)
        {
            /// To check for sub-properties type needs to be:
            /// - Currently assigned an invalid Action or action allows sub-properties
            /// - A class or struct type
            /// - Not a generic type definition
            /// - Not a primitive type or Enum/Flag
            /// - ShouldCheckForSupProperties should return true
            /// - If all the above passes and type is nullable, then make sure you class has a
            /// default constructor.
            if ((!propertyAction.IsValid || propertyAction.Action.AllowSubProperties)
                && (propertyType.IsClass || propertyType.IsValueType)
                && !propertyType.IsGenericTypeDefinition
                && !propertyType.IsPrimitive
                && !propertyType.IsEnum
                && baseGame.ShouldCheckForSupProperties(propertyType))
            {
                // Next check if the property is constructible is nullable
                if (propertyType.IsNullable())
                {
                    propertyType = propertyType.RemoveNullable();

                    if (propertyType.IsAbstract)
                        return;

                    try
                    {
                        _ = System.Activator.CreateInstance(propertyType);
                    }
                    catch
                    {
                        // Can't create an instance of this type, so skip searching for sub-properties
                        return;
                    }
                }

                addProperties(baseGame, recordType, null, properties, propertyType, name, propertyAliases, subClassID);
            }
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