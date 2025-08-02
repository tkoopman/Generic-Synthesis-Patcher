using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Common;

using Loqui;

namespace GenericSynthesisPatcher.Rules.Loaders.KID
{
    /// <summary>
    ///     Represents the data from a single line in a KID INI file.
    /// </summary>
    public partial class KidIniLine
    {
        public const int DEFAULTCHANCE = 100;
        private const string KID_GSP_PREFIX = "; Handled by GSP - ";

        public KidIniLine (int lineNumber, string line)
        {
            LineNumber = lineNumber;

            // Has line been processed by GSP in past?
            WasHandledByGsp = KIDGspPrefixRegex().IsMatch(line);
            Line = WasHandledByGsp ? KIDGspPrefixRegex().Replace(line, string.Empty).TrimStart() : line;

            // Confirm line starts with Keyword =
            var regex = KIDKeywordPrefixRegex();
            if (!regex.IsMatch(Line))
                return;

            string iniLine = regex.Replace(Line, string.Empty, 1);
            string[] parts = iniLine.Split('|');

            // Valid lines must have between 3 and 5 parts
            if (parts.Length is < 3 or > 5)
                return;

            Type = Global.Game.GetRecordType(parts[1]);
            if (Type is null)
                return;

            _ = SynthCommon.TryConvertToBethesdaID(parts[0].Trim(), out var keyword);
            Keyword = keyword;
            Strings = cleanString(parts[2], out string? strings) ? [.. strings.Split(',').Select(s => s.Trim()).Where(s => s.Length != 0)] : [];
            Traits = parts.Length > 3 && cleanString(parts[3], out string? traits) ? [.. traits.Split(',').Select(s => s.Trim()).Where(s => s.Length != 0)] : [];
            Chance = parts.Length > 4 && cleanString(parts[4], out string? sChance) ? (int.TryParse(sChance, out int chance) ? chance : -1) : DEFAULTCHANCE;
        }

        /// <summary>
        ///     Marked internal as used in xUnit testing.
        /// </summary>
        internal KidIniLine (string line, bool hadGspPrefix, RecordID keyword, ILoquiRegistration? type, string[]? strings, string[]? traits, int chance)
        {
            Line = line;
            WasHandledByGsp = hadGspPrefix;
            Keyword = keyword;
            Type = type;
            Strings = strings;
            Traits = traits;
            Chance = chance;
        }

        public int Chance { get; internal set; } = DEFAULTCHANCE;

        /// <summary>
        ///     Has this line been successfully converted into a GSP Rule
        /// </summary>
        public bool HandledByGsp { get; set; }

        public RecordID Keyword { get; }

        /// <summary>
        ///     Original line from INI file, with GSP Prefix removed if it existed.
        /// </summary>
        public string Line { get; }

        public int LineNumber { get; }

        /// <summary>
        ///     Returns if the line seems to be valid basic format for KID entry. This is just basic
        ///     checks not validating the data like valid FormKey(s) or EditorID(s) used.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Keyword))]
        [MemberNotNullWhen(true, nameof(Type))]
        [MemberNotNullWhen(true, nameof(Strings))]
        [MemberNotNullWhen(true, nameof(Traits))]
        public bool SeemsValid
            => Keyword.Type is IDType.FormKey or IDType.Name
            && Type is not null
            && Strings is not null
            && Traits is not null
            && Chance is > 0 && Chance <= DEFAULTCHANCE
            && (Strings.Length != 0 || Traits.Length != 0);

        public string[]? Strings { get; }

        public string[]? Traits { get; }

        public ILoquiRegistration? Type { get; }

        /// <summary>
        ///     Was line masked as handled by GSP when read from file.
        /// </summary>
        public bool WasHandledByGsp { get; }

        public bool Equals (KidIniLine? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            // Main compare
            return
                Line == other.Line &&
                HandledByGsp == other.HandledByGsp &&
                Keyword == other.Keyword &&
                Type == other.Type &&
                Chance == other.Chance &&
                Strings.SafeSequenceEqual(other.Strings) &&
                Traits.SafeSequenceEqual(other.Traits);
        }

        public override bool Equals (object? obj) => obj is KidIniLine kidEntry && Equals(kidEntry);

        public override int GetHashCode () => HashCode.Combine(Keyword, Type, Chance);

        public override string ToString () => HandledByGsp ? $"{KID_GSP_PREFIX}{Line}" : Line;

        private static bool cleanString (string? input, [NotNullWhen(true)] out string? output)
        {
            input = input?.Trim();

            output = input is null || input.Length == 0 || input.Equals("NONE", StringComparison.OrdinalIgnoreCase) ? null : input;

            return output is not null;
        }

        [GeneratedRegex($"(?i)^{KID_GSP_PREFIX}")]
        private static partial Regex KIDGspPrefixRegex ();

        [GeneratedRegex(@"(?i)^\s*Keyword\s*=\s*")]
        private static partial Regex KIDKeywordPrefixRegex ();
    }
}