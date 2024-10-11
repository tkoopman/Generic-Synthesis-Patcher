using GenericSynthesisPatcher.Json.Data;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Converters
{
    public partial class FilterFormLinksConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert ( Type objectType ) => objectType == typeof(FilterFormLinks);

        public override object? ReadJson ( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer )
        {
            string? key = reader.Value?.ToString();
            return (key != null) ? new FilterFormLinks(key) : null;
        }

        public override void WriteJson ( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotImplementedException();
    }
}