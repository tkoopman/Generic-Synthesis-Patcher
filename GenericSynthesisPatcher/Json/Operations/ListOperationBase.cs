using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public abstract class ListOperationBase<TClass, TValue> : OperationBase<TClass, ListLogic>
        where TClass : ListOperationBase<TClass, TValue>
    {
        protected static readonly IReadOnlyDictionary<char, ListLogic> ValidPrefixes = new ReadOnlyDictionary<char, ListLogic>(new Dictionary<char, ListLogic>() {
            { '-', ListLogic.DEL },
            { '!', ListLogic.NOT },
            { '+', ListLogic.ADD } });

        private TValue? value;

        public ListLogic Operation { get; private set; }

        public TValue? Value
        {
            get => value;
            protected set
            {
                this.@value = value;
                ValueUpdated();
            }
        }

        public ListOperationBase ( string? value )
        {
            if (value == null)
            {
                Operation = default;
                Value = default;
            }
            else
            {
                (Operation, string v) = Split(value, ValidPrefixes);
                Value = ConvertValue(v);
            }
        }

        public ListOperationBase ( ListLogic operation, TValue? value )
        {
            Operation = operation;
            Value = value;
        }

        public override bool Equals ( object? obj ) => obj is ListOperationBase<TClass, TValue> other && Equals(other);

        public bool Equals ( ListOperationBase<TClass, TValue> other ) => Operation == other.Operation && ValueEquals(other.Value);

        public override int GetHashCode () => HashCode.Combine(Operation, Value);

        public override TClass Inverse () => System.Activator.CreateInstance(GetType(), [Inverse(Operation), Value]) is TClass result ? result : throw new Exception("Failed to invert operation");

        public override string? ToString ()
        {
            if (Value == null)
                return "null";

            char? prefix = Operation switch
            {
                ListLogic.DEL => '-',
                _ => null,
            };

            return prefix != null ? prefix + Value.ToString() : Value.ToString();
        }

        public bool ValueEquals ( TValue? other )
            => (Value == null && other == null)
            || (Value != null && other != null && Value.Equals(other));

        protected static ListLogic Inverse ( ListLogic logic ) => logic == ListLogic.NOT ? ListLogic.Default : ListLogic.NOT;

        protected abstract TValue? ConvertValue ( string? value );

        protected virtual void ValueUpdated ()
        { }
    }
}