using System.Collections.Immutable;
using System.Drawing;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher
{
    internal static class GenerateDoco
    {
        private const string RPMPopulateFooter = """
            #pragma warning restore format
                    }
                }
            }
            """;

        private const string RPMPopulateHeader = """
            using System.Drawing;

            using GenericSynthesisPatcher.Helpers.Action;

            using Mutagen.Bethesda.Skyrim;

            using Noggog;

            namespace GenericSynthesisPatcher.Helpers
            {
                public static partial class RecordPropertyMappings
                {
                    private static void populateMappings ()
                    {
            #pragma warning disable format
            """;

        private static readonly Type[] ForceDeeperTypes = [
            typeof(P2Double),
            typeof(P2Float),
            typeof(P2Int),
            typeof(P2Int16),
            typeof(P3Double),
            typeof(P3Float),
            typeof(P3Int),
            typeof(P3Int16),
            ];

        private static readonly Type[] IgnoreDeepScanOnTypes = [
            typeof(AMagicEffectArchetype),
            typeof(AssetLink<>),
            typeof(Cell),
            typeof(CellMaxHeightData),
            typeof(DialogResponsesAdapter),
            typeof(ExtendedList<>),
            typeof(FaceFxPhonemes),
            typeof(FormLink<>),
            typeof(FormLinkNullable<>),
            typeof(IFormLink<>),
            typeof(IFormLinkNullable<>),
            typeof(Landscape),
            typeof(LocationTargetRadius),
            typeof(Model),
            typeof(PackageAdapter),
            typeof(PerkAdapter),
            typeof(QuestAdapter),
            typeof(RegionGrasses),
            typeof(RegionLand),
            typeof(RegionMap),
            typeof(RegionObjects),
            typeof(RegionSounds),
            typeof(RegionWeather),
            typeof(SceneAdapter),
            typeof(string),
            typeof(TranslatedString),
            typeof(VirtualMachineAdapter),
            typeof(WorldspaceMaxHeight),
            ];

        private static readonly string[] IgnoreProperty = [
            "BodyTemplate.ActsLike44",
            "DATADataTypeState",
            "FormKey",
            "IsCompressed",
            "IsDeleted",
            "MajorRecordFlagsRaw",
            "PersistentTimestamp",
            "PersistentUnknownGroupData",
            "SubCellsTimestamp",
            "SubCellsUnknown",
            "TemporaryTimestamp",
            "TemporaryUnknownGroupData",
            "Timestamp",
            "TopCell",
            "Unknown",
            "Unknown08",
            "Unknown09",
            "Unknown0A",
            "Unknown0B",
            "Unknown0C",
            "Unknown0D",
            "Unknown0E",
            "Unknown0F",
            "Unknown1",
            "Unknown10",
            "Unknown14",
            "Unknown2",
            "Unknown3",
            "Unknown4",
            "Unknown4",
            "Unknown48",
            "Unknown49",
            "Unknown4A",
            "Unknown4B",
            "Unknown4C",
            "Unknown4D",
            "Unknown4E",
            "Unknown4F",
            "Unknown5",
            "Unknown50",
            "Unknown54",
            "Unknown6",
            "Unknown7",
            "UnknownGroupData",
            "Unused",
            "Unused2",
            "Unused3",
            "Unused4",
            "UnusedNoisemaps",
            "Version2",
            "VersionControl",
            "Versioning",
            ];

        private static readonly int[] PropCols = [0, 0, 0, 100, 100];

        private static StringWriter sw = new();

        internal static void generate (IPatcherState<ISkyrimMod, ISkyrimModGetter>? state)
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
            };

            bool OutputUnimplemented = state is not null;
            if (state is not null)
                Global.SetState(state);

            List<RecordTypeMapping> ImplementedRTMs = [];
            List<RPMDetails> buildRPMs = [];
            List<string[]> subPropertyAliases = [];

            // Output Implemented Properties per Record type
            foreach (var rtm in Global.RecordTypeMappings.All)
            {
                // Loop all properties of setter looking for implemented properties
                var properties = processProperties(state, rtm, rtm.StaticRegistration.SetterType, null);

                buildRPMs.AddRange(properties);

                // Output heading per record type
                if ((OutputUnimplemented && properties.Count > 0) || properties.Any(p => p.RPM.Action is not null && p.RecordActionInterface is not null))
                {
                    ImplementedRTMs.Add(rtm);

                    // Sort and populate documentation fields
                    properties.Sort((l, r) => string.Compare(l.PropertyName, r.PropertyName, StringComparison.OrdinalIgnoreCase));
                    foreach (var row in properties)
                    {
                        if (row.RPM.Action is not null && row.RecordActionInterface is not null)
                        {
                            if (row.RPM.Action.TryGetDocumentation(row.PropertyType, row.PropertyName, out string? description, out string? example))
                            {
                                row.Description = description;
                                row.Example = example;
                            }
                        }
                        else if (OutputUnimplemented)
                        {
                            row.Description = $"{row.PropertyType.GetClassName()} - {row.RecordActionInterface.GetClassName()}";
                        }
                    }

                    subPropertyAliases.AddRange(generateSubPropertyAliases(properties));

                    /*
                     * Output Implemented Properties
                     */

                    using (var sw = new StreamWriter($"{rtm.Name.ToLowerInvariant()}.json"))
                    using (var writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, properties.Where(p => p.RecordActionInterface is not null && p.RPM.Action is not null));
                    }
                }
            }

            /*
             * Output Sub-properties Aliases
             */

            subPropertyAliases.Sort(static (l, r) =>
            {
                int comp = string.Compare(l[0], r[0], StringComparison.OrdinalIgnoreCase);
                return comp != 0 ? comp : string.Compare(l[1], r[1], StringComparison.OrdinalIgnoreCase);
            });

            sw = new();
            sw.WriteLine(RPMPopulateHeader.Replace("populateMappings", "populateSubAliases"));
            printTableRow(subPropertyAliases, sep: ",", prefix: "            ", suffix: "", padLast: false);
            sw.WriteLine(RPMPopulateFooter);

            using var aliasesFW = new StreamWriter(Path.Combine(Path.GetTempPath(), "GSP_Aliases.txt"), false);
            aliasesFW.Write(sw.ToString());
            aliasesFW.Close();

            /*
             * Output Implemented Types
             */

            using (var streamWriter = new StreamWriter(@"types.json"))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(writer, ImplementedRTMs);
            }

            /*
             * Output RPM code for all identified property types
             */

            var groupRPMs = buildRPMs.GroupBy(g => (g.PropertyName, g.RecordActionInterface),
                                              g => (g.RTM.StaticRegistration.GetterType, g.PropertyType), (k, data) => new { k.PropertyName, k.RecordActionInterface, Types = data.Select(d => d.GetterType), PropertyTypes = data.Select(d => d.PropertyType).Distinct()});

            var implemented = groupRPMs.Where(g => g.RecordActionInterface != null);

            var notImplemented = buildRPMs.Where(r => r.RecordActionInterface == null).GroupBy(g => g.PropertyType).Select(g => new { PropertyType = g.Key, RecordTypes = g.Count(), Uses = g.Count(r => r.IsUsed), SubPropertiesMin = g.Min(a => a.SubProperties), SubPropertiesMax = g.Max(a => a.SubProperties), Example = $"{g.First().RTM.FullName}.{g.First().PropertyName}" }).ToList();

            // Sort by uses then name
            notImplemented.Sort((l, r) =>
            {
                int i = l.Uses.CompareTo(r.Uses);

                if (i == 0)
                    i = l.RecordTypes.CompareTo(r.RecordTypes);

                if (i == 0)
                    i = string.Compare(l.PropertyType.GetClassName(), r.PropertyType.GetClassName(), StringComparison.Ordinal);

                return i;
            });

            // Print Unimplemented Entries to Screen
            Console.WriteLine("List of types implemented via sub-properties.");
            foreach (var rpm in notImplemented.Where(i => i.SubPropertiesMin > 0 && i.SubPropertiesMax == i.SubPropertiesMin))
            {
                if (state is null)
                    Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.RecordTypes}");
                else
                    Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.Uses}/{rpm.RecordTypes}");
            }

            Console.WriteLine();
            Console.WriteLine("List of unimplemented types.");
            foreach (var rpm in notImplemented.Where(i => !(i.SubPropertiesMin > 0 && i.SubPropertiesMax == i.SubPropertiesMin)))
            {
                if (state is null)
                    Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.RecordTypes}, {rpm.Example}");
                else
                    Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.Example}, {rpm.Uses}/{rpm.RecordTypes}");
            }

            /*
             * Output implemented to file as RPM code to paste into RecordPropertyMappings.cs
             */

            List<string[]> lines = [];
            foreach (var property in implemented)
            {
                if (property.Types.Count() >= 3)
                {
                    var mismatch = buildRPMs.FirstOrDefault(r => r.PropertyName.Equals(property.PropertyName, StringComparison.OrdinalIgnoreCase) && r.RecordActionInterface is null);

                    if (mismatch is null)
                    {
                        lines.Add(["Add(null", $"\"{property.PropertyName}\"", $"{property.RecordActionInterface.GetClassName()}.Instance);"]);
                        continue;
                    }

                    Console.WriteLine($"Skipping wildcard RPM for {property.PropertyName} due to {mismatch.RTM.Name} - {mismatch.PropertyType.GetClassName()}");
                }

                var types = property.Types.ToList();
                types.Sort(static (l, r) => string.Compare(l.Name, r.Name, StringComparison.Ordinal));
                foreach (var type in types)
                    lines.Add([$"Add(typeof({type.Name})", $"\"{property.PropertyName}\"", $"{property.RecordActionInterface.GetClassName()}.Instance);"]);
            }

            lines.Sort(static (l, r) =>
            {
                int comp = string.Compare(l[1], r[1], StringComparison.OrdinalIgnoreCase);
                if (comp != 0)
                    return comp;

                if (l[0] == "Add(null")
                    return 1;

#pragma warning disable IDE0046 // Convert to conditional expression
                if (r[0] == "Add(null")
                    return -1;
#pragma warning restore IDE0046 // Convert to conditional expression

                return string.Compare(l[0], r[0], StringComparison.OrdinalIgnoreCase);
            });

            sw = new();
            sw.WriteLine(RPMPopulateHeader);
            printTableRow(lines, sep: ",", prefix: "            ", suffix: "", padLast: false);
            sw.WriteLine(RPMPopulateFooter);

            using var rpmFW = new StreamWriter(Path.Combine(Path.GetTempPath(), "GSP_RPM.txt"), false);
            rpmFW.Write(sw.ToString());
            rpmFW.Close();

            //_ = System.Diagnostics.Process.Start("explorer", $"\"{Path.Combine(Path.GetTempPath(), "GSP_RPM.txt")}\"");
        }

        private static Type? calcRPMAction (Type type)
        {
            type = type.RemoveNullable();

            var mainType = type.GetIfGenericTypeDefinition();
            var subType = type.GetIfUnderlyingType()?.GetIfGenericTypeDefinition();
            var subSubType = type.GetIfUnderlyingType()?.GetIfUnderlyingType();

            if (type.IsEnum)
                return type.GetCustomAttributes(typeof(FlagsAttribute), true).FirstOrDefault() == null ? typeof(EnumsAction) : typeof(FlagsAction);

            if (type.IsAssignableTo(typeof(ITranslatedStringGetter)))
                return typeof(ConvertibleAction<string>);

            if (type.IsAssignableTo(typeof(IConvertible)) && (type.IsPrimitive || type == typeof(string)))
            {
                return type.Name switch
                {
                    nameof(Boolean) => typeof(ConvertibleAction<bool>),
                    nameof(Byte) => typeof(ConvertibleAction<byte>),
                    nameof(Char) => typeof(ConvertibleAction<char>),
                    nameof(Int16) => typeof(ConvertibleAction<short>),
                    nameof(Int32) => typeof(ConvertibleAction<int>),
                    nameof(SByte) => typeof(ConvertibleAction<sbyte>),
                    nameof(Single) => typeof(ConvertibleAction<float>),
                    nameof(String) => typeof(ConvertibleAction<string>),
                    nameof(UInt16) => typeof(ConvertibleAction<ushort>),
                    nameof(UInt32) => typeof(ConvertibleAction<uint>),
                    _ => null
                };
            }
            else if (type == typeof(Percent))
            {
                return typeof(BasicAction<Percent>);
            }
            else if (type == typeof(Color))
            {
                return typeof(BasicAction<Color>);
            }
            else if (type.IsAssignableTo(typeof(IObjectBoundsGetter)))
            {
                return typeof(ObjectBoundsAction);
            }
            else if (type.IsAssignableTo(typeof(IPlayerSkillsGetter)))
            {
                return typeof(PlayerSkillsAction);
            }
            else if (type == typeof(WorldspaceMaxHeight))
            {
                return typeof(WorldspaceMaxHeightAction);
            }
            else if (type == typeof(CellMaxHeightData))
            {
                return typeof(CellMaxHeightDataAction);
            }
            else if (type == typeof(MemorySlice<byte>))
            {
                return typeof(MemorySliceByteAction);
            }
            else if (type == typeof(Model))
            {
                return typeof(ModelAction);
            }
            else if (subType != null)
            {
                if (mainType.IsAssignableTo(typeof(IFormLink<>)) || mainType.IsAssignableTo(typeof(IFormLinkNullable<>)))
                {
                    return typeof(FormLinkAction<>).MakeGenericType(subType);
                }
                else if (mainType.IsAssignableTo(typeof(ExtendedList<>)))
                {
                    if (subType.IsAssignableTo(typeof(IFormLinkGetter<>)) && subSubType != null)
                        return typeof(FormLinksAction<>).MakeGenericType(subSubType);
                    if (subType.IsAssignableTo(typeof(IContainerEntryGetter)))
                        return typeof(ContainerItemsAction);
                    if (subType.IsAssignableTo(typeof(IEffectGetter)))
                        return typeof(EffectsAction);
                    if (subType.IsAssignableTo(typeof(ILeveledItemEntryGetter)))
                        return typeof(LeveledItemAction);
                    if (subType.IsAssignableTo(typeof(ILeveledNpcEntryGetter)))
                        return typeof(LeveledNpcAction);
                    if (subType.IsAssignableTo(typeof(ILeveledSpellEntryGetter)))
                        return typeof(LeveledSpellAction);
                    if (subType.IsAssignableTo(typeof(IRankPlacementGetter)))
                        return typeof(RankPlacementAction);
                    if (subType.IsAssignableTo(typeof(IRelationGetter)))
                        return typeof(RelationsAction);
                }
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1826:Do not use Enumerable methods on indexable collections", Justification = "<Pending>")]
        private static List<string[]> generateSubPropertyAliases (List<RPMDetails> properties)
        {
            List<string[]> lines = [];
            properties = [.. properties.Where(p => p.PropertyName.Contains('.'))];
            properties.Sort(static (l, r) => string.Compare(l.PropertyName, r.PropertyName, StringComparison.OrdinalIgnoreCase));

            Type? lastType = null;
            string? lastName = null;
            string? lastAlias = null;

            foreach (var property in properties)
            {
                if (lastType is null || !property.RTM.StaticRegistration.GetterType.Equals(lastType))
                {
                    lastType = property.RTM.StaticRegistration.GetterType;
                    lastName = null;
                    lastAlias = null;
                }

                // Skip if already covered by null alias
                if (RecordPropertyMappings.GetNullAliases(property.PropertyName).Any())
                    continue;

                string name = property.PropertyName[..property.PropertyName.IndexOf('.')];
                if (!name.Equals(lastName, StringComparison.Ordinal))
                {
                    lastName = name;
                    lastAlias = RecordPropertyMappings.GetAllAliases(lastType, name).FirstOrDefault();
                }

                if (lastAlias is not null)
                    lines.Add([$"AddAlias(typeof({lastType.Name})", $"\"{lastAlias}{property.PropertyName[property.PropertyName.IndexOf('.')..]}\"", $"\"{property.PropertyName}\");"]);
            }

            return lines;
        }

        private static void printTableRow (List<string[]> lines, int maxPaddedWidth = 250, char padding = ' ', string sep = " |", string? prefix = "| ", string? suffix = " |", bool padLast = true)
        {
            if (lines.Count == 0 || lines.Any(v => v.Length != lines[0].Length))
                throw new Exception($"Nope! Try again");

            int[] widths = new int[lines[0].Length - (padLast ? 0:1)];
            for (int i = 0; i < widths.Length; i++)
                widths[i] = lines.Select(v => v[i].Length).Max();

            maxPaddedWidth = maxPaddedWidth - (prefix?.Length ?? 0) - (sep.Length * widths.Length) - suffix?.Length ?? 0;
            while (maxPaddedWidth > 0 && widths.Sum() > maxPaddedWidth)
            {
                int max = widths.Max();
                for (int i = widths.Length - 1; i >= 0; i--)
                {
                    if (widths[i] == max)
                    {
                        widths[i] -= (widths.Sum() > (maxPaddedWidth * 2)) ? maxPaddedWidth / widths.Length : 1;
                        break;
                    }
                }
            }

            foreach (string[] line in lines)
            {
                if (prefix != null)
                    sw.Write(prefix);

                bool spacer = padLast && line[0].Length == 1 && line.Count(l => l == line[0]) == line.Length;
                if (spacer)
                {
                    sw.Write($"{"".PadRight(widths[0], line[0].First())}{sep}");
                    for (int i = 1; i < line.Length - 1; i++)
                        sw.Write($" {"".PadRight(widths[i], line[0].First())}{sep}");
                    sw.Write($" {"".PadRight(widths[^1], line[0].First())}");
                }
                else
                {
                    sw.Write($"{line[0].PadRight(widths[0], padding)}{sep}");
                    for (int i = 1; i < line.Length - 1; i++)
                        sw.Write($" {line[i].PadRight(widths[i], padding)}{sep}");

                    if (line.Length == widths.Length)
                        sw.Write($" {line[^1].PadRight(widths[^1], padding)}");
                    else
                        sw.Write($" {line[^1]}");
                }

                if (suffix != null)
                    sw.Write(suffix);

                sw.WriteLine();
            }
        }

        private static List<RPMDetails> processProperties (IPatcherState<ISkyrimMod, ISkyrimModGetter>? state, RecordTypeMapping rtm, Type parentType, string? parentName)
        {
            List<RPMDetails> buildRPMs = [];
            bool OutputUnimplemented = state is not null;

            var properties = parentType.GetPublicProperties().Where(p => (p.CanRead && p.CanWrite) || p.PropertyType.GetIfGenericTypeDefinition() == typeof(ExtendedList<>)).DistinctBy(p => p.Name);
            foreach (var property in properties)
            {
                string propertyFullName = parentName is null ? property.Name : $"{parentName}.{property.Name}";
                if (property.GetGetMethod()?.GetParameters().Length != 0)
                    continue;
                if (IgnoreProperty.Contains(propertyFullName) || IgnoreProperty.Contains(property.Name))
                    continue;

                var rpmDetails = new RPMDetails (rtm, propertyFullName, property.PropertyType);
                buildRPMs.Add(rpmDetails);

                rpmDetails.RecordActionInterface = calcRPMAction(property.PropertyType);
                rpmDetails.IsUsed = rpmDetails.RecordActionInterface is not null;

                if (RecordPropertyMappings.tryFindMapping(rtm.StaticRegistration.GetterType, propertyFullName, out var rpm))
                {
                    // record property mapping (RPM) was found
                    if (rpm.Action.GetType().Equals(rpmDetails.RecordActionInterface))
                        rpmDetails.RPM = rpm;
                    else
                        Console.WriteLine($"RPM mismatch on {rtm.Name}.{propertyFullName}");
                }

                if (parentName is null || ForceDeeperTypes.Contains(property.PropertyType.RemoveNullable().GetIfGenericTypeDefinition()))
                {
                    var type = property.PropertyType;

                    if (type.IsNullable())
                    {
                        type = type.RemoveNullable();

                        // Confirm you can create new instance
                        try
                        {
                            _ = System.Activator.CreateInstance(type);
                        }
                        catch
                        {
                            type = null;
                        }
                    }

                    if (type is not null && !IgnoreDeepScanOnTypes.Contains(type.GetIfGenericTypeDefinition()))
                    {
                        var subProperties = processProperties(null, rtm, type, propertyFullName);
                        buildRPMs.AddRange(subProperties);
                        rpmDetails.SubProperties = subProperties.Count;
                    }

                    if (OutputUnimplemented)
                    {
                        // Check if property used (On at least 1 record has a non-null, non-default
                        // or list count > 0)
                        FormKey? used = null;

                        if (state is not null)
                        {
                            foreach (var c in rtm.WinningContextOverrides())
                            {
                                // Check if any record has value set to a non-null value
                                if (Mod.TryGetProperty(c.Record, propertyFullName, out object? value) && !Mod.IsNullOrEmpty(value))
                                {
                                    used = c.Record.FormKey;
                                    rpmDetails.IsUsed = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return buildRPMs;
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class RPMDetails (RecordTypeMapping rtm, string propertyName, Type propertyType)
        {
            [JsonProperty]
            public string Aliases
            {
                get
                {
                    if (RecordActionInterface is null)
                        return "";

                    var names = RecordPropertyMappings.GetAllAliases(RTM.StaticRegistration.GetterType, PropertyName).ToList();
                    names.Sort();

                    return string.Join(';', names);
                }
            }

            [JsonProperty]
            public string Description { get; set; } = "";

            [JsonProperty]
            public string Example { get; set; } = "";

            public bool IsUsed { get; set; }

            [JsonProperty]
            public string MFFSM => RPM.Action is null
                ? "-----"
                : string.Join<char>(string.Empty,
                [
                    RPM.Action.CanMatch() ? 'M' : '-',
                    RPM.Action.CanFill() ? 'F' : '-',
                    RPM.Action.CanForward() ? 'F': '-',
                    RPM.Action.CanForwardSelfOnly() ? 'S': '-',
                    RPM.Action.CanMerge() ? 'M': '-',
                ]);

            [JsonProperty(propertyName: "Name")]
            public string PropertyName { get; set; } = propertyName;

            public Type PropertyType { get; set; } = propertyType;
            public Type? RecordActionInterface { get; set; }
            public RecordPropertyMapping RPM { get; set; }
            public RecordTypeMapping RTM { get; set; } = rtm;
            public int SubProperties { get; set; }
        }
    }
}