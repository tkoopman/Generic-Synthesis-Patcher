using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public class PlayerSkillsSkillsData
    {
        [JsonProperty(PropertyName = "Alchemy")]
        public byte? Alchemy { get; set; } = null;

        [JsonProperty(PropertyName = "Alteration")]
        public byte? Alteration { get; set; } = null;

        [JsonProperty(PropertyName = "Archery")]
        public byte? Archery { get; set; } = null;

        [JsonProperty(PropertyName = "Block")]
        public byte? Block { get; set; } = null;

        [JsonProperty(PropertyName = "Conjuration")]
        public byte? Conjuration { get; set; } = null;

        [JsonProperty(PropertyName = "Destruction")]
        public byte? Destruction { get; set; } = null;

        [JsonProperty(PropertyName = "Enchanting")]
        public byte? Enchanting { get; set; } = null;

        [JsonProperty(PropertyName = "HeavyArmor")]
        public byte? HeavyArmor { get; set; } = null;

        [JsonProperty(PropertyName = "Illusion")]
        public byte? Illusion { get; set; } = null;

        [JsonProperty(PropertyName = "LightArmor")]
        public byte? LightArmor { get; set; } = null;

        [JsonProperty(PropertyName = "Lockpicking")]
        public byte? Lockpicking { get; set; } = null;

        [JsonProperty(PropertyName = "OneHanded")]
        public byte? OneHanded { get; set; } = null;

        [JsonProperty(PropertyName = "Pickpocket")]
        public byte? Pickpocket { get; set; } = null;

        [JsonProperty(PropertyName = "Restoration")]
        public byte? Restoration { get; set; } = null;

        [JsonProperty(PropertyName = "Smithing")]
        public byte? Smithing { get; set; } = null;

        [JsonProperty(PropertyName = "Sneak")]
        public byte? Sneak { get; set; } = null;

        [JsonProperty(PropertyName = "Speech")]
        public byte? Speech { get; set; } = null;

        [JsonProperty(PropertyName = "TwoHanded")]
        public byte? TwoHanded { get; set; } = null;

        /// <summary>
        ///     Compares to PlayerSkillsData Values and Offsets however if a value is null here then
        ///     ignores checking PlayerSkillsData Values and Offsets. If all values are null then
        ///     will match all.
        /// </summary>
        /// <returns>
        ///     Returns true if any defined player skill matches PlayerSkillsData Values and Offsets
        /// </returns>
        public static bool NonNullEquals (PlayerSkillsSkillsData? lhs, IReadOnlyDictionary<Skill, byte>? rhs)
            => lhs is null || (rhs is not null
            && (lhs.OneHanded is null || lhs.OneHanded == rhs[Skill.OneHanded])
            && (lhs.TwoHanded is null || lhs.TwoHanded == rhs[Skill.TwoHanded])
            && (lhs.Archery is null || lhs.Archery == rhs[Skill.Archery])
            && (lhs.Block is null || lhs.Block == rhs[Skill.Block])
            && (lhs.Smithing is null || lhs.Smithing == rhs[Skill.Smithing])
            && (lhs.HeavyArmor is null || lhs.HeavyArmor == rhs[Skill.HeavyArmor])
            && (lhs.LightArmor is null || lhs.LightArmor == rhs[Skill.LightArmor])
            && (lhs.Pickpocket is null || lhs.Pickpocket == rhs[Skill.Pickpocket])
            && (lhs.Lockpicking is null || lhs.Lockpicking == rhs[Skill.Lockpicking])
            && (lhs.Sneak is null || lhs.Sneak == rhs[Skill.Sneak])
            && (lhs.Alchemy is null || lhs.Alchemy == rhs[Skill.Alchemy])
            && (lhs.Speech is null || lhs.Speech == rhs[Skill.Speech])
            && (lhs.Alteration is null || lhs.Alteration == rhs[Skill.Alteration])
            && (lhs.Conjuration is null || lhs.Conjuration == rhs[Skill.Conjuration])
            && (lhs.Destruction is null || lhs.Destruction == rhs[Skill.Destruction])
            && (lhs.Illusion is null || lhs.Illusion == rhs[Skill.Illusion])
            && (lhs.Restoration is null || lhs.Restoration == rhs[Skill.Restoration])
            && (lhs.Enchanting is null || lhs.Enchanting == rhs[Skill.Enchanting]));

        public bool Equals (PlayerSkillsSkillsData? other)
            => other is not null
            && OneHanded == other.OneHanded
            && TwoHanded == other.TwoHanded
            && Archery == other.Archery
            && Block == other.Block
            && Smithing == other.Smithing
            && HeavyArmor == other.HeavyArmor
            && LightArmor == other.LightArmor
            && Pickpocket == other.Pickpocket
            && Lockpicking == other.Lockpicking
            && Sneak == other.Sneak
            && Alchemy == other.Alchemy
            && Speech == other.Speech
            && Alteration == other.Alteration
            && Conjuration == other.Conjuration
            && Destruction == other.Destruction
            && Illusion == other.Illusion
            && Restoration == other.Restoration
            && Enchanting == other.Enchanting;

        public override bool Equals (object? obj) => Equals(obj as PlayerSkillsSkillsData);

        public override int GetHashCode ()
        {
            var hash = new HashCode();
            hash.Add(Alchemy);
            hash.Add(Alteration);
            hash.Add(Archery);
            hash.Add(Block);
            hash.Add(Conjuration);
            hash.Add(Destruction);
            hash.Add(Enchanting);
            hash.Add(HeavyArmor);
            hash.Add(Illusion);
            hash.Add(LightArmor);
            hash.Add(Lockpicking);
            hash.Add(OneHanded);
            hash.Add(Pickpocket);
            hash.Add(Restoration);
            hash.Add(Smithing);
            hash.Add(Sneak);
            hash.Add(Speech);
            hash.Add(TwoHanded);
            return hash.ToHashCode();
        }

        public int Update (IDictionary<Skill, byte> update)
        {
            int count = 0;

            if (OneHanded is not null && OneHanded != update[Skill.OneHanded])
            {
                count++;
                update[Skill.OneHanded] = (byte)OneHanded;
            }

            if (TwoHanded is not null && TwoHanded != update[Skill.TwoHanded])
            {
                count++;
                update[Skill.TwoHanded] = (byte)TwoHanded;
            }

            if (Archery is not null && Archery != update[Skill.Archery])
            {
                count++;
                update[Skill.Archery] = (byte)Archery;
            }

            if (Block is not null && Block != update[Skill.Block])
            {
                count++;
                update[Skill.Block] = (byte)Block;
            }

            if (Smithing is not null && Smithing != update[Skill.Smithing])
            {
                count++;
                update[Skill.Smithing] = (byte)Smithing;
            }

            if (HeavyArmor is not null && HeavyArmor != update[Skill.HeavyArmor])
            {
                count++;
                update[Skill.HeavyArmor] = (byte)HeavyArmor;
            }

            if (LightArmor is not null && LightArmor != update[Skill.LightArmor])
            {
                count++;
                update[Skill.LightArmor] = (byte)LightArmor;
            }

            if (Pickpocket is not null && Pickpocket != update[Skill.Pickpocket])
            {
                count++;
                update[Skill.Pickpocket] = (byte)Pickpocket;
            }

            if (Lockpicking is not null && Lockpicking != update[Skill.Lockpicking])
            {
                count++;
                update[Skill.Lockpicking] = (byte)Lockpicking;
            }

            if (Sneak is not null && Sneak != update[Skill.Sneak])
            {
                count++;
                update[Skill.Sneak] = (byte)Sneak;
            }

            if (Alchemy is not null && Alchemy != update[Skill.Alchemy])
            {
                count++;
                update[Skill.Alchemy] = (byte)Alchemy;
            }

            if (Speech is not null && Speech != update[Skill.Speech])
            {
                count++;
                update[Skill.Speech] = (byte)Speech;
            }

            if (Alteration is not null && Alteration != update[Skill.Alteration])
            {
                count++;
                update[Skill.Alteration] = (byte)Alteration;
            }

            if (Conjuration is not null && Conjuration != update[Skill.Conjuration])
            {
                count++;
                update[Skill.Conjuration] = (byte)Conjuration;
            }

            if (Destruction is not null && Destruction != update[Skill.Destruction])
            {
                count++;
                update[Skill.Destruction] = (byte)Destruction;
            }

            if (Illusion is not null && Illusion != update[Skill.Illusion])
            {
                count++;
                update[Skill.Illusion] = (byte)Illusion;
            }

            if (Restoration is not null && Restoration != update[Skill.Restoration])
            {
                count++;
                update[Skill.Restoration] = (byte)Restoration;
            }

            if (Enchanting is not null && Enchanting != update[Skill.Enchanting])
            {
                count++;
                update[Skill.Enchanting] = (byte)Enchanting;
            }

            return count;
        }
    }
}