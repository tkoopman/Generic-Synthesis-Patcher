using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using static GenericSynthesisPatcher.Program;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x3xx
    internal class Flags : IAction
    {
        // Log Codes: 0x31x
        private static bool GetFlags ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out Enum? value )
        {
            value = null;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"Failed to find property. Skipping.", 0x311);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null || !FlagEnums.IsFlagEnum(_value.GetType()))
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "FlagEnums", _value?.GetType().Name ?? "?", 0x312);
                return false;
            }

            value = (Enum)_value;
            return true;
        }
        // Log Codes: 0x32x
        public static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd )
        {
            var flags = rule.GetValueAs<List<string>>(valueKey);
            if (context.Record == null || flags == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "No flags to set.");
                return false;
            }

            if (!GetFlags(context, context.Record, rcd, out var curValue) || curValue == null)
                return false;

            if (rule.OnlyIfDefault && origin != null)
            {
                if (GetFlags(context, origin, rcd, out var originValue))
                {
                    if (curValue != originValue)
                    {
                        LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Skipping as keywords don't match origin");
                        return false;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, $"Unable to find origin keywords to check.", 0x321);
                    return false;
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
                var patch = (INamed)context.GetOrAddAsOverride(Global.State.PatchMod);
                var setFlagProp = patch.GetType().GetProperty(rcd.PropertyName);
                if (setFlagProp == null)
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"Failed to find property. Skipping.", 0x322);
                    return false;
                }

                setFlagProp.SetValue(patch, newFlags);
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");
                return true;
            }

            return false;
        }

        public static bool CanFill () => true;
        public static bool CanForward () => false;
        public static bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd ) => throw new NotImplementedException();
    }
}