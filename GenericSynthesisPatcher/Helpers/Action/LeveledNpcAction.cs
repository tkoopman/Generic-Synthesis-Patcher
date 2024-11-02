using GenericSynthesisPatcher.Json.Data.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class LeveledNpcAction : LeveledEntryAction<LeveledNpcData, INpcSpawnGetter, LeveledNpcEntry>
    {
        public static readonly LeveledNpcAction Instance = new();
        private const int ClassLogCode = 0x19;

        public override LeveledNpcEntry? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not ILeveledNpcEntryGetter sourceRecord)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to create entry data?", logLevel: LogLevel.Error);
                return null;
            }

            LeveledNpcEntry entry = new();

            if (sourceRecord.Data != null)
            {
                entry.Data = new LeveledNpcEntryData
                {
                    Reference = sourceRecord.Data.Reference.FormKey.ToLink<INpcSpawnGetter>(),
                    Level = sourceRecord.Data.Level,
                    Count = sourceRecord.Data.Count,
                    Unknown = sourceRecord.Data.Unknown,
                    Unknown2 = sourceRecord.Data.Unknown2
                };
            }

            if (sourceRecord.ExtraData != null)
            {
                entry.ExtraData = CreateExtraData(sourceRecord);
            }

            return entry;
        }

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
                    => left is ILeveledNpcEntryGetter l
            && right is ILeveledNpcEntryGetter r
            && l.Data?.Count == r.Data?.Count
            && l.Data?.Level == r.Data?.Level
            && l.Data?.Reference.FormKey == r.Data?.Reference.FormKey
            && l.ExtraData?.ItemCondition == r.ExtraData?.ItemCondition
            && (l.ExtraData?.Owner.Equals(r.ExtraData?.Owner) ?? (l.ExtraData?.Owner == null && r.ExtraData?.Owner == null));

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from) => from is ILeveledNpcEntryGetter record ? (record.Data?.Reference.FormKey ?? default) : throw new ArgumentNullException(nameof(from));

        public override string ToString (IFormLinkContainerGetter source) => source is ILeveledNpcEntryGetter entry ? $"[Lvl{entry.Data?.Level}] {entry.Data?.Count}x{entry.Data?.Reference.FormKey}" : throw new InvalidCastException();
    }
}