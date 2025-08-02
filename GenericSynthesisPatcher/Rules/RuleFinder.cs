using Common;

using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Rules
{
    public sealed class RuleFinder
    {
        private static readonly IEqualityComparer<GSPBase> comparer = ReferenceEqualityComparer<GSPBase>.Instance;
        private readonly IndexedRecordIDs<GSPBase> excludeIndex = new(comparer);
        private readonly List<GSPBase> excludOnlyIndexed = [];
        private readonly IndexedRecordIDs<GSPBase> includeIndex = new(comparer);
        private readonly List<GSPBase> nonIndexed = [];

        /// <summary>
        ///     Adds rule to finder.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns>True if rule was indexable, false if added non-indexed.</returns>
        public bool AddRule (GSPBase rule)
        {
            if (rule.GetIndexableData(out var include, out var exclude))
            {
                foreach (var i in include)
                    includeIndex.Add(i, rule);

                foreach (var e in exclude)
                    excludeIndex.Add(e, rule);

                // If only excluded items add to extra list to quickly access
                if (exclude.Count != 0 && include.Count == 0)
                    excludOnlyIndexed.Add(rule);

                return true;
            }

            nonIndexed.Add(rule);
            return false;
        }

        public void AddRules (IEnumerable<GSPBase> rules)
        {
            foreach (var rule in rules)
                _ = AddRule(rule);
        }

        public void PrintStats (TextWriter output)
        {
            output.WriteLine($"Non-Indexed: {nonIndexed.Count}");

            output.WriteLine($"Exclude Only Indexed: {excludOnlyIndexed.Count}");
            output.WriteLine("Exclude Index Stats...");
            excludeIndex.PrintStats(output);

            output.WriteLine("Include Index Stats...");
            includeIndex.PrintStats(output);
        }

        public IOrderedEnumerable<GSPBase> Rules (ProcessingKeys proKeys)
        {
            Dictionary<GSPBase,HashSet<RecordID>> rules = [];
            foreach (var (rule, recordIDs) in possibleRules(proKeys.Record))
            {
                if (rule.FullyIndexed || (proKeys.SetRule(rule) && rule.Matches(proKeys)))
                    rules.Add(rule, recordIDs);
            }

            return rules.Keys.OrderBy(i => i.Priority);
        }

        internal Dictionary<GSPBase, HashSet<RecordID>> possibleRules (IMajorRecordGetter record)
        {
            var exclude = excludeIndex.FindAll(record, RecordID.EqualsOptions.All, Global.Game.State.LinkCache).Select(e => e.value).ToHashSet();

            Dictionary<GSPBase, HashSet<RecordID>> rules = [];
            foreach (var r in nonIndexed)
                rules[r] = [];

            foreach (var r in exclude.Count == 0 ? excludOnlyIndexed : excludOnlyIndexed.Where(n => !exclude.Contains(n)))
                rules[r] = [];

            var found = includeIndex.FindAll(record, RecordID.EqualsOptions.All, Global.Game.State.LinkCache);
            foreach (var (rule, recordIDs) in found)
            {
                if (exclude.Contains(rule))
                    continue;

                rules.AddValues(rule, recordIDs);
            }

            return rules;
        }
    }
}