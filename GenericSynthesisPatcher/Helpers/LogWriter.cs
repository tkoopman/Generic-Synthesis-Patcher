using System.Runtime.CompilerServices;
using System.Text;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers
{
    public class LogWriter : TextWriter
    {
        private readonly StringBuilder _log = new();

        public LogWriter (LogLevel logLevel, int classLogCode, GSPBase? rule, IModContext<IMajorRecordGetter>? context, [CallerLineNumber] int line = 0)
        {
            NewLine = "\n";
            LogLevel = logLevel;
            ClassLogCode = classLogCode;
            Context = context;
            Rule = rule;
            Line = line;
        }

        public int ClassLogCode { get; set; }
        public IModContext<IMajorRecordGetter>? Context { get; set; }
        public override Encoding Encoding => Encoding.Default;
        public int Line { get; set; }
        public LogLevel LogLevel { get; set; }
        public GSPBase? Rule { get; set; }

        public override void Flush ()
        {
            string log = _log.ToString();
            _ = _log.Clear();

            if (!string.IsNullOrEmpty(log))
                LogHelper.WriteLog(LogLevel, ClassLogCode, log, rule: Rule, context: Context, line: Line);
        }

        public void Log (int classCode, string log, LogLevel logLevel = LogLevel.None, string? propertyName = null, [CallerLineNumber] int line = 0)
                    => LogHelper.WriteLog((logLevel == LogLevel.None) ? LogLevel : logLevel, classCode, log, rule: Rule, context: Context, propertyName: propertyName, line: line);

        public void LogAction (int classCode, string action, LogLevel logLevel = LogLevel.None, string? propertyName = null, [CallerLineNumber] int line = 0)
        {
            if (Global.Settings.Value.Logging.NoisyLogs.CallingAction)
                LogHelper.WriteLog((logLevel == LogLevel.None) ? LogLevel : logLevel, classCode, log: $"Calling {action}", rule: Rule, context: Context, propertyName: propertyName, line: line);
        }

        public void LogInvalidTypeFound (int classCode, string propertyName, string expected, string found, [CallerLineNumber] int line = 0) => Log(classCode, $"Invalid type returned. Expected: {expected}. Found: {found}.", propertyName: propertyName, line: line);

        public override void Write (char value)
        {
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
    }
}