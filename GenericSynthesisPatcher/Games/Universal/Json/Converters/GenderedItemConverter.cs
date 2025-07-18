using Common;

using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Games.Universal.Json.Converters
{
    /// <summary>
    ///     Converter for <see cref="GenderedItem" /> implementations.
    /// </summary>
    public class GenderedItemConverter : JsonConverter
    {
        public override bool CanConvert (Type objectType)
        {
            var exploded = objectType.Explode(2);
            return exploded.Length == 2 && exploded[0] == typeof(GenderedItem<>);
        }

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var exploded = objectType.Explode(2);
            var type = exploded[1];
            if (reader.TokenType == JsonToken.StartObject)
            {
                var jObject = JObject.Load(reader);
                var male = jObject.Property("Male", StringComparison.OrdinalIgnoreCase);
                var female = jObject.Property("Female", StringComparison.OrdinalIgnoreCase);

                if (male is not null && female is not null)
                {
                    object? maleValue = serializer.Deserialize(male.Value.CreateReader(), type);
                    object? femaleValue = serializer.Deserialize(female.Value.CreateReader(), type);

                    return Activator.CreateInstance(objectType, maleValue, femaleValue) ?? throw new JsonSerializationException($"Failed to create instance of {objectType.GetClassName()} with provided values.");
                }

                object objValue = serializer.Deserialize(jObject.CreateReader(), type) ?? throw new JsonSerializationException($"Failed to create instance of {objectType.GetClassName()} with provided values.");
                return Activator.CreateInstance(objectType, objValue, objValue) ?? throw new JsonSerializationException($"Failed to create instance of {objectType.GetClassName()} with provided values.");
            }

            if (reader.TokenType == JsonToken.Boolean)
            {
                bool value = reader.ReadAsBoolean() ?? throw new JsonSerializationException($"Failed to create instance of {objectType.GetClassName()} with provided values.");
                return Activator.CreateInstance(objectType, value, value) ?? throw new JsonSerializationException($"Failed to create instance of {objectType.GetClassName()} with provided values.");
            }

            throw new JsonSerializationException($"Failed to create instance of {objectType.GetClassName()} with provided values.");
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var property = value.GetType().GetProperty(nameof(MaleFemaleGender.Female), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) ?? throw new JsonSerializationException($"Type {value.GetType().GetClassName()} doesn't seem to be correct IGenderedItem<T> type");

            object? female = property.GetValue(value);

            property = value.GetType().GetProperty(nameof(MaleFemaleGender.Male), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (property is null)
                throw new JsonSerializationException($"Type {value.GetType().GetClassName()} doesn't seem to be correct IGenderedItem<T> type");

            object? male = property.GetValue(value);

            if (Equals(female, male))
            {
                serializer.Serialize(writer, female);
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName(nameof(MaleFemaleGender.Female));
            serializer.Serialize(writer, female);

            writer.WritePropertyName(nameof(MaleFemaleGender.Male));
            serializer.Serialize(writer, male);

            writer.WriteEndObject();
        }
    }
}