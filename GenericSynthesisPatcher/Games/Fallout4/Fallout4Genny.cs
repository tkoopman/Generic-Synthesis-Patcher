using Common;

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

                ]);
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
                ];
    }
}