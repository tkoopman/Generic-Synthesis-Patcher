using GenericSynthesisPatcher.Json.Operations;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class ConvertibleAction<T> : BasicAction<T> where T : IConvertible
    {
        public new static readonly ConvertibleAction<T> Instance = new();

        private ConvertibleAction () : base()
        {
        }

        public override bool CanMatch () => true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public override bool MatchesRule (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetMatchValueAs(out bool fromCache, out List<ListOperation<T>>? matches))
                return false;

            if (!matches.SafeAny())
                return true;

            if (!fromCache && !MatchesHelper.Validate(matches))
                throw new InvalidDataException("Json data for matches invalid");

            if (!Mod.TryGetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                return false;

            return MatchesHelper.Matches(curValue, matches, propertyName: proKeys.Property.PropertyName);
        }
    }
}