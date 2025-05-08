using Common;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Universal.Json.Converters
{
    public class FormKeyConverter<T> : FormKeyConverter where T : class, IMajorRecordQueryableGetter, IMajorRecord
    {
        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            string? key = reader.Value?.ToString();
            return key == null ? null
                 : FormKey.TryFactory(SynthCommon.FixFormKey(key), out var formKey) ? formKey
                 : Global.State.LinkCache.TryResolve<T>(key, out var record) ? record.FormKey
                 : throw new JsonSerializationException($"Unable to parse \"{key}\" into valid FormKey or EditorID");
        }
    }

    public class FormKeyConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert (Type objectType) => objectType == typeof(FormKey);

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            string? key = reader.Value?.ToString();
            return key != null ? FormKey.Factory(SynthCommon.FixFormKey(key)) : null;
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}