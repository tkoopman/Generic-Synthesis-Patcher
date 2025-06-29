using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

using GenericSynthesisPatcher.Games.Universal.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers
{
    public static class LogHelper
    {
        public const string MissingProperty = "Unable to find property";
        public const string OriginMismatch = "Skipping as not matching origin";
        public const string PropertyIsEqual = "Skipping as already matches";
        private static readonly uint[] Count = new uint[7];
        private static readonly char[] Divider = [':', ' '];
        private static TextWriter? _out;

        /// <summary>
        ///     Where to write logs to. By default or if set to null! will output to <see cref="Console.Out" />.
        /// </summary>
        public static TextWriter Out
        {
            get => _out ?? Console.Out;

            set => _out = value;
        }

        /// <summary>
        ///     Output summary of the total number of each log severity that has been printed to output.
        /// </summary>
        public static void PrintCounts ()
        {
            for (int i = (int)LogLevel.Warning; i <= (int)LogLevel.Critical; i++)
            {
                if (Count[i] > 0)
                    Out.WriteLine($"{(LogLevel)i}: {Count[i]:N0}");
            }
        }

        /// <summary>
        ///     Write a line to the output.
        /// </summary>
        public static void WriteLine (string text = "") => Out.WriteLine(text);

        /// <summary>
        ///     Write a log message to the output with the specified log level and additional context.
        /// </summary>
        /// <param name="logLevel">
        ///     Log level for this log entry. Will only print if greater than or equal to <see cref="Global.Settings.Value.Logging.LogLevel" />
        /// </param>
        /// <param name="classCode">Unique ID for the class that called this method.</param>
        /// <param name="log">Log message</param>
        /// <param name="rule">Rule that the log relates to.</param>
        /// <param name="context">Record context this log relates to.</param>
        /// <param name="record">Record this log relates to.</param>
        /// <param name="propertyName">Property name this log relates to.</param>
        /// <param name="line">Line number that called this. Auto populated by <see cref="CallerLineNumberAttribute" /></param>
        public static void WriteLog (LogLevel logLevel, int classCode, string log, GSPBase? rule = null, IModContext<IMajorRecordGetter>? context = null, IMajorRecordGetter? record = null, string? propertyName = null, [CallerLineNumber] int line = 0)
        {
            if (logLevel < Global.Settings.Value.Logging.LogLevel)
                return;

            Count[(int)logLevel]++;

            var sb = new StringBuilder(Enum.GetName(logLevel));
            _ = sb.Append(CultureInfo.InvariantCulture, $" [#{classCode:X2}{line:X3}]");
            _ = sb.Append(Divider);

            if (rule is not null)
            {
                _ = sb.Append(CultureInfo.InvariantCulture, $"{rule.ConfigFile}.");
                if (rule is GSPRule gspRule && gspRule.Group is not null)
                    _ = sb.Append(CultureInfo.InvariantCulture, $"{gspRule.Group.ConfigRule}.");
                _ = sb.Append(CultureInfo.InvariantCulture, $"{rule.ConfigRule}");
                _ = sb.Append(Divider);
            }

            record ??= context?.Record;
            if (record is not null)
            {
                _ = sb.Append(record.Registration.Name);
                _ = sb.Append(' ');
                _ = sb.Append(record.FormKey);
                _ = sb.Append(Divider);
            }

            if (context is not null && (record is null || context.ModKey != record.FormKey.ModKey))
            {
                _ = sb.Append(context.ModKey.FileName);
                _ = sb.Append(Divider);
            }

            if (propertyName is not null)
            {
                _ = sb.Append(propertyName);
                _ = sb.Append(Divider);
            }

            _ = sb.Append(log);

            Out.WriteLine(sb.ToString());
        }
    }
}