using System.Collections.ObjectModel;

using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Universal.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public class FilterOperation (string value) : FilterOperation<string>(value);

    [JsonConverter(typeof(OperationsConverter))]
    public class FilterOperation<T> : OperationBase<FilterLogic> where T : IConvertible
    {
        private static readonly ReadOnlyDictionary<char, FilterLogic> ValidPrefixes = new(new Dictionary<char, FilterLogic>() {
            { '&', FilterLogic.AND },
            { '^', FilterLogic.XOR },
            { '|', FilterLogic.OR } });

        // Required for OperationsConverter
        public FilterOperation (string value)
        {
            (Operation, string? v) = split(value, ValidPrefixes);

            Value = (T)((IConvertible)v).ToType(typeof(T), null);
        }

        public FilterLogic Operation { get; }
        public T Value { get; }

        public static bool operator != (FilterOperation<T> left, FilterOperation<T> right) => !(left == right);

        public static bool operator == (FilterOperation<T> left, FilterOperation<T> right) => left.Equals(right);

        public override bool Equals (object? obj)
                    => obj is FilterOperation<T> other
                    && Operation == other.Operation
                    && ((Value is string v && v.Equals(other.Value as string, StringComparison.OrdinalIgnoreCase))
                    || Value.Equals(other.Value));

        public override int GetHashCode ()
        {
            var hash = new HashCode ();
            hash.Add(Operation);
            if (Value is string v)
                hash.Add(v, StringComparer.OrdinalIgnoreCase);
            else
                hash.Add(Value);

            return hash.ToHashCode();
        }

        public override FilterOperation<T> Inverse () => throw new NotImplementedException();

        public override string? ToString ()
        {
            char? prefix = Operation switch
            {
                FilterLogic.AND => '&',
                FilterLogic.XOR => '^',
                _ => null,
            };

            return prefix is not null ? prefix + Value.ToString() : Value.ToString();
        }
    }
}