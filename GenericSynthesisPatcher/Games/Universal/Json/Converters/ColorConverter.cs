using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Universal.Json.Converters
{
    public partial class ColorConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert (Type objectType) => objectType == typeof(Color);

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    int c = reader.ReadAsInt32() ?? throw new JsonSerializationException("Unable to read color");
                    return Color.FromArgb(c);

                case JsonToken.StartArray:
                    var data = serializer.Deserialize<List<short>>(reader);
                    if (data is null || data.Count < 3 || data.Count > 4)
                        throw new JsonSerializationException("Unable to read object bounds. Array requires 3 or 4 numbers [A,R,G,B] or [R,G,B]");

                    if (data.Count == 3)
                    {
                        return Color.FromArgb(
                                data[0],
                                data[1],
                                data[2]);
                    }

                    return Color.FromArgb(
                            data[0],
                            data[1],
                            data[2],
                            data[3]);

                case JsonToken.String:
                    string str = reader.ReadAsString() ?? throw new JsonSerializationException("Unable to read color");

                    var argb = HexRegex().Match(str);
                    if (argb.Success)
                    {
                        return Color.FromArgb(
                            argb.Groups.ContainsKey("a") ? byte.Parse(argb.Groups["a"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture) : byte.MaxValue,
                            byte.Parse(argb.Groups["r"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                            byte.Parse(argb.Groups["g"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                            byte.Parse(argb.Groups["b"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                    }

                    var color = Color.FromName(str);

                    if (color.IsKnownColor)
                        return color;
                    break;
            }

            throw new JsonSerializationException("Unable to read color");
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();

        [GeneratedRegex(@"^#(?<a>[0-9A-Fa-f]{2})?(?<r>[0-9A-Fa-f]{2})(?<g>[0-9A-Fa-f]{2})(?<b>[0-9A-Fa-f]{2})$")]
        private static partial Regex HexRegex ();
    }
}