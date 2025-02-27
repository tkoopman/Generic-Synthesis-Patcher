using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using System.Reflection;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

using Noggog;

namespace GenericSynthesisPatcher
{
    internal static class GenerateDoco
    {
        private static readonly string[] IgnoreProperty = [
            "DATADataTypeState",
            "FormKey",
            "IsCompressed",
            "IsDeleted",
            "MajorRecordFlagsRaw",
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
            "Version2",
            "VersionControl",
            "VirtualMachineAdapter",
            ];

        private static readonly int[] PropCols = [0, 0, 0, 100, 100];
        private static StringWriter sw = new();

        internal static void generate ()
        {
            checkAliases();
            Console.WriteLine();

            List<string[]> lines = [];

            List<RecordTypeMapping> ImplementedRTMs = [];
            Dictionary<(Type, Type?), string> Implemented = [];
            List<(Type, string, string?, Type)> buildRPMs = [];

            // Output Implemented Properties per Record type
            foreach (var rtm in RecordTypeMappings.All)
            {
                // Loop all properties of setter looking for implemented properties
                List<string[]> Properties = [];

                var props = rtm.StaticRegistration.SetterType.GetPublicProperties().Where(p => ((p.CanRead && p.CanWrite) || p.PropertyType.GetIfGenericTypeDefinition() == typeof(ExtendedList<>)) && !IgnoreProperty.Contains(p.Name)).DistinctBy(p => p.Name);
                foreach (var prop in props)
                {
                    if (IgnoreProperty.Contains(prop.Name))
                        continue;

                    buildRPMs.Add(calcRPM(rtm, prop));

                    // Check for matching RPM
                    if (!RecordPropertyMappings.tryFindMapping(rtm.StaticRegistration.GetterType, prop.Name, out var rpm))
                    {
                        // Include Unimplemented
                        //Properties.Add([$"*{prop.Name}", "", "-----", prop.PropertyType.GetClassName(), ""]);
                        continue;
                    }

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
                            rpm.Action.CanMatch() ? 'M' : '-',
                            rpm.Action.CanFill() ? 'F' : '-',
                            rpm.Action.CanForward() ? 'F': '-',
                            rpm.Action.CanForwardSelfOnly() ? 'S': '-',
                            rpm.Action.CanMerge() ? 'M': '-',
                        ]);

                    var baseType = actionType.GetIfGenericTypeDefinition();

                    if (actionType == typeof(ConvertibleAction<string>))
                    {
                        desc = "String value";
                        exam = $"\"{rpm.PropertyName}\": \"Hello\"";
                    }
                    else if (actionType == typeof(ConvertibleAction<bool>))
                    {
                        desc = "True / False";
                        exam = $"\"{rpm.PropertyName}\": true";
                    }
                    else if (actionType == typeof(ConvertibleAction<float>))
                    {
                        desc = "Decimal value";
                        exam = $"\"{rpm.PropertyName}\": 3.14";
                    }
                    else if (baseType == typeof(ConvertibleAction<>))
                    {
                        desc = "Numeric value";
                        exam = $"\"{rpm.PropertyName}\": 7";
                    }
                    else if (baseType == typeof(Helpers.Action.FormLinkAction<>))
                    {
                        desc = "Form Key or Editor ID";
                        exam = "";
                    }
                    else if (baseType == typeof(FormLinksAction<>))
                    {
                        desc = "Form Keys or Editor IDs";
                        exam = "";
                    }
                    else if (actionType == typeof(BasicAction<Percent>))
                    {
                        desc = "Decimal value between 0.00 - 1.00, or string ending in %";
                        exam = $"\"{rpm.PropertyName}\": \"30.5%\"";
                    }
                    else if (actionType == typeof(BasicAction<Color>))
                    {
                        desc = "Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value (\"#0A0A0A0A\") or named color \"Blue\". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted.";
                        exam = $"\"{rpm.PropertyName}\": [40,50,60]";
                    }
                    else if (actionType == typeof(ObjectBoundsAction))
                    {
                        desc = "Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]";
                        exam = $"\"{rpm.PropertyName}\": [6,6,6,9,9,9]";
                    }
                    else if (actionType == typeof(FlagsAction))
                    {
                        var type = (prop.PropertyType.GetIfGenericTypeDefinition() == typeof(Nullable<>)) ? prop.PropertyType.GetIfUnderlyingType() ?? throw new Exception("WTF - This not meant to happen") : prop.PropertyType;

                        string[] flags = Enum.GetNames(type);
                        desc = $"Flags ({string.Join(", ", flags)})";
                        exam = (flags.Length > 1) ? $"\"{rpm.PropertyName}\": [ \"{flags.First()}\", \"-{flags.Last()}\" ]" : $"\"{rpm.PropertyName}\": \"{flags.First()}\"";
                    }
                    else if (actionType == typeof(EnumsAction))
                    {
                        var type = (prop.PropertyType.GetIfGenericTypeDefinition() == typeof(Nullable<>)) ? prop.PropertyType.GetIfUnderlyingType() ?? throw new Exception("WTF - This not meant to happen") : prop.PropertyType;
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
                    else if (actionType == typeof(LeveledItemAction))
                    {
                        desc = "Array of JSON objects containing Item Form Key/Editor ID and level/count data";
                        exam = $"\"{rpm.PropertyName}\": [{{ \"Item\": \"000ABC:Skyrim.esm\", \"Level\": 36, \"Count\": 1 }}]";
                    }
                    else if (actionType == typeof(LeveledNpcAction))
                    {
                        desc = "Array of JSON objects containing NPC Form Key/Editor ID and level/count data";
                        exam = $"\"{rpm.PropertyName}\": [{{ \"NPC\": \"000ABC:Skyrim.esm\", \"Level\": 36, \"Count\": 1 }}]";
                    }
                    else if (actionType == typeof(LeveledSpellAction))
                    {
                        desc = "Array of JSON objects containing Spell Form Key/Editor ID and level/count data";
                        exam = $"\"{rpm.PropertyName}\": [{{ \"Spell\": \"000ABC:Skyrim.esm\", \"Level\": 36, \"Count\": 1 }}]";
                    }
                    else if (actionType == typeof(RankPlacementAction))
                    {
                        desc = "JSON objects containing faction Form Key/Editor ID and Rank";
                        exam = $"\"{rpm.PropertyName}\": {{ \"Faction\": \"021FED:Skyrim.esm\", \"Rank\": 0 }}";
                    }
                    else if (actionType == typeof(RelationsAction))
                    {
                        desc = "JSON objects containing target Form Key/Editor ID, Reaction (Neutral, Enemy, Ally, Friend) and Modifier (Defaults to 0)";
                        exam = $"\"{rpm.PropertyName}\": {{ \"Target\": \"021FED:Skyrim.esm\", \"Reaction\": \"Friend\" }}";
                    }
                    else if (actionType == typeof(PlayerSkillsAction))
                    {
                        desc = "JSON object containing the values under PlayerSkills you want to set";
                        exam = "See ../Examples/NPC Player Skills.json";
                    }
                    else if (actionType == typeof(MemorySliceByteAction))
                    {
                        desc = "Memory slice in form of array of bytes";
                        exam = $"\"{rpm.PropertyName}\": [0x1A,0x00,0x3F]";
                    }
                    else if (actionType == typeof(WorldspaceMaxHeightAction))
                    {
                        desc = "Forward Worldspace Max Height data.";
                        exam = """[{ "types": ["Worldspace"], "ForwardOptions": ["HPU", "NonNull"], "Forward": { "MHDT": [] }}]""";
                    }
                    else if (actionType == typeof(CellMaxHeightDataAction))
                    {
                        desc = "Forward Cell Max Height data.";
                        exam = """[{ "types": ["Cell"], "ForwardOptions": ["HPU", "NonNull"], "Forward": { "MHDT": [] }}]""";
                    }

                    if (desc == null)
                        throw new Exception("Fix missing description");

                    //desc ??= "????";

                    Properties.Add([rpm.PropertyName, string.Join(';', names), MFFSM, desc, exam]);
                }

                // Output heading per record type
                if (Properties.Count > 0)
                {
                    ImplementedRTMs.Add(rtm);

                    sw.WriteLine();
                    string h = rtm.Name;
                    h += rtm.Name.Equals(rtm.FullName, StringComparison.Ordinal) ? "" : $" - {rtm.FullName}";
                    sw.WriteLine($"## {h}");
                    sw.WriteLine();

                    lines.Add(["Field", "Alt", "MFFSM", "Value Type", "Example"]);
                    lines.Add(["-", "-", "-", "-", "-"]);

                    // Output properties
                    Properties.Sort((l, r) => string.Compare(l[0], r[0], StringComparison.OrdinalIgnoreCase));
                    foreach (string[] row in Properties)
                        lines.Add(row);

                    printTableRow(lines);
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
                string anchor = rtm.Name.ToLower(CultureInfo.InvariantCulture);
                string fullName = "";
                if (!rtm.Name.Equals(rtm.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    fullName = rtm.FullName;
                    anchor += "---" + fullName.ToLower(CultureInfo.InvariantCulture);
                }

                lines.Add([$"[{rtm.Name}](Fields.md#{anchor})", fullName]);
            }

            printTableRow(lines);
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

            var implemented = groupRPMs.Where(g => g.RPM != null);

            //var notImplemented = groupRPMs.Where(g => g.RPM == null).ToList();
            var notImplemented = buildRPMs.Where(r => r.Item3 == null).GroupBy(g => g.Item4).Select(g => new { PropertyType = g.Key, RecordTypes = g.Select(r => $"{r.Item1.GetClassName()}.{r.Item2}") }).ToList();

            // Sort by uses then name
            notImplemented.Sort((l, r) =>
            {
                int i = l.RecordTypes.Count().CompareTo(r.RecordTypes.Count());
                if (i == 0)
                    i = string.Compare(l.PropertyType.GetClassName(), r.PropertyType.GetClassName(), StringComparison.Ordinal);

                return i;
            });

            // Print Unimplemented Entries to Screen
            foreach (var rpm in notImplemented)
                Console.WriteLine($"{rpm.PropertyType.GetClassName()}, {rpm.RecordTypes.Count()}");

            /*
             * Output implemented to file as RPM code to paste into RecordPropertyMappings.cs
             */

            lines = [];
            foreach (var rpm in implemented)
            {
                if (rpm.Types.Count() < 3)
                {
                    var types = rpm.Types.ToList();
                    types.Sort(static (l, r) => string.Compare(l.Name, r.Name, StringComparison.Ordinal));
                    foreach (var type in types)
                        lines.Add([$"Add(typeof({type.Name})", $"\"{rpm.PropertyName}\"", $"{rpm.RPM}.Instance);"]);
                }
                else
                {
                    lines.Add(["Add(null", $"\"{rpm.PropertyName}\"", $"{rpm.RPM}.Instance);"]);
                }
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

        private static (Type type, string name, string? iAction, Type propertyType) calcRPM (RecordTypeMapping rtm, PropertyInfo propertyInfo)
        {
            string name = propertyInfo.Name;
            string? actionClass = null;
            var type = (propertyInfo.PropertyType.GetIfGenericTypeDefinition() == typeof(Nullable<>)) ? propertyInfo.PropertyType.GetIfUnderlyingType() ??  throw new Exception("WTF - This not meant to happen") : propertyInfo.PropertyType;

            var mainType = type.GetIfGenericTypeDefinition();
            var subType = type.GetIfUnderlyingType()?.GetIfGenericTypeDefinition();
            var subSubType = type.GetIfUnderlyingType()?.GetIfUnderlyingType();

            if (type.IsEnum)
            {
                actionClass = type.GetCustomAttributes(typeof(FlagsAttribute), true).FirstOrDefault() == null ? nameof(EnumsAction) : nameof(FlagsAction);
            }
            else if (type.IsAssignableTo(typeof(ITranslatedStringGetter)))
            {
                actionClass = $"{nameof(ConvertibleAction<string>)}<string>";
            }
            else if (type.IsAssignableTo(typeof(IConvertible)) && (type.IsPrimitive || type == typeof(string)))
            {
                string? n = type.Name switch
                {
                    nameof(Boolean) => "bool",
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
                    actionClass = $"{nameof(ConvertibleAction<byte>)}<{n}>";
            }
            else if (type == typeof(Percent))
            {
                actionClass = typeof(BasicAction<Percent>).GetClassName();
            }
            else if (type == typeof(Color))
            {
                actionClass = typeof(BasicAction<Color>).GetClassName();
            }
            else if (type.IsAssignableTo(typeof(IObjectBoundsGetter)))
            {
                actionClass = typeof(ObjectBoundsAction).GetClassName();
            }
            else if (type.IsAssignableTo(typeof(IPlayerSkillsGetter)))
            {
                actionClass = typeof(PlayerSkillsAction).GetClassName();
            }
            else if (type == typeof(WorldspaceMaxHeight))
            {
                actionClass = typeof(WorldspaceMaxHeightAction).GetClassName();
            }
            else if (type == typeof(CellMaxHeightData))
            {
                actionClass = typeof(CellMaxHeightDataAction).GetClassName();
            }
            else if (type == typeof(MemorySlice<byte>))
            {
                actionClass = typeof(MemorySliceByteAction).GetClassName();
            }
            else if (subType != null)
            {
                if (mainType.IsAssignableTo(typeof(IFormLink<>)) || mainType.IsAssignableTo(typeof(IFormLinkNullable<>)))
                {
                    actionClass = $"{nameof(FormLinkAction<IActionRecordGetter>)}<{subType.Name}>";
                }
                else if (mainType.IsAssignableTo(typeof(ExtendedList<>)))
                {
                    if (subType.IsAssignableTo(typeof(IFormLinkGetter<>)) && subSubType != null)
                        actionClass = $"{nameof(FormLinksAction<IActionRecordGetter>)}<{subSubType.Name}>";
                    else if (subType.IsAssignableTo(typeof(IContainerEntryGetter)))
                        actionClass = nameof(ContainerItemsAction);
                    else if (subType.IsAssignableTo(typeof(IEffectGetter)))
                        actionClass = nameof(EffectsAction);
                    else if (subType.IsAssignableTo(typeof(ILeveledItemEntryGetter)))
                        actionClass = nameof(LeveledItemAction);
                    else if (subType.IsAssignableTo(typeof(ILeveledNpcEntryGetter)))
                        actionClass = nameof(LeveledNpcAction);
                    else if (subType.IsAssignableTo(typeof(ILeveledSpellEntryGetter)))
                        actionClass = nameof(LeveledSpellAction);
                    else if (subType.IsAssignableTo(typeof(IRankPlacementGetter)))
                        actionClass = nameof(RankPlacementAction);
                    else if (subType.IsAssignableTo(typeof(IRelationGetter)))
                        actionClass = nameof(RelationsAction);
                }
            }

            return (rtm.StaticRegistration.GetterType, name, actionClass, type);
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