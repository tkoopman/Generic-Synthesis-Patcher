using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class LeveledSpellData (FormKeyListOperation<ISpellRecordGetter> formKey, short count, short level) : LeveledEntryData<ISpellRecordGetter, LeveledSpellEntry>(count, level)
    {
        [JsonProperty(PropertyName = "Spell", Required = Required.Always)]
        public override FormKeyListOperation<ISpellRecordGetter> FormKey { get; } = formKey ?? new(null);

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is ILeveledSpellEntryGetter entry
            && entry.Data != null
            && FormKey.ValueEquals(entry.Data.Reference.FormKey)
            && Count == entry.Data?.Count
            && Level == entry.Data?.Level
            && ItemCondition == entry.ExtraData?.ItemCondition
            && (Owner == null && entry.ExtraData?.Owner == null
             || Owner != null && Owner.ToActionData().Equals(entry.ExtraData?.Owner));

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

                ExtraData = createExtraData()
            };

            return entry;
        }
    }
}