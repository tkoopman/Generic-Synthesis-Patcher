using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class PlayerSkillsData : IEquatable<PlayerSkillsData>, IEquatable<IPlayerSkillsGetter>
    {
        [JsonProperty(PropertyName = "FarAwayModelDistance")]
        public float? FarAwayModelDistance { get; set; }

        [JsonProperty(PropertyName = "GearedUpWeapons")]
        public byte? GearedUpWeapons { get; set; }

        [JsonProperty(PropertyName = "Health")]
        public ushort? Health { get => Stats.Health; set => Stats.Health = value; }

        [JsonProperty(PropertyName = "Magicka")]
        public ushort? Magicka { get => Stats.Magicka; set => Stats.Magicka = value; }

        [JsonProperty(PropertyName = "SkillOffsets")]
        public PlayerSkillsSkillsData? SkillOffsets { get; set; } = null;

        [JsonProperty(PropertyName = "SkillValues")]
        public PlayerSkillsSkillsData? SkillValues { get; set; } = null;

        [JsonProperty(PropertyName = "Stamina")]
        public ushort? Stamina { get => Stats.Stamina; set => Stats.Stamina = value; }

        public PlayerSkillsStatsData Stats { get; set; } = new PlayerSkillsStatsData();

        public bool Equals (PlayerSkillsData? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;

            return
                FarAwayModelDistance == other.FarAwayModelDistance &&
                GearedUpWeapons == other.GearedUpWeapons &&
                PlayerSkillsStatsData.Equals(Stats, other.Stats) &&
                PlayerSkillsSkillsData.Equals(SkillOffsets, other.SkillOffsets) &&
                PlayerSkillsSkillsData.Equals(SkillValues, other.SkillValues);
        }

        public bool Equals (IPlayerSkillsGetter? other)
        {
            return
                other != null &&
                (FarAwayModelDistance == null || FarAwayModelDistance == other.FarAwayModelDistance) &&
                (GearedUpWeapons == null || GearedUpWeapons == other.GearedUpWeapons) &&
                Stats.Equals(other) &&
                (SkillOffsets == null || SkillOffsets.Equals(other.SkillOffsets)) &&
                (SkillValues == null || SkillValues.Equals(other.SkillValues));
        }

        public int Update (IPlayerSkills update)
        {
            int count = 0;

            if (FarAwayModelDistance != null && FarAwayModelDistance != update.FarAwayModelDistance)
            {
                count++;
                update.FarAwayModelDistance = (float)FarAwayModelDistance;
            }
            if (GearedUpWeapons != null && GearedUpWeapons != update.GearedUpWeapons)
            {
                count++;
                update.GearedUpWeapons = (byte)GearedUpWeapons;
            }

            count += Stats.Update(update);
            if (SkillOffsets != null)
                count += SkillOffsets.Update(update.SkillOffsets);
            if (SkillValues != null)
                count += SkillValues.Update(update.SkillValues);

            return count;
        }
    }
}