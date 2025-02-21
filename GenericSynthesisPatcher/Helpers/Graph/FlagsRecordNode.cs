using EnumsNET;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Graph
{
    public class FlagsRecordNode : RecordNodeBase
    {
        protected int workingList;

        public FlagsRecordNode (ModKey modKey, IMajorRecordGetter record, IReadOnlyList<ModKeyListOperation>? modKeys, string propertyName) : base(modKey, record, modKeys)
        {
            PropertyName = propertyName;
            workingList = Mod.TryGetProperty(record, PropertyName, out int value, out var propertyInfo) ? value : throw new Exception();
            Type = propertyInfo.PropertyType;
        }

        public FlagsRecordNode (IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys, string propertyName) : this(context.ModKey, context.Record, modKeys, propertyName)
        {
        }

        protected string PropertyName { get; }
        protected Type Type { get; }

        protected override RecordNodeBase CreateChild (IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context, IReadOnlyList<ModKeyListOperation>? modKeys) => new FlagsRecordNode(context, modKeys, PropertyName);

        protected bool Merge (int parentValue, out int add, out int forceAdd, out int remove)
        {
            // Done with List instead of Hashtable as in rare cases may have same entry multiple times.
            int myAdds = default;
            int myRemoves = default;
            int forceAdds = default;

            bool _forceAdd = ModKeys?.FirstOrDefault(m => m.Value.Equals(ModKey)) != null; // Must be + as - wouldn't have a node

            foreach (var node in OverwrittenBy)
            {
                if (node is FlagsRecordNode myNode && myNode.Merge(workingList, out var _add, out var _forceAdds, out var _remove))
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
                forceAdds = (int)FlagEnums.CombineFlags(Type, forceAdds, workingList);

            forceAdd = forceAdds;

            if (myRemoves > 0)
            {
                Global.TraceLogger?.WriteLine($"Merge {ModKey.FileName} << All. Del: {FlagEnums.FormatFlags(Type, myRemoves)}");
                workingList = (int)FlagEnums.RemoveFlags(Type, workingList, myRemoves);
            }

            if (myAdds > 0)
            {
                Global.TraceLogger?.WriteLine($"Merge {ModKey.FileName} << All. Add: {FlagEnums.FormatFlags(Type, myAdds)}");
                workingList = (int)FlagEnums.CombineFlags(Type, workingList, myAdds);
            }

            if (parentValue == -1)
            {
                // Just return current results as will be ignored by RecordGraph which is only one to call this with no parent
                add = myAdds;
                remove = myRemoves;

                // Time to actually force add now
                int f =  FlagEnums.GetFlagCount(Type, workingList);
                workingList = workingList | forceAdds;
                f = FlagEnums.GetFlagCount(Type, workingList) - f;

                Global.TraceLogger?.WriteLine($"Merge All. Force Added: {f}/{FlagEnums.GetFlagCount(Type, forceAdds)}");

                return add > 0 || remove > 0 || f > 0;
            }
            else
            {
                // Work out changes in workingList compared to parent
                add = _forceAdd ? default : workingList & ((int)FlagEnums.GetAllFlags(Type) ^ parentValue);
                remove = parentValue & ((int)FlagEnums.GetAllFlags(Type) ^ workingList);
            }

            return add > 0 || remove > 0 || forceAdd > 0;
        }
    }
}