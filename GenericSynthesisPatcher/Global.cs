using System.Runtime.CompilerServices;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json;
using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher
{
    internal static class Global
    {
        internal static Lazy<GSPSettings> settings = null!;
        private static IPatcherState<ISkyrimMod, ISkyrimModGetter>? state;

        public static JsonSerializerSettings SerializerSettings { get; } = new()
        {
            ContractResolver = ContractResolver.Instance,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static Lazy<GSPSettings> Settings { get => settings; private set => settings = value; }
        public static IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get => state ?? throw new Exception("Oh boy this shouldn't happen!"); set => state = value; }

        #region Log Writers

        public static LogWriter? DebugLogger { get; private set; } = null;
        public static LogWriter Logger { get; private set; } = new LogWriter(LogLevel.Information, 0x00, rule: null, context: null);
        public static LogWriter? TraceLogger { get; private set; } = null;

        #endregion Log Writers

        public static void ForceTrace ( int classLogCode, GSPBase? rule, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, [CallerLineNumber] int line = 0 )
        {
            if (TraceLogger == null)
            {
                TraceLogger = new LogWriter(LogLevel.Trace, classLogCode, rule, context, line: line);
            }
            else
            {
                TraceLogger.ClassLogCode = classLogCode;
                TraceLogger.Line = line;
            }
        }

        public static void Processing ( int classLogCode, GSPBase? rule, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>? context, [CallerLineNumber] int line = 0 )
        {
            TraceLogger = Settings.Value.Logging.LogLevel == LogLevel.Trace
                                  && ((rule != null && rule.Debug)
                                  || (context != null && Settings.Value.Logging.FormKey == context.Record.FormKey)
                                  || Settings.Value.Logging.All)
                     ? new LogWriter(LogLevel.Trace, classLogCode, rule, context, line: line)
                     : null;

            DebugLogger = Settings.Value.Logging.LogLevel <= LogLevel.Debug
                                  && ((rule != null && rule.Debug)
                                  || (context != null && Settings.Value.Logging.FormKey == context.Record.FormKey)
                                  || Settings.Value.Logging.All)
                     ? new LogWriter(LogLevel.Debug, classLogCode, rule, context, line: line)
                     : null;

            Logger.ClassLogCode = classLogCode;
            Logger.Context = context;
            Logger.Rule = rule;
            Logger.Line = line;
        }

        public static void UpdateLoggers ( int classLogCode, [CallerLineNumber] int line = 0 )
        {
            if (TraceLogger != null)
            {
                TraceLogger.ClassLogCode = classLogCode;
                TraceLogger.Line = line;
            }

            if (DebugLogger != null)
            {
                DebugLogger.ClassLogCode = classLogCode;
                DebugLogger.Line = line;
            }

            if (Logger != null)
            {
                Logger.ClassLogCode = classLogCode;
                Logger.Line = line;
            }
        }
    }
}