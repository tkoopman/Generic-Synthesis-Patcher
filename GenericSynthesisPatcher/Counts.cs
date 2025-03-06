using System.Diagnostics;

namespace GenericSynthesisPatcher
{
    public class Counts (int total = 0, int matched = 0, int updated = 0, int changes = 0)
    {
        public int Changes { get; set; } = changes;

        public int Matched { get; set; } = matched;

        public Stopwatch Stopwatch { get; } = new Stopwatch();

        public int Total { get; set; } = total;

        public int Updated { get; set; } = updated;
    }
}