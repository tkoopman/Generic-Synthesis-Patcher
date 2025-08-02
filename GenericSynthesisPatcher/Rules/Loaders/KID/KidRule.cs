using System.Data;

using Common;

using Loqui;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Aspects;

using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Rules.Loaders.KID
{
    public partial class KidRule : GSPRule
    {
        private const int ClassLogCode = 0x20;

        private KidRule (ILoquiRegistration type, IKeywordCommonGetter keyword, List<RecordID> all, List<RecordID> any, List<RecordID> not)
        {
            Types = [type];
            Keyword = keyword;
            Fill.Add(new("Keywords"), new JValue(keyword.FormKey.ToString()));

            AllStrings = all;
            AnyStrings = any;
            NotStrings = not;
        }

        public enum Logic
        {
            ALL,
            ANY,
            NOT,
        }

        public List<RecordID> AllStrings { get; }

        public List<RecordID> AnyStrings { get; }

        public IKeywordCommonGetter Keyword { get; }

        public List<RecordID> NotStrings { get; }

        public static KidRule? Convert (KidIniLine kidIniLine)
        {
            if (!kidIniLine.SeemsValid)
                return null;

            // Currently unsupported features
            if (kidIniLine.Traits.Length != 0 || kidIniLine.Chance != KidIniLine.DEFAULTCHANCE)
                return null;

            // Keyword needs to be either FormKey or EditorID, but KID supports FormID for DLC so
            // handle that here.
            var recordID = SynthCommon.ConvertToBethesdaID(kidIniLine.Keyword, Global.Game.FormIDToFormKeyConverter);
            IKeywordCommonGetter? keyword;
            switch (recordID.Type)
            {
                case IDType.FormKey:
                    if (!Global.Game.State.LinkCache.TryResolve<IKeywordCommonGetter>(recordID.FormKey, out keyword))
                        return null;

                    break;

                case IDType.Name when !recordID.IsWildcard:
                    if (Global.Game.State.LinkCache.TryResolve<IKeywordCommonGetter>(recordID.Name, out keyword))
                        break;

                    // Create keyword if it didn't exist
                    switch (Global.Game.State.PatchMod)
                    {
                        case Mutagen.Bethesda.Skyrim.ISkyrimMod skyrimMod:
                            var skyrimKeyword = new Mutagen.Bethesda.Skyrim.Keyword(skyrimMod, recordID.Name);
                            skyrimMod.Keywords.Add(skyrimKeyword);
                            keyword = skyrimKeyword;
                            Global.Logger.WriteLog(LogLevel.Debug, Helpers.LogType.RecordUpdated, $"Created new keyword {recordID.Name}.", ClassLogCode);
                            break;

                        case Mutagen.Bethesda.Fallout4.IFallout4Mod fallout4Mod:
                            var fallout4Keyword = new Mutagen.Bethesda.Fallout4.Keyword(fallout4Mod, recordID.Name);
                            fallout4Mod.Keywords.Add(fallout4Keyword);
                            keyword = fallout4Keyword;
                            Global.Logger.WriteLog(LogLevel.Debug, Helpers.LogType.RecordUpdated, $"Created new keyword {recordID.Name}.", ClassLogCode);
                            break;

                        case Mutagen.Bethesda.Oblivion.IOblivionMod oblivionMod:
                        default:
                            return null;
                    }

                    break;

                default:
                    return null;
            }

            var strings = ParseStrings(kidIniLine.Strings ?? []);

            foreach (var s in new List<(Logic Logic, RecordID RecordID)>(strings))
            {
                bool remove = s.RecordID.Type switch
                {
                    IDType.FormKey => !Global.Game.State.RawLoadOrder.Any(m => m.ModKey == s.RecordID.FormKey.ModKey),
                    IDType.ModKey => !Global.Game.State.RawLoadOrder.Any(m => m.ModKey == s.RecordID.ModKey),
                    IDType.FormID => true,
                    _ => false,
                };

                if (remove)
                {
                    if (s.Logic == Logic.ALL)
                    {
                        Global.Logger.WriteLog(LogLevel.Trace, Helpers.LogType.CONFIG, "KID Entry: Ignoring as include + entry pointing to mod that is not in load order.", ClassLogCode);
                        return null;
                    }

                    _ = strings.Remove(s);
                }
                else if (s.RecordID.Type is IDType.FormID or IDType.Invalid)
                {
                    return null;
                }
            }

            if (strings.Count == 0)
            {
                Global.Logger.WriteLog(LogLevel.Trace, Helpers.LogType.CONFIG, "KID Entry: Ignoring as all string entries pointed to mods not in load order.", ClassLogCode);
                return null;
            }

            return new KidRule(kidIniLine.Type, keyword, [.. strings.Where(r => r.Logic == Logic.ALL).Select(r => r.RecordID)], [.. strings.Where(r => r.Logic == Logic.ANY).Select(r => r.RecordID)], [.. strings.Where(r => r.Logic == Logic.NOT).Select(r => r.RecordID)]);
        }

        public static List<(Logic Logic, RecordID RecordID)> ParseStrings (string[] strings)
        {
            List<(Logic Logic, RecordID RecordID)> recordIDs = [];
            foreach (string str in strings)
            {
                if (str.Contains('+'))
                {
                    foreach (string s in str.Split('+').Select(s => s.Trim()).Where(s => s.Length != 0))
                        recordIDs.Add((Logic.ALL, SynthCommon.ConvertToBethesdaID(s, Global.Game.FormIDToFormKeyConverter)));
                }
                else
                {
                    switch (str[0])
                    {
                        case '-':
                            recordIDs.Add((Logic.NOT, SynthCommon.ConvertToBethesdaID(str[1..], Global.Game.FormIDToFormKeyConverter)));
                            break;

                        case '*':
                        default:
                            recordIDs.Add((Logic.ANY, SynthCommon.ConvertToBethesdaID(str, Global.Game.FormIDToFormKeyConverter)));
                            break;
                    }
                }
            }

            return recordIDs;
        }

        public override bool GetIndexableData (out List<RecordID> include, out List<RecordID> exclude)
        {
            _ = base.GetIndexableData(out include, out exclude);

            if (AllStrings.Count != 0)
            {
                // Only need to add one as if it doesn't match doesn't matter if others do
                include.Add(AllStrings.First());
                FullyIndexed = false;
            }

            foreach (var id in AnyStrings)
                include.Add(id);

            foreach (var id in NotStrings)
                exclude.Add(id);

            return include.Count != 0 || exclude.Count != 0;
        }

        public override bool Matches (ProcessingKeys proKeys)
            => proKeys.Record is IKeywordedGetter keywordedGetter // No use if record does have keywords property
            && base.Matches(proKeys)  // Base rule does the record type validations
            && (!keywordedGetter.Keywords?.Any(r => r.FormKey == Keyword.FormKey) ?? true) // Check doesn't already contain keyword
            && !NotStrings.Any(recordID => recordID.Equals(proKeys.Record, RecordID.EqualsOptions.All, Global.Game.State.LinkCache, RecordID.Field.None))
            && AllStrings.All(recordID => recordID.Equals(proKeys.Record, RecordID.EqualsOptions.All, Global.Game.State.LinkCache, RecordID.Field.None))
            && (AnyStrings.Count == 0 || AnyStrings.Any(recordID => recordID.Equals(proKeys.Record, RecordID.EqualsOptions.All, Global.Game.State.LinkCache, RecordID.Field.None)));
    }
}