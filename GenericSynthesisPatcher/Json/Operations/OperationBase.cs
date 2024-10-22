using System.Text.RegularExpressions;

namespace GenericSynthesisPatcher.Json.Operations
{
    public abstract class OperationBase<TOperation, TPrefix, TSuffix> : OperationBase<TOperation, TPrefix>
        where TOperation : OperationBase<TOperation, TPrefix, TSuffix>
        where TPrefix : struct, Enum
        where TSuffix : struct, Enum
    {
        protected static (TPrefix, string, TSuffix) Split ( string input, IReadOnlyDictionary<char, TPrefix> prefixes, IReadOnlyDictionary<char, TSuffix> suffixes, bool allowLongPrefixes )
        {
            (var p, string v) = Split(input, prefixes, allowLongPrefixes);

            return suffixes.TryGetValue(v.Last(), out var suffix) ? (p, v[..^1], suffix) : (p, v, default);
        }

        protected static (TPrefix, string, TSuffix) Split ( string input, IReadOnlyDictionary<char, TPrefix> prefixes, IReadOnlyDictionary<char, TSuffix> suffixes )
        {
            (var p, string v) = Split(input, prefixes);

            return suffixes.TryGetValue(v.Last(), out var suffix) ? (p, v[..^1], suffix) : (p, v, default);
        }
    }

    public abstract partial class OperationBase<TOperation, TPrefix>
        where TOperation : OperationBase<TOperation, TPrefix>
        where TPrefix : struct, Enum
    {
        public abstract TOperation Inverse ();

        [GeneratedRegex(@"^\(([A-Za-z0-9]+)\)(.+)$")]
        protected static partial Regex LongPrefix ();

        protected static (TPrefix, string) Split ( string input, IReadOnlyDictionary<char, TPrefix> prefixes, bool allowLongPrefixes )
        {
            if (allowLongPrefixes)
            {
                var m = LongPrefix().Match(input);
                if (m.Success && Enum.TryParse<TPrefix>(m.Groups[1].Value, true, out var prefix))
                    return (prefix, m.Groups[2].Value);
            }

            return Split(input, prefixes);
        }

        protected static (TPrefix, string) Split ( string input, IReadOnlyDictionary<char, TPrefix> prefixes ) => prefixes.TryGetValue(input.First(), out var prefix) ? (prefix, input[1..]) : (default, input);
    }
}