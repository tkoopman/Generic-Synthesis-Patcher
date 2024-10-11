using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Converters
{
    public partial class ModKeyConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert ( Type objectType ) => objectType == typeof(ModKey);

        public override object? ReadJson ( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer )
        {
            string? key = reader.Value?.ToString();
            return (key != null) ? ModKey.FromFileName(key) : ModKey.Null;
        }

        public override void WriteJson ( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotImplementedException();
    }
}