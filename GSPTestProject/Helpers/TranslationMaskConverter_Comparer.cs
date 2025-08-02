using System.Diagnostics.CodeAnalysis;

using Loqui;

namespace GSPTestProject.Helpers
{
    internal class TranslationMaskConverter_Comparer : IEqualityComparer<ITranslationMask?>
    {
        public bool Equals (ITranslationMask? x, ITranslationMask? y)
        {
            if (x is null && y is null)
                return true;
            if (x is null || y is null)
                return false;
            if (x.GetType() != y.GetType())
                return false;

            // Compare properties of the masks
            foreach (var field in x.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (field.FieldType == typeof(bool))
                {
                    if (!Equals(field.GetValue(x), field.GetValue(y)))
                        return false;

                    continue;
                }

                if (!field.FieldType.IsAssignableTo(typeof(ITranslationMask)))
                    continue;

                var xValue = field.GetValue(x) as ITranslationMask;
                var yValue = field.GetValue(y) as ITranslationMask;
                if (!Equals(xValue, yValue))
                    return false;
            }

            return true;
        }

        public int GetHashCode ([DisallowNull] ITranslationMask obj) => throw new NotImplementedException();
    }
}