using System.Diagnostics.CodeAnalysis;

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
                if (!proKeys.TryGetFillValueAs(out FormKeyListOperation<TMajor>? formKey) || !Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, out var propertyType))
                    return -1;

                if (formKey == null || formKey.Value == FormKey.Null)
                {
                    if (curValue != null && !curValue.IsNull)
                    {
                        if (propertyType.IsAssignableTo(typeof(IFormLinkNullableGetter<TMajor>)))
                        {
                            if (!Mod.TryGetPropertyValueForEditing<IFormLinkNullable<TMajor>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var setValue) || setValue == null)
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

                return performFill(proKeys, curValue, formKey.ToLinkGetter());
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);
            return -1;
        }

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods)
        {
            bool nonNull = proKeys.Rule.HasForwardType(ForwardOptions._nonNullMod);
            List<FormKey?> history = [];
            int hpu = -1;
            int hpuHistory = -1;

            foreach (int i in indexes.Reverse())
            {
                var mc = AllRecordMods[mods.ElementAt(i)];

                if (Mod.TryGetProperty<IFormLinkGetter<TMajor>>(mc.Record, proKeys.Property.PropertyName, out var curValue)
                    && (!nonNull || (curValue is not null && !curValue.IsNull)))
                {
                    int historyIndex = history.IndexOf(curValue?.FormKey);
                    if (historyIndex == -1)
                    {
                        historyIndex = history.Count;
                        history.Add(curValue?.FormKey);
                        Global.TraceLogger?.Log(ClassLogCode, $"Added value from {mc.ModKey} to history", propertyName: proKeys.Property.PropertyName);
                    }

                    if (validMods is null || validMods.Contains(mc.ModKey))
                    {
                        // If this a valid mod to be selected then check when it's value was added
                        // to history and if higher or equal we found new HPU.
                        if (hpuHistory <= historyIndex)
                        {
                            hpu = i;
                            hpuHistory = historyIndex;
                            Global.TraceLogger?.Log(ClassLogCode, $"Updated HPU value to {mc.ModKey} with index of {i} and history index of {historyIndex}", propertyName: proKeys.Property.PropertyName);
                        }
                    }
                }
            }

            return hpu;
        }

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                return Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                    && Mod.TryGetProperty<IFormLinkGetter<TMajor>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)
                    ? performFill(proKeys, curValue, newValue)
                    : -1;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);
            return -1;
        }

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty<IFormLinkGetter<TMajor>>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || curValue is null || curValue.IsNull;

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if form link matches</returns>
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<IFormLinkGetter<TMajor>>(recordContext.Record, proKeys.Property.PropertyName, out var curValue)
                && Mod.TryGetProperty<IFormLinkGetter<TMajor>>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue)
                && !(curValue == null ^ originValue == null)
                && !(curValue != null ^ originValue != null)
                && ((curValue == null && originValue == null)
                    || (curValue != null && originValue != null && curValue.FormKey.Equals(originValue.FormKey))));

        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
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

        // <inheritdoc />
        public bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Form Key or Editor ID";
            example = "";

            return true;
        }

        private static int performFill (ProcessingKeys proKeys, IFormLinkGetter<TMajor>? curValue, IFormLinkGetter<TMajor>? newValue)
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
                return 0;
            }

            if (!Mod.TryGetPropertyValueForEditing<IFormLink<TMajor>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var formLink))
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