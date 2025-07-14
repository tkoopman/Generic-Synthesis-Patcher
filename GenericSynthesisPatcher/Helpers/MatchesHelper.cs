using Common;

using GenericSynthesisPatcher.Rules.Operations;

using Microsoft.Extensions.Logging;

namespace GenericSynthesisPatcher.Helpers
{
    public static class MatchesHelper
    {
        private const int ClassLogCode = 0x03;

        public static bool Matches<TValue> (IEnumerable<TValue>? values, FilterLogic logic, IEnumerable<ListOperationBase<TValue>>? matches, bool debug = true) => Matches(values, logic, matches, static (l, r) => MyEqualityComparer.Equals(l, r), debug);

        /// <summary>
        ///     Performs match operation against a list of values. Assumes you have validated pre
        ///     running this.
        /// </summary>
        public static bool Matches<TValue> (IEnumerable<TValue>? values, FilterLogic logic, IEnumerable<ListOperationBase<TValue>>? matches, Func<ListOperationBase<TValue>, TValue?, bool> predicate, bool debug = true)
        {
            if (!matches.SafeAny())
                return true;

            // If no values then if we are to match against any included values it will not match.
            if (!values.SafeAny())
                return !matches.Any(m => m.Operation != ListLogic.NOT);

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

            if (debug)
                Global.Logger.WriteLog(LogLevel.Trace, result ? LogType.MatchSuccess : LogType.MatchFailure, $"Matched: {result} Operation: {logic} Trigger: {matchedOn}", ClassLogCode);

            return result;
        }

        /// <summary>
        ///     Performs match operation against a single value. Assumes you have validated pre
        ///     running this. object.Equals will be used for comparing values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool Matches<TValue> (TValue? value, IEnumerable<ListOperationBase<TValue>>? matches, bool debug = true)
            => Matches(value, matches, static (l, r) => MyEqualityComparer.Equals(l, r), debug);

        /// <summary>
        ///     Performs match operation against a single value. Assumes you have validated pre
        ///     running this.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="matches"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Matches<TValue, TListValue> (TValue? value, IEnumerable<ListOperationBase<TListValue>>? matches, Func<ListOperationBase<TListValue>, TValue?, bool> predicate, bool debug = true)
        {
            if (!matches.SafeAny())
                return true;

            bool hasEntry = matches.Any(m => predicate(m, value));
            bool isNeg = matches.First().Operation == ListLogic.NOT;
            bool result = isNeg ? !hasEntry : hasEntry;

            if (debug)
                Global.Logger.WriteLog(LogLevel.Trace, result ? LogType.MatchSuccess : LogType.MatchFailure, $"Matched: {result} Found: {hasEntry} Not: {isNeg}", ClassLogCode);

            return result;
        }

        /// <summary>
        ///     Validates matches based on matching a list of values field.
        /// </summary>
        /// <returns>True if valid</returns>
        public static bool Validate<T> (FilterLogic logic, IEnumerable<ListOperationBase<T>>? matches)
        {
            if (logic != FilterLogic.AND && matches.SafeAny() && matches.Any(m => m.Operation == ListLogic.NOT) && matches.Any(m => m.Operation != ListLogic.NOT))
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.GeneralConfigFailure, "Includes both include and exclude values to match against, which does not compute for matching against a list of values unless using AND operation.", ClassLogCode);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validates matches based on matching a single value field.
        /// </summary>
        /// <returns>True if valid</returns>
        public static bool Validate<T> (IEnumerable<ListOperationBase<T>>? matches)
        {
            if (matches.SafeAny() && matches.Any(m => m.Operation == ListLogic.NOT) && matches.Any(m => m.Operation != ListLogic.NOT))
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.GeneralConfigFailure, "Includes both include and exclude values to match against, which does not compute for matching against a single value.", ClassLogCode);
                return false;
            }

            return true;
        }
    }
}