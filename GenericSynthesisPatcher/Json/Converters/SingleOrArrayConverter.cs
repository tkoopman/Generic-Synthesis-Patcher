using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Converters
{
    internal class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert ( Type objectType ) => objectType == typeof(T) || objectType == typeof(List<T>);

        public override object? ReadJson ( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer ) => reader.TokenType switch
        {
            JsonToken.Null => null,
            JsonToken.StartArray => serializer.Deserialize<List<T>>(reader),
            JsonToken.StartObject => [serializer.Deserialize<T>(reader)],
            JsonToken.String => [serializer.Deserialize<T>(reader)],
            _ => throw new JsonSerializationException($"Invalid Json object - {reader.TokenType}")
        };

        public override void WriteJson ( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotImplementedException();
    }
}