using Common;

using GenericSynthesisPatcher.Games.Universal;

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
    }
}