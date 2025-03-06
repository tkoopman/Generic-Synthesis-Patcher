using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers
{
    public static partial class RecordPropertyMappings
    {
        private static readonly HashSet<IRecordProperty> propertyAliases = new(comparer: new RecordPropertyComparer());
        private static readonly HashSet<IRecordProperty> propertyMappings = new(comparer: new RecordPropertyComparer());

        static RecordPropertyMappings ()
        {
            populateMappings();
            populateAliases();
        }

        public static IReadOnlyList<PropertyAliasMapping> AllAliases => propertyAliases.Select(a => (PropertyAliasMapping)a).ToList().AsReadOnly();
        public static IReadOnlyList<RecordPropertyMapping> AllRPMs => propertyMappings.Select(a => (RecordPropertyMapping)a).ToList().AsReadOnly();

        public static IReadOnlyList<string> GetAllAliases (Type type, string propertyName)
        {
            var list = propertyAliases.Where(p => p is PropertyAliasMapping pam && (pam.RealPropertyName?.Equals(propertyName, StringComparison.Ordinal) ?? true) && (pam.Type == null || pam.Type == type)).Select(p => (PropertyAliasMapping)p).ToList();

            foreach (var item in list.ToArray().Where(l => l.Type == null && list.Count(i => i.PropertyName == l.PropertyName) > 1))
                _ = list.Remove(item);

            return list.Where(i => i.RealPropertyName is not null).Select(l => l.PropertyName).ToList().AsReadOnly();
        }

        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool TryFind (Type? type, string propertyName, out RecordPropertyMapping rpm)
        {
            if (tryFindMapping(type, propertyName, out rpm))
                return true;

            if (tryFindAlias(type, propertyName, out var pam) && pam.RealPropertyName is not null)
                return tryFindMapping(type, pam.RealPropertyName, out rpm);

            return false;
        }

        internal static bool tryFindMapping (Type? type, string key, out RecordPropertyMapping rpm)
        {
            if (type != null && propertyMappings.TryGetValue(new RecordProperty(type, key), out var _rpm) && _rpm is RecordPropertyMapping _RPM)
            {
                rpm = _RPM;
                return true;
            }
            else if (propertyMappings.TryGetValue(new RecordProperty(key), out var _rpmNoType) && _rpmNoType is RecordPropertyMapping _RPMNoType)
            {
                rpm = _RPMNoType;
                return true;
            }

            rpm = default;
            return false;
        }

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private static void Add (Type? type, string propertyName, IRecordAction action) => _ = propertyMappings.Add(new RecordPropertyMapping(type, propertyName, action));

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private static void AddAlias (Type? type, string propertyName, string? realPropertyName)
        {
            if (type is not null)
            {
                foreach (var prop in propertyAliases.Where(p => p.Type is null && p is PropertyAliasMapping pam && (pam.RealPropertyName?.Equals(realPropertyName, StringComparison.Ordinal) ?? false)).ToArray())
                {
                    if (propertyName.Equals(prop.PropertyName, StringComparison.Ordinal))
                        throw new Exception($"Invalid alias that matches null entry. Type: {type.GetClassName()}. Alias: {propertyName}");

                    _ = propertyAliases.Add(new PropertyAliasMapping(type, prop.PropertyName, null));
                }
            }

            _ = propertyAliases.Add(new PropertyAliasMapping(type, propertyName, realPropertyName));
        }

        private static void populateAliases ()
        {
#pragma warning disable format
            AddAlias(null                                 , "DESC"       , nameof(IAmmunitionGetter.Description));
            AddAlias(null                                 , "EAMT"       , nameof(IArmorGetter.EnchantmentAmount));
            AddAlias(null                                 , "EDID"       , nameof(IArmorGetter.EditorID));
            AddAlias(null                                 , "EITM"       , nameof(IArmorGetter.ObjectEffect));
            AddAlias(null                                 , "ETYP"       , nameof(IWeaponGetter.EquipmentType));
            AddAlias(null                                 , "FULL"       , nameof(INamedGetter.Name));
            AddAlias(null                                 , "Item"       , nameof(IContainerGetter.Items));
            AddAlias(null                                 , "KWDA"       , nameof(IKeywordedGetter.Keywords));
            AddAlias(null                                 , "OBND"       , nameof(IArmorGetter.ObjectBounds));
            AddAlias(null                                 , "ONAM"       , nameof(INpcGetter.ShortName));
            AddAlias(null                                 , "RecordFlags", nameof(IAmmunitionGetter.MajorFlags));
            AddAlias(null                                 , "RNAM"       , nameof(INpcGetter.Race));
            AddAlias(null                                 , "VMAD"       , nameof(IArmorGetter.VirtualMachineAdapter));
            AddAlias(null                                 , "XCWT"       , nameof(ICellGetter.Water));
            AddAlias(null                                 , "XEZN"       , nameof(ICellGetter.EncounterZone));
            AddAlias(null                                 , "XLCN"       , nameof(ICellGetter.Location));
            AddAlias(null                                 , "YNAM"       , nameof(IAmmunitionGetter.PickUpSound));
            AddAlias(null                                 , "ZNAM"       , nameof(IAmmunitionGetter.PutDownSound));

            AddAlias(typeof(IAcousticSpaceGetter)         , "BNAM"       , nameof(IAcousticSpaceGetter.EnvironmentType));
            AddAlias(typeof(IAcousticSpaceGetter)         , "RDAT"       , nameof(IAcousticSpaceGetter.UseSoundFromRegion));
            AddAlias(typeof(IAcousticSpaceGetter)         , "SNAM"       , nameof(IAcousticSpaceGetter.AmbientSound));

            AddAlias(typeof(IActionRecordGetter)          , "CNAM"       , nameof(IActionRecordGetter.Color));

            AddAlias(typeof(IActivatorGetter)             , "FNAM"       , nameof(IActivatorGetter.Flags));
            AddAlias(typeof(IActivatorGetter)             , "KNAM"       , nameof(IActivatorGetter.InteractionKeyword));
            AddAlias(typeof(IActivatorGetter)             , "PNAM"       , nameof(IActivatorGetter.MarkerColor));
            AddAlias(typeof(IActivatorGetter)             , "RNAM"       , nameof(IActivatorGetter.ActivateTextOverride));
            AddAlias(typeof(IActivatorGetter)             , "SNAM"       , nameof(IActivatorGetter.LoopingSound));
            AddAlias(typeof(IActivatorGetter)             , "VNAM"       , nameof(IActivatorGetter.ActivationSound));
            AddAlias(typeof(IActivatorGetter)             , "WNAM"       , nameof(IActivatorGetter.WaterType));

            AddAlias(typeof(IActorValueInformationGetter) , "ANAM"       , nameof(IActorValueInformationGetter.Abbreviation));
            AddAlias(typeof(IActorValueInformationGetter) , "AVSK"       , nameof(IActorValueInformationGetter.Skill));

            AddAlias(typeof(IAddonNodeGetter)             , "DATA"       , nameof(IAddonNodeGetter.NodeIndex));
            AddAlias(typeof(IAddonNodeGetter)             , "SNAM"       , nameof(IAddonNodeGetter.Sound));

            AddAlias(typeof(IAlchemicalApparatusGetter)   , "QUAL"       , nameof(IAlchemicalApparatusGetter.Quality));

            AddAlias(typeof(IAmmunitionGetter)            , "DMG"        , nameof(IAmmunitionGetter.Damage));

            AddAlias(typeof(IAnimatedObjectGetter)        , "BNAM"       , nameof(IAnimatedObjectGetter.UnloadEvent));

            AddAlias(typeof(IArmorAddonGetter)            , "BODT"       , nameof(IArmorAddonGetter.BodyTemplate));
            AddAlias(typeof(IArmorAddonGetter)            , "MODL"       , nameof(IArmorAddonGetter.AdditionalRaces));
            AddAlias(typeof(IArmorAddonGetter)            , "NAM0"       , "SkinTexture.Male");
            AddAlias(typeof(IArmorAddonGetter)            , "NAM1"       , "SkinTexture.Female");
            AddAlias(typeof(IArmorAddonGetter)            , "NAM2"       , "TextureSwapList.Male");
            AddAlias(typeof(IArmorAddonGetter)            , "NAM3"       , "TextureSwapList.Female");
            AddAlias(typeof(IArmorAddonGetter)            , "ONAM"       , nameof(IArmorAddonGetter.ArtObject));
            AddAlias(typeof(IArmorAddonGetter)            , "SNDD"       , nameof(IArmorAddonGetter.FootstepSound));

            AddAlias(typeof(IArmorGetter)                 , "BAMT"       , nameof(IArmorGetter.AlternateBlockMaterial));
            AddAlias(typeof(IArmorGetter)                 , "BIDS"       , nameof(IArmorGetter.BashImpactDataSet));
            AddAlias(typeof(IArmorGetter)                 , "BMCT"       , nameof(IArmorGetter.RagdollConstraintTemplate));
            AddAlias(typeof(IArmorGetter)                 , "BOD2"       , nameof(IArmorGetter.BodyTemplate));
            AddAlias(typeof(IArmorGetter)                 , "DNAM"       , nameof(IArmorGetter.ArmorRating));
            AddAlias(typeof(IArmorGetter)                 , "MODL"       , nameof(IArmorGetter.Armature));
            AddAlias(typeof(IArmorGetter)                 , "TNAM"       , nameof(IArmorGetter.TemplateArmor));

            AddAlias(typeof(IArtObjectGetter)             , "DNAM"       , nameof(IArtObjectGetter.Type));

            AddAlias(typeof(IAssociationTypeGetter)       , "DATA"       , nameof(IAssociationTypeGetter.IsFamily));
            AddAlias(typeof(IAssociationTypeGetter)       , "FCHT"       , "Title.Female");
            AddAlias(typeof(IAssociationTypeGetter)       , "FPRT"       , "ParentTitle.Female");
            AddAlias(typeof(IAssociationTypeGetter)       , "MCHT"       , "Title.Male");
            AddAlias(typeof(IAssociationTypeGetter)       , "MPRT"       , "ParentTitle.Male");

            AddAlias(typeof(IBookGetter)                  , "CNAM"       , nameof(IBookGetter.Description));
            AddAlias(typeof(IBookGetter)                  , "DESC"       , nameof(IBookGetter.BookText));
            AddAlias(typeof(IBookGetter)                  , "INAM"       , nameof(IBookGetter.InventoryArt));

            AddAlias(typeof(ICameraPathGetter)            , "ANAM"       , nameof(ICameraPathGetter.RelatedPaths));
            AddAlias(typeof(ICameraPathGetter)            , "DATA"       , nameof(ICameraPathGetter.Zoom));
            AddAlias(typeof(ICameraPathGetter)            , "SNAM"       , nameof(ICameraPathGetter.Shots));

            AddAlias(typeof(ICameraShotGetter)            , "MNAM"       , nameof(ICameraShotGetter.ImageSpaceModifier));

            AddAlias(typeof(ICellGetter)                  , "DATA"       , nameof(ICellGetter.Flags));
            AddAlias(typeof(ICellGetter)                  , "LTMP"       , nameof(ICellGetter.LightingTemplate));
            AddAlias(typeof(ICellGetter)                  , "MHDT"       , nameof(ICellGetter.MaxHeightData));
            AddAlias(typeof(ICellGetter)                  , "TVDT"       , nameof(ICellGetter.OcclusionData));
            AddAlias(typeof(ICellGetter)                  , "XCAS"       , nameof(ICellGetter.AcousticSpace));
            AddAlias(typeof(ICellGetter)                  , "XCCM"       , nameof(ICellGetter.SkyAndWeatherFromRegion));
            AddAlias(typeof(ICellGetter)                  , "XCIM"       , nameof(ICellGetter.ImageSpace));
            AddAlias(typeof(ICellGetter)                  , "XCLL"       , nameof(ICellGetter.Lighting));
            AddAlias(typeof(ICellGetter)                  , "XCLR"       , nameof(ICellGetter.Regions));
            AddAlias(typeof(ICellGetter)                  , "XCLW"       , nameof(ICellGetter.WaterHeight));
            AddAlias(typeof(ICellGetter)                  , "XCLX"       , nameof(ICellGetter.Grid));
            AddAlias(typeof(ICellGetter)                  , "XCMO"       , nameof(ICellGetter.Music));
            AddAlias(typeof(ICellGetter)                  , "XILL"       , nameof(ICellGetter.LockList));
            AddAlias(typeof(ICellGetter)                  , "XNAM"       , nameof(ICellGetter.WaterNoiseTexture));
            AddAlias(typeof(ICellGetter)                  , "XOWN"       , nameof(ICellGetter.Owner));
            AddAlias(typeof(ICellGetter)                  , "XRNK"       , nameof(ICellGetter.FactionRank));
            AddAlias(typeof(ICellGetter)                  , "XWEM"       , nameof(ICellGetter.WaterEnvironmentMap));

            AddAlias(typeof(IClimateGetter)               , "FNAM"       , nameof(IClimateGetter.SunTexture));
            AddAlias(typeof(IClimateGetter)               , "GNAM"       , nameof(IClimateGetter.SunGlareTexture));
            AddAlias(typeof(IClimateGetter)               , "WLST"       , nameof(IClimateGetter.WeatherTypes));

            AddAlias(typeof(ICollisionLayerGetter)        , "BNAM"       , nameof(ICollisionLayerGetter.Index));
            AddAlias(typeof(ICollisionLayerGetter)        , "CNAM"       , nameof(ICollisionLayerGetter.CollidesWith));
            AddAlias(typeof(ICollisionLayerGetter)        , "FNAM"       , nameof(ICollisionLayerGetter.DebugColor));
            AddAlias(typeof(ICollisionLayerGetter)        , "GNAM"       , nameof(ICollisionLayerGetter.Flags));
            AddAlias(typeof(ICollisionLayerGetter)        , "MNAM"       , nameof(ICollisionLayerGetter.Name));

            AddAlias(typeof(IColorRecordGetter)           , "CNAM"       , nameof(IColorRecordGetter.Color));
            AddAlias(typeof(IColorRecordGetter)           , "FNAM"       , nameof(IColorRecordGetter.Playable));

            AddAlias(typeof(ICombatStyleGetter)           , "CSCR"       , nameof(ICombatStyleGetter.CloseRange));
            AddAlias(typeof(ICombatStyleGetter)           , "CSFL"       , nameof(ICombatStyleGetter.Flight));
            AddAlias(typeof(ICombatStyleGetter)           , "CSLR"       , nameof(ICombatStyleGetter.LongRangeStrafeMult));
            AddAlias(typeof(ICombatStyleGetter)           , "CSME"       , nameof(ICombatStyleGetter.Melee));
            AddAlias(typeof(ICombatStyleGetter)           , "DATA"       , nameof(ICombatStyleGetter.Flags));

            AddAlias(typeof(IConstructibleObjectGetter)   , "BNAM"       , nameof(IConstructibleObjectGetter.WorkbenchKeyword));
            AddAlias(typeof(IConstructibleObjectGetter)   , "CNAM"       , nameof(IConstructibleObjectGetter.CreatedObject));
            AddAlias(typeof(IConstructibleObjectGetter)   , "NAM1"       , nameof(IConstructibleObjectGetter.CreatedObjectCount));

            AddAlias(typeof(IContainerGetter)             , "QNAM"       , nameof(IContainerGetter.CloseSound));
            AddAlias(typeof(IContainerGetter)             , "SNAM"       , nameof(IContainerGetter.OpenSound));

            AddAlias(typeof(IDefaultObjectManagerGetter)  , "DNAM"       , nameof(IDefaultObjectManagerGetter.Objects));

            AddAlias(typeof(IDialogBranchGetter)          , "DNAM"       , nameof(IDialogBranchGetter.Flags));
            AddAlias(typeof(IDialogBranchGetter)          , "QNAM"       , nameof(IDialogBranchGetter.Quest));
            AddAlias(typeof(IDialogBranchGetter)          , "SNAM"       , nameof(IDialogBranchGetter.StartingTopic));
            AddAlias(typeof(IDialogBranchGetter)          , "TNAM"       , nameof(IDialogBranchGetter.Category));

            AddAlias(typeof(IDialogTopicGetter)           , "BNAM"       , nameof(IDialogTopicGetter.Branch));
            AddAlias(typeof(IDialogTopicGetter)           , "PNAM"       , nameof(IDialogTopicGetter.Priority));
            AddAlias(typeof(IDialogTopicGetter)           , "QNAM"       , nameof(IDialogTopicGetter.Quest));
            AddAlias(typeof(IDialogTopicGetter)           , "SNAM"       , nameof(IDialogTopicGetter.Subtype));

            AddAlias(typeof(IDialogViewGetter)            , "BNAM"       , nameof(IDialogViewGetter.Branches));
            AddAlias(typeof(IDialogViewGetter)            , "QNAM"       , nameof(IDialogViewGetter.Quest));

            AddAlias(typeof(IDoorGetter)                  , "ANAM"       , nameof(IDoorGetter.CloseSound));
            AddAlias(typeof(IDoorGetter)                  , "BNAM"       , nameof(IDoorGetter.LoopSound));
            AddAlias(typeof(IDoorGetter)                  , "FNAM"       , nameof(IDoorGetter.Flags));
            AddAlias(typeof(IDoorGetter)                  , "SNAM"       , nameof(IDoorGetter.OpenSound));

            AddAlias(typeof(IEffectShaderGetter)          , "ICO2"       , nameof(IEffectShaderGetter.ParticleShaderTexture));
            AddAlias(typeof(IEffectShaderGetter)          , "ICON"       , nameof(IEffectShaderGetter.FillTexture));
            AddAlias(typeof(IEffectShaderGetter)          , "NAM7"       , nameof(IEffectShaderGetter.HolesTexture));
            AddAlias(typeof(IEffectShaderGetter)          , "NAM8"       , nameof(IEffectShaderGetter.MembranePaletteTexture));
            AddAlias(typeof(IEffectShaderGetter)          , "NAM9"       , nameof(IEffectShaderGetter.ParticlePaletteTexture));

            AddAlias(typeof(IEquipTypeGetter)             , "DATA"       , nameof(IEquipTypeGetter.UseAllParents));
            AddAlias(typeof(IEquipTypeGetter)             , "PNAM"       , nameof(IEquipTypeGetter.SlotParents));

            AddAlias(typeof(IExplosionGetter)             , "MNAM"       , nameof(IExplosionGetter.ImageSpaceModifier));

            AddAlias(typeof(IEyesGetter)                  , "DATA"       , nameof(IEyesGetter.Flags));
            AddAlias(typeof(IEyesGetter)                  , "Texture"    , nameof(IEyesGetter.Icon));

            AddAlias(typeof(IFactionGetter)               , "CRGR"       , nameof(IFactionGetter.SharedCrimeFactionList));
            AddAlias(typeof(IFactionGetter)               , "CRVA"       , nameof(IFactionGetter.CrimeValues));
            AddAlias(typeof(IFactionGetter)               , "DATA"       , nameof(IFactionGetter.Flags));
            AddAlias(typeof(IFactionGetter)               , "JAIL"       , nameof(IFactionGetter.ExteriorJailMarker));
            AddAlias(typeof(IFactionGetter)               , "JOUT"       , nameof(IFactionGetter.JailOutfit));
            AddAlias(typeof(IFactionGetter)               , "PLCN"       , nameof(IFactionGetter.PlayerInventoryContainer));
            AddAlias(typeof(IFactionGetter)               , "PLVD"       , nameof(IFactionGetter.VendorLocation));
            AddAlias(typeof(IFactionGetter)               , "STOL"       , nameof(IFactionGetter.StolenGoodsContainer));
            AddAlias(typeof(IFactionGetter)               , "VENC"       , nameof(IFactionGetter.MerchantContainer));
            AddAlias(typeof(IFactionGetter)               , "VEND"       , nameof(IFactionGetter.VendorBuySellList));
            AddAlias(typeof(IFactionGetter)               , "VENV"       , nameof(IFactionGetter.VendorValues));
            AddAlias(typeof(IFactionGetter)               , "WAIT"       , nameof(IFactionGetter.FollowerWaitMarker));
            AddAlias(typeof(IFactionGetter)               , "XNAM"       , nameof(IFactionGetter.Relations));

            AddAlias(typeof(IFloraGetter)                 , "PFIG"       , nameof(IFloraGetter.Ingredient));
            AddAlias(typeof(IFloraGetter)                 , "PFPC"       , nameof(IFloraGetter.Production));
            AddAlias(typeof(IFloraGetter)                 , "RNAM"       , nameof(IFloraGetter.ActivateTextOverride));
            AddAlias(typeof(IFloraGetter)                 , "SNAM"       , nameof(IFloraGetter.HarvestSound));

            AddAlias(typeof(IFootstepGetter)              , "ANAM"       , nameof(IFootstepGetter.Tag));
            AddAlias(typeof(IFootstepGetter)              , "DATA"       , nameof(IFootstepGetter.ImpactDataSet));

            AddAlias(typeof(IFormListGetter)              , "FormID"     , nameof(IFormListGetter.Items));
            AddAlias(typeof(IFormListGetter)              , "FormIDs"    , nameof(IFormListGetter.Items));
            AddAlias(typeof(IFormListGetter)              , "LNAM"       , nameof(IFormListGetter.Items));

            AddAlias(typeof(IFurnitureGetter)             , "FNAM"       , nameof(IFurnitureGetter.Flags));
            AddAlias(typeof(IFurnitureGetter)             , "KNAM"       , nameof(IFurnitureGetter.InteractionKeyword));
            AddAlias(typeof(IFurnitureGetter)             , "MNAM"       , nameof(IFurnitureGetter.Flags));
            AddAlias(typeof(IFurnitureGetter)             , "NAM1"       , nameof(IFurnitureGetter.AssociatedSpell));
            AddAlias(typeof(IFurnitureGetter)             , "WBDT"       , nameof(IFurnitureGetter.WorkbenchData));
            AddAlias(typeof(IFurnitureGetter)             , "XMRK"       , nameof(IFurnitureGetter.ModelFilename));

            AddAlias(typeof(IGlobalGetter)                , "FLTV"       , nameof(IGlobalFloatGetter.Data));
            AddAlias(typeof(IGlobalGetter)                , "FNAM"       , nameof(IGlobalGetter.Type));

            AddAlias(typeof(IHazardGetter)                , "MNAM"       , nameof(IHazardGetter.ImageSpaceModifier));

            AddAlias(typeof(IHeadPartGetter)              , "CNAM"       , nameof(IHeadPartGetter.Color));
            AddAlias(typeof(IHeadPartGetter)              , "DATA"       , nameof(IHeadPartGetter.Flags));
            AddAlias(typeof(IHeadPartGetter)              , "HNAM"       , nameof(IHeadPartGetter.ExtraParts));
            AddAlias(typeof(IHeadPartGetter)              , "PNAM"       , nameof(IHeadPartGetter.Type));
            AddAlias(typeof(IHeadPartGetter)              , "RNAM"       , nameof(IHeadPartGetter.ValidRaces));
            AddAlias(typeof(IHeadPartGetter)              , "TNAM"       , nameof(IHeadPartGetter.TextureSet));

            AddAlias(typeof(IIdleAnimationGetter)         , "ANAM"       , nameof(IIdleAnimationGetter.RelatedIdles));
            AddAlias(typeof(IIdleAnimationGetter)         , "DNAM"       , nameof(IIdleAnimationGetter.Filename));
            AddAlias(typeof(IIdleAnimationGetter)         , "ENAM"       , nameof(IIdleAnimationGetter.AnimationEvent));

            AddAlias(typeof(IIdleMarkerGetter)            , "IDLA"       , nameof(IIdleMarkerGetter.Animations));
            AddAlias(typeof(IIdleMarkerGetter)            , "IDLF"       , nameof(IIdleMarkerGetter.Flags));
            AddAlias(typeof(IIdleMarkerGetter)            , "IDLT"       , nameof(IIdleMarkerGetter.IdleTimer));

            AddAlias(typeof(IImageSpaceGetter)            , "CNAM"       , nameof(IImageSpaceGetter.Cinematic));
            AddAlias(typeof(IImageSpaceGetter)            , "DNAM"       , nameof(IImageSpaceGetter.DepthOfField));
            AddAlias(typeof(IImageSpaceGetter)            , "HNAM"       , nameof(IImageSpaceGetter.Hdr));
            AddAlias(typeof(IImageSpaceGetter)            , "TNAM"       , nameof(IImageSpaceGetter.Tint));

            AddAlias(typeof(IImageSpaceAdapterGetter)     , "BNAM"       , nameof(IImageSpaceAdapterGetter.BlurRadius));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "NAM1"       , nameof(IImageSpaceAdapterGetter.RadialBlurRampDown));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "NAM2"       , nameof(IImageSpaceAdapterGetter.RadialBlurDownStart));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "NAM3"       , nameof(IImageSpaceAdapterGetter.FadeColor));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "NAM4"       , nameof(IImageSpaceAdapterGetter.MotionBlurStrength));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "RNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurStrength));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "SNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurRampUp));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "TNAM"       , nameof(IImageSpaceAdapterGetter.TintColor));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "UNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurStart));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "WNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldStrength));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "VNAM"       , nameof(IImageSpaceAdapterGetter.DoubleVisionStrength));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "YNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldDistance));
            AddAlias(typeof(IImageSpaceAdapterGetter)     , "ZNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldRange));

            AddAlias(typeof(IImpactGetter)                , "DNAM"       , nameof(IImpactGetter.TextureSet));
            AddAlias(typeof(IImpactGetter)                , "DODT"       , nameof(IImpactGetter.Decal));
            AddAlias(typeof(IImpactGetter)                , "ENAM"       , nameof(IImpactGetter.SecondaryTextureSet));
            AddAlias(typeof(IImpactGetter)                , "NAM1"       , nameof(IImpactGetter.Sound2));
            AddAlias(typeof(IImpactGetter)                , "NAM2"       , nameof(IImpactGetter.Hazard));
            AddAlias(typeof(IImpactGetter)                , "SNAM"       , nameof(IImpactGetter.Sound1));

            AddAlias(typeof(IIngestibleGetter)            , "DATA"       , nameof(IIngestibleGetter.Weight));

            AddAlias(typeof(IIngredientGetter)            , "ETYP"       , nameof(IIngredientGetter.EquipType));

            AddAlias(typeof(IKeywordGetter)               , "CNAM"       , nameof(IKeywordGetter.Color));

            AddAlias(typeof(ILandscapeTextureGetter)      , "GNAM"       , nameof(ILandscapeTextureGetter.Grasses));
            AddAlias(typeof(ILandscapeTextureGetter)      , "INAM"       , nameof(ILandscapeTextureGetter.Flags));
            AddAlias(typeof(ILandscapeTextureGetter)      , "MNAM"       , nameof(ILandscapeTextureGetter.MaterialType));
            AddAlias(typeof(ILandscapeTextureGetter)      , "SNAM"       , nameof(ILandscapeTextureGetter.TextureSpecularExponent));
            AddAlias(typeof(ILandscapeTextureGetter)      , "TNAM"       , nameof(ILandscapeTextureGetter.TextureSet));

            AddAlias(typeof(ILeveledItemGetter)           , "LVLD"       , nameof(ILeveledItemGetter.ChanceNone));
            AddAlias(typeof(ILeveledItemGetter)           , "LVLF"       , nameof(ILeveledItemGetter.Flags));
            AddAlias(typeof(ILeveledItemGetter)           , "LVLG"       , nameof(ILeveledItemGetter.Global));

            AddAlias(typeof(ILeveledNpcGetter)            , "LVLD"       , nameof(ILeveledNpcGetter.ChanceNone));
            AddAlias(typeof(ILeveledNpcGetter)            , "LVLF"       , nameof(ILeveledNpcGetter.Flags));
            AddAlias(typeof(ILeveledNpcGetter)            , "LVLG"       , nameof(ILeveledNpcGetter.Global));

            AddAlias(typeof(ILeveledSpellGetter)          , "LVLD"       , nameof(ILeveledSpellGetter.ChanceNone));
            AddAlias(typeof(ILeveledSpellGetter)          , "LVLF"       , nameof(ILeveledSpellGetter.Flags));

            AddAlias(typeof(ILightGetter)                 , "FNAM"       , nameof(ILightGetter.FadeValue));
            AddAlias(typeof(ILightGetter)                 , "LNAM"       , nameof(ILightGetter.Lens));
            AddAlias(typeof(ILightGetter)                 , "SNAM"       , nameof(ILightGetter.Sound));

            AddAlias(typeof(ILightingTemplateGetter)      , "DALC"       , nameof(ILightingTemplateGetter.DirectionalAmbientColors));

            AddAlias(typeof(ILoadScreenGetter)            , "MOD2"       , nameof(ILoadScreenGetter.CameraPath));
            AddAlias(typeof(ILoadScreenGetter)            , "NNAM"       , nameof(ILoadScreenGetter.LoadingScreenNif));
            AddAlias(typeof(ILoadScreenGetter)            , "ONAM"       , nameof(ILoadScreenGetter.RotationOffsetConstraints));
            AddAlias(typeof(ILoadScreenGetter)            , "RNAM"       , nameof(ILoadScreenGetter.InitialRotation));
            AddAlias(typeof(ILoadScreenGetter)            , "SNAM"       , nameof(ILoadScreenGetter.InitialScale));
            AddAlias(typeof(ILoadScreenGetter)            , "XNAM"       , nameof(ILoadScreenGetter.InitialTranslationOffset));

            AddAlias(typeof(ILocationGetter)              , "CNAM"       , nameof(ILocationGetter.Color));
            AddAlias(typeof(ILocationGetter)              , "FNAM"       , nameof(ILocationGetter.UnreportedCrimeFaction));
            AddAlias(typeof(ILocationGetter)              , "MNAM"       , nameof(ILocationGetter.WorldLocationMarkerRef));
            AddAlias(typeof(ILocationGetter)              , "NAM0"       , nameof(ILocationGetter.HorseMarkerRef));
            AddAlias(typeof(ILocationGetter)              , "NAM1"       , nameof(ILocationGetter.Music));
            AddAlias(typeof(ILocationGetter)              , "PNAM"       , nameof(ILocationGetter.ParentLocation));
            AddAlias(typeof(ILocationGetter)              , "RNAM"       , nameof(ILocationGetter.WorldLocationRadius));

            AddAlias(typeof(ILocationReferenceTypeGetter) , "CNAM"       , nameof(ILocationReferenceTypeGetter.Color));

            AddAlias(typeof(IMagicEffectGetter)           , "DNAM"       , nameof(IMagicEffectGetter.Description));
            AddAlias(typeof(IMagicEffectGetter)           , "MDOB"       , nameof(IMagicEffectGetter.MenuDisplayObject));
            AddAlias(typeof(IMagicEffectGetter)           , "SNDD"       , nameof(IMagicEffectGetter.Sounds));

            AddAlias(typeof(IMaterialTypeGetter)          , "BNAM"       , nameof(IMaterialTypeGetter.Buoyancy));
            AddAlias(typeof(IMaterialTypeGetter)          , "CNAM"       , nameof(IMaterialTypeGetter.HavokDisplayColor));
            AddAlias(typeof(IMaterialTypeGetter)          , "FNAM"       , nameof(IMaterialTypeGetter.Flags));
            AddAlias(typeof(IMaterialTypeGetter)          , "HNAM"       , nameof(IMaterialTypeGetter.HavokImpactDataSet));
            AddAlias(typeof(IMaterialTypeGetter)          , "MNAM"       , nameof(IMaterialTypeGetter.Name));
            AddAlias(typeof(IMaterialTypeGetter)          , "PNAM"       , nameof(IMaterialTypeGetter.Parent));

            AddAlias(typeof(IMessageGetter)               , "DNAM"       , nameof(IMessageGetter.Flags));
            AddAlias(typeof(IMessageGetter)               , "QNAM"       , nameof(IMessageGetter.Quest));
            AddAlias(typeof(IMessageGetter)               , "TNAM"       , nameof(IMessageGetter.DisplayTime));

            AddAlias(typeof(IMiscItemGetter)              , "ICON"       , "Icons.LargeIconFilename");
            AddAlias(typeof(IMiscItemGetter)              , "MICO"       , "Icons.SmallIconFilename");

            AddAlias(typeof(IMoveableStaticGetter)        , "DATA"       , nameof(IMoveableStaticGetter.Flags));
            AddAlias(typeof(IMoveableStaticGetter)        , "SNAM"       , nameof(IMoveableStaticGetter.LoopingSound));

            AddAlias(typeof(IMovementTypeGetter)          , "FULL"       , null);
            AddAlias(typeof(IMovementTypeGetter)          , "INAM"       , nameof(IMovementTypeGetter.AnimationChangeThresholds));
            AddAlias(typeof(IMovementTypeGetter)          , "MNAM"       , nameof(IMovementTypeGetter.Name));

            AddAlias(typeof(IMusicTrackGetter)            , "ANAM"       , nameof(IMusicTrackGetter.TrackFilename));
            AddAlias(typeof(IMusicTrackGetter)            , "BNAM"       , nameof(IMusicTrackGetter.FinaleFilename));
            AddAlias(typeof(IMusicTrackGetter)            , "CNAM"       , nameof(IMusicTrackGetter.Type));
            AddAlias(typeof(IMusicTrackGetter)            , "DNAM"       , nameof(IMusicTrackGetter.FadeOut));
            AddAlias(typeof(IMusicTrackGetter)            , "FLTV"       , nameof(IMusicTrackGetter.Duration));
            AddAlias(typeof(IMusicTrackGetter)            , "FNAM"       , nameof(IMusicTrackGetter.CuePoints));
            AddAlias(typeof(IMusicTrackGetter)            , "LNAM"       , nameof(IMusicTrackGetter.LoopData));
            AddAlias(typeof(IMusicTrackGetter)            , "SNAM"       , nameof(IMusicTrackGetter.Tracks));

            AddAlias(typeof(IMusicTypeGetter)             , "FNAM"       , nameof(IMusicTypeGetter.Flags));
            AddAlias(typeof(IMusicTypeGetter)             , "TNAM"       , nameof(IMusicTypeGetter.Tracks));
            AddAlias(typeof(IMusicTypeGetter)             , "WNAM"       , nameof(IMusicTypeGetter.FadeDuration));

            AddAlias(typeof(INpcGetter)                   , "AIDT"       , nameof(INpcGetter.AIData));
            AddAlias(typeof(INpcGetter)                   , "ANAM"       , nameof(INpcGetter.FarAwayModel));
            AddAlias(typeof(INpcGetter)                   , "ATKR"       , nameof(INpcGetter.AttackRace));
            AddAlias(typeof(INpcGetter)                   , "CNAM"       , nameof(INpcGetter.Class));
            AddAlias(typeof(INpcGetter)                   , "CRIF"       , nameof(INpcGetter.CrimeFaction));
            AddAlias(typeof(INpcGetter)                   , "DNAM"       , nameof(INpcGetter.PlayerSkills));
            AddAlias(typeof(INpcGetter)                   , "DOFT"       , nameof(INpcGetter.DefaultOutfit));
            AddAlias(typeof(INpcGetter)                   , "DPLT"       , nameof(INpcGetter.DefaultPackageList));
            AddAlias(typeof(INpcGetter)                   , "ECOR"       , nameof(INpcGetter.CombatOverridePackageList));
            AddAlias(typeof(INpcGetter)                   , "FTST"       , nameof(INpcGetter.HeadTexture));
            AddAlias(typeof(INpcGetter)                   , "GNAM"       , nameof(INpcGetter.GiftFilter));
            AddAlias(typeof(INpcGetter)                   , "GWOR"       , nameof(INpcGetter.GuardWarnOverridePackageList));
            AddAlias(typeof(INpcGetter)                   , "HCLF"       , nameof(INpcGetter.HairColor));
            AddAlias(typeof(INpcGetter)                   , "INAM"       , nameof(INpcGetter.DeathItem));
            AddAlias(typeof(INpcGetter)                   , "NAM6"       , nameof(INpcGetter.Height));
            AddAlias(typeof(INpcGetter)                   , "NAM7"       , nameof(INpcGetter.Weight));
            AddAlias(typeof(INpcGetter)                   , "NAM8"       , nameof(INpcGetter.SoundLevel));
            AddAlias(typeof(INpcGetter)                   , "NAM9"       , nameof(INpcGetter.FaceMorph));
            AddAlias(typeof(INpcGetter)                   , "NAMA"       , nameof(INpcGetter.FaceParts));
            AddAlias(typeof(INpcGetter)                   , "OCOR"       , nameof(INpcGetter.ObserveDeadBodyOverridePackageList));
            AddAlias(typeof(INpcGetter)                   , "PRKR"       , nameof(INpcGetter.Perks));
            AddAlias(typeof(INpcGetter)                   , "PKID"       , nameof(INpcGetter.Packages));
            AddAlias(typeof(INpcGetter)                   , "PNAM"       , nameof(INpcGetter.HeadParts));
            AddAlias(typeof(INpcGetter)                   , "QNAM"       , nameof(INpcGetter.TextureLighting));
            AddAlias(typeof(INpcGetter)                   , "SHRT"       , nameof(INpcGetter.ShortName));
            AddAlias(typeof(INpcGetter)                   , "SNAM"       , nameof(INpcGetter.Factions));
            AddAlias(typeof(INpcGetter)                   , "SOFT"       , nameof(INpcGetter.SleepingOutfit));
            AddAlias(typeof(INpcGetter)                   , "SPLO"       , nameof(INpcGetter.ActorEffect));
            AddAlias(typeof(INpcGetter)                   , "SPOR"       , nameof(INpcGetter.SpectatorOverridePackageList));
            AddAlias(typeof(INpcGetter)                   , "TPLT"       , nameof(INpcGetter.Template));
            AddAlias(typeof(INpcGetter)                   , "VTCK"       , nameof(INpcGetter.Voice));
            AddAlias(typeof(INpcGetter)                   , "WNAM"       , nameof(INpcGetter.WornArmor));
            AddAlias(typeof(INpcGetter)                   , "ZNAM"       , nameof(INpcGetter.CombatStyle));

            AddAlias(typeof(IOutfitGetter)                , "INAM"       , nameof(IOutfitGetter.Items));

            AddAlias(typeof(IPackageGetter)               , "CNAM"       , nameof(IPackageGetter.CombatStyle));
            AddAlias(typeof(IPackageGetter)               , "QNAM"       , nameof(IPackageGetter.OwnerQuest));
            AddAlias(typeof(IPackageGetter)               , "XNAM"       , nameof(IPackageGetter.XnamMarker));

            AddAlias(typeof(IPerkGetter)                  , "NNAM"       , nameof(IPerkGetter.NextPerk));

            AddAlias(typeof(IProjectileGetter)            , "VNAM"       , nameof(IProjectileGetter.SoundLevel));

            AddAlias(typeof(IQuestGetter)                 , "ANAM"       , nameof(IQuestGetter.NextAliasID));
            AddAlias(typeof(IQuestGetter)                 , "DESC"       , null);
            AddAlias(typeof(IQuestGetter)                 , "ENAM"       , nameof(IQuestGetter.Event));
            AddAlias(typeof(IQuestGetter)                 , "FLTR"       , nameof(IQuestGetter.Filter));
            AddAlias(typeof(IQuestGetter)                 , "NNAM"       , nameof(IQuestGetter.Description));
            AddAlias(typeof(IQuestGetter)                 , "QTGL"       , nameof(IQuestGetter.TextDisplayGlobals));

            AddAlias(typeof(IRaceGetter)                  , "ANAM"       , nameof(IRaceGetter.SkeletalModel));
            AddAlias(typeof(IRaceGetter)                  , "ATKR"       , nameof(IRaceGetter.AttackRace));
            AddAlias(typeof(IRaceGetter)                  , "BOD2"       , nameof(IRaceGetter.BodyTemplate));
            AddAlias(typeof(IRaceGetter)                  , "DNAM"       , nameof(IRaceGetter.DecapitateArmors));
            AddAlias(typeof(IRaceGetter)                  , "ENAM"       , nameof(IRaceGetter.Eyes));
            AddAlias(typeof(IRaceGetter)                  , "FLMV"       , nameof(IRaceGetter.BaseMovementDefaultFly));
            AddAlias(typeof(IRaceGetter)                  , "GNAM"       , nameof(IRaceGetter.BodyPartData));
            AddAlias(typeof(IRaceGetter)                  , "HCLF"       , nameof(IRaceGetter.DefaultHairColors));
            AddAlias(typeof(IRaceGetter)                  , "HNAM"       , nameof(IRaceGetter.Hairs));
            AddAlias(typeof(IRaceGetter)                  , "LNAM"       , nameof(IRaceGetter.CloseLootSound));
            AddAlias(typeof(IRaceGetter)                  , "MTNM"       , nameof(IRaceGetter.MovementTypeNames));
            AddAlias(typeof(IRaceGetter)                  , "NAM4"       , nameof(IRaceGetter.MaterialType));
            AddAlias(typeof(IRaceGetter)                  , "NAM5"       , nameof(IRaceGetter.ImpactDataSet));
            AddAlias(typeof(IRaceGetter)                  , "NAM7"       , nameof(IRaceGetter.DecapitationFX));
            AddAlias(typeof(IRaceGetter)                  , "NAM8"       , nameof(IRaceGetter.MorphRace));
            AddAlias(typeof(IRaceGetter)                  , "ONAM"       , nameof(IRaceGetter.OpenLootSound));
            AddAlias(typeof(IRaceGetter)                  , "PNAM"       , nameof(IRaceGetter.FacegenMainClamp));
            AddAlias(typeof(IRaceGetter)                  , "QNAM"       , nameof(IRaceGetter.EquipmentSlots));
            AddAlias(typeof(IRaceGetter)                  , "RNAM"       , nameof(IRaceGetter.ArmorRace));
            AddAlias(typeof(IRaceGetter)                  , "RNMV"       , nameof(IRaceGetter.BaseMovementDefaultRun));
            AddAlias(typeof(IRaceGetter)                  , "SNMV"       , nameof(IRaceGetter.BaseMovementDefaultSneak));
            AddAlias(typeof(IRaceGetter)                  , "SPMV"       , nameof(IRaceGetter.BaseMovementDefaultSprint));
            AddAlias(typeof(IRaceGetter)                  , "SWMV"       , nameof(IRaceGetter.BaseMovementDefaultSwim));
            AddAlias(typeof(IRaceGetter)                  , "UNAM"       , nameof(IRaceGetter.FacegenFaceClamp));
            AddAlias(typeof(IRaceGetter)                  , "UNES"       , nameof(IRaceGetter.UnarmedEquipSlot));
            AddAlias(typeof(IRaceGetter)                  , "VNAM"       , nameof(IRaceGetter.EquipmentFlags));
            AddAlias(typeof(IRaceGetter)                  , "VTCK"       , nameof(IRaceGetter.Voices));
            AddAlias(typeof(IRaceGetter)                  , "WKMV"       , nameof(IRaceGetter.BaseMovementDefaultWalk));
            AddAlias(typeof(IRaceGetter)                  , "WNAM"       , nameof(IRaceGetter.Skin));

            AddAlias(typeof(IRegionGetter)                , "RCLR"       , nameof(IRegionGetter.MapColor));
            AddAlias(typeof(IRegionGetter)                , "WNAM"       , nameof(IRegionGetter.Worldspace));

            AddAlias(typeof(ISceneGetter)                 , "FNAM"       , nameof(ISceneGetter.Flags));
            AddAlias(typeof(ISceneGetter)                 , "INAM"       , nameof(ISceneGetter.LastActionIndex));
            AddAlias(typeof(ISceneGetter)                 , "PNAM"       , nameof(ISceneGetter.Quest));

            AddAlias(typeof(IScrollGetter)                , "MDOB"       , nameof(IScrollGetter.MenuDisplayObject));

            AddAlias(typeof(IShaderParticleGeometryGetter), "ICON"       , nameof(IShaderParticleGeometryGetter.ParticleTexture));

            AddAlias(typeof(IShoutGetter)                 , "MDOB"       , nameof(IShoutGetter.MenuDisplayObject));

            AddAlias(typeof(ISoulGemGetter)               , "NAM0"       , nameof(ISoulGemGetter.LinkedTo));
            AddAlias(typeof(ISoulGemGetter)               , "SLCP"       , nameof(ISoulGemGetter.MaximumCapacity));
            AddAlias(typeof(ISoulGemGetter)               , "SOUL"       , nameof(ISoulGemGetter.ContainedSoul));

            AddAlias(typeof(ISoundCategoryGetter)         , "FNAM"       , nameof(ISoundCategoryGetter.Flags));
            AddAlias(typeof(ISoundCategoryGetter)         , "PNAM"       , nameof(ISoundCategoryGetter.Parent));
            AddAlias(typeof(ISoundCategoryGetter)         , "UNAM"       , nameof(ISoundCategoryGetter.DefaultMenuVolume));
            AddAlias(typeof(ISoundCategoryGetter)         , "VNAM"       , nameof(ISoundCategoryGetter.StaticVolumeMultiplier));

            AddAlias(typeof(ISoundDescriptorGetter)       , "CNAM"       , nameof(ISoundDescriptorGetter.Type));
            AddAlias(typeof(ISoundDescriptorGetter)       , "FNAM"       , nameof(ISoundDescriptorGetter.String));
            AddAlias(typeof(ISoundDescriptorGetter)       , "GNAM"       , nameof(ISoundDescriptorGetter.Category));
            AddAlias(typeof(ISoundDescriptorGetter)       , "ONAM"       , nameof(ISoundDescriptorGetter.OutputModel));
            AddAlias(typeof(ISoundDescriptorGetter)       , "SNAM"       , nameof(ISoundDescriptorGetter.AlternateSoundFor));

            AddAlias(typeof(ISoundMarkerGetter)           , "SDSC"       , nameof(ISoundMarkerGetter.SoundDescriptor));

            AddAlias(typeof(ISoundOutputModelGetter)      , "MNAM"       , nameof(ISoundOutputModelGetter.Type));
            AddAlias(typeof(ISoundOutputModelGetter)      , "ONAM"       , nameof(ISoundOutputModelGetter.OutputChannels));

            AddAlias(typeof(ISpellGetter)                 , "MDOB"       , nameof(ISpellGetter.MenuDisplayObject));

            AddAlias(typeof(IStoryManagerBranchNodeGetter), "DNAM"       , nameof(IStoryManagerBranchNodeGetter.Flags));
            AddAlias(typeof(IStoryManagerBranchNodeGetter), "PNAM"       , nameof(IStoryManagerBranchNodeGetter.Parent));
            AddAlias(typeof(IStoryManagerBranchNodeGetter), "SNAM"       , nameof(IStoryManagerBranchNodeGetter.PreviousSibling));
            AddAlias(typeof(IStoryManagerBranchNodeGetter), "XNAM"       , nameof(IStoryManagerBranchNodeGetter.MaxConcurrentQuests));

            AddAlias(typeof(IStoryManagerEventNodeGetter) , "DNAM"       , nameof(IStoryManagerEventNodeGetter.Flags));
            AddAlias(typeof(IStoryManagerEventNodeGetter) , "ENAM"       , nameof(IStoryManagerEventNodeGetter.Type));
            AddAlias(typeof(IStoryManagerEventNodeGetter) , "PNAM"       , nameof(IStoryManagerEventNodeGetter.Parent));
            AddAlias(typeof(IStoryManagerEventNodeGetter) , "SNAM"       , nameof(IStoryManagerEventNodeGetter.PreviousSibling));
            AddAlias(typeof(IStoryManagerEventNodeGetter) , "XNAM"       , nameof(IStoryManagerEventNodeGetter.MaxConcurrentQuests));

            AddAlias(typeof(IStoryManagerQuestNodeGetter) , "MNAM"       , nameof(IStoryManagerQuestNodeGetter.MaxNumQuestsToRun));
            AddAlias(typeof(IStoryManagerQuestNodeGetter) , "PNAM"       , nameof(IStoryManagerQuestNodeGetter.Parent));
            AddAlias(typeof(IStoryManagerQuestNodeGetter) , "SNAM"       , nameof(IStoryManagerQuestNodeGetter.PreviousSibling));
            AddAlias(typeof(IStoryManagerQuestNodeGetter) , "XNAM"       , nameof(IStoryManagerQuestNodeGetter.MaxConcurrentQuests));

            AddAlias(typeof(ITalkingActivatorGetter)      , "SNAM"       , nameof(ITalkingActivatorGetter.LoopingSound));
            AddAlias(typeof(ITalkingActivatorGetter)      , "VNAM"       , nameof(ITalkingActivatorGetter.VoiceType));

            AddAlias(typeof(ITextureSetGetter)            , "DNAM"       , nameof(ITextureSetGetter.Flags));
            AddAlias(typeof(ITextureSetGetter)            , "DODT"       , nameof(ITextureSetGetter.Decal));

            AddAlias(typeof(ITreeGetter)                  , "PFIG"       , nameof(ITreeGetter.Ingredient));
            AddAlias(typeof(ITreeGetter)                  , "SNAM"       , nameof(ITreeGetter.HarvestSound));

            AddAlias(typeof(IVoiceTypeGetter)             , "DNAM"       , nameof(IVoiceTypeGetter.Flags));

            AddAlias(typeof(IWaterGetter)                 , "ANAM"       , nameof(IWaterGetter.Opacity));
            AddAlias(typeof(IWaterGetter)                 , "FNAM"       , nameof(IWaterGetter.Flags));
            AddAlias(typeof(IWaterGetter)                 , "INAM"       , nameof(IWaterGetter.ImageSpace));
            AddAlias(typeof(IWaterGetter)                 , "NAM0"       , nameof(IWaterGetter.LinearVelocity));
            AddAlias(typeof(IWaterGetter)                 , "NAM1"       , nameof(IWaterGetter.AngularVelocity));
            AddAlias(typeof(IWaterGetter)                 , "NAM2"       , nameof(IWaterGetter.NoiseLayerOneTexture));
            AddAlias(typeof(IWaterGetter)                 , "NAM3"       , nameof(IWaterGetter.NoiseLayerTwoTexture));
            AddAlias(typeof(IWaterGetter)                 , "NAM4"       , nameof(IWaterGetter.NoiseLayerThreeTexture));
            AddAlias(typeof(IWaterGetter)                 , "NAM5"       , nameof(IWaterGetter.FlowNormalsNoiseTexture));
            AddAlias(typeof(IWaterGetter)                 , "SNAM"       , nameof(IWaterGetter.OpenSound));
            AddAlias(typeof(IWaterGetter)                 , "TNAM"       , nameof(IWaterGetter.Material));
            AddAlias(typeof(IWaterGetter)                 , "XNAM"       , nameof(IWaterGetter.Spell));

            AddAlias(typeof(IWeaponGetter)                , "BAMT"       , nameof(IWeaponGetter.AlternateBlockMaterial));
            AddAlias(typeof(IWeaponGetter)                , "BIDS"       , nameof(IWeaponGetter.BlockBashImpact));
            AddAlias(typeof(IWeaponGetter)                , "CNAM"       , nameof(IWeaponGetter.Template));
            AddAlias(typeof(IWeaponGetter)                , "INAM"       , nameof(IWeaponGetter.ImpactDataSet));
            AddAlias(typeof(IWeaponGetter)                , "NAM7"       , nameof(IWeaponGetter.AttackLoopSound));
            AddAlias(typeof(IWeaponGetter)                , "NAM8"       , nameof(IWeaponGetter.UnequipSound));
            AddAlias(typeof(IWeaponGetter)                , "NAM9"       , nameof(IWeaponGetter.EquipSound));
            AddAlias(typeof(IWeaponGetter)                , "SNAM"       , nameof(IWeaponGetter.AttackSound));
            AddAlias(typeof(IWeaponGetter)                , "TNAM"       , nameof(IWeaponGetter.AttackFailSound));
            AddAlias(typeof(IWeaponGetter)                , "UNAM"       , nameof(IWeaponGetter.IdleSound));
            AddAlias(typeof(IWeaponGetter)                , "VNAM"       , nameof(IWeaponGetter.DetectionSoundLevel));
            AddAlias(typeof(IWeaponGetter)                , "WNAM"       , nameof(IWeaponGetter.FirstPersonModel));
            AddAlias(typeof(IWeaponGetter)                , "XNAM"       , nameof(IWeaponGetter.AttackSound2D));

            AddAlias(typeof(IWeatherGetter)               , "DALC"       , nameof(IWeatherGetter.DirectionalAmbientLightingColors));
            AddAlias(typeof(IWeatherGetter)               , "GNAM"       , nameof(IWeatherGetter.SunGlareLensFlare));
            AddAlias(typeof(IWeatherGetter)               , "HNAM"       , nameof(IWeatherGetter.VolumetricLighting));
            AddAlias(typeof(IWeatherGetter)               , "IMSP"       , nameof(IWeatherGetter.ImageSpaces));
            AddAlias(typeof(IWeatherGetter)               , "MNAM"       , nameof(IWeatherGetter.Precipitation));
            AddAlias(typeof(IWeatherGetter)               , "NNAM"       , nameof(IWeatherGetter.VisualEffect));

            AddAlias(typeof(IWordOfPowerGetter)           , "TNAM"       , nameof(IWordOfPowerGetter.Translation));

            AddAlias(typeof(IWorldspaceGetter)            , "CNAM"       , nameof(IWorldspaceGetter.Climate));
            AddAlias(typeof(IWorldspaceGetter)            , "DATA"       , nameof(IWorldspaceGetter.Flags));
            AddAlias(typeof(IWorldspaceGetter)            , "DNAM"       , nameof(IWorldspaceGetter.LandDefaults));
            AddAlias(typeof(IWorldspaceGetter)            , "ICON"       , nameof(IWorldspaceGetter.MapImage));
            AddAlias(typeof(IWorldspaceGetter)            , "LTMP"       , nameof(IWorldspaceGetter.InteriorLighting));
            AddAlias(typeof(IWorldspaceGetter)            , "MNAM"       , nameof(IWorldspaceGetter.MapData));
            AddAlias(typeof(IWorldspaceGetter)            , "MHDT"       , nameof(IWorldspaceGetter.MaxHeight));
            AddAlias(typeof(IWorldspaceGetter)            , "NAM0"       , nameof(IWorldspaceGetter.ObjectBoundsMin));
            AddAlias(typeof(IWorldspaceGetter)            , "NAM2"       , nameof(IWorldspaceGetter.Water));
            AddAlias(typeof(IWorldspaceGetter)            , "NAM3"       , nameof(IWorldspaceGetter.LodWater));
            AddAlias(typeof(IWorldspaceGetter)            , "NAM4"       , nameof(IWorldspaceGetter.LodWaterHeight));
            AddAlias(typeof(IWorldspaceGetter)            , "NAM9"       , nameof(IWorldspaceGetter.ObjectBoundsMax));
            AddAlias(typeof(IWorldspaceGetter)            , "NAMA"       , nameof(IWorldspaceGetter.DistantLodMultiplier));
            AddAlias(typeof(IWorldspaceGetter)            , "NNAM"       , nameof(IWorldspaceGetter.CanopyShadow));
            AddAlias(typeof(IWorldspaceGetter)            , "OFST"       , nameof(IWorldspaceGetter.OffsetData));
            AddAlias(typeof(IWorldspaceGetter)            , "TNAM"       , nameof(IWorldspaceGetter.HdLodDiffuseTexture));
            AddAlias(typeof(IWorldspaceGetter)            , "UNAM"       , nameof(IWorldspaceGetter.HdLodNormalTexture));
            AddAlias(typeof(IWorldspaceGetter)            , "WCTR"       , nameof(IWorldspaceGetter.FixedDimensionsCenterCell));
            AddAlias(typeof(IWorldspaceGetter)            , "XNAM"       , nameof(IWorldspaceGetter.WaterNoiseTexture));
            AddAlias(typeof(IWorldspaceGetter)            , "XWEM"       , nameof(IWorldspaceGetter.WaterEnvironmentMap));
            AddAlias(typeof(IWorldspaceGetter)            , "ZNAM"       , nameof(IWorldspaceGetter.Music));
#pragma warning restore format
        }

        private static bool tryFindAlias (Type? type, string key, out PropertyAliasMapping pam)
        {
            if (type != null && propertyAliases.TryGetValue(new RecordProperty(type, key), out var _rpm) && _rpm is PropertyAliasMapping _PAM)
            {
                pam = _PAM;
                return true;
            }
            else if (propertyAliases.TryGetValue(new RecordProperty(key), out var _rpmNoType) && _rpmNoType is PropertyAliasMapping _PAMNoType)
            {
                pam = _PAMNoType;
                return true;
            }

            pam = default;
            return false;
        }
    }

    #region Support Classes

    internal readonly struct RecordProperty (Type? type, string propertyName) : IRecordProperty
    {
        public RecordProperty (string propertyName) : this(null, propertyName)
        {
        }

        public string PropertyName { get; } = propertyName;

        public Type? Type { get; } = type;
    }

    internal sealed class RecordPropertyComparer : IEqualityComparer<IRecordProperty>
    {
        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals (IRecordProperty? x, IRecordProperty? y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            return x.Type == y.Type && x.PropertyName.Equals(y.PropertyName, StringComparison.OrdinalIgnoreCase);
        }

        public static int GetHashCode ([DisallowNull] IRecordProperty obj) => (obj.Type?.GetHashCode() ?? 0) ^ obj.PropertyName.GetHashCode(StringComparison.OrdinalIgnoreCase);

        bool IEqualityComparer<IRecordProperty>.Equals (IRecordProperty? x, IRecordProperty? y) => Equals(x, y);

        int IEqualityComparer<IRecordProperty>.GetHashCode ([DisallowNull] IRecordProperty obj) => GetHashCode(obj);
    }

    #endregion Support Classes
}