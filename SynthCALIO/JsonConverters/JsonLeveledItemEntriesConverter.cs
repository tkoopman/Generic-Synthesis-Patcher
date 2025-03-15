using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace SynthCALIO.JsonConverters
{
    public partial class JsonLeveledItemEntriesConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert (Type objectType) => objectType == typeof(List<JsonLeveledItemEntry>);

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            List<JsonLeveledItemEntry> entries = [];

            var data = reader.TokenType switch
            {
                JsonToken.StartArray => serializer.Deserialize<List<string>>(reader) ?? throw new JsonSerializationException("Unable to read entries"),
                JsonToken.String => [(string)(reader.Value ?? throw new JsonSerializationException("Unable to read entries"))],
                _ => throw new JsonSerializationException("Unable to read entries"),
            };

            foreach (string entry in data)
            {
                if (!tryParseLeveledItemEntry(entry, out var item))
                    throw new JsonSerializationException($"Unable to parse leveled item entry: {entry}");
                entries.Add(item);
            }

            return entries;
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();

        [GeneratedRegex(@"^(?:\[(?:[Ll][vV])?(?'Level'\d+)\]\s?)?(?:(?'Count'\d+)x\s?)?(?'ID'.+)$")]
        private static partial Regex RegexLeveledItemEntry ();

        private static bool tryParseLeveledItemEntry (string entry, [MaybeNullWhen(false)] out JsonLeveledItemEntry data)
        {
            data = default;

            var m = RegexLeveledItemEntry().Match(entry);
            if (!m.Success)
                return false;

            short count = m.Groups.TryGetValue("Count", out var c) && !string.IsNullOrWhiteSpace(c.Value) ? short.Parse(c.Value) : (short)1;
            short level = m.Groups.TryGetValue("Level", out var l) && !string.IsNullOrWhiteSpace(l.Value) ? short.Parse(l.Value) : (short)1;
            data = new()
            {
                Level = level,
                Count = count,
                ID = m.Groups["ID"].Value
            };

            return true;
        }
    }
}