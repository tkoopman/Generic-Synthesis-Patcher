using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Helpers
{
    public static class Extensions
    {
        public static void AddDictionary<TKey> ( this HashCode hashCode, IDictionary<TKey, JToken> dic )
        {
            foreach (var pair in dic)
            {
                hashCode.Add(pair.Key);
                hashCode.Add(pair.Value.ToString());
            }
        }

        /// <summary>
        /// Same as IEnumerable<>.Any() but will return false instead of throwing ArgumentNullException if null.
        /// </summary>
        /// <returns>False if null else result of .Any()</returns>
        public static bool SafeAny<TSource> ( [NotNullWhen(true)] this IEnumerable<TSource>? source ) => source != null && source.Any();
    }
}