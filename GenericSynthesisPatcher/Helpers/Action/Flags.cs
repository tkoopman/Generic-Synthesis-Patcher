using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

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

            if (!Mod.GetProperty<Enum>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue) || curValue == null)
                return -1;

            if (patchedRecord == null && rule.OnlyIfDefault && origin != null)
            {
                if (Mod.GetProperty<Enum>(origin, rcd.PropertyName, out var originValue))
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

            if (curValue == newFlags)
                return 0;

            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.SetProperty(patchedRecord, rcd.PropertyName, newFlags))
                return -1;

            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.", ClassLogPrefix | 0x15);
            return 1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();
    }
}