using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class RecordNode<TItem> (ModKey modKey, IMajorRecordGetter record, IReadOnlyList<ModKeyListOperation>? modKeys, Func<IMajorRecordGetter, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate) : RecordNodeBase(modKey, record, modKeys)
        where TItem : class
    {
        private const int ClassLogCode = -1;
        protected Func<TItem, string> DebugPredicate { get; } = debugPredicate;
        protected Func<IMajorRecordGetter, IReadOnlyList<TItem>?> Predicate { get; } = predicate;
        protected List<TItem> WorkingList { get; } = predicate(record)?.ToList() ?? [];

        protected override RecordNodeBase createChild (IModContext<IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys) => new RecordNode<TItem>(context.ModKey, context.Record, modKeys, Predicate, DebugPredicate);

        protected bool performMerge (IReadOnlyList<TItem>? parent, out IEnumerable<TItem> add, out IEnumerable<TItem> forceAdd, out IEnumerable<TItem> remove)
        {
            // Done with List instead of Hashtable as in rare cases may have same entry multiple times.
            List<TItem> myAdds = [];
            List<TItem> myRemoves = [];

            List<TItem> forceAdds = [];
            bool _forceAdd = ModKeys?.FirstOrDefault(m => m.Value.Equals(ModKey)) is not null; // Must be + as - wouldn't have a node

            foreach (var node in OverwrittenBy)
            {
                if (node is RecordNode<TItem> myNode && myNode.performMerge(WorkingList.AsReadOnly(), out var _add, out var _forceAdds, out var _remove))
                {
                    foreach (var ai in _add)
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Merge {ModKey.FileName} << {node.ModKey.FileName}. Add: {DebugPredicate(ai)}", ClassLogCode);
                    int a = myAdds.AddMissing(_add);

                    foreach (var fi in _forceAdds)
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Merge {ModKey.FileName} << {node.ModKey.FileName}. Force Add: {DebugPredicate(fi)}", ClassLogCode);
                    int f = forceAdds.AddMissing(_forceAdds);

                    foreach (var ri in _remove)
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Merge {ModKey.FileName} << {node.ModKey.FileName}. Del: {DebugPredicate(ri)}", ClassLogCode);
                    int r = myRemoves.AddMissing(_remove);

                    Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Merge {ModKey.FileName} << {node.ModKey.FileName}. Add: {a}/{_add.Count()} Force Add: {f}/{_forceAdds.Count()} Del: {r}/{_remove.Count()}", ClassLogCode);
                }
            }

            // Combine this records items with any other force adds
            if (_forceAdd)
                _ = forceAdds.AddMissing(WorkingList);

            forceAdd = forceAdds;

            foreach (var myRemove in myRemoves)
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Merge {ModKey.FileName} << All. Del: {DebugPredicate(myRemove)}", ClassLogCode);
                _ = WorkingList.Remove(myRemove);
            }

            foreach (var myAdd in myAdds)
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Merge {ModKey.FileName} << All. Add: {DebugPredicate(myAdd)}", ClassLogCode);
                WorkingList.Add(myAdd);
            }

            if (parent is null)
            {
                // Just return current results as will be ignored by RecordGraph which is only one
                // to call this with no parent
                add = myAdds.AsEnumerable<TItem>();
                remove = myRemoves.AsEnumerable<TItem>();

                // Time to actually force add now
                int f = WorkingList.AddMissing(forceAdds);
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Merge All. Force Added: {f}/{forceAdds.Count}", ClassLogCode);

                return add.Any() || remove.Any() || f > 0;
            }
            else
            {
                // Work out changes in workingList compared to parent
                add = _forceAdd ? [] : WorkingList.WhereNotIn(parent);
                remove = parent.WhereNotIn(WorkingList);
            }

            return add.Any() || remove.Any() || forceAdd.Any();
        }
    }
}