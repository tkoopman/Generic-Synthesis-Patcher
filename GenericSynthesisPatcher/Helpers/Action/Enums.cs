using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    internal class Enums : IAction
    {
        private const int ClassLogPrefix = 0x900;

        public static bool CanFill () => true;

        public static bool CanForward () => false;

        public static bool CanForwardSelfOnly () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            string? setValueStr = rule.GetFillValueAs<string>(valueKey);
            if (context.Record == null || setValueStr == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"No {rcd.PropertyName} to set.", ClassLogPrefix | 0x11);
                return -1;
            }

            if (!Mod.GetProperty<Enum>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue) || curValue == null)
                return -1;

            var enumType = curValue.GetType();
            if (!Enum.TryParse(enumType, setValueStr, true, out object? setValue))
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"{setValueStr} is not a valid value for {rcd.PropertyName}.", ClassLogPrefix | 0x12);
                return -1;
            }

            if (curValue.Equals(setValue))
                return 0;

            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.SetProperty(patchedRecord, rcd.PropertyName, setValue))
                return -1;

            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.", ClassLogPrefix | 0x13);
            return 1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd )
        {
            if (valueKey.Operation != FilterLogic.OR)
                LogHelper.Log(LogLevel.Warning, check, rcd.PropertyName, $"Invalid operation for checking a single value. Default OR only valid for this property. Continuing check as OR.", 0x21);

            if (!Mod.GetProperty<Enum>(check, rcd.PropertyName, out var curValue))
                return false;

            int includesChecked = 0; // Only count !Neg

            var checkValues = rule.GetMatchValueAs<List<ListOperation>>(valueKey);
            if (!checkValues.SafeAny())
                return true;

            if (curValue == null)
                return !checkValues.Any(k => k.Operation == ListLogic.Default);

            var valueType = curValue.GetType();

            foreach (var checkValueOp in checkValues)
            {
                if (!Enum.TryParse(valueType, checkValueOp.Value, true, out object? checkValue) || checkValue == null)
                    continue;

                if (checkValueOp.Operation != ListLogic.NOT)
                    includesChecked++;

                if (curValue.Equals(checkValue))
                    return checkValueOp.Operation != ListLogic.NOT;
            }

            return includesChecked == 0;
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => origin != null
                && Mod.GetProperty<Enum>(check, rcd.PropertyName, out var curValue)
                && Mod.GetProperty<Enum>(origin, rcd.PropertyName, out var originValue)
                && curValue == originValue;
    }
}