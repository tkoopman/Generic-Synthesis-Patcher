using System.Runtime.CompilerServices;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json;

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

        public static JsonSerializerSettings SerializerSettings { get; } = new() { MissingMemberHandling = MissingMemberHandling.Ignore, ContractResolver = ContractResolver.Instance };
        public static Lazy<GSPSettings> Settings { get => settings; private set => settings = value; }
        public static IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get => state ?? throw new Exception("Oh boy this shouldn't happen!"); set => state = value; }

        public static LogWriter? TraceLogger { get; private set; } = null;

        public static void Processing ( int classLogCode, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, [CallerLineNumber] int line = 0 )
            => TraceLogger = Settings.Value.TraceFormKey != null && Settings.Value.TraceFormKey == context.Record.FormKey
             ? new LogWriter(LogLevel.Trace, classLogCode, context, line: line)
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