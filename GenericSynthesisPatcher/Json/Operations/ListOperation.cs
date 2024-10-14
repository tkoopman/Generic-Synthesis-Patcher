using System.Collections.ObjectModel;

using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Data;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public class ListOperation ( string value ) : ListOperation<string>(value);

    [JsonConverter(typeof(OperationsConverter))]
    public class ListOperation<T> : OperationBase<ListLogic> where T : IConvertible
    {
        public readonly ListLogic Operation;
        public readonly T Value;

        private static readonly ReadOnlyDictionary<char, ListLogic> ValidPrefixes = new(new Dictionary<char, ListLogic>() {
            { '-', ListLogic.DEL },
            { '!', ListLogic.NOT },
            { '+', ListLogic.ADD } });

        public ListOperation ( string value )
        {
            var split = Split(value, ValidPrefixes);

            Operation = split.Item1;
            Value = (T)((IConvertible)split.Item2).ToType(typeof(T), null);
        }

        public override bool Equals ( object? obj )
            => obj is ListOperation<T> other
            && Operation == other.Operation
            && Value.Equals(other.Value);

        public override int GetHashCode () => HashCode.Combine(Operation, Value);

        public override string? ToString ()
        {
            char? prefix = Operation switch
            {
                ListLogic.DEL => '-',
                _ => null,
            };

            return prefix != null ? prefix + Value.ToString() : Value.ToString();
        }
    }
}