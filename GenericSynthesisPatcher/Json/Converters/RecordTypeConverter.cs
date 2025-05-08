using GenericSynthesisPatcher.Games.Universal;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Converters
{
    public class RecordTypeConverter : JsonConverter<RecordTypeMapping>
    {
        public override RecordTypeMapping ReadJson (JsonReader reader, Type objectType, RecordTypeMapping existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string? key = reader.Value?.ToString();
            return key == null ? default
                 : Global.RecordTypeMappings.FindByName(reader.Value?.ToString());
        }

        public override void WriteJson (JsonWriter writer, RecordTypeMapping value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}