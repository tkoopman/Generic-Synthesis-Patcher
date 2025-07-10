using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class FlagsRecordGraph : FlagsRecordNode, IRecordNode
    {
        private const int ClassLogCode = -1;

        private FlagsRecordGraph (IModContext<IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, string propertyName) : base(context.ModKey, context.Record, modKeys, propertyName)
        {
        }

        public static FlagsRecordGraph? Create (ProcessingKeys proKeys)
        {
            var modKeys = proKeys.Rule.Merge[proKeys.RuleKey];

            if (proKeys.Context.IsMaster())
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.NoOverwrites, "No record overwrites found", ClassLogCode);
                return null;
            }

            var master = Mod.FindOriginContext(proKeys.Context);

            var root = new FlagsRecordGraph(master, modKeys, proKeys.Property.PropertyName);
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