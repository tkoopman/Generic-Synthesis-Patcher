using System.Collections.Frozen;
using System.Data;
using System.Text.RegularExpressions;

using EnumsNET;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

using Noggog;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher
{
    public partial class Program
    {
        private const int ClassLogPrefix = 0x000;
        private static RecordTypes EnabledTypes;

        public static int FillRecord ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, ValueKey valueKey )
        {
            if (context.Record is not IMajorRecordGetter)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Trace, context, "", typeof(IMajorRecordGetter).Name, context.Record.GetType().Name, ClassLogPrefix | 0x11);
                return -1;
            }

            var rcd = RCDMapping.FindRecordCallData(context, valueKey.Key);
            if (rcd == null || !rcd.CanFill())
            {
                LogHelper.Log(LogLevel.Trace, context, $"Unknown / Unimplemented field for fill action: {valueKey.Key}", ClassLogPrefix | 0x12);
                return -1;
            }

            if (rule.OnlyIfDefault && !rcd.Matches(context.Record, origin, rcd))
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0xFE);
                return -1;
            }

            ISkyrimMajorRecord? patchedRecord = null;
            return rcd.Fill(context, rule, valueKey, rcd, ref patchedRecord);
        }

        public static int ForwardRecord ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, ValueKey valueKey )
        {
            if (context.Record is not IMajorRecordGetter)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Trace, context, "", typeof(IMajorRecordGetter).Name, context.Record.GetType().Name, ClassLogPrefix | 0x21);
                return 0;
            }

            if (!rule.TryGetForward(valueKey, out var mods, out string[]? fields))
                return -1;

            var All = Global.State.LinkCache.ResolveAllContexts(context.Record.FormKey, context.Record.Registration.GetterType);
            if (!All.SafeAny())
                return -1;

            var orderedMods = mods.Select((key,value) => new { key = key.Key, value = All.FirstOrDefault(m => m.ModKey.Equals(key.Key)) }).ToFrozenDictionary(x => x.key, x => x.value);

            if (!orderedMods.Any(k => k.Value != null))
                return -1;

            int changed = 0;
            foreach (string field in fields)
            {
                var rcd = RCDMapping.FindRecordCallData(context, field.ToLower());
                if (rcd == null || !rcd.CanForward() || (rule.ForwardType.HasFlag(ForwardTypes.SelfMasterOnly) && !rcd.CanForwardSelfOnly()))
                {
                    LogHelper.Log(LogLevel.Trace, context, field, $"Unknown / Unimplemented field for forward action type: {Enum.GetName(rule.ForwardType)}", ClassLogPrefix | 0x26);
                    continue;
                }

                bool firstMod = true;
                ISkyrimMajorRecord? patchedRecord = null;

                if (rule.HasForwardType(ForwardTypeFlags.SelfMasterOnly))
                {
                    foreach (var mod in orderedMods)
                    {
                        LogHelper.Log(LogLevel.Trace, context, $"Attempt {Enum.GetName(rule.ForwardType)} forward field {field} from {mod.Key}", ClassLogPrefix | 0x24);
                        if (rule.ForwardType.HasFlag(ForwardTypes.DefaultThenSelfMasterOnly))
                        {
                            if (firstMod)
                            {   // First mod of DefaultThenSelfMasterOnly
                                if (mod.Value == null) // We don't continue if first mod can't be default forwarded
                                    break;

                                if (rule.OnlyIfDefault && !rcd.Matches(context.Record, origin, rcd))
                                {
                                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0xE1);
                                    break;
                                }

                                int changes = rcd.Forward(context, rule, mod.Value, rcd, ref patchedRecord);
                                if (changes < 0)
                                {   // If default forward fails we do not continue with the SelfMasterOnly forwards
                                    LogHelper.Log(LogLevel.Trace, context, "DefaultThenSelfMasterOnly: Default forward failed so skipping SelfMasterOnly mods.", ClassLogPrefix | 0x25);
                                    break;
                                }
                                else
                                {
                                    changed += changes;
                                    firstMod = false;
                                }
                            }
                            else
                            {   // All other mods in DefaultThenSelfMasterOnly - No need to check origin
                                int changes = (mod.Value != null)?rcd.ForwardSelfOnly(context, rule, mod.Value, rcd, ref patchedRecord): 0;
                                if (changes > 0)
                                    changed += changes;
                            }
                        }
                        else if (rule.ForwardType.HasFlag(ForwardTypes.SelfMasterOnly))
                        {   //  SelfMasterOnly
                            if (patchedRecord != null && rule.OnlyIfDefault && !rcd.Matches(context.Record, origin, rcd))
                            {
                                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0xE2);
                                break;
                            }

                            // modContext == null fine here we just skip those ones
                            int changes = (mod.Value != null)?rcd.ForwardSelfOnly(context, rule, mod.Value, rcd, ref patchedRecord): 0;
                            if (changes > 0)
                                changed += changes;
                        }
                        else
                        {   // Should never reach here as Default already handled outside of foreach loop.
                            throw new Exception("WTF. Code should never reach this point.");
                        }
                    }
                }
                else
                {   // Default Forward Type
                    var filtered = orderedMods.Where(x => x.Value != null).ToList(); // This will always return at least 1 entry due to previous checks
                    int index = filtered.Count != 1 && rule.HasForwardType(ForwardTypeFlags.Random) ? new Random(HashCode.Combine(context.Record.FormKey, field, rule)).Next(filtered.Count) : 0;

                    if (filtered.Count > 1)
                        LogHelper.Log(LogLevel.Trace, context, field, $"Method: {Enum.GetName(rule.ForwardType)}. Selected #{index + 1} from {filtered.Count} available mods.", 0x28);

                    var modContext = filtered.ElementAt(index);
                    LogHelper.Log(LogLevel.Trace, context, field, $"Default forwarding from: {modContext.Key.FileName}", ClassLogPrefix | 0x27);

                    if (rule.OnlyIfDefault && !rcd.Matches(context.Record, origin, rcd))
                    {
                        LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0xE3);
                    }
                    else
                    {
                        int changes = (modContext.Value != null) ? rcd.Forward(context, rule, modContext.Value, rcd, ref patchedRecord) : throw new Exception("WTF Should never hit this!");
                        if (changes > 0)
                            changed += changes;
                    }
                }
            }

            return changed;
        }

        public static async Task<int> Main ( string[] args ) => await SynthesisPipeline.Instance
                                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings(nickname: "Generic Synthesis Patcher Settings", path: "settings.json", out Global.settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "GenericSynthesisPatcher.esp")
                .Run(args);

        public static void RunPatch ( IPatcherState<ISkyrimMod, ISkyrimModGetter> state )
        {
            Global.State = state;

            if (Global.Settings.Value.LogLevel != LogLevel.Trace)
                Global.Settings.Value.TraceFormKey = null;
            else
                LogHelper.Log(LogLevel.Trace, "Extra logging for FormKey: " + ((Global.Settings.Value.TraceFormKey == null) ? "null" : Global.Settings.Value.TraceFormKey), ClassLogPrefix | 0x31);

            var Rules = LoadRules();
            if (Rules.Count == 0)
                return;

            int total = 0, updated = 0, changed = 0;
            // subTotals values = (Total, Matched, Updated)
            SortedDictionary<RecordTypes, (int, int, int)> subTotals = [];

            while (EnabledTypes != RecordTypes.NONE)
            {
                IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>> ProcessTypeRecords;
                if (EnabledTypes.HasFlag(RecordTypes.ALCH))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingestible().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.ALCH);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.AMMO))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ammunition().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.AMMO);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.ARMO))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Armor().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.ARMO);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.BOOK))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Book().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.BOOK);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.CELL))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Cell().WinningContextOverrides(state.LinkCache);
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.CELL);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.CONT))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Container().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.CONT);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.FACT))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Faction().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.FACT);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.INGR))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingredient().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.INGR);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.KEYM))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Key().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.KEYM);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.MISC))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MiscItem().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.MISC);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.NPC))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Npc().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.NPC);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.OTFT))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Outfit().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.OTFT);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.SCRL))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Scroll().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.SCRL);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.WEAP))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weapon().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.WEAP);
                }
                else if (EnabledTypes.HasFlag(RecordTypes.WRLD))
                {
                    ProcessTypeRecords = state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Worldspace().WinningContextOverrides();
                    EnabledTypes = EnabledTypes.RemoveFlags(RecordTypes.WRLD);
                }
                else
                {
                    LogHelper.Log(LogLevel.Critical, $"Found unsupported types. {(int)EnabledTypes:B18}", ClassLogPrefix | 0xFF);
                    break;
                }

                foreach (var context in ProcessTypeRecords)
                {
                    var recordType = GSPRule.GetGSPRuleType(context.Record);
                    if (!subTotals.ContainsKey(recordType))
                        subTotals.Add(recordType, (0, 0, 0));
                    subTotals[recordType] = (subTotals[recordType].Item1 + 1, subTotals[recordType].Item2, subTotals[recordType].Item3);

                    total++;
                    foreach (var rule in Rules)
                    {
                        if (rule.Matches(context))
                        {
                            if (rule is GSPRule gspRule)
                            {
                                subTotals[recordType] = (subTotals[recordType].Item1, subTotals[recordType].Item2 + 1, subTotals[recordType].Item3);
                                int changes = ProcessRule(context, gspRule);
                                if (changes > 0)
                                {
                                    changed += changes;
                                    subTotals[recordType] = (subTotals[recordType].Item1, subTotals[recordType].Item2, subTotals[recordType].Item3 + 1);
                                    updated++;
                                }
                            }
                            else if (rule is GSPGroup group)
                            {
                                LogHelper.Log(LogLevel.Trace, context, $"Matched group. Processing Rules.", 0x34);
                                foreach (var groupRule in group.Rules)
                                {
                                    if (groupRule.Matches(context))
                                    {
                                        subTotals[recordType] = (subTotals[recordType].Item1, subTotals[recordType].Item2 + 1, subTotals[recordType].Item3);
                                        int changes = ProcessRule(context, groupRule);
                                        if (changes > 0)
                                        {
                                            changed += changes;
                                            subTotals[recordType] = (subTotals[recordType].Item1, subTotals[recordType].Item2, subTotals[recordType].Item3 + 1);
                                            updated++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            LogHelper.Log(LogLevel.Information, $"Completed. Applied {changed:N0} changes over {updated:N0} updated records.", ClassLogPrefix | 0x33);

            Console.WriteLine($"Record Type Totals");
            Console.WriteLine($"{"Type",-10} {"Total",10} {"Matched",10} {"Updated",10}");

            foreach (var t in from t in subTotals
                              where t.Key != RecordTypes.UNKNOWN
                              select t)
            {
                Console.WriteLine($"{Enum.GetName(t.Key),-10} {t.Value.Item1,10:N0} {t.Value.Item2,10:N0} {t.Value.Item3,10:N0}");
            }

            if (subTotals.TryGetValue(RecordTypes.UNKNOWN, out var value))
                Console.WriteLine($"{Enum.GetName(RecordTypes.UNKNOWN),-10} {value.Item1,10:N0} {value.Item2,10:N0} {value.Item3,10:N0}");
        }

        private static List<GSPBase> LoadRules ()
        {
            EnabledTypes = RecordTypes.NONE;
            string dataFolder = Global.Settings.Value.Folder;
            dataFolder = dataFolder.Replace("{SkyrimData}", Global.State.DataFolderPath);
            dataFolder = dataFolder.Replace("{SynthesisData}", Global.State.ExtraSettingsDataPath);

            if (!Directory.Exists(dataFolder))
            {
                LogHelper.Log(LogLevel.Error, $"Missing data folder: {dataFolder}", ClassLogPrefix | 0x51);
                return [];
            }

            var LoadedRules = new List<GSPBase>();
            int count = 0;

            var files = Directory.GetFiles(dataFolder).Where(x => x.EndsWith(".json"));
            files.ForEach(f =>
            {
                if (Path.Combine(dataFolder, f).Equals(Path.Combine(Global.State.ExtraSettingsDataPath ?? "", "settings.json")))
                {
                    LogHelper.Log(LogLevel.Information, $"Skipping: {Path.Combine(dataFolder, f)}", ClassLogPrefix | 0x52);
                }
                else
                {
                    LogHelper.Log(LogLevel.Information, $"Loading config file: {Path.Combine(dataFolder, f)}", ClassLogPrefix | 0x53);
                    var rules = JsonConvert.DeserializeObject<List<GSPBase>>(File.ReadAllText(Path.Combine(dataFolder, f)), Global.SerializerSettings);

                    foreach (var rule in rules ?? [])
                    {
                        if (!rule.Validate())
                        {
                            LogHelper.Log(LogLevel.Critical, "Error validating rules.", 0x58);
                            LoadedRules = [];
                            return;
                        }

                        if (rule is GSPGroup group)
                            count += group.Rules.Count;
                        else
                            count++;

                        if (EnabledTypes == RecordTypes.All)
                            break;

                        if (rule == null)
                            continue;

                        if (rule.Types == RecordTypes.All)
                        {
                            LogHelper.Log(LogLevel.Information, "Found rule with no or all defined types. For best performance you should always define at least 1 type, and only required types for the rule.", 0x57);
                            EnabledTypes = RecordTypes.All;
                        }
                        else
                        {
                            EnabledTypes |= rule.Types;
                        }
                    }

                    LoadedRules = [.. LoadedRules, .. rules];
                }
            });

            if (LoadedRules.Count == 0)
            {
                LogHelper.Log(LogLevel.Error, $"No rules found in data location: {dataFolder}", ClassLogPrefix | 0x54);
                return [];
            }

            LoadedRules.Sort();

            if (LoadedRules.Count != count)
                LogHelper.Log(LogLevel.Information, $"Loaded {LoadedRules.Count} primary rules and {count} total rules.", ClassLogPrefix | 0x56);
            else
                LogHelper.Log(LogLevel.Information, $"Loaded {count} total rules.", ClassLogPrefix | 0x58);

            return LoadedRules;
        }

        private static int ProcessRule ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule )
        {
            int changes = 0;

            var origin = rule.OnlyIfDefault ? Mod.FindOrigin(context) : null;

            foreach (var x in rule.fillValues)
                changes += FillRecord(context, origin, rule, x.Key);

            foreach (var x in rule.forwardValues)
                changes += ForwardRecord(context, origin, rule, x.Key);

            return changes;
        }
    }
}