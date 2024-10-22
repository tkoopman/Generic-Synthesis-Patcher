using System.Reflection;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Data;

using Mutagen.Bethesda.Skyrim;

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
            "UnknownGroupData",
            "Unused",
            "VirtualMachineAdapter",
            ];

        private static readonly int[] NICols = [14, 30, 30, 30, 40];
        private static readonly int[] PropCols = [30, 14, 5, 30, 30];
        private static readonly int[] TypesCols = [4, 14];

        internal static void Generate ()
        {
            // Get list of implemented record types and any aliases for them
            Dictionary<RecordTypes, List<string>> recordTypes = [];

            foreach (string rt in Enum.GetNames<RecordTypes>())
            {
                var r = Enum.Parse<RecordTypes>(rt);
                if (recordTypes.TryGetValue(r, out var value))
                    value.Add(rt);
                else
                    recordTypes.Add(r, [rt]);
            }

            var keys = recordTypes.Keys.ToList();
            _ = keys.Remove(RecordTypes.NONE);
            _ = keys.Remove(RecordTypes.All);
            keys.Sort();

            // Output Implemented Types
            PrintTableRow(["Type", "Synonyms"], TypesCols);
            PrintTableRow(["", ""], TypesCols, '-');
            foreach (var k in keys)
            {
                // Sort so Length 4, which most internal names are, will be first in the list
                recordTypes[k].Sort(( a, b ) => a.Length == b.Length ? 0 : a.Length == 4 ? -1 : b.Length == 4 ? 1 : a.Length.CompareTo(b.Length));
                string t = recordTypes[k].First();
                string s = (recordTypes[k].Count > 1)?recordTypes[k].Last():"";
                PrintTableRow([t, s], TypesCols);
            }

            Dictionary<RecordTypes, List<PropertyInfo>> NotImplemented = [];
            Dictionary<(Type, Type?), string> Implemented = [];

            // Output Implemented Properties per Record type
            foreach (var k in keys)
            {
                NotImplemented.Add(k, []);

                // Map record type to classes
                var rego = k switch
                {
                    RecordTypes.ALCH => IIngestibleGetter.StaticRegistration,
                    RecordTypes.AMMO => IAmmunitionGetter.StaticRegistration,
                    RecordTypes.ARMO => IArmorGetter.StaticRegistration,
                    RecordTypes.BOOK => IBookGetter.StaticRegistration,
                    RecordTypes.CELL => ICellGetter.StaticRegistration,
                    RecordTypes.CONT => IContainerGetter.StaticRegistration,
                    RecordTypes.FACT => IFactionGetter.StaticRegistration,
                    RecordTypes.INGR => IIngredientGetter.StaticRegistration,
                    RecordTypes.KEYM => IKeyGetter.StaticRegistration,
                    RecordTypes.MISC => IMiscItemGetter.StaticRegistration,
                    RecordTypes.NPC  => INpcGetter.StaticRegistration,
                    RecordTypes.OTFT => IOutfitGetter.StaticRegistration,
                    RecordTypes.SCRL => IScrollGetter.StaticRegistration,
                    RecordTypes.WEAP => IWeaponGetter.StaticRegistration,
                    RecordTypes.WRLD => IWorldspaceGetter.StaticRegistration,
                    _ => throw new Exception("Missing RecordType")
                };

                // Loop all properties of setter looking for implemented properties
                List<string[]> Properties = [];
                foreach (var prop in rego.GetterType.GetProperties())
                {
                    if (IgnoreProperty.Contains(prop.Name))
                        continue;

                    // Check for matching RCD
                    var rcds = RCDMapping.RecordCallDataMapping.Where(r => r.Value.PropertyName.Equals(prop.Name) && (r.Key.RecordType == null || r.Key.RecordType.IsAssignableFrom(rego.GetterType))).ToList();
                    if (rcds.Count == 0)
                    {
                        NotImplemented[k].Add(prop);
                        continue;
                    }

                    var pt = prop.PropertyType;
                    Type? pst = null;
                    if (pt.IsGenericType)
                    {
                        pst = pt.GenericTypeArguments[0];
                        pt = pt.GetGenericTypeDefinition();
                    }
                    _ = Implemented.TryAdd((pt, pst), $"{rego.GetterType.Name}.{prop.Name}");

                    //rcds.Sort(( l, r ) => l.Key.JsonKey.Length.CompareTo(r.Key.JsonKey.Length) * -1);
                    RecordCallData? rcd = null;
                    List<string> names = [];
                    foreach (var r in rcds)
                    {
                        if (rcd != null && rcd != r.Value)
                            throw new Exception("Not good");
                        rcd ??= r.Value;
                        names.Add(r.Key.JsonKey);
                    }
                    names = names.Distinct().ToList();
                    names.Sort(( l, r ) =>
                    {
                        if (l.Equals(prop.Name))
                            return -1;
                        if (r.Equals(prop.Name))
                            return 1;

                        return string.Compare(l, r, StringComparison.OrdinalIgnoreCase);
                    });
                    if (rcd == null)
                        throw new Exception("WTF");

                    // Get the RCD IAction type
                    var rcdType = rcd.GetType().GenericTypeArguments[0];
                    string? desc = null;
                    string exam = "";
                    string MFFSM = string.Join<char>(string.Empty,
                        [
                            'M',
                            rcd.CanFill() ? 'F' : '-',
                            rcd.CanForward() ? 'F': '-',
                            rcd.CanForwardSelfOnly() ? 'S': '-',
                            rcd.CanMerge() ? 'M': '-',
                        ]);

                    if (rcdType.IsGenericType)
                    {
                        // If IAction generic get it's implemented type
                        var rcdGeneric = rcdType.GetGenericTypeDefinition();
                        var rcdSubType = rcdType.GenericTypeArguments[0];

                        if (rcdGeneric == typeof(Generic<>))
                        {
                            if (rcdSubType == typeof(float))
                            {
                                desc = "A decimal value";
                                exam = $"\"{names[0]}\": 3.14";
                            }
                            else if (rcdType.IsAssignableTo(typeof(Generic<string>)))
                            {
                                desc = "A string value";
                                exam = $"\"{names[0]}\": \"Hello\"";
                            }
                            else
                            {
                                desc = "A numeric value";
                                exam = $"\"{names[0]}\": 7";
                            }
                        }
                        else if (rcdGeneric == typeof(FormLink<>))
                        {
                            desc = "Form Key or Editor ID";
                            exam = "";
                        }
                        else if (rcdGeneric == typeof(FormLinks<>))
                        {
                            desc = "Form Keys or Editor IDs";
                            exam = "";
                        }
                        else if (rcdGeneric == typeof(FormLinksWithData<>) || rcdGeneric == typeof(FormLinksWithData<,>))
                        {
                            if (rcdSubType == typeof(ContainerItemsAction))
                            {
                                desc = "JSON objects containing item Form Key/Editor ID and Count (QTY)";
                                exam = $"\"{names[0]}\": {{ \"Item\": \"021FED:Skyrim.esm\", \"Count\": 3 }}";
                            }
                            else if (rcdSubType == typeof(EffectsAction))
                            {
                                desc = "JSON objects containing effect Form Key/Editor ID and effect data";
                                exam = $"\"{names[0]}\": {{ \"Effect\": \"021FED:Skyrim.esm\", \"Area\": 3, \"Duration\": 3, \"Magnitude\": 3 }}";
                            }
                        }
                    }
                    else if (rcdType.IsAssignableTo(typeof(Flags)))
                    {
                        string[] flags = Enum.GetNames(rego.GetterType.GetProperty(rcd.PropertyName)?.PropertyType ?? throw new Exception("Failed to get flags"));
                        desc = $"Flags ({string.Join(", ", flags)})";
                        exam = (flags.Length > 1) ? $"\"{names[0]}\": [ \"{flags.First()}\", \"-{flags.Last()}\" ]" : $"\"{names[0]}\": \"{flags.First()}\"";
                    }
                    else if (rcdType.IsAssignableTo(typeof(Enums)))
                    {
                        string[] values = Enum.GetNames(rego.GetterType.GetProperty(rcd.PropertyName)?.PropertyType ?? throw new Exception("Failed to get Enum values"));
                        desc = $"Possible values ({string.Join(", ", values)})";
                        exam = $"\"{names[0]}\": \"{values.First()}\"";
                    }

                    if (desc == null)
                        throw new Exception("Fix Missing Doco");

                    Properties.Add([names[0], string.Join(';', names[1..]), MFFSM, desc, exam]);
                }

                // Output heading per record type
                Console.WriteLine();
                string h = recordTypes[k].First();
                h += (recordTypes[k].Count > 1) ? $" - {recordTypes[k].Last()}" : "";
                Console.WriteLine($"## {h}");
                Console.WriteLine();
                PrintTableRow(["Field", "Alt", "MFFSM", "Value Type", "Example"], PropCols);
                PrintTableRow(["", "", "", "", ""], PropCols, '-');

                // Output properties
                Properties.Sort(( l, r ) => string.Compare(l[0], r[0], StringComparison.OrdinalIgnoreCase));
                foreach (string[] row in Properties)
                    PrintTableRow(row, PropCols);
            }

            // Output not implemented properties
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("### Not Implemented Properties");
            Console.WriteLine();

            PrintTableRow(["Record", "Property", "Type", "Sub Type", "Easy?"], NICols);
            PrintTableRow(["", "", "", "", ""], NICols, '-');
            foreach ((var k, var v) in NotImplemented.Where(k2 => k2.Value.Count > 0))
            {
                foreach (var p in v)
                {
                    var pt = p.PropertyType;
                    Type? pst = null;
                    if (pt.IsGenericType)
                    {
                        pst = pt.GenericTypeArguments[0];
                        pt = pt.GetGenericTypeDefinition();
                    }
                    string easy = "";
                    if (Implemented.TryGetValue((pt, pst), out string? s))
                        easy = s;
                    else if (pt.IsEnum)
                        easy = "Enum";

                    PrintTableRow([recordTypes[k].First(), p.Name, pt.Name, pst?.Name ?? "", easy], NICols);
                }
            }
        }

        private static void PrintTableRow ( string[] values, int[] widths, char padding = ' ' )
        {
            if (values.Length != widths.Length)
                throw new Exception("Get it together!");

            Console.Write('|');
            for (int i = 0; i < widths.Length; i++)
                Console.Write($" {values[i].PadRight(widths[i], padding)} |");
            Console.WriteLine();
        }
    }
}