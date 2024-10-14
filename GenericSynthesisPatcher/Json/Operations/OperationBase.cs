using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using GenericSynthesisPatcher.Helpers;

namespace GenericSynthesisPatcher.Json.Operations
{
    public abstract class OperationBase<P, S> : OperationBase<P> where P : struct, Enum where S : struct, Enum
    {
        protected static (P, string, S) Split ( string input, IReadOnlyDictionary<char, P> prefixes, IReadOnlyDictionary<char, S> suffixes, bool allowLongPrefixes )
        {
            var split = Split(input, prefixes, allowLongPrefixes);

            return suffixes.TryGetValue(split.Item2.Last(), out var suffix) ? (split.Item1, split.Item2[..^1], suffix) : (split.Item1, split.Item2, default);
        }

        protected static (P, string, S) Split ( string input, IReadOnlyDictionary<char, P> prefixes, IReadOnlyDictionary<char, S> suffixes )
        {
            var split = Split(input, prefixes);

            return suffixes.TryGetValue(split.Item2.Last(), out var suffix) ? (split.Item1, split.Item2[..^1], suffix) : (split.Item1, split.Item2, default);
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