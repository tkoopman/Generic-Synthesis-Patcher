using System.ComponentModel;

using DynamicData;

using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class ContainerItemsAction ( FormKeyListOperation<IItemGetter> formKey, int count ) : IFormLinksWithData<ContainerItemsAction, IItemGetter>
    {
        [JsonProperty(PropertyName = "Count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public int Count = count;

        private const int ClassLogCode = 0x16;

        [JsonProperty(PropertyName = "Item", Required = Required.Always)]
        public FormKeyListOperation<IItemGetter> FormKey { get; set; } = formKey;

        FormKeyListOperation IFormLinksWithData<ContainerItemsAction>.FormKey => FormKey;

        public static int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter source )
        {
            if (source is not IContainerEntryGetter sourceRecord)
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, $"Failed to add item. No Items?", context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = sourceRecord.Item.Item.FormKey;
            containerEntry.Item.Count = sourceRecord.Item.Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.GetPropertyForEditing<ExtendedList<ContainerEntry>>(patch, rcd.PropertyName, out var items))
                return -1;

            items.Add(containerEntry);

            Global.TraceLogger?.Log(ClassLogCode, $"Added item {containerEntry.Item.Item.FormKey} ({containerEntry.Item.Count})");

            return 1;
        }

        public static bool CanMerge () => true;

        public static bool DataEquals ( IFormLinkContainerGetter left, IFormLinkContainerGetter right )
            => left is IContainerEntryGetter l
            && right is IContainerEntryGetter r
            && l.Item.Item.FormKey.Equals(r.Item.Item.FormKey)
            && l.Item.Count == r.Item.Count;

        public static IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list, FormKey key ) => list?.FirstOrDefault(s => s != null && GetFormKey(s).Equals(key), null);

        public static List<ContainerItemsAction>? GetFillValueAs ( GSPRule rule, FilterOperation key ) => rule.GetFillValueAs<List<ContainerItemsAction>>(key);

        public static FormKey GetFormKey ( IFormLinkContainerGetter from ) => from is IContainerEntryGetter record ? record.Item.Item.FormKey : throw new ArgumentNullException(nameof(from));

        public static int Merge ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord )
        {
            _ = Global.UpdateTrace(ClassLogCode);

            var root = RecordGraph<IContainerEntryGetter>.Create(
                context.Record.FormKey,
                context.Record.Registration.GetterType,
                rule.Merge[valueKey],
                list => Mod.GetProperty<IReadOnlyList<IContainerEntryGetter>>(list.Record, rcd.PropertyName, out var value) ? value : null,
                item => $"{item.Item.Item.FormKey} ({item.Item.Count})");

            if (root == null)
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, "Failed to generate graph for merge", context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            return root.Merge(out var newList) ? Replace(context, rcd, ref patchedRecord, newList) : 0;
        }

        public static int Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter remove )
        {
            if (remove is not IContainerEntryGetter sourceRecord)
                return -1;

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = sourceRecord.Item.Item.FormKey;
            containerEntry.Item.Count = sourceRecord.Item.Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.GetPropertyForEditing<ExtendedList<ContainerEntry>>(patch, rcd.PropertyName, out var items))
                return -1;

            if (items.Remove(containerEntry))
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Removed item {containerEntry.Item.Item.FormKey} ({containerEntry.Item.Count})");
                return 1;
            }

            LogHelper.Log(LogLevel.Error, ClassLogCode, $"Failed to remove item [{sourceRecord.Item.Item.FormKey}].", context: context, propertyName: rcd.PropertyName);
            return 0;
        }

        public static int Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patch, IEnumerable<IFormLinkContainerGetter>? _newList )
        {
            if (_newList is not IReadOnlyList<IContainerEntryGetter> newList || !Mod.GetProperty<IReadOnlyList<IContainerEntryGetter>>(context.Record, rcd.PropertyName, out var curList))
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, "Failed to replace items", context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.GetPropertyForEditing<ExtendedList<ContainerEntry>>(patch, rcd.PropertyName, out _))
                return -1;

            foreach (var d in del)
                _ = Remove(context, rcd, ref patch, d);

            foreach (var a in add)
                _ = Add(context, rcd, ref patch, a);

            return add.Count() + del.Count();
        }

        public int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patch )
        {
            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = FormKey.Value;
            containerEntry.Item.Count = Count;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.GetPropertyForEditing<ExtendedList<ContainerEntry>>(patch, rcd.PropertyName, out var items))
                return -1;

            items.Add(containerEntry);
            return 1;
        }

        public bool DataEquals ( IFormLinkContainerGetter other ) => other is IContainerEntryGetter otherContainer && otherContainer.Item.Item.FormKey.Equals(FormKey.Value) && otherContainer.Item.Count == Count;

        public IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list ) => Find(list, FormKey.Value);
    }
}