using System.Runtime.CompilerServices;
using System.Text;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers
{
    public static class LogHelper
    {
        public const string MissingProperty = "Unable to find property";
        public const string OriginMismatch = "Skipping as not matching origin";
        public const string PropertyIsEqual = "Skipping as already matches";
        private static readonly uint[] Count = new uint[7];
        private static readonly char[] Divider = [':', ' '];

        public static void Log ( LogLevel logLevel, int classCode, string log, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>? context = null, IMajorRecordGetter? record = null, string? propertyName = null, [CallerLineNumber] int line = 0 )
        {
            if (logLevel < Global.Settings.Value.LogLevel)
                return;

            Count[(int)logLevel]++;

            var sb = new StringBuilder(Enum.GetName(logLevel));
            _ = sb.Append($" [#{classCode:X2}{line:X3}]");
            _ = sb.Append(Divider);

            record ??= context?.Record;
            if (record != null)
            {
                _ = sb.Append(GSPBase.GetGSPRuleTypeAsString(record));
                _ = sb.Append(' ');
                _ = sb.Append(record.FormKey);
                _ = sb.Append(Divider);
            }

            if (context != null && (record == null || context.ModKey != record.FormKey.ModKey))
            {
                _ = sb.Append(context.ModKey.FileName);
                _ = sb.Append(Divider);
            }

            if (propertyName != null)
            {
                _ = sb.Append(propertyName);
                _ = sb.Append(Divider);
            }

            _ = sb.Append(log);

            Console.WriteLine(sb.ToString());
        }

        public static void LogInvalidTypeFound ( LogLevel logLevel, int classCode, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string propertyName, string expected, string found, [CallerLineNumber] int line = 0 ) => Log(logLevel, classCode, $"Invalid type returned. Expected: {expected}. Found: {found}.", context: context, propertyName: propertyName, line: line);

        public static void LogInvalidTypeFound ( LogLevel logLevel, int classCode, IMajorRecordGetter record, string propertyName, string expected, string found, [CallerLineNumber] int line = 0 ) => Log(logLevel, classCode, $"Invalid type returned. Expected: {expected}. Found: {found}.", record: record, propertyName: propertyName, line: line);

        public static void PrintCounts ()
        {
            for (var i = (int)LogLevel.Warning; i <= (int)LogLevel.Critical; i++)
            {
                if (Count[i] > 0)
                    Console.WriteLine($"{(LogLevel)i}: {Count[i]:N0}");
            }
        }
    }
}