using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Json.Operations;

namespace GenericSynthesisPatcher.Json.Data
{
    /// <summary>
    /// Key used for storing actions.
    /// </summary>
    public readonly struct ValueKey
    {
        public readonly string Key;
        public readonly FilterLogic Operation;

        /// <param name="key"></param>
        public ValueKey ( string key )
        {
            var operationValue = new FilterOperation(key);
            Key = operationValue.Value;
            Operation = operationValue.Operation;
        }

        public static bool operator != ( ValueKey left, ValueKey right ) => !(left == right);

        public static bool operator == ( ValueKey left, ValueKey right ) => left.Equals(right);

        public override bool Equals ( [NotNullWhen(true)] object? obj )
                                    => obj is ValueKey key
                                    && Operation == key.Operation
                                    && Key.Equals(key.Key, StringComparison.OrdinalIgnoreCase);

        public override readonly int GetHashCode () => HashCode.Combine(Operation, Key.GetHashCode(StringComparison.OrdinalIgnoreCase));
    }
}