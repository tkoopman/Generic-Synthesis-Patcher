using System.Data;

using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    public class ContainerItemsAction ( FilterFormLinks formKey, int count ) : IFormLinksWithData<ContainerItemsAction, IContainerEntryGetter>, IFormLinksWithData<ContainerItemsAction, IFormLinkContainerGetter>
    {
        [JsonProperty(PropertyName = "count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [System.ComponentModel.DefaultValue(1)]
        public int Count = count;

        [JsonProperty(PropertyName = "item", Required = Required.Always)]
        public FilterFormLinks FormKey { get; set; } = formKey;

        public static bool Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IFormLinkContainerGetter source )
        {
            if (context.Record is not IContainerGetter || source is not IContainerEntryGetter sourceRecord)
                return false;

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = sourceRecord.Item.Item.FormKey;
            containerEntry.Item.Count = sourceRecord.Item.Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IContainer p)
                p.Items?.Add(containerEntry);

            return true;
        }

        public static FormKey GetFormKey ( IFormLinkContainerGetter from ) => from is IContainerEntryGetter record ? record.Item.Item.FormKey : throw new ArgumentNullException(nameof(from));

        public static List<ContainerItemsAction>? GetValueAs ( GSPRule rule, GSPRule.ValueKey key ) => rule.GetValueAs<List<ContainerItemsAction>>(key);

        public static bool Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IFormLinkContainerGetter remove )
        {
            if (!((patch == null || (patch is IContainer)) && remove is IContainerEntryGetter entry))
                return false;

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = entry.Item.Item.FormKey;
            containerEntry.Item.Count = entry.Item.Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is not IContainer p || (!p.Items?.Remove(containerEntry) ?? false))
                LogHelper.Log(Microsoft.Extensions.Logging.LogLevel.Error, context, $"Failed to remove item [{entry.Item.Item.FormKey}] from container.");

            return true;
        }

        public static bool Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IEnumerable<IFormLinkContainerGetter> newList )
        {
            if (context.Record is not IContainerGetter record || newList is not IReadOnlyList<IContainerEntryGetter> list)
                return false;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IContainer p)
            {
                _ = p.Items?.RemoveAll(_ => true);

                foreach (var add in list)
                    _ = Add(context, ref patch, add);
            }

            return true;
        }

        public bool Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch )
        {
            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = FormKey.FormKey;
            containerEntry.Item.Count = Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IContainer p)
                p.Items?.Add(containerEntry);

            return true;
        }

        public bool DataEquals ( IFormLinkContainerGetter other ) => other is IContainerEntryGetter otherContainer && otherContainer.Item.Item.FormKey.Equals(FormKey.FormKey) && otherContainer.Item.Count == Count;

        public IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list ) => list?.SingleOrDefault(s => (s != null) && GetFormKey(s).Equals(FormKey.FormKey), null);

        IContainerEntryGetter? IFormLinksWithData<ContainerItemsAction, IContainerEntryGetter>.Find ( IEnumerable<IFormLinkContainerGetter>? list ) => (IContainerEntryGetter?)list?.SingleOrDefault(s => (s != null) && GetFormKey(s).Equals(FormKey.FormKey), null);
    }
}