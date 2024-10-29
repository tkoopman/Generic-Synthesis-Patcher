using GenericSynthesisPatcher.Json.Data.Action;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Json.Converters
{
    public class OwnerBaseConverter : JsonConverter<OwnerBase>
    {
        public override OwnerBase? ReadJson (JsonReader reader, Type objectType, OwnerBase? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = jObject["NPC"] != null ? new NpcOwner()
                : jObject["Faction"] != null ? (OwnerBase)new FactionOwner()
                : null;

            if (target == null)
                return null;

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson (JsonWriter writer, OwnerBase? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}