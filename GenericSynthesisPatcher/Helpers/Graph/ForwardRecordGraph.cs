using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class ForwardRecordGraph : ForwardRecordNode, IRecordNode
    {
        private ForwardRecordGraph (IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context) : base(context.ModKey, context.Record)
        {
        }

        public static ForwardRecordGraph? Create (ProcessingKeys proKeys)
        {
            if (proKeys.Context.IsMaster())
            {
                Global.TraceLogger?.WriteLine("No record overwrites found");
                return null;
            }

            var master = Mod.FindOriginContext(proKeys.Context);

            var root = new ForwardRecordGraph(master);
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
    }
}