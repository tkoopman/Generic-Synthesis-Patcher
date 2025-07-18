using GenericSynthesisPatcher.Rules;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Games.Universal.Json.Converters
{
    public class GSPBaseConverter : JsonConverter<GSPBase>
    {
        public override GSPBase? ReadJson (JsonReader reader, Type objectType, GSPBase? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = jObject.GetValue("rules", StringComparison.OrdinalIgnoreCase) is not null ? new GSPGroup() : (GSPBase)new GSPRule();

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson (JsonWriter writer, GSPBase? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}