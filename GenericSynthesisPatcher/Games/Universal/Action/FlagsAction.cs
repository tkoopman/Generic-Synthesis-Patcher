using System.Diagnostics.CodeAnalysis;

using Common;

using EnumsNET;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;
using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Graph;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    /// <summary>
    ///     Represents an action that can be performed on enum properties that contain the Flags
    ///     attribute of records.
    /// </summary>
    public class FlagsAction : IRecordAction
    {
        public static readonly FlagsAction Instance = new();
        private const int ClassLogCode = 0x12;

        private FlagsAction ()
        {
        }

        // <inheritdoc />
        public bool AllowSubProperties => false;

        // <inheritdoc />
        public bool CanFill () => true;

        // <inheritdoc />
        public bool CanForward () => true;

        // <inheritdoc />
        public bool CanForwardSelfOnly () => false;

        // <inheritdoc />
        public bool CanMatch () => true;

        // <inheritdoc />
        public bool CanMerge () => true;

        // <inheritdoc />
        public int Fill (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetFillValueAs(out List<ListOperation>? flags) || flags is null)
            {
                Global.TraceLogger?.Log(ClassLogCode, "No flags to set.", propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (!Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, out var flagType))
                return -1;

            curValue ??= (Enum)Enum.ToObject(flagType, 0);
            var newFlags = curValue;

            foreach (var flag in flags)
            {
                if (flag.Value is null)
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

        /// <inheritdoc />
        public IModContext<IMajorRecordGetter>? FindHPUIndex (ProcessingKeys proKeys, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? endNodes) => Mod.FindHPUIndex<Enum>(proKeys, AllRecordMods, endNodes);

        // <inheritdoc />
        public int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
            => Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
            && Mod.TryGetProperty<Enum>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)
            && curValue != newValue
            && Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue) ? 1 : -1;

        // <inheritdoc />
        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
                                    => !Mod.TryGetProperty<Enum>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || Mod.IsNullOrEmpty(curValue);

        // <inheritdoc />
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<Enum>(recordContext.Record, proKeys.Property.PropertyName, out var curValue)
            && Mod.TryGetProperty<Enum>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue)
            && curValue == originValue);

        // <inheritdoc />
        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        // <inheritdoc />
        [SuppressMessage("Usage", "CA2248:Provide correct 'enum' argument to 'Enum.HasFlag'", Justification = "They do match just errors due to generic nature.")]
        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (!Mod.TryGetProperty<Enum>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curFlags))
                return false;

            if (!proKeys.TryGetMatchValueAs(out _, out List<ListOperation>? matches))
                return false;

            if (!matches.SafeAny())
                return true;

            // If no values then if we are to match against any included values it will not match.
            if (curFlags is null)
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
                if (!Enum.TryParse(flagType, m.Value, true, out object? mFlag) || mFlag is null)
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

        // <inheritdoc />
        public int Merge (ProcessingKeys proKeys)
        {
            Global.UpdateLoggers(ClassLogCode);

            var root = FlagsRecordGraph.Create(proKeys);

            return root is not null && root.Merge(out var newValue) && Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue) ? 1 : 0;
        }

        // <inheritdoc />
        public virtual bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            propertyType = propertyType.GetIfGenericTypeDefinition() == typeof(Nullable<>) ? propertyType.GetIfUnderlyingType() ?? throw new Exception("WTF - This not meant to happen") : propertyType;
            if (!propertyType.IsEnum)
            {
                description = null;
                example = null;
                return false;
            }

            string[] flags = Enum.GetNames(propertyType);

            description = $"Flags ({string.Join(", ", flags)})";
            example = flags.Length > 1 ? $"\"{propertyName}\": [ \"{flags.First()}\", \"-{flags.Last()}\" ]" : $"\"{propertyName}\": \"{flags.First()}\"";

            return true;
        }
    }
}