namespace GenericSynthesisPatcher.Json.Data
{
    public partial class GSPRule
    {
        public enum Type
        {
            UNKNOWN = -1,
            ALCH,
            AMMO,
            ARMO,
            BOOK,
            CELL,
            FACT,
            INGR,
            KEYM,
            MISC,
            NPC,
            OTFT,
            SCRL,
            WEAP,
            Ammunition = AMMO,
            Armor = ARMO,
            Faction = FACT,
            Ingestible = ALCH,
            Ingredient = INGR,
            Key = KEYM,
            MiscItem = MISC,
            Outfit = OTFT,
            Scroll = SCRL,
            Weapon = WEAP
        }
    }
}