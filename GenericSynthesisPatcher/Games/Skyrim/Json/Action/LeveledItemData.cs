using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class LeveledItemData (FormKeyListOperation<IItemGetter> formKey, short count, short level) : LeveledEntryData<IItemGetter, LeveledItemEntry>(count, level)
    {
        [JsonProperty(PropertyName = "Item", Required = Required.Always)]
        public override FormKeyListOperation<IItemGetter> FormKey { get; } = formKey ?? new(null);

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is ILeveledItemEntryGetter entry
            && entry.Data != null
            && FormKey.ValueEquals(entry.Data.Reference.FormKey)
            && Count == entry.Data?.Count
            && Level == entry.Data?.Level
            && ItemCondition == entry.ExtraData?.ItemCondition
            && (Owner == null && entry.ExtraData?.Owner == null
             || Owner != null && Owner.ToActionData().Equals(entry.ExtraData?.Owner));

        public override LeveledItemEntry ToActionData ()
        {
            var entry = new LeveledItemEntry
            {
                Data = new LeveledItemEntryData
                {
                    Level = Level,
                    Reference = FormKey.Value.ToLink<IItemGetter>(),
                    Count = Count
                },

                ExtraData = createExtraData()
            };

            return entry;
        }
    }
}