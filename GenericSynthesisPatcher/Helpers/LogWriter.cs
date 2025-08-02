using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

using GenericSynthesisPatcher.Rules;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Helpers
{
    public class LogWriter : TextWriter
    {
        public const string PropertyIsEqual = "Skipping as already matches";
        public const string RecordUpdated = "Updated";
        public const string RecordUpdatedChanges = "change(s)";
        private const string Divider = ": ";

        private readonly StringBuilder _log = new();

        private readonly uint[] Count = new uint[7];
        private TextWriter? _out;

        private string? logRecordDetails;
        private string? logRuleDetails;

        public LogWriter ()
        {
            NewLine = Environment.NewLine;
            CurrentLogLevel = calculateCurrentLogLevel(null, FormKey.Null);
        }

        /// <summary>
        ///     Current log level based on Global settings and current rule and record being
        ///     processed. Use
        ///     <see cref="UpdateCurrentProcess(GSPBase?, IModContext{IMajorRecordGetter}?, int, int)" />
        ///     to update."/&gt;
        /// </summary>
        public LogLevel CurrentLogLevel { get; private set; }

        public string? CurrentPropertyName { get; set; }

        /// <summary>
        ///     When logging and no ClassLogCode provided, like when using TextWriter.WriteLine(),
        ///     it will use this ClassLogCode. Use the following calls to update this value:
        ///     - <see cref="LogWriter.UpdateCurrentProcess(GSPBase?, IModContext{IMajorRecordGetter}?, int, int)" />
        ///     - <see cref="LogWriter.UpdateDefaultCallingLocation(int, int)" />
        /// </summary>
        public int DefaultCallingClassLogCode { get; set; }

        /// <summary>
        ///     When logging and no CallingLine provided, like when using TextWriter.WriteLine(), it
        ///     will use this line number. Use the following calls to update this value:
        ///     - <see cref="LogWriter.UpdateCurrentProcess(GSPBase?, IModContext{IMajorRecordGetter}?, int, int)" />
        ///     - <see cref="LogWriter.UpdateDefaultCallingLocation(int, int)" />
        /// </summary>
        public int DefaultCallingLine { get; set; }

        /// <summary>
        ///     When logging and no log level provided, like when using TextWriter.WriteLine(), it
        ///     will use this log level.
        /// </summary>
        public LogLevel DefaultLogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        ///     When logging and no log type provided, like when using TextWriter.WriteLine(), it
        ///     will use this log type.
        /// </summary>
        public LogType DefaultLogType { get; private set; } = LogType.OTHER;

        public override Encoding Encoding => Encoding.Default;

        /// <summary>
        ///     Where to write logs to. By default or if set to null! will output to <see cref="Console.Out" />.
        /// </summary>
        public TextWriter Out
        {
            protected get => _out ?? Console.Out;

            set => _out = value;
        }

        protected bool BlockWrittenTo { get; private set; }

        public override void Flush ()
        {
            string log = _log.ToString();
            _ = _log.Clear();

            log = log.TrimEnd(['\r', '\n']);

            if (!string.IsNullOrEmpty(log))
                WriteLog(DefaultLogLevel, DefaultLogType, log, DefaultCallingClassLogCode, line: DefaultCallingLine);
        }

        /// <summary>
        ///     Output summary of the total number of each log severity that has been printed to output.
        /// </summary>
        public string? GetCounts ()
        {
            var sw = new StringWriter();
            for (int i = (int)LogLevel.Warning; i <= (int)LogLevel.Critical; i++)
            {
                if (Count[i] != 0)
                    sw.WriteLine($"{(LogLevel)i}: {Count[i]:N0}");
            }

            string result = sw.ToString();
            return result.IsNullOrWhitespace() ? null : result;
        }

        public bool IsLogLevelEnabled (LogLevel logLevel, LogType logType)
        {
            if (logLevel < CurrentLogLevel)
                return false;

            // We don't filter out logs that are not at least Debug level.
            if (logLevel > LogLevel.Debug)
                return true;

            // Check if log type is enabled based on settings.
            return logType switch
            {
                LogType.RecordFound or LogType.RecordProcessing
                    => Global.Settings.Logging.NoisyLogs.ActionLogs.ActionOther,

                LogType.ACTION
                    => Global.Settings.Logging.NoisyLogs.ActionLogs.ActionCalled,

                LogType.Cache
                    => Global.Settings.Logging.NoisyLogs.Cache,

                LogType.MatchSuccess
                    => Global.Settings.Logging.NoisyLogs.MatchLogs.Matched,

                LogType.MatchFailure
                    => Global.Settings.Logging.NoisyLogs.MatchLogs.NotMatched,

                LogType.RecordUpdated
                    => Global.Settings.Logging.NoisyLogs.ActionLogs.RecordUpdated,

                LogType.NoOverwrites or LogType.RecordProcessSkipped or LogType.NoUpdateAlreadyMatches or LogType.SkippingRule
                    => Global.Settings.Logging.NoisyLogs.ActionLogs.ActionSkipped,

                LogType.OriginMatch
                    => Global.Settings.Logging.NoisyLogs.MatchLogs.Matched
                    && Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeOnlyIfDefault,

                LogType.OriginNotMatch
                    => Global.Settings.Logging.NoisyLogs.MatchLogs.NotMatched
                    && Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeOnlyIfDefault,

                _ => true,
            };
        }

        public void LogAction (string action, int classCode, [CallerLineNumber] int line = 0) => WriteLog(LogLevel.Trace, LogType.ACTION, log: $"Calling {action}", classCode: classCode, line: line);

        public void LogInvalidTypeFound (string expected, string found, int classCode, [CallerLineNumber] int line = 0) => WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, $"Invalid type returned. Expected: {expected}. Found: {found}.", classCode, line: line);

        public void LogMissingProperty (string propertyName, int classCode, [CallerLineNumber] int line = 0) => WriteLog(LogLevel.Error, LogType.PropertyNotExist, $"Property missing or no valid action found for: {propertyName}.", classCode, line: line);

        public void StartNewBlock ()
        {
            if (BlockWrittenTo)
                Out.WriteLine();

            BlockWrittenTo = false;
        }

        /// <summary>
        ///     Call to set current default values and log level for the current record and rule
        ///     being processed
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="context"></param>
        /// <param name="classLogCode"></param>
        /// <param name="line">Exclude to auto fill with line number that called this method.</param>
        public void UpdateCurrentProcess (GSPBase? rule, IModContext<IMajorRecordGetter>? context, int classLogCode, [CallerLineNumber] int line = 0)
        {
            UpdateDefaultCallingLocation(classLogCode, line);

            CurrentLogLevel = calculateCurrentLogLevel(rule, context?.Record.FormKey ?? FormKey.Null);
            logRuleDetails = rule is null ? null : $"{rule.GetLogRuleID()}{Divider}";
            CurrentPropertyName = null;

            if (context is null)
            {
                DefaultLogType = LogType.OTHER;
                logRecordDetails = null;
            }
            else
            {
                DefaultLogType = LogType.RecordProcessing;
                var record = context.Record;

                logRecordDetails = context.ModKey == record.FormKey.ModKey
                    ? $"{record.Registration.Name} {record.FormKey}{Divider}"
                    : $"{record.Registration.Name} {record.FormKey}{Divider}{context.ModKey.FileName}{Divider}";
            }
        }

        /// <summary>
        ///     Update DefaultCallingClassLogCode and DefaultCallingLine.
        /// </summary>
        /// <param name="classLogCode"></param>
        /// <param name="line">Exclude to auto fill with line number that called this method.</param>
        public void UpdateDefaultCallingLocation (int classLogCode, [CallerLineNumber] int line = 0)
        {
            DefaultCallingClassLogCode = classLogCode;
            DefaultCallingLine = line;
        }

        /// <summary>
        ///     Writes log entry with global ClassLogCode, Line, LogType and LogLevel.
        /// </summary>
        /// <param name="value"></param>
        public override void Write (char value)
        {
            if (DefaultLogLevel < CurrentLogLevel)
                return;

            switch (value)
            {
                case '\n':

                    // Time to output a log entry
                    Flush();
                    break;

                default:
                    _ = _log.Append(value);
                    break;
            }
        }

        /// <summary>
        ///     Write a log message to the output with the specified log level and additional context.
        /// </summary>
        /// <param name="logLevel">
        ///     Log level for this log entry. Will only print if greater than or equal to <see cref="Global.Settings.Logging.LogLevel" />
        /// </param>
        /// <param name="classCode">Unique ID for the class that called this method.</param>
        /// <param name="log">Log message</param>
        /// <param name="rule">Rule that the log relates to.</param>
        /// <param name="context">Record context this log relates to.</param>
        /// <param name="record">Record this log relates to.</param>
        /// <param name="propertyName">Property name this log relates to.</param>
        /// <param name="line">Line number that called this. Auto populated by <see cref="CallerLineNumberAttribute" /></param>
        public void WriteLog (LogLevel logLevel, LogType logType, string log, int classCode, string? includePrefix = null, [CallerLineNumber] int line = 0)
        {
            if (!IsLogLevelEnabled(logLevel, logType))
                return;

            classCode = classCode == -1 ? DefaultCallingClassLogCode : classCode;

            Count[(int)logLevel]++;

            var sb = new StringBuilder(Enum.GetName(logLevel));
            _ = sb.Append(CultureInfo.InvariantCulture, $" [#{classCode:X2}{line:X3}]");
            _ = sb.Append(Divider);

            if (includePrefix is not null)
            {
                _ = sb.Append(includePrefix);
                _ = sb.Append(Divider);
            }

            _ = sb.Append(logRuleDetails);
            _ = sb.Append(logRecordDetails);

            if (CurrentPropertyName is not null)
            {
                _ = sb.Append(CurrentPropertyName);
                _ = sb.Append(Divider);
            }

            _ = sb.Append(log);

            BlockWrittenTo = true;
            Out.WriteLine(sb.ToString());
        }

        /// <summary>
        ///     Writes string with blank line as separator at start, but only if string is not empty.
        /// </summary>
        /// <remarks>
        ///     This method writes the string provided as is, without adding standard logging format.
        /// </remarks>
        public void WriteRawBlock (string? str)
        {
            if (str.IsNullOrWhitespace())
                return;

            StartNewBlock();
            Out.WriteLine(str);
            StartNewBlock();
        }

        public void WriteRawLine (string? str)
        {
            BlockWrittenTo = true;
            Out.WriteLine(str);
        }

        internal static LogLevel calculateCurrentLogLevel (GSPBase? rule, FormKey formKey)
            => Global.Settings.Logging.LogLevel <= LogLevel.Debug
               ? Global.Settings.Logging.All
              || (rule is null && formKey.IsNull)
              || (rule is not null && rule.Debug)
              || (!formKey.IsNull && Global.Settings.Logging.FormKey == formKey)
                    ? Global.Settings.Logging.LogLevel
                    : LogLevel.Information
               : Global.Settings.Logging.LogLevel;
    }
}