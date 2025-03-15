using GSPShared;
using GSPShared.JsonConverters;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

using Noggog;

using SynthCALIO.JsonConverters;

namespace SynthCALIO
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
        ///     Number of entries, that should be added, but are missing, that will cause
        ///     LeveledItem to be skipped?
        ///     - 1 (Default): Skip if any entry is missing.
        ///     - 2+: Number of missing entries to trigger skip. If larger than number of entries in
        ///     list then will never skip.
        ///     - 0: Never Skip
        ///     - -1: Skip only if all entries are missing
        /// </summary>
        [JsonProperty]
        public int SkipIfMissing { get; set; } = SynthCALIO.SkipIfMissing.Any;

        /// <summary>
        ///     This is the list of SPID entries, starting from the StringFilters, that will be
        ///     added to the INI file. Everything before StringFilters will be filled in
        ///     automatically. As such it must contain 5 pipe (|) characters if provided.
        ///     Item=FormID|StringFilters|FormFilters|LevelFilters|TraitFilters|CountOrPackageIndex|Chance
        ///
        ///     Will only add SPID entries if the LeveledItem was added to the mod.
        /// </summary>
        [JsonProperty]
        public string[] SPID { get; set; } = [];

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
            if (Entries.Count == 0 && SkipIfMissing != SynthCALIO.SkipIfMissing.Never)
                throw new InvalidDataException($"LeveledItem {EditorID} from {FromFile}, contains no entries");

            foreach (var entry in Entries)
            {
                if (Common.TryConvertToSkyrimID(entry.ID, out _, out _) == SkyrimIDType.Invalid)
                    throw new InvalidDataException($"LeveledItem {EditorID} from {FromFile}, contains invalid FormKey or EditorID: {entry.ID}");
            }

            foreach (string item in SPID)
                _ = Program.formatSPID(item, FormKey.Null, "Test");
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
            int skipped = 0;
            foreach (var data in Entries)
            {
                var entry = new LeveledItemEntry();
                entry.Data ??= new LeveledItemEntryData();
                entry.Data.Count = data.Count;
                entry.Data.Level = data.Level;

                if (!Program.TryGetRecord<IItemGetter>(data.ID, out var record))
                {
                    if (++skipped == SkipIfMissing)
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

            if (SkipIfMissing == SynthCALIO.SkipIfMissing.All && leveledItem.Entries.Count == 0)
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