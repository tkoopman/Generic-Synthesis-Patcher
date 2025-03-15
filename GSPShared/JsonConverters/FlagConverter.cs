using Newtonsoft.Json;

namespace GSPShared.JsonConverters
{
    public class FlagConverter : JsonConverter
    {
        public override bool CanConvert (Type objectType) => objectType.IsEnum;

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            objectType = objectType.GenericTypeArguments.FirstOrDefault(objectType);

            if (reader.TokenType == JsonToken.String)
                return Enum.Parse(objectType, serializer.Deserialize<string>(reader) ?? "", true);

            if (reader.TokenType == JsonToken.StartArray)
            {
                string[] flags = serializer.Deserialize<string[]>(reader) ?? [];
                return Enum.Parse(objectType, string.Join(',', flags), true);
            }

            throw new JsonReaderException("Invalid type found for Flag");
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}