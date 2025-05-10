using Mutagen.Bethesda;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Synthesis;

namespace GenericSynthesisPatcher.Games.Oblivion
{
    public class RecordTypeMappings : Universal.RecordTypeMappings
    {
        public RecordTypeMappings (IPatcherState<IOblivionMod, IOblivionModGetter> state)
        {
#pragma warning disable format
            Add(IActivatorGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Activator().WinningContextOverrides());
            Add(IAIPackageGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AIPackage().WinningContextOverrides());
            Add(IAlchemicalApparatusGetter.StaticRegistration, () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AlchemicalApparatus().WinningContextOverrides());
            Add(IAmmunitionGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ammunition().WinningContextOverrides());
            Add(IAnimatedObjectGetter.StaticRegistration     , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AnimatedObject().WinningContextOverrides());
            Add(IArmorGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Armor().WinningContextOverrides());
            Add(IBirthsignGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Birthsign().WinningContextOverrides());
            Add(IBookGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Book().WinningContextOverrides());
            Add(ICellGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Cell().WinningContextOverrides(state.LinkCache));
            Add(IClassGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Class().WinningContextOverrides());
            Add(IClimateGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Climate().WinningContextOverrides());
            Add(IClothingGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Clothing().WinningContextOverrides());
            Add(ICombatStyleGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CombatStyle().WinningContextOverrides());
            Add(IContainerGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Container().WinningContextOverrides());
            Add(ICreatureGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Creature().WinningContextOverrides());
            Add(IDialogTopicGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DialogTopic().WinningContextOverrides());
            Add(IDoorGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Door().WinningContextOverrides());
            Add(IEffectShaderGetter.StaticRegistration       , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EffectShader().WinningContextOverrides());
            Add(IEnchantmentGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Enchantment().WinningContextOverrides());
            Add(IEyeGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Eye().WinningContextOverrides());
            Add(IFactionGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Faction().WinningContextOverrides());
            Add(IFloraGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Flora().WinningContextOverrides());
            Add(IFurnitureGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Furniture().WinningContextOverrides());
            Add(IGameSettingGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().GameSetting().WinningContextOverrides());
            Add(IGlobalGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Global().WinningContextOverrides());
            Add(IGrassGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Grass().WinningContextOverrides());
            Add(IHairGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Hair().WinningContextOverrides());
            Add(IIdleAnimationGetter.StaticRegistration      , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().IdleAnimation().WinningContextOverrides());
            Add(IIngredientGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingredient().WinningContextOverrides());
            Add(IKeyGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Key().WinningContextOverrides());
            Add(ILandTextureGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LandTexture().WinningContextOverrides());
            Add(ILeveledCreatureGetter.StaticRegistration    , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledCreature().WinningContextOverrides());
            Add(ILeveledItemGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledItem().WinningContextOverrides());
            Add(ILeveledSpellGetter.StaticRegistration       , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledSpell().WinningContextOverrides());
            Add(ILightGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Light().WinningContextOverrides());
            Add(ILoadScreenGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LoadScreen().WinningContextOverrides());
            Add(IMagicEffectGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MagicEffect().WinningContextOverrides());
            Add(IMiscellaneousGetter.StaticRegistration      , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Miscellaneous().WinningContextOverrides());
            Add(INpcGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Npc().WinningContextOverrides());
            Add(IPotionGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Potion().WinningContextOverrides());
            Add(IQuestGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Quest().WinningContextOverrides());
            Add(IRaceGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Race().WinningContextOverrides());
            Add(IRegionGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Region().WinningContextOverrides());
            Add(IScriptGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Script().WinningContextOverrides());
            Add(ISigilStoneGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SigilStone().WinningContextOverrides());
            Add(ISkillRecordGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SkillRecord().WinningContextOverrides());
            Add(ISoulGemGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoulGem().WinningContextOverrides());
            Add(ISoundGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Sound().WinningContextOverrides());
            Add(ISpellGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Spell().WinningContextOverrides());
            Add(IStaticGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Static().WinningContextOverrides());
            Add(ISubspaceGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Subspace().WinningContextOverrides());
            Add(ITreeGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Tree().WinningContextOverrides());
            Add(IWaterGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Water().WinningContextOverrides());
            Add(IWeaponGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weapon().WinningContextOverrides());
            Add(IWeatherGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weather().WinningContextOverrides());
            Add(IWorldspaceGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Worldspace().WinningContextOverrides());
#pragma warning restore format
        }
    }
}