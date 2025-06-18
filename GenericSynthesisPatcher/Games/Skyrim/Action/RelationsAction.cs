using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Games.Skyrim.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Games.Skyrim.Action
{
    public class RelationsAction : FormLinksWithDataAction<RelationsData, IRelatableGetter, Relation>
    {
        public static readonly RelationsAction Instance = new();
        private const int ClassLogCode = 0x1D;

        public override Relation? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not IRelationGetter sourceRecord)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to add item. No Items?", logLevel: LogLevel.Error);
                return null;
            }

            var relation = new Relation
            {
                Target = sourceRecord.Target.FormKey.ToLink<IRelatableGetter>(),
                Modifier = sourceRecord.Modifier,
                Reaction = sourceRecord.Reaction
            };

            return relation;
        }

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IRelationGetter l
            && right is IRelationGetter r
            && l.Target.FormKey.Equals(r.Target.FormKey)
            && l.Modifier == r.Modifier
            && l.Reaction == r.Reaction;

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from) => from is IRelationGetter record ? record.Target.FormKey : throw new ArgumentNullException(nameof(from));

        public override string ToString (IFormLinkContainerGetter source) => source is IRelationGetter relation ? $"{relation.Reaction}-{relation.Target.FormKey}" : throw new InvalidCastException();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "JSON objects containing target Form Key/Editor ID, Reaction (Neutral, Enemy, Ally, Friend) and Modifier (Defaults to 0)";
            example = $$"""
                        "{{propertyName}}": { "Target": "021FED:Skyrim.esm", "Reaction": "Friend" }
                        """;

            return true;
        }
    }
}