using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Operations;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    public class ConvertibleAction<T> : BasicAction<T> where T : IConvertible
    {
        public new static readonly ConvertibleAction<T> Instance = new();

        private ConvertibleAction () : base()
        {
        }

        public override bool CanMatch () => true;

        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
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

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Numeric value"; // We assume if no overwritten by type checks it must be on of the many numeric types
            example = $"""
                       "{propertyName}": 7
                       """;

            if (typeof(T).Equals(typeof(string)))
            {
                description = "String value";
                example = $"""
                           "{propertyName}": "Hello"
                           """;
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                description = "True / False";
                example = $"""
                           "{propertyName}": true
                           """;
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                description = "Decimal value";
                example = $"""
                           "{propertyName}": 3.14
                           """;
            }

            return true;
        }
    }
}