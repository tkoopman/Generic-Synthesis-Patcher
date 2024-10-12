using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLink<T> : IAction where T : class, IMajorRecordGetter
    {
        private const int ClassLogPrefix = 0x500;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IFormLinkGetter<T>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue))
                    return -1;

                var newValue = rule.GetValueAs<FormKey>(valueKey);
                if (!Global.State.LinkCache.TryResolve(newValue, typeof(T), out var link))
                {
                    LogHelper.Log(LogLevel.Warning, context, rcd.PropertyName, $"Unable to find {newValue}", ClassLogPrefix | 0x12);
                    return -1;
                }

                return Fill(context, origin, rule, rcd, curValue, link.ToLinkGetter<T>(), ref patchedRecord);
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x13);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                return Mod.GetProperty<IFormLinkGetter<T>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue)
                    && Mod.GetProperty<IFormLinkGetter<T>>(forwardContext.Record, rcd.PropertyName, out var newValue)
                    ? Fill(context, origin, rule, rcd, curValue, newValue, ref patchedRecord)
                    : -1;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x21);
            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        private static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, RecordCallData rcd, IFormLinkGetter<T>? curValue, IFormLinkGetter<T>? newValue, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                LogHelper.Log(LogLevel.Trace, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x31);
                return 0;
            }

            if (patchedRecord == null && rule.OnlyIfDefault && origin != null)
            {
                if (Mod.GetProperty<IFormLinkGetter<T>>(origin, rcd.PropertyName, out var originValue))
                {
                    if (!(curValue != null && originValue != null)
                        && ((curValue == null && originValue != null)
                             || (curValue != null && originValue == null)
                             || (!curValue?.FormKey.Equals(originValue?.FormKey) ?? throw new Exception("Impossible???"))))
                    {
                        LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch, ClassLogPrefix | 0x32);
                        return -1;
                    }
                }
            }

            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.SetProperty(patchedRecord, rcd.PropertyName, newValue))
                return -1;

            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.", ClassLogPrefix | 0x35);
            return 1;
        }
    }
}