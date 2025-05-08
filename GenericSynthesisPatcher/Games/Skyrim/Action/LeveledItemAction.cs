using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Json.Data.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Games.Skyrim.Action
{
    public class LeveledItemAction : LeveledEntryAction<LeveledItemData, IItemGetter, LeveledItemEntry>
    {
        public static readonly LeveledItemAction Instance = new();
        private const int ClassLogCode = 0x1A;

        public override LeveledItemEntry? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not ILeveledItemEntryGetter sourceRecord)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to create entry data?", logLevel: LogLevel.Error);
                return null;
            }

            LeveledItemEntry entry = new();

            if (sourceRecord.Data != null)
            {
                entry.Data = new LeveledItemEntryData
                {
                    Reference = sourceRecord.Data.Reference.FormKey.ToLink<IItemGetter>(),
                    Level = sourceRecord.Data.Level,
                    Count = sourceRecord.Data.Count,
                    Unknown = sourceRecord.Data.Unknown,
                    Unknown2 = sourceRecord.Data.Unknown2
                };
            }

            if (sourceRecord.ExtraData != null)
                entry.ExtraData = createExtraData(sourceRecord);

            return entry;
        }

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is ILeveledItemEntryGetter l
            && right is ILeveledItemEntryGetter r
            && l.Data?.Count == r.Data?.Count
            && l.Data?.Level == r.Data?.Level
            && l.Data?.Reference.FormKey == r.Data?.Reference.FormKey
            && l.ExtraData?.ItemCondition == r.ExtraData?.ItemCondition
            && (l.ExtraData?.Owner.Equals(r.ExtraData?.Owner) ?? l.ExtraData?.Owner == null && r.ExtraData?.Owner == null);

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from) => from is ILeveledItemEntryGetter record ? record.Data?.Reference.FormKey ?? default : throw new ArgumentNullException(nameof(from));

        public override string ToString (IFormLinkContainerGetter source) => source is ILeveledItemEntryGetter entry ? $"[LVL{entry.Data?.Level}] {entry.Data?.Count}x{entry.Data?.Reference.FormKey}" : throw new InvalidCastException();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Array of JSON objects containing Item Form Key/Editor ID and level/count data";
            example = $$"""
                        "{{propertyName}}": [{ "Item": "000ABC:Skyrim.esm", "Level": 36, "Count": 1 }]
                        """;

            return true;
        }
    }
}