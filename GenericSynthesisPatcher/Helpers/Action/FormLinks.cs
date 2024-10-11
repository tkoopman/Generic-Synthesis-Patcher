using GenericSynthesisPatcher.Json.Data;

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

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, patchedRecord ?? context.Record, rcd, out var curValue))
                    return -1;

                if (rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, origin, rcd, out var originValue))
                    {
                        if (!curValue.SequenceEqualNullable(originValue))
                        {
                            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x11);
                            return -1;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x12);
                        return -1;
                    }
                }

                int changes = 0;
                foreach (string c in rule.GetValueAs<List<string>>(valueKey) ?? [])
                {
                    bool actionRemove = c.StartsWith('-');
                    var actionKey = FormKey.Factory(actionRemove || c.StartsWith('+') ? c[1..] : c);
                    var curKey = curValue?.SingleOrDefault(flg => flg?.FormKey.Equals(actionKey) ?? false, null);

                    if ((curKey != null && actionRemove) || (curKey == null && !actionRemove))
                    {
                        patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                        var setProperty = patchedRecord.GetType().GetProperty(rcd.PropertyName);
                        if (setProperty == null)
                        {
                            LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x13);
                            return -1;
                        }

                        object? _setList = setProperty.GetValue(patchedRecord);
                        if (_setList == null)
                        {
                            LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x14);
                            return -1;
                        }

                        if (_setList is not List<IFormLinkGetter<T>> setList)
                        {
                            LogHelper.LogInvalidTypeFound(LogLevel.Error, context, rcd.PropertyName, "List", _setList.GetType().Name, ClassLogPrefix | 0x15);
                            return -1;
                        }

                        if (!Global.State.LinkCache.TryResolve(actionKey, typeof(T), out var link))
                        {
                            LogHelper.Log(LogLevel.Warning, context, rcd.PropertyName, $"Unable to find {actionKey}", ClassLogPrefix | 0x16);
                        }
                        else if (actionRemove)
                        {
                            _ = setList.Remove(link.ToLinkGetter<T>());
                            changes++;
                        }
                        else
                        {
                            setList.Add(link.ToLinkGetter<T>());
                            changes++;
                        }
                    }
                }

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{changes} change(s).", ClassLogPrefix | 0x17);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x18);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, patchedRecord ?? context.Record, rcd, out var curValue))
                    return -1;

                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, forwardContext.Record, rcd, out var newValue))
                    return -1;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x21);
                    return 0;
                }

                if (patchedRecord != null && rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, origin, rcd, out var originValue))
                    {
                        if (!curValue.SequenceEqualNullable(originValue))
                        {
                            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x22);
                            return -1;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x23);
                        return -1;
                    }
                }

                patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);

                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, patchedRecord, rcd, out var patchValue) || patchValue is not ExtendedList<IFormLinkGetter<T>> pvl)
                    return -1;

                int changes = pvl.RemoveAll(_ => true);

                if (newValue != null)
                {
                    pvl.AddRange(newValue);
                    changes += newValue.Count;
                }

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
                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, patchedRecord ?? context.Record, rcd, out var curValue))
                    return -1;

                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, forwardContext.Record, rcd, out var newValue))
                    return -1;

                if (!newValue.SafeAny())
                    return 0;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x31);
                    return 0;
                }

                if (patchedRecord != null && rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, origin, rcd, out var originValue))
                    {
                        if (!curValue.SequenceEqualNullable(originValue))
                        {
                            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x32);
                            return -1;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x33);
                        return -1;
                    }
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
                                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, patch, rcd, out var patchValue) || patchValue is not ExtendedList<IFormLinkGetter<T>> pvl)
                                    return -1;

                                patchValueLinks = pvl;
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

        private static bool GetFormLinks<LT> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out LT? value ) where LT : class, IEnumerable<IFormLinkGetter<IMajorRecordGetter>>
        {
            value = null;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x41);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null)
                return true;

            if (_value is not LT __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, typeof(LT).Name, _value.GetType().Name, ClassLogPrefix | 0x42);
                return false;
            }

            value = __value;

            return true;
        }
    }
}