using GenericSynthesisPatcher.Json.Converters;

using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    [JsonConverter(typeof(OwnerBaseConverter))]
    public abstract class OwnerBase
    {
        public abstract OwnerTarget ToActionData ();
    }
}