using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class RecordGraph<TItem> : RecordNode<TItem>, IRecordNode
        where TItem : class
    {
        protected RecordGraph (IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, Func<IMajorRecordGetter, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate) : base(context, modKeys, predicate, debugPredicate)
        {
        }

        public static RecordGraph<TItem>? Create (ProcessingKeys proKeys, Func<IMajorRecordGetter, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate)
        {
            var modKeys = proKeys.Rule.Merge[proKeys.RuleKey];

            var master = Mod.FindOriginContext(proKeys.Context);

            if (master == null)
            {
                Global.TraceLogger?.WriteLine("No record overwrites found");
                return null;
            }

            var root = new RecordGraph<TItem>(master, modKeys, predicate, debugPredicate);
            Populate(root);

            if (Global.TraceLogger != null)
            {
                Global.TraceLogger?.WriteLine("Graph Pre Cleanup");
                root.Print(string.Empty);
            }

            root.CleanUp();

            if (Global.TraceLogger != null)
            {
                Global.TraceLogger?.WriteLine("Graph Post Cleanup");
                root.Print(string.Empty);
            }

            return root;
        }

        public bool Merge ([NotNullWhen(true)] out IReadOnlyList<TItem>? newList)
        {
            if (!Merge(null, out _, out _, out _))
            {
                newList = null;
                return false;
            }

            newList = workingList.AsReadOnly();
            return true;
        }
    }
}