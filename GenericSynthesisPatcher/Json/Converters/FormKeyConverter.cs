using System.Text.RegularExpressions;

using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Converters
{
    public partial class FormKeyConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public static string FixFormKey ( string input ) => RegexFormKey().Replace(input, m => m.Value.PadLeft(6, '0'));

        public override bool CanConvert ( Type objectType ) => objectType == typeof(FormKey);

        public override object? ReadJson ( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer )
        {
            string? key = reader.Value?.ToString();
            return (key != null) ? FormKey.Factory(global::GenericSynthesisPatcher.Json.Converters.FormKeyConverter.FixFormKey(key)) : null;
        }

        public override void WriteJson ( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotImplementedException();

        [GeneratedRegex(@"^[0-9A-Fa-f]{1,6}")]
        private static partial Regex RegexFormKey ();
    }
}