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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals (FactionOwner? l, FactionOwner? r)
        {
            if (ReferenceEquals(l, r))
                return true;

            if (l == null && r == null)
                return true;

            if (l == null || r == null)
                return false;

            return l.RequiredRank == r.RequiredRank && FormKeyListOperationAdvanced<IFactionGetter>.Equals(l.Faction, r.Faction);
        }

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