using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using static GenericSynthesisPatcher.Helpers.RecordTypeMapping;

namespace GenericSynthesisPatcher.Helpers
{
    public static class RecordTypeMappings
    {
        public static readonly IReadOnlyList<RecordTypeMapping> All;
        private const int ClassLogCode = 0x06;
        private static readonly Dictionary<string, RecordTypeMapping> ByName = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<Type, RecordTypeMapping> ByType = [];

        static RecordTypeMappings ()
        {
            List<RecordTypeMapping> all = [];
            #pragma warning disable format
            all.Add(Add(IAcousticSpaceGetter.StaticRegistration,          () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AcousticSpace().WinningContextOverrides()));
            all.Add(Add(IActionRecordGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ActionRecord().WinningContextOverrides()));
            all.Add(Add(IActivatorGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Activator().WinningContextOverrides()));
            all.Add(Add(IActorValueInformationGetter.StaticRegistration,  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ActorValueInformation().WinningContextOverrides()));
            all.Add(Add(IAddonNodeGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AddonNode().WinningContextOverrides()));
            all.Add(Add(IAlchemicalApparatusGetter.StaticRegistration,    () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AlchemicalApparatus().WinningContextOverrides()));
            all.Add(Add(IAmmunitionGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ammunition().WinningContextOverrides()));
            all.Add(Add(IAnimatedObjectGetter.StaticRegistration,         () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AnimatedObject().WinningContextOverrides()));
            all.Add(Add(IArmorAddonGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ArmorAddon().WinningContextOverrides()));
            all.Add(Add(IArmorGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Armor().WinningContextOverrides()));
            all.Add(Add(IArtObjectGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ArtObject().WinningContextOverrides()));
            all.Add(Add(IAssociationTypeGetter.StaticRegistration,        () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().AssociationType().WinningContextOverrides()));
            all.Add(Add(IBodyPartDataGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().BodyPartData().WinningContextOverrides()));
            all.Add(Add(IBookGetter.StaticRegistration,                   () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Book().WinningContextOverrides()));
            all.Add(Add(ICameraPathGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CameraPath().WinningContextOverrides()));
            all.Add(Add(ICameraShotGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CameraShot().WinningContextOverrides()));
            all.Add(Add(ICellGetter.StaticRegistration,                   () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Cell().WinningContextOverrides(Global.State.LinkCache)));
            all.Add(Add(IClassGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Class().WinningContextOverrides()));
            all.Add(Add(IClimateGetter.StaticRegistration,                () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Climate().WinningContextOverrides()));
            all.Add(Add(ICollisionLayerGetter.StaticRegistration,         () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CollisionLayer().WinningContextOverrides()));
            all.Add(Add(IColorRecordGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ColorRecord().WinningContextOverrides()));
            all.Add(Add(ICombatStyleGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().CombatStyle().WinningContextOverrides()));
            all.Add(Add(IConstructibleObjectGetter.StaticRegistration,    () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ConstructibleObject().WinningContextOverrides()));
            all.Add(Add(IContainerGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Container().WinningContextOverrides()));
            all.Add(Add(IDebrisGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Debris().WinningContextOverrides()));
            all.Add(Add(IDefaultObjectManagerGetter.StaticRegistration,   () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DefaultObjectManager().WinningContextOverrides()));
            all.Add(Add(IDialogBranchGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DialogBranch().WinningContextOverrides()));
            all.Add(Add(IDialogTopicGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DialogTopic().WinningContextOverrides()));
            all.Add(Add(IDialogViewGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DialogView().WinningContextOverrides()));
            all.Add(Add(IDoorGetter.StaticRegistration,                   () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Door().WinningContextOverrides()));
            all.Add(Add(IDualCastDataGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().DualCastData().WinningContextOverrides()));
            all.Add(Add(IEffectShaderGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EffectShader().WinningContextOverrides()));
            all.Add(Add(IEncounterZoneGetter.StaticRegistration,          () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EncounterZone().WinningContextOverrides()));
            all.Add(Add(IEquipTypeGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().EquipType().WinningContextOverrides()));
            all.Add(Add(IExplosionGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Explosion().WinningContextOverrides()));
            all.Add(Add(IEyesGetter.StaticRegistration,                   () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Eyes().WinningContextOverrides()));
            all.Add(Add(IFactionGetter.StaticRegistration,                () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Faction().WinningContextOverrides()));
            all.Add(Add(IFloraGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Flora().WinningContextOverrides()));
            all.Add(Add(IFootstepGetter.StaticRegistration,               () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Footstep().WinningContextOverrides()));
            all.Add(Add(IFootstepSetGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().FootstepSet().WinningContextOverrides()));
            all.Add(Add(IFormListGetter.StaticRegistration,               () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().FormList().WinningContextOverrides()));
            all.Add(Add(IFurnitureGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Furniture().WinningContextOverrides()));
            all.Add(Add(IGameSettingGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().GameSetting().WinningContextOverrides()));
            all.Add(Add(IGlobalGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Global().WinningContextOverrides()));
            all.Add(Add(IGrassGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Grass().WinningContextOverrides()));
            all.Add(Add(IHazardGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Hazard().WinningContextOverrides()));
            all.Add(Add(IHeadPartGetter.StaticRegistration,               () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().HeadPart().WinningContextOverrides()));
            all.Add(Add(IIdleAnimationGetter.StaticRegistration,          () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().IdleAnimation().WinningContextOverrides()));
            all.Add(Add(IIdleMarkerGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().IdleMarker().WinningContextOverrides()));
            all.Add(Add(IImageSpaceAdapterGetter.StaticRegistration,      () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ImageSpaceAdapter().WinningContextOverrides()));
            all.Add(Add(IImageSpaceGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ImageSpace().WinningContextOverrides()));
            all.Add(Add(IImpactDataSetGetter.StaticRegistration,          () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ImpactDataSet().WinningContextOverrides()));
            all.Add(Add(IImpactGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Impact().WinningContextOverrides()));
            all.Add(Add(IIngestibleGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingestible().WinningContextOverrides()));
            all.Add(Add(IIngredientGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Ingredient().WinningContextOverrides()));
            all.Add(Add(IKeyGetter.StaticRegistration,                    () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Key().WinningContextOverrides()));
            all.Add(Add(IKeywordGetter.StaticRegistration,                () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Keyword().WinningContextOverrides()));
            all.Add(Add(ILandscapeTextureGetter.StaticRegistration,       () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LandscapeTexture().WinningContextOverrides()));
            all.Add(Add(ILeveledItemGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledItem().WinningContextOverrides()));
            all.Add(Add(ILeveledNpcGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledNpc().WinningContextOverrides()));
            all.Add(Add(ILeveledSpellGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LeveledSpell().WinningContextOverrides()));
            all.Add(Add(ILightGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Light().WinningContextOverrides()));
            all.Add(Add(ILightingTemplateGetter.StaticRegistration,       () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LightingTemplate().WinningContextOverrides()));
            all.Add(Add(ILoadScreenGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LoadScreen().WinningContextOverrides()));
            all.Add(Add(ILocationGetter.StaticRegistration,               () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Location().WinningContextOverrides()));
            all.Add(Add(ILocationReferenceTypeGetter.StaticRegistration,  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().LocationReferenceType().WinningContextOverrides()));
            all.Add(Add(IMagicEffectGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MagicEffect().WinningContextOverrides()));
            all.Add(Add(IMaterialObjectGetter.StaticRegistration,         () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MaterialObject().WinningContextOverrides()));
            all.Add(Add(IMaterialTypeGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MaterialType().WinningContextOverrides()));
            all.Add(Add(IMessageGetter.StaticRegistration,                () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Message().WinningContextOverrides()));
            all.Add(Add(IMiscItemGetter.StaticRegistration,               () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MiscItem().WinningContextOverrides()));
            all.Add(Add(IMoveableStaticGetter.StaticRegistration,         () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MoveableStatic().WinningContextOverrides()));
            all.Add(Add(IMovementTypeGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MovementType().WinningContextOverrides()));
            all.Add(Add(IMusicTrackGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MusicTrack().WinningContextOverrides()));
            all.Add(Add(IMusicTypeGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().MusicType().WinningContextOverrides()));
            all.Add(Add(INpcGetter.StaticRegistration,                    () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Npc().WinningContextOverrides()));
            all.Add(Add(IObjectEffectGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ObjectEffect().WinningContextOverrides()));
            all.Add(Add(IOutfitGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Outfit().WinningContextOverrides()));
            all.Add(Add(IPackageGetter.StaticRegistration,                () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Package().WinningContextOverrides()));
            all.Add(Add(IPerkGetter.StaticRegistration,                   () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Perk().WinningContextOverrides()));
            all.Add(Add(IProjectileGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Projectile().WinningContextOverrides()));
            all.Add(Add(IQuestGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Quest().WinningContextOverrides()));
            all.Add(Add(IRaceGetter.StaticRegistration,                   () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Race().WinningContextOverrides()));
            all.Add(Add(IRegionGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Region().WinningContextOverrides()));
            all.Add(Add(IRelationshipGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Relationship().WinningContextOverrides()));
            all.Add(Add(IReverbParametersGetter.StaticRegistration,       () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ReverbParameters().WinningContextOverrides()));
            all.Add(Add(ISceneGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Scene().WinningContextOverrides()));
            all.Add(Add(IScrollGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Scroll().WinningContextOverrides()));
            all.Add(Add(IShaderParticleGeometryGetter.StaticRegistration, () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().ShaderParticleGeometry().WinningContextOverrides()));
            all.Add(Add(IShoutGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Shout().WinningContextOverrides()));
            all.Add(Add(ISoulGemGetter.StaticRegistration,                () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoulGem().WinningContextOverrides()));
            all.Add(Add(ISoundCategoryGetter.StaticRegistration,          () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundCategory().WinningContextOverrides()));
            all.Add(Add(ISoundDescriptorGetter.StaticRegistration,        () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundDescriptor().WinningContextOverrides()));
            all.Add(Add(ISoundMarkerGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundMarker().WinningContextOverrides()));
            all.Add(Add(ISoundOutputModelGetter.StaticRegistration,       () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().SoundOutputModel().WinningContextOverrides()));
            all.Add(Add(ISpellGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Spell().WinningContextOverrides()));
            all.Add(Add(IStaticGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Static().WinningContextOverrides()));
            //all.Add(Add(IStoryManagerBranchNodeGetter.StaticRegistration, () => []));
            //all.Add(Add(IStoryManagerEventNodeGetter.StaticRegistration,  () => []));
            //all.Add(Add(IStoryManagerQuestNodeGetter.StaticRegistration,  () => []));
            all.Add(Add(ITalkingActivatorGetter.StaticRegistration,       () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().TalkingActivator().WinningContextOverrides()));
            all.Add(Add(ITextureSetGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().TextureSet().WinningContextOverrides()));
            all.Add(Add(ITreeGetter.StaticRegistration,                   () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Tree().WinningContextOverrides()));
            all.Add(Add(IVisualEffectGetter.StaticRegistration,           () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().VisualEffect().WinningContextOverrides()));
            all.Add(Add(IVoiceTypeGetter.StaticRegistration,              () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().VoiceType().WinningContextOverrides()));
            all.Add(Add(IWaterGetter.StaticRegistration,                  () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Water().WinningContextOverrides()));
            all.Add(Add(IWeaponGetter.StaticRegistration,                 () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weapon().WinningContextOverrides()));
            all.Add(Add(IWeatherGetter.StaticRegistration,                () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Weather().WinningContextOverrides()));
            all.Add(Add(IWordOfPowerGetter.StaticRegistration,            () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().WordOfPower().WinningContextOverrides()));
            all.Add(Add(IWorldspaceGetter.StaticRegistration,             () => Global.State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting().Worldspace().WinningContextOverrides()));
            #pragma warning restore format

            All = all.AsReadOnly();
        }

        public static RecordTypeMapping FindByName (string? value)
            => TryFindByName(value, out var mapping) ? mapping
             : throw new InvalidCastException($"Record Type not found by name: \"{value}\"");

        public static RecordTypeMapping FindByObjectType (object obj)
            => FindByType(obj.GetType());

        public static RecordTypeMapping FindByType (Type type)
                    => TryFindByType(type, out var mapping) ? mapping
             : throw new InvalidCastException($"No Record Type implements: \"{type.Name}\"");

        public static RecordTypeMapping FindByType (RecordType type)
            => TryFindByName(type.Type, out var mapping) ? mapping
             : throw new InvalidCastException($"No Record Type found: \"{type.Type}\"");

        public static bool TryFindByName (string? value, out RecordTypeMapping mapping)
        {
            mapping = default;
            return value != null && ByName.TryGetValue(value, out mapping);
        }

        public static bool TryFindByType (IMajorRecordGetter record, out RecordTypeMapping mapping)
        {
            if (!TryFindByType(record.GetType(), out mapping))
            {
                Global.TraceLogger?.Log(ClassLogCode, "Find Types: Type Unknown");
                return false;
            }

            return true;
        }

        public static bool TryFindByType (Type type, out RecordTypeMapping mapping)
            => ByType.TryGetValue(type, out mapping);

        private static RecordTypeMapping Add (ILoquiRegistration staticRegistration, WinningContextOverridesDelegate getWinningContexts)
        {
            var map = new RecordTypeMapping(staticRegistration, getWinningContexts);
            ByName.Add(map.Type.Type, map);
            if (!map.Type.Type.Equals(map.StaticRegistration.Name, StringComparison.OrdinalIgnoreCase))
                ByName.Add(map.StaticRegistration.Name, map);
            ByType.Add(map.StaticRegistration.GetterType, map);
            ByType.Add(map.StaticRegistration.SetterType, map);

            return map;
        }
    }
}