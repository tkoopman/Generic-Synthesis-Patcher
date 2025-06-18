using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class FactionOwner : OwnerBase
    {
        [JsonProperty(PropertyName = "Faction")]
        public FormKeyListOperationAdvanced<IFactionGetter>? Faction { get; set; }

        [JsonProperty(PropertyName = "RequiredRank")]
        public int RequiredRank { get; set; }

        public override bool Equals (object? obj) => Equals(obj as FactionOwner);

        public bool Equals (FactionOwner? other)
            => other is not null
            && RequiredRank == other.RequiredRank
            && object.Equals(Faction, other.Faction);

        public override int GetHashCode ()
        {
            var hash = new HashCode ();
            hash.Add(Faction);
            hash.Add(RequiredRank);
            return hash.ToHashCode();
        }

        public override OwnerTarget ToActionData ()
        {
            if (Faction is null || Faction.Value == FormKey.Null)
                return new NoOwner();

            var owner = new Mutagen.Bethesda.Skyrim.FactionOwner
            {
                Faction = Faction.Value.ToLink<IFactionGetter>(),
                RequiredRank = RequiredRank
            };

            return owner;
        }
    }
}