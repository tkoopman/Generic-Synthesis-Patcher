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
        private const int ClassLogCode = 0x13;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => false;

        public static bool CanMerge () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IFormLinkGetter<T>>(patchRecord ?? context.Record, rcd.PropertyName, out var curValue, out var propertyInfo))
                    return -1;

                var formKey = rule.GetFillValueAs<FormKeyListOperation<T>>(valueKey);

                if (formKey == null || formKey.Value == FormKey.Null)
                {
                    if (curValue != null && !curValue.IsNull)
                    {
                        if (propertyInfo.PropertyType.IsAssignableTo(typeof(IFormLinkNullableGetter<T>)))
                        {
                            patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                            if (!Mod.GetProperty<IFormLinkNullable<T>>(patchRecord, rcd.PropertyName, out var setValue) || setValue == null)
                                return -1;

                            setValue.SetToNull();
                            Global.DebugLogger?.Log(ClassLogCode, "Set to null.", propertyName: rcd.PropertyName);
                            return 1;
                        }
                        else
                        {
                            Global.Logger.Log(ClassLogCode, "Not nullable.", logLevel: LogLevel.Error, propertyName: rcd.PropertyName);
                            return -1;
                        }
                    }

                    return 0;
                }

                if (formKey.ToLinkGetter() == null)
                {
                    Global.Logger.Log(ClassLogCode, $"Unable to find {formKey?.Value.ToString() ?? "FormKey"}", logLevel: LogLevel.Warning, propertyName: rcd.PropertyName);
                    return -1;
                }

                return Fill(context, rcd, curValue, formKey.ToLinkGetter(), ref patchRecord);
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                return Mod.GetProperty<IFormLinkGetter<T>>(patchRecord ?? context.Record, rcd.PropertyName, out var curValue)
                    && Mod.GetProperty<IFormLinkGetter<T>>(forwardContext.Record, rcd.PropertyName, out var newValue)
                    ? Fill(context, rcd, curValue, newValue, ref patchRecord)
                    : -1;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name);
            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord ) => throw new NotImplementedException();

        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, FilterOperation valueKey, RecordCallData rcd )
        {
            if (valueKey.Operation != FilterLogic.OR)
                Global.Logger.Log(ClassLogCode, $"Invalid operation for checking a single value. Default OR only valid for this property. Continuing check as OR.", logLevel: LogLevel.Warning, propertyName: rcd.PropertyName);

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

        public static int Merge ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord ) => throw new NotImplementedException();

        private static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, IFormLinkGetter<T>? curValue, IFormLinkGetter<T>? newValue, ref ISkyrimMajorRecord? patchRecord )
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: rcd.PropertyName);
                return 0;
            }

            patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.SetProperty(patchRecord, rcd.PropertyName, newValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Updated.", propertyName: rcd.PropertyName);
            return 1;
        }
    }
}