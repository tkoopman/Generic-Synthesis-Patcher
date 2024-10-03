<!--- cSpell:enable --->
<a id="readme-top"></a>

<!-- GETTING STARTED -->
## About

Generic Synthesis Patcher uses JSON configuration files to apply patches to many different record types. It has a number of ways to filter for records to match and also 2 main actions.

Fill action will apply static changes to the winning record. Could be used to rename items, or add / remove keywords.

Forward action will find matching parent record and forward only the selected fields to the winning record. This is useful when you have multiple patches for the one record all for different fields.

Note: This is not meant to be a replacer for more specific Synthesis patchers. I wrote this to help when no Synthesis patcher for what I need didn't exist and it was just a simple fill or forward.
Also to replace some SkyPatcher patches as personally I don't like to use them as when I am manually resolving conflicts, it hard to remember / trust you getting what I want when I can't see the result in xEdit.

### Prerequisites

This of course requires [Synthesis](https://github.com/Mutagen-Modding/Synthesis)

<!-- USAGE EXAMPLES -->
## Configuration

Config files are to be located in a subfolder of the game's Data folder, called "GSP". This directory can be changed in settings.  
x:\SteamLibrary\steamapps\common\Skyrim Special Edition\Data\GSP\  

If using MO2 or other mod manager this means you can install your configurations as a normal mod.

Configuration files in this directory must end in .json with the following format. See examples directory for real world examples.

    [
      {
        "priority": 0,
        "types": [ "AMMO" ],
        "editorID": "...",
        "formID": "123ABC:Skyrim.esm",
        "fill": {
          "field": "value",
          "keywords": [ "KeyWord", "-Keyword" ],
          "value": 100,
          "weight": 1.234
        },
        "forward": {
          "mod.esp": [ "fields" ]
        }
      }
    ]

### Configuration fields
**Priority**: All matching rules will be applied to a record in ascending priority. Default: 0  
Matching priority will be applied in random order so be carful.

**Base Filters**

At least 1 base filter must be applied. If multiple filters provided then the logic is **Type AND (EditorID OR FormID)**  
All filters can be either single value or array of possible values.

- **types**: List valid types for this rule. If no types provided then can match any type. Types are case insensitive.
- **editorID**: EditorID to match. Can be a regular expression if starts and ends with /. Example "editorID": "/.\*fur.\*/"
- **formID**: FormID to match in above format. Can exclude leading 0s.

**Exclude Filters**

These can be single values or array of values to exclude.

- **-editorID**: EditorID to exclude from matches. Can be a regular expression if starts and ends with /. Example "-editorID": "/.\*fur.\*/"
- **-formID**: FormID to exclude from matches in above format.
- **OnlyIfDefault**: False by default. If set to True will only apply action if the winning record's field value matches the original value set by the master record.  
Good for example if you want to forward one patches changes, but only if a later patch set the value back to the default, so protecting other winning patches.

**Advanced Filters**

- **InFaction**: Only valid if Type set to only NPC. NPC will need to belong to one of the listed factions. Factions listed by FormID.
- **InFactionAnd**: False by default. Setting to True will mean NPC must belong to all factions listed in InFaction.

**Actions**

At least 1 action must exist. If both provided then both will be applied.

- **Fill**: This will just apply the changes to all listed fields.
- **Forward**: This will forward fields from a parent that this winning record overrode.

#### Implemented Types
| Type | Synonyms       |
| ---- | -------------- |
| ALCH | Ingestible     |
| AMMO | Ammunition     |
| ARMO | Armor          |
| BOOK |
| CELL |
| FACT | Faction        |
| INGR | Ingredient     |
| KEYM | Key            |
| MISC | MiscItem       |
| NPC  |
| OTFT | Outfit         |
| SCRL | Scroll         |
| WEAP | Weapon         |

#### Implemented Fields *WIP*

Fields that are invalid for a record that matches your filter will just be ignored for those records.  
NOTE: Not all implemented fields listed here yet. Check Examples folder for more.

##### Name (String)
Pretty self-explanatory. Set the name of matching records.

##### Keywords (String or Array of strings)
Add (+) or remove (-) keywords depending on prefix. No prefix will default to adding that keyword.

##### Flags (String or Array of strings)
Add (+) or remove (-) flags depending on prefix. No prefix will default to adding that keyword.  
These are the Data Record Flags. Flags vary depending on record type.
- ALCH: NoAutoCalc, FoodItem, Medicine, Poison
- AMMO: IgnoresNormalWeaponResistance, NonBolt, NotPlayable
- BOOK: CantBeTaken
- FACT: HiddenFromPC, SpecialCombat, TrackCrime, IgnoreMurder, IgnoreAssault, IgnoreStealing, IgnoreTrespass, DoNotReportCrimesAgainstMembers, CrimeGoldUseDefaults, IgnorePickpocket, Vendor, CanBeOwner, IgnoreWerewolf
- INGR: NoAutoCalculation, FoodItem, ReferencesPersist

##### MajorFlags (String or Array of strings)
Major Flags. Same as Flags but this is the major record flags this time. Flags vary depending on record type.
- ALCH: Medicine
- AMMO: NonPlayable
- ARMO: NonPlayable, Shield
- KEYM: NonPlayable
- MISC: NonPlayable
- NPC: BleedoutOverride
- WEAP: NonPlayable

##### Value (Number)
Also pretty self-explanatory. Set the value of matching items.

##### Weight (Float / Decimal)
You guessed it, pretty self-explanatory. Set the weight of matching items.