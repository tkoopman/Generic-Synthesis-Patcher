using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
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

        public LogWriter ( LogLevel logLevel, int classLogCode, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>? context, [CallerLineNumber] int line = 0 )
        {
            NewLine = "\n";
            LogLevel = logLevel;
            ClassLogCode = classLogCode;
            Context = context;
            Line = line;
        }

        public override void Flush ()
        {
            string log = _log.ToString();
            _ = _log.Clear();

            if (!string.IsNullOrEmpty(log))
                LogHelper.Log(LogLevel, ClassLogCode, log, context: Context, line: Line);
        }

        public void Log ( LogLevel logLevel, int classCode, string log, string? propertyName = null, [CallerLineNumber] int line = 0 )
                    => LogHelper.Log(logLevel, classCode, log, context: Context, propertyName: propertyName, line: line);

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