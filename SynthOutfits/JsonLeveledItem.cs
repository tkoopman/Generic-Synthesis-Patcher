using GSPShared;
using GSPShared.JsonConverters;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

using Noggog;

using SynthOutfits.JsonConverters;

namespace SynthOutfits
{
    public struct JsonLeveledItemEntry : IEquatable<JsonLeveledItemEntry>
    {
        public short Count { get; set; }
        public string ID { get; set; }
        public short Level { get; set; }

        public override readonly bool Equals (object? obj) => obj is JsonLeveledItemEntry entry && Equals(entry);

        public readonly bool Equals (JsonLeveledItemEntry other) => Count == other.Count && string.Equals(ID, other.ID, StringComparison.OrdinalIgnoreCase) && Level == other.Level;

        public override readonly int GetHashCode () => HashCode.Combine(Count, ID.GetHashCode(StringComparison.OrdinalIgnoreCase), Level);
    }

    [JsonObject(MemberSerialization.OptIn)]
    public partial class JsonLeveledItem (string editorID)
    {
        [JsonProperty]
        [JsonConverter(typeof(PercentConverter))]
        public Percent ChanceNone { get; set; }

        [JsonProperty(propertyName: "Name", Required = Required.Always)]
        public string EditorID { get; } = editorID.IsValidEditorID() ? editorID : throw new ArgumentException($"Invalid EditorID: {editorID}", nameof(EditorID));

        /// <summary>
        ///     List of items to add to the leveled list.
        ///
        ///     Format: [Lv{Level}] {count}x {FormID or EditorID}
        ///     - Only the FormID or EditorID is required.
        ///     - 1 is default for both Level and count.
        ///     - Lv is optional. Just included to make same as format seen in xEdit.
        ///     - Spaces are optional.
        ///
        ///     Both the below examples are valid and equal to each other.
        ///     - Example 1: [Lv1] 1x LItemEnchIronSword
        ///     - Example 2: LItemEnchIronSword
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(JsonLeveledItemEntriesConverter))]
        public List<JsonLeveledItemEntry> Entries { get; set; } = [];

        [JsonProperty]
        [JsonConverter(typeof(FlagConverter))]
        public LeveledItem.Flag Flags { get; set; }

        /// <summary>
        ///     If entries that are to be added to LeveledItem are missing, should the LeveledItem
        ///     be skipped?
        ///     - Any (Default): Skip if any entry is missing.
        ///     - All: Only skip if all entries are missing which would make an empty LeveledItem.
        ///     - Never: Never skip the LeveledItem.
        /// </summary>
        [JsonProperty]
        public SkipIfMissing SkipIfMissing { get; set; }

        /// <summary>
        ///     Stores the config file this was loaded from.
        /// </summary>
        internal string? FromFile { get; } = JsonConfig.CurrentFile;

        /// <summary>
        ///     Basic checks to ensure the outfit record looks valid. Doesn't actually check if
        ///     items referenced are valid, just that they have a valid FormID or EditorID format.
        /// </summary>
        public void BasicChecks ()
        {
            if (Entries.Count == 0 && SkipIfMissing != SkipIfMissing.Never)
                throw new InvalidDataException($"LeveledItem {EditorID} from {FromFile}, contains no entries");

            foreach (var entry in Entries)
            {
                if (Common.TryConvertToSkyrimID(entry.ID, out _, out _) == SkyrimIDType.Invalid)
                    throw new InvalidDataException($"LeveledItem {EditorID} from {FromFile}, contains invalid FormKey or EditorID: {entry.ID}");
            }
        }

        public LeveledItem? ToLeveledItem ()
        {
            var leveledItem = new LeveledItem(new FormKey(Program.State.PatchMod.ModKey, Program.GetFormID(EditorID)), Program.State.GameRelease.ToSkyrimRelease())
            {
                EditorID = EditorID,
                Flags = Flags,
                ChanceNone = ChanceNone,
            };

            leveledItem.Entries ??= [];

            foreach (var data in Entries)
            {
                var entry = new LeveledItemEntry();
                entry.Data ??= new LeveledItemEntryData();
                entry.Data.Count = data.Count;
                entry.Data.Level = data.Level;

                if (!Common.TryGetRecord<IItemGetter>(data.ID, Program.State.LinkCache, out var record))
                {
                    if (SkipIfMissing == SkipIfMissing.Any)
                    {
                        Console.WriteLine($"Skipping LeveledItem: {EditorID}. Record not found: {data.ID}. Config file: {FromFile}.");
                        return null;
                    }
                    else
                    {
                        Console.WriteLine($"Skipping adding {data.ID} to LeveledItem: {EditorID}. Record not found. Config file: {FromFile}.");
                    }
                }
                else
                {
                    entry.Data.Reference = record.ToLink();
                    leveledItem.Entries.Add(entry);
                }
            }

            if (SkipIfMissing != SkipIfMissing.Never && leveledItem.Entries.Count == 0)
            {
                Console.WriteLine($"Skipping LeveledItem: {EditorID}. No valid entries found. Config file: {FromFile}.");
                return null;
            }

            return leveledItem;
        }
    }

    internal class JsonLeveledItemKey : IEqualityComparer<JsonLeveledItem>
    {
        public bool Equals (JsonLeveledItem? x, JsonLeveledItem? y) => string.Equals(x?.EditorID, y?.EditorID, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode (JsonLeveledItem obj) => obj.EditorID.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}