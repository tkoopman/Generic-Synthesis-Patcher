//using System.Diagnostics.CodeAnalysis;

//using GenericSynthesisPatcher.Json.Operations;

//namespace GenericSynthesisPatcher.Json.Data
//{
//    /// <summary>
//    /// Key used for storing actions.
//    /// </summary>

//    public readonly struct FilterKey
//    {
//        public readonly string Key;
//        public readonly FilterLogic Operation;

//        /// <param name="key"></param>
//        public FilterKey ( string key )
//        {
//            var operationValue = new FilterOperation(key);
//            Key = operationValue.Value;
//            Operation = operationValue.Operation;
//        }

//        public static bool operator != ( FilterKey left, FilterKey right ) => !(left == right);

//        public static bool operator == ( FilterKey left, FilterKey right ) => left.Equals(right);

//        public override bool Equals ( [NotNullWhen(true)] object? obj )
//                                    => obj is FilterKey key
//                                    && Operation == key.Operation
//                                    && Key.Equals(key.Key, StringComparison.OrdinalIgnoreCase);

//        public override readonly int GetHashCode () => HashCode.Combine(Operation, Key.GetHashCode(StringComparison.OrdinalIgnoreCase));
//    }
//}