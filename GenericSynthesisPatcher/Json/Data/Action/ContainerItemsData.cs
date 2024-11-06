using System.ComponentModel;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class ContainerItemsData (FormKeyListOperation<IItemGetter> formKey, int count) : ActionDataBase<IItemGetter, ContainerEntry>, IEquatable<ContainerItemsData>
    {
        [JsonProperty(PropertyName = "Count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public int Count = count;

        [JsonProperty(PropertyName = "Item", Required = Required.Always)]
        public override FormKeyListOperation<IItemGetter> FormKey { get; } = formKey ?? new(null);

        public static bool Equals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IContainerEntryGetter l
            && right is IContainerEntryGetter r
            && l.Item.Item.FormKey.Equals(r.Item.Item.FormKey)
            && l.Item.Count == r.Item.Count;

        public bool Equals (ContainerItemsData? other) => FormKey.Equals(other?.FormKey) && Count.Equals(other?.Count);

        public override bool Equals (object? obj) => Equals(obj as ContainerItemsData);

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is IContainerEntryGetter otherContainer
            && FormKey.ValueEquals(otherContainer.Item.Item.FormKey)
            && otherContainer.Item.Count == Count;

        public override int GetHashCode () => FormKey.GetHashCode() ^ Count;

        public override ContainerEntry ToActionData ()
        {
            var containerEntry = new ContainerEntry();
            containerEntry.Item.Item.FormKey = FormKey.Value;
            containerEntry.Item.Count = Count;

            return containerEntry;
        }

        public override string? ToString () => $"{Count}x{FormKey.Value}";
    }
}