using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class FlagsRecordGraph : FlagsRecordNode, IRecordNode
    {
        private FlagsRecordGraph (IModContext<IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, string propertyName) : base(context.ModKey, context.Record, modKeys, propertyName)
        {
        }

        public static FlagsRecordGraph? Create (ProcessingKeys proKeys)
        {
            var modKeys = proKeys.Rule.Merge[proKeys.RuleKey];

            if (proKeys.Context.IsMaster())
            {
                Global.TraceLogger?.WriteLine("No record overwrites found");
                return null;
            }

            var master = Mod.FindOriginContext(proKeys.Context);

            var root = new FlagsRecordGraph(master, modKeys, proKeys.Property.PropertyName);
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

        public bool Merge (out Enum newValue)
        {
            if (!performMerge(-1, out _, out _, out _))
            {
                newValue = (Enum)Enum.ToObject(Type, 0);
                return false;
            }

            newValue = (Enum)Enum.ToObject(Type, WorkingFlags);
            return true;
        }
    }
}