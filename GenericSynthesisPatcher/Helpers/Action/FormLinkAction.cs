using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLinkAction<TMajor> : IRecordAction where TMajor : class, IMajorRecordGetter
    {
        public static readonly FormLinkAction<TMajor> Instance = new();
        private const int ClassLogCode = 0x13;

        private FormLinkAction ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => false;

        public bool CanMatch () => true;

        public bool CanMerge () => false;

        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, out var propertyInfo) || !proKeys.TryGetFillValueAs(out FormKeyListOperation<TMajor>? formKey))
                    return -1;

                if (formKey == null || formKey.Value == FormKey.Null)
                {
                    if (curValue != null && !curValue.IsNull)
                    {
                        if (propertyInfo.PropertyType.IsAssignableTo(typeof(IFormLinkNullableGetter<TMajor>)))
                        {
                            if (!Mod.TryGetProperty<IFormLinkNullable<TMajor>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var setValue) || setValue == null)
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
                return Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                    && Mod.TryGetProperty<IFormLinkGetter<TMajor>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)
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
                        && Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.TryGetProperty<IFormLinkGetter<TMajor>>(origin, proKeys.Property.PropertyName, out var originValue)
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

            if (!proKeys.TryGetMatchValueAs(out bool fromCache, out List<FormKeyListOperationAdvanced<TMajor>>? matches))
                return false;

            if (!matches.SafeAny())
                return true;

            if (!fromCache && !MatchesHelper.Validate(matches))
                throw new InvalidDataException("Json data for matches invalid");

            if (!Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue) || curValue == null)
                return !matches.Any(k => k.Operation != ListLogic.NOT);

            return MatchesHelper.Matches(curValue.FormKey, matches, propertyName: proKeys.Property.PropertyName);
        }

        public int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        private static int Fill (ProcessingKeys proKeys, IFormLinkGetter<TMajor>? curValue, IFormLinkGetter<TMajor>? newValue)
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
                return 0;
            }

            if (!Mod.TryGetPropertyForEditing<IFormLink<TMajor>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var formLink))
                return -1;

            if (newValue == null || newValue.IsNull)
                formLink.SetToNull();
            else
                formLink.SetTo(newValue.FormKey);

            Global.DebugLogger?.Log(ClassLogCode, "Updated.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }
    }
}