using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers
{
    internal interface IRecordProperty
    {
        public string PropertyName { get; }

        public Type? Type { get; }
    }

    public readonly struct PropertyAliasMapping (Type? type, string propertyName, string realPropertyName) : IRecordProperty
    {
        public string PropertyName { get; } = propertyName;
        public string RealPropertyName { get; } = realPropertyName;
        public Type? Type { get; } = type;
    }

    public readonly struct RecordPropertyMapping (Type? type, string propertyName, IRecordAction action) : IRecordProperty
    {
        public IRecordAction Action { get; } = action;

        public string PropertyName { get; } = propertyName;
        public Type? Type { get; } = type;
    }

    internal readonly struct RecordProperty (Type? type, string propertyName) : IRecordProperty
    {
        public string PropertyName { get; } = propertyName;

        public Type? Type { get; } = type;

        public RecordProperty (string propertyName) : this(null, propertyName)
        {
        }
    }

    public static class RecordPropertyMappings
    {
        private static readonly HashSet<IRecordProperty> propertyAliases = new(comparer: new RecordPropertyComparer());
        private static readonly HashSet<IRecordProperty> propertyMappings = new(comparer: new RecordPropertyComparer());

        static RecordPropertyMappings ()
        {
            PopulateMappings();
            PopulateAliases();
        }

        public static IReadOnlyList<string> GetAllAliases (Type type, string propertyName)
        {
            var list = propertyAliases.Where(p => p is PropertyAliasMapping pam && pam.RealPropertyName.Equals(propertyName) && (pam.Type == null || pam.Type == type)).Select(p => (PropertyAliasMapping)p).ToList();

            foreach (var item in list.ToArray().Where(l => l.Type == null && list.Count(i => i.PropertyName == l.PropertyName) > 1))
                _ = list.Remove(item);

            return list.Select(l => l.PropertyName).ToList().AsReadOnly();
        }

        public static bool TryFind (Type? type, string propertyName, out RecordPropertyMapping rpm)
        {
            if (TryFindMapping(type, propertyName, out rpm))
                return true;

            if (TryFindAlias(type, propertyName, out var pam))
                return TryFindMapping(type, pam.RealPropertyName, out rpm);

            return false;
        }

        internal static bool TryFindMapping (Type? type, string key, out RecordPropertyMapping rpm)
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

        private static void Add (Type? type, string propertyName, IRecordAction action) => _ = propertyMappings.Add(new RecordPropertyMapping(type, propertyName, action));

        private static void Add (Type? type, string propertyName, string realPropertyName)
        {
#if DEBUG
            if (!(TryFindMapping(type, realPropertyName, out _) || (type == null && propertyMappings.Any(p => p.PropertyName.Equals(realPropertyName)))))
                throw new Exception($"Invalid Alias - {type?.GetClassName()} - {realPropertyName}");
#endif
            _ = propertyAliases.Add(new PropertyAliasMapping(type, propertyName, realPropertyName));
        }

        private static void PopulateAliases ()
        {
            Add(null, "BAMT", nameof(IArmorGetter.AlternateBlockMaterial));
            Add(null, "DataFlags", nameof(IAmmunitionGetter.Flags));
            Add(null, "DESC", nameof(IAmmunitionGetter.Description));
            Add(null, "EAMT", nameof(IArmorGetter.EnchantmentAmount));
            Add(null, "EITM", nameof(IArmorGetter.ObjectEffect));
            Add(null, "ETYP", nameof(IWeaponGetter.EquipmentType));
            Add(null, "FULL", nameof(INamedGetter.Name));
            Add(null, "Item", nameof(IContainerGetter.Items));
            Add(null, "KWDA", nameof(IKeywordedGetter.Keywords));
            Add(null, "ONAM", nameof(INpcGetter.ShortName));
            Add(null, "RecordFlags", nameof(IAmmunitionGetter.MajorFlags));
            Add(null, "RNAM", nameof(INpcGetter.Race));
            Add(null, "XCWT", nameof(ICellGetter.Water));
            Add(null, "XEZN", nameof(ICellGetter.EncounterZone));
            Add(null, "XLCN", nameof(ICellGetter.Location));
            Add(null, "YNAM", nameof(IAmmunitionGetter.PickUpSound));
            Add(null, "ZNAM", nameof(IAmmunitionGetter.PutDownSound));
            Add(typeof(IAmmunitionGetter), "DMG", nameof(IAmmunitionGetter.Damage));
            Add(typeof(IArmorGetter), "BIDS", nameof(IArmorGetter.BashImpactDataSet));
            Add(typeof(IArmorGetter), "BMCT", nameof(IArmorGetter.RagdollConstraintTemplate));
            Add(typeof(IArmorGetter), "DNAM", nameof(IArmorGetter.ArmorRating));
            Add(typeof(IArmorGetter), "MODL", nameof(IArmorGetter.Armature));
            Add(typeof(IArmorGetter), "TNAM", nameof(IArmorGetter.TemplateArmor));
            Add(typeof(IBookGetter), "CNAM", nameof(IBookGetter.Description));
            Add(typeof(IBookGetter), "DESC", nameof(IBookGetter.BookText));
            Add(typeof(IBookGetter), "INAM", nameof(IBookGetter.InventoryArt));
            Add(typeof(ICellGetter), "LTMP", nameof(ICellGetter.LightingTemplate));
            Add(typeof(ICellGetter), "XCAS", nameof(ICellGetter.AcousticSpace));
            Add(typeof(ICellGetter), "XCCM", nameof(ICellGetter.SkyAndWeatherFromRegion));
            Add(typeof(ICellGetter), "XCIM", nameof(ICellGetter.ImageSpace));
            Add(typeof(ICellGetter), "XCLW", nameof(ICellGetter.WaterHeight));
            Add(typeof(ICellGetter), "XILL", nameof(ICellGetter.LockList));
            Add(typeof(ICellGetter), "XNAM", nameof(ICellGetter.WaterNoiseTexture));
            Add(typeof(ICellGetter), "XWEM", nameof(ICellGetter.WaterEnvironmentMap));
            Add(typeof(ICellGetter), "ZNAM", nameof(ICellGetter.Music));
            Add(typeof(IContainerGetter), "QNAM", nameof(IContainerGetter.CloseSound));
            Add(typeof(IContainerGetter), "SNAM", nameof(IContainerGetter.OpenSound));
            Add(typeof(IFactionGetter), "CRGR", nameof(IFactionGetter.SharedCrimeFactionList));
            Add(typeof(IFactionGetter), "JAIL", nameof(IFactionGetter.ExteriorJailMarker));
            Add(typeof(IFactionGetter), "JOUT", nameof(IFactionGetter.JailOutfit));
            Add(typeof(IFactionGetter), "PLCN", nameof(IFactionGetter.PlayerInventoryContainer));
            Add(typeof(IFactionGetter), "STOL", nameof(IFactionGetter.StolenGoodsContainer));
            Add(typeof(IFactionGetter), "VENC", nameof(IFactionGetter.MerchantContainer));
            Add(typeof(IFactionGetter), "VEND", nameof(IFactionGetter.VendorBuySellList));
            Add(typeof(IFactionGetter), "WAIT", nameof(IFactionGetter.FollowerWaitMarker));
            Add(typeof(IIngredientGetter), "ETYP", nameof(IIngredientGetter.EquipType));
            Add(typeof(INpcGetter), "ANAM", nameof(INpcGetter.FarAwayModel));
            Add(typeof(INpcGetter), "ATKR", nameof(INpcGetter.AttackRace));
            Add(typeof(INpcGetter), "CNAM", nameof(INpcGetter.Class));
            Add(typeof(INpcGetter), "CRIF", nameof(INpcGetter.CrimeFaction));
            Add(typeof(INpcGetter), "DOFT", nameof(INpcGetter.DefaultOutfit));
            Add(typeof(INpcGetter), "DPLT", nameof(INpcGetter.DefaultPackageList));
            Add(typeof(INpcGetter), "ECOR", nameof(INpcGetter.CombatOverridePackageList));
            Add(typeof(INpcGetter), "FTST", nameof(INpcGetter.HeadTexture));
            Add(typeof(INpcGetter), "GNAM", nameof(INpcGetter.GiftFilter));
            Add(typeof(INpcGetter), "GWOR", nameof(INpcGetter.GuardWarnOverridePackageList));
            Add(typeof(INpcGetter), "HCLF", nameof(INpcGetter.HairColor));
            Add(typeof(INpcGetter), "INAM", nameof(INpcGetter.DeathItem));
            Add(typeof(INpcGetter), "NAM6", nameof(INpcGetter.Height));
            Add(typeof(INpcGetter), "NAM7", nameof(INpcGetter.Weight));
            Add(typeof(INpcGetter), "NAM8", nameof(INpcGetter.SoundLevel));
            Add(typeof(INpcGetter), "OCOR", nameof(INpcGetter.ObserveDeadBodyOverridePackageList));
            Add(typeof(INpcGetter), "PKID", nameof(INpcGetter.Packages));
            Add(typeof(INpcGetter), "PNAM", nameof(INpcGetter.HeadParts));
            Add(typeof(INpcGetter), "SOFT", nameof(INpcGetter.SleepingOutfit));
            Add(typeof(INpcGetter), "SPLO", nameof(INpcGetter.ActorEffect));
            Add(typeof(INpcGetter), "SPOR", nameof(INpcGetter.SpectatorOverridePackageList));
            Add(typeof(INpcGetter), "TPLT", nameof(INpcGetter.Template));
            Add(typeof(INpcGetter), "VTCK", nameof(INpcGetter.Voice));
            Add(typeof(INpcGetter), "WNAM", nameof(INpcGetter.WornArmor));
            Add(typeof(INpcGetter), "ZNAM", nameof(INpcGetter.CombatStyle));
            Add(typeof(IScrollGetter), "MDOB", nameof(IScrollGetter.MenuDisplayObject));
            Add(typeof(IWeaponGetter), "BIDS", nameof(IWeaponGetter.BlockBashImpact));
            Add(typeof(IWeaponGetter), "INAM", nameof(IWeaponGetter.ImpactDataSet));
            Add(typeof(IWeaponGetter), "NAM7", nameof(IWeaponGetter.AttackLoopSound));
            Add(typeof(IWeaponGetter), "NAM8", nameof(IWeaponGetter.UnequipSound));
            Add(typeof(IWeaponGetter), "NAM9", nameof(IWeaponGetter.EquipSound));
            Add(typeof(IWeaponGetter), "SNAM", nameof(IWeaponGetter.AttackSound));
            Add(typeof(IWeaponGetter), "TNAM", nameof(IWeaponGetter.AttackFailSound));
            Add(typeof(IWeaponGetter), "UNAM", nameof(IWeaponGetter.IdleSound));
            Add(typeof(IWeaponGetter), "XNAM", nameof(IWeaponGetter.AttackSound2D));
            Add(typeof(IWorldspaceGetter), "CNAM", nameof(IWorldspaceGetter.Climate));
            Add(typeof(IWorldspaceGetter), "LTMP", nameof(IWorldspaceGetter.InteriorLighting));
            Add(typeof(IWorldspaceGetter), "NAM3", nameof(IWorldspaceGetter.LodWater));
            Add(typeof(IWorldspaceGetter), "NAM4", nameof(IWorldspaceGetter.LodWaterHeight));
            Add(typeof(IWorldspaceGetter), "NAMA", nameof(IWorldspaceGetter.DistantLodMultiplier));
            Add(typeof(IWorldspaceGetter), "ZNAM", nameof(IWorldspaceGetter.Music));
        }

        private static void PopulateMappings ()
        {
            #pragma warning disable format
            Add(typeof(IActorValueInformationGetter) ,  "Abbreviation"                              ,  Generic<string>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AccelerationRate"                          ,  Generic<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "AcousticSpace"                             ,  FormLink<IAcousticSpaceGetter>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "Action"                                    ,  Enums.Instance);
            Add(typeof(IActivatorGetter)             ,  "ActivateTextOverride"                      ,  Generic<string>.Instance);
            Add(typeof(IFloraGetter)                 ,  "ActivateTextOverride"                      ,  Generic<string>.Instance);
            Add(typeof(IActivatorGetter)             ,  "ActivationSound"                           ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "ActorCellMarkerReference"                  ,  FormLinks<IPlacedGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "ActorEffect"                               ,  FormLinks<ISpellRecordGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "ActorEffect"                               ,  FormLinks<ISpellRecordGetter>.Instance);
            Add(typeof(IIngestibleGetter)            ,  "Addiction"                                 ,  FormLink<ISkyrimMajorRecordGetter>.Instance);
            Add(typeof(IIngestibleGetter)            ,  "AddictionChance"                           ,  Generic<float>.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "AdditionalRaces"                           ,  FormLinks<IRaceGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModels"                               ,  FormLink<IDebrisGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsFadeInTime"                     ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsFadeOutTime"                    ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsScaleEnd"                       ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsScaleInTime"                    ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsScaleOutTime"                   ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsScaleStart"                     ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AimAngleTolerance"                         ,  Generic<float>.Instance);
            Add(typeof(IArmorGetter)                 ,  "AlternateBlockMaterial"                    ,  FormLink<IMaterialTypeGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AlternateBlockMaterial"                    ,  FormLink<IMaterialTypeGetter>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "AlternateSoundFor"                         ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IAcousticSpaceGetter)         ,  "AmbientSound"                              ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AmbientSound"                              ,  FormLink<ISoundGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "AngleThreshold"                            ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AngularAccelerationRate"                   ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AngularTolerance"                          ,  Generic<float>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "AnimationEvent"                            ,  Generic<string>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "AnimationGroupSection"                     ,  Generic<byte>.Instance);
            Add(typeof(IIdleMarkerGetter)            ,  "Animations"                                ,  FormLinks<IIdleAnimationGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "Armature"                                  ,  FormLinks<IArmorAddonGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "ArmorRace"                                 ,  FormLink<IRaceGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "ArmorRating"                               ,  Generic<float>.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "ArtObject"                                 ,  FormLink<IArtObjectGetter>.Instance);
            Add(typeof(IFurnitureGetter)             ,  "AssociatedSpell"                           ,  FormLink<ISpellGetter>.Instance);
            Add(typeof(IRelationshipGetter)          ,  "AssociationType"                           ,  FormLink<IAssociationTypeGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AttackFailSound"                           ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AttackLoopSound"                           ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "AttackRace"                                ,  FormLink<IRaceGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AttackRace"                                ,  FormLink<IRaceGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AttackSound"                               ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AttackSound2D"                             ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "AvoidThreatChance"                         ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "BackRun"                                   ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "BackWalk"                                  ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseCarryWeight"                           ,  Generic<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "BaseCost"                                  ,  Generic<float>.Instance);
            Add(typeof(IScrollGetter)                ,  "BaseCost"                                  ,  Generic<uint>.Instance);
            Add(typeof(ISpellGetter)                 ,  "BaseCost"                                  ,  Generic<uint>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "BaseEnchantment"                           ,  FormLink<IObjectEffectGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMass"                                  ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultFly"                    ,  FormLink<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultRun"                    ,  FormLink<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultSneak"                  ,  FormLink<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultSprint"                 ,  FormLink<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultSwim"                   ,  FormLink<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultWalk"                   ,  FormLink<IMovementTypeGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "BashImpactDataSet"                         ,  FormLink<IImpactDataSetGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "BirthPositionOffset"                       ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "BirthPositionOffsetRangePlusMinus"         ,  Generic<float>.Instance);
            Add(typeof(IClassGetter)                 ,  "BleedoutDefault"                           ,  Generic<float>.Instance);
            Add(typeof(IWeaponGetter)                ,  "BlockBashImpact"                           ,  FormLink<IImpactDataSetGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BodyBipedObject"                           ,  Enums.Instance);
            Add(typeof(IRaceGetter)                  ,  "BodyPartData"                              ,  FormLink<IBodyPartDataGetter>.Instance);
            Add(typeof(IBookGetter)                  ,  "BookText"                                  ,  Generic<string>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "BoxSize"                                   ,  Generic<uint>.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "Branch"                                    ,  FormLink<IDialogBranchGetter>.Instance);
            Add(typeof(ITreeGetter)                  ,  "BranchFlexibility"                         ,  Generic<float>.Instance);
            Add(typeof(IDialogViewGetter)            ,  "Branches"                                  ,  FormLinks<IDialogBranchGetter>.Instance);
            Add(typeof(IMaterialTypeGetter)          ,  "Buoyancy"                                  ,  Generic<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "CSGDDataTypeState"                         ,  Flags.Instance);
            Add(typeof(IScrollGetter)                ,  "CastDuration"                              ,  Generic<float>.Instance);
            Add(typeof(ISpellGetter)                 ,  "CastDuration"                              ,  Generic<float>.Instance);
            Add(null                                 ,  "CastType"                                  ,  Enums.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "CastingArt"                                ,  FormLink<IArtObjectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "CastingLight"                              ,  FormLink<ILightGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "CastingSoundLevel"                         ,  Enums.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "Category"                                  ,  FormLink<ISoundCategoryGetter>.Instance);
            Add(typeof(IDialogBranchGetter)          ,  "Category"                                  ,  Enums.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "Category"                                  ,  Enums.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "CenterOffsetMax"                           ,  Generic<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "CenterOffsetMin"                           ,  Generic<float>.Instance);
            Add(null                                 ,  "ChargeTime"                                ,  Generic<float>.Instance);
            Add(typeof(IRelationshipGetter)          ,  "Child"                                     ,  FormLink<INpcGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Class"                                     ,  FormLink<IClassGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "Climate"                                   ,  FormLink<IClimateGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "CloseLootSound"                            ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IContainerGetter)             ,  "CloseSound"                                ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IDoorGetter)                  ,  "CloseSound"                                ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(ICollisionLayerGetter)        ,  "CollidesWith"                              ,  FormLinks<ICollisionLayerGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "CollisionLayer"                            ,  FormLink<ICollisionLayerGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "CollisionRadius"                           ,  Generic<float>.Instance);
            Add(typeof(IHeadPartGetter)              ,  "Color"                                     ,  FormLink<IColorRecordGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey1Alpha"                            ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey1Time"                             ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey2Alpha"                            ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey2Time"                             ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey3Alpha"                            ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey3Time"                             ,  Generic<float>.Instance);
            Add(typeof(IGrassGetter)                 ,  "ColorRange"                                ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorScale"                                ,  Generic<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "CombatOverridePackageList"                 ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "CombatStyle"                               ,  FormLink<ICombatStyleGetter>.Instance);
            Add(typeof(IPackageGetter)               ,  "CombatStyle"                               ,  FormLink<ICombatStyleGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "ConeSpread"                                ,  Generic<float>.Instance);
            Add(typeof(IIngestibleGetter)            ,  "ConsumeSound"                              ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(ISoulGemGetter)               ,  "ContainedSoul"                             ,  Enums.Instance);
            Add(typeof(IProjectileGetter)            ,  "CountdownSound"                            ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "CounterEffects"                            ,  FormLinks<IMagicEffectGetter>.Instance);
            Add(typeof(IConstructibleObjectGetter)   ,  "CreatedObject"                             ,  FormLink<IConstructibleGetter>.Instance);
            Add(typeof(IConstructibleObjectGetter)   ,  "CreatedObjectCount"                        ,  Generic<ushort>.Instance);
            Add(typeof(INpcGetter)                   ,  "CrimeFaction"                              ,  FormLink<IFactionGetter>.Instance);
            Add(typeof(IStaticGetter)                ,  "DNAMDataTypeState"                         ,  Flags.Instance);
            Add(typeof(IWaterGetter)                 ,  "DNAMDataTypeState"                         ,  Flags.Instance);
            Add(typeof(IAmmunitionGetter)            ,  "Damage"                                    ,  Generic<float>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Damage"                                    ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DamagePerSecond"                           ,  Generic<ushort>.Instance);
            Add(typeof(IPackageGetter)               ,  "DataInputVersion"                          ,  Generic<int>.Instance);
            Add(typeof(INpcGetter)                   ,  "DeathItem"                                 ,  FormLink<ILeveledItemGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "DecalData"                                 ,  FormLink<ITextureSetGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "DecapitationFX"                            ,  FormLink<IArtObjectGetter>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "DecayHfRatio"                              ,  Generic<float>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "DecayMilliseconds"                         ,  Generic<ushort>.Instance);
            Add(typeof(IRaceGetter)                  ,  "DecelerationRate"                          ,  Generic<float>.Instance);
            Add(typeof(ISoundCategoryGetter)         ,  "DefaultMenuVolume"                         ,  Generic<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "DefaultOutfit"                             ,  FormLink<IOutfitGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "DefaultPackageList"                        ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "DefaultWeaponSource"                       ,  FormLink<IWeaponGetter>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "DefensiveMult"                             ,  Generic<float>.Instance);
            Add(typeof(IGrassGetter)                 ,  "Density"                                   ,  Generic<byte>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DepthNormals"                              ,  Generic<float>.Instance);
            Add(typeof(IImageSpaceAdapterGetter)     ,  "DepthOfFieldFlags"                         ,  Flags.Instance);
            Add(typeof(IWaterGetter)                 ,  "DepthReflections"                          ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DepthRefraction"                           ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DepthSpecularLighting"                     ,  Generic<float>.Instance);
            Add(null                                 ,  "Description"                               ,  Generic<string>.Instance);
            Add(typeof(IWeaponGetter)                ,  "DetectionSoundLevel"                       ,  Enums.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "DetectionSoundValue"                       ,  Generic<byte>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "DirectionalFade"                           ,  Generic<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "DirectionalRotationXY"                     ,  Generic<int>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "DirectionalRotationZ"                      ,  Generic<int>.Instance);
            Add(typeof(IProjectileGetter)            ,  "DisaleSound"                               ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementDampner"                       ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementFalloff"                       ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementFoce"                          ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementStartingSize"                  ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementVelocity"                      ,  Generic<float>.Instance);
            Add(typeof(IMessageGetter)               ,  "DisplayTime"                               ,  Generic<uint>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "DistantLodMultiplier"                      ,  Generic<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "DualCastArt"                               ,  FormLink<IDualCastDataGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "DualCastScale"                             ,  Generic<float>.Instance);
            Add(null                                 ,  "Duration"                                  ,  Generic<float>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "ENITDataTypeState"                         ,  Flags.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectAlphaFadeInTime"                 ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectAlphaFadeOutTime"                ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectAlphaPulseAmplitude"             ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectAlphaPulseFrequency"             ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectFallOff"                         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectFullAlphaRatio"                  ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectFullAlphaTime"                   ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectPersistentAlphaRatio"            ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeWidth"                                 ,  Generic<float>.Instance);
            Add(typeof(IVisualEffectGetter)          ,  "EffectArt"                                 ,  FormLink<IArtObjectGetter>.Instance);
            Add(typeof(IDualCastDataGetter)          ,  "EffectShader"                              ,  FormLink<IEffectShaderGetter>.Instance);
            Add(null                                 ,  "Effects"                                   ,  FormLinksWithData<EffectsAction, IMagicEffectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "EnchantArt"                                ,  FormLink<IArtObjectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "EnchantShader"                             ,  FormLink<IEffectShaderGetter>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "EnchantType"                               ,  Enums.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "EnchantVisuals"                            ,  FormLink<IVisualEffectGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "EnchantmentAmount"                         ,  Generic<ushort>.Instance);
            Add(typeof(IWeaponGetter)                ,  "EnchantmentAmount"                         ,  Generic<ushort>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "EnchantmentAmount"                         ,  Generic<int>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "EnchantmentCost"                           ,  Generic<uint>.Instance);
            Add(typeof(ICellGetter)                  ,  "EncounterZone"                             ,  FormLink<IEncounterZoneGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "EncounterZone"                             ,  FormLink<IEncounterZoneGetter>.Instance);
            Add(typeof(IAcousticSpaceGetter)         ,  "EnvironmentType"                           ,  FormLink<IReverbParametersGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "EquipAbility"                              ,  FormLink<ISpellGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "EquipSound"                                ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IIngredientGetter)            ,  "EquipType"                                 ,  FormLink<IEquipTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "EquipmentFlags"                            ,  Flags.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultMagic"                   ,  Generic<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultMelee"                   ,  Generic<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultRanged"                  ,  Generic<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultShout"                   ,  Generic<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultStaff"                   ,  Generic<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultUnarmed"                 ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "EquipmentSlots"                            ,  FormLinks<IEquipTypeGetter>.Instance);
            Add(null                                 ,  "EquipmentType"                             ,  FormLink<IEquipTypeGetter>.Instance);
            Add(null                                 ,  "Explosion"                                 ,  FormLink<IExplosionGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "ExplosionAltTriggerProximity"              ,  Generic<float>.Instance);
            Add(typeof(IProjectileGetter)            ,  "ExplosionAltTriggerTimer"                  ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ExplosionWindSpeed"                        ,  Generic<float>.Instance);
            Add(typeof(IFactionGetter)               ,  "ExteriorJailMarker"                        ,  FormLink<IPlacedObjectGetter>.Instance);
            Add(typeof(IHeadPartGetter)              ,  "ExtraParts"                                ,  FormLinks<IHeadPartGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "Eyes"                                      ,  FormLinks<IEyesGetter>.Instance);
            Add(typeof(ITalkingActivatorGetter)      ,  "FNAM"                                      ,  Generic<short>.Instance);
            Add(typeof(ILightGetter)                 ,  "FOV"                                       ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "FacegenFaceClamp"                          ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "FacegenMainClamp"                          ,  Generic<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "FactionRank"                               ,  Generic<int>.Instance);
            Add(typeof(IMusicTypeGetter)             ,  "FadeDuration"                              ,  Generic<float>.Instance);
            Add(typeof(IProjectileGetter)            ,  "FadeDuration"                              ,  Generic<float>.Instance);
            Add(typeof(IMusicTrackGetter)            ,  "FadeOut"                                   ,  Generic<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "FadeValue"                                 ,  Generic<float>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "FalloffBias"                               ,  Generic<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "FalloffExponent"                           ,  Generic<float>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "FalloffScale"                              ,  Generic<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "FarAwayModel"                              ,  FormLink<IArmorGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillAlphaFadeInTime"                       ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillAlphaPulseAmplitude"                   ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillAlphaPulseFrequency"                   ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey1Scale"                        ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey1Time"                         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey2Scale"                        ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey2Time"                         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey3Scale"                        ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey3Time"                         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillFadeOutTime"                           ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillFullAlphaRatio"                        ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillFullAlphaTime"                         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillPersistentAlphaRatio"                  ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillTextureAnimationSpeedU"                ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillTextureAnimationSpeedV"                ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillTextureScaleU"                         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillTextureScaleV"                         ,  Generic<float>.Instance);
            Add(typeof(IQuestGetter)                 ,  "Filter"                                    ,  Generic<string>.Instance);
            Add(typeof(IWeaponGetter)                ,  "FirstPersonModel"                          ,  FormLink<IStaticGetter>.Instance);
            Add(null                                 ,  "Flags"                                     ,  Flags.Instance);
            Add(typeof(ILightGetter)                 ,  "FlickerIntensityAmplitude"                 ,  Generic<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "FlickerMovementAmplitude"                  ,  Generic<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "FlickerPeriod"                             ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "FlightRadius"                              ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogAboveWaterAmount"                       ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogAboveWaterDistanceFarPlane"             ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogAboveWaterDistanceNearPlane"            ,  Generic<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogClipDistance"                           ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceDayFar"                         ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceDayMax"                         ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceDayNear"                        ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceDayPower"                       ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceNightFar"                       ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceNightMax"                       ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceNightNear"                      ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceNightPower"                     ,  Generic<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogFar"                                    ,  Generic<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogMax"                                    ,  Generic<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogNear"                                   ,  Generic<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogPower"                                  ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogUnderWaterAmount"                       ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogUnderWaterDistanceFarPlane"             ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogUnderWaterDistanceNearPlane"            ,  Generic<float>.Instance);
            Add(typeof(IFactionGetter)               ,  "FollowerWaitMarker"                        ,  FormLink<IPlacedObjectGetter>.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "FootstepSound"                             ,  FormLink<IFootstepSetGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Force"                                     ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "ForwardRun"                                ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "ForwardWalk"                               ,  Generic<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "GiftFilter"                                ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(ILeveledItemGetter)           ,  "Global"                                    ,  FormLink<IGlobalGetter>.Instance);
            Add(typeof(ILeveledNpcGetter)            ,  "Global"                                    ,  FormLink<IGlobalGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "Grasses"                                   ,  FormLinks<IGrassGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "Gravity"                                   ,  Generic<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "GravityVelocity"                           ,  Generic<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "GroupOffensiveMult"                        ,  Generic<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "GuardWarnOverridePackageList"              ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "HairBipedObject"                           ,  Enums.Instance);
            Add(typeof(INpcGetter)                   ,  "HairColor"                                 ,  FormLink<IColorRecordGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "Hairs"                                     ,  FormLinks<IHairGetter>.Instance);
            Add(typeof(IScrollGetter)                ,  "HalfCostPerk"                              ,  FormLink<IPerkGetter>.Instance);
            Add(typeof(ISpellGetter)                 ,  "HalfCostPerk"                              ,  FormLink<IPerkGetter>.Instance);
            Add(typeof(IFloraGetter)                 ,  "HarvestSound"                              ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(ITreeGetter)                  ,  "HarvestSound"                              ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "HavokFriction"                             ,  Generic<byte>.Instance);
            Add(typeof(IMaterialTypeGetter)          ,  "HavokImpactDataSet"                        ,  FormLink<IImpactDataSetGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "HavokRestitution"                          ,  Generic<byte>.Instance);
            Add(typeof(IImpactGetter)                ,  "Hazard"                                    ,  FormLink<IHazardGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "HeadBipedObject"                           ,  Enums.Instance);
            Add(typeof(INpcGetter)                   ,  "HeadParts"                                 ,  FormLinks<IHeadPartGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "HeadTexture"                               ,  FormLink<ITextureSetGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Height"                                    ,  Generic<float>.Instance);
            Add(typeof(IGrassGetter)                 ,  "HeightRange"                               ,  Generic<float>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "HfReferenceHertz"                          ,  Generic<ushort>.Instance);
            Add(typeof(IDualCastDataGetter)          ,  "HitEffectArt"                              ,  FormLink<IArtObjectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "HitEffectArt"                              ,  FormLink<IArtObjectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "HitShader"                                 ,  FormLink<IEffectShaderGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "HitVisuals"                                ,  FormLink<IVisualEffectGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "HolesEndTime"                              ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "HolesEndValue"                             ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "HolesStartTime"                            ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "HolesStartValue"                           ,  Generic<float>.Instance);
            Add(typeof(ILocationGetter)              ,  "HorseMarkerRef"                            ,  FormLink<IPlacedObjectGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "ISRadius"                                  ,  Generic<float>.Instance);
            Add(typeof(IClassGetter)                 ,  "Icon"                                      ,  Generic<string>.Instance);
            Add(typeof(IWeaponGetter)                ,  "IdleSound"                                 ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IIdleMarkerGetter)            ,  "IdleTimer"                                 ,  Generic<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "ImageSpace"                                ,  FormLink<IImageSpaceGetter>.Instance);
            Add(typeof(IWaterGetter)                 ,  "ImageSpace"                                ,  FormLink<IImageSpaceGetter>.Instance);
            Add(null                                 ,  "ImageSpaceModifier"                        ,  FormLink<IImageSpaceAdapterGetter>.Instance);
            Add(typeof(IHazardGetter)                ,  "ImageSpaceRadius"                          ,  Generic<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "ImpactData"                                ,  FormLink<IImpactDataSetGetter>.Instance);
            Add(null                                 ,  "ImpactDataSet"                             ,  FormLink<IImpactDataSetGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "ImpactForce"                               ,  Generic<float>.Instance);
            Add(typeof(ICollisionLayerGetter)        ,  "Index"                                     ,  Generic<uint>.Instance);
            Add(typeof(IFloraGetter)                 ,  "Ingredient"                                ,  FormLink<IHarvestTargetGetter>.Instance);
            Add(typeof(ITreeGetter)                  ,  "Ingredient"                                ,  FormLink<IHarvestTargetGetter>.Instance);
            Add(typeof(IIngredientGetter)            ,  "IngredientValue"                           ,  Generic<int>.Instance);
            Add(typeof(IDualCastDataGetter)          ,  "InheritScale"                              ,  Flags.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "InitialRotationRange"                      ,  Generic<float>.Instance);
            Add(typeof(ILoadScreenGetter)            ,  "InitialScale"                              ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "InjuredHealthPercent"                      ,  Generic<float>.Instance);
            Add(typeof(IActivatorGetter)             ,  "InteractionKeyword"                        ,  FormLink<IKeywordGetter>.Instance);
            Add(typeof(IFurnitureGetter)             ,  "InteractionKeyword"                        ,  FormLink<IKeywordGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "InteriorLighting"                          ,  FormLink<ILightingTemplateGetter>.Instance);
            Add(typeof(IPackageGetter)               ,  "InterruptOverride"                         ,  Enums.Instance);
            Add(typeof(IPackageGetter)               ,  "InteruptFlags"                             ,  Flags.Instance);
            Add(typeof(IBookGetter)                  ,  "InventoryArt"                              ,  FormLink<IStaticGetter>.Instance);
            Add(typeof(IOutfitGetter)                ,  "Items"                                     ,  FormLinks<IOutfitTargetGetter>.Instance);
            Add(typeof(IFormListGetter)              ,  "Items"                                     ,  FormLinks<ISkyrimMajorRecordGetter>.Instance);
            Add(null                                 ,  "Items"                                     ,  FormLinksWithData<ContainerItemsAction, IItemGetter>.Instance);
            Add(typeof(IFactionGetter)               ,  "JailOutfit"                                ,  FormLink<IOutfitGetter>.Instance);
            Add(null                                 ,  "Keywords"                                  ,  FormLinks<IKeywordGetter>.Instance);
            Add(typeof(ISceneGetter)                 ,  "LastActionIndex"                           ,  Generic<uint>.Instance);
            Add(typeof(ITreeGetter)                  ,  "LeafAmplitude"                             ,  Generic<float>.Instance);
            Add(typeof(ITreeGetter)                  ,  "LeafFrequency"                             ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "LeftRun"                                   ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "LeftWalk"                                  ,  Generic<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "Lens"                                      ,  FormLink<ILensFlareGetter>.Instance);
            Add(typeof(IPerkGetter)                  ,  "Level"                                     ,  Generic<byte>.Instance);
            Add(typeof(IHazardGetter)                ,  "Lifetime"                                  ,  Generic<float>.Instance);
            Add(typeof(IProjectileGetter)            ,  "Lifetime"                                  ,  Generic<float>.Instance);
            Add(null                                 ,  "Light"                                     ,  FormLink<ILightGetter>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "LightFadeEndDistance"                      ,  Generic<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "LightFadeStartDistance"                    ,  Generic<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "LightingTemplate"                          ,  FormLink<ILightingTemplateGetter>.Instance);
            Add(typeof(IHazardGetter)                ,  "Limit"                                     ,  Generic<uint>.Instance);
            Add(typeof(ISoulGemGetter)               ,  "LinkedTo"                                  ,  FormLink<ISoulGemGetter>.Instance);
            Add(typeof(ILoadScreenGetter)            ,  "LoadingScreenNif"                          ,  FormLink<IStaticGetter>.Instance);
            Add(null                                 ,  "Location"                                  ,  FormLink<ILocationGetter>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "Location"                                  ,  Enums.Instance);
            Add(typeof(ILocationGetter)              ,  "LocationCellMarkerReference"               ,  FormLinks<IPlacedGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "LockList"                                  ,  FormLink<ILockListGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "LodWater"                                  ,  FormLink<IWaterGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "LodWaterHeight"                            ,  Generic<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "LongRangeStrafeMult"                       ,  Generic<float>.Instance);
            Add(typeof(IDoorGetter)                  ,  "LoopSound"                                 ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "LoopingSecondsMax"                         ,  Generic<byte>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "LoopingSecondsMin"                         ,  Generic<byte>.Instance);
            Add(typeof(IActivatorGetter)             ,  "LoopingSound"                              ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IMoveableStaticGetter)        ,  "LoopingSound"                              ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(ITalkingActivatorGetter)      ,  "LoopingSound"                              ,  FormLink<ISoundMarkerGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "MagicSkill"                                ,  Enums.Instance);
            Add(null                                 ,  "MajorFlags"                                ,  Flags.Instance);
            Add(typeof(IAddonNodeGetter)             ,  "MasterParticleSystemCap"                   ,  Generic<ushort>.Instance);
            Add(typeof(IStaticGetter)                ,  "Material"                                  ,  FormLink<IMaterialObjectGetter>.Instance);
            Add(typeof(IWaterGetter)                 ,  "Material"                                  ,  FormLink<IMaterialTypeGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "MaterialType"                              ,  FormLink<IMaterialTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "MaterialType"                              ,  FormLink<IMaterialTypeGetter>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "MaterialUvScale"                           ,  Generic<float>.Instance);
            Add(typeof(IStaticGetter)                ,  "MaxAngle"                                  ,  Generic<float>.Instance);
            Add(typeof(IEncounterZoneGetter)         ,  "MaxLevel"                                  ,  Generic<sbyte>.Instance);
            Add(typeof(IGrassGetter)                 ,  "MaxSlope"                                  ,  Generic<byte>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "MaxTime"                                   ,  Generic<float>.Instance);
            Add(typeof(IClassGetter)                 ,  "MaxTrainingLevel"                          ,  Generic<byte>.Instance);
            Add(typeof(ISoulGemGetter)               ,  "MaximumCapacity"                           ,  Enums.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "MembraneBlendOperation"                    ,  Enums.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "MembraneDestBlendMode"                     ,  Enums.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "MembraneSourceBlendMode"                   ,  Enums.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "MembraneZTest"                             ,  Enums.Instance);
            Add(null                                 ,  "MenuDisplayObject"                         ,  FormLink<IStaticGetter>.Instance);
            Add(typeof(IFactionGetter)               ,  "MerchantContainer"                         ,  FormLink<IPlacedObjectGetter>.Instance);
            Add(typeof(IEncounterZoneGetter)         ,  "MinLevel"                                  ,  Generic<sbyte>.Instance);
            Add(typeof(IGrassGetter)                 ,  "MinSlope"                                  ,  Generic<byte>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "MinTime"                                   ,  Generic<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "MinimumSkillLevel"                         ,  Generic<uint>.Instance);
            Add(typeof(IClimateGetter)               ,  "Moons"                                     ,  Flags.Instance);
            Add(typeof(IRaceGetter)                  ,  "MorphRace"                                 ,  FormLink<IRaceGetter>.Instance);
            Add(null                                 ,  "Music"                                     ,  FormLink<IMusicTypeGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "MuzzleFlash"                               ,  FormLink<ILightGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "MuzzleFlashDuration"                       ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "NAM0DataTypeState"                         ,  Flags.Instance);
            Add(typeof(INpcGetter)                   ,  "NAM5"                                      ,  Generic<ushort>.Instance);
            Add(null                                 ,  "Name"                                      ,  Generic<string>.Instance);
            Add(typeof(ILightGetter)                 ,  "NearClip"                                  ,  Generic<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "NearTargetDistance"                        ,  Generic<float>.Instance);
            Add(typeof(IQuestGetter)                 ,  "NextAliasID"                               ,  Generic<uint>.Instance);
            Add(typeof(IPerkGetter)                  ,  "NextPerk"                                  ,  FormLink<IPerkGetter>.Instance);
            Add(typeof(IAddonNodeGetter)             ,  "NodeIndex"                                 ,  Generic<int>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseFalloff"                              ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseFlowmapScale"                         ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerOneAmplitudeScale"               ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerOneUvScale"                      ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerOneWindDirection"                ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerOneWindSpeed"                    ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerThreeAmplitudeScale"             ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerThreeUvScale"                    ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerThreeWindDirection"              ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerThreeWindSpeed"                  ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerTwoAmplitudeScale"               ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerTwoUvScale"                      ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerTwoWindDirection"                ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerTwoWindSpeed"                    ,  Generic<float>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "NoiseUvScale"                              ,  Generic<float>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "NormalDampener"                            ,  Generic<float>.Instance);
            Add(typeof(IPerkGetter)                  ,  "NumRanks"                                  ,  Generic<byte>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "NumSubtexturesX"                           ,  Generic<uint>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "NumSubtexturesY"                           ,  Generic<uint>.Instance);
            Add(typeof(IRaceGetter)                  ,  "NumberOfTintsInList"                       ,  Generic<ushort>.Instance);
            Add(null                                 ,  "ObjectEffect"                              ,  FormLink<IEffectRecordGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "ObserveDeadBodyOverridePackageList"        ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "OffensiveMult"                             ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "Opacity"                                   ,  Generic<byte>.Instance);
            Add(typeof(IRaceGetter)                  ,  "OpenLootSound"                             ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(null                                 ,  "OpenSound"                                 ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "Orientation"                               ,  Enums.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "OutputModel"                               ,  FormLink<ISoundOutputModelGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "Owner"                                     ,  FormLink<IOwnerGetter>.Instance);
            Add(typeof(IEncounterZoneGetter)         ,  "Owner"                                     ,  FormLink<IOwnerGetter>.Instance);
            Add(typeof(IPackageGetter)               ,  "OwnerQuest"                                ,  FormLink<IQuestGetter>.Instance);
            Add(typeof(ITalkingActivatorGetter)      ,  "PNAM"                                      ,  Generic<int>.Instance);
            Add(typeof(IPackageGetter)               ,  "PackageTemplate"                           ,  FormLink<IPackageGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Packages"                                  ,  FormLinks<IPackageGetter>.Instance);
            Add(typeof(IRelationshipGetter)          ,  "Parent"                                    ,  FormLink<INpcGetter>.Instance);
            Add(typeof(ISoundCategoryGetter)         ,  "Parent"                                    ,  FormLink<ISoundCategoryGetter>.Instance);
            Add(typeof(IMaterialTypeGetter)          ,  "Parent"                                    ,  FormLink<IMaterialTypeGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "ParentLocation"                            ,  FormLink<ILocationGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAcceleration1"                     ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAcceleration2"                     ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAcceleration3"                     ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAccelerationAlongNormal"           ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedEndFrame"                  ,  Generic<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedFrameCount"                ,  Generic<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedFrameCountVariation"       ,  Generic<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedLoopStartFrame"            ,  Generic<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedLoopStartVariation"        ,  Generic<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedStartFrame"                ,  Generic<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedStartFrameVariation"       ,  Generic<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleBirthRampDownTime"                 ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleBirthRampUpTime"                   ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleBlendOperation"                    ,  Enums.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "ParticleDensity"                           ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleDestBlendMode"                     ,  Enums.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleFullBirthRatio"                    ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleFullBirthTime"                     ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialRotationDegree"             ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialRotationDegreePlusMinus"    ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialSpeedAlongNormal"           ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialSpeedAlongNormalPlusMinus"  ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialVelocity1"                  ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialVelocity2"                  ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialVelocity3"                  ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleLifetime"                          ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleLifetimePlusMinus"                 ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticlePeristentCount"                    ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleRotationSpeedDegreePerSec"         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleRotationSpeedDegreePerSecPlusMinus",  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleScaleKey1"                         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleScaleKey1Time"                     ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleScaleKey2"                         ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleScaleKey2Time"                     ,  Generic<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "ParticleSizeX"                             ,  Generic<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "ParticleSizeY"                             ,  Generic<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleSourceBlendMode"                   ,  Enums.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleZTest"                             ,  Enums.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "PerkToApply"                               ,  FormLink<IPerkGetter>.Instance);
            Add(typeof(IClimateGetter)               ,  "PhaseLength"                               ,  Generic<byte>.Instance);
            Add(null                                 ,  "PickUpSound"                               ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "PlacedObject"                              ,  FormLink<IExplodeSpawnGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "PlacementRadius"                           ,  Generic<float>.Instance);
            Add(typeof(IFactionGetter)               ,  "PlayerInventoryContainer"                  ,  FormLink<IPlacedObjectGetter>.Instance);
            Add(typeof(IGrassGetter)                 ,  "PositionRange"                             ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "Precipitation"                             ,  FormLink<IShaderParticleGeometryGetter>.Instance);
            Add(typeof(IPackageGetter)               ,  "PreferredSpeed"                            ,  Enums.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "Priority"                                  ,  Generic<float>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "Priority"                                  ,  Generic<sbyte>.Instance);
            Add(typeof(IQuestGetter)                 ,  "Priority"                                  ,  Generic<byte>.Instance);
            Add(null                                 ,  "Projectile"                                ,  FormLink<IProjectileGetter>.Instance);
            Add(null                                 ,  "PutDownSound"                              ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IAlchemicalApparatusGetter)   ,  "Quality"                                   ,  Enums.Instance);
            Add(null                                 ,  "Quest"                                     ,  FormLink<IQuestGetter>.Instance);
            Add(typeof(IQuestGetter)                 ,  "QuestFormVersion"                          ,  Generic<byte>.Instance);
            Add(null                                 ,  "Race"                                      ,  FormLink<IRaceGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Radius"                                    ,  Generic<float>.Instance);
            Add(typeof(IHazardGetter)                ,  "Radius"                                    ,  Generic<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "Radius"                                    ,  Generic<uint>.Instance);
            Add(typeof(IArmorGetter)                 ,  "RagdollConstraintTemplate"                 ,  Generic<string>.Instance);
            Add(null                                 ,  "Range"                                     ,  Generic<float>.Instance);
            Add(typeof(IRelationshipGetter)          ,  "Rank"                                      ,  Enums.Instance);
            Add(typeof(IEncounterZoneGetter)         ,  "Rank"                                      ,  Generic<sbyte>.Instance);
            Add(typeof(ILocationGetter)              ,  "ReferenceCellPersistentReferences"         ,  FormLinks<IPlacedSimpleGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "ReferenceCellStaticReferences"             ,  FormLinks<IPlacedSimpleGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "ReferenceCellUnique"                       ,  FormLinks<INpcGetter>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "ReflectDelayMS"                            ,  Generic<byte>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "Reflections"                               ,  Generic<sbyte>.Instance);
            Add(typeof(ICellGetter)                  ,  "Regions"                                   ,  FormLinks<IRegionGetter>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "RelatedIdles"                              ,  FormLinks<IIdleRelationGetter>.Instance);
            Add(typeof(ICameraPathGetter)            ,  "RelatedPaths"                              ,  FormLinks<ICameraPathGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "RelaunchInterval"                          ,  Generic<float>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "ReplayDelay"                               ,  Generic<ushort>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "ResistValue"                               ,  Enums.Instance);
            Add(typeof(IImpactGetter)                ,  "Result"                                    ,  Enums.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "ReverbAmp"                                 ,  Generic<sbyte>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "ReverbDelayMS"                             ,  Generic<byte>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RightRun"                                  ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RightWalk"                                 ,  Generic<float>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "RoomFilter"                                ,  Generic<sbyte>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "RoomHfFilter"                              ,  Generic<sbyte>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RotateInPlaceRun"                          ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RotateInPlaceWalk"                         ,  Generic<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RotateWhileMovingRun"                      ,  Generic<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "RotationVelocity"                          ,  Generic<float>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "RunForwardAlternateFootsteps"              ,  FormLinks<IFootstepGetter>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "RunForwardFootsteps"                       ,  FormLinks<IFootstepGetter>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "SPEDDataTypeState"                         ,  Flags.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "SceneGraphEmitDepthLimit"                  ,  Generic<uint>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleDate"                              ,  Generic<byte>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleDayOfWeek"                         ,  Enums.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleDurationInMinutes"                 ,  Generic<int>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleHour"                              ,  Generic<sbyte>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleMinute"                            ,  Generic<sbyte>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleMonth"                             ,  Generic<sbyte>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "ScriptEffectAIDelayTime"                   ,  Generic<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "ScriptEffectAIScore"                       ,  Generic<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SecondActorValue"                          ,  Enums.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SecondActorValueWeight"                    ,  Generic<float>.Instance);
            Add(typeof(IImpactGetter)                ,  "SecondaryTextureSet"                       ,  FormLink<ITextureSetGetter>.Instance);
            Add(typeof(IVisualEffectGetter)          ,  "Shader"                                    ,  FormLink<IEffectShaderGetter>.Instance);
            Add(typeof(IFactionGetter)               ,  "SharedCrimeFactionList"                    ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "ShieldBipedObject"                         ,  Enums.Instance);
            Add(typeof(IAmmunitionGetter)            ,  "ShortName"                                 ,  Generic<string>.Instance);
            Add(typeof(INpcGetter)                   ,  "ShortName"                                 ,  Generic<string>.Instance);
            Add(typeof(ICameraPathGetter)            ,  "Shots"                                     ,  FormLinks<ICameraShotGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "Size"                                      ,  Enums.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SkillUsageMultiplier"                      ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "Skin"                                      ,  FormLink<IArmorGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "SkyAndWeatherFromRegion"                   ,  FormLink<IRegionGetter>.Instance);
            Add(typeof(IWeatherGetter)               ,  "SkyStatics"                                ,  FormLinks<IStaticGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "SleepingOutfit"                            ,  FormLink<IOutfitGetter>.Instance);
            Add(typeof(IEquipTypeGetter)             ,  "SlotParents"                               ,  FormLinks<IEquipTypeGetter>.Instance);
            Add(null                                 ,  "Sound"                                     ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Sound1"                                    ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "Sound1"                                    ,  FormLink<ISoundGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Sound2"                                    ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "Sound2"                                    ,  FormLink<ISoundGetter>.Instance);
            Add(typeof(ISoundMarkerGetter)           ,  "SoundDescriptor"                           ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(null                                 ,  "SoundLevel"                                ,  Enums.Instance);
            Add(typeof(IProjectileGetter)            ,  "SoundLevel"                                ,  Generic<uint>.Instance);
            Add(typeof(IExplosionGetter)             ,  "SpawnProjectile"                           ,  FormLink<IProjectileGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "SpectatorOverridePackageList"              ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularBrightness"                        ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularPower"                             ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularRadius"                            ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularSunPower"                          ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularSunSparkleMagnitude"               ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularSunSparklePower"                   ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularSunSpecularMagnitude"              ,  Generic<float>.Instance);
            Add(typeof(IProjectileGetter)            ,  "Speed"                                     ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "Spell"                                     ,  FormLink<ISpellGetter>.Instance);
            Add(typeof(IHazardGetter)                ,  "Spell"                                     ,  FormLink<IEffectRecordGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SpellmakingArea"                           ,  Generic<uint>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SpellmakingCastingTime"                    ,  Generic<float>.Instance);
            Add(typeof(IDialogBranchGetter)          ,  "StartingTopic"                             ,  FormLink<IDialogTopicGetter>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "StaticAttenuation"                         ,  Generic<float>.Instance);
            Add(typeof(ISoundCategoryGetter)         ,  "StaticVolumeMultiplier"                    ,  Generic<float>.Instance);
            Add(typeof(IFactionGetter)               ,  "StolenGoodsContainer"                      ,  FormLink<IPlacedObjectGetter>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "String"                                    ,  Generic<string>.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "Subtype"                                   ,  Enums.Instance);
            Add(typeof(IWeatherGetter)               ,  "SunGlareLensFlare"                         ,  FormLink<ILensFlareGetter>.Instance);
            Add(typeof(IFootstepGetter)              ,  "Tag"                                       ,  Generic<string>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "TaperCurve"                                ,  Generic<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "TaperDuration"                             ,  Generic<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "TaperWeight"                               ,  Generic<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "Target"                                    ,  Enums.Instance);
            Add(typeof(IHazardGetter)                ,  "TargetInterval"                            ,  Generic<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "TargetPercentBetweenActors"                ,  Generic<float>.Instance);
            Add(null                                 ,  "TargetType"                                ,  Enums.Instance);
            Add(typeof(IClassGetter)                 ,  "Teaches"                                   ,  Enums.Instance);
            Add(typeof(IWeaponGetter)                ,  "Template"                                  ,  FormLink<IWeaponGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Template"                                  ,  FormLink<INpcSpawnGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "TemplateArmor"                             ,  FormLink<IArmorGetter>.Instance);
            Add(typeof(IQuestGetter)                 ,  "TextDisplayGlobals"                        ,  FormLinks<IGlobalGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "TextureCountU"                             ,  Generic<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "TextureCountV"                             ,  Generic<uint>.Instance);
            Add(null                                 ,  "TextureSet"                                ,  FormLink<ITextureSetGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "TextureSpecularExponent"                   ,  Generic<byte>.Instance);
            Add(typeof(ILightGetter)                 ,  "Time"                                      ,  Generic<int>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "TimeMultiplierGlobal"                      ,  Generic<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "TimeMultiplierPlayer"                      ,  Generic<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "TimeMultiplierTarget"                      ,  Generic<float>.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "TopicFlags"                                ,  Flags.Instance);
            Add(typeof(IProjectileGetter)            ,  "TracerChance"                              ,  Generic<float>.Instance);
            Add(typeof(IMusicTrackGetter)            ,  "Tracks"                                    ,  FormLinks<IMusicTrackGetter>.Instance);
            Add(typeof(IMusicTypeGetter)             ,  "Tracks"                                    ,  FormLinks<IMusicTrackGetter>.Instance);
            Add(typeof(IWeatherGetter)               ,  "TransDelta"                                ,  Generic<float>.Instance);
            Add(typeof(IWordOfPowerGetter)           ,  "Translation"                               ,  Generic<string>.Instance);
            Add(typeof(ITreeGetter)                  ,  "TrunkFlexibility"                          ,  Generic<float>.Instance);
            Add(null                                 ,  "Type"                                      ,  Enums.Instance);
            Add(typeof(IArtObjectGetter)             ,  "Type"                                      ,  Flags.Instance);
            Add(typeof(IGlobalGetter)                ,  "TypeChar"                                  ,  Generic<char>.Instance);
            Add(typeof(IRaceGetter)                  ,  "UnarmedDamage"                             ,  Generic<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "UnarmedEquipSlot"                          ,  FormLink<IEquipTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "UnarmedReach"                              ,  Generic<float>.Instance);
            Add(typeof(IWeaponGetter)                ,  "UnequipSound"                              ,  FormLink<ISoundDescriptorGetter>.Instance);
            Add(typeof(IGrassGetter)                 ,  "UnitsFromWater"                            ,  Generic<ushort>.Instance);
            Add(typeof(IGrassGetter)                 ,  "UnitsFromWaterType"                        ,  Enums.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "Unknown1"                                  ,  Generic<ushort>.Instance);
            Add(typeof(IAnimatedObjectGetter)        ,  "UnloadEvent"                               ,  Generic<string>.Instance);
            Add(typeof(ILocationGetter)              ,  "UnreportedCrimeFaction"                    ,  FormLink<IFactionGetter>.Instance);
            Add(typeof(IAcousticSpaceGetter)         ,  "UseSoundFromRegion"                        ,  FormLink<IRegionGetter>.Instance);
            Add(typeof(IHeadPartGetter)              ,  "ValidRaces"                                ,  FormLink<IFormListGetter>.Instance);
            Add(null                                 ,  "Value"                                     ,  Generic<uint>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "Variance"                                  ,  Generic<sbyte>.Instance);
            Add(typeof(IFactionGetter)               ,  "VendorBuySellList"                         ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "VerticalOffsetMult"                        ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "VisualEffect"                              ,  FormLink<IVisualEffectGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Voice"                                     ,  FormLink<IVoiceTypeGetter>.Instance);
            Add(typeof(IClassGetter)                 ,  "VoicePoints"                               ,  Generic<uint>.Instance);
            Add(typeof(ITalkingActivatorGetter)      ,  "VoiceType"                                 ,  FormLink<IVoiceTypeGetter>.Instance);
            Add(typeof(IClimateGetter)               ,  "Volatility"                                ,  Generic<byte>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "WalkForwardAlternateFootsteps"             ,  FormLinks<IFootstepGetter>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "WalkForwardAlternateFootsteps2"            ,  FormLinks<IFootstepGetter>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "WalkForwardFootsteps"                      ,  FormLinks<IFootstepGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "Water"                                     ,  FormLink<IWaterGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "Water"                                     ,  FormLink<IWaterGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "WaterEnvironmentMap"                       ,  Generic<string>.Instance);
            Add(typeof(IWaterGetter)                 ,  "WaterFresnel"                              ,  Generic<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "WaterHeight"                               ,  Generic<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "WaterNoiseTexture"                         ,  Generic<string>.Instance);
            Add(typeof(IWaterGetter)                 ,  "WaterReflectionMagnitude"                  ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "WaterReflectivity"                         ,  Generic<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "WaterRefractionMagnitude"                  ,  Generic<float>.Instance);
            Add(typeof(IActivatorGetter)             ,  "WaterType"                                 ,  FormLink<IWaterGetter>.Instance);
            Add(typeof(IGrassGetter)                 ,  "WavePeriod"                                ,  Generic<float>.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "WeaponAdjust"                              ,  Generic<float>.Instance);
            Add(null                                 ,  "Weight"                                    ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "WindDirection"                             ,  Generic<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "WindDirectionRange"                        ,  Generic<float>.Instance);
            Add(typeof(IConstructibleObjectGetter)   ,  "WorkbenchKeyword"                          ,  FormLink<IKeywordGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "WorldLocationMarkerRef"                    ,  FormLink<IPlacedSimpleGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "WorldLocationRadius"                       ,  Generic<float>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "WorldMapOffsetScale"                       ,  Generic<float>.Instance);
            Add(typeof(IRegionGetter)                ,  "Worldspace"                                ,  FormLink<IWorldspaceGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "WornArmor"                                 ,  FormLink<IArmorGetter>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "WornRestrictions"                          ,  FormLink<IFormListGetter>.Instance);
            Add(typeof(ICameraPathGetter)            ,  "Zoom"                                      ,  Flags.Instance);
            #pragma warning restore format
        }

        private static bool TryFindAlias (Type? type, string key, out PropertyAliasMapping pam)
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

    internal class RecordPropertyComparer : IEqualityComparer<IRecordProperty>
    {
        public static bool Equals (IRecordProperty? x, IRecordProperty? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Type == y.Type && x.PropertyName.Equals(y.PropertyName, StringComparison.OrdinalIgnoreCase);
        }

        public static int GetHashCode ([DisallowNull] IRecordProperty obj) => obj.Type?.GetHashCode() ?? 0 ^ obj.PropertyName.GetHashCode(StringComparison.OrdinalIgnoreCase);

        bool IEqualityComparer<IRecordProperty>.Equals (IRecordProperty? x, IRecordProperty? y) => Equals(x, y);

        int IEqualityComparer<IRecordProperty>.GetHashCode ([DisallowNull] IRecordProperty obj) => GetHashCode(obj);
    }
}