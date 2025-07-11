﻿using System.Text.RegularExpressions;

using Common;

using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Helpers;

using GSPTestProject.GameData;
using GSPTestProject.GameData.Stateless;
using GSPTestProject.JsonData;

using Mutagen.Bethesda.Oblivion;

using Newtonsoft.Json;

using Xunit.Abstractions;

namespace GSPTestProject
{
    public partial class ValidateJSONDocumentation
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
        [ClassData(typeof(AllGames_AllRecordTypes))]
        public void CheckType_JsonFiles (GameRecordType gameRecordType)
        {
            Assert.True(gameRecordType.RecordType.TryGetRecordType(out var recordID));
            string filename = gameRecordType.BaseGame.GetRecordType(recordID.Type) == gameRecordType.RecordType
                ? Path.Join(DocumentationPath, gameRecordType.GameName, $"{recordID.Type.ToLowerInvariant()}.json")
                : Path.Join(DocumentationPath, gameRecordType.GameName, $"{gameRecordType.RecordType.Name.ToLowerInvariant()}.json");

            List<Property> properties = [];
            var aliases = gameRecordType.BaseGame.AliasMappings.Where(a => a.Type is null || a.Type == gameRecordType.RecordType.ClassType);

            addProperties(gameRecordType, aliases, properties, gameRecordType.RecordType.ClassType);

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

        private void addProperties (GameRecordType gameRecordType, IEnumerable<GenericSynthesisPatcher.Games.Universal.PropertyAliasMapping>? aliases, List<Property> properties, Type parent, string? parentName = null, IEnumerable<string>? parentAliases = null)
        {
            var checkProperties = GameRecordType.GetProperties(parent);

            foreach (var property in checkProperties)
            {
                string name = parentName is null
                    ? property.Name
                    : $"{parentName}.{property.Name}";

                var alias = aliases?.Where(a => string.Equals(a.RealPropertyName, property.Name, StringComparison.Ordinal));
                if (alias.SafeAny(a => a.Type is not null))
                    alias = alias.Where(a => a.Type is not null);

                var propertyAliases = alias.SafeAny()
                    ? alias.Select(a => a.PropertyName)
                    : parentName is not null && parentAliases.SafeAny()
                    ? parentAliases.Select(a => $"{a}.{property.Name}")
                    : [];

                var propertyAction = gameRecordType.BaseGame.GetAction(gameRecordType.RecordType, name);
                if (!propertyAction.IsValid)
                {
                    Output.WriteLine($"Invalid property action for {gameRecordType.RecordType.ClassType.GetClassName()}.{name}. Type of: {property.PropertyType.GetClassName()}");

                    checkForSubProperties(gameRecordType, properties, property.PropertyType, name, propertyAction, propertyAliases);
                    continue;
                }

                if (!propertyAction.Action.TryGetDocumentation(property.PropertyType, name, out string? description, out string? example) || (propertyAction.Action.CanFill() && (string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(example))))
                    Output.WriteLine($"Failed to get documentation. Property {property.Name} Action: {propertyAction.Action.GetType().GetClassName()} Type: {property.PropertyType.GetClassName()}");

                var flags = PropertyFlags.None;

                if (parentName is not null)
                    flags |= PropertyFlags.SubProperty;

                if (UnusedUnknown().IsMatch(property.Name) || gameRecordType.BaseGame.HiddenProperties.Contains(property.Name))
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

                properties.Add(new Property(name, string.Join(';', propertyAliases), flags, description ?? "", example ?? ""));

                // Check to see if we should search for sub-properties
                checkForSubProperties(gameRecordType, properties, property.PropertyType, name, propertyAction, propertyAliases);
            }
        }

        private void checkForSubProperties (GameRecordType gameRecordType, List<Property> properties, Type propertyType, string name, PropertyAction propertyAction, IEnumerable<string> propertyAliases)
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
                && gameRecordType.BaseGame.ShouldCheckForSupProperties(propertyType))
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

                addProperties(gameRecordType, null, properties, propertyType, name, propertyAliases);
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