using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using DynamicData;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    /// <summary>
    /// Base node class for creating a graph of mod record relationships.
    /// </summary>
    /// <param name="modKey">Mod this record is in (Not the mod that created the record)</param>
    /// <param name="record">This record found in this mod</param>
    /// <param name="modKeys">Mod Keys Operations for forcing a mod's changes or ignoring</param>

    public abstract class RecordNodeBase (ModKey modKey, IMajorRecordGetter record, IReadOnlyList<ModKeyListOperation>? modKeys) : IRecordNode
    {
        private readonly List<RecordNodeBase> overwrites = [];
        private readonly List<RecordNodeBase> overwrittenBy = [];

        /// <summary>
        /// Create base node class from record context
        /// </summary>
        /// <param name="context">Record context</param>
        /// <param name="modKeys">Mod Key Operations for forcing a mod's changes or ignoring them</param>
        public RecordNodeBase (IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys) : this(context.ModKey, context.Record, modKeys)
        {
        }

        /// <summary>
        /// Key to mod that this record came from
        /// </summary>
        public ModKey ModKey { get; } = modKey;

        /// <summary>
        /// List of nodes this record overwrites (Parents)
        /// Based on mod's Masters
        /// </summary>
        public IReadOnlyList<IRecordNode> Overwrites => overwrites.AsReadOnly();

        /// <summary>
        /// List of nodes this record is overwritten by (Children)
        /// Base on other mod's Masters
        /// </summary>
        public IReadOnlyList<IRecordNode> OverwrittenBy => overwrittenBy.AsReadOnly();

        public IMajorRecordGetter Record { get; } = record;

        /// <summary>
        /// Mod Key Operations for forcing a mod's changes or ignoring them
        /// </summary>
        protected IReadOnlyList<ModKeyListOperation>? ModKeys { get; } = modKeys;

        /// <summary>
        /// Create the graph
        /// </summary>
        /// <param name="root">Graph node pointing to parent record</param>
        protected static void Populate (RecordNodeBase root)
        {
            var all = Global.State.LinkCache.ResolveAllContexts(root.Record.FormKey, root.Record.Registration.GetterType);
            int count = all.Count() - 2;

            for (int i = count; i >= 0; i--)
            {
                var m = root.ModKeys?.FirstOrDefault(m => m.Value.Equals(all.ElementAt(i).ModKey));
                if (m != null && m.Operation == ListLogic.NOT)
                {
                    Global.TraceLogger?.WriteLine($"Merge {root.ModKey.FileName}. Excluding {all.ElementAt(i).ModKey.FileName}");
                    continue;
                }

                var node = root.CreateChild(all.ElementAt(i), root.ModKeys);
                int index = Global.State.LinkCache.ListedOrder.IndexOf(node.ModKey, static (i, k) => i.ModKey == k);

                Global.TraceLogger?.WriteLine($"Creating graph node {node.ModKey} under {root.ModKey}");

                var masters = Global.State.LinkCache.ListedOrder[index].MasterReferences.Select(m => m.Master);

                // If last entry in load order but has no masters it must be an existing GSP patch record,
                // so link it to previous winner.
                if (i == 0 && !masters.Any())
                    masters = [all.ElementAt(1).ModKey];

                foreach (var nodeMaster in masters)
                {
                    if (root.TryFindRecord(nodeMaster, out var nodeOverwrites))
                    {
                        Global.TraceLogger?.WriteLine($"{nodeOverwrites.ModKey} overwritten by {node.ModKey}");
                        nodeOverwrites.overwrittenBy.Add(node);
                        node.overwrites.Add(nodeOverwrites);
                    }
                    else
                    {
                        Global.TraceLogger?.WriteLine($"{nodeMaster} not found on graph.");
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

            foreach (var c in overwrittenBy.ToArray())
                c.CleanUp();
        }

        protected abstract RecordNodeBase CreateChild (IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys);

        protected void Print (string line)
        {
            if (!string.IsNullOrEmpty(line))
                line += " < ";
            line += ModKey.FileName;

            if (overwrittenBy.Count == 0)
            {
                Global.TraceLogger?.WriteLine(line);
                return;
            }

            foreach (var node in overwrittenBy)
                node.Print(line);
        }

        protected bool TryFindRecord (ModKey modKey, [NotNullWhen(true)] out RecordNodeBase? result)
        {
            result = null;
            if (ModKey == modKey)
            {
                result = this;
                return true;
            }

            foreach (var node in overwrittenBy)
            {
                if (node.TryFindRecord(modKey, out result))
                    return true;
            }

            return false;
        }

        private void RemoveOverwrittenBy (RecordNodeBase recordGraph)
        {
            if (overwrittenBy.Remove(recordGraph) && overwrittenBy.Count == 0)
                throw new Exception("Houston, I think we have a problem!");

            foreach (var o in overwrites)
                o.RemoveOverwrittenBy(recordGraph);
        }
    }
}