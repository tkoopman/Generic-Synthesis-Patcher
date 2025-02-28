using System.Diagnostics.CodeAnalysis;

using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class ObjectBoundsAction : BasicGetterSetterAction<IObjectBoundsGetter, ObjectBounds>
    {
        public static readonly ObjectBoundsAction Instance = new();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]";
            example = $"""
                       "{propertyName}": [6,6,6,9,9,9]
                       """;

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        protected override bool compareValues (IObjectBoundsGetter? lhs, ObjectBounds? rhs)
        {
            if (lhs == null && rhs == null)
                return true;

            if (lhs == null || rhs == null)
                return false;

            return
                lhs.First.Equals(rhs.First) &&
                lhs.Second.Equals(rhs.Second);
        }

        protected override ObjectBounds? getSetter (IObjectBoundsGetter? getter)
            => getter == null
            ? null
            : new ObjectBounds
            {
                First = getter.First,
                Second = getter.Second
            };
    }
}