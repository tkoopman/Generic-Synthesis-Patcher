using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class LeveledNpcData (FormKeyListOperation<INpcSpawnGetter> formKey, short count, short level) : LeveledEntryData<INpcSpawnGetter, LeveledNpcEntry>(count, level)
    {
        [JsonProperty(PropertyName = "NPC", Required = Required.Always)]
        public override FormKeyListOperation<INpcSpawnGetter> FormKey { get; } = formKey ?? new(null);

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

                ExtraData = CreateExtraData()
            };

            return entry;
        }
    }
}