using System.Runtime.CompilerServices;

using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Games.Universal.Json.Data;
using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher
{
    internal static class Global
    {
        internal static Lazy<GSPSettings> settings = null!;

        public static LoadOrder<IModListingGetter> LoadOrder { get; private set; } = null!;
        public static RecordPropertyMappings RecordPropertyMappings { get; private set; } = null!;
        public static RecordTypeMappings RecordTypeMappings { get; private set; } = null!;
        public static JsonSerializerSettings SerializerSettings { get; private set; } = null!;
        public static Lazy<GSPSettings> Settings { get => settings; private set => settings = value; }
        public static IPatcherState State { get; private set; } = null!;

        public static void ForceTrace (int classLogCode, GSPBase? rule, IModContext<IMajorRecordGetter> context, [CallerLineNumber] int line = 0)
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

        public static void Processing (int classLogCode, GSPBase? rule, IModContext<IMajorRecordGetter>? context, [CallerLineNumber] int line = 0)
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

        public static void SetState (IPatcherState state)
        {
            State = state;

            //TODO Add games
            switch (State)
            {
                case IPatcherState<ISkyrimMod, ISkyrimModGetter> skyrim:
                    RecordTypeMappings = new Games.Skyrim.RecordTypeMappings(skyrim);
                    RecordPropertyMappings = new Games.Skyrim.RecordPropertyMappings();

                    SerializerSettings = new()
                    {
                        ContractResolver = Games.Skyrim.Json.ContractResolver.Instance,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                    };

                    LoadOrder = new(skyrim.LoadOrder.Select(m => (IModListingGetter)m.Value));
                    break;

                default:
                    throw new InvalidCastException();
            }
        }

        #region Log Writers

        public static LogWriter? DebugLogger { get; private set; }
        public static LogWriter Logger { get; private set; } = new LogWriter(LogLevel.Information, 0x00, rule: null, context: null);
        public static LogWriter? TraceLogger { get; private set; }

        #endregion Log Writers

        public static void UpdateLoggers (int classLogCode, [CallerLineNumber] int line = 0)
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