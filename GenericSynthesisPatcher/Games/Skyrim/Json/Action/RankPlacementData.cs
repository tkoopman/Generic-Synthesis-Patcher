using System.ComponentModel;

using GenericSynthesisPatcher.Games.Universal.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class RankPlacementData (FormKeyListOperation<IFactionGetter> formKey, sbyte rank) : FormLinksWithDataActionDataBase<IFactionGetter, RankPlacement>, IEquatable<RankPlacementData>
    {
        [JsonProperty(PropertyName = "Faction", Required = Required.Always)]
        public override FormKeyListOperation<IFactionGetter> FormKey { get; } = formKey ?? new(null);

        [JsonProperty(PropertyName = "Rank", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public sbyte Rank { get; set; } = rank;

        public static bool Equals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IRankPlacementGetter l
            && right is IRankPlacementGetter r
            && l.Faction.FormKey.Equals(r.Faction.FormKey)
            && l.Rank == r.Rank;

        public bool Equals (RankPlacementData? other) => FormKey.Equals(other?.FormKey) && Rank.Equals(other?.Rank);

        public override bool Equals (object? obj) => Equals(obj as RankPlacementData);

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is IRankPlacementGetter otherRank
            && FormKey.ValueEquals(otherRank.Faction.FormKey)
            && otherRank.Rank == Rank;

        public override int GetHashCode () => FormKey.GetHashCode() ^ Rank;

        public override RankPlacement ToActionData ()
        {
            var rank = new RankPlacement();
            rank.Faction.FormKey = FormKey.Value;
            rank.Rank = Rank;

            return rank;
        }

        public override string? ToString () => $"{Rank}-{FormKey.Value}";
    }
}