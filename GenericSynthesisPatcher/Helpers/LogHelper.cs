using System.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Helpers
{
    public static class LogHelper
    {
        public const string MissingProperty = "Unable to find property";
        public const string OriginMismatch = "Skipping as not matching origin";
        public const string PropertyIsEqual = "Skipping as already matches";

        public static void Log ( LogLevel logLevel, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string propertyName, string log, int code ) => Log(logLevel, context, $"{string.Concat(propertyName.Select(static x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ')}: {log}", code);

        public static void Log ( LogLevel logLevel, IMajorRecordGetter record, string propertyName, string log, int code ) => Log(logLevel, record, $"{string.Concat(propertyName.Select(static x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ')}: {log}", code);

        public static void Log ( LogLevel logLevel, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string log, int code )
        {
            if (context.ModKey != context.Record.FormKey.ModKey)
                Log(logLevel, context.Record, $"{context.ModKey.FileName}: {log}", code);
        }

        public static void Log ( LogLevel logLevel, IMajorRecordGetter record, string log, int code ) => Log(logLevel, $"{GetGSPRuleTypeAsString(record)} {record.FormKey}: {log}", code);

        public static void Log ( LogLevel logLevel, string log, int code )
        {
            string codeStr = (code <= 0) ? "" : $" [#{code:X3}]";
            if (logLevel >= Global.Settings.Value.LogLevel)
                Console.WriteLine($"{Enum.GetName(logLevel)}{codeStr}: {log}");
        }

        public static void LogInvalidTypeFound ( LogLevel logLevel, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string propertyName, string expected, string found, int code ) => Log(logLevel, context, propertyName, $"Invalid type returned. Expected: {expected}. Found: {found}.", code);

        public static void LogInvalidTypeFound ( LogLevel logLevel, IMajorRecordGetter record, string propertyName, string expected, string found, int code ) => Log(logLevel, record, propertyName, $"Invalid type returned. Expected: {expected}. Found: {found}.", code);
    }
}