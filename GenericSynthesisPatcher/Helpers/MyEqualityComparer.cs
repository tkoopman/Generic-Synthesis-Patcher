using GenericSynthesisPatcher.Games.Universal.Json.Operations;

namespace GenericSynthesisPatcher.Helpers
{
    public static class MyEqualityComparer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals<T> (ListOperationBase<T> x, T? y)
        {
            if (x == null)
                return y == null;

            return x.ValueEquals(y);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals<T1, T2> (T1? x, T2? y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (x is IEquatable<T2> xx)
                return xx.Equals(y);

            if (y is IEquatable<T1> yy)
                return yy.Equals(x);

            return x.Equals(y);
        }
    }
}