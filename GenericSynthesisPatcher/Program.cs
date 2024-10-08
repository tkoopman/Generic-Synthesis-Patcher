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
    // Log Codes: 0x0xx
    public partial class Program
    {
        // Log Code: 0x01x
        public static bool FillRecord ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey )
        {
            if (context.Record is not IMajorRecordGetter)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Trace, context, "", typeof(IMajorRecordGetter).Name, context.Record.GetType().Name, 0x011);
                return false;
            }

            var rcd = FindRecordCallData(context, valueKey.Key);

            if (rcd != null && rcd.CanFill())
                return rcd.Fill(context, origin, rule, valueKey, rcd);

            LogHelper.Log(LogLevel.Trace, context, $"Unknown / Unimplemented field for fill action: {valueKey.Key}", 0x012);
            return false;
        }

        // Log Code: 0x02x
        public static uint ForwardRecord ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey )
        {
            if (context.Record is not IMajorRecordGetter)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Trace, context, "", typeof(IMajorRecordGetter).Name, context.Record.GetType().Name, 0x021);
                return 0;
            }

            var forwardRecord = Mod.GetModRecord(context, valueKey.Key);
            if (forwardRecord == null)
            {
                //LogHelper.Log(LogLevel.Trace, context, "No forwarding record.", 0x022);
                return 0;
            }

            var forwardFields = rule.GetValueAs<List<string>>(valueKey);
            if (forwardFields == null || forwardFields.Count == 0)
            {
                LogHelper.Log(LogLevel.Trace, context, "No fields in config to forward", 0x023);
                return 0;
            }

            uint changed = 0;
            foreach (string forwardField in forwardFields)
            {
                LogHelper.Log(LogLevel.Trace, context, $"Attempt forward field {forwardField} for {context.Record.FormKey}");
                var rcd = FindRecordCallData(context, forwardField.ToLower());

                if (rcd != null && rcd.CanForward())
                {
                    if (rcd.Forward(context, origin, rule, forwardRecord, rcd))
                        changed++;
                }
                else
                {
                    LogHelper.Log(LogLevel.Trace, context, $"Unknown / Unimplemented field for forward action: {forwardField}", 0x024);
                }
            }

            return changed;
        }

        public static async Task<int> Main ( string[] args ) => await SynthesisPipeline.Instance
                                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings(nickname: "Generic Synthesis Patcher Settings", path: "settings.json", out Global.settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "GenericSynthesisPatcher.esp")
                .Run(args);

        // Log Codes: 0x00x
        public static void RunPatch ( IPatcherState<ISkyrimMod, ISkyrimModGetter> state )
        {
            Global.State = state;

            if (Global.Settings.Value.LogLevel != LogLevel.Trace)
                Global.Settings.Value.TraceFormKey = null;
            else
                LogHelper.Log(LogLevel.Trace, "Extra logging for FormKey: " + ((Global.Settings.Value.TraceFormKey == null) ? "null" : Global.Settings.Value.TraceFormKey));

            var Rules = LoadRules();
            if (Rules.Count == 0)
                return;

            uint total = 0, updated = 0, changes = 0;
            SortedDictionary<GSPRule.Type, (uint, uint, uint)> subTotals = [];

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
                                    if (FillRecord(context, origin, rule, x.Key))
                                    {
                                        changes++;
                                        recordUpdated = true;
                                    }

                                    break;

                                case GSPRule.ActionType.Forward:
                                    uint changed = ForwardRecord(context, origin, rule, x.Key);
                                    changes += changed;
                                    if (changed > 0)
                                        recordUpdated = true;

                                    break;

                                default:
                                    LogHelper.Log(LogLevel.Warning, context, "Unknown action type", 0x001);
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

            LogHelper.Log(LogLevel.Information, $"Completed. Applied {changes:N0} changes over {updated:N0} updated records.");

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
                LogHelper.Log(LogLevel.Trace, context, $"No RCD found - {valueKey}");

            return rcd;
        }

        private static List<GSPRule> LoadRules ()
        {
            string dataFolder = Global.Settings.Value.Folder;
            dataFolder = dataFolder.Replace("{SkyrimData}", Global.State.DataFolderPath);
            dataFolder = dataFolder.Replace("{SynthesisData}", Global.State.ExtraSettingsDataPath);

            if (!Directory.Exists(dataFolder))
            {
                LogHelper.Log(LogLevel.Error, $"Missing data folder: {dataFolder}");
                return [];
            }

            var rules = new List<GSPRule>();

            var files = Directory.GetFiles(dataFolder).Where(x => x.EndsWith(".json"));
            files.ForEach(f =>
            {
                if (Path.Combine(dataFolder, f).Equals(Path.Combine(Global.State.ExtraSettingsDataPath ?? "", "settings.json")))
                {
                    LogHelper.Log(LogLevel.Information, $"Skipping: {Path.Combine(dataFolder, f)}");
                }
                else
                {
                    LogHelper.Log(LogLevel.Information, $"Loading config file: {Path.Combine(dataFolder, f)}");
                    var a = JsonConvert.DeserializeObject<List<GSPRule>>(File.ReadAllText(Path.Combine(dataFolder, f)), Global.SerializerSettings);
                    rules = [.. rules, .. a];
                }
            });

            if (rules.Count == 0)
            {
                LogHelper.Log(LogLevel.Error, $"No rules found in data location: {dataFolder}");
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

            LogHelper.Log(LogLevel.Information, $"Loaded {rules.Count} rules");

            return rules;
        }
    }
}