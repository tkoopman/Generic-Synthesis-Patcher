using EnumsNET;

using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

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

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => false;

        public bool CanMatch () => true;

        public bool CanMerge () => true;

        public int Fill (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetFillValueAs(out List<ListOperation>? flags) || flags == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, "No flags to set.", propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (!Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, out var propertyInfo))
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

            if (!Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newFlags))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, $"Flags set to {newFlags}", propertyName: proKeys.Property.PropertyName);

            return 1;
        }

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
                    => Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                    && Mod.TryGetProperty<Enum>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)
                    && curValue != newValue
                    && Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue) ? 1 : -1;

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.TryGetProperty<Enum>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.TryGetProperty<Enum>(origin, proKeys.Property.PropertyName, out var originValue)
                        && curValue == originValue;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2248:Provide correct 'enum' argument to 'Enum.HasFlag'", Justification = "They do match just errors due to generic nature.")]
        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (!Mod.TryGetProperty<Enum>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curFlags))
                return false;

            if (!proKeys.TryGetMatchValueAs(out _, out List<ListOperation>? matches))
                return false;

            if (!matches.SafeAny())
                return true;

            // If no values then if we are to match against any included values it will not match.
            if (curFlags == null)
                return matches.Any(m => m.Operation != ListLogic.NOT);

            var flagType = curFlags.GetType();

            int matchedCount = 0;
            int countIncludes = 0;
            bool result = false;
            bool loopFinished = true;
            ListOperation? matchedOn = null;

            foreach (var m in matches)
            {
                loopFinished = false;
                if (!Enum.TryParse(flagType, m.Value, true, out object? mFlag) || mFlag == null)
                {
                    Global.Logger.Log(ClassLogCode, $"{m.Value} is not a valid flag for flag type {flagType.Name}. Ignoring this entry.", logLevel: LogLevel.Warning);
                    loopFinished = true;
                    continue;
                }

                if (m.Operation != ListLogic.NOT)
                    countIncludes++;

                if (curFlags.HasFlag((Enum)mFlag))
                {
                    if (proKeys.RuleKey.Operation == FilterLogic.OR)
                    {
                        Global.TraceLogger?.Log(ClassLogCode, $"Matched: {m.Operation != ListLogic.NOT}. Matched: {m.ToString('!')}");
                        result = m.Operation != ListLogic.NOT;
                        matchedOn = m;
                        break;
                    }

                    if (proKeys.RuleKey.Operation == FilterLogic.AND && m.Operation == ListLogic.NOT)
                    {
                        Global.TraceLogger?.Log(ClassLogCode, $"Matched: False. Matched {m.ToString('!')}");
                        matchedOn = m;
                        break;
                    }

                    matchedCount++;
                }
                else if (proKeys.RuleKey.Operation == FilterLogic.AND && m.Operation != ListLogic.NOT)
                {
                    Global.TraceLogger?.Log(ClassLogCode, $"Matched: False. Matched {m.Value}");
                    matchedOn = m;
                    break;
                }

                loopFinished = true;
            }

            if (loopFinished)
            {
                result = proKeys.RuleKey.Operation switch
                {
                    FilterLogic.AND => matchedCount == countIncludes, // Any failed matches returned above already so as long as all included matches matched we good.
                    FilterLogic.XOR => matchedCount == 1,
                    _ => countIncludes == 0 // OR - Any matches would of returned results above, so now only fail if any check was that it had to match a entry.
                };
            }

            Global.TraceLogger?.Log(ClassLogCode, $"Matched: {result} Operation: {proKeys.RuleKey.Operation} Trigger: {matchedOn}", propertyName: proKeys.Property.PropertyName);
            return result;
        }

        public int Merge (ProcessingKeys proKeys)
        {
            Global.UpdateLoggers(ClassLogCode);

            var root = FlagsRecordGraph.Create(proKeys);

            return root != null && root.Merge(out var newValue) && Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue) ? 1 : 0;
        }
    }
}