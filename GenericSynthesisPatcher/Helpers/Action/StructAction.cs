using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class StructAction<T> : IRecordAction where T : struct
    {
        public static readonly StructAction<T> Instance = new();
        private const int ClassLogCode = 0x10;

        private StructAction ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => false;

        public bool CanMatch () => false;

        public bool CanMerge () => false;

        public int Fill (ProcessingKeys proKeys)
        {
            if (!Mod.GetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                return -1;

            var newValue = proKeys.GetFillValueAs<T>();

            return StructAction<T>.Fill(proKeys, curValue, newValue);
        }

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
                    => (Mod.GetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                     && Mod.GetProperty<T>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)) ?
                        StructAction<T>.Fill(proKeys, curValue, newValue)
                        : -1;

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.GetProperty<T>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.GetProperty<T>(origin, proKeys.Property.PropertyName, out var originValue)
                        && curValue.Equals(originValue);
        }

        public bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        public int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        private static int Fill (ProcessingKeys proKeys, T curValue, T newValue)
        {
            if (curValue.Equals(newValue))
                return 0;

            if (!Mod.SetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Changed.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }
    }
}