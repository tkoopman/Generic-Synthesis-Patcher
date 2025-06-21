using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Data;
using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    public abstract class BasicGetterSetterAction<TGetter, TSetter> : IRecordAction
    {
        private const int ClassLogCode = 0x1B;

        protected BasicGetterSetterAction ()
        {
        }

        // <inheritdoc />
        public abstract bool AllowSubProperties { get; }

        // <inheritdoc />
        public virtual bool CanFill () => true;

        // <inheritdoc />
        public virtual bool CanForward () => true;

        // <inheritdoc />
        public virtual bool CanForwardSelfOnly () => false;

        // <inheritdoc />
        public virtual bool CanMatch () => false;

        // <inheritdoc />
        public virtual bool CanMerge () => false;

        // <inheritdoc />
        public virtual int Fill (ProcessingKeys proKeys)
            => !Mod.TryGetProperty<TGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
            || !proKeys.TryGetFillValueAs(out TSetter? newValue)
            ? -1
            : performFill(proKeys, curValue, newValue);

        // <inheritdoc />
        public virtual int Fill (ProcessingKeys proKeys, TGetter? curValue, TGetter? newValue) => performFill(proKeys, curValue, getSetter(newValue));

        // <inheritdoc />
        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods)
        {
            bool nonNull = proKeys.Rule.HasForwardType(ForwardOptions._nonNullMod);
            List<TGetter?> history = [];
            int hpu = -1;
            int hpuHistory = -1;

            foreach (int i in indexes.Reverse())
            {
                var mc = AllRecordMods[mods.ElementAt(i)];

                if (Mod.TryGetProperty<TGetter>(mc.Record, proKeys.Property.PropertyName, out var curValue)
                    && (!nonNull || curValue is not null))
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

        // <inheritdoc />
        public virtual int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
            => Mod.TryGetProperty<TGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
            && Mod.TryGetProperty<TGetter>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)
            ? Fill(proKeys, curValue, newValue)
            : -1;

        // <inheritdoc />
        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => !Mod.TryGetProperty<TGetter>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || curValue is null;

        // <inheritdoc />
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<TGetter>(recordContext.Record, proKeys.Property.PropertyName, out var curValue)
                && Mod.TryGetProperty<TGetter>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue)
                && Equals(curValue, originValue));

        // <inheritdoc />
        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        // <inheritdoc />
        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        // <inheritdoc />
        public abstract bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example);

        /// <summary>
        ///     Compare values of getter and setter.
        /// </summary>
        /// <param name="lhs">Getter</param>
        /// <param name="rhs">Setter</param>
        /// <returns>True if values both match</returns>
        protected abstract bool compareValues (TGetter? lhs, TSetter? rhs);

        /// <summary>
        ///     Create setter from getter.
        /// </summary>
        /// <returns>Setter version of getter</returns>
        protected abstract TSetter? getSetter (TGetter? getter);

        /// <summary>
        ///     Update value on record if new and current values don't match.
        /// </summary>
        /// <param name="proKeys"></param>
        /// <param name="curValue">Current value in property</param>
        /// <param name="newValue">Value to set if different to current</param>
        /// <returns>
        ///     <list type="table">
        ///         <item>-1 if failed to get patch record.</item>
        ///         <item>0 if both values matched already.</item>
        ///         <item>1 if updated</item>
        ///     </list>
        /// </returns>
        private int performFill (ProcessingKeys proKeys, TGetter? curValue, TSetter? newValue)
        {
            if (compareValues(curValue, newValue))
                return 0;

            if (!Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Changed.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }
    }
}