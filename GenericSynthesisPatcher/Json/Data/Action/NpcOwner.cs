using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class NpcOwner : OwnerBase
    {
        [JsonProperty(PropertyName = "Global")]
        public FormKeyListOperationAdvanced<IGlobalGetter>? Global { get; set; }

        [JsonProperty(PropertyName = "NPC")]
        public FormKeyListOperationAdvanced<INpcGetter>? NPC { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals (NpcOwner? l, NpcOwner? r)
        {
            if (ReferenceEquals(l, r))
                return true;

            if (l == null || r == null)
                return false;

            return FormKeyListOperationAdvanced<IGlobalGetter>.Equals(l.Global, r.Global) && FormKeyListOperationAdvanced<INpcGetter>.Equals(r.NPC, l.NPC);
        }

        public override OwnerTarget ToActionData ()
        {
            if ((NPC == null || NPC.Value == FormKey.Null) && (Global == null || Global.Value == FormKey.Null))
                return new NoOwner();

            var owner = new Mutagen.Bethesda.Skyrim.NpcOwner();
            if (NPC != null && NPC.Value != FormKey.Null)
                owner.Npc = NPC.Value.ToLink<INpcGetter>();

            if (Global != null && Global.Value != FormKey.Null)
                owner.Global = Global.Value.ToLink<IGlobalGetter>();

            return owner;
        }
    }
}