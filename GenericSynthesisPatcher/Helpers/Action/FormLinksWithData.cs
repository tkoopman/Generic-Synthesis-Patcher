using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLinksWithData<T> : IAction
        where T : class, IFormLinksWithData<T>
    {
        private const int ClassLogPrefix = 0x700;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => true;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curList))
                    return -1;

                if (patchedRecord == null && rule.OnlyIfDefault && origin != null && !Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(origin, rcd.PropertyName, out var originList) && !RecordsMatch(curList, originList))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x12);
                    return -1;
                }

                int changes = 0;

                foreach (var action in T.GetValueAs(rule, valueKey) ?? [])
                {
                    var e = action.Find(curList);

                    if (e != null && (action.FormKey.Neg || !action.DataEquals(e)))
                    {
                        _ = T.Remove(context, ref patchedRecord, e);
                        changes++;
                    }

                    if (!action.FormKey.Neg && (e == null || (e != null && !action.DataEquals(e))))
                    {
                        _ = action.Add(context, ref patchedRecord);
                        changes++;
                    }
                }

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x13);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x14);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curList))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(forwardContext.Record, rcd.PropertyName, out var newList))
                    return -1;

                if (curList.SequenceEqualNullable(newList))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x21);
                    return 0;
                }

                if (patchedRecord == null && rule.OnlyIfDefault && origin != null && !Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(origin, rcd.PropertyName, out var originList) && !RecordsMatch(curList, originList))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x22);
                    return -1;
                }

                int changes = T.Replace(context, ref patchedRecord, newList);

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x24);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x25);

            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curList))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(forwardContext.Record, rcd.PropertyName, out var newList))
                    return -1;

                if (!newList.SafeAny())
                    return 0;

                if (curList.SequenceEqualNullable(newList))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x31);
                    return 0;
                }

                if (patchedRecord == null && rule.OnlyIfDefault && origin != null && !Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(origin, rcd.PropertyName, out var originList) && !RecordsMatch(curList, originList))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x32);
                    return -1;
                }

                int changes = 0;
                foreach (var item in newList)
                {
                    var key = T.GetFormKey(item);
                    if (key.ModKey == forwardContext.ModKey)
                    {
                        var i = T.Find(curList, key);
                        bool eq = i != null && T.DataEquals(item, i);

                        if (i != null && !eq)
                        {
                            _ = T.Remove(context, ref patchedRecord, i);
                            _ = T.Add(context, ref patchedRecord, item);
                            changes++;
                        }

                        if (i == null)
                        {
                            _ = T.Add(context, ref patchedRecord, item);
                            changes++;
                        }
                    }
                }

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x34);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x35);
            return -1;
        }

        private static bool RecordsMatch ( IReadOnlyList<IFormLinkContainerGetter>? left, IReadOnlyList<IFormLinkContainerGetter>? right )
        {
            if (!left.SafeAny() && !right.SafeAny())
                return true;

            if (!left.SafeAny() || !right.SafeAny() || left.Count != right.Count)
                return false;

            for (int i = 0; i < left.Count; i++)
            {
                if (!T.DataEquals(left.ElementAt(i), right.ElementAt(i)))
                    return false;
            }

            return true;
        }
    }
}