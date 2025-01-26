using System.ComponentModel;

using Loqui;

using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class PlayerSkillsStatsData : IEquatable<PlayerSkillsStatsData>, IEquatable<IPlayerSkillsGetter>
    {
        [JsonProperty(PropertyName = "Health")]
        public ushort? Health { get; set; } = null;

        [JsonProperty(PropertyName = "Magicka")]
        public ushort? Magicka { get; set; } = null;

        [JsonProperty(PropertyName = "Stamina")]
        public ushort? Stamina { get; set; } = null;

        public static bool Equals (PlayerSkillsStatsData? left, PlayerSkillsStatsData? right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left == null || right == null)
                return false;

            return
                left.Health == right.Health &&
                left.Stamina == right.Stamina &&
                left.Magicka == right.Magicka;
        }

        public bool Equals (PlayerSkillsStatsData? other) => Equals(this, other);
        public bool Equals (IPlayerSkillsGetter? other)
        {
            return
                other != null &&
                (Health == null || Health == other.Health) &&
                (Stamina == null || Stamina == other.Stamina) &&
                (Magicka == null || Magicka == other.Magicka);
        }

        public int Update (IPlayerSkills update)
        {
            int count = 0;

            if (Health != null && Health != update.Health)
            {
                count++;
                update.Health = (ushort)Health;
            }
            if (Stamina != null && Stamina != update.Stamina)
            {
                count++;
                update.Stamina = (ushort)Stamina;
            }
            if (Magicka != null && Magicka != update.Magicka)
            {
                count++;
                update.Magicka = (ushort)Magicka;
            }

            return count;
        }
    }
}