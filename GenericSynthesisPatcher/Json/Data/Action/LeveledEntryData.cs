using System.ComponentModel;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public abstract class LeveledEntryData<TMajor, TData> (short count, short level) : FormLinksWithDataActionDataBase<TMajor, TData>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        [JsonProperty(PropertyName = "Count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public short Count { get; set; } = count;

        [JsonProperty(PropertyName = "ItemCondition")]
        public float ItemCondition { get; set; } = -1;

        [JsonProperty(PropertyName = "Level", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public short Level { get; } = level;

        [JsonProperty(PropertyName = "Owner")]
        public OwnerBase? Owner { get; set; }

        public abstract override bool Equals (IFormLinkContainerGetter? other);

        public override int GetHashCode () => FormKey.GetHashCode() ^ Count;

        public override string? ToString () => $"{Count}x{FormKey.Value}";

        protected ExtraData? createExtraData ()
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