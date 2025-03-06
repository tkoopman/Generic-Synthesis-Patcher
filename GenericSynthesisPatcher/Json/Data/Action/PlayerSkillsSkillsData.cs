using System.Diagnostics.CodeAnalysis;

using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class PlayerSkillsSkillsData : IEquatable<PlayerSkillsSkillsData>, IEquatable<IReadOnlyDictionary<Skill, byte>>
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

        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals (PlayerSkillsSkillsData? left, PlayerSkillsSkillsData? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left == null || right == null)
                return false;

            return
                left.OneHanded == right.OneHanded
                && left.TwoHanded == right.TwoHanded
                && left.Archery == right.Archery
                && left.Block == right.Block
                && left.Smithing == right.Smithing
                && left.HeavyArmor == right.HeavyArmor
                && left.LightArmor == right.LightArmor
                && left.Pickpocket == right.Pickpocket
                && left.Lockpicking == right.Lockpicking
                && left.Sneak == right.Sneak
                && left.Alchemy == right.Alchemy
                && left.Speech == right.Speech
                && left.Alteration == right.Alteration
                && left.Conjuration == right.Conjuration
                && left.Destruction == right.Destruction
                && left.Illusion == right.Illusion
                && left.Restoration == right.Restoration
                && left.Enchanting == right.Enchanting;
        }

        public bool Equals (PlayerSkillsSkillsData? other) => Equals(this, other);

        public bool Equals (IReadOnlyDictionary<Skill, byte>? other)
            => other != null
            && (OneHanded == null || OneHanded == other[Skill.OneHanded])
            && (TwoHanded == null || TwoHanded == other[Skill.TwoHanded])
            && (Archery == null || Archery == other[Skill.Archery])
            && (Block == null || Block == other[Skill.Block])
            && (Smithing == null || Smithing == other[Skill.Smithing])
            && (HeavyArmor == null || HeavyArmor == other[Skill.HeavyArmor])
            && (LightArmor == null || LightArmor == other[Skill.LightArmor])
            && (Pickpocket == null || Pickpocket == other[Skill.Pickpocket])
            && (Lockpicking == null || Lockpicking == other[Skill.Lockpicking])
            && (Sneak == null || Sneak == other[Skill.Sneak])
            && (Alchemy == null || Alchemy == other[Skill.Alchemy])
            && (Speech == null || Speech == other[Skill.Speech])
            && (Alteration == null || Alteration == other[Skill.Alteration])
            && (Conjuration == null || Conjuration == other[Skill.Conjuration])
            && (Destruction == null || Destruction == other[Skill.Destruction])
            && (Illusion == null || Illusion == other[Skill.Illusion])
            && (Restoration == null || Restoration == other[Skill.Restoration])
            && (Enchanting == null || Enchanting == other[Skill.Enchanting]);

        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public override bool Equals (object? obj)
        {
            if (obj is null)
                return false;

            if (obj is PlayerSkillsSkillsData psdObj)
                return Equals(psdObj);

            if (obj is IReadOnlyDictionary<Skill, byte> rodObj)
                return Equals(rodObj);

            return false;
        }

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

            if (OneHanded != null && OneHanded != update[Skill.OneHanded])
            {
                count++;
                update[Skill.OneHanded] = (byte)OneHanded;
            }

            if (TwoHanded != null && TwoHanded != update[Skill.TwoHanded])
            {
                count++;
                update[Skill.TwoHanded] = (byte)TwoHanded;
            }

            if (Archery != null && Archery != update[Skill.Archery])
            {
                count++;
                update[Skill.Archery] = (byte)Archery;
            }

            if (Block != null && Block != update[Skill.Block])
            {
                count++;
                update[Skill.Block] = (byte)Block;
            }

            if (Smithing != null && Smithing != update[Skill.Smithing])
            {
                count++;
                update[Skill.Smithing] = (byte)Smithing;
            }

            if (HeavyArmor != null && HeavyArmor != update[Skill.HeavyArmor])
            {
                count++;
                update[Skill.HeavyArmor] = (byte)HeavyArmor;
            }

            if (LightArmor != null && LightArmor != update[Skill.LightArmor])
            {
                count++;
                update[Skill.LightArmor] = (byte)LightArmor;
            }

            if (Pickpocket != null && Pickpocket != update[Skill.Pickpocket])
            {
                count++;
                update[Skill.Pickpocket] = (byte)Pickpocket;
            }

            if (Lockpicking != null && Lockpicking != update[Skill.Lockpicking])
            {
                count++;
                update[Skill.Lockpicking] = (byte)Lockpicking;
            }

            if (Sneak != null && Sneak != update[Skill.Sneak])
            {
                count++;
                update[Skill.Sneak] = (byte)Sneak;
            }

            if (Alchemy != null && Alchemy != update[Skill.Alchemy])
            {
                count++;
                update[Skill.Alchemy] = (byte)Alchemy;
            }

            if (Speech != null && Speech != update[Skill.Speech])
            {
                count++;
                update[Skill.Speech] = (byte)Speech;
            }

            if (Alteration != null && Alteration != update[Skill.Alteration])
            {
                count++;
                update[Skill.Alteration] = (byte)Alteration;
            }

            if (Conjuration != null && Conjuration != update[Skill.Conjuration])
            {
                count++;
                update[Skill.Conjuration] = (byte)Conjuration;
            }

            if (Destruction != null && Destruction != update[Skill.Destruction])
            {
                count++;
                update[Skill.Destruction] = (byte)Destruction;
            }

            if (Illusion != null && Illusion != update[Skill.Illusion])
            {
                count++;
                update[Skill.Illusion] = (byte)Illusion;
            }

            if (Restoration != null && Restoration != update[Skill.Restoration])
            {
                count++;
                update[Skill.Restoration] = (byte)Restoration;
            }

            if (Enchanting != null && Enchanting != update[Skill.Enchanting])
            {
                count++;
                update[Skill.Enchanting] = (byte)Enchanting;
            }

            return count;
        }
    }
}