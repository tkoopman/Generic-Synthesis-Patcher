using System.Collections.ObjectModel;

using GenericSynthesisPatcher.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public abstract class ListOperationBase<TOperation, TValue> : OperationBase<TOperation, ListLogic>
        where TOperation : ListOperationBase<TOperation, TValue>
    {
        protected static readonly IReadOnlyDictionary<char, ListLogic> ValidPrefixes = new ReadOnlyDictionary<char, ListLogic>(new Dictionary<char, ListLogic>() {
            { '-', ListLogic.DEL },
            { '!', ListLogic.NOT },
            { '+', ListLogic.ADD } });

        public abstract ListLogic Operation { get; protected set; }
        public abstract TValue Value { get; protected set; }

        public override bool Equals ( object? obj )
            => obj is ListOperationBase<TOperation, TValue> other
            && Operation == other.Operation
            && (Value?.Equals(other.Value ?? default) ?? false);

        public override int GetHashCode () => HashCode.Combine(Operation, Value);

        public override string? ToString ()
        {
            char? prefix = Operation switch
            {
                ListLogic.DEL => '-',
                _ => null,
            };

            return prefix != null ? prefix + Value?.ToString() : Value?.ToString();
        }

        protected ListLogic InverseOperation () => Operation == ListLogic.NOT ? ListLogic.Default : ListLogic.NOT;
    }
}