using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class Generic<T> : IRecordAction where T : IConvertible
    {
        public static readonly Generic<T> Instance = new();
        private const int ClassLogCode = 0x10;

        private Generic ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => false;

        public bool CanMerge () => false;

        public int Fill (ProcessingKeys proKeys)
        {
            if (!Mod.GetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                return -1;

            var newValue = proKeys.GetFillValueAs<T>();

            return Generic<T>.Fill(proKeys, curValue, newValue);
        }

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
                    => (Mod.GetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                     && Mod.GetProperty<T>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)) ?
                        Generic<T>.Fill(proKeys, curValue, newValue)
                        : -1;

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.GetProperty<T>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.GetProperty<T>(origin, proKeys.Property.PropertyName, out var originValue)
                        && !(curValue == null ^ originValue == null)
                        && !(curValue != null ^ originValue != null)
                        && ((curValue == null && originValue == null)
                           || (curValue != null && originValue != null && curValue.Equals(originValue)));
        }

        public bool MatchesRule (ProcessingKeys proKeys)
        {
            var values = proKeys.GetMatchValueAs<List<ListOperation<T>>>();

            if (!values.SafeAny())
                return true;

            if (!Mod.GetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue) || curValue == null)
                return !values.Any(k => k.Operation != ListLogic.NOT);

            foreach (var v in values)
            {
                if (v is string str && str.StartsWith('/') && str.EndsWith('/') && curValue is string curStr)
                {
                    if (new Regex(str.Trim('/'), RegexOptions.IgnoreCase).IsMatch(curStr))
                        return v.Operation != ListLogic.NOT;
                }
                else if (curValue.Equals(v.Value))
                {
                    if (v.Operation == ListLogic.NOT)
                        return v.Operation != ListLogic.NOT;
                }
            }

            return !values.Any(k => k.Operation != ListLogic.NOT);
        }

        public int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        private static int Fill (ProcessingKeys proKeys, T? curValue, T? newValue)
        {
            if (curValue == null && newValue == null)
                return 0;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return 0;

            if (!Mod.SetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Changed.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }
    }
}