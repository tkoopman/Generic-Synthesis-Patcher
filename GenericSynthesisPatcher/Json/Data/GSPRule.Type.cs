using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

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
            WRLD,
            Ammunition = AMMO,
            Armor = ARMO,
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
    }
}