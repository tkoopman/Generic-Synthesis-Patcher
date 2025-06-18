using System.Runtime.CompilerServices;

using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Games.Universal.Json.Data;
using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace GenericSynthesisPatcher
{
    internal static class Global
    {
        internal static Lazy<GSPSettings> settings = null!;

        /// <summary>
        ///     Global Game instance that contains all game specific information.
        /// </summary>
        public static BaseGame Game { get; private set; } = null!;

        public static Lazy<GSPSettings> Settings { get => settings; private set => settings = value; }

        public static void ForceTrace (int classLogCode, GSPBase? rule, IModContext<IMajorRecordGetter> context, [CallerLineNumber] int line = 0)
        {
            if (TraceLogger is null)
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
                                  && ((rule is not null && rule.Debug)
                                  || (context is not null && Settings.Value.Logging.FormKey == context.Record.FormKey)
                                  || Settings.Value.Logging.All)
                     ? new LogWriter(LogLevel.Trace, classLogCode, rule, context, line: line)
                     : null;

            DebugLogger = Settings.Value.Logging.LogLevel <= LogLevel.Debug
                                  && ((rule is not null && rule.Debug)
                                  || (context is not null && Settings.Value.Logging.FormKey == context.Record.FormKey)
                                  || Settings.Value.Logging.All)
                     ? new LogWriter(LogLevel.Debug, classLogCode, rule, context, line: line)
                     : null;

            Logger.ClassLogCode = classLogCode;
            Logger.Context = context;
            Logger.Rule = rule;
            Logger.Line = line;
        }

        //TODO Add games
        public static void SetState (IPatcherState state)
            => Game = state switch
            {
                IPatcherState<ISkyrimMod, ISkyrimModGetter> gameState => Games.Skyrim.SkyrimGame.Constructor(gameState),
                IPatcherState<IFallout4Mod, IFallout4ModGetter> gameState => Games.Fallout4.Fallout4Game.Constructor(gameState),
                IPatcherState<IOblivionMod, IOblivionModGetter> gameState => Games.Oblivion.OblivionGame.Constructor(gameState),
                _ => throw new InvalidCastException(),
            };

        #region Log Writers

        public static LogWriter? DebugLogger { get; private set; }
        public static LogWriter Logger { get; private set; } = new LogWriter(LogLevel.Information, 0x00, rule: null, context: null);
        public static LogWriter? TraceLogger { get; private set; }

        #endregion Log Writers

        public static void UpdateLoggers (int classLogCode, [CallerLineNumber] int line = 0)
        {
            if (TraceLogger is not null)
            {
                TraceLogger.ClassLogCode = classLogCode;
                TraceLogger.Line = line;
            }

            if (DebugLogger is not null)
            {
                DebugLogger.ClassLogCode = classLogCode;
                DebugLogger.Line = line;
            }

            if (Logger is not null)
            {
                Logger.ClassLogCode = classLogCode;
                Logger.Line = line;
            }
        }
    }
}