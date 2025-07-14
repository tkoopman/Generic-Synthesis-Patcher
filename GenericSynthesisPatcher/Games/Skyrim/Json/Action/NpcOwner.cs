using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Rules.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class NpcOwner : OwnerBase
    {
        [JsonProperty(PropertyName = "Global")]
        public FormKeyListOperationAdvanced<IGlobalGetter>? Global { get; set; }

        [JsonProperty(PropertyName = "NPC")]
        public FormKeyListOperationAdvanced<INpcGetter>? NPC { get; set; }

        public static bool IsNull ([NotNullWhen(false)] NpcOwner? owner) => owner is null || ((owner.NPC is null || owner.NPC.Value == FormKey.Null) && (owner.Global is null || owner.Global.Value == FormKey.Null));

        public override bool Equals (object? obj) => obj is NpcOwner owner && Equals(owner);

        public bool Equals (NpcOwner? other)
            => (IsNull() && IsNull(other))
            || (!IsNull() && !IsNull(other)
            && object.Equals(Global, other.Global)
            && object.Equals(NPC, other.NPC));

        public override int GetHashCode () => HashCode.Combine(Global, NPC);

        public bool IsNull () => IsNull(this);

        public override OwnerTarget ToActionData ()
        {
            if (IsNull())
                return new NoOwner();

            var owner = new Mutagen.Bethesda.Skyrim.NpcOwner();
            if (NPC is not null && NPC.Value != FormKey.Null)
                owner.Npc = NPC.Value.ToLink<INpcGetter>();

            if (Global is not null && Global.Value != FormKey.Null)
                owner.Global = Global.Value.ToLink<IGlobalGetter>();

            return owner;
        }
    }
}