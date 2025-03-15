## About

SynthCALIO can create new LeveledItem and Outfit records, and assign them to NPC's Default or Sleeping outfits, and/or add to Spell Perk Item Distributor (SPID) file.  
SynthCALIO keeps track of IDs assigned in previous runs to make sure the same FormID is used each time, even if you add/remove configurations.

## Links
[Examples](./Examples/SynthCALIO/)

## Prerequisites

- [Synthesis](https://github.com/Mutagen-Modding/Synthesis)
- [Spell Perk Item Distributor (SPID)](https://www.nexusmods.com/skyrimspecialedition/mods/36869)

## Using

By default, the JSON configuration files should be added into a sub-folder SynthCALIO in your Skyrim's data folder. This way you can add these configuration files like normal mods in your mod manager of choice.
Location can be overridden in the Synthesis settings of SynthCALIO.

Next you will need a working Synthesis. See there documentation for getting this setup.

You can then add SynthCALIO, however if adding to a Synthesis group that contains other patchers, SynthCALIO MUST be the first patcher in the group. This is so it doesn't get possible FormID conflicts.

## How SynthCALIO maintains FormID's

There are 2 methods used.  
First, at the end of each run it will save a file formIDCache.txt to the Synthesis data folder. This contains a mapping of each known FormID to EditorID. This is then loaded at the start of every run.
As this file will not remove entries just because they may not of been exported this run (maybe you removed a JSON config file), it will keep those IDs available unless you manually clear the file.
You may delete this cache if you need to reset it, but if next method is enabled it will be repopulated with any entries that exist in the current version. So disable it as well to fully reset FormIDs.

Then optionally it will also read the current SynthCALIO's mod for all IDs to EditorID mappings it contains and update/add to the entries loaded from formIDCache.txt.
In most situations this should never be required as the cache file should contain everything, but this is here as backup.
You can disable this in the Synthesis SynthCALIO Settings, but is enabled by default.

This does mean that if you rename an entry in a JSON config file, it will get a new FormID as the EditorID has changed.

## Configuration file format

By default SynthCALIO will look for a sub-folder called SynthCALIO in the Skyrim Data folder for all JSON files to load.
Each JSON configuration file has the following structure:
```json
{
  "LeveledItems": [
    {
      "Name": "EditorID4LeveledItem1", // Mandatory - Must be valid EditorID and unique across all JSON config files
      "Flags": "UseAll", // Valid values are "CalculateFromAllLevelsLessThanOrEqualPlayer, "CalculateForEachItemInCount", "UseAll", "SpecialLoot"
      "ChanceNone": "10%", // Can be a percent form like this. Default: 0%
      "SkipIfMissing": 1, // Number of entries that are unable to be found, before it will skip adding this LeveledItem. Default 1 (Any), 0 = Never Skip, -1 = Skip if all missing
      "Entries": [  // List of leveled item entries - can be FormKey or EditorID
        "123456:FormKey.esp",
        "EditorID"
      ]
    },
    {
      "Name": "EditorID4LeveledItem2",
      "Flags": "UseAll",
      "ChanceNone": 0.1,  // Can be a decimal like this. This is equivalent to 10%
      "Entries": [
        "[Lv5] 1x 123456:FormKey.esp", // Can include Level and Count information in this format.
        "[Lv5] EditorID" // Can exclude Level or Count if you want the default value of 1.
      ]
    },
    {
      "Name": "EditorID4LeveledItemAll",
      "Flags": [ "CalculateFromAllLevelsLessThanOrEqualPlayer", "CalculateForEachItemInCount" ], // Can have multiple flags
      "Entries": [
        "EditorID4LeveledItem1", // Must use EditorID when referencing other records created by SynthCALIO
        "EditorID4LeveledItem2"
      ],
      "SPID": [ "StringFilters|FormFilters|LevelFilters|TraitFilters|CountOrPackageIndex|Chance" ] // SPID entry to add to INI, unless skipped. Excludes the starting Item=FormOrEditorID| part as that is automatically added.
    }
  ],
  "Outfits": [
    {
      "Name": "EditorID4Outfit",
      "Items": [ "EditorID4LeveledItemAll" ], // List of items to include in the outfit. Can be FormKey or EditorID but if referencing a LeveledItem created by SynthCALIO, it must be the EditorID
      "SkipIfMissing": 1, // Number of items that are unable to be found, before it will skip adding this Outfit. Default 1 (Any), 0 = Never Skip, -1 = Skip if all missing
      "DefaultOutfit": [ // List of NPC records to update the Default Outfit on to point to this outfit if created.
        "ABC123:Dragonborn.esm" // Can have comments
      ],
      "SleepingOutfit": [ // List of NPC records to update the Sleeping Outfit on to point to this outfit if created.
        "NPCEditorID" // Can use EditorID
      ],
      "SPID": [ 
        "StringFilters|FormFilters|LevelFilters|TraitFilters|CountOrPackageIndex|Chance", // SPID entry to add to INI, unless skipped. Excludes the starting Outfit=FormOrEditorID| part as that is automatically added.
        "SleepOutfit = |StringFilters|FormFilters|LevelFilters|TraitFilters|CountOrPackageIndex|Chance" // Can include FormType if you don't want Outfit, but FormOrEditorID must still be empty, so first | should be straight after =.
      ]
    }
  ]
}
```

## Bugs, Requests and Contributions

Please log any bugs or requests you may have via [GitHub Issues](https://github.com/tkoopman/Generic-Synthesis-Patcher/issues) or over on the appropriate Nexus page.  
While I make no guarantee to fixing or implementing new requests due to other commitments, I will try, especially fixing bugs.  
Also if you want to contribute please do, even if you don't known how to program, just improving my awful documentation would help others.
