using System.Text.RegularExpressions;

using Noggog;

namespace GenericSynthesisPatcher.Rules.Operations
{
    public abstract class OperationBase<TPrefix, TSuffix> : OperationBase<TPrefix>
        where TPrefix : struct, Enum
        where TSuffix : struct, Enum
    {
        protected static (TPrefix, string, TSuffix) split (string input, IReadOnlyDictionary<char, TPrefix> prefixes, IReadOnlyDictionary<char, TSuffix> suffixes)
        {
            (var p, string v) = split(input, prefixes);

            return suffixes.TryGetValue(v.Last(), out var suffix) ? (p, v[..^1], suffix) : (p, v, default);
        }

        protected static (TPrefix, string, TSuffix) split (string input, IReadOnlyDictionary<char, TPrefix> prefixes, IReadOnlyDictionary<char, TSuffix> suffixes, bool allowLongPrefixes)
        {
            (var p, string v) = split(input, prefixes, allowLongPrefixes);

            return suffixes.TryGetValue(v.Last(), out var suffix) ? (p, v[..^1], suffix) : (p, v, default);
        }
    }

    public abstract partial class OperationBase<TPrefix>
        where TPrefix : struct, Enum
    {
        public abstract OperationBase<TPrefix> Inverse ();

        [GeneratedRegex(@"^\(([A-Za-z0-9]+)\)(.+)$")]
        protected static partial Regex LongPrefix ();

        protected static (TPrefix, string) split (string input, IReadOnlyDictionary<char, TPrefix> prefixes, bool allowLongPrefixes)
        {
            if (allowLongPrefixes)
            {
                var m = LongPrefix().Match(input);
                if (m.Success && Enum.TryParse<TPrefix>(m.Groups[1].Value, true, out var prefix))
                    return (prefix, m.Groups[2].Value);
            }

            return split(input, prefixes);
        }

        protected static (TPrefix, string) split (string input, IReadOnlyDictionary<char, TPrefix> prefixes)
            => input.IsNullOrEmpty() ? (default, input)
             : prefixes.TryGetValue(input.First(), out var prefix) ? (prefix, input[1..]) : (default, input);
    }
}