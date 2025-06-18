using System.Diagnostics.CodeAnalysis;

using DynamicData;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    /// <summary>
    ///     Base node class for creating a graph of mod record relationships.
    /// </summary>
    /// <param name="modKey">Mod this record is in (Not the mod that created the record)</param>
    /// <param name="record">This record found in this mod</param>
    /// <param name="modKeys">Mod Keys Operations for forcing a mod's changes or ignoring</param>

    public abstract class RecordNodeBase (ModKey modKey, IMajorRecordGetter record, IReadOnlyList<ModKeyListOperation>? modKeys) : IRecordNode
    {
        private readonly List<RecordNodeBase> overwrites = [];
        private readonly List<RecordNodeBase> overwrittenBy = [];

        /// <summary>
        ///     Key to mod that this record came from
        /// </summary>
        public ModKey ModKey { get; } = modKey;

        /// <summary>
        ///     List of nodes this record overwrites (Parents) Based on mod's Masters
        /// </summary>
        public IReadOnlyList<IRecordNode> Overwrites => overwrites.AsReadOnly();

        /// <summary>
        ///     List of nodes this record is overwritten by (Children) Base on other mod's Masters
        /// </summary>
        public IReadOnlyList<IRecordNode> OverwrittenBy => overwrittenBy.AsReadOnly();

        public IMajorRecordGetter Record { get; } = record;

        /// <summary>
        ///     Mod Key Operations for forcing a mod's changes or ignoring them
        /// </summary>
        protected IReadOnlyList<ModKeyListOperation>? ModKeys { get; } = modKeys;

        /// <summary>
        ///     Create the graph
        /// </summary>
        /// <param name="root">Graph node pointing to parent record</param>
        protected static void populate (RecordNodeBase root)
        {
            var all = Global.Game.State.LinkCache.ResolveAllSimpleContexts(root.Record.FormKey, root.Record.Registration.GetterType);
            int count = all.Count() - 2;

            for (int i = count; i >= 0; i--)
            {
                var m = root.ModKeys?.FirstOrDefault(m => m.Value.Equals(all.ElementAt(i).ModKey));
                if (m is not null && m.Operation == ListLogic.NOT)
                {
                    Global.TraceLogger?.WriteLine($"Merge {root.ModKey.FileName}. Excluding {all.ElementAt(i).ModKey.FileName}");
                    continue;
                }

                var node = root.createChild(all.ElementAt(i), root.ModKeys);
                int index = Global.Game.State.LinkCache.ListedOrder.IndexOf(node.ModKey, static (i, k) => i.ModKey == k);

                Global.TraceLogger?.WriteLine($"Creating graph node {node.ModKey} under {root.ModKey}");

                var masters = Global.Settings.Value.DynamicMods.Contains(all.ElementAt(i).ModKey)
                    ? [all.ElementAt(i + 1).ModKey]
                    : Global.Game.State.LinkCache.ListedOrder[index].MasterReferences.Select(m => m.Master);

                // If last entry in load order but has no masters it must be an existing GSP patch
                // record, so link it to previous winner.
                if (i == 0 && !masters.Any())
                    masters = [all.ElementAt(1).ModKey];

                foreach (var nodeMaster in masters)
                {
                    if (root.tryFindRecord(nodeMaster, out var nodeOverwrites))
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

        protected void cleanUp ()
        {
            foreach (var a in overwrites)
            {
                foreach (var b in a.overwrites)
                {
                    b.removeOverwrittenBy(this);
                }
            }

            foreach (var c in overwrittenBy.ToArray())
                c.cleanUp();
        }

        protected abstract RecordNodeBase createChild (IModContext<IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys);

        protected void print (string line)
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
                node.print(line);
        }

        private void removeOverwrittenBy (RecordNodeBase recordGraph)
        {
            if (overwrittenBy.Remove(recordGraph) && overwrittenBy.Count == 0)
                throw new Exception("Houston, I think we have a problem!");

            foreach (var o in overwrites)
                o.removeOverwrittenBy(recordGraph);
        }

        private bool tryFindRecord (ModKey modKey, [NotNullWhen(true)] out RecordNodeBase? result)
        {
            result = null;
            if (ModKey == modKey)
            {
                result = this;
                return true;
            }

            foreach (var node in overwrittenBy)
            {
                if (node.tryFindRecord(modKey, out result))
                    return true;
            }

            return false;
        }
    }
}