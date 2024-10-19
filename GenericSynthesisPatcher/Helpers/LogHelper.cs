using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers
{
    public static partial class LogHelper
    {
        public const string MissingProperty = "Unable to find property";
        public const string OriginMismatch = "Skipping as not matching origin";
        public const string PropertyIsEqual = "Skipping as already matches";

        public static string AddSpaces ( string input ) => SpaceUpper().Replace(input, m => $"{m} ");

        public static void Log ( LogLevel logLevel, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string propertyName, string log, int code ) => Log(logLevel, context, $"{AddSpaces(propertyName)}: {log}", code);

        public static void Log ( LogLevel logLevel, IMajorRecordGetter record, string propertyName, string log, int code ) => Log(logLevel, record, $"{AddSpaces(propertyName)}: {log}", code);

        public static void Log ( LogLevel logLevel, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string log, int code )
        {
            if (context.ModKey != context.Record.FormKey.ModKey)
                Log(logLevel, context.Record, $"{context.ModKey.FileName}: {log}", code);
        }

        public static void Log ( LogLevel logLevel, IMajorRecordGetter record, string log, int code ) => Log(logLevel, $"{GSPBase.GetGSPRuleTypeAsString(record)} {record.FormKey}: {log}", code);

        public static void Log ( LogLevel logLevel, string log, int code )
        {
            string codeStr = (code <= 0) ? "" : $" [#{code:X3}]";
            if (logLevel >= Global.Settings.Value.LogLevel)
                Console.WriteLine($"{Enum.GetName(logLevel)}{codeStr}: {log}");
        }

        public static void LogInvalidTypeFound ( LogLevel logLevel, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string propertyName, string expected, string found, int code ) => Log(logLevel, context, propertyName, $"Invalid type returned. Expected: {expected}. Found: {found}.", code);

        public static void LogInvalidTypeFound ( LogLevel logLevel, IMajorRecordGetter record, string propertyName, string expected, string found, int code ) => Log(logLevel, record, propertyName, $"Invalid type returned. Expected: {expected}. Found: {found}.", code);

        [GeneratedRegex(@"([A-Z])(?=[A-Z][a-z])|([a-z])(?=[A-Z])")]
        private static partial Regex SpaceUpper ();
    }
}