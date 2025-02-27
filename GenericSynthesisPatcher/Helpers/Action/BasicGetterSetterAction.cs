using GenericSynthesisPatcher.Json.Data;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public abstract class BasicGetterSetterAction<TGetter, TSetter> : IRecordAction
    {
        private const int ClassLogCode = 0x1B;

        protected BasicGetterSetterAction ()
        {
        }

        public virtual bool CanFill () => true;

        public virtual bool CanForward () => true;

        public virtual bool CanForwardSelfOnly () => false;

        public virtual bool CanMatch () => false;

        public virtual bool CanMerge () => false;

        public virtual int Fill (ProcessingKeys proKeys)
            => !Mod.TryGetProperty<TGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
            || !proKeys.TryGetFillValueAs(out TSetter? newValue)
            ? -1
            : performFill(proKeys, curValue, newValue);

        /// <summary>
        ///     Update value on record if new and current values don't match.
        /// </summary>
        /// <param name="proKeys"></param>
        /// <param name="curValue">Current value in property</param>
        /// <param name="newValue">Value to set if different to current</param>
        /// <returns>
        ///     -1 if failed to get patch record 0 if both values matched already 1 if updated
        /// </returns>
        public virtual int Fill (ProcessingKeys proKeys, TGetter? curValue, TGetter? newValue) => performFill(proKeys, curValue, getSetter(newValue));

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods)
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

        public virtual int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
            => (Mod.TryGetProperty<TGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
            && Mod.TryGetProperty<TGetter>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue))
            ? Fill(proKeys, curValue, newValue)
            : -1;

        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext)
            => !Mod.TryGetProperty<TGetter>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || curValue is null;

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if Enum value matches</returns>
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<TGetter>(recordContext.Record, proKeys.Property.PropertyName, out var curValue)
                && Mod.TryGetProperty<TGetter>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue)
                && Equals(curValue, originValue));

        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        public virtual int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        /// <summary>
        ///     Compare values of getter and setter.
        /// </summary>
        /// <param name="lhs">Getter</param>
        /// <param name="rhs">Setter</param>
        /// <returns>True if values both match</returns>
        protected abstract bool compareValues (TGetter? lhs, TSetter? rhs);

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