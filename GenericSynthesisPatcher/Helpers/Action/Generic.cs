using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    internal class Generic<T> : IAction where T : IConvertible
    {
        private const int ClassLogPrefix = 0x300;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (!Mod.GetProperty<T>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue))
                return -1;

            var newValue = rule.GetFillValueAs<T>(valueKey);

            return Fill(context, rcd, curValue, newValue, ref patchedRecord);
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
                    => (Mod.GetProperty<T>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue)
                     && Mod.GetProperty<T>(forwardContext.Record, rcd.PropertyName, out var newValue)) ?
                        Fill(context, rcd, curValue, newValue, ref patchedRecord)
                        : -1;

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd )
        {
            var values = rule.GetMatchValueAs<List<OperationValue<T>>>(valueKey);

            if (!values.SafeAny())
                return true;

            if (!Mod.GetProperty<T>(check, rcd.PropertyName, out var curValue) || curValue == null)
                return !values.Any(k => k.Operation != Operation.NOT);

            foreach (var v in values)
            {
                if (curValue.Equals(v.Value))
                {
                    if (v.Operation == Operation.NOT)
                        return false;
                }
                else if (v.Operation != Operation.NOT)
                {   // Always OR as single value field
                    return true;
                }
            }

            return !values.Any(k => k.Operation != Operation.NOT);
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => origin != null
                && Mod.GetProperty<T>(check, rcd.PropertyName, out var curValue)
                && Mod.GetProperty<T>(origin, rcd.PropertyName, out var originValue)
                && !(curValue == null ^ originValue == null)
                && !(curValue != null ^ originValue != null)
                && ((curValue == null && originValue == null)
                   || (curValue != null && originValue != null && curValue.Equals(originValue)));

        private static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, T? curValue, T? newValue, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (curValue == null && newValue == null)
                return 0;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return 0;

            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);

            if (!Mod.SetProperty(patchedRecord, rcd.PropertyName, newValue))
                return -1;

            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Changed.", ClassLogPrefix | 0x14);
            return 1;
        }
    }
}