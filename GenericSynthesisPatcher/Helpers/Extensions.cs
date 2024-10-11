using System.Diagnostics.CodeAnalysis;

namespace GenericSynthesisPatcher.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Same as IEnumerable<>.Any() but will return false instead of throwing ArgumentNullException if null.
        /// </summary>
        /// <returns>False if null else result of .Any()</returns>
        public static bool SafeAny<TSource> ( [NotNullWhen(true)] this IEnumerable<TSource>? source ) => source != null && source.Any();
    }
}