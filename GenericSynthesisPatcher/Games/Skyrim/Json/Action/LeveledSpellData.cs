using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class LeveledSpellData (FormKeyListOperation<ISpellRecordGetter> formKey, short count, short level) : LeveledEntryData<ISpellRecordGetter, LeveledSpellEntry>(count, level)
    {
        [JsonProperty(PropertyName = "Spell", Required = Required.Always)]
        public override FormKeyListOperation<ISpellRecordGetter> FormKey { get; } = formKey;

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is ILeveledSpellEntryGetter entry
            && entry.Data is not null
            && FormKey.ValueEquals(entry.Data.Reference.FormKey)
            && Count == entry.Data?.Count
            && Level == entry.Data?.Level
            && ItemCondition == entry.ExtraData?.ItemCondition
            && object.Equals(Owner?.ToActionData(), entry.ExtraData?.Owner);

        public bool Equals (LeveledSpellData? other)
            => other is not null
            && Count == other.Count
            && Level == other.Level
            && ItemCondition == other.ItemCondition
            && FormKey.Equals(other.FormKey)
            && object.Equals(Owner, other.Owner);

        public override bool Equals (object? obj) => Equals(obj as LeveledSpellData);

        public override int GetHashCode () => HashCode.Combine(Count, Level, ItemCondition);

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