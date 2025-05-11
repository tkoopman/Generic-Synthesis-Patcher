using Common;

using GenericSynthesisPatcher.Games.Oblivion.Action;
using GenericSynthesisPatcher.Games.Universal;

using Mutagen.Bethesda.Oblivion;

using Noggog;

namespace GenericSynthesisPatcher.Games.Oblivion
{
    public class OblivionGenny : Genny
    {
        public OblivionGenny ()
        {
            IgnoreDeepScanOnTypes = IgnoreDeepScanOnTypes.AddRange(
                [
                typeof(Cell),
                typeof(Landscape),
                typeof(Model),
                typeof(RegionGrasses),
                typeof(RegionMap),
                typeof(RegionObjects),
                typeof(RegionSounds),
                typeof(RegionWeather),
                ]);

            addMapping([typeof(ExtendedList<>), typeof(IContainerItemGetter)], false, typeof(ContainerItemsAction));
        }

        public override string GameName => "Oblivion";

        public override Type[] IgnoreMajorRecordGetterTypes
            => [
                typeof(IGlobalGetter), // No implemented fields
                typeof(IGameSettingGetter), // No implemented fields
                typeof(IScriptGetter), // No implemented fields
                typeof(IStaticGetter), // No implemented fields
                ];
    }
}