using GenericSynthesisPatcher.Helpers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Json.Converters
{
    public class DictionaryConverter<TKey, TValue> : JsonConverter where TKey : notnull
    {
        public override bool CanConvert (Type objectType) => objectType.IsEnum;

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var output = new Dictionary<TKey, TValue?>();

            if (reader.TokenType == JsonToken.String)
            {
                output.Add(serializer.Deserialize<TKey>(reader) ?? throw new JsonReaderException($"Failed to Deserialize directory key to type {typeof(TKey)}."), default);
                return output;
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                var keys = serializer.Deserialize<TKey[]>(reader) ?? [];
                foreach (var key in keys)
                    output.Add(key, default);

                return output;
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                var value = serializer.Deserialize<JObject>(reader) ?? [];

                foreach (var x in value ?? [])
                {
                    if (x.Value != null)
                    {
                        var key = typeof(TKey) == typeof(string) ? (TKey)(object)x.Key : (TKey)(new JValue(x.Key).Deserialize<TKey>() ?? throw new JsonReaderException($"Failed to Deserialize directory key to type {typeof(TKey)}."));

                        if (x.Value is TValue v)
                            output.Add(key, v);
                        else
                            output.Add(key, x.Value.Deserialize<TValue>());
                    }
                }

                return output;
            }

            throw new JsonReaderException($"Invalid type of {reader.TokenType} found.");
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}