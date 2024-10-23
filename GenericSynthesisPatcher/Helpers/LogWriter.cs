using System.Runtime.CompilerServices;
using System.Text;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers
{
    public class LogWriter : TextWriter
    {
        private readonly StringBuilder _log = new();

        public int ClassLogCode { get; set; }
        public IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>? Context { get; set; }
        public override Encoding Encoding => Encoding.Default;
        public int Line { get; set; }
        public LogLevel LogLevel { get; set; }
        public GSPBase? Rule { get; set; }

        public LogWriter ( LogLevel logLevel, int classLogCode, GSPBase? rule, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>? context, [CallerLineNumber] int line = 0 )
        {
            NewLine = "\n";
            LogLevel = logLevel;
            ClassLogCode = classLogCode;
            Context = context;
            Rule = rule;
            Line = line;
        }

        public override void Flush ()
        {
            string log = _log.ToString();
            _ = _log.Clear();

            if (!string.IsNullOrEmpty(log))
                LogHelper.Log(LogLevel, ClassLogCode, log, rule: Rule, context: Context, line: Line);
        }

        public void Log ( int classCode, string log, string? propertyName = null, [CallerLineNumber] int line = 0 )
                    => LogHelper.Log(LogLevel, classCode, log, rule: Rule, context: Context, propertyName: propertyName, line: line);

        public override void Write ( char value )
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