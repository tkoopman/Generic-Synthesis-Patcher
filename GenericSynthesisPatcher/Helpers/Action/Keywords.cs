using System.Xml.Linq;

using DynamicData;

using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x2xx
    public class Keywords : IAction
    {
        private static Dictionary<string, IKeywordGetter>? keywords;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        // Log Codes: 0x21x
        public static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, ValueKey valueKey, RecordCallData rcd )
        {
            IKeyworded<IKeywordGetter>? patch = null;

            if (context.Record is IKeywordedGetter<IKeywordGetter> record)
            {
                if (rule.OnlyIfDefault && origin != null && origin is IKeywordedGetter<IKeywordGetter> originKG && !record.Keywords.SequenceEqualNullable(originKG.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch);
                    return false;
                }

                foreach (string c in rule.GetValueAs<List<string>>(valueKey) ?? [])
                {
                    string s = c;
                    if (s.StartsWith('-'))
                    {
                        s = s[1..];
                        var remove = GetKeyword(s);
                        if (remove != null && record.HasKeyword(remove))
                        {
                            patch ??= (IKeyworded<IKeywordGetter>)context.GetOrAddAsOverride(Global.State.PatchMod);
                            patch.Keywords?.Remove(remove);
                        }

                        continue;
                    }

                    if (s.StartsWith('+'))
                        s = s[1..];

                    var add = GetKeyword(s);
                    if (add != null && !record.HasKeyword(add))
                    {
                        patch ??= (IKeyworded<IKeywordGetter>)context.GetOrAddAsOverride(Global.State.PatchMod);
                        patch.Keywords?.Add(add);
                    }
                }
            }
            else
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IKeywordedGetter", context.Record.GetType().Name, 0x211);
            }

            if (patch != null)
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");
            return patch != null;
        }

        // Log Codes: 0x22x
        public static bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd )
        {
            IKeyworded<IKeywordGetter>? patch = null;

            if (context.Record is IKeywordedGetter<IKeywordGetter> record && forwardContext.Record is IKeywordedGetter<IKeywordGetter> forward)
            {
                if (forward.Keywords.SequenceEqualNullable(record.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual);
                    return false;
                }

                if (rule.OnlyIfDefault && origin != null && origin is IKeywordedGetter<IKeywordGetter> originGetter && !record.Keywords.SequenceEqualNullable(originGetter.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch);
                    return false;
                }

                if (rule.ForwardType.GetFlags().Contains(ForwardTypes.SelfMasterOnly))
                {
                    if (forward.Keywords == null)
                        return false;

                    foreach (var item in forward.Keywords)
                    {
                        if (item.FormKey.ModKey == forwardContext.ModKey)
                        {
                            if (record.Keywords == null || !record.Keywords.Contains(item))
                            {
                                patch ??= (IKeyworded<IKeywordGetter>)context.GetOrAddAsOverride(Global.State.PatchMod);
                                patch.Keywords?.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    patch ??= (IKeyworded<IKeywordGetter>)context.GetOrAddAsOverride(Global.State.PatchMod);

                    _ = patch.Keywords?.RemoveAll(_ => true);

                    if (forward.Keywords != null)
                        patch.Keywords?.AddRange(forward.Keywords);
                }
            }
            else
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IKeywordedGetter", context.Record.GetType().Name, 0x221);
            }

            if (patch != null)
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");

            return patch != null;
        }

        public static IKeywordGetter? GetKeyword ( string name )
        {
            if (keywords == null)
            {
                keywords = [];
                Global.State.LoadOrder.PriorityOrder.Keyword().WinningOverrides().ForEach(k => keywords[k.EditorID ?? ""] = k);
                LogHelper.Log(LogLevel.Information, $"{keywords.Count} keywords loaded into cache.");
            }

            _ = keywords.TryGetValue(name, out var value);
            return value;
        }
    }
}