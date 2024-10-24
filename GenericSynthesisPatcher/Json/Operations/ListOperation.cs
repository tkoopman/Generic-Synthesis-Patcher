using GenericSynthesisPatcher.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public class ListOperation ( string value ) : ListOperation<string>(value);

    [JsonConverter(typeof(OperationsConverter))]
    public class ListOperation<T> : ListOperationBase<T> where T : IConvertible
    {
        public override ListLogic Operation { get; protected set; }
        public override T Value { get; protected set; }

        public ListOperation ( string value )
        {
            (Operation, string? v) = Split(value, ValidPrefixes);
            Value = (T)((IConvertible)v).ToType(typeof(T), null);
        }
    }
}