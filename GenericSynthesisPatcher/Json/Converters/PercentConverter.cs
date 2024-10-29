using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Json.Converters
{
    public class PercentConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert (Type objectType) => objectType == typeof(Percent);

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Bytes:
                    double p = reader.ReadAsDouble() ?? throw new JsonSerializationException("Unable to read percent");
                    return new Percent(p);

                case JsonToken.String:
                    string str = reader.ReadAsString() ?? throw new JsonSerializationException("Unable to read percent");
                    bool signed = str.EndsWith('%');
                    double pd = double.Parse(str.TrimEnd('%'));
                    if (signed)
                        pd /= 100;
                    return new Percent(pd);

                default:
                    throw new JsonSerializationException("Unable to read percent");
            }
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}