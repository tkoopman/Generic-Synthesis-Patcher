using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    internal class Generic<T> : IAction
    {
        private const int ClassLogPrefix = 0x300;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (!Mod.GetProperty<T>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue))
                return -1;

            var newValue = rule.GetValueAs<T>(valueKey);

            return Fill(context, origin, rule, rcd, curValue, newValue, ref patchedRecord);
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
                    => (Mod.GetProperty<T>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue)
                     && Mod.GetProperty<T>(forwardContext.Record, rcd.PropertyName, out var newValue)) ?
                        Fill(context, origin, rule, rcd, curValue, newValue, ref patchedRecord)
                        : -1;

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        private static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, RecordCallData rcd, T? curValue, T? newValue, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (curValue == null && newValue == null)
                return 0;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return 0;

            if (rule.OnlyIfDefault && origin != null)
            {
                if (Mod.GetProperty<T>(origin, rcd.PropertyName, out var originValue))
                {
                    if ((curValue != null || originValue != null) && (curValue == null || originValue == null || !curValue.Equals(originValue)))
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

            if (!Mod.SetProperty(patchedRecord, rcd.PropertyName, newValue))
                return -1;

            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Changed.", ClassLogPrefix | 0x14);
            return 1;
        }
    }
}