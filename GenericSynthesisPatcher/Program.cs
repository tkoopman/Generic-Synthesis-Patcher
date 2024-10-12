using System.Data;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher
{
    public partial class Program
    {
        private const int ClassLogPrefix = 0x000;

        public static int FillRecord ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey )
        {
            if (context.Record is not IMajorRecordGetter)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Trace, context, "", typeof(IMajorRecordGetter).Name, context.Record.GetType().Name, ClassLogPrefix | 0x11);
                return -1;
            }

            var rcd = FindRecordCallData(context, valueKey.Key);

            ISkyrimMajorRecord? patchedRecord = null;

            if (rcd != null && rcd.CanFill())
                return rcd.Fill(context, origin, rule, valueKey, rcd, ref patchedRecord);

            LogHelper.Log(LogLevel.Trace, context, $"Unknown / Unimplemented field for fill action: {valueKey.Key}", ClassLogPrefix | 0x12);
            return -1;
        }

        public static int ForwardRecord ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey )
        {
            if (context.Record is not IMajorRecordGetter)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Trace, context, "", typeof(IMajorRecordGetter).Name, context.Record.GetType().Name, ClassLogPrefix | 0x21);
                return 0;
            }

            Dictionary<string, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>?> modContexts = [];
            List<string> fields = [];
            int ruleHashCode = 0;
            if (rule.ForwardIndexedByField)
            {
                fields.Add(valueKey.Key);
                foreach (string mod in rule.GetValueAs<List<string>>(valueKey) ?? [])
                {
                    modContexts.Add(mod, Mod.GetModRecord(context, mod));
                    ruleHashCode = unchecked((ruleHashCode * 13) + mod.GetHashCode());
                }
            }
            else
            {
                var forwardRecord = Mod.GetModRecord(context, valueKey.Key);
                if (forwardRecord != null)
                    modContexts.Add(valueKey.Key, forwardRecord);

                fields = rule.GetValueAs<List<string>>(valueKey) ?? fields;
            }

            if (!modContexts.Any(x => x.Value != null))
            {
                //LogHelper.Log(LogLevel.Trace, context, "No forwarding record.", ClassLogPrefix | 0x22);
                return 0;
            }

            if (fields.Count == 0)
            {
                LogHelper.Log(LogLevel.Trace, context, "No fields in config to forward", ClassLogPrefix | 0x23);
                return 0;
            }

            int changed = 0;
            foreach (string field in fields)
            {
                var rcd = FindRecordCallData(context, field.ToLower());
                if (rcd != null && rcd.CanForward() && (!rule.ForwardType.HasFlag(GSPRule.ForwardTypes.SelfMasterOnly) || rcd.CanForwardSelfOnly()))
                {
                    bool firstMod = true;
                    ISkyrimMajorRecord? patchedRecord = null;

                    if (rule.HasForwardType(GSPRule.ForwardTypeFlags.SelfMasterOnly))
                    {
                        foreach (var modContext in modContexts)
                        {
                            LogHelper.Log(LogLevel.Trace, context, $"Attempt {Enum.GetName(rule.ForwardType)} forward field {field} from {modContext.Key}", ClassLogPrefix | 0x24);
                            if (rule.ForwardType.HasFlag(GSPRule.ForwardTypes.DefaultThenSelfMasterOnly))
                            {
                                if (firstMod)
                                {   // First mod of DefaultThenSelfMasterOnly
                                    if (modContext.Value == null) // We don't continue if first mod can't be default forwarded
                                        break;

                                    int changes = rcd.Forward(context, origin, rule, modContext.Value, rcd, ref patchedRecord);
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
                                {   // All other mods in DefaultThenSelfMasterOnly
                                    int changes = (modContext.Value != null)?rcd.ForwardSelfOnly(context, origin, rule, modContext.Value, rcd, ref patchedRecord): 0;
                                    if (changes > 0)
                                        changed += changes;
                                }
                            }
                            else if (rule.ForwardType.HasFlag(GSPRule.ForwardTypes.SelfMasterOnly))
                            {   //  SelfMasterOnly
                                int changes = (modContext.Value != null)?rcd.ForwardSelfOnly(context, origin, rule, modContext.Value, rcd, ref patchedRecord): 0;
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
                        var filtered = modContexts.Where(x => x.Value != null).ToList(); // This will always return at least 1 entry due to previous checks
                        int index = filtered.Count != 1 && rule.HasForwardType(GSPRule.ForwardTypeFlags.Random) ? new Random(HashCode.Combine(context.Record.FormKey, field, ruleHashCode)).Next(filtered.Count) : 0;

                        if (filtered.Count > 1)
                            LogHelper.Log(LogLevel.Trace, context, field, $"Method: {Enum.GetName(rule.ForwardType)}. Selected #{index + 1} from {filtered.Count} available mods.", 0x28);

                        var modContext = filtered.ElementAt(index);

                        LogHelper.Log(LogLevel.Trace, context, field, $"Default forwarding from: {modContext.Key}", ClassLogPrefix | 0x27);

                        int changes = (modContext.Value != null) ? rcd.Forward(context, origin, rule, modContext.Value, rcd, ref patchedRecord) : throw new Exception("WTF Should never hit this!");
                        if (changes > 0)
                            changed += changes;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Trace, context, field, $"Unknown / Unimplemented field for forward action type: {Enum.GetName(rule.ForwardType)}", ClassLogPrefix | 0x26);
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
            SortedDictionary<GSPRule.Type, (int, int, int)> subTotals = [];

            foreach (var context in state.LoadOrder.PriorityOrder.SkyrimMajorRecord().WinningContextOverrides(state.LinkCache))
            {
                var recordType = GSPRule.GetGSPRuleType(context.Record);
                if (!subTotals.ContainsKey(recordType))
                    subTotals.Add(recordType, (0, 0, 0));
                subTotals[recordType] = (subTotals[recordType].Item1 + 1, subTotals[recordType].Item2, subTotals[recordType].Item3);

                total++;
                Rules.ForEach(rule =>
                {
                    if (rule.Matches(context, out var origin))
                    {
                        subTotals[recordType] = (subTotals[recordType].Item1, subTotals[recordType].Item2 + 1, subTotals[recordType].Item3);

                        bool recordUpdated = false;
                        foreach (var x in rule.jsonValues ?? [])
                        {
                            switch (x.Key.ActionType)
                            {
                                case GSPRule.ActionType.Fill:
                                    int fillChanges = FillRecord(context, origin, rule, x.Key);
                                    if (fillChanges > 0)
                                    {
                                        changed += fillChanges;
                                        recordUpdated = true;
                                    }

                                    break;

                                case GSPRule.ActionType.Forward:
                                    int forwardChanges = ForwardRecord(context, origin, rule, x.Key);
                                    if (forwardChanges > 0)
                                    {
                                        changed += forwardChanges;
                                        recordUpdated = true;
                                    }

                                    break;

                                default:
                                    LogHelper.Log(LogLevel.Warning, context, "Unknown action type", ClassLogPrefix | 0x32);
                                    break;
                            }
                        }

                        if (recordUpdated)
                        {
                            subTotals[recordType] = (subTotals[recordType].Item1, subTotals[recordType].Item2, subTotals[recordType].Item3 + 1);
                            updated++;
                        }
                    }
                });
            }

            LogHelper.Log(LogLevel.Information, $"Completed. Applied {changed:N0} changes over {updated:N0} updated records.", ClassLogPrefix | 0x33);

            Console.WriteLine($"Record Type Totals");
            Console.WriteLine($"{"Type",-10} {"Total",10} {"Matched",10} {"Updated",10}");

            foreach (var t in from t in subTotals
                              where t.Key != GSPRule.Type.UNKNOWN
                              select t)
            {
                Console.WriteLine($"{Enum.GetName(t.Key),-10} {t.Value.Item1,10:N0} {t.Value.Item2,10:N0} {t.Value.Item3,10:N0}");
            }

            if (subTotals.TryGetValue(GSPRule.Type.UNKNOWN, out var value))
                Console.WriteLine($"{Enum.GetName(GSPRule.Type.UNKNOWN),-10} {value.Item1,10:N0} {value.Item2,10:N0} {value.Item3,10:N0}");
        }

        private static RecordCallData? FindRecordCallData ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string valueKey )
        {
            RecordCallData? rcd = null;
            foreach (var r in RecordCallDataMapping)
            {
                int c = r.Key.JsonKey.CompareTo(valueKey);

                if (c == 0 && (r.Key.RecordType == null || r.Key.RecordType.IsAssignableFrom(context.Record.GetType())))
                {
                    rcd = r.Value;
                    break;
                }

                // No need to keep searching sorted list if we already after where a match would be
                if (c == 1)
                    break;
            }

            if (rcd == null)
                LogHelper.Log(LogLevel.Trace, context, $"No RCD found - {valueKey}", ClassLogPrefix | 0x41);

            return rcd;
        }

        private static List<GSPRule> LoadRules ()
        {
            string dataFolder = Global.Settings.Value.Folder;
            dataFolder = dataFolder.Replace("{SkyrimData}", Global.State.DataFolderPath);
            dataFolder = dataFolder.Replace("{SynthesisData}", Global.State.ExtraSettingsDataPath);

            if (!Directory.Exists(dataFolder))
            {
                LogHelper.Log(LogLevel.Error, $"Missing data folder: {dataFolder}", ClassLogPrefix | 0x51);
                return [];
            }

            var rules = new List<GSPRule>();

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
                    var a = JsonConvert.DeserializeObject<List<GSPRule>>(File.ReadAllText(Path.Combine(dataFolder, f)), Global.SerializerSettings);
                    rules = [.. rules, .. a];
                }
            });

            if (rules.Count == 0)
            {
                LogHelper.Log(LogLevel.Error, $"No rules found in data location: {dataFolder}", ClassLogPrefix | 0x54);
                return [];
            }

            rules.Sort(delegate ( GSPRule rule1, GSPRule rule2 )
            {
                return rule1.Priority == rule2.Priority
                        ? rule1.Types != null && rule2.Types == null
                            ? 1 : rule1.Types == null && rule2.Types != null
                            ? -1 : (rule1.Types == null && rule2.Types == null) || (rule1.Types?.Count == rule2.Types?.Count)
                            ? rule1.FormID != null && rule2.FormID == null
                                ? 1 : rule1.FormID == null && rule2.FormID != null
                                ? -1 : (rule1.FormID == null && rule2.FormID == null) || (rule1.FormID?.Count == rule2.FormID?.Count)
                                ? rule1.EditorID != null && rule2.EditorID == null
                                    ? 1 : rule1.EditorID == null && rule2.EditorID != null
                                    ? -1 : rule1.EditorID?.Count.CompareTo(rule2.EditorID?.Count) ?? 0
                                : rule1.FormID?.Count.CompareTo(rule2.FormID?.Count) ?? 0
                            : rule1.Types?.Count.CompareTo(rule2.Types?.Count) ?? 0
                        : rule1.Priority.CompareTo(rule2.Priority);
            });

            LogHelper.Log(LogLevel.Information, $"Loaded {rules.Count} rules", ClassLogPrefix | 0x56);

            return rules;
        }
    }
}