# Rule Properties

These are case insensitive. If a property accepts multiple values as an array, you do not need to use the [ ] to denote an array if only entering a single value.

> **<font color="green">Priority</font>**: All matching rules will be applied to a record in ascending priority. Default: 0  
Matching priority will be applied in random order so be carful if multiple rules edit the same fields on a record.

# Basic Filters

At least 1 basic filter must be applied to a rule. If multiple filters provided then the logic is **Type AND (EditorID OR FormID)**  
All filters can be either single value or array of possible values.

>**<font color="green">Types</font>**: List valid types for this rule. If no types provided then can match any type. Types are case insensitive.  
[List of implemented types](Types.md).

> **<font color="green">EditorID</font>**: EditorID to match. Can be a regular expression if starts and ends with /. Example "EditorID": "/.\*fur.\*/"

> **<font color="green">FormID</font>**: FormID to match in the format "0123AB:Skyrim.esm". Can exclude leading 0s.

## Extra Filters

These can be single values or array of values.

>**<font color="red">-EditorID</font>**: EditorID to exclude from matches. Can be a regular expression if starts and ends with /. Example "-editorID": "/.\*fur.\*/"

>**<font color="red">-FormID</font>**: FormID to exclude from matches in above format.

>**<font color="red">Masters</font>**: List of mod names ("Skyrim.esm"). Record must come from one of these master plugins to match. Good if you only using the **Type** basic filter to limit it to just type from the listed mod.

>**<font color="red">OnlyIfDefault</font>**: False by default. If set to True will only apply action if the winning record's field value matches the original value set by the master record.
Good for example if you want to forward one patches changes, but only if a later patch set the value back to the default, so protecting other winning patches.  
Checks are per field, meaning one action may fail to apply due to the field it updating not matching, but other action on the same record applies as it does match.  
NOTE: Just because multiple fields are listed on a single fill/forward action, they are all still processed separately. No need to split into different rules.

# Advanced Filters

These filters can be applied in addition to the basic filters. Advanced filters should work for any [supported field](Fields.md).  
In general they follow the following rules:

- If you use one of these filters, any record that doesn't contain that field will never match the rule.
- &/|/^ Operator: You can change the default operation using a prefix on the field name. Default is OR (|). Can set to And (&) or XOR (^).
- +/-: You can add a prefix to these to say if they must be included or excluded from filter. Example below won't match any record with keyword Survival_ArmorCold.
- Include (+) and OR (|) prefixes are default so no need to enter them however, can be used for unforeseen cases where the value you want to enter may start with a prefix value. Like trying to enter a negative number ("+-123" to allowed -123).
- Exclude (-) values ignore Operation. A record with any excluded values will make it not match not matter the overall operation. ! can also be used instead of -.
- Currently operation only available per field. If you list multiple fields to match it will always be AND (&) between all the fields.

## Example

    ... Single Value Match
    "Matches": { "Keywords": "ArmorMaterialLeather" },
    ... OR Match with exclude Example
    "Matches": { "Keywords": [ "ArmorMaterialLeather", "ArmorMaterialHide", "-WAF_ClothingCloak" ] },
    ... AND Match Example
    "Matches": { "&Keywords": [ "ArmorMaterialLeather", "ArmorMaterialHide" ] },

# Actions

At least 1 action must exist. If both provided then both will be applied to matching records.

>**<font color="green">Fill</font>**: This will just apply the changes to all listed fields. The most basic of action.  
If field selected is a <font color="blue">list</font> then you must provide value as a list surrounded by [ ]. All other fields do not use [ ].  
In the below example Name just requires a single value, while Effects could have multiple so uses [ ]

    {
        "Types": "Ingestible",
        "FormID": "FAA:CookingAdventuresInSkyrim.esp",
        "Fill":
            {
                "Name" : "My Name for This",
                "Effects": [ { "effect": "002EE1:Update.esm" } ]
            }
    }

>**<font color="green">Forward</font>**: This will forward fields from a parent that this winning record overrode.  
By default this is just a straight replace including if it is a list field like Items. It doesn't do any merging.

>**<font color="green">ForwardIndexedByField</font>**: This changes how the contents of the Forward action is defined.  
By default or with this set to false, it is "Mod.esp" : ["field1", ...].  
Setting this to true changes it to "field":["Mod1.esp", "Mod2.esp", ....].  
Depending on what you are doing one or the other may be more efficient for you to read. Inside the patcher both produce the same result.  
If used with ForwardType of Default, as only a single mods value would be copied over it will pick a mod from the list to forward that contains this record.
Some settings may require this to be True. In those cases you can still just exclude this from the config as they will auto set it. However if you include it you must have it set correctly else you will get an error.  
Below example will forward record from PrvtI_HeavyArmory.esp unless it doesn't contain the record in which case Immersive Weapons.esp will be tried.

        {
            "types": [ "NPC" ],
            "OnlyIfDefault": true,
            "ForwardIndexedByField": true,
            "Forward": { "Items": [ "PrvtI_HeavyArmory.esp", "Immersive Weapons.esp" ] }
        }

>**<font color="green">ForwardType</font>**: ForwardType can change how Forwarding actions work. Following valid options:  
- **<font color="green">Default</font>**: Will replace winning with value from forwarding mod as described above.  
- **<font color="green">SelfMasterOnly</font>**: This is only relevant when forwarding changes from lists of FormIDs. This includes Keywords.  
It will only add new FormIDs to lists if FormID is from the same Forwarding Mod.  
It will not remove any current items from winning records.  
Other field types will just behave like they do in Default.  
Below example would make sure any cloaks that "Cloaks - Dawnguard.esp" added to outfits would be added to winning records if it was removed by another patch.
However the couple of changes it makes like adding back "Vampire Boots" [00B5DE:Dawnguard.esm] that the original Cloaks.esp removed would not be forwarded.

        {
            "types": "Outfit",
            "Masters": "Dawnguard.esm",
            "ForwardType": "SelfMasterOnly",
            "Forward": { "Cloaks - Dawnguard.esp": "Items" }
        }

- **<font color="green">DefaultThenSelfMasterOnly</font>**: This combines the two options above. This requires ForwardIndexedByField = true.  
The first mod listed for a field will use the Default method. All others will follow using the SelfMasterOnly method.  
NOTE: The first mod must successfully, by finding the record in that mod, and if OnlyIfDefault set, pass that check, else it will not apply any of the SelfMasterOnly forwards.
All other mods do not check OnlyIfDefault, and can fail to find matching record without stopping the processing of other mods.

        {
            "types": [ "Outfit" ],
            "Masters": "Dawnguard.esm",
            "ForwardType": "DefaultThenSelfMasterOnly",
            "Forward": { "Items": [ "Unofficial Skyrim Modders Patch.esp", "Cloaks - Dawnguard.esp" ] }
        }

- **<font color="green">DefaultRandom</font>**: This requires ForwardIndexedByField = true, however instead of picking the first valid mod to forward it will pick a random valid mod from the list.  
The randomness is seeded for each rule, field & record combination so unless you change the rule you should get the same results each time.  
Below example is the random version of the above example for ForwardIndexedByField. Changing the order of the listed mods, will change the seed used for randomness.

        {
            "types": [ "NPC" ],
            "OnlyIfDefault": true,
            "ForwardType": "DefaultRandom",
            "Forward": { "Items": [ "PrvtI_HeavyArmory.esp", "Immersive Weapons.esp" ] }
        }

# Groups

Groups allow you to group multiple rules together. This can assist in making the JSON files easier to maintain, but also opens up performance improvements, and some special processing.  
You **can not** have groups within groups. Rules in groups are processed in order the are in the JSON file. This means the Priority field for Rules in a group is ignored.

## Group Properties

> **<font color="green">Priority</font>**: This is the same priority as rules above. Groups are sorted with ungrouped rules based on priority.

>**<font color="green">Types</font>**: List valid types for this group. You can still specify types per rule to further limit a rule, but rules in a group cannot match types not defined here (Unless none defined at group level which is the same as All types).

>**<font color="red">Masters</font>**: List of mod names ("Skyrim.esm"). Same as types, you can still further limit it per group rule.

>**<font color="red">SingleMatch</font>**: True or False (Default). If true a record will stop trying to match rules once it finds it's first match.

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