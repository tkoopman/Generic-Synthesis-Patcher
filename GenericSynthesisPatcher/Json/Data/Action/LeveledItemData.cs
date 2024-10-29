using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class LeveledItemData (FormKeyListOperation<IItemGetter> formKey, short count, short level) : LeveledEntryData<IItemGetter, LeveledItemEntry>(count, level)
    {
        [JsonProperty(PropertyName = "Item", Required = Required.Always)]
        public override FormKeyListOperation<IItemGetter> FormKey { get; } = formKey ?? new(null);

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

                ExtraData = CreateExtraData()
            };

            return entry;
        }
    }
}