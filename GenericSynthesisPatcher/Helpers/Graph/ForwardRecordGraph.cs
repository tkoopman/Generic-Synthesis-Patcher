using Common;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class ForwardRecordGraph : ForwardRecordNode, IRecordNode
    {
        private const int ClassLogCode = -1;

        private ForwardRecordGraph (IModContext<IMajorRecordGetter> context) : base(context.ModKey, context.Record)
        {
        }

        public static ForwardRecordGraph? Create (ProcessingKeys proKeys)
        {
            if (proKeys.Context.IsMaster())
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.NoOverwrites, "No record overwrites found", ClassLogCode);
                return null;
            }

            var master = Mod.FindOriginContext(proKeys.Context);

            var root = new ForwardRecordGraph(master);
            populate(root);

            if (Global.Logger.CurrentLogLevel == LogLevel.Trace)
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, "Graph Pre Cleanup", ClassLogCode);
                root.print(string.Empty);
            }

            root.cleanUp();

            if (Global.Logger.CurrentLogLevel == LogLevel.Trace)
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, "Graph Post Cleanup", ClassLogCode);
                root.print(string.Empty);
            }

            return root;
        }
    }
}