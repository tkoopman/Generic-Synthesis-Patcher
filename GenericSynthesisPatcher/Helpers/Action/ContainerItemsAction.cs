using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Json.Data.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class ContainerItemsAction : FormLinksWithData<ContainerItemsData, IItemGetter, ContainerEntry>
    {
        public static readonly ContainerItemsAction Instance = new();
        private const int ClassLogCode = 0x16;

        public override int Add (ProcessingKeys proKeys, ContainerItemsData data)
        {
            if (!Mod.GetPropertyForEditing<ExtendedList<ContainerEntry>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var items))
                return -1;

            items.Add(data.ToActionData());
            return 1;
        }

        public override bool CanMerge () => true;

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IContainerEntryGetter l
            && right is IContainerEntryGetter r
            && l.Item.Item.FormKey.Equals(r.Item.Item.FormKey)
            && l.Item.Count == r.Item.Count;

        public override IFormLinkContainerGetter? FindRecord (IEnumerable<IFormLinkContainerGetter>? list, FormKey key) => list?.FirstOrDefault(s => s != null && GetFormKeyFromRecord(s).Equals(key), null);

        public override int Forward (ProcessingKeys proKeys, IFormLinkContainerGetter source)
        {
            if (source is not IContainerEntryGetter sourceRecord)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to add item. No Items?", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = sourceRecord.Item.Item.FormKey;
            containerEntry.Item.Count = sourceRecord.Item.Count;

            if (!Mod.GetPropertyForEditing<ExtendedList<ContainerEntry>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var items))
                return -1;

            items.Add(containerEntry);

            Global.TraceLogger?.Log(ClassLogCode, $"Added item {containerEntry.Item.Item.FormKey} ({containerEntry.Item.Count})");

            return 1;
        }

        public override List<ContainerItemsData>? GetFillValueAs (ProcessingKeys proKeys) => proKeys.Rule.GetFillValueAs<List<ContainerItemsData>>(proKeys.RuleKey);

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from) => from is IContainerEntryGetter record ? record.Item.Item.FormKey : throw new ArgumentNullException(nameof(from));

        public override int Merge (ProcessingKeys proKeys)
        {
            Global.UpdateLoggers(ClassLogCode);

            var root = RecordGraph<IContainerEntryGetter>.Create(
                proKeys.Record.FormKey,
                proKeys.Type.StaticRegistration.GetterType,
                proKeys.Rule.Merge[proKeys.RuleKey],
                list => Mod.GetProperty<IReadOnlyList<IContainerEntryGetter>>(list.Record, proKeys.Property.PropertyName, out var value) ? value : null,
                item => $"{item.Item.Item.FormKey} ({item.Item.Count})");

            if (root == null)
            {
                Global.Logger.Log(ClassLogCode, "Failed to generate graph for merge", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            return root.Merge(out var newList) ? Replace(proKeys, newList) : 0;
        }

        public override int Remove (ProcessingKeys proKeys, IFormLinkContainerGetter remove)
        {
            if (remove is not IContainerEntryGetter sourceRecord)
                return -1;

            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = sourceRecord.Item.Item.FormKey;
            containerEntry.Item.Count = sourceRecord.Item.Count;

            if (!Mod.GetPropertyForEditing<ExtendedList<ContainerEntry>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var items))
                return -1;

            if (items.Remove(containerEntry))
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Removed item {containerEntry.Item.Item.FormKey} ({containerEntry.Item.Count})");
                return 1;
            }

            Global.Logger.Log(ClassLogCode, $"Failed to remove item [{sourceRecord.Item.Item.FormKey}].", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
            return 0;
        }

        public override int Replace (ProcessingKeys proKeys, IEnumerable<IFormLinkContainerGetter>? _newList)
        {
            if (_newList is not IReadOnlyList<IContainerEntryGetter> newList || !Mod.GetProperty<IReadOnlyList<IContainerEntryGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
            {
                Global.Logger.Log(ClassLogCode, "Failed to replace items", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            if (!Mod.GetPropertyForEditing<ExtendedList<ContainerEntry>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out _))
                return -1;

            foreach (var d in del)
                _ = Remove(proKeys, d);

            foreach (var a in add)
                _ = Forward(proKeys, a);

            return add.Count() + del.Count();
        }
    }
}