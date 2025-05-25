using Common;

using GenericSynthesisPatcher.Games.Fallout4.Action;
using GenericSynthesisPatcher.Games.Universal;

using Mutagen.Bethesda.Fallout4;

using Noggog;

namespace GenericSynthesisPatcher.Games.Fallout4
{
    public class Fallout4Genny : Genny
    {
        public Fallout4Genny ()
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

            addMapping(typeof(IObjectBoundsGetter), false, typeof(ObjectBoundsAction));

            addMapping([typeof(ExtendedList<>), typeof(IContainerEntryGetter)], false, typeof(ContainerItemsAction));
            addMapping([typeof(ExtendedList<>), typeof(IEffectGetter)], false, typeof(EffectsAction));
        }

        public override string GameName => "Fallout4";

        public override Type[] IgnoreMajorRecordGetterTypes
            => [
                typeof(IStoryManagerBranchNodeGetter),
                typeof(IStoryManagerEventNodeGetter),
                typeof(IStoryManagerQuestNodeGetter),
                typeof(IAudioEffectChainGetter), // No implemented fields
                typeof(IDebrisGetter), // No implemented fields
                typeof(IADamageTypeGetter), // No implemented fields
                typeof(IDefaultObjectManagerGetter), // No implemented fields
                typeof(IGlobalGetter), // No implemented fields
                typeof(IGameSettingGetter), // No implemented fields
                typeof(IInstanceNamingRuleGetter), // No implemented fields
                typeof(IImpactDataSetGetter), // No implemented fields
                typeof(INavigationMapInfoGetter), // No implemented fields
                typeof(INavigationMeshObstacleManagerGetter), // No implemented fields
                typeof(IObjectVisibilityManagerGetter), // No implemented fields
                typeof(IAnimationSoundTagSetGetter), // No implemented fields
                typeof(IVoiceTypeGetter), // No implemented fields
                typeof(IBodyPartDataGetter), // No implemented fields
                ];
    }
}