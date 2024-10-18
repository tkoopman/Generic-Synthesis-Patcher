using System.Text.RegularExpressions;

namespace GenericSynthesisPatcher.Json.Operations
{
    public abstract class OperationBase<P, S> : OperationBase<P> where P : struct, Enum where S : struct, Enum
    {
        protected static (P, string, S) Split ( string input, IReadOnlyDictionary<char, P> prefixes, IReadOnlyDictionary<char, S> suffixes, bool allowLongPrefixes )
        {
            (var p, string v) = Split(input, prefixes, allowLongPrefixes);

            return suffixes.TryGetValue(v.Last(), out var suffix) ? (p, v[..^1], suffix) : (p, v, default);
        }

        protected static (P, string, S) Split ( string input, IReadOnlyDictionary<char, P> prefixes, IReadOnlyDictionary<char, S> suffixes )
        {
            (var p, string v) = Split(input, prefixes);

            return suffixes.TryGetValue(v.Last(), out var suffix) ? (p, v[..^1], suffix) : (p, v, default);
        }
    }

    public abstract partial class OperationBase<P> where P : struct, Enum
    {
        [GeneratedRegex(@"^\(([A-Za-z0-9]+)\)(.+)$")]
        protected static partial Regex LongPrefix ();

        protected static (P, string) Split ( string input, IReadOnlyDictionary<char, P> prefixes, bool allowLongPrefixes )
        {
            if (allowLongPrefixes)
            {
                var m = LongPrefix().Match(input);
                if (m.Success && Enum.TryParse<P>(m.Groups[1].Value, true, out var prefix))
                    return (prefix, m.Groups[2].Value);
            }

            return Split(input, prefixes);
        }

        protected static (P, string) Split ( string input, IReadOnlyDictionary<char, P> prefixes ) => prefixes.TryGetValue(input.First(), out var prefix) ? (prefix, input[1..]) : (default, input);
    }
}