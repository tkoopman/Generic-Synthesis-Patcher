- [Rule Properties](#rule-properties)
    - [Filters](#filters)
        - [Basic](#basic-filters)
        - [Extra Filters](#extra-filters)
        - [Advanced Filters](#advanced-filters-matches)
    - [Actions](#actions)
        - [Fill](#fill)
        - [Forward](#forward)
        - [Merge](#merge)
    - [Debug / Trace Logging](#debug--trace-logging)

# Group / Rule Properties

Property names are case insensitive. If a property accepts multiple values as an array, you do not need to use the \[ ] to denote an array if only entering a single value.

> **<font color="green">Priority</font>**: All matching rules will be applied to a record in ascending priority. Default: 0  
Matching priority will be applied in random order so be carful if multiple rules edit the same field on a record.

## Filters

All rules must list, at least 1 record type, in the Types filter, either directly in the rule or via a group.  
All other filters are optional, but when using multiple filters record must match all.

### Filters for Groups or Rules

These filters can be used when creating a rule or group.

>**<font color="green">Types</font>**: List valid record types for this rule. Types are case insensitive.  
[List of implemented types](Types.md).

>**<font color="green">Masters</font>**: List of mod names ("Skyrim.esm"). Original record must come from one of these master mods to match.  
To change to a list of masters to exclude, use "!Masters" instead.

>**<font color="green">PatchedBy</font>**: List of mod names ("unofficial skyrim special edition patch.esp"). By default this is an OR check, so record must of been patched by one or more of these mods to match.  
Replace "PatchedBy" with
- "!PatchedBy": Exclude if matched by any listed mod.
- "^PatchedBy": XOR. Be patched by exactly one of the listed mods.
- "&PatchedBy": Change to AND check, so to match record, it must match all listed mods. When using AND each listed mod can be prefixed:
    - "!unofficial skyrim special edition patch.esp": to mean mod must not of been patched by this mod.

NOTE: PatchedBy will not match records that originate from a mod, only if patched by. Use Masters for that.  
So if listing in the same rule, the same single mod in both Masters and PatchedBy no records will ever match.

>**<font color="green">Patched</font>**: null (default), true or false. Checked if a previous rule has already applied a patch to this record.  
Using "Patched": false will mean this rule will only match if the record hasn't already been patched by another rule.  
Omit this or set to null to ignore patched state.  
Note: This doesn't care what field has been patched, just that a patch record has been created.

### Rule Only Filters

> **<font color="green">EditorID</font>**: EditorID(s) to match. Can be a regular expression if starts and ends with /. Example "EditorID": "/.\*fur.\*/"  
To change to a list of EditorID(s) to exclude, use "!EditorID" instead.

> **<font color="green">FormID</font>**: FormID(s) to match in the format "0123AB:Skyrim.esm". Can exclude leading 0s.  
To change to a list of FormID(s) to exclude, use "!FormID" instead.

>**<font color="green">OnlyIfDefault</font>**: False by default. If set to True will only apply action if the winning record's field value matches the original value set by the master record.
Good for example if you want to forward one patches changes, but only if a later patch set the value back to the default, so protecting other winning patches.  
Checks are per field, meaning one action may fail to apply due to the field it updating not matching, but other action on the same record applies as it does match.  
NOTE: Just because multiple fields are listed on a single fill/forward action, they are all still processed separately. No need to split into different rules.

### Rule Only - Field Matches

Using Matches you can filter on any [supported field](Fields.md).  
In general they follow the following rules:

- If you use one of these filters, any record that doesn't contain that field will never match the rule.
- If field is single value, like Name then:
    - +/!: You can add a prefix to these to say if they must be included or excluded from filter, however all entries should be the same prefix.
- If field is list of multiple values, like Keywords then:
    - &/|/^ Operator: You can change the default operation using a prefix on the field name. Default is OR (|). Can set to And (&) or XOR (^).
    - +/!: You can add a prefix to these to say if they must be included or excluded from filter. If using AND (&) each can be different prefix.
- Include (+) and OR (|) prefixes are default so no need to enter them however, can be used for unforeseen cases where the value you want to enter may start with a prefix value. Like trying to enter a negative number ("+-123" to allowed -123).
- If matching against a field that accepts a string or Editor ID you can use regular expression to match, like EditorID filter. If used with exclude operator the ! goes before the first /, like in the 4th example below.
- \- is same as ! prefix
- Just like other filters if you list multiple different fields, the record must match all.

#### Examples

    ... Single Value Match - Matches any record when keywords assigned to record contains this keyword.
    "Matches": { "Keywords": "ArmorMaterialLeather" },

    ... AND Match with exclude Example, so must contain all included keyworks, but none of the excluded keywords.
    "Matches": { "&Keywords": [ "ArmorMaterialLeather", "ArmorMaterialHide", "!WAF_ClothingCloak" ] },

    ... OR Not Match Example. Will match any record that doesn't contain any of these keywords.
    "Matches": { "Keywords": [ "!ArmorMaterialLeather", "!ArmorMaterialHide" ] },

    ... Exclude Regex Example - Will match any record where EditorID of the assigned InventoryArt, doesn't match the Regex.
    "Matches": { "InventoryArt": "!/^(?!.*(?:(?:Book)|(?:Journal)|(?:Tome))).*((?:Note)|(?:Scroll)|(?:Paper)|(?:Map)|(?:Recipe)).*$/" }

## Actions

At least 1 action should exist, under most circumstances (see SingleMatch Groups below), else what's the point of the rule. If multiple provided then multiple actions the processing order will be:

- Merge
- Forward - Should be self only if also using merge, else will overwrite merge.
- Fill - Could be used to add / remove extra stuff post Merge/Forward.

### Fill
This will just apply the changes to all listed fields. The most basic of action.  
If single value field, then will just overwrite field with what you put in fill action.  
If field selected is a <font color="blue">list</font> then:

- Fill will add / remove items from the list, based on prefix of the item.
- To clear all current values use null as first element in list.
- Surrounded multiple entries with \[ ]

    {
        "Types": "Ingestible",
        "FormID": "FAA:CookingAdventuresInSkyrim.esp",
        "Fill":
            {
                "Name" : "My Name for This",
                "Effects": [ null, { "effect": "002EE1:Update.esm" } ]
            }
    }

### Forward

This will forward fields from a parent that the current winning record.  
By default this is just a straight replace including if it is a list field like Items. It doesn't do any merging.

>**<font color="green">ForwardOptions</font>**: ForwardOptions can change how Forward actions work. Multiple options can be combined. Following are the valid options:  
- **<font color="green">Default</font>**: Will replace winning with value from forwarding mod as described above.  

- **<font color="green">SelfMasterOnly</font>**: This is only relevant when forwarding changes from lists of FormIDs. This includes Keywords.  
It will only add new FormIDs to lists if FormID is from the same mod that you are forwarding from.  
It will not remove any current items from winning records. If however it is combined with the Default option, then the first mod in the list will perform the Default forward, while all other listed mods will perform SelfMasterOnly.  
Below example would make sure any cloaks that "Cloaks - Dawnguard.esp" added to outfits would be added to winning records if it was removed by another patch.
However the couple of changes it makes like adding back "Vampire Boots" [00B5DE:Dawnguard.esm] that the original Cloaks.esp removed would not be forwarded.

        {
            "types": "Outfit",
            "Masters": "Dawnguard.esm",
            "ForwardOptions": ["SelfMasterOnly"],
            "Forward": { "Cloaks - Dawnguard.esp": "Items" }
        }

- **<font color="green">IndexedByField</font>**: This changes how the contents of the Forward action is defined.  
By default, it is "Mod.esp" : ["field1", ...].  
Setting this to true changes it to "field":["Mod1.esp", "Mod2.esp", ....].  
Depending on what you are doing one or the other may be more efficient for you to read. Inside the patcher both produce the same result.  
If used with ForwardOption of Default, as only a single mods value would be copied over it will pick the first mod from the list that contains this record.
Below example will forward record from PrvtI_HeavyArmory.esp unless it doesn't contain the record in which case Immersive Weapons.esp will be tried.

        {
            "types": [ "NPC" ],
            "OnlyIfDefault": true,
            "ForwardOptions": ["IndexedByField"],
            "Forward": { "Items": [ "PrvtI_HeavyArmory.esp", "Immersive Weapons.esp" ] }
        }

- **<font color="green">NonDefault</font>**: When performing a Default forward with multiple mods listed, will exclude picking a mod that would set the value back to it's original value defined in the master.  
Automatically adds Default and IndexedByField options.  
Ignored if used with SelfMasterOnly.

- **<font color="green">NonNull</font>**: When performing a Default forward with multiple mods listed, will exclude picking a mod that would set the value to null or equivalent (Empty string, 0).  
Automatically adds Default and IndexedByField options.  
Ignored if used with SelfMasterOnly.

- **<font color="green">Sort</font>**: Sorts mods listed by load order priority, with higher priority at the start. Useful when leaving mods empty, which would add every mod in load order to the list.  
Automatically adds IndexedByField option.

- **<font color="green">HPU</font>**: This one complex for me to explain in full. Firstly had troubles coming up with name so for now it is Highest Priority Unique (HPU).  
This could be seen as the "Merge" for non list fields, and on that HPU is invalid on list fields that implement the Merge action.  
Basically HPU will look at all values a record's field has had across all mods, and find the highest priority unique value. However it does respect defined Masters of mods.
So if a patch mod has defined Master(s) then patch record's value will still be used for those defined master(s) even if setting it back to previous value.  
This is mainly for record types that are often validly included in mods but not changed like Worldspace and Cell records.  
Automatically adds Default and IndexedByField options.

- **<font color="green">Random</font>**: For selecting a random mod to forward. If used with Sort, then mod order is sorted into a random order once and that order applied to all records for this field (Normal picking of which mod to use from the random order still applies).  
Without Sort then a random mod will be picked per record. If you have multiple forward fields with same list of mods, each field does random independently. 
Automatically adds Default and IndexedByField options.  
Crazy if used with SelfMasterOnly and Sort (Which mod is first and as such does the Default forward would be random), ignored if just used with SelfMasterOnly.
Below example is the random version of the above example for IndexedByField.

        {
            "types": [ "NPC" ],
            "OnlyIfDefault": true,
            "ForwardOptions": "DefaultRandom",
            "Forward": { "Items": [ "PrvtI_HeavyArmory.esp", "Immersive Weapons.esp" ] }
        }

- **<font color="green">DefaultThenSelfMasterOnly</font>**: Combination alias for selecting the Default, SelfMasterOnly and IndexedByField.
The first mod listed for a field will use the Default method. All others will follow using the SelfMasterOnly method.  
NOTE: The first mod must contain the record and if OnlyIfDefault set, pass that check, else it will not apply any of the SelfMasterOnly forwards.
All other mods do not check OnlyIfDefault, and can fail to find matching record without stopping the processing of other mods.

        {
            "types": [ "Outfit" ],
            "Masters": "Dawnguard.esm",
            "ForwardOptions": "DefaultThenSelfMasterOnly",
            "Forward": { "Items": [ "Unofficial Skyrim Modders Patch.esp", "Cloaks - Dawnguard.esp" ] }
        }



>**<font color="red">ForwardIndexedByField</font>**: This has been deprecated in v2.0, and will be removed in a future version. You should use ForwardOptions instead.

>**<font color="red">ForwardType</font>**: This has been deprecated in v2.0, and will be removed in a future version. You should use ForwardOptions instead.

### Merge

This is like Bashing but with a lot more control, as you cannot only merge just the fields you want, but use filters to only merge the records you want.
You also can tell the merge to exclude a plugin or force the values of a plugin to not be removed. Please see [Merge Example.json](../Examples/Merge%20Example.json)

# Groups

Groups allow you to group multiple rules together. This can assist in making the JSON files easier to maintain, but also opens up performance improvements, and some special processing.  
You **can not** have groups within groups. Rules in groups are processed in order the are in the JSON file. This means the Priority field for Rules in a group is ignored.

## Group Properties

>**<font color="red">SingleMatch</font>**: True or False (Default). If true a record will stop trying to match rules once it finds it's first match.
When this is true, it is the one time having a rule with no actions does something. That something is stop any records that match the rule from progressing to further rules in the group.

>**<font color="red">Rules</font>**: This is the list of all the rules in this group.

Example - Both rules in this group must be Factions in Skyrim.esm, it will also stop search for rules once it find a match for a record.  
SingleMatch not necessary in this example, as only a single rule would match anyway, but if lots of rules in group it could still save processing time by having it.

    [
      {
        "Types": "Faction",
        "Masters": "Skyrim.esm",
	    "SingleMatch": true,
        "Rules": [
          {
            "FormID": [
              "000013:Skyrim.esm",
              "01C4ED:Skyrim.esm",
              ...
            ],
            "Fill": {
              "flags": [
                "IgnoreMurder",
                "IgnoreAssault",
                "IgnoreStealing",
                "IgnoreTrespass",
                "DoNotReportCrimesAgainstMembers",
                "IgnorePickpocket",
                "IgnoreWerewolf"
              ]
            }
          },
          {
            "FormID": [
              "01BCC0:Skyrim.esm",
              "025F95:Skyrim.esm",
              ...
            ],
            "Fill": {
              "flags": [
                "IgnoreMurder",
                "IgnoreAssault",
                "IgnoreStealing",
                "IgnoreTrespass",
                "IgnorePickpocket",
                "-IgnoreWerewolf"
              ]
            }
          }
        ]
      }
    ]

## Debug / Trace Logging

If you have set your logging level to Debug or Trace in the settings page of GSP in Synthesis, you can use the following to enable logs to be generated for select rules.  
If used on a group it will only enable the Debug/Trace logging for the filters defined on the group. So you can add to both the group and/or rules in that group to get the logs you want.

> **<font color="green">Debug</font>**: True or False (Default).

        {
            "types": [ "NPC" ],
            "OnlyIfDefault": true,
            "ForwardOptions": "DefaultRandom",
            "Debug": true,
            "Forward": { "Items": [ "PrvtI_HeavyArmory.esp", "Immersive Weapons.esp" ] }
        }

Remember logging must be set to either Debug or Trace in Synthesis GSP plugin settings for this to have any effect.  
Debug in general will output extra logs about what the patcher has done.  
Trace will include all debug logs but also a lot extra, like why the patcher may not of done something (Failed to match rule, Value already matched what you tried to set).

You should always include Trace logs if reporting bugs, but as they are big:
- Use FormKey filter on settings page of GSP in Synthesis to limit to logging a single record that is affected by bug,
- or if FormKey not available the above Debug filter to only output logs for rule relevant to bug.
- Then either attach as text file to GitHub issue, 
- or if you using Nexus you <ins>must</ins> use spoiler tags.