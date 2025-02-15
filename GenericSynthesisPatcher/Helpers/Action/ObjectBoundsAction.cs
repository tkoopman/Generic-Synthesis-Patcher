using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class ObjectBoundsAction : BasicGetterSetterAction<IObjectBoundsGetter, ObjectBounds>
    {
        public static readonly ObjectBoundsAction Instance = new();

        protected override bool CompareValues (IObjectBoundsGetter? lhs, ObjectBounds? rhs)
        {
            if (lhs == null && rhs == null)
                return true;

            if (lhs == null || rhs == null)
                return false;

            return
                lhs.First.Equals(rhs.First) &&
                lhs.Second.Equals(rhs.Second);
        }

        protected override ObjectBounds? GetSetter (IObjectBoundsGetter? getter)
        {
            if (getter == null)
                return null;

            return new ObjectBounds
            {
                First = getter.First,
                Second = getter.Second
            };
        }
    }
}