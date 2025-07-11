using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;
using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    /// <summary>
    ///     Action that handles form links pointing to a single major records.
    /// </summary>
    public class FormLinkAction<TMajor> : IRecordAction where TMajor : class, IMajorRecordGetter
    {
        public static readonly FormLinkAction<TMajor> Instance = new();
        private const int ClassLogCode = 0x17;

        private FormLinkAction ()
        {
        }

        // <inheritdoc />
        public bool AllowSubProperties => false;

        // <inheritdoc />
        public bool CanFill () => true;

        // <inheritdoc />
        public bool CanForward () => true;

        // <inheritdoc />
        public bool CanForwardSelfOnly () => false;

        // <inheritdoc />
        public bool CanMatch () => true;

        // <inheritdoc />
        public bool CanMerge () => false;

        // <inheritdoc />
        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!proKeys.TryGetFillValueAs(out FormKeyListOperation<TMajor>? formKey) || !Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, out var propertyType, ClassLogCode))
                    return -1;

                if (formKey is null || formKey.Value == FormKey.Null)
                {
                    if (curValue is not null && !curValue.IsNull)
                    {
                        if (propertyType.IsAssignableTo(typeof(IFormLinkNullableGetter<TMajor>)))
                        {
                            if (!Mod.TryGetPropertyValueForEditing<IFormLinkNullable<TMajor>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var setValue, ClassLogCode) || setValue is null)
                                return -1;

                            setValue.SetToNull();
                            Global.Logger.WriteLog(LogLevel.Debug, LogType.RecordUpdated, $"{LogWriter.RecordUpdated} to null.", ClassLogCode);
                            return 1;
                        }
                        else
                        {
                            Global.Logger.WriteLog(LogLevel.Error, LogType.RecordActionInvalid, "Not nullable.", ClassLogCode);
                            return -1;
                        }
                    }

                    return 0;
                }

                if (formKey.ToLinkGetter() is null)
                {
                    Global.Logger.WriteLog(LogLevel.Error, LogType.RecordActionInvalid, $"Unable to find {formKey?.Value.ToString() ?? "FormKey"}", ClassLogCode);
                    return -1;
                }

                return performFill(proKeys, curValue, formKey.ToLinkGetter());
            }

            Global.Logger.LogInvalidTypeFound("IFormLinkContainerGetter", proKeys.Record.GetType().Name, ClassLogCode);
            return -1;
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordGetter>? FindHPUIndex (ProcessingKeys proKeys, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? endNodes) => Mod.FindHPUIndex<FormKey>(proKeys, AllRecordMods, endNodes, ClassLogCode);

        // <inheritdoc />
        public int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                return Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode)
                    && Mod.TryGetProperty<IFormLinkGetter<TMajor>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue, ClassLogCode)
                    ? performFill(proKeys, curValue, newValue)
                    : -1;
            }

            Global.Logger.LogInvalidTypeFound("IFormLinkContainerGetter", proKeys.Record.GetType().Name, ClassLogCode);
            return -1;
        }

        // <inheritdoc />
        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty<IFormLinkGetter<TMajor>>(recordContext.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode) || curValue is null || curValue.IsNull;

        // <inheritdoc />
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<IFormLinkGetter<TMajor>>(recordContext.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode)
                && Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue, ClassLogCode)
                && ((curValue is null && originValue is null)
                    || (curValue is not null && originValue is not null && curValue.FormKey.Equals(originValue.FormKey))));

        // <inheritdoc />
        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        // <inheritdoc />
        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (proKeys.RuleKey.Operation != FilterLogic.OR)
                Global.Logger.WriteLog(LogLevel.Warning, LogType.GeneralConfigFailure, "Invalid operation for checking a single value. Default OR only valid for this property. Continuing check as OR.", ClassLogCode);

            if (proKeys.Record is not IFormLinkContainerGetter)
                return false;

            if (!proKeys.TryGetMatchValueAs(out bool fromCache, out List<FormKeyListOperationAdvanced<TMajor>>? matches))
                return false;

            if (!matches.SafeAny())
                return true;

            if (!fromCache && !MatchesHelper.Validate(matches))
                throw new InvalidDataException("Json data for matches invalid");

            if (!Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode) || curValue is null)
                return !matches.Any(k => k.Operation != ListLogic.NOT);

            return MatchesHelper.Matches(curValue.FormKey, matches);
        }

        // <inheritdoc />
        public int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        // <inheritdoc />
        public bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Form Key or Editor ID";
            example = $"""
                        "{propertyName}": "123:Skyrim.esm" or "{propertyName}": "MyEditorID"
                        """;

            return true;
        }

        private static int performFill (ProcessingKeys proKeys, IFormLinkGetter<TMajor>? curValue, IFormLinkGetter<TMajor>? newValue)
        {
            if (curValue is not null && newValue is not null && curValue.FormKey.Equals(newValue.FormKey))
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.NoUpdateAlreadyMatches, LogWriter.PropertyIsEqual, ClassLogCode);
                return 0;
            }

            if (!Mod.TryGetPropertyValueForEditing<IFormLink<TMajor>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var formLink, ClassLogCode))
                return -1;

            if (newValue is null || newValue.IsNull)
                formLink.SetToNull();
            else
                formLink.SetTo(newValue.FormKey);

            Global.Logger.WriteLog(LogLevel.Debug, LogType.RecordUpdated, LogWriter.RecordUpdated, ClassLogCode);
            return 1;
        }
    }
}