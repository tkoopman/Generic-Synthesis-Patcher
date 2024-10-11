using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class String : IAction
    {
        private const int ClassLogPrefix = 0x200;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (!GetString(context, patchedRecord ?? context.Record, rcd, out string? curValue))
                return -1;

            string? newValue = rule.GetValueAsString(valueKey);

            return FillString(context, origin, rule, rcd, curValue, newValue, ref patchedRecord);
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
                    => (GetString(context, patchedRecord ?? context.Record, rcd, out string? curValue)
                     && GetString(context, forwardContext.Record, rcd, out string? newValue))
                        ? FillString(context, origin, rule, rcd, curValue, newValue, ref patchedRecord)
                        : -1;

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        private static int FillString ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, RecordCallData rcd, string? curValue, string? newValue, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (curValue == null && newValue == null)
                return 0;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return 0;

            if (patchedRecord != null && rule.OnlyIfDefault && origin != null)
            {
                if (GetString(context, origin, rcd, out string? originValue))
                {
                    if (curValue != originValue)
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

            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            var setProperty = patchedRecord.GetType().GetProperty(rcd.PropertyName);
            if (setProperty == null)
            {
                LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x13);
                return -1;
            }

            setProperty.SetValue(patchedRecord, new TranslatedString(Language.English, newValue));
            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"changed.", ClassLogPrefix | 0x14);
            return 1;
        }

        private static bool GetString ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out string? value )
        {
            value = null;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x21);
                return false;
            }

            bool translatedString = property.PropertyType == typeof(ITranslatedStringGetter);

            if (!translatedString && property.PropertyType != typeof(string))
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Error, context, rcd.PropertyName, "string", property.PropertyType.Name, ClassLogPrefix | 0x22);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value != null)
            {
                if (translatedString)
                {
                    if (_value is not ITranslatedStringGetter __value)
                    {
                        LogHelper.LogInvalidTypeFound(LogLevel.Error, context, rcd.PropertyName, "ITranslatedStringGetter", _value.GetType().Name ?? "Unknown", ClassLogPrefix | 0x23);
                        return false;
                    }

                    value = __value.String;
                }
                else
                {
                    if (_value is not string __value)
                    {
                        LogHelper.LogInvalidTypeFound(LogLevel.Error, context, rcd.PropertyName, "string", _value.GetType().Name ?? "Unknown", ClassLogPrefix | 0x24);
                        return false;
                    }

                    value = __value;
                }
            }

            return true;
        }
    }
}