using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes 0x1xx
    internal static class String
    {
        // Log Codes: 0x110
        private static bool GetString ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, string propertyName, out string? value )
        {
            value = null;
            var property = record.GetType().GetProperty(propertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, propertyName, $"Failed to find property. Skipping.", 0x110);
                return false;
            }

            bool translatedString = property.PropertyType == typeof(ITranslatedStringGetter);

            if (!translatedString && property.PropertyType != typeof(string))
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Error, context, propertyName, "string", property.PropertyType.Name, 0x111);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value != null)
            {
                if (translatedString)
                {
                    if (_value is not ITranslatedStringGetter __value)
                    {
                        LogHelper.LogInvalidTypeFound(LogLevel.Error, context, propertyName, "ITranslatedStringGetter", _value.GetType().Name ?? "Unknown", 0x112);
                        return false;
                    }

                    value = __value.String;
                }
                else
                {
                    if (_value is not string __value)
                    {
                        LogHelper.LogInvalidTypeFound(LogLevel.Error, context, propertyName, "string", _value.GetType().Name ?? "Unknown", 0x113);
                        return false;
                    }

                    value = __value;
                }
            }

            return true;
        }

        // Log Codes: 0x12x
        private static bool FillString ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, string propertyName, string? curValue, string? newValue )
        {
            if (curValue == null && newValue == null)
                return false;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return false;

            if (rule.OnlyIfDefault && origin != null)
            {
                if (GetString(context, origin, propertyName, out string? originValue))
                {
                    if (curValue != originValue)
                    {
                        LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as property doesn't match origin");
                        return false;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find origin property to check.", 0x121);
                    return false;
                }
            }

            var patch = context.GetOrAddAsOverride(Global.State.PatchMod);
            var setProperty = patch.GetType().GetProperty(propertyName);
            if (setProperty == null)
            {
                LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find property to set new value to.", 0x122);
                return false;
            }

            setProperty.SetValue(patch, new TranslatedString(Language.English, newValue));
            LogHelper.Log(LogLevel.Debug, context, propertyName, $"Set to {newValue}.");
            return true;

        }

        // Log Codes: 0x13x
        public static bool FillString ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, string propertyName )
        {
            if (!GetString(context, context.Record, propertyName, out string? curValue))
                return false;

            string? newValue = rule.GetValueAsString(valueKey);

            return FillString(context, origin, rule, propertyName, curValue, newValue);

        }

        public static bool ForwardString ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IMajorRecordGetter forwardRecord, string propertyName )
            => GetString(context, context.Record, propertyName, out string? curValue)
            && GetString(context, forwardRecord, propertyName, out string? newValue)
            && FillString(context, origin, rule, propertyName, curValue, newValue);

    }
}