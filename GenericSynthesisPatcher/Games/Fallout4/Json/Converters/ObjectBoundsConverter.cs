using Mutagen.Bethesda.Fallout4;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Noggog;

namespace GenericSynthesisPatcher.Games.Fallout4.Json.Converters
{
    public class ObjectBoundsConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert (Type objectType) => objectType == typeof(ObjectBounds);

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    var data = serializer.Deserialize<List<short>>(reader);
                    if (data is null || data.Count != 6)
                        throw new JsonSerializationException("Unable to read object bounds. Array requires 6 numbers [x1,y1,z1,x2,y2,z2]");

                    return new ObjectBounds()
                    {
                        First = new P3Int16(data[0], data[1], data[2]),
                        Second = new P3Int16(data[3], data[4], data[5]),
                    };

                case JsonToken.StartObject:
                    var jObject = JObject.Load(reader);
                    if (jObject.ContainsKey("x1")
                     && jObject.ContainsKey("y1")
                     && jObject.ContainsKey("z1")
                     && jObject.ContainsKey("x2")
                     && jObject.ContainsKey("y2")
                     && jObject.ContainsKey("z2"))
                    {
                        var obj = serializer.Deserialize<Dictionary<string, short>>(jObject.CreateReader());
                        if (obj is not null)
                        {
                            return new ObjectBounds()
                            {
                                First = new P3Int16(obj["x1"], obj["y1"], obj["z1"]),
                                Second = new P3Int16(obj["x2"], obj["y2"], obj["z2"]),
                            };
                        }
                    }

                    break;
            }

            throw new JsonSerializationException("Unable to read object bounds.");
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}