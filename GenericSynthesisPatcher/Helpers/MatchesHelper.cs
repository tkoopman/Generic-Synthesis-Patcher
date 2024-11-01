using System.Diagnostics;

using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

namespace GenericSynthesisPatcher.Helpers
{
    public static class MatchesHelper
    {
        private const int ClassLogCode = 0x07;
        public static Stopwatch Stopwatch { get; } = new();

        public static bool Matches<TValue> (IEnumerable<TValue>? values, FilterLogic logic, IEnumerable<ListOperationBase<TValue>>? matches, string debugPrefix = "") => Matches(values, logic, matches, static (l, r) => MyEqualityComparer.Equals(l, r), debugPrefix);

        /// <summary>
        /// Performs match operation against a list of values. Assumes you have validated pre running this.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="matches"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Matches<TValue> (IEnumerable<TValue>? values, FilterLogic logic, IEnumerable<ListOperationBase<TValue>>? matches, Func<ListOperationBase<TValue>, TValue?, bool> predicate, string debugPrefix = "")
        {
            try
            {
                Stopwatch.Start();
                if (!matches.SafeAny())
                    return true;

                // If no values then if we are to match against any included values it will not match.
                if (!values.SafeAny())
                    return matches.Any(m => m.Operation != ListLogic.NOT);

                int matchedCount = 0;
                int countIncludes = 0;

                foreach (var m in matches)
                {
                    if (m.Operation != ListLogic.NOT)
                        countIncludes++;

                    if (values.Any(value => predicate(m, value)))
                    {
                        if (logic == FilterLogic.OR)
                        {
                            Global.TraceLogger?.Log(ClassLogCode, $"{debugPrefix} Matched: {m.Operation != ListLogic.NOT}. Matched: {m.ToString('!')}");
                            return m.Operation != ListLogic.NOT;
                        }

                        if (logic == FilterLogic.AND && m.Operation == ListLogic.NOT)
                        {
                            Global.TraceLogger?.Log(ClassLogCode, $"{debugPrefix} Matched: False. Matched {m.ToString('!')}");
                            return false;
                        }

                        matchedCount++;
                    }
                    else if (logic == FilterLogic.AND && m.Operation != ListLogic.NOT)
                    {
                        Global.TraceLogger?.Log(ClassLogCode, $"{debugPrefix} Matched: False. Failed to match {m.Value}");
                        return false;
                    }
                }

                bool result = logic switch
                {
                    FilterLogic.AND => matchedCount == countIncludes, // Any failed matches returned above already so as long as all included matches matched we good.
                    FilterLogic.XOR => matchedCount == 1,
                    _ => countIncludes == 0 // OR - Any matches would of returned results above, so now only fail if any check was that it had to match a entry.
                };

                Global.TraceLogger?.Log(ClassLogCode, $"Matched PatchedBy: {result} Operation: {logic} Matched: {matchedCount}/{matches?.Count()} All Excludes: {countIncludes == 0}");

                return result;
            }
            finally
            {
                Stopwatch.Stop();
            }
        }

        /// <summary>
        /// Performs match operation against a single value. Assumes you have validated pre running this.
        /// object.Equals(TValue, Tlop) will be used for comparing values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool Matches<TValue> (TValue? value, IEnumerable<ListOperationBase<TValue>>? matches, string debugPrefix = "") => Matches(value, matches, static (l, r) => MyEqualityComparer.Equals(l, r), debugPrefix);

        /// <summary>
        /// Performs match operation against a single value. Assumes you have validated pre running this.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="matches"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Matches<TValue, Tlop> (TValue? value, IEnumerable<ListOperationBase<Tlop>>? matches, Func<ListOperationBase<Tlop>, TValue?, bool> predicate, string debugPrefix = "")
        {
            try
            {
                Stopwatch.Start();
                if (!matches.SafeAny())
                    return true;

                bool hasEntry = matches.Any(m => predicate(m, value));
                bool isNeg = matches.First().Operation == ListLogic.NOT;
                bool result = isNeg ? !hasEntry : hasEntry;

                Global.TraceLogger?.Log(ClassLogCode, $"{debugPrefix}Matched: {result} Found: {hasEntry} Not: {isNeg}");

                return result;
            }
            finally
            {
                Stopwatch.Stop();
            }
        }

        /// <summary>
        /// Validates matches based on matching a list of values field.
        /// </summary>
        /// <returns>True if valid</returns>
        public static bool Validate<T> (FilterLogic logic, IEnumerable<ListOperationBase<T>>? matches, string debugPrefix = "")
        {
            if (logic != FilterLogic.AND && matches.SafeAny() && matches.Any(m => m.Operation == ListLogic.NOT) && matches.Any(m => m.Operation != ListLogic.NOT))
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, $"{debugPrefix}Includes both include and exclude values to match against, which does not compute for matching against a list of values unless using AND operation.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates matches based on matching a single value field.
        /// </summary>
        /// <returns>True if valid</returns>
        public static bool Validate<T> (IEnumerable<ListOperationBase<T>>? matches, string debugPrefix = "")
        {
            if (matches.SafeAny() && matches.Any(m => m.Operation == ListLogic.NOT) && matches.Any(m => m.Operation != ListLogic.NOT))
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, $"{debugPrefix}Includes both include and exclude values to match against, which does not compute for matching against a single value.");
                return false;
            }

            return true;
        }
    }
}