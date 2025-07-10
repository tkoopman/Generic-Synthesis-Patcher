using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class RecordGraph<TItem> : RecordNode<TItem>, IRecordNode
        where TItem : class
    {
        private const int ClassLogCode = -1;

        private RecordGraph (IModContext<IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, Func<IMajorRecordGetter, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate) : base(context.ModKey, context.Record, modKeys, predicate, debugPredicate)
        {
        }

        public static RecordGraph<TItem>? Create (ProcessingKeys proKeys, Func<IMajorRecordGetter, IReadOnlyList<TItem>?> predicate, Func<TItem, string> debugPredicate)
        {
            var modKeys = proKeys.Rule.Merge[proKeys.RuleKey];

            if (proKeys.Context.IsMaster())
            {
                Global.Logger.WriteLog(LogLevel.Trace, LogType.NoOverwrites, "No record overwrites found", ClassLogCode);
                return null;
            }

            var master = Mod.FindOriginContext(proKeys.Context);

            var root = new RecordGraph<TItem>(master, modKeys, predicate, debugPredicate);
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