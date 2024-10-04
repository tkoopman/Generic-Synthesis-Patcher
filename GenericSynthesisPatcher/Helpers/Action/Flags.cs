using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x3xx
    internal static class Flags
    {
        // Log Codes: 0x31x
        private static bool GetFlags ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, string propertyName, out Enum? value )
        {
            value = null;
            var property = record.GetType().GetProperty(propertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, propertyName, $"Failed to find property. Skipping.", 0x311);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null || !FlagEnums.IsFlagEnum(_value.GetType()))
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "FlagEnums", _value?.GetType().Name ?? "?", 0x312);
                return false;
            }

            value = (Enum)_value;
            return true;
        }
        // Log Codes: 0x32x
        public static bool FillFlags ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, string propertyName )
        {
            var flags = rule.GetValueAs<List<string>>(valueKey);
            if (context.Record == null || flags == null)
            {
                LogHelper.Log(LogLevel.Debug, context, propertyName, "No flags to set.");
                return false;
            }

            if (!GetFlags(context, context.Record, propertyName, out var curValue) || curValue == null)
                return false;

            if (rule.OnlyIfDefault && origin != null)
            {
                if (GetFlags(context, origin, propertyName, out var originValue))
                {
                    if (curValue != originValue)
                    {
                        LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as keywords don't match origin");
                        return false;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find origin keywords to check.", 0x321);
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
                var setFlagProp = patch.GetType().GetProperty(propertyName);
                if (setFlagProp == null)
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, $"Failed to find property. Skipping.", 0x322);
                    return false;
                }

                setFlagProp.SetValue(patch, newFlags);
                LogHelper.Log(LogLevel.Debug, context, propertyName, "Updated.");
                return true;
            }

            return false;
        }
    }
}