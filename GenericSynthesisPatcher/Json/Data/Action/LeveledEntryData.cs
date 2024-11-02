using System.ComponentModel;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public abstract class LeveledEntryData<TMajor, TData> (short count, short level) : ActionDataBase<TMajor, TData>, IEquatable<ContainerItemsData>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        [JsonProperty(PropertyName = "Count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public short Count = count;

        [JsonProperty(PropertyName = "ItemCondition")]
        public float ItemCondition { get; set; } = -1;

        [JsonProperty(PropertyName = "Level")]
        public short Level { get; } = level;

        [JsonProperty(PropertyName = "Owner")]
        public OwnerBase? Owner { get; set; }

        public static bool Equals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IContainerEntryGetter l
            && right is IContainerEntryGetter r
            && l.Item.Item.FormKey.Equals(r.Item.Item.FormKey)
            && l.Item.Count == r.Item.Count;

        public bool Equals (ContainerItemsData? other) => Equals(this, other);

        public override bool Equals (object? obj) => Equals(obj as ContainerItemsData);

        public override bool Equals (IFormLinkContainerGetter? other) => other is IContainerEntryGetter otherContainer && otherContainer.Item.Item.FormKey.Equals(FormKey.Value) && otherContainer.Item.Count == Count;

        public override int GetHashCode () => FormKey.GetHashCode() ^ Count;

        public override string? ToString () => $"{Count}x{FormKey.Value}";

        protected ExtraData? CreateExtraData ()
        {
            if (Owner == null && ItemCondition == -1)
            {
                Global.TraceLogger?.Log(0xFF, $"No extra data to add");
                return null;
            }

            var extraData = new ExtraData
            {
                ItemCondition = ItemCondition,
                Owner = Owner?.ToActionData() ?? new NoOwner(),
            };

            return extraData;
        }
    }
}