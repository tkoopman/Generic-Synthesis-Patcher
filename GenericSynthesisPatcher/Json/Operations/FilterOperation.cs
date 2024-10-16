using System.Collections.ObjectModel;

using GenericSynthesisPatcher.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public class FilterOperation ( string value ) : FilterOperation<string>(value);

    [JsonConverter(typeof(OperationsConverter))]
    public class FilterOperation<T> : OperationBase<FilterLogic> where T : IConvertible
    {
        public readonly FilterLogic Operation;
        public readonly T Value;

        private static readonly ReadOnlyDictionary<char, FilterLogic> ValidPrefixes = new(new Dictionary<char, FilterLogic>() {
            { '&', FilterLogic.AND },
            { '^', FilterLogic.XOR },
            { '|', FilterLogic.OR } });

        public FilterOperation ( string value )
        {
            var split = Split(value, ValidPrefixes);

            Operation = split.Item1;
            Value = (T)((IConvertible)split.Item2).ToType(typeof(T), null);
        }

        public override bool Equals ( object? obj )
            => obj is FilterOperation<T> other
            && Operation == other.Operation
            && Value.Equals(other.Value);

        public override int GetHashCode () => HashCode.Combine(Operation, Value);

        public override string? ToString ()
        {
            char? prefix = Operation switch
            {
                FilterLogic.AND => '&',
                FilterLogic.XOR => '^',
                _ => null,
            };

            return prefix != null ? prefix + Value.ToString() : Value.ToString();
        }
    }
}