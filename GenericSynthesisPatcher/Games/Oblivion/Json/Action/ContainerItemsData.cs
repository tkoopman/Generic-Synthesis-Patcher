using System.ComponentModel;

using GenericSynthesisPatcher.Games.Universal.Json.Action;
using GenericSynthesisPatcher.Rules.Operations;

using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Oblivion.Json.Action
{
    /// <summary>
    ///     JSON Data class used by ContainerItemsAction class
    /// </summary>
    /// <param name="formKey">FormKey of item</param>
    /// <param name="count">QTY</param>
    public class ContainerItemsData (FormKeyListOperation<IItemGetter> formKey, uint count) : FormLinksWithDataActionDataBase<IItemGetter, ContainerItem>
    {
        [JsonProperty(PropertyName = "Count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public uint Count { get; set; } = count;

        [JsonProperty(PropertyName = "Item", Required = Required.Always)]
        public override FormKeyListOperation<IItemGetter> FormKey { get; } = formKey;

        public bool Equals (ContainerItemsData? other)
            => other is not null
            && FormKey.Equals(other.FormKey)
            && Count == other.Count;

        public override bool Equals (object? obj) => Equals(obj as ContainerItemsData);

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is IContainerItemGetter otherContainer
            && FormKey.ValueEquals(otherContainer.Item.FormKey)
            && otherContainer.Count == Count;

        public override int GetHashCode () => FormKey.GetHashCode() ^ Count.GetHashCode();

        public override ContainerItem ToActionData ()
        {
            var containerEntry = new ContainerItem();
            containerEntry.Item.FormKey = FormKey.Value;
            containerEntry.Count = Count;

            return containerEntry;
        }

        public override string? ToString () => $"{Count}x{FormKey.Value}";
    }
}