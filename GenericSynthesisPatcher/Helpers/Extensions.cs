using System.Collections;
using System.Reflection;

using Common;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Helpers
{
    public static class Extensions
    {
        /// <summary>
        ///     Deserialize JSON token with my standard stuff, depending on output type and JSOn
        ///     token type.
        /// </summary>
        public static T? Deserialize<T> (this JToken token)
                            => typeof(T) == typeof(string) && token.Type == JTokenType.String ? (T?)(object)token.ToString()
             : typeof(T).IsAssignableTo(typeof(IEnumerable)) && token.Type != JTokenType.Array ? JsonSerializer.Create(Global.Game.SerializerSettings).Deserialize<T>(new JArray(token).CreateReader())
             : JsonSerializer.Create(Global.Game.SerializerSettings).Deserialize<T>(token.CreateReader());

        public static object GetSingletonInstance (this Type type) => type.GetField("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null) ??
            throw new InvalidOperationException($"Type {type.Name} is not a singleton.");

        public static T GetSingletonInstance<T> (this Type type) where T : class => type.GetSingletonInstance() as T ??
            throw new InvalidOperationException($"Type {type.Name} is not of type {typeof(T).GetClassName()}.");
    }
}