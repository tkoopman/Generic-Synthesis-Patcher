using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class ObjectBoundsAction : BasicGetterSetterAction<IObjectBoundsGetter, ObjectBounds>
    {
        public static readonly ObjectBoundsAction Instance = new();

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