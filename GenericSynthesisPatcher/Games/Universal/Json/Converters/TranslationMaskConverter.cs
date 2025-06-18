using Common;

using Loqui;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Games.Universal.Json.Converters
{
    /// <summary>
    ///     Converter for <see cref="ITranslationMask" /> implementations. Must have constructor
    ///     that takes two boolean parameters:
    ///     defaultOn: If not defined in JSON, the will default to true IF all defined properties
    ///     are false. Else it will default to false
    /// </summary>
    public class TranslationMaskConverter : JsonConverter
    {
        public override bool CanConvert (Type objectType) => objectType.IsClass && objectType.IsAssignableTo(typeof(ITranslationMask));

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var con = objectType.GetConstructor([typeof(bool), typeof(bool)]) ?? throw new JsonSerializationException($"Type {objectType.GetClassName()} does not have a public constructor that takes two boolean parameters.");

            switch (reader.TokenType)
            {
                case JsonToken.Boolean:
                    bool value = (bool)(reader.Value ?? throw new JsonSerializationException("Unable to read boolean value for TranslationMask"));

                    // If boolean then we create using same method as the implicit operator
                    // TranslationMask's have
                    return con.Invoke([value, value]);

                case JsonToken.StartObject:

                    var jObject = JObject.Load(reader);
                    if (!tryBoolValue(jObject, "DefaultOn", out bool defaultOn))
                    {
                        // DefaultOn is not specified, determine it based on the defined properties
                        // DefaultOn is true if nothing defined or all defined properties are false.
                        defaultOn = !jObject.Values().Any() || !jObject.Values().Any(v => v.Type != JTokenType.Boolean || v.Value<bool>());
                    }

                    bool onOverAll = (bool)(con.GetParameters()[1].DefaultValue ?? throw new JsonSerializationException($"Type {objectType.GetClassName()} does not have a public constructor that takes one boolean parameter."));
                    var mask = con.Invoke([defaultOn, onOverAll]) as ITranslationMask ?? throw new JsonSerializationException($"Type {objectType.GetClassName()} failed to create.");

                    // Set any included properties on the mask
                    foreach (var field in objectType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    {
                        if (field.IsInitOnly)
                            continue; // Skip readonly fields

                        var jToken = jObject.GetValue(field.Name, StringComparison.OrdinalIgnoreCase);
                        if (jToken is null)
                            continue;

                        if (field.FieldType == typeof(bool))
                        {
                            if (jToken.Type != JTokenType.Boolean)
                                throw new JsonSerializationException($"Expected boolean value for '{field.Name}' but found {jToken.Type}.");
                            field.SetValue(mask, jToken.Value<bool>());
                            continue;
                        }

                        if (!field.FieldType.IsAssignableTo(typeof(ITranslationMask)))
                            continue;

                        if (jToken.Type is JTokenType.Boolean or JTokenType.Object)
                        {
                            object? pValue = serializer.Deserialize(jToken.CreateReader(), field.FieldType);
                            field.SetValue(mask, pValue);
                            continue;
                        }

                        throw new JsonSerializationException($"Expected boolean or object value for '{field.Name}' but found {jToken.Type}.");
                    }

                    return mask;

                default:
                    throw new JsonSerializationException($"Invalid token type {reader.TokenType} found for TranslationMask.");
            }
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();

        private static bool tryBoolValue (JObject jObject, string name, out bool value)
        {
            var token = jObject.GetValue(name, StringComparison.OrdinalIgnoreCase);
            if (token is null || token.Type != JTokenType.Boolean)
            {
                value = false;
                return false;
            }

            value = token.Value<bool>();
            return true;
        }
    }
}