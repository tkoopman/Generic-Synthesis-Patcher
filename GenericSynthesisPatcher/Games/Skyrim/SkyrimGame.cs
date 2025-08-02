using System.Reflection;

using GenericSynthesisPatcher.Games.Skyrim.Action;
using GenericSynthesisPatcher.Games.Skyrim.Json.Converters;
using GenericSynthesisPatcher.Games.Universal.Action;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
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
        private readonly int _hasDLC;

        /// <summary>
        ///     Create new Skyrim Game instance.
        /// </summary>
        /// <param name="gameState">
        ///     Patcher state. Can use null! in xUnit testing as long as you don't call anything
        ///     that requires State or LoadOrder.
        /// </param>
        public SkyrimGame (IPatcherState<ISkyrimMod, ISkyrimModGetter> gameState) : base(constructLoadOrder(gameState?.LoadOrder.Select(m => (IModListingGetter)m.Value))!)
        {
            State = gameState!;

            _hasDLC = gameState is null || (GameRelease != GameRelease.SkyrimLE && GameRelease != GameRelease.EnderalLE) ? 4
                : !gameState.LoadOrder.PriorityOrder.Any(m => m.ModKey.Name == "Update.esm") ? 0
                : !gameState.LoadOrder.PriorityOrder.Any(m => m.ModKey.Name == "Dawnguard.esm") ? 1
                : !gameState.LoadOrder.PriorityOrder.Any(m => m.ModKey.Name == "HearthFires.esm") ? 2
                : !gameState.LoadOrder.PriorityOrder.Any(m => m.ModKey.Name == "Dragonborn.esm") ? 3
                : 4;

            SerializerSettings.Converters.Add(new ObjectBoundsConverter());
            IgnoreSubPropertiesOnTypes.Add(
                [
                    typeof(Cell),
                    typeof(Destructible)
                ]);

            addExactMatch(typeof(WorldspaceMaxHeight), WorldspaceMaxHeightAction.Instance);
            addExactMatch(typeof(CellMaxHeightData), CellMaxHeightDataAction.Instance);
            addExactMatch(typeof(Model), ModelAction.Instance);

            addAliases(this);
        }

        public override GameCategory GameCategory => GameCategory.Skyrim;

        public override GameRelease GameRelease => State?.GameRelease ?? GameRelease.SkyrimSE;

        public override IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; }

        public override Type TypeOptionSolidifierMixIns => typeof(TypeOptionSolidifierMixIns);

        public override FormKey FormIDToFormKeyConverter (FormID formID)
        {
            uint modID = formID.MasterIndex(MasterStyle.Full);

            if (_hasDLC < modID)
                return FormKey.Null;

            string? modKey = modID switch
            {
                0x00u => "Skyrim",
                0x01u => "Update",
                0x02u => "Dawnguard",
                0x03u => "HearthFires",
                0x04u => "Dragonborn",
                _ => null,
            };

            return modKey is null ? FormKey.Null : new FormKey(new ModKey(modKey, ModType.Master), formID.Id(MasterStyle.Full));
        }

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

        private static void addAliases (SkyrimGame game)
        {
#pragma warning disable format

            // Global Aliases
            game.AddAlias(null                                            , "DESC"       , nameof(IAmmunitionGetter.Description));
            game.AddAlias(null                                            , "EAMT"       , nameof(IArmorGetter.EnchantmentAmount));
            game.AddAlias(null                                            , "EDID"       , nameof(IArmorGetter.EditorID));
            game.AddAlias(null                                            , "EITM"       , nameof(IArmorGetter.ObjectEffect));
            game.AddAlias(null                                            , "ETYP"       , nameof(IWeaponGetter.EquipmentType));
            game.AddAlias(null                                            , "FULL"       , nameof(INamedGetter.Name));
            game.AddAlias(null                                            , "Item"       , nameof(IContainerGetter.Items));
            game.AddAlias(null                                            , "KWDA"       , nameof(IKeywordedGetter.Keywords));
            game.AddAlias(null                                            , "OBND"       , nameof(IArmorGetter.ObjectBounds));
            game.AddAlias(null                                            , "ONAM"       , nameof(INpcGetter.ShortName));
            game.AddAlias(null                                            , "RecordFlags", nameof(IAmmunitionGetter.MajorFlags));
            game.AddAlias(null                                            , "RNAM"       , nameof(INpcGetter.Race));
            game.AddAlias(null                                            , "VMAD"       , nameof(IArmorGetter.VirtualMachineAdapter));
            game.AddAlias(null                                            , "XCWT"       , nameof(ICellGetter.Water));
            game.AddAlias(null                                            , "XEZN"       , nameof(ICellGetter.EncounterZone));
            game.AddAlias(null                                            , "XLCN"       , nameof(ICellGetter.Location));
            game.AddAlias(null                                            , "YNAM"       , nameof(IAmmunitionGetter.PickUpSound));
            game.AddAlias(null                                            , "ZNAM"       , nameof(IAmmunitionGetter.PutDownSound));

            // Record specific aliases
            game.AddAlias(IAcousticSpaceGetter.StaticRegistration         , "BNAM"       , nameof(IAcousticSpaceGetter.EnvironmentType));
            game.AddAlias(IAcousticSpaceGetter.StaticRegistration         , "RDAT"       , nameof(IAcousticSpaceGetter.UseSoundFromRegion));
            game.AddAlias(IAcousticSpaceGetter.StaticRegistration         , "SNAM"       , nameof(IAcousticSpaceGetter.AmbientSound));

            game.AddAlias(IActionRecordGetter.StaticRegistration          , "CNAM"       , nameof(IActionRecordGetter.Color));

            game.AddAlias(IActivatorGetter.StaticRegistration             , "FNAM"       , nameof(IActivatorGetter.Flags));
            game.AddAlias(IActivatorGetter.StaticRegistration             , "KNAM"       , nameof(IActivatorGetter.InteractionKeyword));
            game.AddAlias(IActivatorGetter.StaticRegistration             , "PNAM"       , nameof(IActivatorGetter.MarkerColor));
            game.AddAlias(IActivatorGetter.StaticRegistration             , "RNAM"       , nameof(IActivatorGetter.ActivateTextOverride));
            game.AddAlias(IActivatorGetter.StaticRegistration             , "SNAM"       , nameof(IActivatorGetter.LoopingSound));
            game.AddAlias(IActivatorGetter.StaticRegistration             , "VNAM"       , nameof(IActivatorGetter.ActivationSound));
            game.AddAlias(IActivatorGetter.StaticRegistration             , "WNAM"       , nameof(IActivatorGetter.WaterType));

            game.AddAlias(IActorValueInformationGetter.StaticRegistration , "ANAM"       , nameof(IActorValueInformationGetter.Abbreviation));
            game.AddAlias(IActorValueInformationGetter.StaticRegistration , "AVSK"       , nameof(IActorValueInformationGetter.Skill));

            game.AddAlias(IAddonNodeGetter.StaticRegistration             , "DATA"       , nameof(IAddonNodeGetter.NodeIndex));
            game.AddAlias(IAddonNodeGetter.StaticRegistration             , "SNAM"       , nameof(IAddonNodeGetter.Sound));

            game.AddAlias(IAlchemicalApparatusGetter.StaticRegistration   , "QUAL"       , nameof(IAlchemicalApparatusGetter.Quality));

            game.AddAlias(IAmmunitionGetter.StaticRegistration            , "DMG"        , nameof(IAmmunitionGetter.Damage));

            game.AddAlias(IAnimatedObjectGetter.StaticRegistration        , "BNAM"       , nameof(IAnimatedObjectGetter.UnloadEvent));

            game.AddAlias(IArmorAddonGetter.StaticRegistration            , "BODT"       , nameof(IArmorAddonGetter.BodyTemplate));
            game.AddAlias(IArmorAddonGetter.StaticRegistration            , "MODL"       , nameof(IArmorAddonGetter.AdditionalRaces));
            game.AddAlias(IArmorAddonGetter.StaticRegistration            , "NAM0"       , "SkinTexture.Male");
            game.AddAlias(IArmorAddonGetter.StaticRegistration            , "NAM1"       , "SkinTexture.Female");
            game.AddAlias(IArmorAddonGetter.StaticRegistration            , "NAM2"       , "TextureSwapList.Male");
            game.AddAlias(IArmorAddonGetter.StaticRegistration            , "NAM3"       , "TextureSwapList.Female");
            game.AddAlias(IArmorAddonGetter.StaticRegistration            , "ONAM"       , nameof(IArmorAddonGetter.ArtObject));
            game.AddAlias(IArmorAddonGetter.StaticRegistration            , "SNDD"       , nameof(IArmorAddonGetter.FootstepSound));

            game.AddAlias(IArmorGetter.StaticRegistration                 , "BAMT"       , nameof(IArmorGetter.AlternateBlockMaterial));
            game.AddAlias(IArmorGetter.StaticRegistration                 , "BIDS"       , nameof(IArmorGetter.BashImpactDataSet));
            game.AddAlias(IArmorGetter.StaticRegistration                 , "BMCT"       , nameof(IArmorGetter.RagdollConstraintTemplate));
            game.AddAlias(IArmorGetter.StaticRegistration                 , "BOD2"       , nameof(IArmorGetter.BodyTemplate));
            game.AddAlias(IArmorGetter.StaticRegistration                 , "DNAM"       , nameof(IArmorGetter.ArmorRating));
            game.AddAlias(IArmorGetter.StaticRegistration                 , "MODL"       , nameof(IArmorGetter.Armature));
            game.AddAlias(IArmorGetter.StaticRegistration                 , "TNAM"       , nameof(IArmorGetter.TemplateArmor));

            game.AddAlias(IArtObjectGetter.StaticRegistration             , "DNAM"       , nameof(IArtObjectGetter.Type));

            game.AddAlias(IAssociationTypeGetter.StaticRegistration       , "DATA"       , nameof(IAssociationTypeGetter.IsFamily));
            game.AddAlias(IAssociationTypeGetter.StaticRegistration       , "FCHT"       , "Title.Female");
            game.AddAlias(IAssociationTypeGetter.StaticRegistration       , "FPRT"       , "ParentTitle.Female");
            game.AddAlias(IAssociationTypeGetter.StaticRegistration       , "MCHT"       , "Title.Male");
            game.AddAlias(IAssociationTypeGetter.StaticRegistration       , "MPRT"       , "ParentTitle.Male");

            game.AddAlias(IBookGetter.StaticRegistration                  , "CNAM"       , nameof(IBookGetter.Description));
            game.AddAlias(IBookGetter.StaticRegistration                  , "DESC"       , nameof(IBookGetter.BookText));
            game.AddAlias(IBookGetter.StaticRegistration                  , "INAM"       , nameof(IBookGetter.InventoryArt));

            game.AddAlias(ICameraPathGetter.StaticRegistration            , "ANAM"       , nameof(ICameraPathGetter.RelatedPaths));
            game.AddAlias(ICameraPathGetter.StaticRegistration            , "DATA"       , nameof(ICameraPathGetter.Zoom));
            game.AddAlias(ICameraPathGetter.StaticRegistration            , "SNAM"       , nameof(ICameraPathGetter.Shots));
            game.AddAlias(ICameraShotGetter.StaticRegistration            , "MNAM"       , nameof(ICameraShotGetter.ImageSpaceModifier));

            game.AddAlias(ICellGetter.StaticRegistration                  , "DATA"       , nameof(ICellGetter.Flags));
            game.AddAlias(ICellGetter.StaticRegistration                  , "LTMP"       , nameof(ICellGetter.LightingTemplate));
            game.AddAlias(ICellGetter.StaticRegistration                  , "MHDT"       , nameof(ICellGetter.MaxHeightData));
            game.AddAlias(ICellGetter.StaticRegistration                  , "TVDT"       , nameof(ICellGetter.OcclusionData));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XCAS"       , nameof(ICellGetter.AcousticSpace));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XCCM"       , nameof(ICellGetter.SkyAndWeatherFromRegion));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XCIM"       , nameof(ICellGetter.ImageSpace));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XCLL"       , nameof(ICellGetter.Lighting));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XCLR"       , nameof(ICellGetter.Regions));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XCLW"       , nameof(ICellGetter.WaterHeight));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XCLX"       , nameof(ICellGetter.Grid));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XCMO"       , nameof(ICellGetter.Music));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XILL"       , nameof(ICellGetter.LockList));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XNAM"       , nameof(ICellGetter.WaterNoiseTexture));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XOWN"       , nameof(ICellGetter.Owner));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XRNK"       , nameof(ICellGetter.FactionRank));
            game.AddAlias(ICellGetter.StaticRegistration                  , "XWEM"       , nameof(ICellGetter.WaterEnvironmentMap));

            game.AddAlias(IClimateGetter.StaticRegistration               , "FNAM"       , nameof(IClimateGetter.SunTexture));
            game.AddAlias(IClimateGetter.StaticRegistration               , "GNAM"       , nameof(IClimateGetter.SunGlareTexture));
            game.AddAlias(IClimateGetter.StaticRegistration               , "WLST"       , nameof(IClimateGetter.WeatherTypes));

            game.AddAlias(ICollisionLayerGetter.StaticRegistration        , "BNAM"       , nameof(ICollisionLayerGetter.Index));
            game.AddAlias(ICollisionLayerGetter.StaticRegistration        , "CNAM"       , nameof(ICollisionLayerGetter.CollidesWith));
            game.AddAlias(ICollisionLayerGetter.StaticRegistration        , "FNAM"       , nameof(ICollisionLayerGetter.DebugColor));
            game.AddAlias(ICollisionLayerGetter.StaticRegistration        , "GNAM"       , nameof(ICollisionLayerGetter.Flags));
            game.AddAlias(ICollisionLayerGetter.StaticRegistration        , "MNAM"       , nameof(ICollisionLayerGetter.Name));

            game.AddAlias(IColorRecordGetter.StaticRegistration           , "CNAM"       , nameof(IColorRecordGetter.Color));
            game.AddAlias(IColorRecordGetter.StaticRegistration           , "FNAM"       , nameof(IColorRecordGetter.Playable));

            game.AddAlias(ICombatStyleGetter.StaticRegistration           , "CSCR"       , nameof(ICombatStyleGetter.CloseRange));
            game.AddAlias(ICombatStyleGetter.StaticRegistration           , "CSFL"       , nameof(ICombatStyleGetter.Flight));
            game.AddAlias(ICombatStyleGetter.StaticRegistration           , "CSLR"       , nameof(ICombatStyleGetter.LongRangeStrafeMult));
            game.AddAlias(ICombatStyleGetter.StaticRegistration           , "CSME"       , nameof(ICombatStyleGetter.Melee));
            game.AddAlias(ICombatStyleGetter.StaticRegistration           , "DATA"       , nameof(ICombatStyleGetter.Flags));

            game.AddAlias(IConstructibleObjectGetter.StaticRegistration   , "BNAM"       , nameof(IConstructibleObjectGetter.WorkbenchKeyword));
            game.AddAlias(IConstructibleObjectGetter.StaticRegistration   , "CNAM"       , nameof(IConstructibleObjectGetter.CreatedObject));
            game.AddAlias(IConstructibleObjectGetter.StaticRegistration   , "NAM1"       , nameof(IConstructibleObjectGetter.CreatedObjectCount));

            game.AddAlias(IContainerGetter.StaticRegistration             , "QNAM"       , nameof(IContainerGetter.CloseSound));
            game.AddAlias(IContainerGetter.StaticRegistration             , "SNAM"       , nameof(IContainerGetter.OpenSound));

            game.AddAlias(IDefaultObjectManagerGetter.StaticRegistration  , "DNAM"       , nameof(IDefaultObjectManagerGetter.Objects));

            game.AddAlias(IDialogBranchGetter.StaticRegistration          , "DNAM"       , nameof(IDialogBranchGetter.Flags));
            game.AddAlias(IDialogBranchGetter.StaticRegistration          , "QNAM"       , nameof(IDialogBranchGetter.Quest));
            game.AddAlias(IDialogBranchGetter.StaticRegistration          , "SNAM"       , nameof(IDialogBranchGetter.StartingTopic));
            game.AddAlias(IDialogBranchGetter.StaticRegistration          , "TNAM"       , nameof(IDialogBranchGetter.Category));

            game.AddAlias(IDialogTopicGetter.StaticRegistration           , "BNAM"       , nameof(IDialogTopicGetter.Branch));
            game.AddAlias(IDialogTopicGetter.StaticRegistration           , "PNAM"       , nameof(IDialogTopicGetter.Priority));
            game.AddAlias(IDialogTopicGetter.StaticRegistration           , "QNAM"       , nameof(IDialogTopicGetter.Quest));
            game.AddAlias(IDialogTopicGetter.StaticRegistration           , "SNAM"       , nameof(IDialogTopicGetter.Subtype));

            game.AddAlias(IDialogViewGetter.StaticRegistration            , "BNAM"       , nameof(IDialogViewGetter.Branches));
            game.AddAlias(IDialogViewGetter.StaticRegistration            , "QNAM"       , nameof(IDialogViewGetter.Quest));

            game.AddAlias(IDoorGetter.StaticRegistration                  , "ANAM"       , nameof(IDoorGetter.CloseSound));
            game.AddAlias(IDoorGetter.StaticRegistration                  , "BNAM"       , nameof(IDoorGetter.LoopSound));
            game.AddAlias(IDoorGetter.StaticRegistration                  , "FNAM"       , nameof(IDoorGetter.Flags));
            game.AddAlias(IDoorGetter.StaticRegistration                  , "SNAM"       , nameof(IDoorGetter.OpenSound));

            game.AddAlias(IEffectShaderGetter.StaticRegistration          , "ICO2"       , nameof(IEffectShaderGetter.ParticleShaderTexture));
            game.AddAlias(IEffectShaderGetter.StaticRegistration          , "ICON"       , nameof(IEffectShaderGetter.FillTexture));
            game.AddAlias(IEffectShaderGetter.StaticRegistration          , "NAM7"       , nameof(IEffectShaderGetter.HolesTexture));
            game.AddAlias(IEffectShaderGetter.StaticRegistration          , "NAM8"       , nameof(IEffectShaderGetter.MembranePaletteTexture));
            game.AddAlias(IEffectShaderGetter.StaticRegistration          , "NAM9"       , nameof(IEffectShaderGetter.ParticlePaletteTexture));

            game.AddAlias(IEquipTypeGetter.StaticRegistration             , "DATA"       , nameof(IEquipTypeGetter.UseAllParents));
            game.AddAlias(IEquipTypeGetter.StaticRegistration             , "PNAM"       , nameof(IEquipTypeGetter.SlotParents));

            game.AddAlias(IExplosionGetter.StaticRegistration             , "MNAM"       , nameof(IExplosionGetter.ImageSpaceModifier));

            game.AddAlias(IEyesGetter.StaticRegistration                  , "DATA"       , nameof(IEyesGetter.Flags));
            game.AddAlias(IEyesGetter.StaticRegistration                  , "Texture"    , nameof(IEyesGetter.Icon));

            game.AddAlias(IFactionGetter.StaticRegistration               , "CRGR"       , nameof(IFactionGetter.SharedCrimeFactionList));
            game.AddAlias(IFactionGetter.StaticRegistration               , "CRVA"       , nameof(IFactionGetter.CrimeValues));
            game.AddAlias(IFactionGetter.StaticRegistration               , "DATA"       , nameof(IFactionGetter.Flags));
            game.AddAlias(IFactionGetter.StaticRegistration               , "JAIL"       , nameof(IFactionGetter.ExteriorJailMarker));
            game.AddAlias(IFactionGetter.StaticRegistration               , "JOUT"       , nameof(IFactionGetter.JailOutfit));
            game.AddAlias(IFactionGetter.StaticRegistration               , "PLCN"       , nameof(IFactionGetter.PlayerInventoryContainer));
            game.AddAlias(IFactionGetter.StaticRegistration               , "PLVD"       , nameof(IFactionGetter.VendorLocation));
            game.AddAlias(IFactionGetter.StaticRegistration               , "STOL"       , nameof(IFactionGetter.StolenGoodsContainer));
            game.AddAlias(IFactionGetter.StaticRegistration               , "VENC"       , nameof(IFactionGetter.MerchantContainer));
            game.AddAlias(IFactionGetter.StaticRegistration               , "VEND"       , nameof(IFactionGetter.VendorBuySellList));
            game.AddAlias(IFactionGetter.StaticRegistration               , "VENV"       , nameof(IFactionGetter.VendorValues));
            game.AddAlias(IFactionGetter.StaticRegistration               , "WAIT"       , nameof(IFactionGetter.FollowerWaitMarker));
            game.AddAlias(IFactionGetter.StaticRegistration               , "XNAM"       , nameof(IFactionGetter.Relations));

            game.AddAlias(IFloraGetter.StaticRegistration                 , "PFIG"       , nameof(IFloraGetter.Ingredient));
            game.AddAlias(IFloraGetter.StaticRegistration                 , "PFPC"       , nameof(IFloraGetter.Production));
            game.AddAlias(IFloraGetter.StaticRegistration                 , "RNAM"       , nameof(IFloraGetter.ActivateTextOverride));
            game.AddAlias(IFloraGetter.StaticRegistration                 , "SNAM"       , nameof(IFloraGetter.HarvestSound));

            game.AddAlias(IFootstepGetter.StaticRegistration              , "ANAM"       , nameof(IFootstepGetter.Tag));
            game.AddAlias(IFootstepGetter.StaticRegistration              , "DATA"       , nameof(IFootstepGetter.ImpactDataSet));

            game.AddAlias(IFormListGetter.StaticRegistration              , "FormID"     , nameof(IFormListGetter.Items));
            game.AddAlias(IFormListGetter.StaticRegistration              , "FormIDs"    , nameof(IFormListGetter.Items));
            game.AddAlias(IFormListGetter.StaticRegistration              , "LNAM"       , nameof(IFormListGetter.Items));

            game.AddAlias(IFurnitureGetter.StaticRegistration             , "FNAM"       , nameof(IFurnitureGetter.Flags));
            game.AddAlias(IFurnitureGetter.StaticRegistration             , "KNAM"       , nameof(IFurnitureGetter.InteractionKeyword));
            game.AddAlias(IFurnitureGetter.StaticRegistration             , "MNAM"       , nameof(IFurnitureGetter.Flags));
            game.AddAlias(IFurnitureGetter.StaticRegistration             , "NAM1"       , nameof(IFurnitureGetter.AssociatedSpell));
            game.AddAlias(IFurnitureGetter.StaticRegistration             , "WBDT"       , nameof(IFurnitureGetter.WorkbenchData));
            game.AddAlias(IFurnitureGetter.StaticRegistration             , "XMRK"       , nameof(IFurnitureGetter.ModelFilename));

            game.AddAlias(IGlobalGetter.StaticRegistration                , "FLTV"       , nameof(IGlobalFloatGetter.Data));
            game.AddAlias(IGlobalGetter.StaticRegistration                , "FNAM"       , nameof(IGlobalGetter.Type));

            game.AddAlias(IHazardGetter.StaticRegistration                , "MNAM"       , nameof(IHazardGetter.ImageSpaceModifier));

            game.AddAlias(IHeadPartGetter.StaticRegistration              , "CNAM"       , nameof(IHeadPartGetter.Color));
            game.AddAlias(IHeadPartGetter.StaticRegistration              , "DATA"       , nameof(IHeadPartGetter.Flags));
            game.AddAlias(IHeadPartGetter.StaticRegistration              , "HNAM"       , nameof(IHeadPartGetter.ExtraParts));
            game.AddAlias(IHeadPartGetter.StaticRegistration              , "PNAM"       , nameof(IHeadPartGetter.Type));
            game.AddAlias(IHeadPartGetter.StaticRegistration              , "RNAM"       , nameof(IHeadPartGetter.ValidRaces));
            game.AddAlias(IHeadPartGetter.StaticRegistration              , "TNAM"       , nameof(IHeadPartGetter.TextureSet));

            game.AddAlias(IIdleAnimationGetter.StaticRegistration         , "ANAM"       , nameof(IIdleAnimationGetter.RelatedIdles));
            game.AddAlias(IIdleAnimationGetter.StaticRegistration         , "DNAM"       , nameof(IIdleAnimationGetter.Filename));
            game.AddAlias(IIdleAnimationGetter.StaticRegistration         , "ENAM"       , nameof(IIdleAnimationGetter.AnimationEvent));

            game.AddAlias(IIdleMarkerGetter.StaticRegistration            , "IDLA"       , nameof(IIdleMarkerGetter.Animations));
            game.AddAlias(IIdleMarkerGetter.StaticRegistration            , "IDLF"       , nameof(IIdleMarkerGetter.Flags));
            game.AddAlias(IIdleMarkerGetter.StaticRegistration            , "IDLT"       , nameof(IIdleMarkerGetter.IdleTimer));

            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "BNAM"       , nameof(IImageSpaceAdapterGetter.BlurRadius));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "NAM1"       , nameof(IImageSpaceAdapterGetter.RadialBlurRampDown));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "NAM2"       , nameof(IImageSpaceAdapterGetter.RadialBlurDownStart));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "NAM3"       , nameof(IImageSpaceAdapterGetter.FadeColor));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "NAM4"       , nameof(IImageSpaceAdapterGetter.MotionBlurStrength));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "RNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurStrength));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "SNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurRampUp));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "TNAM"       , nameof(IImageSpaceAdapterGetter.TintColor));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "UNAM"       , nameof(IImageSpaceAdapterGetter.RadialBlurStart));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "VNAM"       , nameof(IImageSpaceAdapterGetter.DoubleVisionStrength));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "WNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldStrength));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "YNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldDistance));
            game.AddAlias(IImageSpaceAdapterGetter.StaticRegistration     , "ZNAM"       , nameof(IImageSpaceAdapterGetter.DepthOfFieldRange));

            game.AddAlias(IImageSpaceGetter.StaticRegistration            , "CNAM"       , nameof(IImageSpaceGetter.Cinematic));
            game.AddAlias(IImageSpaceGetter.StaticRegistration            , "DNAM"       , nameof(IImageSpaceGetter.DepthOfField));
            game.AddAlias(IImageSpaceGetter.StaticRegistration            , "HNAM"       , nameof(IImageSpaceGetter.Hdr));
            game.AddAlias(IImageSpaceGetter.StaticRegistration            , "TNAM"       , nameof(IImageSpaceGetter.Tint));

            game.AddAlias(IImpactGetter.StaticRegistration                , "DNAM"       , nameof(IImpactGetter.TextureSet));
            game.AddAlias(IImpactGetter.StaticRegistration                , "DODT"       , nameof(IImpactGetter.Decal));
            game.AddAlias(IImpactGetter.StaticRegistration                , "ENAM"       , nameof(IImpactGetter.SecondaryTextureSet));
            game.AddAlias(IImpactGetter.StaticRegistration                , "NAM1"       , nameof(IImpactGetter.Sound2));
            game.AddAlias(IImpactGetter.StaticRegistration                , "NAM2"       , nameof(IImpactGetter.Hazard));
            game.AddAlias(IImpactGetter.StaticRegistration                , "SNAM"       , nameof(IImpactGetter.Sound1));

            game.AddAlias(IIngestibleGetter.StaticRegistration            , "DATA"       , nameof(IIngestibleGetter.Weight));
            game.AddAlias(IIngredientGetter.StaticRegistration            , "ETYP"       , nameof(IIngredientGetter.EquipType));

            game.AddAlias(IKeywordGetter.StaticRegistration               , "CNAM"       , nameof(IKeywordGetter.Color));

            game.AddAlias(ILandscapeTextureGetter.StaticRegistration      , "GNAM"       , nameof(ILandscapeTextureGetter.Grasses));
            game.AddAlias(ILandscapeTextureGetter.StaticRegistration      , "INAM"       , nameof(ILandscapeTextureGetter.Flags));
            game.AddAlias(ILandscapeTextureGetter.StaticRegistration      , "MNAM"       , nameof(ILandscapeTextureGetter.MaterialType));
            game.AddAlias(ILandscapeTextureGetter.StaticRegistration      , "SNAM"       , nameof(ILandscapeTextureGetter.TextureSpecularExponent));
            game.AddAlias(ILandscapeTextureGetter.StaticRegistration      , "TNAM"       , nameof(ILandscapeTextureGetter.TextureSet));

            game.AddAlias(ILeveledItemGetter.StaticRegistration           , "LVLD"       , nameof(ILeveledItemGetter.ChanceNone));
            game.AddAlias(ILeveledItemGetter.StaticRegistration           , "LVLF"       , nameof(ILeveledItemGetter.Flags));
            game.AddAlias(ILeveledItemGetter.StaticRegistration           , "LVLG"       , nameof(ILeveledItemGetter.Global));

            game.AddAlias(ILeveledNpcGetter.StaticRegistration            , "LVLD"       , nameof(ILeveledNpcGetter.ChanceNone));
            game.AddAlias(ILeveledNpcGetter.StaticRegistration            , "LVLF"       , nameof(ILeveledNpcGetter.Flags));
            game.AddAlias(ILeveledNpcGetter.StaticRegistration            , "LVLG"       , nameof(ILeveledNpcGetter.Global));

            game.AddAlias(ILeveledSpellGetter.StaticRegistration          , "LVLD"       , nameof(ILeveledSpellGetter.ChanceNone));
            game.AddAlias(ILeveledSpellGetter.StaticRegistration          , "LVLF"       , nameof(ILeveledSpellGetter.Flags));

            game.AddAlias(ILightGetter.StaticRegistration                 , "FNAM"       , nameof(ILightGetter.FadeValue));
            game.AddAlias(ILightGetter.StaticRegistration                 , "LNAM"       , nameof(ILightGetter.Lens));
            game.AddAlias(ILightGetter.StaticRegistration                 , "SNAM"       , nameof(ILightGetter.Sound));

            game.AddAlias(ILightingTemplateGetter.StaticRegistration      , "DALC"       , nameof(ILightingTemplateGetter.DirectionalAmbientColors));

            game.AddAlias(ILoadScreenGetter.StaticRegistration            , "MOD2"       , nameof(ILoadScreenGetter.CameraPath));
            game.AddAlias(ILoadScreenGetter.StaticRegistration            , "NNAM"       , nameof(ILoadScreenGetter.LoadingScreenNif));
            game.AddAlias(ILoadScreenGetter.StaticRegistration            , "ONAM"       , nameof(ILoadScreenGetter.RotationOffsetConstraints));
            game.AddAlias(ILoadScreenGetter.StaticRegistration            , "RNAM"       , nameof(ILoadScreenGetter.InitialRotation));
            game.AddAlias(ILoadScreenGetter.StaticRegistration            , "SNAM"       , nameof(ILoadScreenGetter.InitialScale));
            game.AddAlias(ILoadScreenGetter.StaticRegistration            , "XNAM"       , nameof(ILoadScreenGetter.InitialTranslationOffset));

            game.AddAlias(ILocationGetter.StaticRegistration              , "CNAM"       , nameof(ILocationGetter.Color));
            game.AddAlias(ILocationGetter.StaticRegistration              , "FNAM"       , nameof(ILocationGetter.UnreportedCrimeFaction));
            game.AddAlias(ILocationGetter.StaticRegistration              , "MNAM"       , nameof(ILocationGetter.WorldLocationMarkerRef));
            game.AddAlias(ILocationGetter.StaticRegistration              , "NAM0"       , nameof(ILocationGetter.HorseMarkerRef));
            game.AddAlias(ILocationGetter.StaticRegistration              , "NAM1"       , nameof(ILocationGetter.Music));
            game.AddAlias(ILocationGetter.StaticRegistration              , "PNAM"       , nameof(ILocationGetter.ParentLocation));
            game.AddAlias(ILocationGetter.StaticRegistration              , "RNAM"       , nameof(ILocationGetter.WorldLocationRadius));

            game.AddAlias(ILocationReferenceTypeGetter.StaticRegistration , "CNAM"       , nameof(ILocationReferenceTypeGetter.Color));

            game.AddAlias(IMagicEffectGetter.StaticRegistration           , "DNAM"       , nameof(IMagicEffectGetter.Description));
            game.AddAlias(IMagicEffectGetter.StaticRegistration           , "MDOB"       , nameof(IMagicEffectGetter.MenuDisplayObject));
            game.AddAlias(IMagicEffectGetter.StaticRegistration           , "SNDD"       , nameof(IMagicEffectGetter.Sounds));

            game.AddAlias(IMaterialTypeGetter.StaticRegistration          , "BNAM"       , nameof(IMaterialTypeGetter.Buoyancy));
            game.AddAlias(IMaterialTypeGetter.StaticRegistration          , "CNAM"       , nameof(IMaterialTypeGetter.HavokDisplayColor));
            game.AddAlias(IMaterialTypeGetter.StaticRegistration          , "FNAM"       , nameof(IMaterialTypeGetter.Flags));
            game.AddAlias(IMaterialTypeGetter.StaticRegistration          , "HNAM"       , nameof(IMaterialTypeGetter.HavokImpactDataSet));
            game.AddAlias(IMaterialTypeGetter.StaticRegistration          , "MNAM"       , nameof(IMaterialTypeGetter.Name));
            game.AddAlias(IMaterialTypeGetter.StaticRegistration          , "PNAM"       , nameof(IMaterialTypeGetter.Parent));

            game.AddAlias(IMessageGetter.StaticRegistration               , "DNAM"       , nameof(IMessageGetter.Flags));
            game.AddAlias(IMessageGetter.StaticRegistration               , "QNAM"       , nameof(IMessageGetter.Quest));
            game.AddAlias(IMessageGetter.StaticRegistration               , "TNAM"       , nameof(IMessageGetter.DisplayTime));

            game.AddAlias(IMiscItemGetter.StaticRegistration              , "ICON"       , "Icons.LargeIconFilename");
            game.AddAlias(IMiscItemGetter.StaticRegistration              , "MICO"       , "Icons.SmallIconFilename");

            game.AddAlias(IMoveableStaticGetter.StaticRegistration        , "DATA"       , nameof(IMoveableStaticGetter.Flags));
            game.AddAlias(IMoveableStaticGetter.StaticRegistration        , "SNAM"       , nameof(IMoveableStaticGetter.LoopingSound));

            game.AddAlias(IMovementTypeGetter.StaticRegistration          , "FULL"       , null);
            game.AddAlias(IMovementTypeGetter.StaticRegistration          , "INAM"       , nameof(IMovementTypeGetter.AnimationChangeThresholds));
            game.AddAlias(IMovementTypeGetter.StaticRegistration          , "MNAM"       , nameof(IMovementTypeGetter.Name));

            game.AddAlias(IMusicTrackGetter.StaticRegistration            , "ANAM"       , nameof(IMusicTrackGetter.TrackFilename));
            game.AddAlias(IMusicTrackGetter.StaticRegistration            , "BNAM"       , nameof(IMusicTrackGetter.FinaleFilename));
            game.AddAlias(IMusicTrackGetter.StaticRegistration            , "CNAM"       , nameof(IMusicTrackGetter.Type));
            game.AddAlias(IMusicTrackGetter.StaticRegistration            , "DNAM"       , nameof(IMusicTrackGetter.FadeOut));
            game.AddAlias(IMusicTrackGetter.StaticRegistration            , "FLTV"       , nameof(IMusicTrackGetter.Duration));
            game.AddAlias(IMusicTrackGetter.StaticRegistration            , "FNAM"       , nameof(IMusicTrackGetter.CuePoints));
            game.AddAlias(IMusicTrackGetter.StaticRegistration            , "LNAM"       , nameof(IMusicTrackGetter.LoopData));
            game.AddAlias(IMusicTrackGetter.StaticRegistration            , "SNAM"       , nameof(IMusicTrackGetter.Tracks));

            game.AddAlias(IMusicTypeGetter.StaticRegistration             , "FNAM"       , nameof(IMusicTypeGetter.Flags));
            game.AddAlias(IMusicTypeGetter.StaticRegistration             , "TNAM"       , nameof(IMusicTypeGetter.Tracks));
            game.AddAlias(IMusicTypeGetter.StaticRegistration             , "WNAM"       , nameof(IMusicTypeGetter.FadeDuration));

            game.AddAlias(INpcGetter.StaticRegistration                   , "AIDT"       , nameof(INpcGetter.AIData));
            game.AddAlias(INpcGetter.StaticRegistration                   , "ANAM"       , nameof(INpcGetter.FarAwayModel));
            game.AddAlias(INpcGetter.StaticRegistration                   , "ATKR"       , nameof(INpcGetter.AttackRace));
            game.AddAlias(INpcGetter.StaticRegistration                   , "CNAM"       , nameof(INpcGetter.Class));
            game.AddAlias(INpcGetter.StaticRegistration                   , "CRIF"       , nameof(INpcGetter.CrimeFaction));
            game.AddAlias(INpcGetter.StaticRegistration                   , "DNAM"       , nameof(INpcGetter.PlayerSkills));
            game.AddAlias(INpcGetter.StaticRegistration                   , "DOFT"       , nameof(INpcGetter.DefaultOutfit));
            game.AddAlias(INpcGetter.StaticRegistration                   , "DPLT"       , nameof(INpcGetter.DefaultPackageList));
            game.AddAlias(INpcGetter.StaticRegistration                   , "ECOR"       , nameof(INpcGetter.CombatOverridePackageList));
            game.AddAlias(INpcGetter.StaticRegistration                   , "FTST"       , nameof(INpcGetter.HeadTexture));
            game.AddAlias(INpcGetter.StaticRegistration                   , "GNAM"       , nameof(INpcGetter.GiftFilter));
            game.AddAlias(INpcGetter.StaticRegistration                   , "GWOR"       , nameof(INpcGetter.GuardWarnOverridePackageList));
            game.AddAlias(INpcGetter.StaticRegistration                   , "HCLF"       , nameof(INpcGetter.HairColor));
            game.AddAlias(INpcGetter.StaticRegistration                   , "INAM"       , nameof(INpcGetter.DeathItem));
            game.AddAlias(INpcGetter.StaticRegistration                   , "NAM6"       , nameof(INpcGetter.Height));
            game.AddAlias(INpcGetter.StaticRegistration                   , "NAM7"       , nameof(INpcGetter.Weight));
            game.AddAlias(INpcGetter.StaticRegistration                   , "NAM8"       , nameof(INpcGetter.SoundLevel));
            game.AddAlias(INpcGetter.StaticRegistration                   , "NAM9"       , nameof(INpcGetter.FaceMorph));
            game.AddAlias(INpcGetter.StaticRegistration                   , "NAMA"       , nameof(INpcGetter.FaceParts));
            game.AddAlias(INpcGetter.StaticRegistration                   , "OCOR"       , nameof(INpcGetter.ObserveDeadBodyOverridePackageList));
            game.AddAlias(INpcGetter.StaticRegistration                   , "PKID"       , nameof(INpcGetter.Packages));
            game.AddAlias(INpcGetter.StaticRegistration                   , "PNAM"       , nameof(INpcGetter.HeadParts));
            game.AddAlias(INpcGetter.StaticRegistration                   , "PRKR"       , nameof(INpcGetter.Perks));
            game.AddAlias(INpcGetter.StaticRegistration                   , "QNAM"       , nameof(INpcGetter.TextureLighting));
            game.AddAlias(INpcGetter.StaticRegistration                   , "SHRT"       , nameof(INpcGetter.ShortName));
            game.AddAlias(INpcGetter.StaticRegistration                   , "SNAM"       , nameof(INpcGetter.Factions));
            game.AddAlias(INpcGetter.StaticRegistration                   , "SOFT"       , nameof(INpcGetter.SleepingOutfit));
            game.AddAlias(INpcGetter.StaticRegistration                   , "SPLO"       , nameof(INpcGetter.ActorEffect));
            game.AddAlias(INpcGetter.StaticRegistration                   , "SPOR"       , nameof(INpcGetter.SpectatorOverridePackageList));
            game.AddAlias(INpcGetter.StaticRegistration                   , "TPLT"       , nameof(INpcGetter.Template));
            game.AddAlias(INpcGetter.StaticRegistration                   , "VTCK"       , nameof(INpcGetter.Voice));
            game.AddAlias(INpcGetter.StaticRegistration                   , "WNAM"       , nameof(INpcGetter.WornArmor));
            game.AddAlias(INpcGetter.StaticRegistration                   , "ZNAM"       , nameof(INpcGetter.CombatStyle));

            game.AddAlias(IOutfitGetter.StaticRegistration                , "INAM"       , nameof(IOutfitGetter.Items));

            game.AddAlias(IPackageGetter.StaticRegistration               , "CNAM"       , nameof(IPackageGetter.CombatStyle));
            game.AddAlias(IPackageGetter.StaticRegistration               , "QNAM"       , nameof(IPackageGetter.OwnerQuest));
            game.AddAlias(IPackageGetter.StaticRegistration               , "XNAM"       , nameof(IPackageGetter.XnamMarker));

            game.AddAlias(IPerkGetter.StaticRegistration                  , "NNAM"       , nameof(IPerkGetter.NextPerk));

            game.AddAlias(IProjectileGetter.StaticRegistration            , "VNAM"       , nameof(IProjectileGetter.SoundLevel));

            game.AddAlias(IQuestGetter.StaticRegistration                 , "ANAM"       , nameof(IQuestGetter.NextAliasID));
            game.AddAlias(IQuestGetter.StaticRegistration                 , "DESC"       , null);
            game.AddAlias(IQuestGetter.StaticRegistration                 , "ENAM"       , nameof(IQuestGetter.Event));
            game.AddAlias(IQuestGetter.StaticRegistration                 , "FLTR"       , nameof(IQuestGetter.Filter));
            game.AddAlias(IQuestGetter.StaticRegistration                 , "NNAM"       , nameof(IQuestGetter.Description));
            game.AddAlias(IQuestGetter.StaticRegistration                 , "QTGL"       , nameof(IQuestGetter.TextDisplayGlobals));

            game.AddAlias(IRaceGetter.StaticRegistration                  , "ANAM"       , nameof(IRaceGetter.SkeletalModel));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "ATKR"       , nameof(IRaceGetter.AttackRace));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "BOD2"       , nameof(IRaceGetter.BodyTemplate));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "DNAM"       , nameof(IRaceGetter.DecapitateArmors));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "ENAM"       , nameof(IRaceGetter.Eyes));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "FLMV"       , nameof(IRaceGetter.BaseMovementDefaultFly));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "GNAM"       , nameof(IRaceGetter.BodyPartData));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "HCLF"       , nameof(IRaceGetter.DefaultHairColors));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "HNAM"       , nameof(IRaceGetter.Hairs));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "LNAM"       , nameof(IRaceGetter.CloseLootSound));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "MTNM"       , nameof(IRaceGetter.MovementTypeNames));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "NAM4"       , nameof(IRaceGetter.MaterialType));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "NAM5"       , nameof(IRaceGetter.ImpactDataSet));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "NAM7"       , nameof(IRaceGetter.DecapitationFX));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "NAM8"       , nameof(IRaceGetter.MorphRace));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "ONAM"       , nameof(IRaceGetter.OpenLootSound));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "PNAM"       , nameof(IRaceGetter.FacegenMainClamp));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "QNAM"       , nameof(IRaceGetter.EquipmentSlots));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "RNAM"       , nameof(IRaceGetter.ArmorRace));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "RNMV"       , nameof(IRaceGetter.BaseMovementDefaultRun));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "SNMV"       , nameof(IRaceGetter.BaseMovementDefaultSneak));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "SPMV"       , nameof(IRaceGetter.BaseMovementDefaultSprint));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "SWMV"       , nameof(IRaceGetter.BaseMovementDefaultSwim));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "UNAM"       , nameof(IRaceGetter.FacegenFaceClamp));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "UNES"       , nameof(IRaceGetter.UnarmedEquipSlot));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "VNAM"       , nameof(IRaceGetter.EquipmentFlags));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "VTCK"       , nameof(IRaceGetter.Voices));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "WKMV"       , nameof(IRaceGetter.BaseMovementDefaultWalk));
            game.AddAlias(IRaceGetter.StaticRegistration                  , "WNAM"       , nameof(IRaceGetter.Skin));

            game.AddAlias(IRegionGetter.StaticRegistration                , "RCLR"       , nameof(IRegionGetter.MapColor));
            game.AddAlias(IRegionGetter.StaticRegistration                , "WNAM"       , nameof(IRegionGetter.Worldspace));

            game.AddAlias(ISceneGetter.StaticRegistration                 , "FNAM"       , nameof(ISceneGetter.Flags));
            game.AddAlias(ISceneGetter.StaticRegistration                 , "INAM"       , nameof(ISceneGetter.LastActionIndex));
            game.AddAlias(ISceneGetter.StaticRegistration                 , "PNAM"       , nameof(ISceneGetter.Quest));

            game.AddAlias(IScrollGetter.StaticRegistration                , "MDOB"       , nameof(IScrollGetter.MenuDisplayObject));

            game.AddAlias(IShaderParticleGeometryGetter.StaticRegistration, "ICON"       , nameof(IShaderParticleGeometryGetter.ParticleTexture));

            game.AddAlias(IShoutGetter.StaticRegistration                 , "MDOB"       , nameof(IShoutGetter.MenuDisplayObject));

            game.AddAlias(ISoulGemGetter.StaticRegistration               , "NAM0"       , nameof(ISoulGemGetter.LinkedTo));
            game.AddAlias(ISoulGemGetter.StaticRegistration               , "SLCP"       , nameof(ISoulGemGetter.MaximumCapacity));
            game.AddAlias(ISoulGemGetter.StaticRegistration               , "SOUL"       , nameof(ISoulGemGetter.ContainedSoul));

            game.AddAlias(ISoundCategoryGetter.StaticRegistration         , "FNAM"       , nameof(ISoundCategoryGetter.Flags));
            game.AddAlias(ISoundCategoryGetter.StaticRegistration         , "PNAM"       , nameof(ISoundCategoryGetter.Parent));
            game.AddAlias(ISoundCategoryGetter.StaticRegistration         , "UNAM"       , nameof(ISoundCategoryGetter.DefaultMenuVolume));
            game.AddAlias(ISoundCategoryGetter.StaticRegistration         , "VNAM"       , nameof(ISoundCategoryGetter.StaticVolumeMultiplier));

            game.AddAlias(ISoundDescriptorGetter.StaticRegistration       , "CNAM"       , nameof(ISoundDescriptorGetter.Type));
            game.AddAlias(ISoundDescriptorGetter.StaticRegistration       , "FNAM"       , nameof(ISoundDescriptorGetter.String));
            game.AddAlias(ISoundDescriptorGetter.StaticRegistration       , "GNAM"       , nameof(ISoundDescriptorGetter.Category));
            game.AddAlias(ISoundDescriptorGetter.StaticRegistration       , "ONAM"       , nameof(ISoundDescriptorGetter.OutputModel));
            game.AddAlias(ISoundDescriptorGetter.StaticRegistration       , "SNAM"       , nameof(ISoundDescriptorGetter.AlternateSoundFor));

            game.AddAlias(ISoundMarkerGetter.StaticRegistration           , "SDSC"       , nameof(ISoundMarkerGetter.SoundDescriptor));

            game.AddAlias(ISoundOutputModelGetter.StaticRegistration      , "MNAM"       , nameof(ISoundOutputModelGetter.Type));
            game.AddAlias(ISoundOutputModelGetter.StaticRegistration      , "ONAM"       , nameof(ISoundOutputModelGetter.OutputChannels));

            game.AddAlias(ISpellGetter.StaticRegistration                 , "MDOB"       , nameof(ISpellGetter.MenuDisplayObject));

            game.AddAlias(IStoryManagerBranchNodeGetter.StaticRegistration, "DNAM"       , nameof(IStoryManagerBranchNodeGetter.Flags));
            game.AddAlias(IStoryManagerBranchNodeGetter.StaticRegistration, "PNAM"       , nameof(IStoryManagerBranchNodeGetter.Parent));
            game.AddAlias(IStoryManagerBranchNodeGetter.StaticRegistration, "SNAM"       , nameof(IStoryManagerBranchNodeGetter.PreviousSibling));
            game.AddAlias(IStoryManagerBranchNodeGetter.StaticRegistration, "XNAM"       , nameof(IStoryManagerBranchNodeGetter.MaxConcurrentQuests));

            game.AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "DNAM"       , nameof(IStoryManagerEventNodeGetter.Flags));
            game.AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "ENAM"       , nameof(IStoryManagerEventNodeGetter.Type));
            game.AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "PNAM"       , nameof(IStoryManagerEventNodeGetter.Parent));
            game.AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "SNAM"       , nameof(IStoryManagerEventNodeGetter.PreviousSibling));
            game.AddAlias(IStoryManagerEventNodeGetter.StaticRegistration , "XNAM"       , nameof(IStoryManagerEventNodeGetter.MaxConcurrentQuests));
            game.AddAlias(IStoryManagerQuestNodeGetter.StaticRegistration , "MNAM"       , nameof(IStoryManagerQuestNodeGetter.MaxNumQuestsToRun));
            game.AddAlias(IStoryManagerQuestNodeGetter.StaticRegistration , "PNAM"       , nameof(IStoryManagerQuestNodeGetter.Parent));
            game.AddAlias(IStoryManagerQuestNodeGetter.StaticRegistration , "SNAM"       , nameof(IStoryManagerQuestNodeGetter.PreviousSibling));
            game.AddAlias(IStoryManagerQuestNodeGetter.StaticRegistration , "XNAM"       , nameof(IStoryManagerQuestNodeGetter.MaxConcurrentQuests));

            game.AddAlias(ITalkingActivatorGetter.StaticRegistration      , "SNAM"       , nameof(ITalkingActivatorGetter.LoopingSound));
            game.AddAlias(ITalkingActivatorGetter.StaticRegistration      , "VNAM"       , nameof(ITalkingActivatorGetter.VoiceType));

            game.AddAlias(ITextureSetGetter.StaticRegistration            , "DNAM"       , nameof(ITextureSetGetter.Flags));
            game.AddAlias(ITextureSetGetter.StaticRegistration            , "DODT"       , nameof(ITextureSetGetter.Decal));

            game.AddAlias(ITreeGetter.StaticRegistration                  , "PFIG"       , nameof(ITreeGetter.Ingredient));
            game.AddAlias(ITreeGetter.StaticRegistration                  , "SNAM"       , nameof(ITreeGetter.HarvestSound));

            game.AddAlias(IVoiceTypeGetter.StaticRegistration             , "DNAM"       , nameof(IVoiceTypeGetter.Flags));

            game.AddAlias(IWaterGetter.StaticRegistration                 , "ANAM"       , nameof(IWaterGetter.Opacity));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "FNAM"       , nameof(IWaterGetter.Flags));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "INAM"       , nameof(IWaterGetter.ImageSpace));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "NAM0"       , nameof(IWaterGetter.LinearVelocity));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "NAM1"       , nameof(IWaterGetter.AngularVelocity));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "NAM2"       , nameof(IWaterGetter.NoiseLayerOneTexture));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "NAM3"       , nameof(IWaterGetter.NoiseLayerTwoTexture));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "NAM4"       , nameof(IWaterGetter.NoiseLayerThreeTexture));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "NAM5"       , nameof(IWaterGetter.FlowNormalsNoiseTexture));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "SNAM"       , nameof(IWaterGetter.OpenSound));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "TNAM"       , nameof(IWaterGetter.Material));
            game.AddAlias(IWaterGetter.StaticRegistration                 , "XNAM"       , nameof(IWaterGetter.Spell));

            game.AddAlias(IWeaponGetter.StaticRegistration                , "BAMT"       , nameof(IWeaponGetter.AlternateBlockMaterial));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "BIDS"       , nameof(IWeaponGetter.BlockBashImpact));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "CNAM"       , nameof(IWeaponGetter.Template));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "INAM"       , nameof(IWeaponGetter.ImpactDataSet));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "NAM7"       , nameof(IWeaponGetter.AttackLoopSound));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "NAM8"       , nameof(IWeaponGetter.UnequipSound));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "NAM9"       , nameof(IWeaponGetter.EquipSound));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "SNAM"       , nameof(IWeaponGetter.AttackSound));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "TNAM"       , nameof(IWeaponGetter.AttackFailSound));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "UNAM"       , nameof(IWeaponGetter.IdleSound));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "VNAM"       , nameof(IWeaponGetter.DetectionSoundLevel));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "WNAM"       , nameof(IWeaponGetter.FirstPersonModel));
            game.AddAlias(IWeaponGetter.StaticRegistration                , "XNAM"       , nameof(IWeaponGetter.AttackSound2D));

            game.AddAlias(IWeatherGetter.StaticRegistration               , "DALC"       , nameof(IWeatherGetter.DirectionalAmbientLightingColors));
            game.AddAlias(IWeatherGetter.StaticRegistration               , "GNAM"       , nameof(IWeatherGetter.SunGlareLensFlare));
            game.AddAlias(IWeatherGetter.StaticRegistration               , "HNAM"       , nameof(IWeatherGetter.VolumetricLighting));
            game.AddAlias(IWeatherGetter.StaticRegistration               , "IMSP"       , nameof(IWeatherGetter.ImageSpaces));
            game.AddAlias(IWeatherGetter.StaticRegistration               , "MNAM"       , nameof(IWeatherGetter.Precipitation));
            game.AddAlias(IWeatherGetter.StaticRegistration               , "NNAM"       , nameof(IWeatherGetter.VisualEffect));

            game.AddAlias(IWordOfPowerGetter.StaticRegistration           , "TNAM"       , nameof(IWordOfPowerGetter.Translation));

            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "CNAM"       , nameof(IWorldspaceGetter.Climate));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "DATA"       , nameof(IWorldspaceGetter.Flags));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "DNAM"       , nameof(IWorldspaceGetter.LandDefaults));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "ICON"       , nameof(IWorldspaceGetter.MapImage));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "LTMP"       , nameof(IWorldspaceGetter.InteriorLighting));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "MHDT"       , nameof(IWorldspaceGetter.MaxHeight));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "MNAM"       , nameof(IWorldspaceGetter.MapData));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM0"       , nameof(IWorldspaceGetter.ObjectBoundsMin));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM2"       , nameof(IWorldspaceGetter.Water));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM3"       , nameof(IWorldspaceGetter.LodWater));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM4"       , nameof(IWorldspaceGetter.LodWaterHeight));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "NAM9"       , nameof(IWorldspaceGetter.ObjectBoundsMax));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "NAMA"       , nameof(IWorldspaceGetter.DistantLodMultiplier));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "NNAM"       , nameof(IWorldspaceGetter.CanopyShadow));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "OFST"       , nameof(IWorldspaceGetter.OffsetData));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "TNAM"       , nameof(IWorldspaceGetter.HdLodDiffuseTexture));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "UNAM"       , nameof(IWorldspaceGetter.HdLodNormalTexture));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "WCTR"       , nameof(IWorldspaceGetter.FixedDimensionsCenterCell));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "XNAM"       , nameof(IWorldspaceGetter.WaterNoiseTexture));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "XWEM"       , nameof(IWorldspaceGetter.WaterEnvironmentMap));
            game.AddAlias(IWorldspaceGetter.StaticRegistration            , "ZNAM"       , nameof(IWorldspaceGetter.Music));
#pragma warning restore format
        }
    }
}