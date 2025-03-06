using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Json.Data.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class RankPlacementAction : FormLinksWithDataAction<RankPlacementData, IFactionGetter, RankPlacement>
    {
        public static readonly RankPlacementAction Instance = new();
        private const int ClassLogCode = 0x16;

        public override RankPlacement? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not IRankPlacementGetter sourceRecord)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to add item. No Items?", logLevel: LogLevel.Error);
                return null;
            }

            var rank = new RankPlacement
            {
                Faction = sourceRecord.Faction.FormKey.ToLink<IFactionGetter>(),
                Rank = sourceRecord.Rank
            };

            return rank;
        }

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IRankPlacementGetter l
            && right is IRankPlacementGetter r
            && l.Faction.FormKey.Equals(r.Faction.FormKey)
            && l.Rank == r.Rank;

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from) => from is IRankPlacementGetter record ? record.Faction.FormKey : throw new ArgumentNullException(nameof(from));

        public override string ToString (IFormLinkContainerGetter source) => source is IRankPlacementGetter rank ? $"{rank.Rank}-{rank.Faction.FormKey}" : throw new InvalidCastException();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "JSON objects containing faction Form Key/Editor ID and Rank";
            example = $$"""
                        "{{propertyName}}": { "Faction": "021FED:Skyrim.esm", "Rank": 0 }
                        """;

            return true;
        }
    }
}