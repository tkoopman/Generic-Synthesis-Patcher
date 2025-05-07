using System.Collections;
using System.Text.RegularExpressions;

using Common;

using DynamicData;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Helpers
{
    public static partial class Extensions
    {
        /// <summary>
        ///     Deserialize JSON token with my standard stuff, depending on output type and JSOn
        ///     token type.
        /// </summary>
        public static T? Deserialize<T> (this JToken token)
                            => typeof(T) == typeof(string) && token.Type == JTokenType.String ? (T?)(object)token.ToString()
             : typeof(T).IsAssignableTo(typeof(IEnumerable)) && token.Type != JTokenType.Array ? JsonSerializer.Create(Global.SerializerSettings).Deserialize<T>(new JArray(token).CreateReader())
             : JsonSerializer.Create(Global.SerializerSettings).Deserialize<T>(token.CreateReader());
    }
}