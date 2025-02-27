using System.Collections;
using System.Diagnostics.CodeAnalysis;

using DynamicData;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Helpers
{
    public static class Extensions
    {
        public static void AddDictionary<TKey> (this HashCode hashCode, IDictionary<TKey, JToken> dictionary)
        {
            foreach (var pair in dictionary)
            {
                hashCode.Add(pair.Key);
                hashCode.Add(pair.Value.ToString());
            }
        }

        public static int AddMissing<T> (this IList<T> source, IEnumerable<T>? other) where T : class
        {
            if (!other.SafeAny())
                return 0;

            if (!source.Any())
            {
                source.Add(other);
                return source.Count;
            }

            var sourceGrouped = source.GroupBy(i => i).Select(g => new { Value = g.Key, Count = g.Count() });
            var otherGrouped = other.GroupBy(i => i).Select(g => new { Value = g.Key, Count = g.Count() });

            int count = 0;
            foreach (var o in otherGrouped)
            {
                var s = sourceGrouped.FirstOrDefault(i => o.Value.Equals(i.Value));
                int add = s != null ? o.Count - s.Count : o.Count;

                for (int i = add; i > 0; i--)
                {
                    source.Add(o.Value);
                    count++;
                }
            }

            return count;
        }

        public static T? Deserialize<T> (this JToken token)
            => typeof(T) == typeof(string) && token.Type == JTokenType.String ? (T?)(object)token.ToString()
             : typeof(T).IsAssignableTo(typeof(IEnumerable)) && token.Type != JTokenType.Array ? JsonSerializer.Create(Global.SerializerSettings).Deserialize<T>(new JArray(token).CreateReader())
             : JsonSerializer.Create(Global.SerializerSettings).Deserialize<T>(token.CreateReader());

        public static string GetClassName (this Type type)
        {
            string result = type.Name;
            if (type.IsGenericType)
            {
                string name = type.Name[..type.Name.IndexOf('`')];
                var genericTypes = type.GenericTypeArguments;
                result = $"{name}<{string.Join(",", genericTypes.Select(GetClassName))}>";
            }

            return result;
        }

        /// <summary>
        ///     If type is generic, returns generic type definition, else returns input type
        /// </summary>
        public static Type GetIfGenericTypeDefinition (this Type type) => type.IsGenericType && !type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition() : type;

        /// <summary>
        ///     If type is generic and underlying type exists at index, returns that underlying
        ///     type, else returns null
        /// </summary>
        /// <param name="index">Underlying Type Index</param>
        public static Type? GetIfUnderlyingType (this Type type, int index = 0) => type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericArguments().Length > index ? type.GetGenericArguments()[index] : null;

        /// <summary>
        ///     Is this record context the master
        /// </summary>
        public static bool IsMaster (this IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context) => context.ModKey.Equals(context.Record.FormKey.ModKey);

        /// <summary> Same as IEnumerable<>.Any() but will return false instead of throwing
        /// ArgumentNullException if null. </summary> <returns>False if null else result of .Any()</returns>
        public static bool SafeAny<TSource> ([NotNullWhen(true)] this IEnumerable<TSource>? source) => source != null && source.Any();

        public static bool SafeAny<TSource> (this IEnumerable<TSource>? source, Func<TSource, bool> predicate) => source != null && source.Any(predicate);

        // Handles duplicates. If 3 in source and 1 in notInList result will have 2 copies in it
        public static IEnumerable<T> WhereNotIn<T> (this IEnumerable<T>? source, IEnumerable<T>? notInList) where T : class
        {
            if (!source.SafeAny())
                return [];

            if (!notInList.SafeAny())
                return [.. source]; // Create new array to prevent errors if source modified

            var list = new List<T>(source);

            foreach (var ni in notInList)
                _ = list.Remove(ni);

            return list;
        }
    }
}