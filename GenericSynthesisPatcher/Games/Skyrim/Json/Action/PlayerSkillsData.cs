using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class PlayerSkillsData
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
            => other is not null
            && FarAwayModelDistance == other.FarAwayModelDistance
            && GearedUpWeapons == other.GearedUpWeapons
            && object.Equals(Stats, other.Stats)
            && object.Equals(SkillOffsets, other.SkillOffsets)
            && object.Equals(SkillValues, other.SkillValues);

        public override bool Equals (object? obj) => Equals(obj as PlayerSkillsData);

        public override int GetHashCode () => HashCode.Combine(FarAwayModelDistance, GearedUpWeapons, Stats.GetHashCode());

        public bool NonNullEquals (IPlayerSkillsGetter? other)
            => other is not null
            && (FarAwayModelDistance is null || FarAwayModelDistance == other.FarAwayModelDistance)
            && (GearedUpWeapons is null || GearedUpWeapons == other.GearedUpWeapons)
            && Stats.NonNullEquals(other)
            && PlayerSkillsSkillsData.NonNullEquals(SkillOffsets, other.SkillOffsets)
            && PlayerSkillsSkillsData.NonNullEquals(SkillValues, other.SkillValues);

        public int UpdateRecord (IPlayerSkills update)
        {
            int count = 0;

            if (FarAwayModelDistance is not null && FarAwayModelDistance != update.FarAwayModelDistance)
            {
                count++;
                update.FarAwayModelDistance = (float)FarAwayModelDistance;
            }

            if (GearedUpWeapons is not null && GearedUpWeapons != update.GearedUpWeapons)
            {
                count++;
                update.GearedUpWeapons = (byte)GearedUpWeapons;
            }

            count += Stats.Update(update);
            if (SkillOffsets is not null)
                count += SkillOffsets.Update(update.SkillOffsets);
            if (SkillValues is not null)
                count += SkillValues.Update(update.SkillValues);

            return count;
        }
    }
}