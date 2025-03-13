## About

SynthOutfits can create new LeveledItem and Outfit records, and assign them to NPC's Default or Sleeping outfits, and/or add to Spell Perk Item Distributor (SPID) file.  
SynthOutfits keeps track of IDs assigned in previous runs to make sure the same FormKey is used each time, even if you add/remove configurations.

## Links
[Examples](./Examples/SynthOutfits/)

## Prerequisites

- [Synthesis](https://github.com/Mutagen-Modding/Synthesis)
- [Spell Perk Item Distributor (SPID)](https://www.nexusmods.com/skyrimspecialedition/mods/36869)

## Configuration file format

By default SynthOutfits will look for a sub-folder called SynthOutfits in the Skyrim Data folder for all JSON files to load.
Each JSON configuration file has the following structure:
```json
{
  "LeveledItems": [
    {
      "Name": "EditorID4LeveledItem1",
      "Flags": "UseAll", // Optional - Valid values are "CalculateFromAllLevelsLessThanOrEqualPlayer, "CalculateForEachItemInCount", "UseAll", "SpecialLoot"
      "ChanceNone": "10%", // Optional - can be a percent form like this. Default: 0%
      "SkipIfMissing": "Any", // Optional - Valid values are "Any" (Default), "All", "Never". Will skip adding this if entries not found in load order, based on value.
      "Entries": [  // List of leveled item entries - can be FormKey or EditorID
        "123456:FormKey.esp",
        "EditorID"
      ]
    },
    {
      "Name": "EditorID4LeveledItem2",
      "Flags": "UseAll",
      "ChanceNone": 0.1,  // Optional - can be a decimal like this. This is equivalent to 10%
      "Entries": [
        "[Lv5] 1x 123456:FormKey.esp", // Optional - Can include Level and Count information in this format.
        "[Lv5] EditorID" // Optional - Can exclude Level or Count if you want the default value of 1.
      ]
    },
    {
      "Name": "EditorID4LeveledItemAll",
      "Flags": [ "CalculateFromAllLevelsLessThanOrEqualPlayer", "CalculateForEachItemInCount" ], // Optional - Can have multiple flags
      "Entries": [
        "EditorID4LeveledItem1", // Must use EditorID when referencing other records created by SynthOutfits
        "EditorID4LeveledItem2"
      ]
    }
  ],
  "Outfits": [
    {
      "Name": "EditorID4Outfit",
      "Items": [ "EditorID4LeveledItemAll" ], // List of items to include in the outfit. Can be FormKey or EditorID but if referencing a LeveledItem created by SynthOutfits, it must be the EditorID
      "SkipIfMissing": "Any", // Optional - Valid values are "Any" (Default), "All", "Never"
      "DefaultOutfit": [ // List of NPC records to update the Default Outfit on to point to this outfit if created.
        "ABC123:Dragonborn.esm" // Can have comments
      ],
      "SleepingOutfit": [ // List of NPC records to update the Sleeping Outfit on to point to this outfit if created.
        "321ABC:Dragonborn.esm" // Can have comments
      ],
      "SPID": [ "StringFilters|FormFilters|LevelFilters|TraitFilters|CountOrPackageIndex|Chance" ] // SPID entry to add to INI if this outfit is created. Excludes the starting FormType=FormOrEditorID| part as that is automatically added.
    }
  ]
}
```

## Bugs, Requests and Contributions

Please log any bugs or requests you may have via [GitHub Issues](https://github.com/tkoopman/Generic-Synthesis-Patcher/issues) or over on the appropriate Nexus page.  
While I make no guarantee to fixing or implementing new requests due to other commitments, I will try, especially fixing bugs.  
Also if you want to contribute please do, even if you don't known how to program, just improving my awful documentation would help others.
