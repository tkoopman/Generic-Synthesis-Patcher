## About

Generic Synthesis Patcher uses JSON configuration files to apply patches to many different record types.
It has a number of ways to filter for records to apply actions to, including matching against most implemented fields, and even using RegEx against strings or Editor IDs and also 3 main actions, fill, forward and merge.
Rules can even be grouped to share some common filters, or stop processing rules after one has matched.

Once a record has matched a rule following actions could be applied:
- Fill action will apply static changes to the winning record. Could be used to rename items, or add / remove keywords for example.
- Forward action will find matching parent record and forward only the selected fields to the winning record.  
  This is useful when you have multiple patches for the one record all for different fields.
  It has a few extra modes for when forwarding lists, like Container/Outfit Items.
- Merge action is the newest to be added. This would be similar to bash, but with more control, like using filters to only merge some record, only merging select fields, or excluding a plug-in from a merge.

Please see [Documentation](https://tkoopman.github.io/Generic-Synthesis-Patcher/) and [Examples](Examples/) for more details.

While this is not intended to replace more specific / complex Synthesis patchers, it can do a lot of more generic changes and fixes to your load order, or even be used post bash to assist in fixing anything missed by bash. 
A number of current patchers would however fit into this generic category.  
I have only tested this with latest Skyrim SE however it should work on other versions.

## Links
[Nexus Link](https://www.nexusmods.com/skyrimspecialedition/mods/130978)  
[Documentation](https://tkoopman.github.io/Generic-Synthesis-Patcher/)

### Prerequisites

This of course requires [Synthesis](https://github.com/Mutagen-Modding/Synthesis)

## Bugs, Requests and Contributions

Please log any bugs or requests you may have via [GitHub Issues](https://github.com/tkoopman/Generic-Synthesis-Patcher/issues) or over on Nexus.  
While I make no guarantee to fixing or implementing new requests due to other commitments, I will try, especially fixing bugs.  
Also if you want to contribute please do, even if you don't known how to program, just improving my awful documentation would help others.

## Configuration

Config files are to be located in a sub-folder of the game's Data folder, called "GSP". This directory can be changed in settings.  
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

