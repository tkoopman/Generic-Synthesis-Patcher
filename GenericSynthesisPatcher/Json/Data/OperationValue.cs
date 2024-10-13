using GenericSynthesisPatcher.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    [JsonConverter(typeof(OperationsConverter))]
    public class OperationValue ( string value ) : OperationValue<string>(value);

    [JsonConverter(typeof(OperationsConverter))]
    public class OperationValue<T> where T : IConvertible
    {
        [JsonIgnore]
        public readonly Operation Operation;

        [JsonIgnore]
        public readonly T Value;

        public OperationValue ( string value )
        {
            ArgumentNullException.ThrowIfNull(value);

            switch (value.First())
            {
                case '-':
                    Value = (T)((IConvertible)value[1..]).ToType(typeof(T), null);
                    Operation = Operation.Remove;
                    break;

                case '!':
                    Value = (T)((IConvertible)value[1..]).ToType(typeof(T), null);
                    Operation = Operation.NOT;
                    break;

                case '&':
                    Value = (T)((IConvertible)value[1..]).ToType(typeof(T), null);
                    Operation = Operation.AND;
                    break;

                case '^':
                    Value = (T)((IConvertible)value[1..]).ToType(typeof(T), null);
                    Operation = Operation.XOR;
                    break;

                case '+':
                case '|':
                    Value = (T)((IConvertible)value[1..]).ToType(typeof(T), null);
                    Operation = Operation.Default;
                    break;

                default:
                    Value = (T)((IConvertible)value).ToType(typeof(T), null);
                    Operation = Operation.Default;
                    break;
            }
        }

        public override bool Equals ( object? obj )
            => obj is OperationValue<T> other
            && Operation == other.Operation
            && Value.Equals(other.Value);

        public override int GetHashCode () => HashCode.Combine(Operation, Value);

        public override string? ToString ()
        {
            char? prefix = Operation switch
            {
                Operation.NOT => '!',
                Operation.AND => '&',
                Operation.XOR => '^',
                _ => null,
            };

            return (prefix != null) ? prefix + Value.ToString() : Value.ToString();
        }
    }
}