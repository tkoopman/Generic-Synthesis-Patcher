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

        public static void PrintCounts ()
        {
            for (int i = (int)LogLevel.Warning; i <= (int)LogLevel.Critical; i++)
            {
                if (Count[i] > 0)
                    Console.WriteLine($"{(LogLevel)i}: {Count[i]:N0}");
            }
        }

        public static void WriteLog ( LogLevel logLevel, int classCode, string log, GSPBase? rule = null, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>? context = null, IMajorRecordGetter? record = null, string? propertyName = null, [CallerLineNumber] int line = 0 )
        {
            if (logLevel < Global.Settings.Value.Logging.LogLevel)
                return;

            Count[(int)logLevel]++;

            var sb = new StringBuilder(Enum.GetName(logLevel));
            _ = sb.Append($" [#{classCode:X2}{line:X3}]");
            _ = sb.Append(Divider);

            if (rule != null)
            {
                _ = sb.Append($"{rule.ConfigFile}.");
                if (rule is GSPRule gspRule && gspRule.Group != null)
                    _ = sb.Append($"{gspRule.Group.ConfigRule}.");
                _ = sb.Append($"{rule.ConfigRule}");
                _ = sb.Append(Divider);
            }

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
    }
}