using EnumsNET;

using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

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

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            var flags = rule.GetFillValueAs<List<string>>(valueKey);
            if (context.Record == null || flags == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "No flags to set.", ClassLogPrefix | 0x11);
                return -1;
            }

            if (!Mod.GetProperty<Enum>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue) || curValue == null)
                return -1;

            var flagType = curValue.GetType();
            var newFlags = curValue;

            foreach (string f in flags)
            {
                var checkFlag = new ListOperation(f);

                if (Enum.TryParse(flagType, checkFlag.Value, true, out object? setFlag))
                {
                    newFlags = checkFlag.Operation == ListLogic.DEL
                        ? (Enum)FlagEnums.RemoveFlags(flagType, newFlags, setFlag)
                        : (Enum)FlagEnums.CombineFlags(flagType, newFlags, setFlag);
                }
            }

            if (curValue == newFlags)
                return 0;

            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.SetProperty(patchedRecord, rcd.PropertyName, newFlags))
                return -1;

            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.", ClassLogPrefix | 0x15);
            return 1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2248:Provide correct 'enum' argument to 'Enum.HasFlag'", Justification = "They do match just errors due to generic nature.")]
        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd )
        {
            if (!Mod.GetProperty<Enum>(check, rcd.PropertyName, out var checkFlags))
                return false;

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            var flags = rule.GetMatchValueAs<List<ListOperation>>(valueKey);
            if (!flags.SafeAny())
                return true;

            if (checkFlags == null)
                return !flags.Any(k => k.Operation == ListLogic.Default);

            var flagType = checkFlags.GetType();

            foreach (var flag in flags)
            {
                if (!Enum.TryParse(flagType, flag.Value, true, out object? checkFlag) || checkFlag == null)
                    continue;

                if (flag.Operation != ListLogic.NOT)
                    includesChecked++;

                if (checkFlags.HasFlag((Enum)checkFlag))
                {
                    // Doesn't matter what overall Operation is we always fail on a NOT match
                    if (flag.Operation == ListLogic.NOT)
                        return false;

                    if (valueKey.Operation == FilterLogic.OR)
                        return true;

                    matchedCount++;
                }
                else if (flag.Operation != ListLogic.NOT && valueKey.Operation == FilterLogic.AND)
                {
                    return false;
                }
            }

            return valueKey.Operation switch
            {
                FilterLogic.AND => true,
                FilterLogic.XOR => matchedCount == 1,
                _ => includesChecked == 0 // OR
            };
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => origin != null
                && Mod.GetProperty<Enum>(check, rcd.PropertyName, out var curValue)
                && Mod.GetProperty<Enum>(origin, rcd.PropertyName, out var originValue)
                && curValue == originValue;
    }
}