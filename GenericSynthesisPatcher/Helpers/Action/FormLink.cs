using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLink<T> : IAction where T : class, IMajorRecordGetter
    {
        private const int ClassLogPrefix = 0x500;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IFormLinkGetter<T>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue))
                    return -1;

                var formKey = rule.GetFillValueAs<FormKeyListOperation<T>>(valueKey);
                if (formKey?.ToLinkGetter() == null)
                {
                    LogHelper.Log(LogLevel.Warning, context, rcd.PropertyName, $"Unable to find {formKey?.Value.ToString() ?? "FormKey"}", ClassLogPrefix | 0x12);
                    return -1;
                }

                return Fill(context, rcd, curValue, formKey.ToLinkGetter(), ref patchedRecord);
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x13);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                return Mod.GetProperty<IFormLinkGetter<T>>(patchedRecord ?? context.Record, rcd.PropertyName, out var curValue)
                    && Mod.GetProperty<IFormLinkGetter<T>>(forwardContext.Record, rcd.PropertyName, out var newValue)
                    ? Fill(context, rcd, curValue, newValue, ref patchedRecord)
                    : -1;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, ClassLogPrefix | 0x21);
            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => throw new NotImplementedException();

        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd )
        {
            if (valueKey.Operation != FilterLogic.OR)
                LogHelper.Log(LogLevel.Warning, check, rcd.PropertyName, $"Invalid operation for checking a single value. Default OR only valid for this property. Continuing check as OR.", 0x41);

            if (check is not IFormLinkContainerGetter)
                return false;

            var values = rule.GetMatchValueAs<List<FormKeyListOperationAdvanced<T>>>(valueKey);

            if (!values.SafeAny())
                return true;

            if (!Mod.GetProperty<IFormLinkGetter<T>>(check, rcd.PropertyName, out var curValue) || curValue == null)
                return !values.Any(k => k.Operation != ListLogic.NOT);

            foreach (var v in values)
            {
                if (v.Regex != null)
                {
                    var link = curValue.TryResolve(Global.State.LinkCache);
                    if (link != null && link.EditorID != null && v.Regex.IsMatch(link.EditorID))
                        return v.Operation != ListLogic.NOT;
                }
                else if (curValue.FormKey.Equals(v.Value))
                {
                    return v.Operation != ListLogic.NOT;
                }
            }

            return !values.Any(k => k.Operation != ListLogic.NOT);
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => origin != null
                && Mod.GetProperty<IFormLinkGetter<T>>(check, rcd.PropertyName, out var curValue)
                && Mod.GetProperty<IFormLinkGetter<T>>(origin, rcd.PropertyName, out var originValue)
                && !(curValue == null ^ originValue == null)
                && !(curValue != null ^ originValue != null)
                && ((curValue == null && originValue == null)
                   || (curValue != null && originValue != null && curValue.FormKey.Equals(originValue.FormKey)));

        private static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, IFormLinkGetter<T>? curValue, IFormLinkGetter<T>? newValue, ref ISkyrimMajorRecord? patchedRecord )
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                LogHelper.Log(LogLevel.Trace, context, rcd.PropertyName, LogHelper.PropertyIsEqual, ClassLogPrefix | 0x31);
                return 0;
            }

            patchedRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.SetProperty(patchedRecord, rcd.PropertyName, newValue))
                return -1;

            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.", ClassLogPrefix | 0x35);
            return 1;
        }
    }
}