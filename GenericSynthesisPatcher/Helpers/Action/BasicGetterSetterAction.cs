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
        {
            if (!Mod.TryGetProperty<TGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
             || !proKeys.TryGetFillValueAs(out TSetter? newValue))
                return -1;

            return Fill(proKeys, curValue, newValue);
        }

        public virtual int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
                    => (Mod.TryGetProperty<TGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                     && Mod.TryGetProperty<TGetter>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)) ?
                        Fill(proKeys, curValue, newValue)
                        : -1;

        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public virtual bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.TryGetProperty<TGetter>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.TryGetProperty<TGetter>(origin, proKeys.Property.PropertyName, out var originValue)
                        && Equals(curValue, originValue);
        }

        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        public virtual int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        /// <summary>
        /// Compare values of getter and setter.
        /// </summary>
        /// <param name="lhs">Getter</param>
        /// <param name="rhs">Setter</param>
        /// <returns>True if values both match</returns>
        protected abstract bool CompareValues (TGetter? lhs, TSetter? rhs);

        /// <summary>
        /// Update value on record if new and current values don't match.
        /// </summary>
        /// <param name="proKeys"></param>
        /// <param name="curValue">Current value in property</param>
        /// <param name="newValue">Value to set if different to current</param>
        /// <returns>
        ///     -1 if failed to get patch record
        ///     0 if both values matched already
        ///     1 if updated
        /// </returns>
        protected virtual int Fill (ProcessingKeys proKeys, TGetter? curValue, TSetter? newValue)
        {
            if (CompareValues(curValue, newValue))
                return 0;

            if (!Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Changed.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }

        /// <summary>
        /// Update value on record if new and current values don't match.
        /// </summary>
        /// <param name="proKeys"></param>
        /// <param name="curValue">Current value in property</param>
        /// <param name="newValue">Value to set if different to current</param>
        /// <returns>
        ///     -1 if failed to get patch record
        ///     0 if both values matched already
        ///     1 if updated
        /// </returns>
        protected virtual int Fill (ProcessingKeys proKeys, TGetter? curValue, TGetter? newValue) => Fill(proKeys, curValue, GetSetter(newValue));

        /// <summary>
        /// Gets setter type from values in getter type
        /// </summary>
        /// <param name="getter">Getter of values to convert to setter</param>
        /// <returns>TSetter object with values from getter</returns>
        protected abstract TSetter? GetSetter (TGetter? getter);
    }
}