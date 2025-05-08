using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Operations;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Universal.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public abstract class ListOperationBase<TValue> : OperationBase<ListLogic>, IEquatable<ListOperationBase<TValue>>
    {
        protected static readonly IReadOnlyDictionary<char, ListLogic> ValidPrefixes = new ReadOnlyDictionary<char, ListLogic>(new Dictionary<char, ListLogic>()
        {
            { '-', ListLogic.DEL },
            { '!', ListLogic.NOT },
            { '+', ListLogic.ADD }
        });

        private TValue? value;

        public ListOperationBase (string? value)
        {
            if (value == null)
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

        public override bool Equals (object? obj) => obj switch
        {
            ListOperationBase<TValue> other => Equals(other),
            TValue other => Equals(other),
            _ => false
        };

        public virtual bool Equals (ListOperationBase<TValue>? other) => other != null && Operation == other.Operation && ValueEquals(other.Value);

        public override int GetHashCode () => HashCode.Combine(Value);

        public int GetHashCode ([DisallowNull] TValue obj) => throw new NotImplementedException();

        public override ListOperationBase<TValue> Inverse () => Activator.CreateInstance(GetType(), [getInverse(Operation), Value]) is ListOperationBase<TValue> result ? result : throw new Exception("Failed to invert operation");

        public override string ToString () => ToString('-');

        public string ToString (char delPrefix)
        {
            if (Value == null)
                return "null";

            char? prefix = Operation switch
            {
                ListLogic.DEL => delPrefix,
                _ => null,
            };

            return prefix != null ? prefix + (Value.ToString() ?? "?") : Value.ToString() ?? "?";
        }

        public virtual bool ValueEquals (TValue? other)
            => MyEqualityComparer.Equals(Value, other);

        protected static ListLogic getInverse (ListLogic logic) => logic == ListLogic.NOT ? ListLogic.Default : ListLogic.NOT;

        protected abstract TValue? convertValue (string? value);

        protected virtual void valueUpdated ()
        { }
    }
}