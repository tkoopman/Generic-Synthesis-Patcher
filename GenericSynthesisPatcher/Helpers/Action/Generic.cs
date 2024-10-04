using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x6xx
    internal static class Generic
    {
        // Log Codes: 0x61x
        private static bool Get<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, string propertyName, out T? value )
        {
            value = default;
            var property = record.GetType().GetProperty(propertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, propertyName, $"Failed to find property. Skipping.", 0x611);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null || _value is not T __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, typeof(T).Name, _value?.GetType().Name ?? "?", 0x612);
                return false;
            }

            value = __value;

            return true;
        }

        // Log Codes: 0x62x
        private static bool Fill<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, string propertyName, T? curValue, T? newValue )
        {
            if (curValue == null && newValue == null)
                return false;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return false;

            if (rule.OnlyIfDefault && origin != null)
            {
                if (Get<T>(context, origin, propertyName, out var originValue))
                {
                    if ((curValue != null || originValue != null) && (curValue == null || originValue == null || !curValue.Equals(originValue)))
                    {
                        LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as property doesn't match origin");
                        return false;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find origin property to check.", 0x621);
                    return false;
                }
            }

            var patch = context.GetOrAddAsOverride(Global.State.PatchMod);

            var setProperty = patch.GetType().GetProperty(propertyName);
            if (setProperty == null)
            {
                LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find property to set new value to.", 0x622);
                return false;
            }

            setProperty.SetValue(patch, newValue);
            LogHelper.Log(LogLevel.Debug, context, propertyName, $"Set to {newValue}.");
            return true;
        }

        public static bool Fill<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, string propertyName )
        {
            if (!Get<T>(context, context.Record, propertyName, out var curValue))
                return false;
            
            var newValue = rule.GetValueAs<T>(valueKey);

            return Fill(context, origin, rule, propertyName, curValue, newValue);
        }

        public static bool Forward<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IMajorRecordGetter forwardRecord, string propertyName ) 
            => Get<T>(context, context.Record, propertyName, out var curValue) 
            && Get<T>(context, forwardRecord, propertyName, out var newValue)
            && Fill(context, origin, rule, propertyName, curValue, newValue);
    }
}