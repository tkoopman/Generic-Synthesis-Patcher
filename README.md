<!--- cSpell:enable --->
## About

Generic Synthesis Patcher uses JSON configuration files to apply patches to many different record types. It has a number of ways to filter for records to match and also 2 main actions.

Fill action will apply static changes to the winning record. Could be used to rename items, or add / remove keywords.

Forward action will find matching parent record and forward only the selected fields to the winning record. This is useful when you have multiple patches for the one record all for different fields.

Note: This is not meant to be a replacer for more specific Synthesis patchers. I wrote this to help when a Synthesis patcher for what I need didn't exist and it was just a simple fill or forward.
Also to replace some SkyPatcher patches as personally I don't like to use them as when I am manually resolving conflicts, it's hard to remember / trust I'm getting what I want when I can't see the result in xEdit.

I have only tested this with latest Skyrim SE however it should work on other versions. Fallout however I am not sure on. I am guessing I would probably need to make some modifications to properly work for Fallout.

### Prerequisites

This of course requires [Synthesis](https://github.com/Mutagen-Modding/Synthesis)

## Bugs, Requests and Contributions

Please log any bugs or requests you may have via [GitHub Issues](https://github.com/tkoopman/Generic-Synthesis-Patcher/issues).  
While I make no guarantee to fixing or implementing new requests due to other commitments, I will try especially fixing bugs.  
Also if you want to contribute please do, even if you don't known how to program, just improving my awful documentation would help others.

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

[Available Rule Settings](docs/Settings.md)