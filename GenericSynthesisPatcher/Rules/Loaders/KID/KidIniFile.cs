using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Common;

namespace GenericSynthesisPatcher.Rules.Loaders.KID
{
    /// <summary>
    ///     Stores all lines from KID INI file, along with their parsed <see cref="KidIniLine" /> if
    ///     applicable. Storing non-valid lines so can write back to file if needed.
    /// </summary>
    public class KidIniFile : IEnumerable<KidIniLine>
    {
        internal readonly List<KidIniLine> lines;

        /// <summary>
        ///     Create new KIDFile from the contents of a KID INI file.
        /// </summary>
        /// <param name="contents">INI File contents</param>
        /// <exception cref="ArgumentException">
        ///     If line was marked as previously being processed by GSP, but wasn't a valid KID entry.
        /// </exception>
        public KidIniFile (string contents)
        {
            lines = [];

            foreach (string line in contents.Lines())
            {
                lines.Add(new KidIniLine(line));
            }
        }

        /// <summary>
        ///     For use in xUnit testing.
        /// </summary>
        internal KidIniFile (List<KidIniLine> lines) => this.lines = lines;

        /// <summary>
        ///     Load all KID INI files from the specified directory.
        /// </summary>
        /// <param name="path">Directory path to load files from.</param>
        /// <returns>
        ///     Dictionary keyed by file path. Only includes KID file entries that contain at least
        ///     1 valid entry.
        /// </returns>
        public static Dictionary<string, KidIniFile> Load (string path)
        {
            Dictionary<string, KidIniFile> kidFiles = [];
            if (!Directory.Exists(path))
                return kidFiles;

            foreach (string filePath in Directory.GetFiles(path, "*_KID.ini", SearchOption.TopDirectoryOnly))
            {
                if (TryLoadFile(filePath, out var kidFile))
                    kidFiles[filePath] = kidFile;
            }

            return kidFiles;
        }

        /// <summary>
        ///     Attempts to load a KID INI File from the specified file path.
        /// </summary>
        /// <param name="filePath">File to load. Should end in _KID.ini but not actually checked.</param>
        /// <param name="kidFile">Result if contents read.</param>
        /// <returns>True if file was able to be read and it contained at least 1 KIDEntry</returns>
        public static bool TryLoadFile (string filePath, [NotNullWhen(true)] out KidIniFile? kidFile)
        {
            kidFile = null;

            if (!Path.Exists(filePath))
                return false;

            string contents = File.ReadAllText(filePath);

            kidFile = new KidIniFile(contents);

            return kidFile.lines.Any(k => k.SeemsValid);
        }

        public bool Equals (KidIniFile? other)
        {
            if (other is null || lines.Count != other.lines.Count)
                return false;

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].Equals(other.lines[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Used for xUnit testing to compare KIDFile objects.
        /// </summary>
        public override bool Equals (object? obj) => obj is KidIniFile kidFile && Equals(kidFile);

        public IEnumerator<KidIniLine> GetEnumerator () => lines.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

        /// <summary>
        ///     Not really used in GSP, so just returns the hash code of the line count and first line.
        /// </summary>
        public override int GetHashCode () => lines.Count == 0 ? 0 : HashCode.Combine(lines.Count, lines[0].Line);
    }
}