using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class PlayerSkillsStatsData
    {
        [JsonProperty(PropertyName = "Health")]
        public ushort? Health { get; set; } = null;

        [JsonProperty(PropertyName = "Magicka")]
        public ushort? Magicka { get; set; } = null;

        [JsonProperty(PropertyName = "Stamina")]
        public ushort? Stamina { get; set; } = null;

        public bool Equals (PlayerSkillsStatsData? other)
            => other is not null &&
            Health == other.Health &&
            Stamina == other.Stamina &&
            Magicka == other.Magicka;

        public override bool Equals (object? obj) => Equals(obj as PlayerSkillsStatsData);

        public override int GetHashCode () => HashCode.Combine(Health, Magicka, Stamina);

        public bool NonNullEquals (IPlayerSkillsGetter? other)
            => other is not null
            && (Health is null || Health == other.Health)
            && (Stamina is null || Stamina == other.Stamina)
            && (Magicka is null || Magicka == other.Magicka);

        public int Update (IPlayerSkills update)
        {
            int count = 0;

            if (Health is not null && Health != update.Health)
            {
                count++;
                update.Health = (ushort)Health;
            }

            if (Stamina is not null && Stamina != update.Stamina)
            {
                count++;
                update.Stamina = (ushort)Stamina;
            }

            if (Magicka is not null && Magicka != update.Magicka)
            {
                count++;
                update.Magicka = (ushort)Magicka;
            }

            return count;
        }
    }
}