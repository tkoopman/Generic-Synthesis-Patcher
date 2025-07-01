using Common;

using GenericSynthesisPatcher.Helpers;

using Loqui;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Noggog;

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

                    if (!tryBoolValue(jObject, "OnOverall", out bool onOverall))
                    {
                        // OnOverall is not specified, default to true
                        onOverall = (bool)(con.GetParameters()[1].DefaultValue ?? throw new JsonSerializationException($"Type {objectType.GetClassName()} does not have a public constructor that takes one boolean parameter."));
                    }

                    var mask = con.Invoke([defaultOn, onOverall]) as ITranslationMask ?? throw new JsonSerializationException($"Type {objectType.GetClassName()} failed to create.");

                    foreach (var property in jObject.Properties().Where(p => !p.Name.Equals("DefaultOn", StringComparison.OrdinalIgnoreCase) && !p.Name.Equals("OnOverall", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (property.Value.Type == JTokenType.Boolean)
                        {
                            // If the property is a boolean, set it directly
                            _ = mask.TrySetValue(property.Name, property.Value.Value<bool>(), StringComparison.OrdinalIgnoreCase);
                        }
                        else if (property.Value.Type == JTokenType.Object)
                        {
                            // If the property is an object, deserialize it
                            if (mask.TryGetMaskField(property.Name, StringComparison.OrdinalIgnoreCase, out var field))
                            {
                                object? obj = serializer.Deserialize(property.Value.CreateReader(), field.FieldType) ?? throw new JsonSerializationException($"Failed to deserialize '{property.Name}' into {field.FieldType.GetClassName()}.");
                                field.SetValue(mask, obj);
                            }
                        }
                        else
                        {
                            throw new JsonSerializationException($"Expected boolean or object value for '{property.Name}' but found {property.Value.Type}.");
                        }
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