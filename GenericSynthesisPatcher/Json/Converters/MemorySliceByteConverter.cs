using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Json.Converters
{
    public class MemorySliceByteConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert (Type objectType) => objectType == typeof(MemorySlice<byte>);

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    var data = serializer.Deserialize<List<byte>>(reader) ?? throw new JsonSerializationException("Unable to read byte array.");

                    return new MemorySlice<byte>([.. data]);
            }

            throw new JsonSerializationException("Unable to read byte array.");
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}