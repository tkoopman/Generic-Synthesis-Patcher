using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    public class EnumsAction : IRecordAction
    {
        public static readonly EnumsAction Instance = new();
        private const int ClassLogCode = 0x11;

        private EnumsAction ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => false;

        public bool CanForwardSelfOnly () => false;

        public bool CanMatch () => true;

        public bool CanMerge () => false;

        public int Fill (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetFillValueAs(out string? setValueStr) || setValueStr == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, $"No {proKeys.Property.PropertyName} to set.", propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (!Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue) || curValue == null)
                return -1;

            var enumType = curValue.GetType();
            if (!Enum.TryParse(enumType, setValueStr, true, out object? setValue))
            {
                Global.Logger.Log(ClassLogCode, $"{setValueStr} is not a valid value for {proKeys.Property.PropertyName}.", logLevel: LogLevel.Warning, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (curValue.Equals(setValue))
                return 0;

            if (!Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, setValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Updated.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods)
        {
            bool nonNull = proKeys.Rule.HasForwardType(ForwardOptions._nonNullMod);
            List<Enum?> history = [];
            int hpu = -1;
            int hpuHistory = -1;

            foreach (int i in indexes.Reverse())
            {
                var mc = AllRecordMods[mods.ElementAt(i)];

                if (Mod.TryGetProperty<Enum>(mc.Record, proKeys.Property.PropertyName, out var curValue)
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

        public int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => !Mod.TryGetProperty<Enum>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || Mod.IsNullOrEmpty(curValue);

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if Enum value matches</returns>
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || Mod.TryGetProperty<Enum>(recordContext.Record, proKeys.Property.PropertyName, out var curValue)
            && Mod.TryGetProperty<Enum>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue)
            && curValue == originValue;

        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (proKeys.RuleKey.Operation != FilterLogic.OR)
                Global.Logger.Log(ClassLogCode, $"Invalid operation for checking a single value. Default OR only valid for this property. Continuing check as OR.", logLevel: LogLevel.Warning, propertyName: proKeys.Property.PropertyName);

            if (!Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                return false;

            int includesChecked = 0; // Only count !Neg

            if (!proKeys.TryGetMatchValueAs(out _, out List<ListOperation>? checkValues))
                return false;

            if (!checkValues.SafeAny())
                return true;

            if (curValue == null)
                return !checkValues.Any(k => k.Operation == ListLogic.Default);

            var valueType = curValue.GetType();

            bool result = false;
            bool loopFinished = true;
            object? matchedOn = null;

            foreach (var checkValueOp in checkValues)
            {
                loopFinished = false;
                if (!Enum.TryParse(valueType, checkValueOp.Value, true, out object? checkValue) || checkValue == null)
                {
                    Global.Logger.Log(ClassLogCode, $"{checkValueOp.Value} is not a valid value for enum type {valueType.Name}. Ignoring this entry.", logLevel: LogLevel.Warning);
                    loopFinished = true;
                    continue;
                }

                if (checkValueOp.Operation != ListLogic.NOT)
                    includesChecked++;

                if (curValue.Equals(checkValue))
                {
                    matchedOn = checkValue;
                    result = checkValueOp.Operation != ListLogic.NOT;
                    break;
                }

                loopFinished = true;
            }

            if (loopFinished)
                result = includesChecked == 0;

            Global.TraceLogger?.Log(ClassLogCode, $"Matched: {result} Trigger: {matchedOn}", propertyName: proKeys.Property.PropertyName);
            return result;
        }

        public int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

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

            string[] values = Enum.GetNames(propertyType);

            description = $"Possible values ({string.Join(", ", values)})";
            example = $"""
                       "{propertyName}": "{values.First()}"
                       """;

            return true;
        }
    }
}