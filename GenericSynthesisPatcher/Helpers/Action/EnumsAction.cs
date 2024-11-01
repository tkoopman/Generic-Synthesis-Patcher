using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    internal class EnumsAction : IRecordAction
    {
        public static readonly EnumsAction Instance = new();
        private const int ClassLogCode = 0x11;

        private EnumsAction ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => false;

        public bool CanForwardSelfOnly () => false;

        public bool CanMatch () => true;

        public bool CanMerge () => false;

        public int Fill (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetFillValueAs(out string? setValueStr) || setValueStr == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, $"No {proKeys.Property.PropertyName} to set.", propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (!Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue) || curValue == null)
                return -1;

            var enumType = curValue.GetType();
            if (!Enum.TryParse(enumType, setValueStr, true, out object? setValue))
            {
                Global.Logger.Log(ClassLogCode, $"{setValueStr} is not a valid value for {proKeys.Property.PropertyName}.", logLevel: LogLevel.Warning, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (curValue.Equals(setValue))
                return 0;

            if (!Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, setValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Updated.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.TryGetProperty<Enum>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.TryGetProperty<Enum>(origin, proKeys.Property.PropertyName, out var originValue)
                        && curValue == originValue;
        }

        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (proKeys.RuleKey.Operation != FilterLogic.OR)
                Global.Logger.Log(ClassLogCode, $"Invalid operation for checking a single value. Default OR only valid for this property. Continuing check as OR.", logLevel: LogLevel.Warning, propertyName: proKeys.Property.PropertyName);

            if (!Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                return false;

            int includesChecked = 0; // Only count !Neg

            if (!proKeys.TryGetMatchValueAs(out _, out List<ListOperation>? checkValues))
                return false;

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

        public int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();
    }
}