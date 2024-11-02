using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class LeveledSpellData (FormKeyListOperation<ISpellRecordGetter> formKey, short count, short level) : LeveledEntryData<ISpellRecordGetter, LeveledSpellEntry>(count, level)
    {
        [JsonProperty(PropertyName = "Spell", Required = Required.Always)]
        public override FormKeyListOperation<ISpellRecordGetter> FormKey { get; } = formKey ?? new(null);

        public override LeveledSpellEntry ToActionData ()
        {
            var entry = new LeveledSpellEntry
            {
                Data = new LeveledSpellEntryData
                {
                    Level = Level,
                    Reference = FormKey.Value.ToLink<ISpellRecordGetter>(),
                    Count = Count
                },

                ExtraData = CreateExtraData()
            };

            return entry;
        }
    }
}