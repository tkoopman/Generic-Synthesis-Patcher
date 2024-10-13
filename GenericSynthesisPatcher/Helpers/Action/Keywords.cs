using System.Data;
using System.Threading.Channels;
using System.Xml.Linq;

using DynamicData;

using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

using static GenericSynthesisPatcher.Json.Data.GSPRule;
using static NexusMods.Paths.Delegates;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class Keywords : IAction
    {
        private const int ClassLogPrefix = 0x800;
        private static Dictionary<string, IKeywordGetter>? keywords;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => true;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            var baseRecord = patchedRecord ?? context.Record;

            if (baseRecord is IKeywordedGetter<IKeywordGetter> record)
            {
                int changes = 0;
                foreach (var keyword in rule.GetFillValueAs<List<OperationValue>>(valueKey) ?? [])
                {
                    var k = GetKeyword(keyword.Value);
                    if (k == null)
                        continue;

                    bool found = record.HasKeyword(k);
                    if ((found && keyword.Operation == Operation.Remove) || (!found && keyword.Operation != Operation.Remove))
                    {
                        patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                        if (keyword.Operation == Operation.Remove)
                            ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords?.Remove(k);
                        else
                            ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords?.Add(k);

                        changes++;
                    }
                }

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x12);
                return changes;
            }
            else
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IKeywordedGetter", context.Record.GetType().Name, ClassLogPrefix | 0x13);
                return -1;
            }
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            var baseRecord = patchedRecord ?? context.Record;

            if (baseRecord is IKeywordedGetter<IKeywordGetter> record && forwardContext.Record is IKeywordedGetter<IKeywordGetter> forward)
            {
                if (forward.Keywords.SequenceEqualNullable(record.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x21);
                    return 0;
                }

                patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                if (((IKeyworded<IKeywordGetter>)patchedRecord).Keywords == null)
                {
                    ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords = [];
                    LogHelper.Log(LogLevel.Trace, context, "Created new keyword list.", 0x25);
                }

                int changes = ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords?.RemoveAll(_ => true) ?? 0;

                if (forward.Keywords.SafeAny())
                {
                    ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords?.AddRange(forward.Keywords);
                    changes += forward.Keywords.Count;
                }

                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x23);
                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IKeywordedGetter", context.Record.GetType().Name, ClassLogPrefix | 0x24);
            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            var baseRecord = patchedRecord ?? context.Record;

            if (baseRecord is IKeywordedGetter<IKeywordGetter> record && forwardContext.Record is IKeywordedGetter<IKeywordGetter> forward)
            {
                if (!forward.Keywords.SafeAny())
                    return 0;

                if (forward.Keywords.SequenceEqualNullable(record.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x31);
                    return 0;
                }

                int changes = 0;

                foreach (var item in forward.Keywords)
                {
                    if (item.FormKey.ModKey == forwardContext.ModKey)
                    {
                        if (record.Keywords == null || !record.Keywords.Contains(item))
                        {
                            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                            if (((IKeyworded<IKeywordGetter>)patchedRecord).Keywords == null)
                            {
                                ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords = [];
                                LogHelper.Log(LogLevel.Trace, context, "Created new keyword list.", 0x35);
                            }

                            ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords?.Add(item);
                            changes++;
                        }
                    }
                }

                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x33);
                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IKeywordedGetter", context.Record.GetType().Name, ClassLogPrefix | 0x34);
            return -1;
        }

        public static IKeywordGetter? GetKeyword ( string name )
        {
            if (keywords == null)
            {
                keywords = [];
                Global.State.LoadOrder.PriorityOrder.Keyword().WinningOverrides().ForEach(k => keywords[k.EditorID ?? ""] = k);
                LogHelper.Log(LogLevel.Information, $"{keywords.Count} keywords loaded into cache.", ClassLogPrefix | 0x41);
            }

            _ = keywords.TryGetValue(name, out var value);
            return value;
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd )
        {
            if (check is not IKeywordedGetter<IKeywordGetter> record)
                return false;

            var keywords = rule.GetMatchValueAs<List<OperationValue>>(valueKey);
            if (!keywords.SafeAny())
                return true;

            if (!record.Keywords.SafeAny())
                return !keywords.Any(k => k.Operation != Operation.NOT);

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            foreach (var key in keywords)
            {
                if (key.Operation != Operation.NOT)
                    includesChecked++;

                var keyword = GetKeyword(key.Value);
                if (keyword != null && record.Keywords.Contains(keyword))
                {
                    // Doesn't matter what overall Operation is we always fail on a NOT match
                    if (key.Operation == Operation.NOT)
                        return false;

                    if (valueKey.Operation == Operation.OR)
                        return true;

                    matchedCount++;
                }
                else if (key.Operation != Operation.NOT && valueKey.Operation == Operation.AND)
                {
                    return false;
                }
            }

            return valueKey.Operation switch
            {
                Operation.AND => true,
                Operation.XOR => matchedCount == 1,
                _ => includesChecked == 0 // OR
            };
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => check is IKeywordedGetter<IKeywordGetter> record
                && origin is IKeywordedGetter<IKeywordGetter> originRecord
                && record.Keywords.SequenceEqualNullable(originRecord.Keywords);
    }
}