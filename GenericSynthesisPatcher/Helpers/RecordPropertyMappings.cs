using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers
{
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
            Add(typeof(IActorValueInformationGetter) ,  "Abbreviation"                              ,  GenericAction<string>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AccelerationRate"                          ,  GenericAction<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "AcousticSpace"                             ,  FormLinkAction<IAcousticSpaceGetter>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "Action"                                    ,  EnumsAction.Instance);
            Add(typeof(IActivatorGetter)             ,  "ActivateTextOverride"                      ,  GenericAction<string>.Instance);
            Add(typeof(IFloraGetter)                 ,  "ActivateTextOverride"                      ,  GenericAction<string>.Instance);
            Add(typeof(IActivatorGetter)             ,  "ActivationSound"                           ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "ActorCellMarkerReference"                  ,  FormLinksAction<IPlacedGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "ActorEffect"                               ,  FormLinksAction<ISpellRecordGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "ActorEffect"                               ,  FormLinksAction<ISpellRecordGetter>.Instance);
            Add(typeof(IIngestibleGetter)            ,  "Addiction"                                 ,  FormLinkAction<ISkyrimMajorRecordGetter>.Instance);
            Add(typeof(IIngestibleGetter)            ,  "AddictionChance"                           ,  GenericAction<float>.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "AdditionalRaces"                           ,  FormLinksAction<IRaceGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModels"                               ,  FormLinkAction<IDebrisGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsFadeInTime"                     ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsFadeOutTime"                    ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsScaleEnd"                       ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsScaleInTime"                    ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsScaleOutTime"                   ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AddonModelsScaleStart"                     ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AimAngleTolerance"                         ,  GenericAction<float>.Instance);
            Add(typeof(IArmorGetter)                 ,  "AlternateBlockMaterial"                    ,  FormLinkAction<IMaterialTypeGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AlternateBlockMaterial"                    ,  FormLinkAction<IMaterialTypeGetter>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "AlternateSoundFor"                         ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "AmbientSound"                              ,  FormLinkAction<ISoundGetter>.Instance);
            Add(typeof(IAcousticSpaceGetter)         ,  "AmbientSound"                              ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "AngleThreshold"                            ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AngularAccelerationRate"                   ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AngularTolerance"                          ,  GenericAction<float>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "AnimationEvent"                            ,  GenericAction<string>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "AnimationGroupSection"                     ,  GenericAction<byte>.Instance);
            Add(typeof(IIdleMarkerGetter)            ,  "Animations"                                ,  FormLinksAction<IIdleAnimationGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "Armature"                                  ,  FormLinksAction<IArmorAddonGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "ArmorRace"                                 ,  FormLinkAction<IRaceGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "ArmorRating"                               ,  GenericAction<float>.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "ArtObject"                                 ,  FormLinkAction<IArtObjectGetter>.Instance);
            Add(typeof(IFurnitureGetter)             ,  "AssociatedSpell"                           ,  FormLinkAction<ISpellGetter>.Instance);
            Add(typeof(IRelationshipGetter)          ,  "AssociationType"                           ,  FormLinkAction<IAssociationTypeGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AttackFailSound"                           ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AttackLoopSound"                           ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "AttackRace"                                ,  FormLinkAction<IRaceGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "AttackRace"                                ,  FormLinkAction<IRaceGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AttackSound"                               ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "AttackSound2D"                             ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "AvoidThreatChance"                         ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "BackRun"                                   ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "BackWalk"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseCarryWeight"                           ,  GenericAction<float>.Instance);
            Add(typeof(IScrollGetter)                ,  "BaseCost"                                  ,  GenericAction<uint>.Instance);
            Add(typeof(ISpellGetter)                 ,  "BaseCost"                                  ,  GenericAction<uint>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "BaseCost"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "BaseEnchantment"                           ,  FormLinkAction<IObjectEffectGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMass"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultFly"                    ,  FormLinkAction<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultRun"                    ,  FormLinkAction<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultSneak"                  ,  FormLinkAction<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultSprint"                 ,  FormLinkAction<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultSwim"                   ,  FormLinkAction<IMovementTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BaseMovementDefaultWalk"                   ,  FormLinkAction<IMovementTypeGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "BashImpactDataSet"                         ,  FormLinkAction<IImpactDataSetGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "BirthPositionOffset"                       ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "BirthPositionOffsetRangePlusMinus"         ,  GenericAction<float>.Instance);
            Add(typeof(IClassGetter)                 ,  "BleedoutDefault"                           ,  GenericAction<float>.Instance);
            Add(typeof(IWeaponGetter)                ,  "BlockBashImpact"                           ,  FormLinkAction<IImpactDataSetGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "BodyBipedObject"                           ,  EnumsAction.Instance);
            Add(typeof(IRaceGetter)                  ,  "BodyPartData"                              ,  FormLinkAction<IBodyPartDataGetter>.Instance);
            Add(typeof(IBookGetter)                  ,  "BookText"                                  ,  GenericAction<string>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "BoxSize"                                   ,  GenericAction<uint>.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "Branch"                                    ,  FormLinkAction<IDialogBranchGetter>.Instance);
            Add(typeof(ITreeGetter)                  ,  "BranchFlexibility"                         ,  GenericAction<float>.Instance);
            Add(typeof(IDialogViewGetter)            ,  "Branches"                                  ,  FormLinksAction<IDialogBranchGetter>.Instance);
            Add(typeof(IMaterialTypeGetter)          ,  "Buoyancy"                                  ,  GenericAction<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "CSGDDataTypeState"                         ,  FlagsAction.Instance);
            Add(typeof(IScrollGetter)                ,  "CastDuration"                              ,  GenericAction<float>.Instance);
            Add(typeof(ISpellGetter)                 ,  "CastDuration"                              ,  GenericAction<float>.Instance);
            Add(null                                 ,  "CastType"                                  ,  EnumsAction.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "CastingArt"                                ,  FormLinkAction<IArtObjectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "CastingLight"                              ,  FormLinkAction<ILightGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "CastingSoundLevel"                         ,  EnumsAction.Instance);
            Add(typeof(IDialogBranchGetter)          ,  "Category"                                  ,  EnumsAction.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "Category"                                  ,  EnumsAction.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "Category"                                  ,  FormLinkAction<ISoundCategoryGetter>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "CenterOffsetMax"                           ,  GenericAction<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "CenterOffsetMin"                           ,  GenericAction<float>.Instance);
            Add(null                                 ,  "ChanceNone"                                ,  StructAction<Percent>.Instance);
            Add(null                                 ,  "ChargeTime"                                ,  GenericAction<float>.Instance);
            Add(typeof(IRelationshipGetter)          ,  "Child"                                     ,  FormLinkAction<INpcGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Class"                                     ,  FormLinkAction<IClassGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "Climate"                                   ,  FormLinkAction<IClimateGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "CloseLootSound"                            ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IContainerGetter)             ,  "CloseSound"                                ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IDoorGetter)                  ,  "CloseSound"                                ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(ICollisionLayerGetter)        ,  "CollidesWith"                              ,  FormLinksAction<ICollisionLayerGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "CollisionLayer"                            ,  FormLinkAction<ICollisionLayerGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "CollisionRadius"                           ,  GenericAction<float>.Instance);
            Add(typeof(IHeadPartGetter)              ,  "Color"                                     ,  FormLinkAction<IColorRecordGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey1Alpha"                            ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey1Time"                             ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey2Alpha"                            ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey2Time"                             ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey3Alpha"                            ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorKey3Time"                             ,  GenericAction<float>.Instance);
            Add(typeof(IGrassGetter)                 ,  "ColorRange"                                ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ColorScale"                                ,  GenericAction<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "CombatOverridePackageList"                 ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "CombatStyle"                               ,  FormLinkAction<ICombatStyleGetter>.Instance);
            Add(typeof(IPackageGetter)               ,  "CombatStyle"                               ,  FormLinkAction<ICombatStyleGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "ConeSpread"                                ,  GenericAction<float>.Instance);
            Add(typeof(IIngestibleGetter)            ,  "ConsumeSound"                              ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(ISoulGemGetter)               ,  "ContainedSoul"                             ,  EnumsAction.Instance);
            Add(typeof(IProjectileGetter)            ,  "CountdownSound"                            ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "CounterEffects"                            ,  FormLinksAction<IMagicEffectGetter>.Instance);
            Add(typeof(IConstructibleObjectGetter)   ,  "CreatedObject"                             ,  FormLinkAction<IConstructibleGetter>.Instance);
            Add(typeof(IConstructibleObjectGetter)   ,  "CreatedObjectCount"                        ,  GenericAction<ushort>.Instance);
            Add(typeof(INpcGetter)                   ,  "CrimeFaction"                              ,  FormLinkAction<IFactionGetter>.Instance);
            Add(typeof(IStaticGetter)                ,  "DNAMDataTypeState"                         ,  FlagsAction.Instance);
            Add(typeof(IWaterGetter)                 ,  "DNAMDataTypeState"                         ,  FlagsAction.Instance);
            Add(typeof(IAmmunitionGetter)            ,  "Damage"                                    ,  GenericAction<float>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Damage"                                    ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DamagePerSecond"                           ,  GenericAction<ushort>.Instance);
            Add(typeof(IPackageGetter)               ,  "DataInputVersion"                          ,  GenericAction<int>.Instance);
            Add(typeof(INpcGetter)                   ,  "DeathItem"                                 ,  FormLinkAction<ILeveledItemGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "DecalData"                                 ,  FormLinkAction<ITextureSetGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "DecapitationFX"                            ,  FormLinkAction<IArtObjectGetter>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "DecayHfRatio"                              ,  GenericAction<float>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "DecayMilliseconds"                         ,  GenericAction<ushort>.Instance);
            Add(typeof(IRaceGetter)                  ,  "DecelerationRate"                          ,  GenericAction<float>.Instance);
            Add(typeof(ISoundCategoryGetter)         ,  "DefaultMenuVolume"                         ,  GenericAction<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "DefaultOutfit"                             ,  FormLinkAction<IOutfitGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "DefaultPackageList"                        ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "DefaultWeaponSource"                       ,  FormLinkAction<IWeaponGetter>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "DefensiveMult"                             ,  GenericAction<float>.Instance);
            Add(typeof(IGrassGetter)                 ,  "Density"                                   ,  GenericAction<byte>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "DensityPercent"                            ,  StructAction<Percent>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DepthNormals"                              ,  GenericAction<float>.Instance);
            Add(typeof(IImageSpaceAdapterGetter)     ,  "DepthOfFieldFlags"                         ,  FlagsAction.Instance);
            Add(typeof(IWaterGetter)                 ,  "DepthReflections"                          ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DepthRefraction"                           ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DepthSpecularLighting"                     ,  GenericAction<float>.Instance);
            Add(null                                 ,  "Description"                               ,  GenericAction<string>.Instance);
            Add(typeof(IWeaponGetter)                ,  "DetectionSoundLevel"                       ,  EnumsAction.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "DetectionSoundValue"                       ,  GenericAction<byte>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "DiffusionPercent"                          ,  StructAction<Percent>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "DirectionalFade"                           ,  GenericAction<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "DirectionalRotationXY"                     ,  GenericAction<int>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "DirectionalRotationZ"                      ,  GenericAction<int>.Instance);
            Add(typeof(IProjectileGetter)            ,  "DisaleSound"                               ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementDampner"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementFalloff"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementFoce"                          ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementStartingSize"                  ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "DisplacementVelocity"                      ,  GenericAction<float>.Instance);
            Add(typeof(IMessageGetter)               ,  "DisplayTime"                               ,  GenericAction<uint>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "DistantLodMultiplier"                      ,  GenericAction<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "DualCastArt"                               ,  FormLinkAction<IDualCastDataGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "DualCastScale"                             ,  GenericAction<float>.Instance);
            Add(null                                 ,  "Duration"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "ENITDataTypeState"                         ,  FlagsAction.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectAlphaFadeInTime"                 ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectAlphaFadeOutTime"                ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectAlphaPulseAmplitude"             ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectAlphaPulseFrequency"             ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectFallOff"                         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectFullAlphaRatio"                  ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectFullAlphaTime"                   ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeEffectPersistentAlphaRatio"            ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "EdgeWidth"                                 ,  GenericAction<float>.Instance);
            Add(typeof(IVisualEffectGetter)          ,  "EffectArt"                                 ,  FormLinkAction<IArtObjectGetter>.Instance);
            Add(typeof(IDualCastDataGetter)          ,  "EffectShader"                              ,  FormLinkAction<IEffectShaderGetter>.Instance);
            Add(null                                 ,  "Effects"                                   ,  EffectsAction.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "EnchantArt"                                ,  FormLinkAction<IArtObjectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "EnchantShader"                             ,  FormLinkAction<IEffectShaderGetter>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "EnchantType"                               ,  EnumsAction.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "EnchantVisuals"                            ,  FormLinkAction<IVisualEffectGetter>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "EnchantmentAmount"                         ,  GenericAction<int>.Instance);
            Add(typeof(IArmorGetter)                 ,  "EnchantmentAmount"                         ,  GenericAction<ushort>.Instance);
            Add(typeof(IWeaponGetter)                ,  "EnchantmentAmount"                         ,  GenericAction<ushort>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "EnchantmentCost"                           ,  GenericAction<uint>.Instance);
            Add(typeof(ICellGetter)                  ,  "EncounterZone"                             ,  FormLinkAction<IEncounterZoneGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "EncounterZone"                             ,  FormLinkAction<IEncounterZoneGetter>.Instance);
            Add(typeof(ILeveledNpcGetter)            ,  "Entries"                                   ,  LeveledNpcAction.Instance);
            Add(typeof(ILeveledSpellGetter)          ,  "Entries"                                   ,  LeveledSpellAction.Instance);
            Add(typeof(ILeveledItemGetter)           ,  "Entries"                                   ,  LeveledItemAction.Instance);
            Add(typeof(IAcousticSpaceGetter)         ,  "EnvironmentType"                           ,  FormLinkAction<IReverbParametersGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "EquipAbility"                              ,  FormLinkAction<ISpellGetter>.Instance);
            Add(typeof(IWeaponGetter)                ,  "EquipSound"                                ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IIngredientGetter)            ,  "EquipType"                                 ,  FormLinkAction<IEquipTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "EquipmentFlags"                            ,  FlagsAction.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultMagic"                   ,  GenericAction<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultMelee"                   ,  GenericAction<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultRanged"                  ,  GenericAction<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultShout"                   ,  GenericAction<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultStaff"                   ,  GenericAction<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "EquipmentScoreMultUnarmed"                 ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "EquipmentSlots"                            ,  FormLinksAction<IEquipTypeGetter>.Instance);
            Add(null                                 ,  "EquipmentType"                             ,  FormLinkAction<IEquipTypeGetter>.Instance);
            Add(null                                 ,  "Explosion"                                 ,  FormLinkAction<IExplosionGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "ExplosionAltTriggerProximity"              ,  GenericAction<float>.Instance);
            Add(typeof(IProjectileGetter)            ,  "ExplosionAltTriggerTimer"                  ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ExplosionWindSpeed"                        ,  GenericAction<float>.Instance);
            Add(typeof(IFactionGetter)               ,  "ExteriorJailMarker"                        ,  FormLinkAction<IPlacedObjectGetter>.Instance);
            Add(typeof(IHeadPartGetter)              ,  "ExtraParts"                                ,  FormLinksAction<IHeadPartGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "Eyes"                                      ,  FormLinksAction<IEyesGetter>.Instance);
            Add(typeof(ITalkingActivatorGetter)      ,  "FNAM"                                      ,  GenericAction<short>.Instance);
            Add(typeof(ILightGetter)                 ,  "FOV"                                       ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "FacegenFaceClamp"                          ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "FacegenMainClamp"                          ,  GenericAction<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "FactionRank"                               ,  GenericAction<int>.Instance);
            Add(typeof(IMusicTypeGetter)             ,  "FadeDuration"                              ,  GenericAction<float>.Instance);
            Add(typeof(IProjectileGetter)            ,  "FadeDuration"                              ,  GenericAction<float>.Instance);
            Add(typeof(IMusicTrackGetter)            ,  "FadeOut"                                   ,  GenericAction<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "FadeValue"                                 ,  GenericAction<float>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "FalloffBias"                               ,  GenericAction<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "FalloffExponent"                           ,  GenericAction<float>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "FalloffScale"                              ,  GenericAction<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "FarAwayModel"                              ,  FormLinkAction<IArmorGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillAlphaFadeInTime"                       ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillAlphaPulseAmplitude"                   ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillAlphaPulseFrequency"                   ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey1Scale"                        ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey1Time"                         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey2Scale"                        ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey2Time"                         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey3Scale"                        ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillColorKey3Time"                         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillFadeOutTime"                           ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillFullAlphaRatio"                        ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillFullAlphaTime"                         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillPersistentAlphaRatio"                  ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillTextureAnimationSpeedU"                ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillTextureAnimationSpeedV"                ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillTextureScaleU"                         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "FillTextureScaleV"                         ,  GenericAction<float>.Instance);
            Add(typeof(IQuestGetter)                 ,  "Filter"                                    ,  GenericAction<string>.Instance);
            Add(typeof(IWeaponGetter)                ,  "FirstPersonModel"                          ,  FormLinkAction<IStaticGetter>.Instance);
            Add(null                                 ,  "Flags"                                     ,  FlagsAction.Instance);
            Add(typeof(ILightGetter)                 ,  "FlickerIntensityAmplitude"                 ,  GenericAction<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "FlickerMovementAmplitude"                  ,  GenericAction<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "FlickerPeriod"                             ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "FlightRadius"                              ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogAboveWaterAmount"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogAboveWaterDistanceFarPlane"             ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogAboveWaterDistanceNearPlane"            ,  GenericAction<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogClipDistance"                           ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceDayFar"                         ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceDayMax"                         ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceDayNear"                        ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceDayPower"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceNightFar"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceNightMax"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceNightNear"                      ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "FogDistanceNightPower"                     ,  GenericAction<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogFar"                                    ,  GenericAction<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogMax"                                    ,  GenericAction<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogNear"                                   ,  GenericAction<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "FogPower"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogUnderWaterAmount"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogUnderWaterDistanceFarPlane"             ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "FogUnderWaterDistanceNearPlane"            ,  GenericAction<float>.Instance);
            Add(typeof(IFactionGetter)               ,  "FollowerWaitMarker"                        ,  FormLinkAction<IPlacedObjectGetter>.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "FootstepSound"                             ,  FormLinkAction<IFootstepSetGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Force"                                     ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "ForwardRun"                                ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "ForwardWalk"                               ,  GenericAction<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "GiftFilter"                                ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(ILeveledItemGetter)           ,  "Global"                                    ,  FormLinkAction<IGlobalGetter>.Instance);
            Add(typeof(ILeveledNpcGetter)            ,  "Global"                                    ,  FormLinkAction<IGlobalGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "Grasses"                                   ,  FormLinksAction<IGrassGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "Gravity"                                   ,  GenericAction<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "GravityVelocity"                           ,  GenericAction<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "GroupOffensiveMult"                        ,  GenericAction<float>.Instance);
            Add(typeof(INpcGetter)                   ,  "GuardWarnOverridePackageList"              ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "HairBipedObject"                           ,  EnumsAction.Instance);
            Add(typeof(INpcGetter)                   ,  "HairColor"                                 ,  FormLinkAction<IColorRecordGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "Hairs"                                     ,  FormLinksAction<IHairGetter>.Instance);
            Add(typeof(IScrollGetter)                ,  "HalfCostPerk"                              ,  FormLinkAction<IPerkGetter>.Instance);
            Add(typeof(ISpellGetter)                 ,  "HalfCostPerk"                              ,  FormLinkAction<IPerkGetter>.Instance);
            Add(typeof(IFloraGetter)                 ,  "HarvestSound"                              ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(ITreeGetter)                  ,  "HarvestSound"                              ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "HavokFriction"                             ,  GenericAction<byte>.Instance);
            Add(typeof(IMaterialTypeGetter)          ,  "HavokImpactDataSet"                        ,  FormLinkAction<IImpactDataSetGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "HavokRestitution"                          ,  GenericAction<byte>.Instance);
            Add(typeof(IImpactGetter)                ,  "Hazard"                                    ,  FormLinkAction<IHazardGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "HeadBipedObject"                           ,  EnumsAction.Instance);
            Add(typeof(INpcGetter)                   ,  "HeadParts"                                 ,  FormLinksAction<IHeadPartGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "HeadTexture"                               ,  FormLinkAction<ITextureSetGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Height"                                    ,  GenericAction<float>.Instance);
            Add(typeof(IGrassGetter)                 ,  "HeightRange"                               ,  GenericAction<float>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "HfReferenceHertz"                          ,  GenericAction<ushort>.Instance);
            Add(typeof(IDualCastDataGetter)          ,  "HitEffectArt"                              ,  FormLinkAction<IArtObjectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "HitEffectArt"                              ,  FormLinkAction<IArtObjectGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "HitShader"                                 ,  FormLinkAction<IEffectShaderGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "HitVisuals"                                ,  FormLinkAction<IVisualEffectGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "HolesEndTime"                              ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "HolesEndValue"                             ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "HolesStartTime"                            ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "HolesStartValue"                           ,  GenericAction<float>.Instance);
            Add(typeof(ILocationGetter)              ,  "HorseMarkerRef"                            ,  FormLinkAction<IPlacedObjectGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "ISRadius"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IClassGetter)                 ,  "Icon"                                      ,  GenericAction<string>.Instance);
            Add(typeof(IWeaponGetter)                ,  "IdleSound"                                 ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IIdleMarkerGetter)            ,  "IdleTimer"                                 ,  GenericAction<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "ImageSpace"                                ,  FormLinkAction<IImageSpaceGetter>.Instance);
            Add(typeof(IWaterGetter)                 ,  "ImageSpace"                                ,  FormLinkAction<IImageSpaceGetter>.Instance);
            Add(null                                 ,  "ImageSpaceModifier"                        ,  FormLinkAction<IImageSpaceAdapterGetter>.Instance);
            Add(typeof(IHazardGetter)                ,  "ImageSpaceRadius"                          ,  GenericAction<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "ImpactData"                                ,  FormLinkAction<IImpactDataSetGetter>.Instance);
            Add(null                                 ,  "ImpactDataSet"                             ,  FormLinkAction<IImpactDataSetGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "ImpactForce"                               ,  GenericAction<float>.Instance);
            Add(typeof(ICollisionLayerGetter)        ,  "Index"                                     ,  GenericAction<uint>.Instance);
            Add(typeof(IFloraGetter)                 ,  "Ingredient"                                ,  FormLinkAction<IHarvestTargetGetter>.Instance);
            Add(typeof(ITreeGetter)                  ,  "Ingredient"                                ,  FormLinkAction<IHarvestTargetGetter>.Instance);
            Add(typeof(IIngredientGetter)            ,  "IngredientValue"                           ,  GenericAction<int>.Instance);
            Add(typeof(IDualCastDataGetter)          ,  "InheritScale"                              ,  FlagsAction.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "InitialRotationRange"                      ,  GenericAction<float>.Instance);
            Add(typeof(ILoadScreenGetter)            ,  "InitialScale"                              ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "InjuredHealthPercent"                      ,  GenericAction<float>.Instance);
            Add(typeof(IActivatorGetter)             ,  "InteractionKeyword"                        ,  FormLinkAction<IKeywordGetter>.Instance);
            Add(typeof(IFurnitureGetter)             ,  "InteractionKeyword"                        ,  FormLinkAction<IKeywordGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "InteriorLighting"                          ,  FormLinkAction<ILightingTemplateGetter>.Instance);
            Add(typeof(IPackageGetter)               ,  "InterruptOverride"                         ,  EnumsAction.Instance);
            Add(typeof(IPackageGetter)               ,  "InteruptFlags"                             ,  FlagsAction.Instance);
            Add(typeof(IBookGetter)                  ,  "InventoryArt"                              ,  FormLinkAction<IStaticGetter>.Instance);
            Add(null                                 ,  "Items"                                     ,  ContainerItemsAction.Instance);
            Add(typeof(IFormListGetter)              ,  "Items"                                     ,  FormLinksAction<ISkyrimMajorRecordGetter>.Instance);
            Add(typeof(IOutfitGetter)                ,  "Items"                                     ,  FormLinksAction<IOutfitTargetGetter>.Instance);
            Add(typeof(IFactionGetter)               ,  "JailOutfit"                                ,  FormLinkAction<IOutfitGetter>.Instance);
            Add(null                                 ,  "Keywords"                                  ,  FormLinksAction<IKeywordGetter>.Instance);
            Add(typeof(ISceneGetter)                 ,  "LastActionIndex"                           ,  GenericAction<uint>.Instance);
            Add(typeof(ITreeGetter)                  ,  "LeafAmplitude"                             ,  GenericAction<float>.Instance);
            Add(typeof(ITreeGetter)                  ,  "LeafFrequency"                             ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "LeftRun"                                   ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "LeftWalk"                                  ,  GenericAction<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "Lens"                                      ,  FormLinkAction<ILensFlareGetter>.Instance);
            Add(typeof(IPerkGetter)                  ,  "Level"                                     ,  GenericAction<byte>.Instance);
            Add(typeof(IHazardGetter)                ,  "Lifetime"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IProjectileGetter)            ,  "Lifetime"                                  ,  GenericAction<float>.Instance);
            Add(null                                 ,  "Light"                                     ,  FormLinkAction<ILightGetter>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "LightFadeEndDistance"                      ,  GenericAction<float>.Instance);
            Add(typeof(ILightingTemplateGetter)      ,  "LightFadeStartDistance"                    ,  GenericAction<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "LightingTemplate"                          ,  FormLinkAction<ILightingTemplateGetter>.Instance);
            Add(typeof(IHazardGetter)                ,  "Limit"                                     ,  GenericAction<uint>.Instance);
            Add(typeof(ISoulGemGetter)               ,  "LinkedTo"                                  ,  FormLinkAction<ISoulGemGetter>.Instance);
            Add(typeof(ILoadScreenGetter)            ,  "LoadingScreenNif"                          ,  FormLinkAction<IStaticGetter>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "Location"                                  ,  EnumsAction.Instance);
            Add(null                                 ,  "Location"                                  ,  FormLinkAction<ILocationGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "LocationCellMarkerReference"               ,  FormLinksAction<IPlacedGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "LockList"                                  ,  FormLinkAction<ILockListGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "LodWater"                                  ,  FormLinkAction<IWaterGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "LodWaterHeight"                            ,  GenericAction<float>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "LongRangeStrafeMult"                       ,  GenericAction<float>.Instance);
            Add(typeof(IDoorGetter)                  ,  "LoopSound"                                 ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "LoopingSecondsMax"                         ,  GenericAction<byte>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "LoopingSecondsMin"                         ,  GenericAction<byte>.Instance);
            Add(typeof(IActivatorGetter)             ,  "LoopingSound"                              ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IMoveableStaticGetter)        ,  "LoopingSound"                              ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(ITalkingActivatorGetter)      ,  "LoopingSound"                              ,  FormLinkAction<ISoundMarkerGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "MagicSkill"                                ,  EnumsAction.Instance);
            Add(null                                 ,  "MajorFlags"                                ,  FlagsAction.Instance);
            Add(typeof(IAddonNodeGetter)             ,  "MasterParticleSystemCap"                   ,  GenericAction<ushort>.Instance);
            Add(typeof(IStaticGetter)                ,  "Material"                                  ,  FormLinkAction<IMaterialObjectGetter>.Instance);
            Add(typeof(IWaterGetter)                 ,  "Material"                                  ,  FormLinkAction<IMaterialTypeGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "MaterialType"                              ,  FormLinkAction<IMaterialTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "MaterialType"                              ,  FormLinkAction<IMaterialTypeGetter>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "MaterialUvScale"                           ,  GenericAction<float>.Instance);
            Add(typeof(IStaticGetter)                ,  "MaxAngle"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IEncounterZoneGetter)         ,  "MaxLevel"                                  ,  GenericAction<sbyte>.Instance);
            Add(typeof(IGrassGetter)                 ,  "MaxSlope"                                  ,  GenericAction<byte>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "MaxTime"                                   ,  GenericAction<float>.Instance);
            Add(typeof(IClassGetter)                 ,  "MaxTrainingLevel"                          ,  GenericAction<byte>.Instance);
            Add(typeof(ISoulGemGetter)               ,  "MaximumCapacity"                           ,  EnumsAction.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "MembraneBlendOperation"                    ,  EnumsAction.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "MembraneDestBlendMode"                     ,  EnumsAction.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "MembraneSourceBlendMode"                   ,  EnumsAction.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "MembraneZTest"                             ,  EnumsAction.Instance);
            Add(null                                 ,  "MenuDisplayObject"                         ,  FormLinkAction<IStaticGetter>.Instance);
            Add(typeof(IFactionGetter)               ,  "MerchantContainer"                         ,  FormLinkAction<IPlacedObjectGetter>.Instance);
            Add(typeof(IEncounterZoneGetter)         ,  "MinLevel"                                  ,  GenericAction<sbyte>.Instance);
            Add(typeof(IGrassGetter)                 ,  "MinSlope"                                  ,  GenericAction<byte>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "MinTime"                                   ,  GenericAction<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "MinimumSkillLevel"                         ,  GenericAction<uint>.Instance);
            Add(typeof(IClimateGetter)               ,  "Moons"                                     ,  FlagsAction.Instance);
            Add(typeof(IRaceGetter)                  ,  "MorphRace"                                 ,  FormLinkAction<IRaceGetter>.Instance);
            Add(null                                 ,  "Music"                                     ,  FormLinkAction<IMusicTypeGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "MuzzleFlash"                               ,  FormLinkAction<ILightGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "MuzzleFlashDuration"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "NAM0DataTypeState"                         ,  FlagsAction.Instance);
            Add(typeof(INpcGetter)                   ,  "NAM5"                                      ,  GenericAction<ushort>.Instance);
            Add(null                                 ,  "Name"                                      ,  GenericAction<string>.Instance);
            Add(typeof(ILightGetter)                 ,  "NearClip"                                  ,  GenericAction<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "NearTargetDistance"                        ,  GenericAction<float>.Instance);
            Add(typeof(IQuestGetter)                 ,  "NextAliasID"                               ,  GenericAction<uint>.Instance);
            Add(typeof(IPerkGetter)                  ,  "NextPerk"                                  ,  FormLinkAction<IPerkGetter>.Instance);
            Add(typeof(IAddonNodeGetter)             ,  "NodeIndex"                                 ,  GenericAction<int>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseFalloff"                              ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseFlowmapScale"                         ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerOneAmplitudeScale"               ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerOneUvScale"                      ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerOneWindDirection"                ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerOneWindSpeed"                    ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerThreeAmplitudeScale"             ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerThreeUvScale"                    ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerThreeWindDirection"              ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerThreeWindSpeed"                  ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerTwoAmplitudeScale"               ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerTwoUvScale"                      ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerTwoWindDirection"                ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "NoiseLayerTwoWindSpeed"                    ,  GenericAction<float>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "NoiseUvScale"                              ,  GenericAction<float>.Instance);
            Add(typeof(IMaterialObjectGetter)        ,  "NormalDampener"                            ,  GenericAction<float>.Instance);
            Add(typeof(IPerkGetter)                  ,  "NumRanks"                                  ,  GenericAction<byte>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "NumSubtexturesX"                           ,  GenericAction<uint>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "NumSubtexturesY"                           ,  GenericAction<uint>.Instance);
            Add(typeof(IRaceGetter)                  ,  "NumberOfTintsInList"                       ,  GenericAction<ushort>.Instance);
            Add(null                                 ,  "ObjectEffect"                              ,  FormLinkAction<IEffectRecordGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "ObserveDeadBodyOverridePackageList"        ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(ICombatStyleGetter)           ,  "OffensiveMult"                             ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "Opacity"                                   ,  GenericAction<byte>.Instance);
            Add(typeof(IRaceGetter)                  ,  "OpenLootSound"                             ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(null                                 ,  "OpenSound"                                 ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "Orientation"                               ,  EnumsAction.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "OutputModel"                               ,  FormLinkAction<ISoundOutputModelGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "Owner"                                     ,  FormLinkAction<IOwnerGetter>.Instance);
            Add(typeof(IEncounterZoneGetter)         ,  "Owner"                                     ,  FormLinkAction<IOwnerGetter>.Instance);
            Add(typeof(IPackageGetter)               ,  "OwnerQuest"                                ,  FormLinkAction<IQuestGetter>.Instance);
            Add(typeof(ITalkingActivatorGetter)      ,  "PNAM"                                      ,  GenericAction<int>.Instance);
            Add(typeof(IPackageGetter)               ,  "PackageTemplate"                           ,  FormLinkAction<IPackageGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Packages"                                  ,  FormLinksAction<IPackageGetter>.Instance);
            Add(typeof(IRelationshipGetter)          ,  "Parent"                                    ,  FormLinkAction<INpcGetter>.Instance);
            Add(typeof(ISoundCategoryGetter)         ,  "Parent"                                    ,  FormLinkAction<ISoundCategoryGetter>.Instance);
            Add(typeof(IMaterialTypeGetter)          ,  "Parent"                                    ,  FormLinkAction<IMaterialTypeGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "ParentLocation"                            ,  FormLinkAction<ILocationGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAcceleration1"                     ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAcceleration2"                     ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAcceleration3"                     ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAccelerationAlongNormal"           ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedEndFrame"                  ,  GenericAction<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedFrameCount"                ,  GenericAction<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedFrameCountVariation"       ,  GenericAction<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedLoopStartFrame"            ,  GenericAction<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedLoopStartVariation"        ,  GenericAction<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedStartFrame"                ,  GenericAction<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleAnimatedStartFrameVariation"       ,  GenericAction<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleBirthRampDownTime"                 ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleBirthRampUpTime"                   ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleBlendOperation"                    ,  EnumsAction.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "ParticleDensity"                           ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleDestBlendMode"                     ,  EnumsAction.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleFullBirthRatio"                    ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleFullBirthTime"                     ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialRotationDegree"             ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialRotationDegreePlusMinus"    ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialSpeedAlongNormal"           ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialSpeedAlongNormalPlusMinus"  ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialVelocity1"                  ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialVelocity2"                  ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleInitialVelocity3"                  ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleLifetime"                          ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleLifetimePlusMinus"                 ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticlePeristentCount"                    ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleRotationSpeedDegreePerSec"         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleRotationSpeedDegreePerSecPlusMinus",  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleScaleKey1"                         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleScaleKey1Time"                     ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleScaleKey2"                         ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleScaleKey2Time"                     ,  GenericAction<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "ParticleSizeX"                             ,  GenericAction<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "ParticleSizeY"                             ,  GenericAction<float>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleSourceBlendMode"                   ,  EnumsAction.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "ParticleZTest"                             ,  EnumsAction.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "PercentFrequencyShift"                     ,  StructAction<Percent>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "PercentFrequencyVariance"                  ,  StructAction<Percent>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "PerkToApply"                               ,  FormLinkAction<IPerkGetter>.Instance);
            Add(typeof(IClimateGetter)               ,  "PhaseLength"                               ,  GenericAction<byte>.Instance);
            Add(null                                 ,  "PickUpSound"                               ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "PlacedObject"                              ,  FormLinkAction<IExplodeSpawnGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "PlacementRadius"                           ,  GenericAction<float>.Instance);
            Add(typeof(IFactionGetter)               ,  "PlayerInventoryContainer"                  ,  FormLinkAction<IPlacedObjectGetter>.Instance);
            Add(typeof(IGrassGetter)                 ,  "PositionRange"                             ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "Precipitation"                             ,  FormLinkAction<IShaderParticleGeometryGetter>.Instance);
            Add(typeof(IWeatherGetter)               ,  "PrecipitationBeginFadeIn"                  ,  StructAction<Percent>.Instance);
            Add(typeof(IWeatherGetter)               ,  "PrecipitationEndFadeOut"                   ,  StructAction<Percent>.Instance);
            Add(typeof(IPackageGetter)               ,  "PreferredSpeed"                            ,  EnumsAction.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "Priority"                                  ,  GenericAction<sbyte>.Instance);
            Add(typeof(IQuestGetter)                 ,  "Priority"                                  ,  GenericAction<byte>.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "Priority"                                  ,  GenericAction<float>.Instance);
            Add(null                                 ,  "Projectile"                                ,  FormLinkAction<IProjectileGetter>.Instance);
            Add(null                                 ,  "PutDownSound"                              ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IAlchemicalApparatusGetter)   ,  "Quality"                                   ,  EnumsAction.Instance);
            Add(null                                 ,  "Quest"                                     ,  FormLinkAction<IQuestGetter>.Instance);
            Add(typeof(IQuestGetter)                 ,  "QuestFormVersion"                          ,  GenericAction<byte>.Instance);
            Add(null                                 ,  "Race"                                      ,  FormLinkAction<IRaceGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Radius"                                    ,  GenericAction<float>.Instance);
            Add(typeof(IHazardGetter)                ,  "Radius"                                    ,  GenericAction<float>.Instance);
            Add(typeof(ILightGetter)                 ,  "Radius"                                    ,  GenericAction<uint>.Instance);
            Add(typeof(IArmorGetter)                 ,  "RagdollConstraintTemplate"                 ,  GenericAction<string>.Instance);
            Add(null                                 ,  "Range"                                     ,  GenericAction<float>.Instance);
            Add(typeof(IRelationshipGetter)          ,  "Rank"                                      ,  EnumsAction.Instance);
            Add(typeof(IEncounterZoneGetter)         ,  "Rank"                                      ,  GenericAction<sbyte>.Instance);
            Add(typeof(ILocationGetter)              ,  "ReferenceCellPersistentReferences"         ,  FormLinksAction<IPlacedSimpleGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "ReferenceCellStaticReferences"             ,  FormLinksAction<IPlacedSimpleGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "ReferenceCellUnique"                       ,  FormLinksAction<INpcGetter>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "ReflectDelayMS"                            ,  GenericAction<byte>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "Reflections"                               ,  GenericAction<sbyte>.Instance);
            Add(typeof(ICellGetter)                  ,  "Regions"                                   ,  FormLinksAction<IRegionGetter>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "RelatedIdles"                              ,  FormLinksAction<IIdleRelationGetter>.Instance);
            Add(typeof(ICameraPathGetter)            ,  "RelatedPaths"                              ,  FormLinksAction<ICameraPathGetter>.Instance);
            Add(typeof(IProjectileGetter)            ,  "RelaunchInterval"                          ,  GenericAction<float>.Instance);
            Add(typeof(IIdleAnimationGetter)         ,  "ReplayDelay"                               ,  GenericAction<ushort>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "ResistValue"                               ,  EnumsAction.Instance);
            Add(typeof(IImpactGetter)                ,  "Result"                                    ,  EnumsAction.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "ReverbAmp"                                 ,  GenericAction<sbyte>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "ReverbDelayMS"                             ,  GenericAction<byte>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RightRun"                                  ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RightWalk"                                 ,  GenericAction<float>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "RoomFilter"                                ,  GenericAction<sbyte>.Instance);
            Add(typeof(IReverbParametersGetter)      ,  "RoomHfFilter"                              ,  GenericAction<sbyte>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RotateInPlaceRun"                          ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RotateInPlaceWalk"                         ,  GenericAction<float>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "RotateWhileMovingRun"                      ,  GenericAction<float>.Instance);
            Add(typeof(IShaderParticleGeometryGetter),  "RotationVelocity"                          ,  GenericAction<float>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "RunForwardAlternateFootsteps"              ,  FormLinksAction<IFootstepGetter>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "RunForwardFootsteps"                       ,  FormLinksAction<IFootstepGetter>.Instance);
            Add(typeof(IMovementTypeGetter)          ,  "SPEDDataTypeState"                         ,  FlagsAction.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "SceneGraphEmitDepthLimit"                  ,  GenericAction<uint>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleDate"                              ,  GenericAction<byte>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleDayOfWeek"                         ,  EnumsAction.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleDurationInMinutes"                 ,  GenericAction<int>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleHour"                              ,  GenericAction<sbyte>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleMinute"                            ,  GenericAction<sbyte>.Instance);
            Add(typeof(IPackageGetter)               ,  "ScheduleMonth"                             ,  GenericAction<sbyte>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "ScriptEffectAIDelayTime"                   ,  GenericAction<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "ScriptEffectAIScore"                       ,  GenericAction<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SecondActorValue"                          ,  EnumsAction.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SecondActorValueWeight"                    ,  GenericAction<float>.Instance);
            Add(typeof(IImpactGetter)                ,  "SecondaryTextureSet"                       ,  FormLinkAction<ITextureSetGetter>.Instance);
            Add(typeof(IVisualEffectGetter)          ,  "Shader"                                    ,  FormLinkAction<IEffectShaderGetter>.Instance);
            Add(typeof(IFactionGetter)               ,  "SharedCrimeFactionList"                    ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "ShieldBipedObject"                         ,  EnumsAction.Instance);
            Add(typeof(IAmmunitionGetter)            ,  "ShortName"                                 ,  GenericAction<string>.Instance);
            Add(typeof(INpcGetter)                   ,  "ShortName"                                 ,  GenericAction<string>.Instance);
            Add(typeof(ICameraPathGetter)            ,  "Shots"                                     ,  FormLinksAction<ICameraShotGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "Size"                                      ,  EnumsAction.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SkillUsageMultiplier"                      ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "Skin"                                      ,  FormLinkAction<IArmorGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "SkyAndWeatherFromRegion"                   ,  FormLinkAction<IRegionGetter>.Instance);
            Add(typeof(IWeatherGetter)               ,  "SkyStatics"                                ,  FormLinksAction<IStaticGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "SleepingOutfit"                            ,  FormLinkAction<IOutfitGetter>.Instance);
            Add(typeof(IEquipTypeGetter)             ,  "SlotParents"                               ,  FormLinksAction<IEquipTypeGetter>.Instance);
            Add(null                                 ,  "Sound"                                     ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "Sound1"                                    ,  FormLinkAction<ISoundGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Sound1"                                    ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "Sound2"                                    ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IImpactGetter)                ,  "Sound2"                                    ,  FormLinkAction<ISoundGetter>.Instance);
            Add(typeof(ISoundMarkerGetter)           ,  "SoundDescriptor"                           ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(null                                 ,  "SoundLevel"                                ,  EnumsAction.Instance);
            Add(typeof(IProjectileGetter)            ,  "SoundLevel"                                ,  GenericAction<uint>.Instance);
            Add(typeof(IExplosionGetter)             ,  "SpawnProjectile"                           ,  FormLinkAction<IProjectileGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "SpectatorOverridePackageList"              ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularBrightness"                        ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularPower"                             ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularRadius"                            ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularSunPower"                          ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularSunSparkleMagnitude"               ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularSunSparklePower"                   ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "SpecularSunSpecularMagnitude"              ,  GenericAction<float>.Instance);
            Add(typeof(IProjectileGetter)            ,  "Speed"                                     ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "Spell"                                     ,  FormLinkAction<ISpellGetter>.Instance);
            Add(typeof(IHazardGetter)                ,  "Spell"                                     ,  FormLinkAction<IEffectRecordGetter>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SpellmakingArea"                           ,  GenericAction<uint>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "SpellmakingCastingTime"                    ,  GenericAction<float>.Instance);
            Add(typeof(IDialogBranchGetter)          ,  "StartingTopic"                             ,  FormLinkAction<IDialogTopicGetter>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "StaticAttenuation"                         ,  GenericAction<float>.Instance);
            Add(typeof(ISoundCategoryGetter)         ,  "StaticVolumeMultiplier"                    ,  GenericAction<float>.Instance);
            Add(typeof(IFactionGetter)               ,  "StolenGoodsContainer"                      ,  FormLinkAction<IPlacedObjectGetter>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "String"                                    ,  GenericAction<string>.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "Subtype"                                   ,  EnumsAction.Instance);
            Add(typeof(IWeatherGetter)               ,  "SunDamage"                                 ,  StructAction<Percent>.Instance);
            Add(typeof(IWeatherGetter)               ,  "SunGlare"                                  ,  StructAction<Percent>.Instance);
            Add(typeof(IWeatherGetter)               ,  "SunGlareLensFlare"                         ,  FormLinkAction<ILensFlareGetter>.Instance);
            Add(typeof(IFootstepGetter)              ,  "Tag"                                       ,  GenericAction<string>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "TaperCurve"                                ,  GenericAction<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "TaperDuration"                             ,  GenericAction<float>.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "TaperWeight"                               ,  GenericAction<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "Target"                                    ,  EnumsAction.Instance);
            Add(typeof(IHazardGetter)                ,  "TargetInterval"                            ,  GenericAction<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "TargetPercentBetweenActors"                ,  GenericAction<float>.Instance);
            Add(null                                 ,  "TargetType"                                ,  EnumsAction.Instance);
            Add(typeof(IClassGetter)                 ,  "Teaches"                                   ,  EnumsAction.Instance);
            Add(typeof(IWeaponGetter)                ,  "Template"                                  ,  FormLinkAction<IWeaponGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "Template"                                  ,  FormLinkAction<INpcSpawnGetter>.Instance);
            Add(typeof(IArmorGetter)                 ,  "TemplateArmor"                             ,  FormLinkAction<IArmorGetter>.Instance);
            Add(typeof(IQuestGetter)                 ,  "TextDisplayGlobals"                        ,  FormLinksAction<IGlobalGetter>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "TextureCountU"                             ,  GenericAction<uint>.Instance);
            Add(typeof(IEffectShaderGetter)          ,  "TextureCountV"                             ,  GenericAction<uint>.Instance);
            Add(null                                 ,  "TextureSet"                                ,  FormLinkAction<ITextureSetGetter>.Instance);
            Add(typeof(ILandscapeTextureGetter)      ,  "TextureSpecularExponent"                   ,  GenericAction<byte>.Instance);
            Add(typeof(IWeatherGetter)               ,  "ThunderLightningBeginFadeIn"               ,  StructAction<Percent>.Instance);
            Add(typeof(IWeatherGetter)               ,  "ThunderLightningEndFadeOut"                ,  StructAction<Percent>.Instance);
            Add(typeof(IWeatherGetter)               ,  "ThunderLightningFrequency"                 ,  StructAction<Percent>.Instance);
            Add(typeof(ILightGetter)                 ,  "Time"                                      ,  GenericAction<int>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "TimeMultiplierGlobal"                      ,  GenericAction<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "TimeMultiplierPlayer"                      ,  GenericAction<float>.Instance);
            Add(typeof(ICameraShotGetter)            ,  "TimeMultiplierTarget"                      ,  GenericAction<float>.Instance);
            Add(typeof(IDialogTopicGetter)           ,  "TopicFlags"                                ,  FlagsAction.Instance);
            Add(typeof(IProjectileGetter)            ,  "TracerChance"                              ,  GenericAction<float>.Instance);
            Add(typeof(IMusicTrackGetter)            ,  "Tracks"                                    ,  FormLinksAction<IMusicTrackGetter>.Instance);
            Add(typeof(IMusicTypeGetter)             ,  "Tracks"                                    ,  FormLinksAction<IMusicTrackGetter>.Instance);
            Add(typeof(IWeatherGetter)               ,  "TransDelta"                                ,  GenericAction<float>.Instance);
            Add(typeof(IWordOfPowerGetter)           ,  "Translation"                               ,  GenericAction<string>.Instance);
            Add(typeof(ITreeGetter)                  ,  "TrunkFlexibility"                          ,  GenericAction<float>.Instance);
            Add(null                                 ,  "Type"                                      ,  EnumsAction.Instance);
            Add(typeof(IArtObjectGetter)             ,  "Type"                                      ,  FlagsAction.Instance);
            Add(typeof(IGlobalGetter)                ,  "TypeChar"                                  ,  GenericAction<char>.Instance);
            Add(typeof(IRaceGetter)                  ,  "UnarmedDamage"                             ,  GenericAction<float>.Instance);
            Add(typeof(IRaceGetter)                  ,  "UnarmedEquipSlot"                          ,  FormLinkAction<IEquipTypeGetter>.Instance);
            Add(typeof(IRaceGetter)                  ,  "UnarmedReach"                              ,  GenericAction<float>.Instance);
            Add(typeof(IWeaponGetter)                ,  "UnequipSound"                              ,  FormLinkAction<ISoundDescriptorGetter>.Instance);
            Add(typeof(IGrassGetter)                 ,  "UnitsFromWater"                            ,  GenericAction<ushort>.Instance);
            Add(typeof(IGrassGetter)                 ,  "UnitsFromWaterType"                        ,  EnumsAction.Instance);
            Add(typeof(IMagicEffectGetter)           ,  "Unknown1"                                  ,  GenericAction<ushort>.Instance);
            Add(typeof(IAnimatedObjectGetter)        ,  "UnloadEvent"                               ,  GenericAction<string>.Instance);
            Add(typeof(ILocationGetter)              ,  "UnreportedCrimeFaction"                    ,  FormLinkAction<IFactionGetter>.Instance);
            Add(typeof(IAcousticSpaceGetter)         ,  "UseSoundFromRegion"                        ,  FormLinkAction<IRegionGetter>.Instance);
            Add(typeof(IHeadPartGetter)              ,  "ValidRaces"                                ,  FormLinkAction<IFormListGetter>.Instance);
            Add(null                                 ,  "Value"                                     ,  GenericAction<uint>.Instance);
            Add(typeof(ISoundDescriptorGetter)       ,  "Variance"                                  ,  GenericAction<sbyte>.Instance);
            Add(typeof(IFactionGetter)               ,  "VendorBuySellList"                         ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(IExplosionGetter)             ,  "VerticalOffsetMult"                        ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "VisualEffect"                              ,  FormLinkAction<IVisualEffectGetter>.Instance);
            Add(typeof(IWeatherGetter)               ,  "VisualEffectBegin"                         ,  StructAction<Percent>.Instance);
            Add(typeof(IWeatherGetter)               ,  "VisualEffectEnd"                           ,  StructAction<Percent>.Instance);
            Add(typeof(INpcGetter)                   ,  "Voice"                                     ,  FormLinkAction<IVoiceTypeGetter>.Instance);
            Add(typeof(IClassGetter)                 ,  "VoicePoints"                               ,  GenericAction<uint>.Instance);
            Add(typeof(ITalkingActivatorGetter)      ,  "VoiceType"                                 ,  FormLinkAction<IVoiceTypeGetter>.Instance);
            Add(typeof(IClimateGetter)               ,  "Volatility"                                ,  GenericAction<byte>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "WalkForwardAlternateFootsteps"             ,  FormLinksAction<IFootstepGetter>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "WalkForwardAlternateFootsteps2"            ,  FormLinksAction<IFootstepGetter>.Instance);
            Add(typeof(IFootstepSetGetter)           ,  "WalkForwardFootsteps"                      ,  FormLinksAction<IFootstepGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "Water"                                     ,  FormLinkAction<IWaterGetter>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "Water"                                     ,  FormLinkAction<IWaterGetter>.Instance);
            Add(typeof(ICellGetter)                  ,  "WaterEnvironmentMap"                       ,  GenericAction<string>.Instance);
            Add(typeof(IWaterGetter)                 ,  "WaterFresnel"                              ,  GenericAction<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "WaterHeight"                               ,  GenericAction<float>.Instance);
            Add(typeof(ICellGetter)                  ,  "WaterNoiseTexture"                         ,  GenericAction<string>.Instance);
            Add(typeof(IWaterGetter)                 ,  "WaterReflectionMagnitude"                  ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "WaterReflectivity"                         ,  GenericAction<float>.Instance);
            Add(typeof(IWaterGetter)                 ,  "WaterRefractionMagnitude"                  ,  GenericAction<float>.Instance);
            Add(typeof(IActivatorGetter)             ,  "WaterType"                                 ,  FormLinkAction<IWaterGetter>.Instance);
            Add(typeof(IGrassGetter)                 ,  "WavePeriod"                                ,  GenericAction<float>.Instance);
            Add(typeof(IArmorAddonGetter)            ,  "WeaponAdjust"                              ,  GenericAction<float>.Instance);
            Add(null                                 ,  "Weight"                                    ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "WindDirection"                             ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "WindDirectionRange"                        ,  GenericAction<float>.Instance);
            Add(typeof(IWeatherGetter)               ,  "WindSpeed"                                 ,  StructAction<Percent>.Instance);
            Add(typeof(IConstructibleObjectGetter)   ,  "WorkbenchKeyword"                          ,  FormLinkAction<IKeywordGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "WorldLocationMarkerRef"                    ,  FormLinkAction<IPlacedSimpleGetter>.Instance);
            Add(typeof(ILocationGetter)              ,  "WorldLocationRadius"                       ,  GenericAction<float>.Instance);
            Add(typeof(IWorldspaceGetter)            ,  "WorldMapOffsetScale"                       ,  GenericAction<float>.Instance);
            Add(typeof(IRegionGetter)                ,  "Worldspace"                                ,  FormLinkAction<IWorldspaceGetter>.Instance);
            Add(typeof(INpcGetter)                   ,  "WornArmor"                                 ,  FormLinkAction<IArmorGetter>.Instance);
            Add(typeof(IObjectEffectGetter)          ,  "WornRestrictions"                          ,  FormLinkAction<IFormListGetter>.Instance);
            Add(typeof(ICameraPathGetter)            ,  "Zoom"                                      ,  FlagsAction.Instance);
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

    #region Support Classes

    internal readonly struct RecordProperty (Type? type, string propertyName) : IRecordProperty
    {
        public string PropertyName { get; } = propertyName;

        public Type? Type { get; } = type;

        public RecordProperty (string propertyName) : this(null, propertyName)
        {
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

        public static int GetHashCode ([DisallowNull] IRecordProperty obj) => (obj.Type?.GetHashCode() ?? 0) ^ obj.PropertyName.GetHashCode(StringComparison.OrdinalIgnoreCase);

        bool IEqualityComparer<IRecordProperty>.Equals (IRecordProperty? x, IRecordProperty? y) => Equals(x, y);

        int IEqualityComparer<IRecordProperty>.GetHashCode ([DisallowNull] IRecordProperty obj) => GetHashCode(obj);
    }

    #endregion Support Classes
}