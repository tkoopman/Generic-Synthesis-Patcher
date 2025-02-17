using System.Collections.Frozen;
using System.Data;

using DynamicData;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher
{
    public partial class Program
    {
        private const int ClassLogCode = 0x01;
        private static readonly List<(RecordTypeMapping Type, FormKey FormKey, GSPRule Rule, RecordPropertyMapping Rpm, int Changes)> RecordUpdates = [];
        private static IReadOnlyList<RecordTypeMapping> EnabledTypes = [];

        public static async Task<int> Main (string[] args)
        {
            if (args.Length == 0)
            {
                GenerateDoco.Generate();
                return 0;
            }
            else
            {
                return await SynthesisPipeline.Instance
                    .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                    .SetAutogeneratedSettings(nickname: "Generic Synthesis Patcher Settings", path: "settings.json", out Global.settings)
                    .SetTypicalOpen(GameRelease.SkyrimSE, "GenericSynthesisPatcher.esp")
                    .Run(args);
            }
        }

        public static void RunPatch (IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            Global.State = state;

            if (Global.Settings.Value.Logging.LogLevel <= LogLevel.Debug)
            {
                LogHelper.WriteLog(LogLevel.Debug, ClassLogCode, "Extra logging for FormKey: " +
                    (
                        Global.Settings.Value.Logging.All ? "ALL" :
                        Global.Settings.Value.Logging.FormKey == FormKey.Null ? "None" :
                        Global.Settings.Value.Logging.FormKey.ToString()
                    ));
            }

            var Rules = LoadRules();
            if (Rules.Count == 0)
                return;

            // subTotals values = (Total, Matched, Updated, Changes)
            SortedDictionary<string, Counts> subTotals = [];

            foreach (var rtm in EnabledTypes)
            {
                var ProcessTypeRecords = rtm.WinningContextOverrides();
                var counts = new Counts();
                subTotals.Add(rtm.Name, counts);

                counts.Stopwatch.Start();
                foreach (var context in ProcessTypeRecords)
                {
                    counts.Total++;
                    var proKeys = new ProcessingKeys(rtm, context);

                    foreach (var rule in Rules)
                    {
                        Global.Processing(ClassLogCode, rule, context);

                        _ = proKeys.SetRule(rule);

                        if (rule.Matches(proKeys))
                        {
                            if (proKeys.IsRule)
                            {
                                int changed = ProcessRule(proKeys);
                                if (changed >= 0) // -1 would mean failed OnlyIfDefault check
                                {
                                    counts.Matched++;
                                    if (changed > 0)
                                        counts.Updated++;
                                    counts.Changes += changed;
                                }
                            }
                            else if (proKeys.IsGroup)
                            {
                                if (Global.Settings.Value.Logging.NoisyLogs.GroupMatched)
                                    Global.TraceLogger?.Log(ClassLogCode, $"Matched group. Processing Rules.");

                                var gProKeys = new ProcessingKeys(rtm, context, proKeys);
                                int count = 0;
                                foreach (var groupRule in proKeys.Group.Rules)
                                {
                                    _ = gProKeys.SetRule(groupRule);
                                    Global.Processing(ClassLogCode, groupRule, context);

                                    count++;
                                    if (groupRule.Matches(gProKeys))
                                    {
                                        int changed = ProcessRule(gProKeys);
                                        if (changed >= 0) // -1 would mean failed OnlyIfDefault check
                                        {
                                            counts.Matched++;
                                            if (changed > 0)
                                                counts.Updated++;
                                            counts.Changes += changed;

                                            if (proKeys.Group.SingleMatch)
                                            {
                                                if (count != proKeys.Group.Rules.Count)
                                                    Global.TraceLogger?.Log(ClassLogCode, $"Skipping remaining rules in group due to SingleMatch. Checked {count}/{proKeys.Group.Rules.Count}");
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                counts.Stopwatch.Stop();
            }

            Global.Processing(ClassLogCode, null, null);
            LogHelper.WriteLog(LogLevel.Information, ClassLogCode, $"Completed");
            Console.WriteLine();
            LogHelper.PrintCounts();
            Console.WriteLine();

            var updates = RecordUpdates.GroupBy(g => (g.Type, g.FormKey, g.Rpm.PropertyName),
                                                g => (g.Rule, g.Changes), (k, data) => new { Key = k, Rules = data.Select(d => d.Rule).Count(), Changes = data.Select(d => d.Changes).Sum() })
                                       .Where(g => g.Rules > 1);

            foreach (var update in updates)
                Console.WriteLine($"Warning: Record {update.Key.FormKey} had {update.Key.PropertyName} updated by {update.Rules} different rules, with total of {update.Changes} changes.");

            Console.WriteLine();

            Console.WriteLine($"Record Type Totals");
            Console.WriteLine($"{"Type",-6} {"Total",10} {"Matched",10} {"Updated",10} {"Changes",10}");

            var totals = new Counts();
            TimeSpan ts = new();

            foreach (var (key, subTotal) in subTotals)
            {
                if (Global.Settings.Value.Logging.LogLevel == LogLevel.Trace)
                    Console.WriteLine($"{key,-6} {subTotal.Total,10:N0} {subTotal.Matched,10:N0} {subTotal.Updated,10:N0} {subTotal.Changes,10:N0}   {subTotal.Stopwatch.Elapsed:c}");
                else
                    Console.WriteLine($"{key,-6} {subTotal.Total,10:N0} {subTotal.Matched,10:N0} {subTotal.Updated,10:N0} {subTotal.Changes,10:N0}");

                totals.Total += subTotal.Total;
                totals.Matched += subTotal.Matched;
                totals.Updated += subTotal.Updated;
                totals.Changes += subTotal.Changes;
                ts = ts.Add(subTotal.Stopwatch.Elapsed);
            }

            if (Global.Settings.Value.Logging.LogLevel == LogLevel.Trace)
            {
                Console.WriteLine($"{"Totals",-6} {totals.Total,10:N0} {totals.Matched,10:N0} {totals.Updated,10:N0} {totals.Changes,10:N0}   {ts:c}");
                Console.WriteLine();
                Console.WriteLine($"All matches took: {MatchesHelper.Stopwatch.Elapsed:c}");
            }
            else
            {
                Console.WriteLine($"{"Totals",-6} {totals.Total,10:N0} {totals.Matched,10:N0} {totals.Updated,10:N0} {totals.Changes,10:N0}");
            }
        }

        private static List<GSPBase> LoadRules ()
        {
            HashSet <RecordTypeMapping> enabledTypes = [];
            string dataFolder = Global.Settings.Value.Folder;
            dataFolder = dataFolder.Replace("{SkyrimData}", Global.State.DataFolderPath);
            dataFolder = dataFolder.Replace("{SynthesisData}", Global.State.ExtraSettingsDataPath);

            if (!Directory.Exists(dataFolder))
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, $"Missing data folder: {dataFolder}");
                return [];
            }

            var LoadedRules = new List<GSPBase>();
            int count = 0;

            var files = Directory.GetFiles(dataFolder).Where(x => x.EndsWith(".json"));
            int countFile = 0;
            foreach (string? f in files)
            {
                if (f.Equals(Path.Combine(Global.State.ExtraSettingsDataPath ?? "", "settings.json")))
                {
                    LogHelper.WriteLog(LogLevel.Information, ClassLogCode, $"Skipping: {f}");
                }
                else
                {
                    LogHelper.WriteLog(LogLevel.Information, ClassLogCode, $"Loading config file #{++countFile}: {f}");
                    List<GSPBase>? rules = null;
                    using (var jsonFile = File.OpenText(f))
                    {
                        using var jsonReader = new JsonTextReader(jsonFile);
                        rules = JsonSerializer.Create(Global.SerializerSettings).Deserialize<List<GSPBase>>(jsonReader);
                    }

                    int countRule = 1;
                    foreach (var rule in rules ?? [])
                    {
                        rule.ConfigFile = countFile;
                        rule.ConfigRule = countRule++;

                        if (!rule.Validate())
                        {
                            LogHelper.WriteLog(LogLevel.Critical, ClassLogCode, "Error validating rules.", rule: rule);
                            enabledTypes.Clear();
                            return [];
                        }

                        if (rule is GSPGroup group)
                            count += group.Rules.Count;
                        else
                            count++;

                        LoadedRules.Add(rule);

                        if (EnabledTypes != RecordTypeMappings.All)
                        {
                            if (rule.Types.Count == RecordTypeMappings.All.Count)
                            {
                                LogHelper.WriteLog(LogLevel.Information, ClassLogCode, "Found rule with no or all defined types. For best performance you should always define at least 1 type, and only required types for the rule.");
                                EnabledTypes = RecordTypeMappings.All;
                            }
                            else
                            {
                                enabledTypes.Add(rule.Types);
                            }
                        }
                    }
                }
            }

            EnabledTypes = enabledTypes.ToList().AsReadOnly();

            if (LoadedRules.Count == 0)
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, $"No rules found in data location: {dataFolder}");
                return [];
            }

            LoadedRules.Sort();

            if (LoadedRules.Count != count)
                LogHelper.WriteLog(LogLevel.Information, ClassLogCode, $"Loaded {LoadedRules.Count} primary rules and {count} total rules.");
            else
                LogHelper.WriteLog(LogLevel.Information, ClassLogCode, $"Loaded {count} total rules.");

            return LoadedRules;
        }

        private static int ProcessFillRecord (ProcessingKeys proKeys, FilterOperation ruleKey)
        {
            if (!proKeys.SetProperty(ruleKey, ruleKey.Value) || !proKeys.Property.Action.CanFill())
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Unknown / Unimplemented field for fill.", propertyName: ruleKey.Value);
                return -1;
            }

            if (proKeys.Rule.OnlyIfDefault)
                Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.MatchesOrigin)}", propertyName: proKeys.Property.PropertyName);

            if (proKeys.Rule.OnlyIfDefault && !proKeys.Property.Action.MatchesOrigin(proKeys))
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.OriginMismatch, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.Fill)}", propertyName: proKeys.Property.PropertyName);
            int changed = proKeys.Property.Action.Fill(proKeys);

            if (changed > 0)
                RecordUpdates.Add((proKeys.Type, proKeys.Record.FormKey, proKeys.Rule, proKeys.Property, changed));

            return changed;
        }

        private static int ProcessForwardRecord (ProcessingKeys proKeys, FilterOperation ruleKey)
        {
            // Don't waste time if record is master with no overwrites
            if (proKeys.Context.ModKey.Equals(proKeys.Record.FormKey.ModKey))
                return -1;

            if (!proKeys.Rule.TryGetForward(ruleKey, out var mods, out string[]? fields))
                return -1;

            var All = Global.State.LinkCache.ResolveAllContexts(proKeys.Record.FormKey, proKeys.Record.Registration.GetterType);
            if (!All.SafeAny())
                return -1;

            // Dictionary with key of ModKey from JSON and value of the record context or null if record not found in that mod.
            var orderedMods = mods.Select((key,value) => new { key = key.Key, value = All.FirstOrDefault(m => m.ModKey.Equals(key.Key)) }).ToFrozenDictionary(x => x.key, x => x.value);

            if (!orderedMods.Any(k => k.Value != null))
                return -1;

            int changed = 0;
            foreach (string field in fields)
            {
                if (!proKeys.SetProperty(ruleKey, field) || !proKeys.Property.Action.CanForward() || (proKeys.Rule.ForwardType.HasFlag(ForwardTypes.SelfMasterOnly) && !proKeys.Property.Action.CanForwardSelfOnly()))
                {
                    Global.TraceLogger?.Log(ClassLogCode, $"Unknown / Unimplemented field for forward action type: {Enum.GetName(proKeys.Rule.ForwardType)}", propertyName: field);
                    continue;
                }

                bool firstMod = true;

                if (proKeys.Rule.HasForwardType(ForwardTypeFlags.SelfMasterOnly))
                {
                    foreach (var mod in orderedMods)
                    {
                        if (proKeys.Rule.ForwardType.HasFlag(ForwardTypes.DefaultThenSelfMasterOnly))
                        {
                            if (firstMod)
                            {   // First mod of DefaultThenSelfMasterOnly
                                if (mod.Value == null) // We don't continue if first mod can't be default forwarded
                                    break;

                                if (proKeys.Rule.OnlyIfDefault)
                                    Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.MatchesOrigin)}", propertyName: proKeys.Property.PropertyName);

                                if (proKeys.Rule.OnlyIfDefault && !proKeys.Property.Action.MatchesOrigin(proKeys))
                                {
                                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.OriginMismatch, propertyName: proKeys.Property.PropertyName);
                                    break;
                                }

                                Global.TraceLogger?.Log(ClassLogCode, $"Forwarding Type: Default From: {mod.Value.ModKey.FileName}.", propertyName: proKeys.Property.PropertyName);

                                //Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.Forward)}", propertyName: proKeys.Property.PropertyName);
                                int changes = proKeys.Property.Action.Forward(proKeys, mod.Value);
                                if (changes < 0)
                                {   // If default forward fails we do not continue with the SelfMasterOnly forwards
                                    Global.TraceLogger?.Log(ClassLogCode, "DefaultThenSelfMasterOnly: Default forward failed so skipping SelfMasterOnly mods.");
                                    break;
                                }
                                else
                                {
                                    changed += changes;
                                    firstMod = false;
                                }
                            }
                            else if (mod.Value != null)
                            {   // All other mods in DefaultThenSelfMasterOnly - No need to check origin
                                Global.TraceLogger?.Log(ClassLogCode, $"Forwarding Type: {nameof(ForwardTypes.SelfMasterOnly)} From: {mod.Value.ModKey.FileName}.", propertyName: proKeys.Property.PropertyName);

                                //Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.ForwardSelfOnly)}", propertyName: proKeys.Property.PropertyName);

                                int changes = proKeys.Property.Action.ForwardSelfOnly(proKeys, mod.Value);
                                if (changes > 0)  // Could be -1 which we don't add to changed
                                    changed += changes;
                            }
                        }
                        else if (proKeys.Rule.ForwardType.HasFlag(ForwardTypes.SelfMasterOnly) && mod.Value != null)
                        {   //  SelfMasterOnly
                            if (proKeys.Rule.OnlyIfDefault)
                                Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.MatchesOrigin)}", propertyName: proKeys.Property.PropertyName);

                            if (proKeys.HasPatchRecord && proKeys.Rule.OnlyIfDefault && !proKeys.Property.Action.MatchesOrigin(proKeys))
                            {
                                Global.TraceLogger?.Log(ClassLogCode, LogHelper.OriginMismatch, propertyName: proKeys.Property.PropertyName);
                                break;
                            }

                            Global.TraceLogger?.Log(ClassLogCode, $"Forwarding Type: {nameof(ForwardTypes.SelfMasterOnly)} From: {mod.Value.ModKey.FileName}.", propertyName: proKeys.Property.PropertyName);

                            //Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.ForwardSelfOnly)}", propertyName: proKeys.Property.PropertyName);
                            int changes = proKeys.Property.Action.ForwardSelfOnly(proKeys, mod.Value);
                            if (changes > 0)
                                changed += changes;
                        }
                        else if (mod.Value != null)
                        {   // Should never reach here as Default already handled outside of foreach loop.
                            throw new Exception("WTF. Code should never reach this point.");
                        }
                    }
                }
                else
                {   // Default Forward Type
                    // Find modContext of forwarding record
                    var filtered = orderedMods.Where(x => x.Value != null).ToList(); // This will always return at least 1 entry due to previous checks
                    int index = filtered.Count != 1 && proKeys.Rule.HasForwardType(ForwardTypeFlags.Random) ? proKeys.GetRandom().Next(filtered.Count) : 0;
                    var modContext = filtered.ElementAt(index).Value ?? throw new Exception("WTF Should never hit this!");

                    // If forwarding context mod is same as current lets not waste time continuing
                    if (modContext.ModKey.Equals(proKeys.Context.ModKey))
                    {
                        Global.TraceLogger?.Log(ClassLogCode, "Skipping forward as cannot forward to self", propertyName: proKeys.Property.PropertyName);
                        continue;
                    }

                    if (proKeys.Rule.OnlyIfDefault)
                        Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.MatchesOrigin)}", propertyName: proKeys.Property.PropertyName);

                    if (proKeys.Rule.OnlyIfDefault && !proKeys.Property.Action.MatchesOrigin(proKeys))
                    {
                        Global.TraceLogger?.Log(ClassLogCode, LogHelper.OriginMismatch, propertyName: proKeys.Property.PropertyName);
                        continue;
                    }

                    if (filtered.Count > 1)
                        Global.TraceLogger?.Log(ClassLogCode, $"Forwarding Type: {Enum.GetName(proKeys.Rule.ForwardType)} From: {modContext.ModKey.FileName}. Selected #{index + 1} from {filtered.Count} available mods.", propertyName: proKeys.Property.PropertyName);
                    else
                        Global.TraceLogger?.Log(ClassLogCode, $"Forwarding Type: {Enum.GetName(proKeys.Rule.ForwardType)} From: {modContext.ModKey.FileName}.", propertyName: proKeys.Property.PropertyName);

                    Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.Forward)}", propertyName: proKeys.Property.PropertyName);
                    int changes = proKeys.Property.Action.Forward(proKeys, modContext);
                    if (changes > 0)
                        changed += changes;
                }

                if (changed > 0)
                    RecordUpdates.Add((proKeys.Type, proKeys.Record.FormKey, proKeys.Rule, proKeys.Property, changed));
            }

            return changed;
        }

        private static int ProcessMergeRecord (ProcessingKeys proKeys)
        {
            if (!proKeys.Property.Action.CanMerge())
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Unknown / Unimplemented field for merge action.", propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (proKeys.Rule.OnlyIfDefault)
                Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.MatchesOrigin)}", propertyName: proKeys.Property.PropertyName);

            if (proKeys.Rule.OnlyIfDefault && !proKeys.Property.Action.MatchesOrigin(proKeys))
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.OriginMismatch, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            Global.TraceLogger?.LogAction(ClassLogCode, $"{proKeys.Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.Merge)}", propertyName: proKeys.Property.PropertyName);
            int changed = proKeys.Property.Action.Merge(proKeys);

            if (changed > 0)
                RecordUpdates.Add((proKeys.Type, proKeys.Record.FormKey, proKeys.Rule, proKeys.Property, changed));

            return changed;
        }

        private static int ProcessRule (ProcessingKeys proKeys)
        {
            var rule = proKeys.Rule;

            // We want result to be 0 if no actions so it works with SingleMatch
            if (!rule.Fill.SafeAny() && !rule.Forward.SafeAny() && !rule.Merge.SafeAny())
            {
                Global.TraceLogger?.Log(ClassLogCode, "Rule contains no actions.");
                return 0;
            }

            int changes = -1;

            if (rule.Merge.Count > 0)
            {
                int versions = Global.State.LinkCache.ResolveAllContexts(proKeys.Record.FormKey, proKeys.Record.Registration.GetterType).Count();
                switch (versions)
                {
                    case < 2:
                        if (Global.Settings.Value.Logging.NoisyLogs.MergeNoOverwrites)
                            Global.TraceLogger?.Log(ClassLogCode, "Doesn't have any overwrites to merge with.");
                        break;

                    default:
                        foreach (var x in rule.Merge)
                        {
                            if (!proKeys.SetProperty(x.Key, x.Key.Value))
                            {
                                Global.TraceLogger?.Log(ClassLogCode, $"Failed on merge. No RPM for Field.", propertyName: x.Key.Value);
                                continue;
                            }

                            int changed = ProcessMergeRecord(proKeys);

                            if (changed >= 0)
                                changes = (changes == -1) ? changed : changes + changed;
                        }

                        break;
                }
            }

            foreach (var x in rule.Forward)
            {
                int changed = ProcessForwardRecord(proKeys, x.Key);

                if (changed >= 0)
                    changes = (changes == -1) ? changed : changes + changed;
            }

            foreach (var x in rule.Fill)
            {
                int changed = ProcessFillRecord(proKeys, x.Key);

                if (changed >= 0)
                    changes = (changes == -1) ? changed : changes + changed;
            }

            return changes;
        }
    }
}