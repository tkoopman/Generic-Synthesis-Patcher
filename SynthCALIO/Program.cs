﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using CsvHelper;

using GSPShared;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

using Noggog;

namespace SynthCALIO
{
    public partial class Program
    {
        internal static Lazy<Settings> Settings = null!;
        internal static IPatcherState<ISkyrimMod, ISkyrimModGetter> State = null!;

        private static readonly CsvHelper.Configuration.CsvConfiguration csvConfig = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = "\t",
            DetectDelimiter = true,
            TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
            IgnoreBlankLines = true,
            BadDataFound = null,
            AllowComments = true,
            Comment = '#',
        };

        private static JsonConfig jsonConfig = null!;
        private static uint nextFormID = 0;
        private static FilePath spidFile;
        public static UVDictionary<uint, string> FormIDCache { get; set; } = new(null, StringComparer.OrdinalIgnoreCase);
        public static UVDictionary<uint, string> ThisRunsCache { get; set; } = new(null, StringComparer.OrdinalIgnoreCase);

        public static uint GetFormID (string editorID)
        {
            if (FormIDCache.TryGetKey(editorID, out uint formID))
            {
                return ThisRunsCache.TryAdd(formID, editorID) ? formID : throw new InvalidDataException($"Failed to add {editorID} ({formID}) to cache");
                ;
            }
            else
            {
                uint id = nextFormID++;
                return FormIDCache.TryAdd(id, editorID) && ThisRunsCache.TryAdd(id, editorID) ? id : throw new InvalidDataException($"Failed to add {editorID} ({id}) to cache");
            }
        }

        public static async Task<int> Main (string[] args) => await SynthesisPipeline.Instance
            .SetAutogeneratedSettings(nickname: "SynthCALIO Settings", path: "settings.json", out Settings)
            .AddRunnabilityCheck(loadConfigAndCheckRunnability)
            .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
            .SetTypicalOpen(GameRelease.SkyrimSE, "SynthCALIO.esp")
            .Run(args);

        public static void RunPatch (IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            State = state;
            if (State.PatchMod.EnumerateMajorRecords().Any())
                throw new InvalidOperationException("SynthCALIO must be the first patch in a Synthesis' Group.");

            loadFormIDCache();
            if (Settings.Value.UpdateCache)
                generateFormIDCache();

            Console.WriteLine($"Loaded {FormIDCache.Count} FormIDs from cache");
            nextFormID = FormIDCache.Keys.Count == 0 ? State.PatchMod.NextFormID : FormIDCache.Keys.Max() + 1;
            using var writer = new StreamWriter(spidFile, false);
            writer.WriteLine("; Generated by SynthCALIO");

            createLeveledItems(writer);
            createOutfits(writer);

            writer.Close();
            writeFormIDCache();
        }

        /// <summary> Similar to Common.TryGetRecord<> but first checks cache from this run, to make
        /// sure if it was created in this run we use that entry and don't accidentally find some
        /// other random entry. </summary>
        public static bool TryGetRecord<T> (string id, [NotNullWhen(true)] out T? record)
            where T : class, IMajorRecordQueryableGetter
        {
            record = null;

            return Common.TryConvertToSkyrimID(id, out var formKey, out string editorID) switch
            {
                SkyrimIDType.FormKey => State.LinkCache.TryResolve<T>(formKey, out record),
                SkyrimIDType.EditorID => ThisRunsCache.TryGetKey(editorID, out uint formID) ? State.LinkCache.TryResolve<T>(new FormKey(State.PatchMod.ModKey, formID), out record) : State.LinkCache.TryResolve<T>(editorID, out record),
                _ => false,
            };
        }

        private static void createLeveledItems (StreamWriter writer)
        {
            bool addedSPID = false;

            // Sort LeveledItem list so that we create any that contain entries to others after the
            // others created
            List<JsonLeveledItem> sorted = [];
            var linked = jsonConfig.LeveledItems;

            while (linked.Count != 0)
            {
                var referenced = linked
                    .SelectMany(x => x.Entries) // Return all entries from all LeveledItems
                    .Select(x => x.ID) // Just need ID from each entry
                    .Where(x => !x.Contains(':')) // Exclude FormKey entries
                    .ToHashSet(StringComparer.OrdinalIgnoreCase); // Only unique

                var split = linked.GroupBy(x => referenced.Contains(x.EditorID));
                var notLinked = (split.FirstOrDefault(x => !x.Key)?.ToList()) ?? throw new InvalidDataException("Circular reference found in LeveledItems");

                notLinked.Sort((lhs, rhs) => string.Compare(rhs.EditorID, lhs.EditorID, StringComparison.OrdinalIgnoreCase));
                sorted.AddRange(notLinked);
                linked = (split.FirstOrDefault(x => x.Key)?.ToList()) ?? [];
            }

            sorted.Reverse();
            foreach (var jsonLeveledItem in sorted)
            {
                var lvlItem = jsonLeveledItem.ToLeveledItem();

                if (lvlItem is not null)
                {
                    State.PatchMod.LeveledItems.Add(lvlItem);
                    Console.WriteLine($"Added LeveledItem: {lvlItem.EditorID} [{lvlItem.FormKey}]");

                    foreach (string spid in jsonLeveledItem.SPID)
                    {
                        if (!addedSPID)
                        {
                            writer.WriteLine();
                            writer.WriteLine("; Items");
                            addedSPID = true;
                        }

                        writer.WriteLine($"Item = 0x{lvlItem.FormKey.ID:X}~{lvlItem.FormKey.ModKey}|{spid}");
                    }
                }
            }
        }

        /// <summary>
        ///     Creates and assigns outfits to NPCs.
        /// </summary>
        private static void createOutfits (StreamWriter writer)
        {
            bool addedSPID = false;

            jsonConfig.Outfits.Sort((lhs, rhs) => string.Compare(lhs.EditorID, rhs.EditorID, StringComparison.OrdinalIgnoreCase));

            foreach (var jsonOutfit in jsonConfig.Outfits)
            {
                var outfit = jsonOutfit.ToOutfit();

                if (outfit is not null)
                {
                    State.PatchMod.Outfits.Add(outfit);
                    Console.WriteLine($"Added outfit: {outfit.EditorID} [{outfit.FormKey}]");

                    foreach (string npcOutfit in jsonOutfit.DefaultOutfit)
                    {
                        if (Common.TryGetRecordContext<INpc, INpcGetter>(npcOutfit, State.LinkCache, out var context))
                        {
                            var patchRecord = context.GetOrAddAsOverride(State.PatchMod);
                            patchRecord.DefaultOutfit = outfit.ToNullableLink();

                            Console.WriteLine($"    Added as DefaultOutfit to {patchRecord.Name} [{patchRecord.FormKey}]");
                        }
                        else
                        {
                            Console.WriteLine($"    Failed to add as DefaultOutfit to {npcOutfit}");
                        }
                    }

                    foreach (string npcOutfit in jsonOutfit.SleepingOutfit)
                    {
                        if (Common.TryGetRecordContext<INpc, INpcGetter>(npcOutfit, State.LinkCache, out var context))
                        {
                            var patchRecord = context.GetOrAddAsOverride(State.PatchMod);
                            patchRecord.SleepingOutfit = outfit.ToNullableLink();

                            Console.WriteLine($"    Added as SleepingOutfit to {patchRecord.Name} [{patchRecord.FormKey}]");
                        }
                        else
                        {
                            Console.WriteLine($"    Failed to add as SleepingOutfit to {npcOutfit}");
                        }
                    }

                    foreach (string spid in jsonOutfit.SPID)
                    {
                        if (!addedSPID)
                        {
                            writer.WriteLine();
                            writer.WriteLine("; Outfits");
                            addedSPID = true;
                        }

                        writer.WriteLine($"Outfit = 0x{outfit.FormKey.ID:X}~{outfit.FormKey.ModKey}|{spid}");
                        addedSPID = true;
                    }
                }
            }

            if (addedSPID)
                writer.WriteLine();
        }

        /// <summary>
        ///     Update loaded FormID cache with entries from the current version of this patcher's
        ///     mod. Should override any existing EditorID entries with the new FormID.
        /// </summary>
        private static void generateFormIDCache ()
        {
            var path = State.DataFolderPath.GetFile(State.OutputPath.Name);

            if (!path.Exists)
            {
                Console.WriteLine($"Could not find mod: {path.Name}");
                return;
            }

            Console.WriteLine($"Attempting to generate output cache from {path.Name}");

            var mod = SkyrimMod.CreateFromBinary(path, State.GameRelease.ToSkyrimRelease());

            foreach (var record in mod.EnumerateMajorRecords())
            {
                if (record.EditorID.IsNullOrWhitespace() || !record.FormKey.ModKey.Equals(mod.ModKey))
                    continue;

                if (!FormIDCache.TryUpdateOrAdd(record.FormKey.ID, record.EditorID))
                    throw new InvalidDataException($"Failed to add FormID {record.EditorID} ({record.FormKey}) to cache");
            }
        }

        /// <summary>
        ///     Load and check the validity of the configuration files. Doesn't check for listed
        ///     EditorIDs or FormKeys actually existing, just that the config file format is
        ///     correct.Should throw exception if any issues are found to stop Synthesis from
        ///     starting the actual patch run, as this a Runnability Check for this patcher.
        /// </summary>
        private static void loadConfigAndCheckRunnability (IRunnabilityState state)
        {
            jsonConfig = new JsonConfig();

            jsonConfig.LoadConfigurationFiles(state);

            DirectoryPath spidDir = string.IsNullOrWhiteSpace(Settings.Value.Output) ? state.DataFolderPath : Settings.Value.Output;
            if (!spidDir.Exists)
                throw new FileNotFoundException("Unable to find output path.", spidDir);

            spidFile = spidDir.GetFile($"{state.ExtraSettingsDataPath?.Name.NameWithoutExtension}_DISTR.ini");

            Console.WriteLine($"SPID output file: {spidFile}");
        }

        /// <summary>
        ///     Loads the FormID cache from the CSV data file saved in previous runs.
        /// </summary>
        private static void loadFormIDCache ()
        {
            if (State.ExtraSettingsDataPath is null)
                return;

            string path = Path.Combine(State.ExtraSettingsDataPath, "formIDCache.txt");

            if (!Path.Exists(path))
            {
                Console.WriteLine($"Could not find FormID cache: {path}");
                return;
            }

            Console.WriteLine($"Loading FormID cache from {path}");

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                while (csv.Read())
                {
                    _ = csv.TryGetField(0, out string? key) && csv.TryGetField(1, out string? value) && !string.IsNullOrWhiteSpace(value)
                        ? FormIDCache.TryAdd(Convert.ToUInt32(key, 16), value)
                        : throw new InvalidDataException("FormID cache contains invalid entry.\n" + csv.Parser.RawRow);
                }
            }
        }

        private static void writeFormIDCache ()
        {
            if (State.ExtraSettingsDataPath is null)
                return;

            if (!Path.Exists(State.ExtraSettingsDataPath))
            {
                Console.WriteLine($"Could not find data folder: {State.ExtraSettingsDataPath}");
                return;
            }

            string path = Path.Combine(State.ExtraSettingsDataPath, "formIDCache.txt");

            Console.WriteLine($"Writing FormID cache to {path}");
            using (var writer = new StreamWriter(path, false))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                foreach (var entry in FormIDCache)
                {
                    csv.WriteField(entry.Key.ToString("X3"));
                    csv.WriteField(entry.Value);
                    csv.NextRecord();
                }
            }
        }
    }
}