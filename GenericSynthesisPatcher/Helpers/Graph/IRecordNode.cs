using Mutagen.Bethesda.Plugins;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public interface IRecordNode
    {
        public ModKey ModKey { get; }
        public IReadOnlyList<IRecordNode> Overwrites { get; }
        public IReadOnlyList<IRecordNode> OverwrittenBy { get; }
    }
}