using System.Collections.Immutable;

using Common;

using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher
{
    public sealed class GenerateDoco (IPatcherState state, BaseGenny genny)
    {
        private static readonly int[] PropCols = [0, 0, 0, 100, 100];
        private readonly IPatcherState? State = state;

        public static void generate (IPatcherState<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter> state)
        {
            var genny = new GenerateDoco(state, new Helpers.Skyrim.SkyrimGenny());
            genny.Run(false);
        }

        public static void generateUnused (IPatcherState<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter> state)
        {
            var genny = new GenerateDoco(state, new Helpers.Skyrim.SkyrimGenny());
            genny.Run(true);
        }

        public void Run (bool outputUnimplemented)
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
            };

            if (State is not null)
                Global.SetState(State);

            List<RecordTypeMapping> ImplementedRTMs = [];
            List<RPMDetails> buildRPMs = [];
            List<string[]> subPropertyAliases = [];

            // Output Implemented Properties per Record type
            foreach (var rtm in Global.RecordTypeMappings.All)
            {
                // Loop all properties of setter looking for implemented properties
                var properties = processProperties(rtm, rtm.StaticRegistration.SetterType, null, outputUnimplemented);

                buildRPMs.AddRange(properties);

                // Output heading per record type
                if ((outputUnimplemented && properties.Count > 0) || properties.Any(p => p.RPM.Action is not null && p.RecordActionInterface is not null))
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
                        else if (outputUnimplemented)
                        {
                            row.Description = $"Not implemented type: {row.PropertyType.GetClassName()}";
                        }
                    }

                    subPropertyAliases.AddRange(generateSubPropertyAliases(properties));

                    /*
                     * Output Implemented Properties
                     */

                    using (var sw = new StreamWriter($"{rtm.Name.ToLowerInvariant()}.json"))
                    using (var writer = new JsonTextWriter(sw))
                    {
                        if (outputUnimplemented)
                            serializer.Serialize(writer, properties);
                        else
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

            using (StringWriter sw = new())
            {
                sw.WriteLine(genny.RPMPopulateHeader.Replace("populateMappings", "populateSubAliases"));
                printTableRow(sw, subPropertyAliases, sep: ",", prefix: "            ", suffix: "", padLast: false);
                sw.WriteLine(genny.RPMPopulateFooter);

                using var aliasesFW = new StreamWriter(Path.Combine(Path.GetTempPath(), "GSP_Aliases.txt"), false);
                aliasesFW.Write(sw.ToString());
                aliasesFW.Close();
            }

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
                if (State is null)
                    Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.RecordTypes}");
                else
                    Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.Uses}/{rpm.RecordTypes}");
            }

            Console.WriteLine();
            Console.WriteLine("List of unimplemented types.");
            foreach (var rpm in notImplemented.Where(i => !(i.SubPropertiesMin > 0 && i.SubPropertiesMax == i.SubPropertiesMin)))
            {
                if (State is null)
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

            using (StringWriter sw = new())
            {
                ;
                sw.WriteLine(genny.RPMPopulateHeader);
                printTableRow(sw, lines, sep: ",", prefix: "            ", suffix: "", padLast: false);
                sw.WriteLine(genny.RPMPopulateFooter);

                using var rpmFW = new StreamWriter(Path.Combine(Path.GetTempPath(), "GSP_RPM.txt"), false);
                rpmFW.Write(sw.ToString());
                rpmFW.Close();
            }

            //_ = System.Diagnostics.Process.Start("explorer", $"\"{Path.Combine(Path.GetTempPath(), "GSP_RPM.txt")}\"");
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
                if (Global.RecordPropertyMappings.GetNullAliases(property.PropertyName).Any())
                    continue;

                string name = property.PropertyName[..property.PropertyName.IndexOf('.')];
                if (!name.Equals(lastName, StringComparison.Ordinal))
                {
                    lastName = name;
                    lastAlias = Global.RecordPropertyMappings.GetAllAliases(lastType, name).FirstOrDefault();
                }

                if (lastAlias is not null)
                    lines.Add([$"AddAlias(typeof({lastType.Name})", $"\"{lastAlias}{property.PropertyName[property.PropertyName.IndexOf('.')..]}\"", $"\"{property.PropertyName}\");"]);
            }

            return lines;
        }

        private static void printTableRow (StringWriter sw, List<string[]> lines, int maxPaddedWidth = 250, char padding = ' ', string sep = " |", string? prefix = "| ", string? suffix = " |", bool padLast = true)
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

        private List<RPMDetails> processProperties (RecordTypeMapping rtm, Type parentType, string? parentName, bool OutputUnimplemented)
        {
            List<RPMDetails> buildRPMs = [];

            var properties = parentType.GetPublicProperties().Where(p => (p.CanRead && p.CanWrite) || p.PropertyType.GetIfGenericTypeDefinition() == typeof(ExtendedList<>)).DistinctBy(p => p.Name);
            foreach (var property in properties)
            {
                string propertyFullName = parentName is null ? property.Name : $"{parentName}.{property.Name}";
                if (property.GetGetMethod()?.GetParameters().Length != 0)
                    continue;
                if (genny.IgnoreProperty.Contains(propertyFullName) || genny.IgnoreProperty.Contains(property.Name))
                    continue;

                var rpmDetails = new RPMDetails (rtm, propertyFullName, property.PropertyType);
                buildRPMs.Add(rpmDetails);

                rpmDetails.RecordActionInterface = genny.GetRPMAction(property.PropertyType);
                rpmDetails.IsUsed = rpmDetails.RecordActionInterface is not null;

                if (Global.RecordPropertyMappings.tryFindMapping(rtm.StaticRegistration.GetterType, propertyFullName, out var rpm))
                {
                    // record property mapping (RPM) was found
                    if (rpm.Action.GetType().Equals(rpmDetails.RecordActionInterface))
                        rpmDetails.RPM = rpm;
                    else
                        Console.WriteLine($"RPM mismatch on {rtm.Name}.{propertyFullName}");
                }

                if (parentName is null || genny.ForceDeeperTypes.Contains(property.PropertyType.RemoveNullable().GetIfGenericTypeDefinition()))
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

                    if (type is not null && !genny.IgnoreDeepScanOnTypes.Contains(type.GetIfGenericTypeDefinition()))
                    {
                        var subProperties = processProperties(rtm, type, propertyFullName, false);
                        buildRPMs.AddRange(subProperties);
                        rpmDetails.SubProperties = subProperties.Count;
                    }

                    if (OutputUnimplemented)
                    {
                        // Check if property used (On at least 1 record has a non-null, non-default
                        // or list count > 0)
                        FormKey? used = null;
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

                    var names = Global.RecordPropertyMappings.GetAllAliases(RTM.StaticRegistration.GetterType, PropertyName).ToList();
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