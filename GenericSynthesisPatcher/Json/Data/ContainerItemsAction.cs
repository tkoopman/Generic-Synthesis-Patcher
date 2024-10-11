using System.Data;

using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    public class ContainerItemsAction ( FilterFormLinks formKey, int count ) : IFormLinksWithData<ContainerItemsAction>
    {
        [JsonProperty(PropertyName = "count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [System.ComponentModel.DefaultValue(1)]
        public int Count = count;

        private const int ClassLogPrefix = 0x900;

        [JsonProperty(PropertyName = "item", Required = Required.Always)]
        public FilterFormLinks FormKey { get; set; } = formKey;

        public static int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter source )
        {
            if (context.Record is not IContainerGetter || source is not IContainerEntryGetter sourceRecord)
                return -1;

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = sourceRecord.Item.Item.FormKey;
            containerEntry.Item.Count = sourceRecord.Item.Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IContainer p)
            {
                p.Items?.Add(containerEntry);
                return 1;
            }

            return -1;
        }

        public static bool DataEquals ( IFormLinkContainerGetter left, IFormLinkContainerGetter right ) => left is IContainerEntryGetter l && right is IContainerEntryGetter r && l.Item.Item.FormKey.Equals(r.Item.Item.FormKey) && l.Item.Count == r.Item.Count;

        public static IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list, FormKey key ) => list?.SingleOrDefault(s => (s != null) && GetFormKey(s).Equals(key), null);

        public static FormKey GetFormKey ( IFormLinkContainerGetter from ) => from is IContainerEntryGetter record ? record.Item.Item.FormKey : throw new ArgumentNullException(nameof(from));

        public static List<ContainerItemsAction>? GetValueAs ( GSPRule rule, GSPRule.ValueKey key ) => rule.GetValueAs<List<ContainerItemsAction>>(key);

        public static int Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter remove )
        {
            if (!((patch == null || (patch is IContainer)) && remove is IContainerEntryGetter entry))
                return -1;

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = entry.Item.Item.FormKey;
            containerEntry.Item.Count = entry.Item.Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is not IContainer p || (!p.Items?.Remove(containerEntry) ?? false))
            {
                LogHelper.Log(Microsoft.Extensions.Logging.LogLevel.Error, context, $"Failed to remove item [{entry.Item.Item.FormKey}] from container.", ClassLogPrefix | 0x11);
                return 0;
            }

            return 1;
        }

        public static int Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IEnumerable<IFormLinkContainerGetter> newList )
        {
            if (context.Record is not IContainerGetter record || newList is not IReadOnlyList<IContainerEntryGetter> list)
                return -1;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IContainer p)
            {
                int changes = p.Items?.RemoveAll(_ => true) ?? 0;

                foreach (var add in list)
                {
                    _ = Add(context, ref patch, add);
                    changes++;
                }

                return changes;
            }

            return -1;
        }

        public int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch )
        {
            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = FormKey.FormKey;
            containerEntry.Item.Count = Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IContainer p)
            {
                p.Items?.Add(containerEntry);
                return 1;
            }

            return -1;
        }

        public bool DataEquals ( IFormLinkContainerGetter other ) => other is IContainerEntryGetter otherContainer && otherContainer.Item.Item.FormKey.Equals(FormKey.FormKey) && otherContainer.Item.Count == Count;

        public IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list ) => Find(list, FormKey.FormKey);
    }
}