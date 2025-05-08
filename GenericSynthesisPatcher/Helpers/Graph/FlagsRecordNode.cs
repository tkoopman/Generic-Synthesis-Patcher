using EnumsNET;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class FlagsRecordNode : RecordNodeBase
    {
        public FlagsRecordNode (ModKey modKey, IMajorRecordGetter record, IReadOnlyList<ModKeyListOperation>? modKeys, string propertyName) : base(modKey, record, modKeys)
        {
            PropertyName = propertyName;
            WorkingFlags = Mod.TryGetProperty(record, PropertyName, out int value, out var propertyType) ? value : throw new InvalidDataException();
            Type = propertyType;
        }

        protected string PropertyName { get; }
        protected Type Type { get; }
        protected int WorkingFlags { get; set; }

        protected override RecordNodeBase createChild (IModContext<IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys) => new FlagsRecordNode(context.ModKey, context.Record, modKeys, PropertyName);

        protected bool performMerge (int parentValue, out int add, out int forceAdd, out int remove)
        {
            // Done with List instead of Hashtable as in rare cases may have same entry multiple
            // times.
            int myAdds = default;
            int myRemoves = default;
            int forceAdds = default;

            bool _forceAdd = ModKeys?.FirstOrDefault(m => m.Value.Equals(ModKey)) != null; // Must be + as - wouldn't have a node

            foreach (var node in OverwrittenBy)
            {
                if (node is FlagsRecordNode myNode && myNode.performMerge(WorkingFlags, out int _add, out int _forceAdds, out int _remove))
                {
                    int a = FlagEnums.GetFlagCount(Type, _add);
                    if (a > 0)
                    {
                        Global.TraceLogger?.WriteLine($"Merge {ModKey.FileName} << {node.ModKey.FileName}. Add: {FlagEnums.FormatFlags(Type, _add)}");
                        myAdds = (int)FlagEnums.CombineFlags(Type, myAdds, _add);
                    }

                    int f = FlagEnums.GetFlagCount(Type, _forceAdds);
                    if (a > 0)
                    {
                        Global.TraceLogger?.WriteLine($"Merge {ModKey.FileName} << {node.ModKey.FileName}. Force Add: {FlagEnums.FormatFlags(Type, _forceAdds)}");
                        forceAdds = (int)FlagEnums.CombineFlags(Type, forceAdds, _forceAdds);
                    }

                    int r = FlagEnums.GetFlagCount(Type, _remove);
                    if (a > 0)
                    {
                        Global.TraceLogger?.WriteLine($"Merge {ModKey.FileName} << {node.ModKey.FileName}. Del: {FlagEnums.FormatFlags(Type, _remove)}");
                        myRemoves = (int)FlagEnums.RemoveFlags(Type, forceAdds, _remove);
                    }

                    Global.TraceLogger?.WriteLine($"Merge {ModKey.FileName} << {node.ModKey.FileName}. Add: {a} Force Add: {f} Del: {r}");
                }
            }

            // Combine this records items with any other force adds
            if (_forceAdd)
                forceAdds = (int)FlagEnums.CombineFlags(Type, forceAdds, WorkingFlags);

            forceAdd = forceAdds;

            if (myRemoves > 0)
            {
                Global.TraceLogger?.WriteLine($"Merge {ModKey.FileName} << All. Del: {FlagEnums.FormatFlags(Type, myRemoves)}");
                WorkingFlags = (int)FlagEnums.RemoveFlags(Type, WorkingFlags, myRemoves);
            }

            if (myAdds > 0)
            {
                Global.TraceLogger?.WriteLine($"Merge {ModKey.FileName} << All. Add: {FlagEnums.FormatFlags(Type, myAdds)}");
                WorkingFlags = (int)FlagEnums.CombineFlags(Type, WorkingFlags, myAdds);
            }

            if (parentValue == -1)
            {
                // Just return current results as will be ignored by RecordGraph which is only one
                // to call this with no parent
                add = myAdds;
                remove = myRemoves;

                // Time to actually force add now
                int f =  FlagEnums.GetFlagCount(Type, WorkingFlags);
                WorkingFlags |= forceAdds;
                f = FlagEnums.GetFlagCount(Type, WorkingFlags) - f;

                Global.TraceLogger?.WriteLine($"Merge All. Force Added: {f}/{FlagEnums.GetFlagCount(Type, forceAdds)}");

                return add > 0 || remove > 0 || f > 0;
            }
            else
            {
                // Work out changes in workingList compared to parent
                add = _forceAdd ? default : WorkingFlags & ((int)FlagEnums.GetAllFlags(Type) ^ parentValue);
                remove = parentValue & ((int)FlagEnums.GetAllFlags(Type) ^ WorkingFlags);
            }

            return add > 0 || remove > 0 || forceAdd > 0;
        }
    }
}