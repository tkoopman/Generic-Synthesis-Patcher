using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class BasicAction<T> : IRecordAction
    {
        public static readonly BasicAction<T> Instance = new();
        private const int ClassLogCode = 0x1B;

        protected BasicAction ()
        {
        }

        public virtual bool CanFill () => true;

        public virtual bool CanForward () => true;

        public virtual bool CanForwardSelfOnly () => false;

        public virtual bool CanMatch () => false;

        public virtual bool CanMerge () => false;

        public virtual int Fill (ProcessingKeys proKeys)
        {
            if (!Mod.GetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                return -1;

            var newValue = proKeys.GetFillValueAs<T>();

            return Fill(proKeys, curValue, newValue);
        }

        public virtual int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
                    => (Mod.GetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                     && Mod.GetProperty<T>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)) ?
                        Fill(proKeys, curValue, newValue)
                        : -1;

        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public virtual bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.GetProperty<T>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.GetProperty<T>(origin, proKeys.Property.PropertyName, out var originValue)
                        && Equals(curValue, originValue);
        }

        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        public virtual int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        protected virtual int Fill (ProcessingKeys proKeys, T? curValue, T? newValue)
        {
            if (Equals(curValue, newValue))
                return 0;

            if (!Mod.SetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Changed.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }
    }
}