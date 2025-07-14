using System.Collections.ObjectModel;

using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Helpers;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Rules.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public abstract class ListOperationBase<TValue> : OperationBase<ListLogic>
    {
        protected static readonly IReadOnlyDictionary<char, ListLogic> ValidPrefixes = new ReadOnlyDictionary<char, ListLogic>(new Dictionary<char, ListLogic>()
        {
            { '-', ListLogic.DEL },
            { '!', ListLogic.NOT },
            { '+', ListLogic.ADD }
        });

        private TValue? value;

        // Required for OperationsConverter
        public ListOperationBase (string? value)
        {
            if (value is null)
            {
                Operation = default;
                Value = default;
            }
            else
            {
                (Operation, string v) = split(value, ValidPrefixes);
                Value = convertValue(v);
            }
        }

        public ListOperationBase (ListLogic operation, TValue? value)
        {
            Operation = operation;
            Value = value;
        }

        public ListLogic Operation { get; private set; }

        public TValue? Value
        {
            get => value;
            protected set
            {
                this.@value = value;
                valueUpdated();
            }
        }

        public override bool Equals (object? obj)
            => obj switch
            {
                ListOperationBase<TValue> other => Equals(other),
                TValue other => ValueEquals(other),
                _ => false
            };

        public virtual bool Equals (ListOperationBase<TValue>? other) => other is not null && Operation == other.Operation && ValueEquals(other.Value);

        public override int GetHashCode () => HashCode.Combine(Value);

        public override ListOperationBase<TValue> Inverse () => Activator.CreateInstance(GetType(), [getInverse(Operation), Value]) is ListOperationBase<TValue> result ? result : throw new Exception("Failed to invert operation");

        public override string ToString () => ToString('-');

        public string ToString (char delPrefix)
        {
            if (Value is null)
                return "null";

            char? prefix = Operation switch
            {
                ListLogic.DEL => delPrefix,
                _ => null,
            };

            return prefix is not null ? prefix + (Value.ToString() ?? "?") : Value.ToString() ?? "?";
        }

        public virtual bool ValueEquals (TValue? other) => MyEqualityComparer.Equals(Value, other);

        protected static ListLogic getInverse (ListLogic logic) => logic == ListLogic.NOT ? ListLogic.Default : ListLogic.NOT;

        protected abstract TValue? convertValue (string? value);

        protected virtual void valueUpdated ()
        { }
    }
}