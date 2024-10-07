using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x6xx
    internal class Generic<T> : IAction
    {
        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd )
        {
            if (!Get(context, context.Record, rcd, out var curValue))
                return false;

            var newValue = rule.GetValueAs<T>(valueKey);

            return Fill(context, origin, rule, rcd, curValue, newValue);
        }

        public static bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd )
                    => Get(context, context.Record, rcd, out var curValue)
                    && Get(context, forwardContext.Record, rcd, out var newValue)
                    && Fill(context, origin, rule, rcd, curValue, newValue);

        // Log Codes: 0x62x
        private static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, RecordCallData rcd, T? curValue, T? newValue )
        {
            if (curValue == null && newValue == null)
                return false;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return false;

            if (rule.OnlyIfDefault && origin != null)
            {
                if (Get(context, origin, rcd, out var originValue))
                {
                    if ((curValue != null || originValue != null) && (curValue == null || originValue == null || !curValue.Equals(originValue)))
                    {
                        LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch);
                        return false;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, 0x621);
                    return false;
                }
            }

            var patch = context.GetOrAddAsOverride(Global.State.PatchMod);

            var setProperty = patch.GetType().GetProperty(rcd.PropertyName);
            if (setProperty == null)
            {
                LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, 0x622);
                return false;
            }

            setProperty.SetValue(patch, newValue);
            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"Set to {newValue}.");
            return true;
        }

        // Log Codes: 0x61x
        private static bool Get ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out T? value )
        {
            value = default;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.MissingProperty, 0x611);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null || _value is not T __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, typeof(T).Name, _value?.GetType().Name ?? "?", 0x612);
                return false;
            }

            value = __value;

            return true;
        }
    }
}