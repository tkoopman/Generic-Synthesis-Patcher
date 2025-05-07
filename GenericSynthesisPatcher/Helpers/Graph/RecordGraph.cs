using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class RecordGraph<TItem> : RecordNode<TItem>, IRecordNode
        where TItem : class
    {
        private RecordGraph (IModContext<IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, Func<IMajorRecordGetter, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate) : base(context.ModKey, context.Record, modKeys, predicate, debugPredicate)
        {
        }

        public static RecordGraph<TItem>? Create (ProcessingKeys proKeys, Func<IMajorRecordGetter, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate)
        {
            var modKeys = proKeys.Rule.Merge[proKeys.RuleKey];

            if (proKeys.Context.IsMaster())
            {
                Global.TraceLogger?.WriteLine("No record overwrites found");
                return null;
            }

            var master = Mod.FindOriginContext(proKeys.Context);

            var root = new RecordGraph<TItem>(master, modKeys, predicate, debugPredicate);
            populate(root);

            if (Global.TraceLogger != null)
            {
                Global.TraceLogger?.WriteLine("Graph Pre Cleanup");
                root.print(string.Empty);
            }

            root.cleanUp();

            if (Global.TraceLogger != null)
            {
                Global.TraceLogger?.WriteLine("Graph Post Cleanup");
                root.print(string.Empty);
            }

            return root;
        }

        public bool Merge ([NotNullWhen(true)] out IReadOnlyList<TItem>? newList)
        {
            if (!performMerge(null, out _, out _, out _))
            {
                newList = null;
                return false;
            }

            newList = WorkingList.AsReadOnly();
            return true;
        }
    }
}