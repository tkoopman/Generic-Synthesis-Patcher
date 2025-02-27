using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class CellMaxHeightDataAction : BasicGetterSetterAction<ICellMaxHeightDataGetter, CellMaxHeightData>
    {
        public static readonly CellMaxHeightDataAction Instance = new();

        public override bool CanFill () => false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        protected override bool compareValues (ICellMaxHeightDataGetter? lhs, CellMaxHeightData? rhs)
        {
            if (lhs == null && rhs == null)
                return true;

            if (lhs == null || rhs == null)
                return false;

            return
                lhs.Offset.Equals(rhs.Offset) &&
                lhs.HeightMap.SequenceEqual(rhs.HeightMap);
        }

        protected override CellMaxHeightData? getSetter (ICellMaxHeightDataGetter? getter)
            => getter == null
            ? null
            : new CellMaxHeightData
            {
                Offset = getter.Offset,
                HeightMap = new Array2d<byte>(getter.HeightMap)
            };
    }
}