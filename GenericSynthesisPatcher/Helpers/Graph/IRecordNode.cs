using System.Diagnostics.CodeAnalysis;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public interface IRecordNode
    {
        public IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> Context { get; }

        public IReadOnlyList<IRecordNode> Overwrites { get; }

        public IReadOnlyList<IRecordNode> OverwrittenBy { get; }
    }
}