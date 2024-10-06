using GenericSynthesisPatcher.Json.Data;

using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json;

using System.Text.RegularExpressions;

namespace GenericSynthesisPatcher.Json.Converters
{
    public partial class FilterFormLinksConverter : JsonConverter
    {
        public override bool CanConvert ( Type objectType ) => objectType == typeof(FilterFormLinks);
        public override object? ReadJson ( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer )
        {
            string? key = reader.Value?.ToString();
            return (key != null) ? new FilterFormLinks(key) : null;
        }

        public override bool CanWrite => false;
        public override void WriteJson ( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotImplementedException();

    }
}