using Common;

using GenericSynthesisPatcher.Games.Universal;

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
    }
}