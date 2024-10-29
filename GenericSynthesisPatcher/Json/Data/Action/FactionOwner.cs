using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class FactionOwner : OwnerBase
    {
        [JsonProperty(PropertyName = "Faction")]
        public FormKeyListOperationAdvanced<IFactionGetter>? Faction { get; set; }

        [JsonProperty(PropertyName = "RequiredRank")]
        public int RequiredRank { get; set; }

        public override OwnerTarget ToActionData ()
        {
            if (Faction == null || Faction.Value == FormKey.Null)
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