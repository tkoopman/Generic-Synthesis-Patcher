using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;
using Noggog.StructuredStrings;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class RecordGraph<TItem> : RecordNode<TItem>, IRecordNode
        where TItem : class
    {
        protected RecordGraph ( IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, Func<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate ) : base(context, modKeys, predicate, debugPredicate)
        {
        }

        public static RecordGraph<TItem>? Create ( FormKey formKey, Type getter, IReadOnlyList<ModKeyListOperation>? modKeys, Func<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate )
        {
            var all = Global.State.LinkCache.ResolveAllContexts(formKey, getter);
            if (!all.SafeAny())
            {
                Global.TraceLogger?.WriteLine("Failed to find master references");
                return null;
            }

            var root = new RecordGraph<TItem>(all.Last(), modKeys, predicate, debugPredicate);
            Populate(root, all, predicate);

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

        public bool Merge ( [NotNullWhen(true)] out IReadOnlyList<TItem>? newList )
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