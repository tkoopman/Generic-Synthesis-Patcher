using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Data;
using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

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

        public virtual bool CanFill () => false;

        public virtual bool CanForward () => true;

        public virtual bool CanForwardSelfOnly () => false;

        public virtual bool CanMatch () => false;

        public virtual bool CanMerge () => false;

        public virtual int Fill (ProcessingKeys proKeys) => throw new NotImplementedException();

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods)
        {
            bool nonNull = proKeys.Rule.HasForwardType(ForwardOptions._nonNullMod);
            List<object?> history = [];
            int hpu = -1;
            int hpuHistory = -1;

            foreach (int i in indexes.Reverse())
            {
                var mc = AllRecordMods[mods.ElementAt(i)];

                if (Mod.TryGetProperty<object>(mc.Record, proKeys.Property.PropertyName, out object? curValue)
                    && (!nonNull || !Mod.IsNullOrEmpty(curValue)))
                {
                    int historyIndex = history.IndexOf(curValue);
                    if (historyIndex == -1)
                    {
                        historyIndex = history.Count;
                        history.Add(curValue);
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

        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty(recordContext.Record, proKeys.Property.PropertyName, out object? curValue) || Mod.IsNullOrEmpty(curValue);

        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if value matches</returns>
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty(recordContext.Record, proKeys.Property.PropertyName, out object? curValue)
            && Mod.TryGetProperty(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out object? originValue)
            && Equals(curValue, originValue));

        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

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