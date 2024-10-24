using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLinksWithData<T, TMajor> : IAction
        where T : class, IFormLinksWithData<T, TMajor>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        private const int ClassLogCode = 0x15;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => true;

        public static bool CanMerge () => T.CanMerge();

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(patchRecord ?? context.Record, rcd.PropertyName, out var curList))
                    return -1;

                int changes = 0;

                foreach (var action in T.GetFillValueAs(rule, valueKey) ?? [])
                {
                    if (action?.FormKey == null || action.FormKey.Value == FormKey.Null)
                    {
                        if (curList != null && curList.Count > 0)
                        {
                            patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);

                            if (!Mod.ClearProperty(patchRecord, rcd.PropertyName))
                            {
                                Global.Logger.Log(ClassLogCode, LogHelper.MissingProperty, logLevel: LogLevel.Error, propertyName: rcd.PropertyName);
                                return -1;
                            }

                            curList = [];
                            changes++;
                        }

                        continue;
                    }

                    var e = action.FindFormKey(curList);

                    if (e != null && (action.FormKey.Operation == ListLogic.DEL || !action.DataEquals(e)))
                    {
                        _ = T.Remove(context, rcd, ref patchRecord, e);
                        changes++;
                    }

                    if (action.FormKey.Operation == ListLogic.ADD && (e == null || (e != null && !action.DataEquals(e))))
                    {
                        _ = action.Add(context, rcd, ref patchRecord);
                        changes++;
                    }
                }

                if (changes > 0)
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: rcd.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(patchRecord ?? context.Record, rcd.PropertyName, out var curList))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(forwardContext.Record, rcd.PropertyName, out var newList))
                    return -1;

                if (curList.SequenceEqualNullable(newList))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: rcd.PropertyName);
                    return 0;
                }

                int changes = T.Replace(context, rcd, ref patchRecord, newList);

                if (changes > 0)
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: rcd.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name);

            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(patchRecord ?? context.Record, rcd.PropertyName, out var curList))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(forwardContext.Record, rcd.PropertyName, out var newList))
                    return -1;

                if (!newList.SafeAny())
                    return 0;

                if (curList.SequenceEqualNullable(newList))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: rcd.PropertyName);
                    return 0;
                }

                int changes = 0;
                foreach (var item in newList)
                {
                    var key = T.GetFormKeyFromRecord(item);
                    if (key.ModKey == forwardContext.ModKey)
                    {
                        var i = T.FindRecord(curList, key);

                        if (i != null && !T.DataEquals(item, i))
                        {
                            _ = T.Remove(context, rcd, ref patchRecord, i);
                            _ = T.Forward(context, rcd, ref patchRecord, item);
                            changes++;
                        }

                        if (i == null)
                        {
                            _ = T.Forward(context, rcd, ref patchRecord, item);
                            changes++;
                        }
                    }
                }

                if (changes > 0)
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: rcd.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name);
            return -1;
        }

        /// <summary>
        /// Only checks the FormKeys not the Data
        /// </summary>
        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, FilterOperation valueKey, RecordCallData rcd )
        {
            if (check is not IFormLinkContainerGetter)
                return false;

            var links = rule.GetMatchValueAs<List<FormKeyListOperation<TMajor>>>(valueKey);

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

                if (T.FindRecord(curLinks, link.Value) != null)
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

        /// <summary>
        /// Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if all form keys and data matches</returns>
        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => origin != null
                && Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(check, rcd.PropertyName, out var curList)
                && Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(origin, rcd.PropertyName, out var originList)
                && RecordsMatch(curList, originList);

        public static int Merge ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord ) => T.Merge(context, rule, valueKey, rcd, ref patchRecord);

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