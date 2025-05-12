using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class LeveledNpcData (FormKeyListOperation<INpcSpawnGetter> formKey, short count, short level) : LeveledEntryData<INpcSpawnGetter, LeveledNpcEntry>(count, level)
    {
        [JsonProperty(PropertyName = "NPC", Required = Required.Always)]
        public override FormKeyListOperation<INpcSpawnGetter> FormKey { get; } = formKey;

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is ILeveledNpcEntryGetter entry
            && entry.Data is not null
            && FormKey.ValueEquals(entry.Data.Reference.FormKey)
            && Count == entry.Data?.Count
            && Level == entry.Data?.Level
            && ItemCondition == entry.ExtraData?.ItemCondition
            && object.Equals(Owner?.ToActionData(), entry.ExtraData?.Owner);

        public bool Equals (LeveledNpcData? other)
            => other is not null
            && Count == other.Count
            && Level == other.Level
            && ItemCondition == other.ItemCondition
            && FormKey.Equals(other.FormKey);

        public override bool Equals (object? obj) => Equals(obj as LeveledNpcData);

        public override int GetHashCode () => HashCode.Combine(Count, Level, ItemCondition);

        public override LeveledNpcEntry ToActionData ()
        {
            var entry = new LeveledNpcEntry
            {
                Data = new LeveledNpcEntryData
                {
                    Level = Level,
                    Reference = FormKey.Value.ToLink<INpcSpawn>(),
                    Count = Count,
                },

                ExtraData = createExtraData()
            };

            return entry;
        }
    }
}