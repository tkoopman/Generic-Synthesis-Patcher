using System.Data;
using System.Linq;
using System.Xml.Linq;

using DynamicData;

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
    internal static class Keyword
    {
        private static Dictionary<string, IKeywordGetter>? keywords;
        private static IKeywordGetter? GetKeyword ( string name )
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

        // Log Codes: 0x21x
        public static bool FillKeywords ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, ValueKey valueKey, string propertyName )
        {
            IKeyworded<IKeywordGetter>? patch = null;

            if (context.Record is IKeywordedGetter<IKeywordGetter> record)
            {
                if (rule.OnlyIfDefault && origin != null && origin is IKeywordedGetter<IKeywordGetter> originKG && !record.Keywords.SequenceEqualNullable(originKG.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as keywords don't match origin");
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
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "IKeywordedGetter", context.Record.GetType().Name, 0x211);
            }

            if (patch != null)
                LogHelper.Log(LogLevel.Debug, context, propertyName, "Updated.");
            return patch != null;
        }

        // Log Codes: 0x22x
        public static bool ForwardKeywords ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IMajorRecordGetter forwardRecord, string propertyName )
        {
            IKeyworded<IKeywordGetter>? patch = null;

            if (context.Record is IKeywordedGetter<IKeywordGetter> record && forwardRecord is IKeywordedGetter<IKeywordGetter> forward)
            {
                if (forward.Keywords.SequenceEqualNullable(record.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as already matches forwarding record");
                    return false;
                }

                if (rule.OnlyIfDefault && origin != null && origin is IKeywordedGetter<IKeywordGetter> originGetter && !record.Keywords.SequenceEqualNullable(originGetter.Keywords))
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as keywords don't match origin");
                    return false;
                }

                patch ??= (IKeyworded<IKeywordGetter>)context.GetOrAddAsOverride(Global.State.PatchMod);

                _ = patch.Keywords?.RemoveAll(_ => true);

                if (forward.Keywords != null)
                    patch.Keywords?.AddRange(forward.Keywords);
            }
            else
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "IKeywordedGetter", context.Record.GetType().Name, 0x221);
            }

            if (patch != null)
                LogHelper.Log(LogLevel.Debug, context, propertyName, "Updated.");

            return patch != null;
        }
    }
}