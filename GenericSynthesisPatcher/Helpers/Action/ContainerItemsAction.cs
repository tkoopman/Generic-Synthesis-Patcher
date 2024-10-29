using GenericSynthesisPatcher.Json.Data.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class ContainerItemsAction : FormLinksWithDataAction<ContainerItemsData, IItemGetter, ContainerEntry>
    {
        public static readonly ContainerItemsAction Instance = new();
        private const int ClassLogCode = 0x16;

        public override ContainerEntry? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not IContainerEntryGetter sourceRecord)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to add item. No Items?", logLevel: LogLevel.Error);
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
    }
}