using System.Xml.Linq;

using DynamicData;

using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class Keywords : IAction
    {
        private const int ClassLogPrefix = 0x800;
        private static Dictionary<string, IKeywordGetter>? keywords;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => true;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            var baseRecord = patchedRecord ?? context.Record;

            if (baseRecord is IKeywordedGetter<IKeywordGetter> record)
            {
                if (patchedRecord == null && rule.OnlyIfDefault && origin != null && origin is IKeywordedGetter<IKeywordGetter> originKG && !record.Keywords.SequenceEqualNullable(originKG.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x11);
                    return -1;
                }

                int changes = 0;
                foreach (string c in rule.GetValueAs<List<string>>(valueKey) ?? [])
                {
                    string s = c;
                    if (s.StartsWith('-'))
                    {
                        s = s[1..];
                        var remove = GetKeyword(s);
                        if (remove != null && record.HasKeyword(remove))
                        {
                            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                            ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords?.Remove(remove);
                            changes++;
                        }

                        continue;
                    }

                    if (s.StartsWith('+'))
                        s = s[1..];

                    var add = GetKeyword(s);
                    if (add != null && !record.HasKeyword(add))
                    {
                        patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                        if (((IKeyworded<IKeywordGetter>)patchedRecord).Keywords == null)
                        {
                            ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords = [];
                            LogHelper.Log(LogLevel.Trace, context, "Created new keyword list.", 0x14);
                        }

                        ((IKeyworded<IKeywordGetter>)patchedRecord).Keywords?.Add(add);
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

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            var baseRecord = patchedRecord ?? context.Record;

            if (baseRecord is IKeywordedGetter<IKeywordGetter> record && forwardContext.Record is IKeywordedGetter<IKeywordGetter> forward)
            {
                if (forward.Keywords.SequenceEqualNullable(record.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x21);
                    return 0;
                }

                if (patchedRecord == null && rule.OnlyIfDefault && origin != null && origin is IKeywordedGetter<IKeywordGetter> originGetter && !record.Keywords.SequenceEqualNullable(originGetter.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x22);
                    return -1;
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

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
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

                if (patchedRecord == null && rule.OnlyIfDefault && origin != null && origin is IKeywordedGetter<IKeywordGetter> originGetter && !record.Keywords.SequenceEqualNullable(originGetter.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x32);
                    return -1;
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
    }
}