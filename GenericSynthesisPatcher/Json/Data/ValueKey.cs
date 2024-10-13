using System.Diagnostics.CodeAnalysis;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Json.Data
{
    /// <summary>
    /// Key used for storing actions.
    /// </summary>
    public readonly struct ValueKey
    {
        public readonly string Key;
        public readonly Operation Operation;

        /// <param name="key"></param>
        public ValueKey ( string key )
        {
            var operationValue = new OperationValue(key);
            Key = operationValue.Value.ToLower();
            Operation = operationValue.Operation;
        }

        public static bool operator != ( ValueKey left, ValueKey right ) => !(left == right);

        public static bool operator == ( ValueKey left, ValueKey right ) => left.Equals(right);

        public override bool Equals ( [NotNullWhen(true)] object? obj )
                                    => obj is ValueKey key
                                    && Operation == key.Operation
                                    && Key.Equals(key.Key);

        public override readonly int GetHashCode () => HashCode.Combine(Operation, Key);
    }
}