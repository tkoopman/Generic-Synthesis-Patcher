using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Games.Oblivion.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Games.Oblivion.Action
{
    public class ContainerItemsAction : FormLinksWithDataAction<ContainerItemsData, IItemGetter, ContainerItem>
    {
        public static readonly ContainerItemsAction Instance = new();
        private const int ClassLogCode = 0xC1;

        public override ContainerItem? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not IContainerItemGetter sourceRecord)
            {
                Global.Logger.WriteLog(LogLevel.Error, Helpers.LogType.RecordUpdateFailure, $"Failed to add item. No Items?", ClassLogCode);
                return null;
            }

            var containerEntry = new ContainerItem();
            containerEntry.Item.FormKey = sourceRecord.Item.FormKey;
            containerEntry.Count = sourceRecord.Count;

            return containerEntry;
        }

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IContainerItemGetter l
            && right is IContainerItemGetter r
            && l.Item.FormKey.Equals(r.Item.FormKey)
            && l.Count == r.Count;

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from) => from is IContainerItemGetter record ? record.Item.FormKey : throw new ArgumentNullException(nameof(from));

        public override string ToString (IFormLinkContainerGetter source) => source is IContainerItemGetter item ? $"{item.Count}x{item.Item.FormKey}" : throw new InvalidCastException();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "JSON objects containing item Form Key/Editor ID and Count (QTY)";
            example = $$"""
                        "{{propertyName}}": { "Item": "021FED:Oblivion.esm", "Count": 3 }
                        """;

            return true;
        }
    }
}