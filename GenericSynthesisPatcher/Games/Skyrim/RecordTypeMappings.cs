using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace GenericSynthesisPatcher.Games.Skyrim
{
    public class RecordTypeMappings : Universal.RecordTypeMappings
    {
        public RecordTypeMappings (IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
#pragma warning disable format
            Add(IAcousticSpaceGetter.StaticRegistration,          () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AcousticSpace().WinningContextOverrides());
            Add(IActionRecordGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ActionRecord().WinningContextOverrides());
            Add(IActivatorGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Activator().WinningContextOverrides());
            Add(IActorValueInformationGetter.StaticRegistration,  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ActorValueInformation().WinningContextOverrides());
            Add(IAddonNodeGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AddonNode().WinningContextOverrides());
            Add(IAlchemicalApparatusGetter.StaticRegistration,    () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AlchemicalApparatus().WinningContextOverrides());
            Add(IAmmunitionGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ammunition().WinningContextOverrides());
            Add(IAnimatedObjectGetter.StaticRegistration,         () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AnimatedObject().WinningContextOverrides());
            Add(IArmorAddonGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ArmorAddon().WinningContextOverrides());
            Add(IArmorGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Armor().WinningContextOverrides());
            Add(IArtObjectGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ArtObject().WinningContextOverrides());
            Add(IAssociationTypeGetter.StaticRegistration,        () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AssociationType().WinningContextOverrides());
            Add(IBodyPartDataGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().BodyPartData().WinningContextOverrides());
            Add(IBookGetter.StaticRegistration,                   () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Book().WinningContextOverrides());
            Add(ICameraPathGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CameraPath().WinningContextOverrides());
            Add(ICameraShotGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CameraShot().WinningContextOverrides());
            Add(ICellGetter.StaticRegistration,                   () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Cell().WinningContextOverrides(Global.State.LinkCache));
            Add(IClassGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Class().WinningContextOverrides());
            Add(IClimateGetter.StaticRegistration,                () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Climate().WinningContextOverrides());
            Add(ICollisionLayerGetter.StaticRegistration,         () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CollisionLayer().WinningContextOverrides());
            Add(IColorRecordGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ColorRecord().WinningContextOverrides());
            Add(ICombatStyleGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CombatStyle().WinningContextOverrides());
            Add(IConstructibleObjectGetter.StaticRegistration,    () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ConstructibleObject().WinningContextOverrides());
            Add(IContainerGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Container().WinningContextOverrides());
            Add(IDebrisGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Debris().WinningContextOverrides());
            Add(IDefaultObjectManagerGetter.StaticRegistration,   () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DefaultObjectManager().WinningContextOverrides());
            Add(IDialogBranchGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DialogBranch().WinningContextOverrides());
            Add(IDialogTopicGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DialogTopic().WinningContextOverrides());
            Add(IDialogViewGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DialogView().WinningContextOverrides());
            Add(IDoorGetter.StaticRegistration,                   () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Door().WinningContextOverrides());
            Add(IDualCastDataGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DualCastData().WinningContextOverrides());
            Add(IEffectShaderGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EffectShader().WinningContextOverrides());
            Add(IEncounterZoneGetter.StaticRegistration,          () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EncounterZone().WinningContextOverrides());
            Add(IEquipTypeGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EquipType().WinningContextOverrides());
            Add(IExplosionGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Explosion().WinningContextOverrides());
            Add(IEyesGetter.StaticRegistration,                   () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Eyes().WinningContextOverrides());
            Add(IFactionGetter.StaticRegistration,                () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Faction().WinningContextOverrides());
            Add(IFloraGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Flora().WinningContextOverrides());
            Add(IFootstepGetter.StaticRegistration,               () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Footstep().WinningContextOverrides());
            Add(IFootstepSetGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().FootstepSet().WinningContextOverrides());
            Add(IFormListGetter.StaticRegistration,               () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().FormList().WinningContextOverrides());
            Add(IFurnitureGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Furniture().WinningContextOverrides());
            Add(IGameSettingGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().GameSetting().WinningContextOverrides());
            Add(IGlobalGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Global().WinningContextOverrides());
            Add(IGrassGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Grass().WinningContextOverrides());
            Add(IHazardGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Hazard().WinningContextOverrides());
            Add(IHeadPartGetter.StaticRegistration,               () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().HeadPart().WinningContextOverrides());
            Add(IIdleAnimationGetter.StaticRegistration,          () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().IdleAnimation().WinningContextOverrides());
            Add(IIdleMarkerGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().IdleMarker().WinningContextOverrides());
            Add(IImageSpaceAdapterGetter.StaticRegistration,      () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ImageSpaceAdapter().WinningContextOverrides());
            Add(IImageSpaceGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ImageSpace().WinningContextOverrides());
            Add(IImpactDataSetGetter.StaticRegistration,          () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ImpactDataSet().WinningContextOverrides());
            Add(IImpactGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Impact().WinningContextOverrides());
            Add(IIngestibleGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingestible().WinningContextOverrides());
            Add(IIngredientGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingredient().WinningContextOverrides());
            Add(IKeyGetter.StaticRegistration,                    () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Key().WinningContextOverrides());
            Add(IKeywordGetter.StaticRegistration,                () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Keyword().WinningContextOverrides());
            Add(ILandscapeTextureGetter.StaticRegistration,       () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LandscapeTexture().WinningContextOverrides());
            Add(ILeveledItemGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledItem().WinningContextOverrides());
            Add(ILeveledNpcGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledNpc().WinningContextOverrides());
            Add(ILeveledSpellGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledSpell().WinningContextOverrides());
            Add(ILightGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Light().WinningContextOverrides());
            Add(ILightingTemplateGetter.StaticRegistration,       () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LightingTemplate().WinningContextOverrides());
            Add(ILoadScreenGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LoadScreen().WinningContextOverrides());
            Add(ILocationGetter.StaticRegistration,               () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Location().WinningContextOverrides());
            Add(ILocationReferenceTypeGetter.StaticRegistration,  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LocationReferenceType().WinningContextOverrides());
            Add(IMagicEffectGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MagicEffect().WinningContextOverrides());
            Add(IMaterialObjectGetter.StaticRegistration,         () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MaterialObject().WinningContextOverrides());
            Add(IMaterialTypeGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MaterialType().WinningContextOverrides());
            Add(IMessageGetter.StaticRegistration,                () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Message().WinningContextOverrides());
            Add(IMiscItemGetter.StaticRegistration,               () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MiscItem().WinningContextOverrides());
            Add(IMoveableStaticGetter.StaticRegistration,         () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MoveableStatic().WinningContextOverrides());
            Add(IMovementTypeGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MovementType().WinningContextOverrides());
            Add(IMusicTrackGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MusicTrack().WinningContextOverrides());
            Add(IMusicTypeGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MusicType().WinningContextOverrides());
            Add(INpcGetter.StaticRegistration,                    () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Npc().WinningContextOverrides());
            Add(IObjectEffectGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ObjectEffect().WinningContextOverrides());
            Add(IOutfitGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Outfit().WinningContextOverrides());
            Add(IPackageGetter.StaticRegistration,                () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Package().WinningContextOverrides());
            Add(IPerkGetter.StaticRegistration,                   () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Perk().WinningContextOverrides());
            Add(IProjectileGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Projectile().WinningContextOverrides());
            Add(IQuestGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Quest().WinningContextOverrides());
            Add(IRaceGetter.StaticRegistration,                   () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Race().WinningContextOverrides());
            Add(IRegionGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Region().WinningContextOverrides());
            Add(IRelationshipGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Relationship().WinningContextOverrides());
            Add(IReverbParametersGetter.StaticRegistration,       () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ReverbParameters().WinningContextOverrides());
            Add(ISceneGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Scene().WinningContextOverrides());
            Add(IScrollGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Scroll().WinningContextOverrides());
            Add(IShaderParticleGeometryGetter.StaticRegistration, () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ShaderParticleGeometry().WinningContextOverrides());
            Add(IShoutGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Shout().WinningContextOverrides());
            Add(ISoulGemGetter.StaticRegistration,                () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoulGem().WinningContextOverrides());
            Add(ISoundCategoryGetter.StaticRegistration,          () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundCategory().WinningContextOverrides());
            Add(ISoundDescriptorGetter.StaticRegistration,        () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundDescriptor().WinningContextOverrides());
            Add(ISoundMarkerGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundMarker().WinningContextOverrides());
            Add(ISoundOutputModelGetter.StaticRegistration,       () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundOutputModel().WinningContextOverrides());
            Add(ISpellGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Spell().WinningContextOverrides());
            Add(IStaticGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Static().WinningContextOverrides());

            //Add(IStoryManagerBranchNodeGetter.StaticRegistration, () => []);
            //Add(IStoryManagerEventNodeGetter.StaticRegistration,  () => []);
            //Add(IStoryManagerQuestNodeGetter.StaticRegistration,  () => []);
            Add(ITalkingActivatorGetter.StaticRegistration,       () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().TalkingActivator().WinningContextOverrides());
            Add(ITextureSetGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().TextureSet().WinningContextOverrides());
            Add(ITreeGetter.StaticRegistration,                   () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Tree().WinningContextOverrides());
            Add(IVisualEffectGetter.StaticRegistration,           () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().VisualEffect().WinningContextOverrides());
            Add(IVoiceTypeGetter.StaticRegistration,              () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().VoiceType().WinningContextOverrides());
            Add(IWaterGetter.StaticRegistration,                  () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Water().WinningContextOverrides());
            Add(IWeaponGetter.StaticRegistration,                 () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weapon().WinningContextOverrides());
            Add(IWeatherGetter.StaticRegistration,                () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weather().WinningContextOverrides());
            Add(IWordOfPowerGetter.StaticRegistration,            () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().WordOfPower().WinningContextOverrides());
            Add(IWorldspaceGetter.StaticRegistration,             () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Worldspace().WinningContextOverrides());
#pragma warning restore format
        }
    }
}