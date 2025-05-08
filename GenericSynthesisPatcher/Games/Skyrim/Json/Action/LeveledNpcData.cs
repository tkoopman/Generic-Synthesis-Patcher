using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class LeveledNpcData (FormKeyListOperation<INpcSpawnGetter> formKey, short count, short level) : LeveledEntryData<INpcSpawnGetter, LeveledNpcEntry>(count, level)
    {
        [JsonProperty(PropertyName = "NPC", Required = Required.Always)]
        public override FormKeyListOperation<INpcSpawnGetter> FormKey { get; } = formKey ?? new(null);

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is ILeveledNpcEntryGetter entry
            && entry.Data != null
            && FormKey.ValueEquals(entry.Data.Reference.FormKey)
            && Count == entry.Data?.Count
            && Level == entry.Data?.Level
            && ItemCondition == entry.ExtraData?.ItemCondition
            && (Owner == null && entry.ExtraData?.Owner == null
             || Owner != null && Owner.ToActionData().Equals(entry.ExtraData?.Owner));

        public override LeveledNpcEntry ToActionData ()
        {
            var entry = new LeveledNpcEntry
            {
                Data = new LeveledNpcEntryData
                {
                    Level = Level,
                    Reference = FormKey.Value.ToLink<INpcSpawn>(),
                    Count = Count
                },

                ExtraData = createExtraData()
            };

            return entry;
        }
    }
}