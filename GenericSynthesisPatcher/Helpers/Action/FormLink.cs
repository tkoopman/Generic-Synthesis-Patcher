using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLink<T> : IAction where T : class, IMajorRecordGetter
    {
        private const int ClassLogPrefix = 0x500;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!GetFormLink<IFormLinkGetter<T>>(context, patchedRecord ?? context.Record, rcd, out var curValue))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"Unable to get current value", ClassLogPrefix | 0x11);
                    return -1;
                }

                var newValue = rule.GetValueAs<FormKey>(valueKey);
                if (!Global.State.LinkCache.TryResolve(newValue, typeof(T), out var link))
                {
                    LogHelper.Log(LogLevel.Warning, context, rcd.PropertyName, $"Unable to find {newValue}", ClassLogPrefix | 0x12);
                    return -1;
                }

                return Fill(context, origin, rule, rcd, curValue, link.ToLinkGetter<T>(), ref patchedRecord);
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x13);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!GetFormLink<IFormLinkGetter<T>>(context, patchedRecord ?? context.Record, rcd, out var curValue))
                    return -1;

                if (!GetFormLink<IFormLinkGetter<T>>(context, forwardContext.Record, rcd, out var newValue))
                    return -1;

                return Fill(context, origin, rule, rcd, curValue, newValue, ref patchedRecord);
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x21);
            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        private static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, RecordCallData rcd, IFormLinkGetter<T>? curValue, IFormLinkGetter<T>? newValue, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                LogHelper.Log(LogLevel.Trace, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x31);
                return 0;
            }

            if (patchedRecord != null && rule.OnlyIfDefault && origin != null)
            {
                if (GetFormLink<IFormLinkGetter<T>>(context, origin, rcd, out var originValue))
                {
                    if (!(curValue != null && originValue != null)
                        && ((curValue == null && originValue != null)
                             || (curValue != null && originValue == null)
                             || (!curValue?.FormKey.Equals(originValue?.FormKey) ?? throw new Exception("Impossible???"))))
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

            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            var setProperty = patchedRecord.GetType().GetProperty(rcd.PropertyName);
            if (setProperty == null)
            {
                LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x34);
                return -1;
            }

            setProperty.SetValue(patchedRecord, newValue);
            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.", ClassLogPrefix | 0x35);

            return 1;
        }

        private static bool GetFormLink<LT> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out LT? value ) where LT : class, IFormLinkGetter<IMajorRecordGetter>
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