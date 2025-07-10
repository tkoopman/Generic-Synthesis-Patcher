using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Games.Fallout4.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Games.Fallout4.Action
{
    public class ContainerItemsAction : FormLinksWithDataAction<ContainerItemsData, IItemGetter, ContainerEntry>
    {
        public static readonly ContainerItemsAction Instance = new();
        private const int ClassLogCode = 0xE1;

        public override ContainerEntry? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not IContainerEntryGetter sourceRecord)
            {
                Global.Logger.WriteLog(LogLevel.Error, Helpers.LogType.RecordUpdateFailure, $"Failed to add item. No Items?", ClassLogCode);
                return null;
            }

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = sourceRecord.Item.Item.FormKey;
            containerEntry.Item.Count = sourceRecord.Item.Count;

            return containerEntry;
        }

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IContainerEntryGetter l
            && right is IContainerEntryGetter r
            && l.Item.Item.FormKey.Equals(r.Item.Item.FormKey)
            && l.Item.Count == r.Item.Count;

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from) => from is IContainerEntryGetter record ? record.Item.Item.FormKey : throw new ArgumentNullException(nameof(from));

        public override string ToString (IFormLinkContainerGetter source) => source is IContainerEntryGetter item ? $"{item.Item.Count}x{item.Item.Item.FormKey}" : throw new InvalidCastException();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "JSON objects containing item Form Key/Editor ID and Count (QTY)";
            example = $$"""
                        "{{propertyName}}": { "Item": "021FED:Fallout4.esm", "Count": 3 }
                        """;

            return true;
        }
    }
}