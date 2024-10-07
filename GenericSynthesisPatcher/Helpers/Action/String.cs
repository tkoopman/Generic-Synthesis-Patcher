using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes 0x1xx
    public class String : IAction
    {
        public static bool CanFill () => true;

        public static bool CanForward () => true;

        // Log Codes: 0x13x
        public static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd )
        {
            if (!GetString(context, context.Record, rcd, out string? curValue))
                return false;

            string? newValue = rule.GetValueAsString(valueKey);

            return FillString(context, origin, rule, rcd, curValue, newValue);
        }

        public static bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd )
                    => GetString(context, context.Record, rcd, out string? curValue)
                    && GetString(context, forwardContext.Record, rcd, out string? newValue)
                    && FillString(context, origin, rule, rcd, curValue, newValue);

        // Log Codes: 0x12x
        private static bool FillString ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, RecordCallData rcd, string? curValue, string? newValue )
        {
            if (curValue == null && newValue == null)
                return false;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return false;

            if (rule.OnlyIfDefault && origin != null)
            {
                if (GetString(context, origin, rcd, out string? originValue))
                {
                    if (curValue != originValue)
                    {
                        LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch);
                        return false;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, 0x121);
                    return false;
                }
            }

            var patch = context.GetOrAddAsOverride(Global.State.PatchMod);
            var setProperty = patch.GetType().GetProperty(rcd.PropertyName);
            if (setProperty == null)
            {
                LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, 0x122);
                return false;
            }

            setProperty.SetValue(patch, new TranslatedString(Language.English, newValue));
            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"Set to {newValue}.");
            return true;
        }

        // Log Codes: 0x110
        private static bool GetString ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out string? value )
        {
            value = null;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.MissingProperty, 0x110);
                return false;
            }

            bool translatedString = property.PropertyType == typeof(ITranslatedStringGetter);

            if (!translatedString && property.PropertyType != typeof(string))
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Error, context, rcd.PropertyName, "string", property.PropertyType.Name, 0x111);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value != null)
            {
                if (translatedString)
                {
                    if (_value is not ITranslatedStringGetter __value)
                    {
                        LogHelper.LogInvalidTypeFound(LogLevel.Error, context, rcd.PropertyName, "ITranslatedStringGetter", _value.GetType().Name ?? "Unknown", 0x112);
                        return false;
                    }

                    value = __value.String;
                }
                else
                {
                    if (_value is not string __value)
                    {
                        LogHelper.LogInvalidTypeFound(LogLevel.Error, context, rcd.PropertyName, "string", _value.GetType().Name ?? "Unknown", 0x113);
                        return false;
                    }

                    value = __value;
                }
            }

            return true;
        }
    }
}