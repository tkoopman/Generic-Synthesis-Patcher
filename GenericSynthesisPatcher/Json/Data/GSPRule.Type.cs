using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Json.Data
{
    public partial class GSPRule
    {
        public enum Type
        {
            UNKNOWN = -1,
            ALCH = TypeFlags.ALCH,
            AMMO = TypeFlags.AMMO,
            ARMO = TypeFlags.ARMO,
            BOOK = TypeFlags.BOOK,
            CELL = TypeFlags.CELL,
            CONT = TypeFlags.CONT,
            FACT = TypeFlags.FACT,
            INGR = TypeFlags.INGR,
            KEYM = TypeFlags.KEYM,
            MISC = TypeFlags.MISC,
            NPC = TypeFlags.NPC,
            OTFT = TypeFlags.OTFT,
            SCRL = TypeFlags.SCRL,
            WEAP = TypeFlags.WEAP,
            WRLD = TypeFlags.WRLD,
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
            Worldspace = WRLD
        }

        [Flags]
        public enum TypeFlags
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
            All = ~(~0 << 15)
        }

        /// <summary>
        /// Return GSP rule type for a record.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static GSPRule.Type GetGSPRuleType ( IMajorRecordGetter record ) => record switch
        {
            IIngestibleGetter => GSPRule.Type.ALCH,
            IAmmunitionGetter => GSPRule.Type.AMMO,
            IArmorGetter => GSPRule.Type.ARMO,
            IBookGetter => GSPRule.Type.BOOK,
            ICellGetter => GSPRule.Type.CELL,
            IContainerGetter => GSPRule.Type.CONT,
            IFactionGetter => GSPRule.Type.FACT,
            IIngredientGetter => GSPRule.Type.INGR,
            IKeyGetter => GSPRule.Type.KEYM,
            IMiscItemGetter => GSPRule.Type.MISC,
            INpcGetter => GSPRule.Type.NPC,
            IOutfitGetter => GSPRule.Type.OTFT,
            IScrollGetter => GSPRule.Type.SCRL,
            IWeaponGetter => GSPRule.Type.WEAP,
            IWorldspaceGetter => GSPRule.Type.WRLD,
            _ => GSPRule.Type.UNKNOWN
        };

        /// <summary>
        /// Return GSP rule type as string for a record.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static string GetGSPRuleTypeAsString ( IMajorRecordGetter record ) => record switch
        {
            IIngestibleGetter => "ALCH",
            IAmmunitionGetter => "AMMO",
            IArmorGetter => "ARMO",
            IBookGetter => "BOOK",
            ICellGetter => "CELL",
            IContainerGetter => "CONT",
            IFactionGetter => "FACT",
            IIngredientGetter => "INGR",
            IKeyGetter => "KEYM",
            IMiscItemGetter => "MISC",
            INpcGetter => "NPC",
            IOutfitGetter => "OTFT",
            IScrollGetter => "SCRL",
            IWeaponGetter => "WEAP",
            IWorldspaceGetter => "WRLD",
            _ => "UNKNOWN"
        };
    }
}