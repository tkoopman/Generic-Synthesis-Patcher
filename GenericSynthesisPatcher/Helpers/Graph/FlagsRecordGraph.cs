using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class FlagsRecordGraph : FlagsRecordNode, IRecordNode
    {
        protected FlagsRecordGraph (IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, string propertyName) : base(context.ModKey, context.Record, modKeys, propertyName)
        {
        }

        public static FlagsRecordGraph? Create (ProcessingKeys proKeys)
        {
            var modKeys = proKeys.Rule.Merge[proKeys.RuleKey];

            var master = Mod.FindOriginContext(proKeys.Context);

            if (master == null)
            {
                Global.TraceLogger?.WriteLine("No record overwrites found");
                return null;
            }

            var root = new FlagsRecordGraph(master, modKeys, proKeys.Property.PropertyName);
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

        public bool Merge (out Enum newValue)
        {
            if (!Merge(-1, out _, out _, out _))
            {
                newValue = (Enum)Enum.ToObject(Type, 0);
                return false;
            }

            newValue = (Enum)Enum.ToObject(Type, workingList);
            return true;
        }
    }
}