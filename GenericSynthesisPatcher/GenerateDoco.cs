using System.Collections.Immutable;
using System.Drawing;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher
{
    internal static class GenerateDoco
    {
        private static readonly string[] IgnoreDeepScan = [
            "Conditions",
            "PerkTree",
            "VirtualMachineAdapter",
            ];

        private static readonly string[] IgnoreProperty = [
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
            "UnusedNoisemaps",
            "Version2",
            "VersionControl",
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
                Global.State = state;

            checkAliases();
            Console.WriteLine();

            List<RecordTypeMapping> ImplementedRTMs = [];
            List<RPMDetails> buildRPMs = [];

            // Output Implemented Properties per Record type
            foreach (var rtm in RecordTypeMappings.All)
            {
                // Loop all properties of setter looking for implemented properties
                var properties = processProperties(state, rtm, rtm.StaticRegistration.SetterType, null);

                buildRPMs.AddRange(properties);

                // Output heading per record type
                if ((OutputUnimplemented && properties.Count > 0) || properties.Any(p => p.RPM.Action is not null && p.RecordActionInterface is not null))
                {
                    ImplementedRTMs.Add(rtm);

                    sw.WriteLine();
                    string h = rtm.Name;
                    h += rtm.Name.Equals(rtm.FullName, StringComparison.Ordinal) ? "" : $" - {rtm.FullName}";
                    sw.WriteLine($"## {h}");
                    sw.WriteLine();

                    List<string[]> propertyLines = [["Field", "Alt", "MFFSM", "Value Type", "Example"]];
                    propertyLines.Add(["-", "-", "-", "-", "-"]);

                    // Output properties
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

                            propertyLines.Add([row.PropertyName, row.Aliases, row.MFFSM, row.Description, row.Example]);
                        }
                        else if (OutputUnimplemented)
                        {
                            row.Description = $"{row.PropertyType.GetClassName()} - {row.RecordActionInterface.GetClassName()}";

                            string prefix = row.RecordActionInterface is null ? "*":"#";
                            propertyLines.Add([$"{prefix}{row.PropertyName}", "", "-----", row.Description, row.Example]);
                        }
                    }

                    printTableRow(propertyLines);

                    // Footer
                    sw.WriteLine();
                    sw.WriteLine("[⬅ Back to Types](Types.md)");

                    /*
                     * Output Implemented Types
                     */

                    using (var sw = new StreamWriter($"{rtm.Name.ToLowerInvariant()}.json"))
                    using (var writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, properties);
                    }
                }
            }

            /*
             * Output Implemented Types
             */

            using (var streamWriter = new StreamWriter(@"types.json"))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(writer, ImplementedRTMs);
            }

            // Write Types and Implemented Properties to Temp File
            using var fw = new StreamWriter(Path.Combine(Path.GetTempPath(), "GSPDoco.txt"), false);
            fw.Write(sw.ToString());
            fw.Close();

            /*
             * Output RPM code for all identified property types
             */

            var groupRPMs = buildRPMs.GroupBy(g => (g.PropertyName, g.RecordActionInterface),
                                              g => (g.RTM.StaticRegistration.GetterType, g.PropertyType), (k, data) => new { PropertyName = k.PropertyName, RPM = k.RecordActionInterface, Types = data.Select(d => d.GetterType), PropertyTypes = data.Select(d => d.PropertyType).Distinct()});

            var implemented = groupRPMs.Where(g => g.RPM != null);

            var notImplemented = buildRPMs.Where(r => r.RecordActionInterface == null).GroupBy(g => g.PropertyType).Select(g => new { PropertyType = g.Key, RecordTypes = g.Count(), Uses = g.Count(r => r.IsUsed) }).ToList();

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
            foreach (var rpm in notImplemented)
            {
                if (state is null)
                    Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.RecordTypes}");
                else
                    Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.Uses}/{rpm.RecordTypes}");
            }

            /*
             * Output implemented to file as RPM code to paste into RecordPropertyMappings.cs
             */

            List<string[]> lines = [];
            foreach (var rpm in implemented)
            {
                if (rpm.Types.Count() >= 3)
                {
                    var mismatch = buildRPMs.FirstOrDefault(r => r.PropertyName.Equals(rpm.PropertyName, StringComparison.OrdinalIgnoreCase) && r.RPM.Action is null);

                    if (mismatch is null)
                    {
                        lines.Add(["Add(null", $"\"{rpm.PropertyName}\"", $"{rpm.RPM.GetClassName()}.Instance);"]);
                        continue;
                    }

                    Console.WriteLine($"Skipping wildcard RPM for {rpm.PropertyName} due to {mismatch.RTM.Name} - {mismatch.PropertyType.GetClassName()}");
                }

                var types = rpm.Types.ToList();
                types.Sort(static (l, r) => string.Compare(l.Name, r.Name, StringComparison.Ordinal));
                foreach (var type in types)
                    lines.Add([$"Add(typeof({type.Name})", $"\"{rpm.PropertyName}\"", $"{rpm.RPM.GetClassName()}.Instance);"]);
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

            printTableRow(lines, sep: ",", prefix: "            ", suffix: "", padLast: false);
            using var rpmFW = new StreamWriter(Path.Combine(Path.GetTempPath(), "GSP_RPM.txt"), false);
            rpmFW.Write(sw.ToString());
            rpmFW.Close();

            //_ = System.Diagnostics.Process.Start("explorer", $"\"{Path.Combine(Path.GetTempPath(), "GSPDoco.txt")}\"");
            //_ = System.Diagnostics.Process.Start("explorer", $"\"{Path.Combine(Path.GetTempPath(), "GSP_RPM.txt")}\"");
        }

        private static Type? calcRPMAction (Type type)
        {
            type = (type.GetIfGenericTypeDefinition() == typeof(Nullable<>)) ? type.GetIfUnderlyingType() ?? throw new Exception("WTF - This not meant to happen") : type;

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

        private static void checkAliases ()
        {
            var AllAliases = RecordPropertyMappings.AllAliases;
            var grouped = AllAliases.GroupBy(x => x.PropertyName).Where(g => g.Count() > 1).ToList();

            // Check for typed RPMs that match existing null typed Also remove any groups that
            // contain null typed after checking as no longer required
            foreach (var g in grouped.ToArray().Where(g => g.Any(x => x.Type == null)))
            {
                string nullRPM = g.First(x => x.Type == null).RealPropertyName;
                foreach (var gg in g.Where(x => x.Type != null && x.RealPropertyName == nullRPM))
                    Console.WriteLine($"{gg.Type?.GetClassName()}.{g.Key} = {nullRPM} is same as existing null type entry.");

                _ = grouped.Remove(g);
            }

            // Check for where all typed instances match so could be made a single nulled typed
            // Remove these groups after checking as no longer required
            foreach (var g in grouped.ToArray().Where(g => g.Select(x => x.RealPropertyName).Distinct().Count() == 1))
            {
                Console.WriteLine($"{g.Key} = {g.First().RealPropertyName} on all {g.Count()} aliases");
                _ = grouped.Remove(g);
            }

            // Check for if one alias mapping is dominant over others so could maybe create null
            // typed for that subset
            foreach (var g in grouped)
            {
                var gg = g.GroupBy(x => x.RealPropertyName).Select(x => new { RealPropertyName = x.Key, Count = x.Count(), Types = x.Select(y => y.Type?.GetClassName()) });
                int max = gg.Select(x => x.Count).Max();
                if (max > 1)
                {
                    var ggMax = gg.Where(x => x.Count == max);
                    if (ggMax.Count() == 1)
                    {
                        var gMax = ggMax.First();
                        Console.WriteLine($"{g.Key} = {gMax.RealPropertyName} on {gMax.Types.Count()} alias types ({string.Join(", ", gMax.Types)}).");
                    }
                }
            }
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
                    {
                        rpmDetails.RPM = rpm;
                        continue;
                    }

                    Console.WriteLine($"RPM mismatch on {rtm.Name}.{propertyFullName}");
                }

                if (parentName is null)
                {
                    if (property.PropertyType.GetIfGenericTypeDefinition() != typeof(ExtendedList<>) && !IgnoreDeepScan.Contains(propertyFullName) && !IgnoreDeepScan.Contains(property.Name))
                    {
                        var subProperties = processProperties(null, rtm, property.PropertyType, propertyFullName);
                        buildRPMs.AddRange(subProperties);
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
        }
    }
}