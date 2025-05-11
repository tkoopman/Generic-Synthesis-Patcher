using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Synthesis;

namespace GenericSynthesisPatcher.Games.Fallout4
{
    public class RecordTypeMappings : Universal.RecordTypeMappings
    {
        public RecordTypeMappings (IPatcherState<IFallout4Mod, IFallout4ModGetter> state)
        {
#pragma warning disable format
            Add(IAcousticSpaceGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AcousticSpace().WinningContextOverrides());
            Add(IActionRecordGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ActionRecord().WinningContextOverrides());
            Add(IActivatorGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Activator().WinningContextOverrides());
            Add(IActorValueInformationGetter.StaticRegistration , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ActorValueInformation().WinningContextOverrides());
            Add(IAddonNodeGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AddonNode().WinningContextOverrides());
            Add(IAimModelGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AimModel().WinningContextOverrides());
            Add(IAmmunitionGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ammunition().WinningContextOverrides());
            Add(IAnimatedObjectGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AnimatedObject().WinningContextOverrides());
            Add(IAObjectModificationGetter.StaticRegistration   , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AObjectModification().WinningContextOverrides());
            Add(IArmorAddonGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ArmorAddon().WinningContextOverrides());
            Add(IArmorGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Armor().WinningContextOverrides());
            Add(IArtObjectGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ArtObject().WinningContextOverrides());
            Add(IAssociationTypeGetter.StaticRegistration       , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AssociationType().WinningContextOverrides());
            Add(IAttractionRuleGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AttractionRule().WinningContextOverrides());
            Add(IAudioCategorySnapshotGetter.StaticRegistration , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AudioCategorySnapshot().WinningContextOverrides());
            Add(IBendableSplineGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().BendableSpline().WinningContextOverrides());
            Add(IBookGetter.StaticRegistration                  , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Book().WinningContextOverrides());
            Add(ICameraPathGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CameraPath().WinningContextOverrides());
            Add(ICameraShotGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CameraShot().WinningContextOverrides());
            Add(ICellGetter.StaticRegistration                  , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Cell().WinningContextOverrides(state.LinkCache));
            Add(IClassGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Class().WinningContextOverrides());
            Add(IClimateGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Climate().WinningContextOverrides());
            Add(ICollisionLayerGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CollisionLayer().WinningContextOverrides());
            Add(IColorRecordGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ColorRecord().WinningContextOverrides());
            Add(ICombatStyleGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CombatStyle().WinningContextOverrides());
            Add(IComponentGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Component().WinningContextOverrides());
            Add(IConstructibleObjectGetter.StaticRegistration   , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ConstructibleObject().WinningContextOverrides());
            Add(IContainerGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Container().WinningContextOverrides());
            Add(IDefaultObjectGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DefaultObject().WinningContextOverrides());
            Add(IDialogViewGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DialogView().WinningContextOverrides());
            Add(IDoorGetter.StaticRegistration                  , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Door().WinningContextOverrides());
            Add(IEffectShaderGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EffectShader().WinningContextOverrides());
            Add(IEncounterZoneGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EncounterZone().WinningContextOverrides());
            Add(IEquipTypeGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EquipType().WinningContextOverrides());
            Add(IExplosionGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Explosion().WinningContextOverrides());
            Add(IFactionGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Faction().WinningContextOverrides());
            Add(IFloraGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Flora().WinningContextOverrides());
            Add(IFootstepGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Footstep().WinningContextOverrides());
            Add(IFootstepSetGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().FootstepSet().WinningContextOverrides());
            Add(IFormListGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().FormList().WinningContextOverrides());
            Add(IFurnitureGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Furniture().WinningContextOverrides());
            Add(IGodRaysGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().GodRays().WinningContextOverrides());
            Add(IGrassGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Grass().WinningContextOverrides());
            Add(IHazardGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Hazard().WinningContextOverrides());
            Add(IHeadPartGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().HeadPart().WinningContextOverrides());
            Add(IHolotapeGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Holotape().WinningContextOverrides());
            Add(IIdleAnimationGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().IdleAnimation().WinningContextOverrides());
            Add(IIdleMarkerGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().IdleMarker().WinningContextOverrides());
            Add(IImageSpaceAdapterGetter.StaticRegistration     , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ImageSpaceAdapter().WinningContextOverrides());
            Add(IImageSpaceGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ImageSpace().WinningContextOverrides());
            Add(IImpactGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Impact().WinningContextOverrides());
            Add(IIngestibleGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingestible().WinningContextOverrides());
            Add(IIngredientGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingredient().WinningContextOverrides());
            Add(IInstanceNamingRulesGetter.StaticRegistration   , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().InstanceNamingRules().WinningContextOverrides());
            Add(IKeyGetter.StaticRegistration                   , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Key().WinningContextOverrides());
            Add(IKeywordGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Keyword().WinningContextOverrides());
            Add(ILandscapeTextureGetter.StaticRegistration      , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LandscapeTexture().WinningContextOverrides());
            Add(ILayerGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Layer().WinningContextOverrides());
            Add(ILensFlareGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LensFlare().WinningContextOverrides());
            Add(ILeveledItemGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledItem().WinningContextOverrides());
            Add(ILeveledNpcGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledNpc().WinningContextOverrides());
            Add(ILightGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Light().WinningContextOverrides());
            Add(ILightingTemplateGetter.StaticRegistration      , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LightingTemplate().WinningContextOverrides());
            Add(ILoadScreenGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LoadScreen().WinningContextOverrides());
            Add(ILocationGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Location().WinningContextOverrides());
            Add(ILocationReferenceTypeGetter.StaticRegistration , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LocationReferenceType().WinningContextOverrides());
            Add(IMagicEffectGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MagicEffect().WinningContextOverrides());
            Add(IMaterialObjectGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MaterialObject().WinningContextOverrides());
            Add(IMaterialSwapGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MaterialSwap().WinningContextOverrides());
            Add(IMaterialTypeGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MaterialType().WinningContextOverrides());
            Add(IMessageGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Message().WinningContextOverrides());
            Add(IMiscItemGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MiscItem().WinningContextOverrides());
            Add(IMovableStaticGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MovableStatic().WinningContextOverrides());
            Add(IMovementTypeGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MovementType().WinningContextOverrides());
            Add(IMusicTrackGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MusicTrack().WinningContextOverrides());
            Add(IMusicTypeGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MusicType().WinningContextOverrides());
            Add(INavigationMeshInfoMapGetter.StaticRegistration , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().NavigationMeshInfoMap().WinningContextOverrides());
            Add(INpcGetter.StaticRegistration                   , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Npc().WinningContextOverrides());
            Add(IObjectEffectGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ObjectEffect().WinningContextOverrides());
            Add(IOutfitGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Outfit().WinningContextOverrides());
            Add(IPackageGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Package().WinningContextOverrides());
            Add(IPackInGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().PackIn().WinningContextOverrides());
            Add(IPerkGetter.StaticRegistration                  , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Perk().WinningContextOverrides());
            Add(IProjectileGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Projectile().WinningContextOverrides());
            Add(IQuestGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Quest().WinningContextOverrides());
            Add(IRaceGetter.StaticRegistration                  , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Race().WinningContextOverrides());
            Add(IReferenceGroupGetter.StaticRegistration        , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ReferenceGroup().WinningContextOverrides());
            Add(IRegionGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Region().WinningContextOverrides());
            Add(IRelationshipGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Relationship().WinningContextOverrides());
            Add(IReverbParametersGetter.StaticRegistration      , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ReverbParameters().WinningContextOverrides());
            Add(ISceneCollectionGetter.StaticRegistration       , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SceneCollection().WinningContextOverrides());
            Add(IShaderParticleGeometryGetter.StaticRegistration, () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ShaderParticleGeometry().WinningContextOverrides());
            Add(ISoundCategoryGetter.StaticRegistration         , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundCategory().WinningContextOverrides());
            Add(ISoundDescriptorGetter.StaticRegistration       , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundDescriptor().WinningContextOverrides());
            Add(ISoundKeywordMappingGetter.StaticRegistration   , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundKeywordMapping().WinningContextOverrides());
            Add(ISoundMarkerGetter.StaticRegistration           , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundMarker().WinningContextOverrides());
            Add(ISoundOutputModelGetter.StaticRegistration      , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundOutputModel().WinningContextOverrides());
            Add(ISpellGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Spell().WinningContextOverrides());
            Add(IStaticCollectionGetter.StaticRegistration      , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().StaticCollection().WinningContextOverrides());
            Add(IStaticGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Static().WinningContextOverrides());
            Add(ITalkingActivatorGetter.StaticRegistration      , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().TalkingActivator().WinningContextOverrides());
            Add(ITerminalGetter.StaticRegistration              , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Terminal().WinningContextOverrides());
            Add(ITextureSetGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().TextureSet().WinningContextOverrides());
            Add(ITransformGetter.StaticRegistration             , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Transform().WinningContextOverrides());
            Add(ITreeGetter.StaticRegistration                  , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Tree().WinningContextOverrides());
            Add(IVisualEffectGetter.StaticRegistration          , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().VisualEffect().WinningContextOverrides());
            Add(IWaterGetter.StaticRegistration                 , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Water().WinningContextOverrides());
            Add(IWeaponGetter.StaticRegistration                , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weapon().WinningContextOverrides());
            Add(IWeatherGetter.StaticRegistration               , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weather().WinningContextOverrides());
            Add(IWorldspaceGetter.StaticRegistration            , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Worldspace().WinningContextOverrides());
            Add(IZoomGetter.StaticRegistration                  , () => state.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Zoom().WinningContextOverrides());
#pragma warning restore format
        }
    }
}