using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class ForwardRecordNode (ModKey modKey, IMajorRecordGetter record) : RecordNodeBase(modKey, record, null)
    {
        public IEnumerable<ModKey>? GetEndNodes (IEnumerable<ModKey>? validMods = null)
        {
            if (OverwrittenBy.Any())
            {
                HashSet<ModKey> endNodes = [];
                foreach (var node in OverwrittenBy)
                {
                    if (node is ForwardRecordNode sNode)
                    {
                        var en = sNode.GetEndNodes(validMods);
                        if (en is null)
                        {
                            if (validMods is null || validMods.Contains(ModKey))
                                _ = endNodes.Add(ModKey);
                        }
                        else
                        {
                            foreach (var endNode in en)
                                _ = endNodes.Add(endNode);
                        }
                    }
                }

                return endNodes;
            }
            else
            {
                return validMods is null || validMods.Contains(ModKey) ? [ModKey] : null;
            }
        }

        protected override RecordNodeBase createChild (IModContext<IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys) => new ForwardRecordNode(context.ModKey, context.Record);
    }
}