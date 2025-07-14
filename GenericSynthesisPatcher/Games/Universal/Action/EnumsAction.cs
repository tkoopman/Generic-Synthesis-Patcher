using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Rules.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    /// <summary>
    ///     Represents an action that can be performed on enum properties of records.
    /// </summary>
    public class EnumsAction : IRecordAction
    {
        public static readonly EnumsAction Instance = new();
        private const int ClassLogCode = 0x15;

        private EnumsAction ()
        {
        }

        // <inheritdoc />
        public bool AllowSubProperties => false;

        // <inheritdoc />
        public bool CanFill () => true;

        // <inheritdoc />
        public bool CanForward () => false;

        // <inheritdoc />
        public bool CanForwardSelfOnly () => false;

        // <inheritdoc />
        public bool CanMatch () => true;

        // <inheritdoc />
        public bool CanMerge () => false;

        // <inheritdoc />
        public int Fill (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetFillValueAs(out string? setValueStr) || setValueStr is null)
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordActionInvalid, "Unable to read value to set", ClassLogCode);
                return -1;
            }

            if (!Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode) || curValue is null)
                return -1;

            var enumType = curValue.GetType();
            if (!Enum.TryParse(enumType, setValueStr, true, out object? setValue))
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordActionInvalid, $"{setValueStr} is not a valid value.", ClassLogCode);
                return -1;
            }

            if (curValue.Equals(setValue))
                return 0;

            if (!Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, setValue, ClassLogCode))
                return -1;

            Global.Logger.WriteLog(LogLevel.Debug, LogType.RecordUpdated, LogWriter.RecordUpdated, ClassLogCode);
            return 1;
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordGetter>? FindHPUIndex (ProcessingKeys proKeys, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? endNodes) => Mod.FindHPUIndex<Enum>(proKeys, AllRecordMods, endNodes, ClassLogCode);

        // <inheritdoc />
        public int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        // <inheritdoc />
        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => !Mod.TryGetProperty<Enum>(recordContext.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode) || Mod.IsNullOrEmpty(curValue);

        // <inheritdoc />
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<Enum>(recordContext.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode)
            && Mod.TryGetProperty<Enum>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue, ClassLogCode)
            && curValue == originValue);

        // <inheritdoc />
        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        // <inheritdoc />
        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (proKeys.RuleKey.Operation != FilterLogic.OR)
                Global.Logger.WriteLog(LogLevel.Warning, LogType.GeneralConfigFailure, "Invalid operation for checking a single value. Default OR only valid for this property. Continuing check as OR.", ClassLogCode);

            if (!Mod.TryGetProperty<Enum>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode))
                return false;

            int includesChecked = 0; // Only count !Neg

            if (!proKeys.TryGetMatchValueAs(out _, out List<ListOperation>? checkValues))
                return false;

            if (!checkValues.SafeAny())
                return true;

            if (curValue is null)
                return !checkValues.Any(k => k.Operation == ListLogic.Default);

            var valueType = curValue.GetType();

            bool result = false;
            bool loopFinished = true;
            object? matchedOn = null;

            foreach (var checkValueOp in checkValues)
            {
                loopFinished = false;
                if (!Enum.TryParse(valueType, checkValueOp.Value, true, out object? checkValue) || checkValue is null)
                {
                    Global.Logger.WriteLog(LogLevel.Error, LogType.RecordActionInvalid, $"{checkValueOp.Value} is not a valid value for enum type {valueType.Name}. Ignoring this entry.", ClassLogCode);
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

            Global.Logger.WriteLog(LogLevel.Trace, LogType.MatchSuccess, $"Matched: {result} Trigger: {matchedOn}", ClassLogCode);
            return result;
        }

        // <inheritdoc />
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