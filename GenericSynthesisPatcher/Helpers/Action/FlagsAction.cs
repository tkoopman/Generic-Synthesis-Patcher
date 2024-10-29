using EnumsNET;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    internal class FlagsAction : IRecordAction
    {
        public static readonly FlagsAction Instance = new();
        private const int ClassLogCode = 0x12;

        private FlagsAction ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => false;

        public bool CanForwardSelfOnly () => false;

        public bool CanMatch () => true;

        public bool CanMerge () => false;

        public int Fill (ProcessingKeys proKeys)
        {
            var flags = proKeys.GetFillValueAs<List<ListOperation>>();
            if (flags == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, "No flags to set.", propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (!Mod.GetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, out var propertyInfo))
                return -1;

            var flagType = propertyInfo.PropertyType;
            curValue ??= (Enum)Enum.ToObject(flagType, 0);
            var newFlags = curValue;

            foreach (var flag in flags)
            {
                if (flag.Value == null)
                {
                    newFlags = (Enum)Enum.ToObject(flagType, 0);
                }
                else if (Enum.TryParse(flagType, flag.Value, true, out object? setFlag))
                {
                    newFlags = flag.Operation == ListLogic.DEL
                        ? (Enum)FlagEnums.RemoveFlags(flagType, newFlags, setFlag)
                        : (Enum)FlagEnums.CombineFlags(flagType, newFlags, setFlag);
                }
            }

            if (curValue.Equals(newFlags))
                return 0;

            if (!Mod.SetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newFlags))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, $"Flags set to {newFlags}", propertyName: proKeys.Property.PropertyName);

            return 1;
        }

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.GetProperty<Enum>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.GetProperty<Enum>(origin, proKeys.Property.PropertyName, out var originValue)
                        && curValue == originValue;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2248:Provide correct 'enum' argument to 'Enum.HasFlag'", Justification = "They do match just errors due to generic nature.")]
        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (!Mod.GetProperty<Enum>(proKeys.Context.Record, proKeys.Property.PropertyName, out var checkFlags))
                return false;

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            var flags = proKeys.GetMatchValueAs<List<ListOperation>>();
            if (!flags.SafeAny())
                return true;

            if (checkFlags == null)
                return !flags.Any(k => k.Operation == ListLogic.Default);

            var flagType = checkFlags.GetType();

            foreach (var flag in flags)
            {
                if (!Enum.TryParse(flagType, flag.Value, true, out object? checkFlag) || checkFlag == null)
                    continue;

                if (flag.Operation != ListLogic.NOT)
                    includesChecked++;

                if (checkFlags.HasFlag((Enum)checkFlag))
                {
                    // Doesn't matter what overall Operation is we always fail on a NOT match
                    if (flag.Operation == ListLogic.NOT)
                        return false;

                    if (proKeys.RuleKey.Operation == FilterLogic.OR)
                        return true;

                    matchedCount++;
                }
                else if (flag.Operation != ListLogic.NOT && proKeys.RuleKey.Operation == FilterLogic.AND)
                {
                    return false;
                }
            }

            return proKeys.RuleKey.Operation switch
            {
                FilterLogic.AND => true,
                FilterLogic.XOR => matchedCount == 1,
                _ => includesChecked == 0 // OR
            };
        }

        public int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();
    }
}