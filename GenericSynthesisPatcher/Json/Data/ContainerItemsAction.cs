using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Json.Data
{
    public class ContainerItemsAction ( OperationFormLink formKey, int count ) : IFormLinksWithData<ContainerItemsAction>
    {
        [JsonProperty(PropertyName = "count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [System.ComponentModel.DefaultValue(1)]
        public int Count = count;

        private const int ClassLogPrefix = 0x900;

        [JsonProperty(PropertyName = "item", Required = Required.Always)]
        public OperationFormLink FormKey { get; set; } = formKey;

        public static int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter source )
        {
            if (source is not IContainerEntryGetter sourceRecord)
            {
                LogHelper.Log(Microsoft.Extensions.Logging.LogLevel.Error, context, $"Failed to add item. No Items?", ClassLogPrefix | 0x21);
                return -1;
            }

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = sourceRecord.Item.Item.FormKey;
            containerEntry.Item.Count = sourceRecord.Item.Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            var items = (ExtendedList<ContainerEntry>?)GetItems(patch);

            if (items == null)
            {
                items = [];
                SetItems(patch, items);
                LogHelper.Log(Microsoft.Extensions.Logging.LogLevel.Trace, context, "Created new list.", 0x22);
            }

            items.Add(containerEntry);
            return 1;
        }

        public static bool DataEquals ( IFormLinkContainerGetter left, IFormLinkContainerGetter right ) => left is IContainerEntryGetter l && right is IContainerEntryGetter r && l.Item.Item.FormKey.Equals(r.Item.Item.FormKey) && l.Item.Count == r.Item.Count;

        public static IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list, FormKey key ) => list?.SingleOrDefault(s => (s != null) && GetFormKey(s).Equals(key), null);

        public static List<ContainerItemsAction>? GetFillValueAs ( GSPRule rule, ValueKey key ) => rule.GetFillValueAs<List<ContainerItemsAction>>(key);

        public static FormKey GetFormKey ( IFormLinkContainerGetter from ) => from is IContainerEntryGetter record ? record.Item.Item.FormKey : throw new ArgumentNullException(nameof(from));

        public static int Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter remove )
        {
            if (remove is not IContainerEntryGetter entry)
                return -1;

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = entry.Item.Item.FormKey;
            containerEntry.Item.Count = entry.Item.Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            var items = (ExtendedList<ContainerEntry>?)GetItems(patch);
            if (items == null || !items.Remove(containerEntry))
            {
                LogHelper.Log(Microsoft.Extensions.Logging.LogLevel.Error, context, $"Failed to remove item [{entry.Item.Item.FormKey}].", ClassLogPrefix | 0x11);
                return 0;
            }

            return 1;
        }

        public static int Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IEnumerable<IFormLinkContainerGetter>? newList )
        {
            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            var items = (ExtendedList<ContainerEntry>?)GetItems(patch);

            int changes = items?.RemoveAll(_ => true) ?? 0;

            if (newList is not IReadOnlyList<IContainerEntryGetter> list)
                return -1;

            foreach (var add in list)
            {
                _ = Add(context, ref patch, add);
                changes++;
            }

            return changes;
        }

        public int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch )
        {
            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = FormKey.FormKey;
            containerEntry.Item.Count = Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            var items = (ExtendedList<ContainerEntry>?)GetItems(patch);

            if (items == null)
            {
                items = [containerEntry];
                SetItems(patch, items);
            }
            else
            {
                items.Add(containerEntry);
            }

            return 1;
        }

        public bool DataEquals ( IFormLinkContainerGetter other ) => other is IContainerEntryGetter otherContainer && otherContainer.Item.Item.FormKey.Equals(FormKey.FormKey) && otherContainer.Item.Count == Count;

        public IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list ) => Find(list, FormKey.FormKey);

        private static IReadOnlyList<IContainerEntryGetter>? GetItems ( ISkyrimMajorRecordGetter? record )
            => (record is IContainerGetter cont) ? cont.Items :
               (record is INpcGetter npc) ? npc.Items : null;

        private static void SetItems ( ISkyrimMajorRecordGetter? record, ExtendedList<ContainerEntry> list )
        {
            if (record is IContainer cont)
                cont.Items = list;

            if (record is INpc npc)
                npc.Items = list;
        }
    }
}