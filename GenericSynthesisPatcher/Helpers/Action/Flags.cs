using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    internal class Flags : IAction
    {
        private const int ClassLogPrefix = 0x400;

        public static bool CanFill () => true;

        public static bool CanForward () => false;

        public static bool CanForwardSelfOnly () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            var flags = rule.GetValueAs<List<string>>(valueKey);
            if (context.Record == null || flags == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "No flags to set.", ClassLogPrefix | 0x11);
                return -1;
            }

            if (!GetFlags(context, patchedRecord ?? context.Record, rcd, out var curValue) || curValue == null)
                return -1;

            if (patchedRecord != null && rule.OnlyIfDefault && origin != null)
            {
                if (GetFlags(context, origin, rcd, out var originValue))
                {
                    if (curValue != originValue)
                    {
                        LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x12);
                        return -1;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, $"Unable to find origin keywords to check.", ClassLogPrefix | 0x13);
                    return -1;
                }
            }

            var flagType = curValue.GetType();
            var newFlags = curValue;

            foreach (string f in flags)
            {
                object? setFlag;
                string s = f;
                if (s.StartsWith('-'))
                {
                    s = s[1..];

                    if (!Enum.TryParse(flagType, s, true, out setFlag) || setFlag == null || !setFlag.GetType().IsEnum)
                        continue;

                    newFlags = (Enum)FlagEnums.RemoveFlags(flagType, newFlags, setFlag);
                    continue;
                }

                if (s.StartsWith('+'))
                    s = s[1..];

                if (!Enum.TryParse(flagType, s, true, out setFlag) || setFlag == null || !setFlag.GetType().IsEnum)
                    continue;

                newFlags = (Enum)FlagEnums.CombineFlags(flagType, newFlags, setFlag);
            }

            if (curValue != newFlags)
            {
                patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                var setFlagProp = patchedRecord.GetType().GetProperty(rcd.PropertyName);
                if (setFlagProp == null)
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x14);
                    return -1;
                }

                setFlagProp.SetValue(patchedRecord, newFlags);
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.", ClassLogPrefix | 0x15);
                return 1;
            }

            return 0;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        private static bool GetFlags ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out Enum? value )
        {
            value = null;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x21);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null || !FlagEnums.IsFlagEnum(_value.GetType()))
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "FlagEnums", _value?.GetType().Name ?? "?", ClassLogPrefix | 0x22);
                return false;
            }

            value = (Enum)_value;
            return true;
        }
    }
}