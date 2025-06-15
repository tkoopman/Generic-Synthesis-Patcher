using Loqui;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Universal.Json.Converters
{
    public class RecordTypeConverter : JsonConverter<ILoquiRegistration>
    {
        public override ILoquiRegistration? ReadJson (JsonReader reader, Type objectType, ILoquiRegistration? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string? key = reader.Value?.ToString();
            return key is null ? null
                 : Global.Game.GetRecordType(key);
        }

        public override void WriteJson (JsonWriter writer, ILoquiRegistration? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}