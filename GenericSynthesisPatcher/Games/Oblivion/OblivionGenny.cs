using Common;

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

                ]);
        }

        public override string GameName => "Oblivion";

        public override Type[] IgnoreMajorRecordGetterTypes
            => [
                typeof(IGlobalGetter), // No implemented fields
                typeof(IGameSettingGetter), // No implemented fields
                typeof(ILandTextureGetter), // No implemented fields
                typeof(ISubspaceGetter), // No implemented fields
                typeof(IScriptGetter), // No implemented fields
                typeof(ISoundGetter), // No implemented fields
                ];
    }
}