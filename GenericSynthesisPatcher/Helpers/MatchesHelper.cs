using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

namespace GenericSynthesisPatcher.Helpers
{
    public static class MatchesHelper
    {
        private const int ClassLogCode = 0x07;

        public static bool Matches<TValue> (IEnumerable<TValue>? values, FilterLogic logic, IEnumerable<ListOperationBase<TValue>>? matches, string? propertyName = null, bool debugSuccess = true, bool debugFailure = true) => Matches(values, logic, matches, static (l, r) => MyEqualityComparer.Equals(l, r), propertyName, debugSuccess, debugFailure);

        /// <summary>
        ///     Performs match operation against a list of values. Assumes you have validated pre
        ///     running this.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="matches"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Matches<TValue> (IEnumerable<TValue>? values, FilterLogic logic, IEnumerable<ListOperationBase<TValue>>? matches, Func<ListOperationBase<TValue>, TValue?, bool> predicate, string? propertyName = null, bool debugSuccess = true, bool debugFailure = true)
        {
            if (!matches.SafeAny())
                return true;

            // If no values then if we are to match against any included values it will not match.
            if (!values.SafeAny())
                return matches.Any(m => m.Operation != ListLogic.NOT);

            int matchedCount = 0;
            int countIncludes = 0;

            bool result = false;
            bool loopFinished = true;
            string matchedOn = "";

            foreach (var m in matches)
            {
                loopFinished = false;
                if (m.Operation != ListLogic.NOT)
                    countIncludes++;

                if (values.Any(value => predicate(m, value)))
                {
                    if (logic == FilterLogic.OR)
                    {
                        result = m.Operation != ListLogic.NOT;
                        matchedOn = m.ToString('!');
                        break;
                    }

                    if (logic == FilterLogic.AND && m.Operation == ListLogic.NOT)
                    {
                        matchedOn = m.ToString('!');
                        break;
                    }

                    matchedCount++;
                }
                else if (logic == FilterLogic.AND && m.Operation != ListLogic.NOT)
                {
                    matchedOn = m.ToString('!');
                    break;
                }

                loopFinished = true;
            }

            if (loopFinished)
            {
                result = logic switch
                {
                    FilterLogic.AND => matchedCount == countIncludes, // Any failed matches returned above already so as long as all included matches matched we good.
                    FilterLogic.XOR => matchedCount == 1,
                    _ => countIncludes == 0 // OR - Any matches would of returned results above, so now only fail if any check was that it had to match a entry.
                };
            }

            if ((debugFailure && !result) || (debugSuccess && result))
                Global.TraceLogger?.Log(ClassLogCode, $"Matched: {result} Operation: {logic} Trigger: {matchedOn}", propertyName: propertyName);

            return result;
        }

        /// <summary>
        ///     Performs match operation against a single value. Assumes you have validated pre
        ///     running this. object.Equals will be used for comparing values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool Matches<TValue> (TValue? value, IEnumerable<ListOperationBase<TValue>>? matches, string? propertyName = null, bool debugSuccess = true, bool debugFailure = true)
            => Matches(value, matches, static (l, r) => MyEqualityComparer.Equals(l, r), propertyName, debugSuccess, debugFailure);

        /// <summary>
        ///     Performs match operation against a single value. Assumes you have validated pre
        ///     running this.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="matches"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Matches<TValue, TListValue> (TValue? value, IEnumerable<ListOperationBase<TListValue>>? matches, Func<ListOperationBase<TListValue>, TValue?, bool> predicate, string? propertyName = null, bool debugSuccess = true, bool debugFailure = true)
        {
            if (!matches.SafeAny())
                return true;

            bool hasEntry = matches.Any(m => predicate(m, value));
            bool isNeg = matches.First().Operation == ListLogic.NOT;
            bool result = isNeg ? !hasEntry : hasEntry;

            if ((debugFailure && !result) || (debugSuccess && result))
                Global.TraceLogger?.Log(ClassLogCode, $"Matched: {result} Found: {hasEntry} Not: {isNeg}", propertyName: propertyName);

            return result;
        }

        /// <summary>
        ///     Validates matches based on matching a list of values field.
        /// </summary>
        /// <returns>True if valid</returns>
        public static bool Validate<T> (FilterLogic logic, IEnumerable<ListOperationBase<T>>? matches, string? propertyName = null)
        {
            if (logic != FilterLogic.AND && matches.SafeAny() && matches.Any(m => m.Operation == ListLogic.NOT) && matches.Any(m => m.Operation != ListLogic.NOT))
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, $"Includes both include and exclude values to match against, which does not compute for matching against a list of values unless using AND operation.", propertyName: propertyName);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validates matches based on matching a single value field.
        /// </summary>
        /// <returns>True if valid</returns>
        public static bool Validate<T> (IEnumerable<ListOperationBase<T>>? matches, string? propertyName = null)
        {
            if (matches.SafeAny() && matches.Any(m => m.Operation == ListLogic.NOT) && matches.Any(m => m.Operation != ListLogic.NOT))
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, $"Includes both include and exclude values to match against, which does not compute for matching against a single value.", propertyName: propertyName);
                return false;
            }

            return true;
        }
    }
}