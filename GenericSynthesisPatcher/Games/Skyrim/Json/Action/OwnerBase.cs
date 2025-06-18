using GenericSynthesisPatcher.Games.Skyrim.Json.Converters;

using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    [JsonConverter(typeof(OwnerBaseConverter))]
    public abstract class OwnerBase
    {
        public abstract override bool Equals (object? obj);

        public abstract override int GetHashCode ();

        public abstract OwnerTarget ToActionData ();
    }
}