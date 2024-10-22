using System.Diagnostics.CodeAnalysis;

using DynamicData;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;
using Noggog.StructuredStrings;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class RecordNode<TItem> : IRecordNode
        where TItem : class
    {
        protected readonly List<TItem> workingList;
        private readonly List<RecordNode<TItem>> overwrites = [];
        private readonly List<RecordNode<TItem>> overwrittenBy = [];
        public IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> Context { get; }
        public IReadOnlyList<IRecordNode> Overwrites => overwrites.AsReadOnly();
        public IReadOnlyList<IRecordNode> OverwrittenBy => overwrittenBy.AsReadOnly();

        protected Func<TItem, string> DebugPredicate { get; }
        protected IReadOnlyList<ModKeyListOperation>? ModKeys { get; }

        protected RecordNode ( IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, Func<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate )
        {
            Context = context;
            workingList = predicate(context)?.ToList() ?? [];
            DebugPredicate = debugPredicate;
            ModKeys = modKeys;
        }

        public bool TryFind ( ModKey modKey, [NotNullWhen(true)] out IRecordNode? result )
        {
            bool b = TryFindRecord(modKey, out var r);
            result = r;

            return b;
        }

        protected static void Populate ( RecordGraph<TItem> root, IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> all, Func<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>, IReadOnlyList<TItem>?> predicate )
        {
            int count = all.Count() - 2;

            for (int i = count; i >= 0; i--)
            {
                var m = root.ModKeys?.FirstOrDefault(m => m.Value.Equals(all.ElementAt(i).ModKey));
                if (m != null && m.Operation == ListLogic.NOT)
                {
                    Global.TraceLogger?.WriteLine($"Merge {root.Context.ModKey.FileName}. Excluding {all.ElementAt(i).ModKey.FileName}");
                    continue;
                }

                var node = new RecordNode<TItem>(all.ElementAt(i), root.ModKeys, predicate, root.DebugPredicate);
                int index = Global.State.LinkCache.ListedOrder.IndexOf(node.Context.ModKey, static (i, k) => i.ModKey == k);

                foreach (var nodeMaster in Global.State.LinkCache.ListedOrder[index].MasterReferences)
                {
                    if (root.TryFindRecord(nodeMaster.Master, out var nodeOverwrites))
                    {
                        nodeOverwrites.overwrittenBy.Add(node);
                        node.overwrites.Add(nodeOverwrites);
                    }
                }
            }
        }

        protected void CleanUp ()
        {
            foreach (var a in overwrites)
            {
                foreach (var b in a.overwrites)
                {
                    b.RemoveOverwrittenBy(this);
                }
            }

            foreach (var c in overwrittenBy)
                c.CleanUp();
        }

        protected bool Merge ( IReadOnlyList<TItem>? parent, out IEnumerable<TItem> add, out IEnumerable<TItem> forceAdd, out IEnumerable<TItem> remove )
        {
            // Done with List instead of Hashtable as in rare cases may have same entry multiple times.
            List<TItem> myAdds = [];
            List<TItem> myRemoves = [];

            List<TItem> forceAdds = [];
            bool _forceAdd = ModKeys?.FirstOrDefault(m => m.Value.Equals(Context.ModKey)) != null; // Must be + as - wouldn't have a node

            foreach (var node in overwrittenBy)
            {
                if (node.Merge(workingList.AsReadOnly(), out var _add, out var _forceAdds, out var _remove))
                {
                    foreach (var ai in _add)
                        Global.TraceLogger?.WriteLine($"Merge {Context.ModKey.FileName} << {node.Context.ModKey.FileName}. Add: {DebugPredicate(ai)}");
                    int a = myAdds.AddMissing(_add);

                    foreach (var fi in _forceAdds)
                        Global.TraceLogger?.WriteLine($"Merge {Context.ModKey.FileName} << {node.Context.ModKey.FileName}. Force Add: {DebugPredicate(fi)}");
                    int f = forceAdds.AddMissing(_forceAdds);

                    foreach (var ri in _remove)
                        Global.TraceLogger?.WriteLine($"Merge {Context.ModKey.FileName} << {node.Context.ModKey.FileName}. Del: {DebugPredicate(ri)}");
                    int r = myRemoves.AddMissing(_remove);

                    Global.TraceLogger?.WriteLine($"Merge {Context.ModKey.FileName} << {node.Context.ModKey.FileName}. Add: {a}/{_add.Count()} Force Add: {f}/{_forceAdds.Count()} Del: {r}/{_remove.Count()}");
                }
            }

            // Combine this records items with any other force adds
            if (_forceAdd)
                _ = forceAdds.AddMissing(workingList);

            forceAdd = forceAdds;

            foreach (var myRemove in myRemoves)
            {
                Global.TraceLogger?.WriteLine($"Merge {Context.ModKey.FileName} << All. Del: {DebugPredicate(myRemove)}");
                _ = workingList.Remove(myRemove);
            }

            foreach (var myAdd in myAdds)
            {
                Global.TraceLogger?.WriteLine($"Merge {Context.ModKey.FileName} << All. Add: {DebugPredicate(myAdd)}");
                workingList.Add(myAdd);
            }

            if (parent == null)
            {
                // Just return current results as will be ignored by RecordGraph which is only one to call this with no parent
                add = myAdds.AsEnumerable<TItem>();
                remove = myRemoves.AsEnumerable<TItem>();

                // Time to actually force add now
                int f = workingList.AddMissing(forceAdds);
                Global.TraceLogger?.WriteLine($"Merge All. Force Added: {f}/{forceAdds.Count}");

                return add.Any() || remove.Any() || f > 0;
            }
            else
            {
                // Work out changes in workingList compared to parent
                add = _forceAdd ? [] : workingList.WhereNotIn(parent);
                remove = parent.WhereNotIn(workingList);
            }

            return add.Any() || remove.Any() || forceAdd.Any();
        }

        protected void Print ( string line )
        {
            if (!string.IsNullOrEmpty(line))
                line += " > ";
            line += Context.ModKey.FileName;

            if (overwrittenBy.Count == 0)
            {
                Global.TraceLogger?.WriteLine(line);
                return;
            }

            foreach (var node in overwrittenBy)
                node.Print(line);
        }

        protected bool TryFindRecord ( ModKey modKey, [NotNullWhen(true)] out RecordNode<TItem>? result )
        {
            result = null;
            if (Context.ModKey == modKey)
            {
                result = this;
                return true;
            }

            foreach (var node in overwrites)
            {
                if (node.TryFindRecord(modKey, out result))
                    return true;
            }

            return false;
        }

        private void RemoveOverwrittenBy ( RecordNode<TItem> recordGraph )
        {
            if (overwrittenBy.Remove(recordGraph) && overwrittenBy.Count == 0)
                throw new Exception("Houston, I think we have a problem!");

            foreach (var o in overwrites)
                o.RemoveOverwrittenBy(recordGraph);
        }
    }
}