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

        public static LogWriter? TraceLogger { get; private set; } = null;

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

        public static void Processing ( int classLogCode, GSPBase? rule, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, [CallerLineNumber] int line = 0 )
            => TraceLogger = Settings.Value.LogLevel == LogLevel.Trace
                          && ((rule != null && rule.Trace)
                          || (Settings.Value.TraceFormKey != null && Settings.Value.TraceFormKey == context.Record.FormKey))
             ? new LogWriter(LogLevel.Trace, classLogCode, rule, context, line: line)
             : null;

        public static bool UpdateTrace ( int classLogCode, [CallerLineNumber] int line = 0 )
        {
            if (TraceLogger != null)
            {
                TraceLogger.ClassLogCode = classLogCode;
                TraceLogger.Line = line;
                return true;
            }

            return false;
        }
    }
}