using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLink<T> : IRecordAction where T : class, IMajorRecordGetter
    {
        public static readonly FormLink<T> Instance = new();
        private const int ClassLogCode = 0x13;

        private FormLink ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => false;

        public bool CanMerge () => false;

        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IFormLinkGetter<T>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, out var propertyInfo))
                    return -1;

                var formKey = proKeys.GetFillValueAs<FormKeyListOperation<T>>();

                if (formKey == null || formKey.Value == FormKey.Null)
                {
                    if (curValue != null && !curValue.IsNull)
                    {
                        if (propertyInfo.PropertyType.IsAssignableTo(typeof(IFormLinkNullableGetter<T>)))
                        {
                            if (!Mod.GetProperty<IFormLinkNullable<T>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var setValue) || setValue == null)
                                return -1;

                            setValue.SetToNull();
                            Global.DebugLogger?.Log(ClassLogCode, "Set to null.", propertyName: proKeys.Property.PropertyName);
                            return 1;
                        }
                        else
                        {
                            Global.Logger.Log(ClassLogCode, "Not nullable.", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                            return -1;
                        }
                    }

                    return 0;
                }

                if (formKey.ToLinkGetter() == null)
                {
                    Global.Logger.Log(ClassLogCode, $"Unable to find {formKey?.Value.ToString() ?? "FormKey"}", logLevel: LogLevel.Warning, propertyName: proKeys.Property.PropertyName);
                    return -1;
                }

                return Fill(proKeys, curValue, formKey.ToLinkGetter());
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);
            return -1;
        }

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                return Mod.GetProperty<IFormLinkGetter<T>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                    && Mod.GetProperty<IFormLinkGetter<T>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)
                    ? Fill(proKeys, curValue, newValue)
                    : -1;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);
            return -1;
        }

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.GetProperty<IFormLinkGetter<T>>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.GetProperty<IFormLinkGetter<T>>(origin, proKeys.Property.PropertyName, out var originValue)
                        && !(curValue == null ^ originValue == null)
                        && !(curValue != null ^ originValue != null)
                        && ((curValue == null && originValue == null)
                           || (curValue != null && originValue != null && curValue.FormKey.Equals(originValue.FormKey)));
        }

        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (proKeys.RuleKey.Operation != FilterLogic.OR)
                Global.Logger.Log(ClassLogCode, $"Invalid operation for checking a single value. Default OR only valid for this property. Continuing check as OR.", logLevel: LogLevel.Warning, propertyName: proKeys.Property.PropertyName);

            if (proKeys.Record is not IFormLinkContainerGetter)
                return false;

            var values = proKeys.GetMatchValueAs<List<FormKeyListOperationAdvanced<T>>>();

            if (!values.SafeAny())
                return true;

            if (!Mod.GetProperty<IFormLinkGetter<T>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue) || curValue == null)
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

        public int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        private static int Fill (ProcessingKeys proKeys, IFormLinkGetter<T>? curValue, IFormLinkGetter<T>? newValue)
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
                return 0;
            }

            if (!Mod.SetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Updated.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }
    }
}