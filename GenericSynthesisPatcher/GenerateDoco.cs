using System.Collections.Immutable;
using System.Reflection;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

namespace GenericSynthesisPatcher
{
    internal static class GenerateDoco
    {
        private static readonly string[] IgnoreProperty = [
            "DATADataTypeState",
            "PersistentTimestamp",
            "PersistentUnknownGroupData",
            "StaticRegistration",
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
            "VirtualMachineAdapter",
            ];

        private static readonly int[] PropCols = [0, 0, 0, 100, 100];
        private static StringWriter sw = new();

        internal static (Type type, string name, string? iAction, Type propertyType) CalcRPM (RecordTypeMapping rtm, PropertyInfo propertyInfo)
        {
            string name = propertyInfo.Name;
            string? actionClass = null;
            var type = (propertyInfo.PropertyType.GetIfGenericTypeDefinition() == typeof(Nullable<>)) ? propertyInfo.PropertyType.GetIfUnderlyingType() ?? propertyInfo.PropertyType : propertyInfo.PropertyType;

            var mainType = type.GetIfGenericTypeDefinition();
            var subType = type.GetIfUnderlyingType()?.GetIfGenericTypeDefinition();
            var subSubType = type.GetIfUnderlyingType()?.GetIfUnderlyingType();

            if (type.IsEnum)
            {
                actionClass = type.GetCustomAttributes(typeof(FlagsAttribute), true).FirstOrDefault() == null ? "Enums" : "Flags";
            }
            else if (type.IsAssignableTo(typeof(ITranslatedStringGetter)))
                actionClass = $"Generic<string>";
            else if (type.IsAssignableTo(typeof(IConvertible)) && (type.IsPrimitive || type == typeof(string)))
            {
                string? n = type.Name switch
                {
                    nameof(Boolean) => null,
                    nameof(Byte) => "byte",
                    nameof(Char) => "char",
                    nameof(Int16) => "short",
                    nameof(Int32) => "int",
                    nameof(SByte) => "sbyte",
                    nameof(Single) => "float",
                    nameof(String) => "string",
                    nameof(UInt16) => "ushort",
                    nameof(UInt32) => "uint",
                    _ => type.Name
                };
                if (n != null)
                    actionClass = $"Generic<{n}>";
            }
            else if (subType != null)
            {
                if (mainType.IsAssignableTo(typeof(IFormLinkGetter<>)) || mainType.IsAssignableTo(typeof(IFormLinkNullableGetter<>)))
                    actionClass = $"FormLink<{subType.Name}>";
                else if (mainType.IsAssignableTo(typeof(IReadOnlyList<>)))
                {
                    if (subType.IsAssignableTo(typeof(IFormLinkGetter<>)) && subSubType != null)
                        actionClass = $"FormLinks<{subSubType.Name}>";
                    else if (subType == typeof(IContainerEntryGetter))
                        actionClass = "FormLinksWithData<ContainerItemsAction, IItemGetter>";
                    else if (subType == typeof(IEffectGetter))
                        actionClass = "FormLinksWithData<EffectsAction, IMagicEffectGetter>";
                }
            }

            return (rtm.StaticRegistration.GetterType, name, actionClass, type);
        }

        internal static void Generate ()
        {
            List<string[]> lines = [];

            List<RecordTypeMapping> ImplementedRTMs = [];
            Dictionary<(Type, Type?), string> Implemented = [];
            List<(Type, string, string?, Type)> buildRPMs = [];

            // Output Implemented Properties per Record type
            foreach (var rtm in RecordTypeMappings.All)
            {
                // Loop all properties of setter looking for implemented properties
                List<string[]> Properties = [];
                foreach (var prop in rtm.StaticRegistration.GetterType.GetProperties())
                {
                    if (IgnoreProperty.Contains(prop.Name))
                        continue;

                    buildRPMs.Add(CalcRPM(rtm, prop));

                    // Check for matching RCD
                    if (!RecordPropertyMappings.TryFindMapping(rtm.StaticRegistration.GetterType, prop.Name, out var rpm))
                        continue;

                    var pt = prop.PropertyType;
                    Type? pst = null;
                    if (pt.IsGenericType)
                    {
                        pst = pt.GenericTypeArguments[0];
                        pt = pt.GetGenericTypeDefinition();
                    }

                    _ = Implemented.TryAdd((pt, pst), $"{rtm.StaticRegistration.GetterType.Name}.{prop.Name}");

                    var names = RecordPropertyMappings.GetAllAliases(rtm.StaticRegistration.GetterType, prop.Name).ToList();
                    names.Sort();

                    // Get the RCD IAction type
                    var actionType = rpm.Action.GetType();
                    string? desc = null;
                    string exam = "";
                    string MFFSM = string.Join<char>(string.Empty,
                        [
                            'M',
                            rpm.Action.CanFill() ? 'F' : '-',
                            rpm.Action.CanForward() ? 'F': '-',
                            rpm.Action.CanForwardSelfOnly() ? 'S': '-',
                            rpm.Action.CanMerge() ? 'M': '-',
                        ]);

                    if (actionType.IsGenericType)
                    {
                        // If IAction generic get it's implemented type
                        var rcdGeneric = actionType.GetGenericTypeDefinition();
                        var rcdSubType = actionType.GenericTypeArguments[0];

                        if (rcdGeneric == typeof(Generic<>))
                        {
                            if (rcdSubType == typeof(float))
                            {
                                desc = "A decimal value";
                                exam = $"\"{rpm.PropertyName}\": 3.14";
                            }
                            else if (actionType.IsAssignableTo(typeof(Generic<string>)))
                            {
                                desc = "A string value";
                                exam = $"\"{rpm.PropertyName}\": \"Hello\"";
                            }
                            else
                            {
                                desc = "A numeric value";
                                exam = $"\"{rpm.PropertyName}\": 7";
                            }
                        }
                        else if (rcdGeneric == typeof(Helpers.Action.FormLink<>))
                        {
                            desc = "Form Key or Editor ID";
                            exam = "";
                        }
                        else if (rcdGeneric == typeof(FormLinks<>))
                        {
                            desc = "Form Keys or Editor IDs";
                            exam = "";
                        }
                    }
                    else if (actionType.IsAssignableTo(typeof(Flags)))
                    {
                        var propertyInfo = rtm.StaticRegistration.GetterType.GetProperty(rpm.PropertyName) ?? throw new Exception("Weird");
                        var type = (propertyInfo.PropertyType.GetIfGenericTypeDefinition() == typeof(Nullable<>)) ? propertyInfo.PropertyType.GetIfUnderlyingType() ?? propertyInfo.PropertyType : propertyInfo.PropertyType;

                        string[] flags = Enum.GetNames(type ?? throw new Exception($"Failed to get flags - {rtm.Name} - {rpm.PropertyName}"));
                        desc = $"Flags ({string.Join(", ", flags)})";
                        exam = (flags.Length > 1) ? $"\"{rpm.PropertyName}\": [ \"{flags.First()}\", \"-{flags.Last()}\" ]" : $"\"{rpm.PropertyName}\": \"{flags.First()}\"";
                    }
                    else if (actionType.IsAssignableTo(typeof(Enums)))
                    {
                        var propertyInfo = rtm.StaticRegistration.GetterType.GetProperty(rpm.PropertyName) ?? throw new Exception("Weird");
                        var type = (propertyInfo.PropertyType.GetIfGenericTypeDefinition() == typeof(Nullable<>)) ? propertyInfo.PropertyType.GetIfUnderlyingType() ?? propertyInfo.PropertyType : propertyInfo.PropertyType;
                        string[] values = Enum.GetNames(type);
                        desc = $"Possible values ({string.Join(", ", values)})";
                        exam = $"\"{rpm.PropertyName}\": \"{values.First()}\"";
                    }
                    else if (actionType == typeof(ContainerItemsAction))
                    {
                        desc = "JSON objects containing item Form Key/Editor ID and Count (QTY)";
                        exam = $"\"{rpm.PropertyName}\": {{ \"Item\": \"021FED:Skyrim.esm\", \"Count\": 3 }}";
                    }
                    else if (actionType == typeof(EffectsAction))
                    {
                        desc = "JSON objects containing effect Form Key/Editor ID and effect data";
                        exam = $"\"{rpm.PropertyName}\": {{ \"Effect\": \"021FED:Skyrim.esm\", \"Area\": 3, \"Duration\": 3, \"Magnitude\": 3 }}";
                    }

                    if (desc == null)
                        throw new Exception("Fix Missing Doco");

                    Properties.Add([rpm.PropertyName, string.Join(';', names), MFFSM, desc, exam]);
                }

                // Output heading per record type
                if (Properties.Count > 0)
                {
                    ImplementedRTMs.Add(rtm);

                    sw.WriteLine();
                    string h = rtm.Name;
                    h += rtm.Name.Equals(rtm.FullName) ? "" : $" - {rtm.FullName}";
                    sw.WriteLine($"## {h}");
                    sw.WriteLine();

                    lines.Add(["Field", "Alt", "MFFSM", "Value Type", "Example"]);
                    lines.Add(["-", "-", "-", "-", "-"]);

                    // Output properties
                    Properties.Sort((l, r) => string.Compare(l[0], r[0], StringComparison.OrdinalIgnoreCase));
                    foreach (string[] row in Properties)
                        lines.Add(row);

                    PrintTableRow(lines);
                    lines = [];

                    // Footer
                    sw.WriteLine();
                    sw.WriteLine("[⬅ Back to Types](Types.md)");
                }
            }

            /*
             * Output Implemented Types
             */

            sw.WriteLine();
            lines.Add(["Type", "Synonyms"]);
            lines.Add(["-", "-"]);
            foreach (var rtm in ImplementedRTMs)
            {
                string anchor = rtm.Name.ToLower();
                string fullName = "";
                if (!rtm.Name.Equals(rtm.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    fullName = rtm.FullName;
                    anchor += "---" + fullName.ToLower();
                }

                lines.Add([$"[{rtm.Name}](Fields.md#{anchor})", fullName]);
            }

            PrintTableRow(lines);
            lines = [];

            // Write Types and Implemented Properties to Temp File
            using var fw = new StreamWriter(Path.Combine(Path.GetTempPath(), "GSPDoco.txt"), false);
            fw.Write(sw.ToString());
            fw.Close();

            /*
             * Output RPM code for all identified property types
             */

            var groupRPMs = buildRPMs.GroupBy(g => (g.Item2, g.Item3),
                                              g => (g.Item1, g.Item4), (k, data) => new { PropertyName = k.Item1, RPM = k.Item2, Types = data.Select(d => d.Item1), PropertyTypes = data.Select(d => d.Item2).Distinct()});

            var implemented = groupRPMs.Where(g => g.RPM != null).ToList();
            implemented.Sort((l, r) => string.CompareOrdinal(l.PropertyName, r.PropertyName));

            var notImplemented = groupRPMs.Where(g => g.RPM == null).ToList();
            notImplemented.Sort((l, r) => l.Types.Count().CompareTo(r.Types.Count()));

            // Print Unimplemented Entries to Screen
            foreach (var rpm in notImplemented)
                Console.WriteLine($"{rpm.PropertyName}, {rpm.Types.Count()}, {string.Join('|', rpm.PropertyTypes.Select(p => p.GetClassName()))}");

            lines = [];
            foreach (var rpm in implemented)
            {
                if (rpm.Types.Count() < 3)
                {
                    foreach (var type in rpm.Types)
                        lines.Add([$"Add(typeof({type.Name})", $"\"{rpm.PropertyName}\"", $"{rpm.RPM}.Instance);"]);
                }
                else
                {
                    lines.Add([$"Add(null", $"\"{rpm.PropertyName}\"", $"{rpm.RPM}.Instance);"]);
                }
            }

            sw = new();

            PrintTableRow(lines, sep: ", ", prefix: "            ", suffix: "", padLast: false);
            using var rpmFW = new StreamWriter(Path.Combine(Path.GetTempPath(), "GSP_RPM.txt"), false);
            rpmFW.Write(sw.ToString());
            rpmFW.Close();

            _ = System.Diagnostics.Process.Start("explorer", $"\"{Path.Combine(Path.GetTempPath(), "GSPDoco.txt")}\"");
            _ = System.Diagnostics.Process.Start("explorer", $"\"{Path.Combine(Path.GetTempPath(), "GSP_RPM.txt")}\"");
        }

        private static void PrintTableRow (List<string[]> lines, int maxPaddedWidth = 250, char padding = ' ', string sep = " |", string? prefix = "| ", string? suffix = " |", bool padLast = true)
        {
            if (lines.Count == 0 || lines.Any(v => v.Length != lines[0].Length))
                throw new Exception("Nope! Try again");

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
    }
}