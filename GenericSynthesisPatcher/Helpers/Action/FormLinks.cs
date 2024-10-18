using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLinks<T> : IAction
        where T : class, IMajorRecordGetter
    {
        private const int ClassLogPrefix = 0x600;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => true;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue))
                    return -1;

                int changes = 0;
                foreach (var actionKey in rule.GetFillValueAs<List<FormKeyListOperation<T>>>(valueKey) ?? [])
                {
                    var curKey = curValue?.SingleOrDefault(k => k?.FormKey.Equals(actionKey.Value) ?? false, null);

                    if ((curKey != null && actionKey.Operation == ListLogic.DEL) || (curKey == null && actionKey.Operation == ListLogic.ADD))
                    {
                        patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);

                        if (!Mod.GetPropertyForEditing<List<IFormLinkGetter<T>>>(patchedRecord, rcd.PropertyName, out var setList))
                        {
                            LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x13);
                            return -1;
                        }

                        if (!Global.State.LinkCache.TryResolve(actionKey.Value, typeof(T), out var link))
                        {
                            LogHelper.Log(LogLevel.Warning, context, rcd.PropertyName, $"Unable to find {actionKey}", ClassLogPrefix | 0x16);
                            continue;
                        }

                        changes++;
                        if (actionKey.Operation == ListLogic.DEL)
                            _ = setList.Remove(link.ToLinkGetter<T>());
                        else
                            setList.Add(link.ToLinkGetter<T>());
                    }
                }

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x17);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x18);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(forwardContext.Record, rcd.PropertyName, out var newValue))
                    return -1;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x21);
                    return 0;
                }

                patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);

                if (!Mod.GetPropertyForEditing<ExtendedList<IFormLinkGetter<T>>>(patchedRecord, rcd.PropertyName, out var patchValue))
                {
                    LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, "Patch has null value.", ClassLogPrefix | 0x26);
                    return -1;
                }

                int changes = patchValue.RemoveAll(_ => true);

                if (newValue != null)
                {
                    patchValue.AddRange(newValue);
                    changes += newValue.Count;
                }

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
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(forwardContext.Record, rcd.PropertyName, out var newValue))
                    return -1;

                if (!newValue.SafeAny())
                    return 0;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    LogHelper.Log(LogLevel.Trace, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x31);
                    return 0;
                }

                ISkyrimMajorRecord? patch = null;
                ExtendedList<IFormLinkGetter<T>>? patchValueLinks = null;
                int changes = 0;

                foreach (var item in newValue)
                {
                    if (item.FormKey.ModKey == forwardContext.ModKey)
                    {
                        if (curValue == null || !curValue.Contains(item))
                        {
                            if (patchValueLinks == null)
                            {
                                patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                                if (!Mod.GetPropertyForEditing<ExtendedList<IFormLinkGetter<T>>>(patch, rcd.PropertyName, out var patchValue))
                                    return -1;

                                patchValueLinks = patchValue;
                            }

                            patchValueLinks.Add(item);
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

        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd )
        {
            if (check is not IFormLinkContainerGetter)
                return false;

            var links = rule.GetMatchValueAs<List<FormKeyListOperation<T>>>(valueKey);

            if (!links.SafeAny())
                return true;

            if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(check, rcd.PropertyName, out var curLinks) || !curLinks.SafeAny())
                return !links.Any(k => k.Operation != ListLogic.NOT);

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            foreach (var link in links)
            {
                if (link.Operation != ListLogic.NOT)
                    includesChecked++;

                if (curLinks.Any(l => l.FormKey.Equals(link.Value)))
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
                && Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(check, rcd.PropertyName, out var checkValue)
                && Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(origin, rcd.PropertyName, out var originValue)
                && RecordsMatch(checkValue, originValue);

        private static bool RecordsMatch ( IReadOnlyList<IFormLinkGetter<T>>? left, IReadOnlyList<IFormLinkGetter<T>>? right )
        {
            if (!left.SafeAny() && !right.SafeAny())
                return true;

            if (!left.SafeAny() || !right.SafeAny() || left.Count != right.Count)
                return false;

            foreach (var l in left)
            {
                if (!right.Any(r => l.FormKey.Equals(r.FormKey)))
                    return false;
            }

            return true;
        }
    }
}