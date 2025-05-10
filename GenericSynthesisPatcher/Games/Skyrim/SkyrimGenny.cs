using Common;

using GenericSynthesisPatcher.Games.Skyrim.Action;
using GenericSynthesisPatcher.Games.Universal;

using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Games.Skyrim
{
    public class SkyrimGenny : Genny
    {
        public SkyrimGenny ()
        {
            IgnoreDeepScanOnTypes = IgnoreDeepScanOnTypes.AddRange(
                [
                typeof(AMagicEffectArchetype),
                typeof(Cell),
                typeof(CellMaxHeightData),
                typeof(DialogResponsesAdapter),
                typeof(FaceFxPhonemes),
                typeof(Landscape),
                typeof(LocationTargetRadius),
                typeof(Model),
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

            addMapping(typeof(WorldspaceMaxHeight), true, typeof(WorldspaceMaxHeightAction));
            addMapping(typeof(CellMaxHeightData), true, typeof(CellMaxHeightDataAction));
            addMapping(typeof(Model), true, typeof(ModelAction));

            addMapping(typeof(IObjectBoundsGetter), false, typeof(ObjectBoundsAction));
            addMapping(typeof(IPlayerSkillsGetter), false, typeof(PlayerSkillsAction));

            addMapping([typeof(ExtendedList<>), typeof(IContainerEntryGetter)], false, typeof(ContainerItemsAction));
            addMapping([typeof(ExtendedList<>), typeof(IEffectGetter)], false, typeof(EffectsAction));
            addMapping([typeof(ExtendedList<>), typeof(ILeveledItemEntryGetter)], false, typeof(LeveledItemAction));
            addMapping([typeof(ExtendedList<>), typeof(ILeveledNpcEntryGetter)], false, typeof(LeveledNpcAction));
            addMapping([typeof(ExtendedList<>), typeof(ILeveledSpellEntryGetter)], false, typeof(LeveledSpellAction));
            addMapping([typeof(ExtendedList<>), typeof(IRankPlacementGetter)], false, typeof(RankPlacementAction));
            addMapping([typeof(ExtendedList<>), typeof(IRelationGetter)], false, typeof(RelationsAction));
        }

        public override string GameName => "Skyrim";

        public override Type[] IgnoreMajorRecordGetterTypes
            => [
                typeof(IStoryManagerBranchNodeGetter),
                typeof(IStoryManagerEventNodeGetter),
                typeof(IStoryManagerQuestNodeGetter),
                typeof(IHairGetter), // No implemented fields
                typeof(ILensFlareGetter), // No implemented fields
                typeof(INavigationMeshInfoMapGetter), // No implemented fields
                typeof(IVolumetricLightingGetter), // No implemented fields
                typeof(IDebrisGetter), // No implemented fields
                typeof(IDefaultObjectManagerGetter), // No implemented fields
                typeof(IGlobalGetter), // No implemented fields
                typeof(IGameSettingGetter), // No implemented fields
                typeof(IImpactDataSetGetter), // No implemented fields
                typeof(IVoiceTypeGetter), // No implemented fields
                ];
    }
}