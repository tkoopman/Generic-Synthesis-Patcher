using GenericSynthesisPatcher.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    [Flags]
    [JsonConverter(typeof(FlagConverter))]
    public enum RecordTypes
    {
        NONE = 0 << 0,
        ALCH = 1 << 0,
        AMMO = 1 << 1,
        ARMO = 1 << 2,
        BOOK = 1 << 3,
        CELL = 1 << 4,
        CONT = 1 << 5,
        FACT = 1 << 6,
        INGR = 1 << 7,
        KEYM = 1 << 8,
        MISC = 1 << 9,
        NPC = 1 << 10,
        OTFT = 1 << 11,
        SCRL = 1 << 12,
        WEAP = 1 << 13,
        WRLD = 1 << 14,
        All = ~(~0 << 15),
        Ammunition = AMMO,
        Armor = ARMO,
        Container = CONT,
        Faction = FACT,
        Ingestible = ALCH,
        Ingredient = INGR,
        Key = KEYM,
        MiscItem = MISC,
        Outfit = OTFT,
        Scroll = SCRL,
        Weapon = WEAP,
        Worldspace = WRLD,
        UNKNOWN = NONE
    }
}