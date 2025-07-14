using GenericSynthesisPatcher.Rules.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class LeveledItemData (FormKeyListOperation<IItemGetter> formKey, short count, short level) : LeveledEntryData<IItemGetter, LeveledItemEntry>(count, level)
    {
        [JsonProperty(PropertyName = "Item", Required = Required.Always)]
        public override FormKeyListOperation<IItemGetter> FormKey { get; } = formKey;

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is ILeveledItemEntryGetter entry
            && entry.Data is not null
            && FormKey.ValueEquals(entry.Data.Reference.FormKey)
            && Count == entry.Data?.Count
            && Level == entry.Data?.Level
            && ItemCondition == entry.ExtraData?.ItemCondition
            && object.Equals(Owner?.ToActionData(), entry.ExtraData?.Owner);

        public override bool Equals (object? obj) => Equals(obj as LeveledItemData);

        public bool Equals (LeveledItemData? other)
            => other is not null
            && FormKey.Equals(FormKey)
            && Count == other.Count
            && Level == other.Level
            && ItemCondition == other.ItemCondition;

        public override int GetHashCode () => HashCode.Combine(Count, Level, ItemCondition);

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