using System.Collections;

using Common;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Json.Converters
{
    public class NoggogPxConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert (Type objectType)
            => objectType == typeof(P2Double)
            || objectType == typeof(P2Float)
            || objectType == typeof(P2Int)
            || objectType == typeof(P2Int16)
            || objectType == typeof(P2UInt8)
            || objectType == typeof(P3Double)
            || objectType == typeof(P3Float)
            || objectType == typeof(P3Int)
            || objectType == typeof(P3Int16)
            || objectType == typeof(P3UInt16)
            || objectType == typeof(P3UInt8);

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            bool hasZ = objectType.GetProperty("Z") is not null;
            var t = objectType.GetProperty("X")?.PropertyType ?? throw new JsonReaderException($"Unable to get 'X' property type from {objectType.GetClassName()}");

            object x, y, z = null!;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    var listType = typeof(List<>).MakeGenericType(t);
                    var data = serializer.Deserialize(reader, listType) as IList ?? throw getException(objectType, hasZ);

                    x = data[0] ?? throw getException(objectType, hasZ);
                    y = data[1] ?? throw getException(objectType, hasZ);
                    if (hasZ)
                        z = data[2] ?? throw getException(objectType, hasZ);

                    break;

                case JsonToken.StartObject:
                    var jObject = JObject.Load(reader);
                    if (!jObject.ContainsKey("x")
                     || !jObject.ContainsKey("y")
                     || (hasZ && !jObject.ContainsKey("z")))
                    {
                        throw getExceptionObj(objectType, hasZ);
                    }

                    var dictType = typeof(Dictionary<,>).MakeGenericType(typeof(string), t);
                    var obj = serializer.Deserialize(jObject.CreateReader(), dictType) as IDictionary ?? throw getExceptionObj(objectType, hasZ);
                    x = obj["x"] ?? throw getExceptionObj(objectType, hasZ);
                    y = obj["y"] ?? throw getExceptionObj(objectType, hasZ);
                    if (hasZ)
                        z = obj["z"] ?? throw getExceptionObj(objectType, hasZ);

                    break;

                case JsonToken.Null:
                    return null;

                default:
                    throw new JsonSerializationException($"Unable to read {objectType.GetClassName()}.");
            }

            if (hasZ)
            {
                var con = objectType.GetConstructor([t, t, t]) ?? throw new JsonSerializationException($"Unable to construct {objectType.GetClassName()}.");
                return con.Invoke([x, y, z]);
            }
            else
            {
                var con = objectType.GetConstructor([t, t]) ?? throw new JsonSerializationException($"Unable to construct {objectType.GetClassName()}.");
                return con.Invoke([x, y]);
            }
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();

        private static JsonSerializationException getException (Type objectType, bool hasZ)
                            => hasZ
             ? new JsonSerializationException($"Unable to read {objectType.GetClassName()}. Array requires 3 numbers [x, y, z]")
             : new JsonSerializationException($"Unable to read {objectType.GetClassName()}. Array requires 2 numbers [x, y]");

        private static JsonSerializationException getExceptionObj (Type objectType, bool hasZ)
            => hasZ
             ? new JsonSerializationException($"Unable to read {objectType.GetClassName()}. Json Object requires x, y and z values.")
             : new JsonSerializationException($"Unable to read {objectType.GetClassName()}. Json Object requires x and y values.");
    }
}