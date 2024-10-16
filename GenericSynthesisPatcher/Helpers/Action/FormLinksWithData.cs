using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

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

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curList))
                    return -1;

                int changes = 0;

                foreach (var action in T.GetFillValueAs(rule, valueKey) ?? [])
                {
                    var e = action.Find(curList);

                    if (e != null && (action.FormKey.Operation == ListLogic.DEL || !action.DataEquals(e)))
                    {
                        _ = T.Remove(context, ref patchedRecord, e);
                        changes++;
                    }

                    if (action.FormKey.Operation == ListLogic.ADD && (e == null || (e != null && !action.DataEquals(e))))
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

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
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

                int changes = T.Replace(context, ref patchedRecord, newList);

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x24);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x25);

            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
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

                int changes = 0;
                foreach (var item in newList)
                {
                    var key = T.GetFormKey(item);
                    if (key.ModKey == forwardContext.ModKey)
                    {
                        var i = T.Find(curList, key);

                        if (i != null && !T.DataEquals(item, i))
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

        /// <summary>
        /// Only checks the FormKeys not the Data
        /// </summary>
        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd )
        {
            if (check is not IFormLinkContainerGetter)
                return false;

            var links = rule.GetMatchValueAs<List<OperationFormLink>>(valueKey);

            if (!links.SafeAny())
                return true;

            if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(check, rcd.PropertyName, out var curLinks) || !curLinks.SafeAny())
                return !links.Any(k => k.Operation != ListLogic.NOT);

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            foreach (var link in links)
            {
                if (link.Operation != ListLogic.NOT)
                    includesChecked++;

                if (T.Find(curLinks, link.FormKey) != null)
                {
                    // Doesn't matter what overall Operation is we always fail on a NOT match
                    if (link.Operation == ListLogic.NOT)
                        return false;

                    if (valueKey.Operation == FilterLogic.OR)
                        return true;

                    matchedCount++;
                }
                else if (link.Operation != ListLogic.NOT && valueKey.Operation == FilterLogic.AND)
                {
                    return false;
                }
            }

            return valueKey.Operation switch
            {
                FilterLogic.AND => true,
                FilterLogic.XOR => matchedCount == 1,
                _ => includesChecked == 0 // OR
            };
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => origin != null
                && Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(check, rcd.PropertyName, out var curList)
                && Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(origin, rcd.PropertyName, out var originList)
                && RecordsMatch(curList, originList);

        private static bool RecordsMatch ( IReadOnlyList<IFormLinkContainerGetter>? left, IReadOnlyList<IFormLinkContainerGetter>? right )
        {
            if (!left.SafeAny() && !right.SafeAny())
                return true;

            if (!left.SafeAny() || !right.SafeAny() || left.Count != right.Count)
                return false;

            foreach (var l in left)
            {
                if (!right.Any(r => T.DataEquals(l, r)))
                    return false;
            }

            return true;
        }
    }
}