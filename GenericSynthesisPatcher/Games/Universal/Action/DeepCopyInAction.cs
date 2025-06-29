using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    /// <summary>
    ///     This is the default action for editable properties that are not assigned any other action.
    ///
    ///     As is used DeepCopyIn method from Mutagen, only supports Forward action.
    /// </summary>
    public class DeepCopyInAction : IRecordAction
    {
        public static readonly DeepCopyInAction Instance = new();
        private const int ClassLogCode = 0x1B;

        protected DeepCopyInAction ()
        {
        }

        // <inheritdoc />
        public bool AllowSubProperties => true;

        /// <inheritdoc />
        public virtual bool CanFill () => false;

        /// <inheritdoc />
        public virtual bool CanForward () => true;

        /// <inheritdoc />
        public virtual bool CanForwardSelfOnly () => false;

        /// <inheritdoc />
        public virtual bool CanMatch () => false;

        /// <inheritdoc />
        public virtual bool CanMerge () => false;

        /// <inheritdoc />
        public virtual int Fill (ProcessingKeys proKeys) => throw new NotImplementedException();

        /// <inheritdoc />
        public IModContext<IMajorRecordGetter>? FindHPUIndex (ProcessingKeys proKeys, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? endNodes) => Mod.FindHPUIndex<object>(proKeys, AllRecordMods, endNodes);

        /// <inheritdoc />
        public virtual int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
        {
            if (!TranslationMaskFactory.TryCreate(proKeys.Property.RecordType, false, [proKeys.Property.PropertyName], out var mask)
                || mask is not MajorRecord.TranslationMask majorMask)
            {
                Global.Logger.Log(ClassLogCode, $"No changes to {proKeys.Property.PropertyName} in {forwardContext.ModKey} as couldn't find suitable translation mask. {mask?.GetType().GetClassName()}", propertyName: proKeys.Property.PropertyName, logLevel: LogLevel.Warning);
                return 0;
            }

            if (proKeys.GetPatchRecord() is not IMajorRecordInternal patchRecord)
            {
                Global.Logger.Log(ClassLogCode, $"No changes to {proKeys.Property.PropertyName} in {forwardContext.ModKey} as invalid record type for DeepCopyIn", propertyName: proKeys.Property.PropertyName, logLevel: LogLevel.Warning);
                return 0;
            }

            if (patchRecord.Equals(forwardContext.Record, majorMask))
            {
                Global.TraceLogger?.Log(ClassLogCode, $"No changes to {proKeys.Property.PropertyName} in {forwardContext.ModKey} as already equal", propertyName: proKeys.Property.PropertyName);
                return 0;
            }

            Global.TraceLogger?.Log(ClassLogCode, "Calling DeepCopyIn to update property.", propertyName: proKeys.Property.PropertyName);
            patchRecord.DeepCopyIn(forwardContext.Record, majorMask);

            return 1;
        }

        /// <inheritdoc />
        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty(recordContext.Record, proKeys.Property.PropertyName, out object? curValue) || Mod.IsNullOrEmpty(curValue);

        /// <inheritdoc />
        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        /// <inheritdoc />
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty(recordContext.Record, proKeys.Property.PropertyName, out object? curValue)
            && Mod.TryGetProperty(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out object? originValue)
            && Equals(curValue, originValue));

        /// <inheritdoc />
        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = string.Empty;
            example = string.Empty;

            return description is not null && example is not null;
        }
    }
}