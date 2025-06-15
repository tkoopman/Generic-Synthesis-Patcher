using System.Reflection;

using GenericSynthesisPatcher.Games.Skyrim.Action;
using GenericSynthesisPatcher.Games.Skyrim.Json.Converters;
using GenericSynthesisPatcher.Games.Universal.Action;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

using Noggog;

namespace GenericSynthesisPatcher.Games.Skyrim
{
    public class SkyrimGame : Universal.BaseGame
    {
        public SkyrimGame (IPatcherState<ISkyrimMod, ISkyrimModGetter> gameState) : base(new(gameState.LoadOrder.Select(m => (IModListingGetter)m.Value)))
        {
            State = gameState;
            SerializerSettings.Converters.Add(new ObjectBoundsConverter());
            IgnoreSubPropertiesOnTypes.Add(
                [
                    typeof(AMagicEffectArchetype),
                    typeof(Cell),
                    typeof(CellMaxHeightData),
                    typeof(DialogResponsesAdapter),
                    typeof(FaceFxPhonemes),
                    typeof(Landscape),
                    typeof(LocationTargetRadius),
                    typeof(Model),
                    typeof(ObjectBounds),
                    typeof(PackageAdapter),
                    typeof(PerkAdapter),
                    typeof(QuestAdapter),
                    typeof(RegionGrasses),
                    typeof(RegionLand),
                    typeof(RegionMap),
                    typeof(RegionObjects),
                    typeof(RegionSounds),
                    typeof(RegionWeather),
                    typeof(SceneAdapter),
                    typeof(VirtualMachineAdapter),
                    typeof(WorldspaceMaxHeight),
                ]);

            addExactMatch(typeof(WorldspaceMaxHeight), WorldspaceMaxHeightAction.Instance);
            addExactMatch(typeof(CellMaxHeightData), CellMaxHeightDataAction.Instance);
            addExactMatch(typeof(Model), ModelAction.Instance);

            #region Aliases

#pragma warning disable format

            // Global Aliases
            AddAlias(null                                            , "DESC"       , nameof(IAmmunitionGetter.Description));
            AddAlias(null                                            , "EAMT"       , nameof(IArmorGetter.EnchantmentAmount));
            AddAlias(null                                            , "EDID"       , nameof(IArmorGetter.EditorID));
            AddAlias(null                                            , "EITM"       , nameof(IArmorGetter.ObjectEffect));
            AddAlias(null                                            , "ETYP"       , nameof(IWeaponGetter.EquipmentType));
            AddAlias(null                                            , "FULL"       , nameof(INamedGetter.Name));
            AddAlias(null                                            , "Item"       , nameof(IContainerGetter.Items));
            AddAlias(null                                            , "KWDA"       , nameof(IKeywordedGetter.Keywords));
            AddAlias(null                                            , "OBND"       , nameof(IArmorGetter.ObjectBounds));
            AddAlias(null                                            , "ONAM"       , nameof(INpcGetter.ShortName));
            AddAlias(null                                            , "RecordFlags", nameof(IAmmunitionGetter.MajorFlags));
            AddAlias(null                                            , "RNAM"       , nameof(INpcGetter.Race));
            AddAlias(null                                            , "VMAD"       , nameof(IArmorGetter.VirtualMachineAdapter));
            AddAlias(null                                            , "XCWT"       , nameof(ICellGetter.Water));
            AddAlias(null                                            , "XEZN"       , nameof(ICellGetter.EncounterZone));
            AddAlias(null                                            , "XLCN"       , nameof(ICellGetter.Location));
            AddAlias(null                                            , "YNAM"       , nameof(IAmmunitionGetter.PickUpSound));
            AddAlias(null                                            , "ZNAM"       , nameof(IAmmunitionGetter.PutDownSound));

            // Record specific aliases
            AddAlias(IAcousticSpaceGetter.StaticRegistration         , "BNAM"       , nameof(IAcousticSpaceGetter.EnvironmentType));
            AddAlias(IAcousticSpaceGetter.StaticRegistration         , "RDAT"       , nameof(IAcousticSpaceGetter.UseSoundFromRegion));
            AddAlias(IAcousticSpaceGetter.StaticRegistration         , "SNAM"       , nameof(IAcousticSpaceGetter.AmbientSound));

            AddAlias(IActionRecordGetter.StaticRegistration          , "CNAM"       , nameof(IActionRecordGetter.Color));

            AddAlias(IActivatorGetter.StaticRegistration             , "FNAM"       , nameof(IActivatorGetter.Flags));
            AddAlias(IActivatorGetter.StaticRegistration             , "KNAM"       , nameof(IActivatorGetter.InteractionKeyword));
            AddAlias(IActivatorGetter.StaticRegistration             , "PNAM"       , nameof(IActivatorGetter.MarkerColor));
            AddAlias(IActivatorGetter.StaticRegistration             , "RNAM"       , nameof(IActivatorGetter.ActivateTextOverride));
            AddAlias(IActivatorGetter.StaticRegistration             , "SNAM"       , nameof(IActivatorGetter.LoopingSound));
            AddAlias(IActivatorGetter.StaticRegistration             , "VNAM"       , nameof(IActivatorGetter.ActivationSound));
            AddAlias(IActivatorGetter.StaticRegistration             , "WNAM"       , nameof(IActivatorGetter.WaterType));

            AddAlias(IActorValueInformationGetter.StaticRegistration , "ANAM"       , nameof(IActorValueInformationGetter.Abbreviation));
            AddAlias(IActorValueInformationGetter.StaticRegistration , "AVSK"       , nameof(IActorValueInformationGetter.Skill));

            AddAlias(IAddonNodeGetter.StaticRegistration             , "DATA"       , nameof(IAddonNodeGetter.NodeIndex));
            AddAlias(IAddonNodeGetter.StaticRegistration             , "SNAM"       , nameof(IAddonNodeGetter.Sound));

            AddAlias(IAlchemicalApparatusGetter.StaticRegistration   , "QUAL"       , nameof(IAlchemicalApparatusGetter.Quality));

            AddAlias(IAmmunitionGetter.StaticRegistration            , "DMG"        , nameof(IAmmunitionGetter.Damage));

            AddAlias(IAnimatedObjectGetter.StaticRegistration        , "BNAM"       , nameof(IAnimatedObjectGetter.UnloadEvent));

            AddAlias(IArmorAddonGetter.StaticRegistration            , "BODT"       , nameof(IArmorAddonGetter.BodyTemplate));
            AddAlias(IArmorAddonGetter.StaticRegistration            , "MODL"       , nameof(IArmorAddonGetter.AdditionalRaces));
            AddAlias(IArmorAddonGetter.StaticRegistration            , "NAM0"       , "SkinTexture.Male");
            AddAlias(IArmorAddonGetter.StaticRegistration            , "NAM1"       , "SkinTexture.Female");
            AddAlias(IArmorAddonGetter.StaticRegistration            , "NAM2"       , "TextureSwapList.Male");
            AddAlias(IArmorAddonGetter.StaticRegistration            , "NAM3"       , "TextureSwapList.Female");
            AddAlias(IArmorAddonGetter.StaticRegistration            , "ONAM"       , nameof(IArmorAddonGetter.ArtObject));
            AddAlias(IArmorAddonGetter.StaticRegistration            , "SNDD"       , nameof(IArmorAddonGetter.FootstepSound));

            AddAlias(IArmorGetter.StaticRegistration                 , "BAMT"       , nameof(IArmorGetter.AlternateBlockMaterial));
            AddAlias(IArmorGetter.StaticRegistration                 , "BIDS"       , nameof(IArmorGetter.BashImpactDataSet));
            AddAlias(IArmorGetter.StaticRegistration                 , "BMCT"       , nameof(IArmorGetter.RagdollConstraintTemplate));
            AddAlias(IArmorGetter.StaticRegistration                 , "BOD2"       , nameof(IArmorGetter.BodyTemplate));
            AddAlias(IArmorGetter.StaticRegistration                 , "DNAM"       , nameof(IArmorGetter.ArmorRating));
            AddAlias(IArmorGetter.StaticRegistration                 , "MODL"       , nameof(IArmorGetter.Armature));
            AddAlias(IArmorGetter.StaticRegistration                 , "TNAM"       , nameof(IArmorGetter.TemplateArmor));

            AddAlias(IArtObjectGetter.StaticRegistration             , "DNAM"       , nameof(IArtObjectGetter.Type));

            AddAlias(IAssociationTypeGetter.StaticRegistration       , "DATA"       , nameof(IAssociationTypeGetter.IsFamily));
            AddAlias(IAssociationTypeGetter.StaticRegistration       , "FCHT"       , "Title.Female");
            AddAlias(IAssociationTypeGetter.StaticRegistration       , "FPRT"       , "ParentTitle.Female");
            AddAlias(IAssociationTypeGetter.StaticRegistration       , "MCHT"       , "Title.Male");
            AddAlias(IAssociationTypeGetter.StaticRegistration       , "MPRT"       , "ParentTitle.Male");

            AddAlias(IBookGetter.StaticRegistration                  , "CNAM"       , nameof(IBookGetter.Description));
            AddAlias(IBookGetter.StaticRegistration                  , "DESC"       , nameof(IBookGetter.BookText));
            AddAlias(IBookGetter.StaticRegistration                  , "INAM"       , nameof(IBookGetter.InventoryArt));

            AddAlias(ICameraPathGetter.StaticRegistration            , "ANAM"       , nameof(ICameraPathGetter.RelatedPaths));
            AddAlias(ICameraPathGetter.StaticRegistration            , "DATA"       , nameof(ICameraPathGetter.Zoom));
            AddAlias(ICameraPathGetter.StaticRegistration            , "SNAM"       , nameof(ICameraPathGetter.Shots));
            AddAlias(ICameraShotGetter.StaticRegistration            , "MNAM"       , nameof(ICameraShotGetter.ImageSpaceModifier));

            AddAlias(ICellGetter.StaticRegistration                  , "DATA"       , nameof(ICellGetter.Flags));
            AddAlias(ICellGetter.StaticRegistration                  , "LTMP"       , nameof(ICellGetter.LightingTemplate));
            AddAlias(ICellGetter.StaticRegistration                  , "MHDT"       , nameof(ICellGetter.MaxHeightData));
            AddAlias(ICellGetter.StaticRegistration                  , "TVDT"       , nameof(ICellGetter.OcclusionData));
            AddAlias(ICellGetter.StaticRegistration                  , "XCAS"       , nameof(ICellGetter.AcousticSpace));
            AddAlias(ICellGetter.StaticRegistration                  , "XCCM"       , nameof(ICellGetter.SkyAndWeatherFromRegion));
            AddAlias(ICellGetter.StaticRegistration                  , "XCIM"       , nameof(ICellGetter.ImageSpace));
            AddAlias(ICellGetter.StaticRegistration                  , "XCLL"       , nameof(ICellGetter.Lighting));
            AddAlias(ICellGetter.StaticRegistration                  , "XCLR"       , nameof(ICellGetter.Regions));
            AddAlias(ICellGetter.StaticRegistration                  , "XCLW"       , nameof(ICellGetter.WaterHeight));
            AddAlias(ICellGetter.StaticRegistration                  , "XCLX"       , nameof(ICellGetter.Grid));
            AddAlias(ICellGetter.StaticRegistration                  , "XCMO"       , nameof(ICellGetter.Music));
            AddAlias(ICellGetter.StaticRegistration                  , "XILL"       , nameof(ICellGetter.LockList));
            AddAlias(ICellGetter.StaticRegistration                  , "XNAM"       , nameof(ICellGetter.WaterNoiseTexture));
            AddAlias(ICellGetter.StaticRegistration                  , "XOWN"       , nameof(ICellGetter.Owner));
            AddAlias(ICellGetter.StaticRegistration                  , "XRNK"       , nameof(ICellGetter.FactionRank));
            AddAlias(ICellGetter.StaticRegistration                  , "XWEM"       , nameof(ICellGetter.WaterEnvironmentMap));

            AddAlias(IClimateGetter.StaticRegistration               , "FNAM"       , nameof(IClimateGetter.SunTexture));
            AddAlias(IClimateGetter.StaticRegistration               , "GNAM"       , nameof(IClimateGetter.SunGlareTexture));
            AddAlias(IClimateGetter.StaticRegistration               , "WLST"       , nameof(IClimateGetter.WeatherTypes));

            AddAlias(ICollisionLayerGetter.StaticRegistration        , "BNAM"       , nameof(ICollisionLayerGetter.Index));
            AddAlias(ICollisionLayerGetter.StaticRegistration        , "CNAM"       , nameof(ICollisionLayerGetter.CollidesWith));
            AddAlias(ICollisionLayerGetter.StaticRegistration        , "FNAM"       , nameof(ICollisionLayerGetter.DebugColor));
            AddAlias(ICollisionLayerGetter.StaticRegistration        , "GNAM"       , nameof(ICollisionLayerGetter.Flags));
            AddAlias(ICollisionLayerGetter.StaticRegistration        , "MNAM"       , nameof(ICollisionLayerGetter.Name));

            AddAlias(IColorRecordGetter.StaticRegistration           , "CNAM"       , nameof(IColorRecordGetter.Color));
            AddAlias(IColorRecordGetter.StaticRegistration           , "FNAM"       , nameof(IColorRecordGetter.Playable));

            AddAlias(ICombatStyleGetter.StaticRegistration           , "CSCR"       , nameof(ICombatStyleGetter.CloseRange));
            AddAlias(ICombatStyleGetter.StaticRegistration           , "CSFL"       , nameof(ICombatStyleGetter.Flight));
            AddAlias(ICombatStyleGetter.StaticRegistration           , "CSLR"       , nameof(ICombatStyleGetter.LongRangeStrafeMult));
            AddAlias(ICombatStyleGetter.StaticRegistration           , "CSME"       , nameof(ICombatStyleGetter.Melee));
            AddAlias(ICombatStyleGetter.StaticRegistration           , "DATA"       , nameof(ICombatStyleGetter.Flags));

            AddAlias(IConstructibleObjectGetter.StaticRegistration   , "BNAM"       , nameof(IConstructibleObjectGetter.WorkbenchKeyword));
            AddAlias(IConstructibleObjectGetter.StaticRegistration   , "CNAM"       , nameof(IConstructibleObjectGetter.CreatedObject));
            AddAlias(IConstructibleObjectGetter.StaticRegistration   , "NAM1"       , nameof(IConstructibleObjectGetter.CreatedObjectCount));

            AddAlias(IContainerGetter.StaticRegistration             , "QNAM"       , nameof(IContainerGetter.CloseSound));
            AddAlias(IContainerGetter.StaticRegistration             , "SNAM"       , nameof(IContainerGetter.OpenSound));

            AddAlias(IDefaultObjectManagerGetter.StaticRegistration  , "DNAM"       , nameof(IDefaultObjectManagerGetter.Objects));

            AddAlias(IDialogBranchGetter.StaticRegistration          , "DNAM"       , nameof(IDialogBranchGetter.Flags));
            AddAlias(IDialogBranchGetter.StaticRegistration          , "QNAM"       , nameof(IDialogBranchGetter.Quest));
            AddAlias(IDialogBranchGetter.StaticRegistration          , "SNAM"       , nameof(IDialogBranchGetter.StartingTopic));
            AddAlias(IDialogBranchGetter.StaticRegistration          , "TNAM"       , nameof(IDialogBranchGetter.Category));

            AddAlias(IDialogTopicGetter.StaticRegistration           , "BNAM"       , nameof(IDialogTopicGetter.Branch));
            AddAlias(IDialogTopicGetter.StaticRegistration           , "PNAM"       , nameof(IDialogTopicGetter.Priority));
            AddAlias(IDialogTopicGetter.StaticRegistration           , "QNAM"       , nameof(IDialogTopicGetter.Quest));
            AddAlias(IDialogTopicGetter.StaticRegistration           , "SNAM"       , nameof(IDialogTopicGetter.Subtype));

            AddAlias(IDialogViewGetter.StaticRegistration            , "BNAM"       , nameof(IDialogViewGetter.Branches));
            AddAlias(IDialogViewGetter.StaticRegistration            , "QNAM"       , nameof(IDialogViewGetter.Quest));

            AddAlias(IDoorGetter.StaticRegistration                  , "ANAM"       , nameof(IDoorGetter.CloseSound));
            AddAlias(IDoorGetter.StaticRegistration                  , "BNAM"       , nameof(IDoorGetter.LoopSound));
            AddAlias(IDoorGetter.StaticRegistration                  , "FNAM"       , nameof(IDoorGetter.Flags));
            AddAlias(IDoorGetter.StaticRegistration                  , "SNAM"       , nameof(IDoorGetter.OpenSound));

            AddAlias(IEffectShaderGetter.StaticRegistration          , "ICO2"       , nameof(IEffectShaderGetter.ParticleShaderTexture));
            AddAlias(IEffectShaderGetter.StaticRegistration          , "ICON"       , nameof(IEffectShaderGetter.FillTexture));
            AddAlias(IEffectShaderGetter.StaticRegistration          , "NAM7"       , nameof(IEffectShaderGetter.HolesTexture));
            AddAlias(IEffectShaderGetter.StaticRegistration          , "NAM8"       , nameof(IEffectShaderGetter.MembranePaletteTexture));
            AddAlias(IEffectShaderGetter.StaticRegistration          , "NAM9"       , nameof(IEffectShaderGetter.ParticlePaletteTexture));

            AddAlias(IEquipTypeGetter.StaticRegistration             , "DATA"       , nameof(IEquipTypeGetter.UseAllParents));
            AddAlias(IEquipTypeGetter.StaticRegistration             , "PNAM"       , nameof(IEquipTypeGetter.SlotParents));

            AddAlias(IExplosionGetter.StaticRegistration             , "MNAM"       , nameof(IExplosionGetter.ImageSpaceModifier));

            AddAlias(IEyesGetter.StaticRegistration                  , "DATA"       , nameof(IEyesGetter.Flags));
            AddAlias(IEyesGetter.StaticRegistration                  , "Texture"    , nameof(IEyesGetter.Icon));

            AddAlias(IFactionGetter.StaticRegistration               , "CRGR"       , nameof(IFactionGetter.SharedCrimeFactionList));
            AddAlias(IFactionGetter.StaticRegistration               , "CRVA"       , nameof(IFactionGetter.CrimeValues));
            AddAlias(IFactionGetter.StaticRegistration               , "DATA"       , nameof(IFactionGetter.Flags));
            AddAlias(IFactionGetter.StaticRegistration               , "JAIL"       , nameof(IFactionGetter.ExteriorJailMarker));
            AddAlias(IFactionGetter.StaticRegistration               , "JOUT"       , nameof(IFactionGetter.JailOutfit));
            AddAlias(IFactionGetter.StaticRegistration               , "PLCN"       , nameof(IFactionGetter.PlayerInventoryContainer));
            AddAlias(IFactionGetter.StaticRegistration               , "PLVD"       , nameof(IFactionGetter.VendorLocation));
            AddAlias(IFactionGetter.StaticRegistration               , "STOL"       , nameof(IFactionGetter.StolenGoodsContainer));
            AddAlias(IFactionGetter.StaticRegistration               , "VENC"       , nameof(IFactionGetter.MerchantContainer));
            AddAlias(IFactionGetter.StaticRegistration               , "VEND"       , nameof(IFactionGetter.VendorBuySellList));
            AddAlias(IFactionGetter.StaticRegistration               , "VENV"       , nameof(IFactionGetter.VendorValues));
            AddAlias(IFactionGetter.StaticRegistration               , "WAIT"       , nameof(IFactionGetter.FollowerWaitMarker));
            AddAlias(IFactionGetter.StaticRegistration               , "XNAM"       , nameof(IFactionGetter.Relations));

            AddAlias(IFloraGetter.StaticRegistration                 , "PFIG"       , nameof(IFloraGetter.Ingredient));
            AddAlias(IFloraGetter.StaticRegistration                 , "PFPC"       , nameof(IFloraGetter.Production));
            AddAlias(IFloraGetter.StaticRegistration                 , "RNAM"       , nameof(IFloraGetter.ActivateTextOverride));
            AddAlias(IFloraGetter.StaticRegistration                 , "SNAM"       , nameof(IFloraGetter.HarvestSound));

            AddAlias(IFootstepGetter.StaticRegistration              , "ANAM"       , nameof(IFootstepGetter.Tag));
            AddAlias(IFootstepGetter.StaticRegistration              , "DATA"       , nameof(IFootstepGetter.ImpactDataSet));

            AddAlias(IFormListGetter.StaticRegistration              , "FormID"     , nameof(IFormListGetter.Items));
            AddAlias(IFormListGetter.StaticRegistration              , "FormIDs"    , nameof(IFormListGetter.Items));
            AddAlias(IFormListGetter.StaticRegistration              , "LNAM"       , nameof(IFormListGetter.Items));

            AddAlias(IFurnitureGetter.StaticRegistration             , "FNAM"       , nameof(IFurnitureGetter.Flags));
            AddAlias(IFurnitureGetter.StaticRegistration             , "KNAM"       , nameof(IFurnitureGetter.InteractionKeyword));
            AddAlias(IFurnitureGetter.StaticRegistration             , "MNAM"       , nameof(IFurnitureGetter.Flags));
            AddAlias(IFurnitureGetter.StaticRegistration             , "NAM1"       , nameof(IFurnitureGetter.AssociatedSpell));
            AddAlias(IFurnitureGetter.StaticRegistration             , "WBDT"       , nameof(IFurnitureGetter.WorkbenchData));
            AddAlias(IFurnitureGetter.StaticRegistration             , "XMRK"       , nameof(IFurnitureGetter.ModelFilename));

            AddAlias(IGlobalGetter.StaticRegistration                , "FLTV"       , nameof(IGlobalFloatGetter.Data));
            AddAlias(IGlobalGetter.StaticRegistration                , "FNAM"       , nameof(IGlobalGetter.Type));

            AddAlias(IHazardGetter.StaticRegistration                , "MNAM"       , nameof(IHazardGetter.ImageSpaceModifier));

            AddAlias(IHeadPartGetter.StaticRegistration              , "CNAM"       , nameof(IHeadPartGetter.Color));
            AddAlias(IHeadPartGetter.StaticRegistration              , "DATA"       , nameof(IHeadPartGetter.Flags));
            AddAlias(IHeadPartGetter.StaticRegistration              , "HNAM"       , nameof(IHeadPartGetter.ExtraParts));
            AddAlias(IHeadPartGetter.StaticRegistration              , "PNAM"       , nameof(IHeadPartGetter.Type));
            AddAlias(IHeadPartGetter.StaticRegistration              , "RNAM"       , nameof(IHeadPartGetter.ValidRaces));
            AddAlias(IHeadPartGetter.StaticRegistration              , "TNAM"       , nameof(IHeadPartGetter.TextureSet));

            AddAlias(IIdleAnimationGetter.StaticRegistration         , "ANAM"       , nameof(IIdleAnimationGetter.RelatedIdles));
            AddAlias(IIdleAnimationGetter.StaticRegistration         , "DNAM"       , nameof(IIdleAnimationGetter.Filename));
            AddAlias(IIdleAnimationGetter.StaticRegistration         , "ENAM"       , nameof(IIdleAnimationGetter.AnimationEvent));

            AddAlias(IIdleMarkerGetter.StaticRegistration            , "IDLA"       , nameof(IIdleMarkerGetter.Animations));
            AddAlias(IIdleMarkerGetter.StaticRegistration            , "IDLF"       , nameof(IIdleMarkerGetter.Flags));
            AddAlias(IIdleMarkerGetter.StaticRegistration            , "IDLT"       , nameof(IIdleMarkerGetter.IdleTimer));

            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "BNAM"       , nameof(IImageSpaceAdapterGetter.BlurRadius));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "NAM1"       , nameof(IImageSpaceAdapterGetter.RadialBlurRampDown));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "NAM2"       , nameof(IImageSpaceAdapterGetter.RadialBlurDownStart));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "NAM3"       , nameof(IImageSpaceAdapterGetter.FadeColor));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "NAM4"       , nameof(IImageSpaceAdapterGetter.MotionBlurStrength));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "RNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurStrength));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "SNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurRampUp));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "TNAM"       , nameof(IImageSpaceAdapterGetter.TintColor));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "UNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurStart));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "VNAM"       , nameof(IImageSpaceAdapterGetter.DoubleVisionStrength));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "WNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldStrength));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "YNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldDistance));
            AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "ZNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldRange));

            AddAlias(IImageSpaceGetter.StaticRegistration            , "CNAM"       , nameof(IImageSpaceGetter.Cinematic));
            AddAlias(IImageSpaceGetter.StaticRegistration            , "DNAM"       , nameof(IImageSpaceGetter.DepthOfField));
            AddAlias(IImageSpaceGetter.StaticRegistration            , "HNAM"       , nameof(IImageSpaceGetter.Hdr));
            AddAlias(IImageSpaceGetter.StaticRegistration            , "TNAM"       , nameof(IImageSpaceGetter.Tint));

            AddAlias(IImpactGetter.StaticRegistration                , "DNAM"       , nameof(IImpactGetter.TextureSet));
            AddAlias(IImpactGetter.StaticRegistration                , "DODT"       , nameof(IImpactGetter.Decal));
            AddAlias(IImpactGetter.StaticRegistration                , "ENAM"       , nameof(IImpactGetter.SecondaryTextureSet));
            AddAlias(IImpactGetter.StaticRegistration                , "NAM1"       , nameof(IImpactGetter.Sound2));
            AddAlias(IImpactGetter.StaticRegistration                , "NAM2"       , nameof(IImpactGetter.Hazard));
            AddAlias(IImpactGetter.StaticRegistration                , "SNAM"       , nameof(IImpactGetter.Sound1));

            AddAlias(IIngestibleGetter.StaticRegistration            , "DATA"       , nameof(IIngestibleGetter.Weight));
            AddAlias(IIngredientGetter.StaticRegistration            , "ETYP"       , nameof(IIngredientGetter.EquipType));

            AddAlias(IKeywordGetter.StaticRegistration               , "CNAM"       , nameof(IKeywordGetter.Color));

            AddAlias(ILandscapeTextureGetter.StaticRegistration      , "GNAM"       , nameof(ILandscapeTextureGetter.Grasses));
            AddAlias(ILandscapeTextureGetter.StaticRegistration      , "INAM"       , nameof(ILandscapeTextureGetter.Flags));
            AddAlias(ILandscapeTextureGetter.StaticRegistration      , "MNAM"       , nameof(ILandscapeTextureGetter.MaterialType));
            AddAlias(ILandscapeTextureGetter.StaticRegistration      , "SNAM"       , nameof(ILandscapeTextureGetter.TextureSpecularExponent));
            AddAlias(ILandscapeTextureGetter.StaticRegistration      , "TNAM"       , nameof(ILandscapeTextureGetter.TextureSet));

            AddAlias(ILeveledItemGetter.StaticRegistration           , "LVLD"       , nameof(ILeveledItemGetter.ChanceNone));
            AddAlias(ILeveledItemGetter.StaticRegistration           , "LVLF"       , nameof(ILeveledItemGetter.Flags));
            AddAlias(ILeveledItemGetter.StaticRegistration           , "LVLG"       , nameof(ILeveledItemGetter.Global));

            AddAlias(ILeveledNpcGetter.StaticRegistration            , "LVLD"       , nameof(ILeveledNpcGetter.ChanceNone));
            AddAlias(ILeveledNpcGetter.StaticRegistration            , "LVLF"       , nameof(ILeveledNpcGetter.Flags));
            AddAlias(ILeveledNpcGetter.StaticRegistration            , "LVLG"       , nameof(ILeveledNpcGetter.Global));

            AddAlias(ILeveledSpellGetter.StaticRegistration          , "LVLD"       , nameof(ILeveledSpellGetter.ChanceNone));
            AddAlias(ILeveledSpellGetter.StaticRegistration          , "LVLF"       , nameof(ILeveledSpellGetter.Flags));

            AddAlias(ILightGetter.StaticRegistration                 , "FNAM"       , nameof(ILightGetter.FadeValue));
            AddAlias(ILightGetter.StaticRegistration                 , "LNAM"       , nameof(ILightGetter.Lens));
            AddAlias(ILightGetter.StaticRegistration                 , "SNAM"       , nameof(ILightGetter.Sound));

            AddAlias(ILightingTemplateGetter.StaticRegistration      , "DALC"       , nameof(ILightingTemplateGetter.DirectionalAmbientColors));

            AddAlias(ILoadScreenGetter.StaticRegistration            , "MOD2"       , nameof(ILoadScreenGetter.CameraPath));
            AddAlias(ILoadScreenGetter.StaticRegistration            , "NNAM"       , nameof(ILoadScreenGetter.LoadingScreenNif));
            AddAlias(ILoadScreenGetter.StaticRegistration            , "ONAM"       , nameof(ILoadScreenGetter.RotationOffsetConstraints));
            AddAlias(ILoadScreenGetter.StaticRegistration            , "RNAM"       , nameof(ILoadScreenGetter.InitialRotation));
            AddAlias(ILoadScreenGetter.StaticRegistration            , "SNAM"       , nameof(ILoadScreenGetter.InitialScale));
            AddAlias(ILoadScreenGetter.StaticRegistration            , "XNAM"       , nameof(ILoadScreenGetter.InitialTranslationOffset));

            AddAlias(ILocationGetter.StaticRegistration              , "CNAM"       , nameof(ILocationGetter.Color));
            AddAlias(ILocationGetter.StaticRegistration              , "FNAM"       , nameof(ILocationGetter.UnreportedCrimeFaction));
            AddAlias(ILocationGetter.StaticRegistration              , "MNAM"       , nameof(ILocationGetter.WorldLocationMarkerRef));
            AddAlias(ILocationGetter.StaticRegistration              , "NAM0"       , nameof(ILocationGetter.HorseMarkerRef));
            AddAlias(ILocationGetter.StaticRegistration              , "NAM1"       , nameof(ILocationGetter.Music));
            AddAlias(ILocationGetter.StaticRegistration              , "PNAM"       , nameof(ILocationGetter.ParentLocation));
            AddAlias(ILocationGetter.StaticRegistration              , "RNAM"       , nameof(ILocationGetter.WorldLocationRadius));

            AddAlias(ILocationReferenceTypeGetter.StaticRegistration , "CNAM"       , nameof(ILocationReferenceTypeGetter.Color));

            AddAlias(IMagicEffectGetter.StaticRegistration           , "DNAM"       , nameof(IMagicEffectGetter.Description));
            AddAlias(IMagicEffectGetter.StaticRegistration           , "MDOB"       , nameof(IMagicEffectGetter.MenuDisplayObject));
            AddAlias(IMagicEffectGetter.StaticRegistration           , "SNDD"       , nameof(IMagicEffectGetter.Sounds));

            AddAlias(IMaterialTypeGetter.StaticRegistration          , "BNAM"       , nameof(IMaterialTypeGetter.Buoyancy));
            AddAlias(IMaterialTypeGetter.StaticRegistration          , "CNAM"       , nameof(IMaterialTypeGetter.HavokDisplayColor));
            AddAlias(IMaterialTypeGetter.StaticRegistration          , "FNAM"       , nameof(IMaterialTypeGetter.Flags));
            AddAlias(IMaterialTypeGetter.StaticRegistration          , "HNAM"       , nameof(IMaterialTypeGetter.HavokImpactDataSet));
            AddAlias(IMaterialTypeGetter.StaticRegistration          , "MNAM"       , nameof(IMaterialTypeGetter.Name));
            AddAlias(IMaterialTypeGetter.StaticRegistration          , "PNAM"       , nameof(IMaterialTypeGetter.Parent));

            AddAlias(IMessageGetter.StaticRegistration               , "DNAM"       , nameof(IMessageGetter.Flags));
            AddAlias(IMessageGetter.StaticRegistration               , "QNAM"       , nameof(IMessageGetter.Quest));
            AddAlias(IMessageGetter.StaticRegistration               , "TNAM"       , nameof(IMessageGetter.DisplayTime));

            AddAlias(IMiscItemGetter.StaticRegistration              , "ICON"       , "Icons.LargeIconFilename");
            AddAlias(IMiscItemGetter.StaticRegistration              , "MICO"       , "Icons.SmallIconFilename");

            AddAlias(IMoveableStaticGetter.StaticRegistration        , "DATA"       , nameof(IMoveableStaticGetter.Flags));
            AddAlias(IMoveableStaticGetter.StaticRegistration        , "SNAM"       , nameof(IMoveableStaticGetter.LoopingSound));

            AddAlias(IMovementTypeGetter.StaticRegistration          , "FULL"       , null);
            AddAlias(IMovementTypeGetter.StaticRegistration          , "INAM"       , nameof(IMovementTypeGetter.AnimationChangeThresholds));
            AddAlias(IMovementTypeGetter.StaticRegistration          , "MNAM"       , nameof(IMovementTypeGetter.Name));

            AddAlias(IMusicTrackGetter.StaticRegistration            , "ANAM"       , nameof(IMusicTrackGetter.TrackFilename));
            AddAlias(IMusicTrackGetter.StaticRegistration            , "BNAM"       , nameof(IMusicTrackGetter.FinaleFilename));
            AddAlias(IMusicTrackGetter.StaticRegistration            , "CNAM"       , nameof(IMusicTrackGetter.Type));
            AddAlias(IMusicTrackGetter.StaticRegistration            , "DNAM"       , nameof(IMusicTrackGetter.FadeOut));
            AddAlias(IMusicTrackGetter.StaticRegistration            , "FLTV"       , nameof(IMusicTrackGetter.Duration));
            AddAlias(IMusicTrackGetter.StaticRegistration            , "FNAM"       , nameof(IMusicTrackGetter.CuePoints));
            AddAlias(IMusicTrackGetter.StaticRegistration            , "LNAM"       , nameof(IMusicTrackGetter.LoopData));
            AddAlias(IMusicTrackGetter.StaticRegistration            , "SNAM"       , nameof(IMusicTrackGetter.Tracks));

            AddAlias(IMusicTypeGetter.StaticRegistration             , "FNAM"       , nameof(IMusicTypeGetter.Flags));
            AddAlias(IMusicTypeGetter.StaticRegistration             , "TNAM"       , nameof(IMusicTypeGetter.Tracks));
            AddAlias(IMusicTypeGetter.StaticRegistration             , "WNAM"       , nameof(IMusicTypeGetter.FadeDuration));

            AddAlias(INpcGetter.StaticRegistration                   , "AIDT"       , nameof(INpcGetter.AIData));
            AddAlias(INpcGetter.StaticRegistration                   , "ANAM"       , nameof(INpcGetter.FarAwayModel));
            AddAlias(INpcGetter.StaticRegistration                   , "ATKR"       , nameof(INpcGetter.AttackRace));
            AddAlias(INpcGetter.StaticRegistration                   , "CNAM"       , nameof(INpcGetter.Class));
            AddAlias(INpcGetter.StaticRegistration                   , "CRIF"       , nameof(INpcGetter.CrimeFaction));
            AddAlias(INpcGetter.StaticRegistration                   , "DNAM"       , nameof(INpcGetter.PlayerSkills));
            AddAlias(INpcGetter.StaticRegistration                   , "DOFT"       , nameof(INpcGetter.DefaultOutfit));
            AddAlias(INpcGetter.StaticRegistration                   , "DPLT"       , nameof(INpcGetter.DefaultPackageList));
            AddAlias(INpcGetter.StaticRegistration                   , "ECOR"       , nameof(INpcGetter.CombatOverridePackageList));
            AddAlias(INpcGetter.StaticRegistration                   , "FTST"       , nameof(INpcGetter.HeadTexture));
            AddAlias(INpcGetter.StaticRegistration                   , "GNAM"       , nameof(INpcGetter.GiftFilter));
            AddAlias(INpcGetter.StaticRegistration                   , "GWOR"       , nameof(INpcGetter.GuardWarnOverridePackageList));
            AddAlias(INpcGetter.StaticRegistration                   , "HCLF"       , nameof(INpcGetter.HairColor));
            AddAlias(INpcGetter.StaticRegistration                   , "INAM"       , nameof(INpcGetter.DeathItem));
            AddAlias(INpcGetter.StaticRegistration                   , "NAM6"       , nameof(INpcGetter.Height));
            AddAlias(INpcGetter.StaticRegistration                   , "NAM7"       , nameof(INpcGetter.Weight));
            AddAlias(INpcGetter.StaticRegistration                   , "NAM8"       , nameof(INpcGetter.SoundLevel));
            AddAlias(INpcGetter.StaticRegistration                   , "NAM9"       , nameof(INpcGetter.FaceMorph));
            AddAlias(INpcGetter.StaticRegistration                   , "NAMA"       , nameof(INpcGetter.FaceParts));
            AddAlias(INpcGetter.StaticRegistration                   , "OCOR"       , nameof(INpcGetter.ObserveDeadBodyOverridePackageList));
            AddAlias(INpcGetter.StaticRegistration                   , "PKID"       , nameof(INpcGetter.Packages));
            AddAlias(INpcGetter.StaticRegistration                   , "PNAM"       , nameof(INpcGetter.HeadParts));
            AddAlias(INpcGetter.StaticRegistration                   , "PRKR"       , nameof(INpcGetter.Perks));
            AddAlias(INpcGetter.StaticRegistration                   , "QNAM"       , nameof(INpcGetter.TextureLighting));
            AddAlias(INpcGetter.StaticRegistration                   , "SHRT"       , nameof(INpcGetter.ShortName));
            AddAlias(INpcGetter.StaticRegistration                   , "SNAM"       , nameof(INpcGetter.Factions));
            AddAlias(INpcGetter.StaticRegistration                   , "SOFT"       , nameof(INpcGetter.SleepingOutfit));
            AddAlias(INpcGetter.StaticRegistration                   , "SPLO"       , nameof(INpcGetter.ActorEffect));
            AddAlias(INpcGetter.StaticRegistration                   , "SPOR"       , nameof(INpcGetter.SpectatorOverridePackageList));
            AddAlias(INpcGetter.StaticRegistration                   , "TPLT"       , nameof(INpcGetter.Template));
            AddAlias(INpcGetter.StaticRegistration                   , "VTCK"       , nameof(INpcGetter.Voice));
            AddAlias(INpcGetter.StaticRegistration                   , "WNAM"       , nameof(INpcGetter.WornArmor));
            AddAlias(INpcGetter.StaticRegistration                   , "ZNAM"       , nameof(INpcGetter.CombatStyle));

            AddAlias(IOutfitGetter.StaticRegistration                , "INAM"       , nameof(IOutfitGetter.Items));

            AddAlias(IPackageGetter.StaticRegistration               , "CNAM"       , nameof(IPackageGetter.CombatStyle));
            AddAlias(IPackageGetter.StaticRegistration               , "QNAM"       , nameof(IPackageGetter.OwnerQuest));
            AddAlias(IPackageGetter.StaticRegistration               , "XNAM"       , nameof(IPackageGetter.XnamMarker));

            AddAlias(IPerkGetter.StaticRegistration                  , "NNAM"       , nameof(IPerkGetter.NextPerk));

            AddAlias(IProjectileGetter.StaticRegistration            , "VNAM"       , nameof(IProjectileGetter.SoundLevel));

            AddAlias(IQuestGetter.StaticRegistration                 , "ANAM"       , nameof(IQuestGetter.NextAliasID));
            AddAlias(IQuestGetter.StaticRegistration                 , "DESC"       , null);
            AddAlias(IQuestGetter.StaticRegistration                 , "ENAM"       , nameof(IQuestGetter.Event));
            AddAlias(IQuestGetter.StaticRegistration                 , "FLTR"       , nameof(IQuestGetter.Filter));
            AddAlias(IQuestGetter.StaticRegistration                 , "NNAM"       , nameof(IQuestGetter.Description));
            AddAlias(IQuestGetter.StaticRegistration                 , "QTGL"       , nameof(IQuestGetter.TextDisplayGlobals));

            AddAlias(IRaceGetter.StaticRegistration                  , "ANAM"       , nameof(IRaceGetter.SkeletalModel));
            AddAlias(IRaceGetter.StaticRegistration                  , "ATKR"       , nameof(IRaceGetter.AttackRace));
            AddAlias(IRaceGetter.StaticRegistration                  , "BOD2"       , nameof(IRaceGetter.BodyTemplate));
            AddAlias(IRaceGetter.StaticRegistration                  , "DNAM"       , nameof(IRaceGetter.DecapitateArmors));
            AddAlias(IRaceGetter.StaticRegistration                  , "ENAM"       , nameof(IRaceGetter.Eyes));
            AddAlias(IRaceGetter.StaticRegistration                  , "FLMV"       , nameof(IRaceGetter.BaseMovementDefaultFly));
            AddAlias(IRaceGetter.StaticRegistration                  , "GNAM"       , nameof(IRaceGetter.BodyPartData));
            AddAlias(IRaceGetter.StaticRegistration                  , "HCLF"       , nameof(IRaceGetter.DefaultHairColors));
            AddAlias(IRaceGetter.StaticRegistration                  , "HNAM"       , nameof(IRaceGetter.Hairs));
            AddAlias(IRaceGetter.StaticRegistration                  , "LNAM"       , nameof(IRaceGetter.CloseLootSound));
            AddAlias(IRaceGetter.StaticRegistration                  , "MTNM"       , nameof(IRaceGetter.MovementTypeNames));
            AddAlias(IRaceGetter.StaticRegistration                  , "NAM4"       , nameof(IRaceGetter.MaterialType));
            AddAlias(IRaceGetter.StaticRegistration                  , "NAM5"       , nameof(IRaceGetter.ImpactDataSet));
            AddAlias(IRaceGetter.StaticRegistration                  , "NAM7"       , nameof(IRaceGetter.DecapitationFX));
            AddAlias(IRaceGetter.StaticRegistration                  , "NAM8"       , nameof(IRaceGetter.MorphRace));
            AddAlias(IRaceGetter.StaticRegistration                  , "ONAM"       , nameof(IRaceGetter.OpenLootSound));
            AddAlias(IRaceGetter.StaticRegistration                  , "PNAM"       , nameof(IRaceGetter.FacegenMainClamp));
            AddAlias(IRaceGetter.StaticRegistration                  , "QNAM"       , nameof(IRaceGetter.EquipmentSlots));
            AddAlias(IRaceGetter.StaticRegistration                  , "RNAM"       , nameof(IRaceGetter.ArmorRace));
            AddAlias(IRaceGetter.StaticRegistration                  , "RNMV"       , nameof(IRaceGetter.BaseMovementDefaultRun));
            AddAlias(IRaceGetter.StaticRegistration                  , "SNMV"       , nameof(IRaceGetter.BaseMovementDefaultSneak));
            AddAlias(IRaceGetter.StaticRegistration                  , "SPMV"       , nameof(IRaceGetter.BaseMovementDefaultSprint));
            AddAlias(IRaceGetter.StaticRegistration                  , "SWMV"       , nameof(IRaceGetter.BaseMovementDefaultSwim));
            AddAlias(IRaceGetter.StaticRegistration                  , "UNAM"       , nameof(IRaceGetter.FacegenFaceClamp));
            AddAlias(IRaceGetter.StaticRegistration                  , "UNES"       , nameof(IRaceGetter.UnarmedEquipSlot));
            AddAlias(IRaceGetter.StaticRegistration                  , "VNAM"       , nameof(IRaceGetter.EquipmentFlags));
            AddAlias(IRaceGetter.StaticRegistration                  , "VTCK"       , nameof(IRaceGetter.Voices));
            AddAlias(IRaceGetter.StaticRegistration                  , "WKMV"       , nameof(IRaceGetter.BaseMovementDefaultWalk));
            AddAlias(IRaceGetter.StaticRegistration                  , "WNAM"       , nameof(IRaceGetter.Skin));

            AddAlias(IRegionGetter.StaticRegistration                , "RCLR"       , nameof(IRegionGetter.MapColor));
            AddAlias(IRegionGetter.StaticRegistration                , "WNAM"       , nameof(IRegionGetter.Worldspace));

            AddAlias(ISceneGetter.StaticRegistration                 , "FNAM"       , nameof(ISceneGetter.Flags));
            AddAlias(ISceneGetter.StaticRegistration                 , "INAM"       , nameof(ISceneGetter.LastActionIndex));
            AddAlias(ISceneGetter.StaticRegistration                 , "PNAM"       , nameof(ISceneGetter.Quest));

            AddAlias(IScrollGetter.StaticRegistration                , "MDOB"       , nameof(IScrollGetter.MenuDisplayObject));

            AddAlias(IShaderParticleGeometryGetter.StaticRegistration, "ICON"       , nameof(IShaderParticleGeometryGetter.ParticleTexture));

            AddAlias(IShoutGetter.StaticRegistration                 , "MDOB"       , nameof(IShoutGetter.MenuDisplayObject));

            AddAlias(ISoulGemGetter.StaticRegistration               , "NAM0"       , nameof(ISoulGemGetter.LinkedTo));
            AddAlias(ISoulGemGetter.StaticRegistration               , "SLCP"       , nameof(ISoulGemGetter.MaximumCapacity));
            AddAlias(ISoulGemGetter.StaticRegistration               , "SOUL"       , nameof(ISoulGemGetter.ContainedSoul));

            AddAlias(ISoundCategoryGetter.StaticRegistration         , "FNAM"       , nameof(ISoundCategoryGetter.Flags));
            AddAlias(ISoundCategoryGetter.StaticRegistration         , "PNAM"       , nameof(ISoundCategoryGetter.Parent));
            AddAlias(ISoundCategoryGetter.StaticRegistration         , "UNAM"       , nameof(ISoundCategoryGetter.DefaultMenuVolume));
            AddAlias(ISoundCategoryGetter.StaticRegistration         , "VNAM"       , nameof(ISoundCategoryGetter.StaticVolumeMultiplier));

            AddAlias(ISoundDescriptorGetter.StaticRegistration       , "CNAM"       , nameof(ISoundDescriptorGetter.Type));
            AddAlias(ISoundDescriptorGetter.StaticRegistration       , "FNAM"       , nameof(ISoundDescriptorGetter.String));
            AddAlias(ISoundDescriptorGetter.StaticRegistration       , "GNAM"       , nameof(ISoundDescriptorGetter.Category));
            AddAlias(ISoundDescriptorGetter.StaticRegistration       , "ONAM"       , nameof(ISoundDescriptorGetter.OutputModel));
            AddAlias(ISoundDescriptorGetter.StaticRegistration       , "SNAM"       , nameof(ISoundDescriptorGetter.AlternateSoundFor));

            AddAlias(ISoundMarkerGetter.StaticRegistration           , "SDSC"       , nameof(ISoundMarkerGetter.SoundDescriptor));

            AddAlias(ISoundOutputModelGetter.StaticRegistration      , "MNAM"       , nameof(ISoundOutputModelGetter.Type));
            AddAlias(ISoundOutputModelGetter.StaticRegistration      , "ONAM"       , nameof(ISoundOutputModelGetter.OutputChannels));

            AddAlias(ISpellGetter.StaticRegistration                 , "MDOB"       , nameof(ISpellGetter.MenuDisplayObject));

            AddAlias(IStoryManagerBranchNodeGetter.StaticRegistration, "DNAM"       , nameof(IStoryManagerBranchNodeGetter.Flags));
            AddAlias(IStoryManagerBranchNodeGetter.StaticRegistration, "PNAM"       , nameof(IStoryManagerBranchNodeGetter.Parent));
            AddAlias(IStoryManagerBranchNodeGetter.StaticRegistration, "SNAM"       , nameof(IStoryManagerBranchNodeGetter.PreviousSibling));
            AddAlias(IStoryManagerBranchNodeGetter.StaticRegistration, "XNAM"       , nameof(IStoryManagerBranchNodeGetter.MaxConcurrentQuests));

            AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "DNAM"       , nameof(IStoryManagerEventNodeGetter.Flags));
            AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "ENAM"       , nameof(IStoryManagerEventNodeGetter.Type));
            AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "PNAM"       , nameof(IStoryManagerEventNodeGetter.Parent));
            AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "SNAM"       , nameof(IStoryManagerEventNodeGetter.PreviousSibling));
            AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "XNAM"       , nameof(IStoryManagerEventNodeGetter.MaxConcurrentQuests));
            AddAlias(IStoryManagerQuestNodeGetter.StaticRegistration , "MNAM"       , nameof(IStoryManagerQuestNodeGetter.MaxNumQuestsToRun));
            AddAlias(IStoryManagerQuestNodeGetter.StaticRegistration , "PNAM"       , nameof(IStoryManagerQuestNodeGetter.Parent));
            AddAlias(IStoryManagerQuestNodeGetter.StaticRegistration , "SNAM"       , nameof(IStoryManagerQuestNodeGetter.PreviousSibling));
            AddAlias(IStoryManagerQuestNodeGetter.StaticRegistration , "XNAM"       , nameof(IStoryManagerQuestNodeGetter.MaxConcurrentQuests));

            AddAlias(ITalkingActivatorGetter.StaticRegistration      , "SNAM"       , nameof(ITalkingActivatorGetter.LoopingSound));
            AddAlias(ITalkingActivatorGetter.StaticRegistration      , "VNAM"       , nameof(ITalkingActivatorGetter.VoiceType));

            AddAlias(ITextureSetGetter.StaticRegistration            , "DNAM"       , nameof(ITextureSetGetter.Flags));
            AddAlias(ITextureSetGetter.StaticRegistration            , "DODT"       , nameof(ITextureSetGetter.Decal));

            AddAlias(ITreeGetter.StaticRegistration                  , "PFIG"       , nameof(ITreeGetter.Ingredient));
            AddAlias(ITreeGetter.StaticRegistration                  , "SNAM"       , nameof(ITreeGetter.HarvestSound));

            AddAlias(IVoiceTypeGetter.StaticRegistration             , "DNAM"       , nameof(IVoiceTypeGetter.Flags));

            AddAlias(IWaterGetter.StaticRegistration                 , "ANAM"       , nameof(IWaterGetter.Opacity));
            AddAlias(IWaterGetter.StaticRegistration                 , "FNAM"       , nameof(IWaterGetter.Flags));
            AddAlias(IWaterGetter.StaticRegistration                 , "INAM"       , nameof(IWaterGetter.ImageSpace));
            AddAlias(IWaterGetter.StaticRegistration                 , "NAM0"       , nameof(IWaterGetter.LinearVelocity));
            AddAlias(IWaterGetter.StaticRegistration                 , "NAM1"       , nameof(IWaterGetter.AngularVelocity));
            AddAlias(IWaterGetter.StaticRegistration                 , "NAM2"       , nameof(IWaterGetter.NoiseLayerOneTexture));
            AddAlias(IWaterGetter.StaticRegistration                 , "NAM3"       , nameof(IWaterGetter.NoiseLayerTwoTexture));
            AddAlias(IWaterGetter.StaticRegistration                 , "NAM4"       , nameof(IWaterGetter.NoiseLayerThreeTexture));
            AddAlias(IWaterGetter.StaticRegistration                 , "NAM5"       , nameof(IWaterGetter.FlowNormalsNoiseTexture));
            AddAlias(IWaterGetter.StaticRegistration                 , "SNAM"       , nameof(IWaterGetter.OpenSound));
            AddAlias(IWaterGetter.StaticRegistration                 , "TNAM"       , nameof(IWaterGetter.Material));
            AddAlias(IWaterGetter.StaticRegistration                 , "XNAM"       , nameof(IWaterGetter.Spell));

            AddAlias(IWeaponGetter.StaticRegistration                , "BAMT"       , nameof(IWeaponGetter.AlternateBlockMaterial));
            AddAlias(IWeaponGetter.StaticRegistration                , "BIDS"       , nameof(IWeaponGetter.BlockBashImpact));
            AddAlias(IWeaponGetter.StaticRegistration                , "CNAM"       , nameof(IWeaponGetter.Template));
            AddAlias(IWeaponGetter.StaticRegistration                , "INAM"       , nameof(IWeaponGetter.ImpactDataSet));
            AddAlias(IWeaponGetter.StaticRegistration                , "NAM7"       , nameof(IWeaponGetter.AttackLoopSound));
            AddAlias(IWeaponGetter.StaticRegistration                , "NAM8"       , nameof(IWeaponGetter.UnequipSound));
            AddAlias(IWeaponGetter.StaticRegistration                , "NAM9"       , nameof(IWeaponGetter.EquipSound));
            AddAlias(IWeaponGetter.StaticRegistration                , "SNAM"       , nameof(IWeaponGetter.AttackSound));
            AddAlias(IWeaponGetter.StaticRegistration                , "TNAM"       , nameof(IWeaponGetter.AttackFailSound));
            AddAlias(IWeaponGetter.StaticRegistration                , "UNAM"       , nameof(IWeaponGetter.IdleSound));
            AddAlias(IWeaponGetter.StaticRegistration                , "VNAM"       , nameof(IWeaponGetter.DetectionSoundLevel));
            AddAlias(IWeaponGetter.StaticRegistration                , "WNAM"       , nameof(IWeaponGetter.FirstPersonModel));
            AddAlias(IWeaponGetter.StaticRegistration                , "XNAM"       , nameof(IWeaponGetter.AttackSound2D));

            AddAlias(IWeatherGetter.StaticRegistration               , "DALC"       , nameof(IWeatherGetter.DirectionalAmbientLightingColors));
            AddAlias(IWeatherGetter.StaticRegistration               , "GNAM"       , nameof(IWeatherGetter.SunGlareLensFlare));
            AddAlias(IWeatherGetter.StaticRegistration               , "HNAM"       , nameof(IWeatherGetter.VolumetricLighting));
            AddAlias(IWeatherGetter.StaticRegistration               , "IMSP"       , nameof(IWeatherGetter.ImageSpaces));
            AddAlias(IWeatherGetter.StaticRegistration               , "MNAM"       , nameof(IWeatherGetter.Precipitation));
            AddAlias(IWeatherGetter.StaticRegistration               , "NNAM"       , nameof(IWeatherGetter.VisualEffect));

            AddAlias(IWordOfPowerGetter.StaticRegistration           , "TNAM"       , nameof(IWordOfPowerGetter.Translation));

            AddAlias(IWorldspaceGetter.StaticRegistration            , "CNAM"       , nameof(IWorldspaceGetter.Climate));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "DATA"       , nameof(IWorldspaceGetter.Flags));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "DNAM"       , nameof(IWorldspaceGetter.LandDefaults));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "ICON"       , nameof(IWorldspaceGetter.MapImage));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "LTMP"       , nameof(IWorldspaceGetter.InteriorLighting));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "MHDT"       , nameof(IWorldspaceGetter.MaxHeight));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "MNAM"       , nameof(IWorldspaceGetter.MapData));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM0"       , nameof(IWorldspaceGetter.ObjectBoundsMin));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM2"       , nameof(IWorldspaceGetter.Water));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM3"       , nameof(IWorldspaceGetter.LodWater));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM4"       , nameof(IWorldspaceGetter.LodWaterHeight));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM9"       , nameof(IWorldspaceGetter.ObjectBoundsMax));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "NAMA"       , nameof(IWorldspaceGetter.DistantLodMultiplier));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "NNAM"       , nameof(IWorldspaceGetter.CanopyShadow));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "OFST"       , nameof(IWorldspaceGetter.OffsetData));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "TNAM"       , nameof(IWorldspaceGetter.HdLodDiffuseTexture));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "UNAM"       , nameof(IWorldspaceGetter.HdLodNormalTexture));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "WCTR"       , nameof(IWorldspaceGetter.FixedDimensionsCenterCell));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "XNAM"       , nameof(IWorldspaceGetter.WaterNoiseTexture));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "XWEM"       , nameof(IWorldspaceGetter.WaterEnvironmentMap));
            AddAlias(IWorldspaceGetter.StaticRegistration            , "ZNAM"       , nameof(IWorldspaceGetter.Music));
#pragma warning restore format

            #endregion Aliases
        }

        public override IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; }

        public override IEnumerable<IModContext<IMajorRecordGetter>> GetRecords (ILoquiRegistration recordType)
        {
            var m = typeof(TypeOptionSolidifierMixIns).GetMethod(recordType.Name, BindingFlags.Public | BindingFlags.Static, [typeof(IEnumerable<IModListing<ISkyrimModGetter>>)]) ?? throw new InvalidOperationException($"No method found for record type {recordType.Name}.");

            object records = m.Invoke(null, [State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting()]) ?? throw new InvalidOperationException($"Failed to call method for record type {recordType.Name}.");

            m = records.GetType().GetMethod("WinningContextOverrides", BindingFlags.Public | BindingFlags.Instance, [typeof(bool)]);
            if (m is null)
            {
                m = records.GetType().GetMethod("WinningContextOverrides", BindingFlags.Public | BindingFlags.Instance, [typeof(ILinkCache), typeof(bool)]);
                return (IEnumerable<IModContext<IMajorRecordGetter>>)(m?.Invoke(records, [State.LinkCache, false]) ?? throw new InvalidOperationException($"Failed to call WinningContextOverrides method found for record type {recordType.Name}."));
            }

            return (IEnumerable<IModContext<IMajorRecordGetter>>)(m.Invoke(records, [false]) ?? throw new InvalidOperationException($"Failed to call WinningContextOverrides method found for record type {recordType.Name}."));
        }

        protected override IRecordAction? discoverAction (Type[] explodedType)
        {
            var result = base.discoverAction(explodedType);
            if (result is not null)
                return result;

            switch (explodedType.Length)
            {
                case 1:
                    if (explodedType[0].IsAssignableTo(typeof(IObjectBoundsGetter)))
                        return ObjectBoundsAction.Instance;

                    if (explodedType[0].IsAssignableTo(typeof(IPlayerSkillsGetter)))
                        return PlayerSkillsAction.Instance;

                    return null;

                case 2:
                    if (explodedType[0] == typeof(ExtendedList<>))
                    {
                        if (explodedType[1].IsAssignableTo(typeof(IContainerEntryGetter)))
                            return ContainerItemsAction.Instance;

                        if (explodedType[1].IsAssignableTo(typeof(IEffectGetter)))
                            return EffectsAction.Instance;

                        if (explodedType[1].IsAssignableTo(typeof(ILeveledItemEntryGetter)))
                            return LeveledItemAction.Instance;

                        if (explodedType[1].IsAssignableTo(typeof(ILeveledNpcEntryGetter)))
                            return LeveledNpcAction.Instance;

                        if (explodedType[1].IsAssignableTo(typeof(ILeveledSpellEntryGetter)))
                            return LeveledSpellAction.Instance;

                        if (explodedType[1].IsAssignableTo(typeof(IRankPlacementGetter)))
                            return RankPlacementAction.Instance;

                        if (explodedType[1].IsAssignableTo(typeof(IRelationGetter)))
                            return RelationsAction.Instance;
                    }

                    return null;

                default:
                    return null;
            }
        }

        protected override IEnumerable<ILoquiRegistration> getRecordTypes ()
        {
            List<ILoquiRegistration> types = [];

            foreach (var method in typeof(TypeOptionSolidifierMixIns).GetMethods())
            {
                if (!method.ReturnType.IsGenericType || method.ReturnType.GenericTypeArguments.Length == 0)
                    continue;

                var returnType = method.ReturnType.GenericTypeArguments[^1];

                var regoProperty = returnType.GetProperty("StaticRegistration");
                if (regoProperty?.GetValue(null) is ILoquiRegistration rego)
                    types.Add(rego);
            }

            return types;
        }
    }
}