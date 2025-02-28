using System.Diagnostics.CodeAnalysis;

using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class WorldspaceMaxHeightAction : BasicGetterSetterAction<IWorldspaceMaxHeightGetter, WorldspaceMaxHeight>
    {
        public static readonly WorldspaceMaxHeightAction Instance = new();

        public override bool CanFill () => false;

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Forward Worldspace Max Height data.";
            example = """[{ "types": ["Worldspace"], "ForwardOptions": ["HPU", "NonNull"], "Forward": { "MHDT": [] }}]""";

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        protected override bool compareValues (IWorldspaceMaxHeightGetter? lhs, WorldspaceMaxHeight? rhs)
        {
            if (lhs == null && rhs == null)
                return true;

            if (lhs == null || rhs == null)
                return false;

            return
                lhs.Max.Equals(rhs.Max) &&
                lhs.Min.Equals(rhs.Min) &&
                lhs.CellData.SequenceEqual(rhs.CellData);
        }

        protected override WorldspaceMaxHeight? getSetter (IWorldspaceMaxHeightGetter? getter)
            => getter == null
            ? null
            : new WorldspaceMaxHeight
            {
                Max = getter.Max,
                Min = getter.Min,
                CellData = new([.. getter.CellData])
            };
    }
}