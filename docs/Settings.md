# Rule Fields

These are case insensitive.

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

>**<font color="red">-editorID</font>**: EditorID to exclude from matches. Can be a regular expression if starts and ends with /. Example "-editorID": "/.\*fur.\*/"

>**<font color="red">-formID</font>**: FormID to exclude from matches in above format.

>**<font color="red">Masters</font>**: List of mod names ("Skyrim.esm"). Record must come from one of these master plugins to match. Good if you only using the **Type** basic filter to limit it to just type from the listed mod.

>**<font color="red">OnlyIfDefault</font>**: False by default. If set to True will only apply action if the winning record's field value matches the original value set by the master record.
Good for example if you want to forward one patches changes, but only if a later patch set the value back to the default, so protecting other winning patches.

# Advanced Filters

These filters can be applied in addition to the basic filters. In general they follow the following rules:

- If you use one of these filters, any record that doesn't contain that field will never match the rule.
- Can be a single value in "quotes" or multiple like ["Keyword1","Keyword2", "-Keyword3"]
- +/-: You can add a prefix to these to say if they must be included or excluded from filter.
- Operator: There will be an operation you can define when using multiple entries. Default is OR. Use "<FilterName>Op" : "AND" to change it to only match records that have all listed values.
- Exclude values ignore Operation. A record have any excluded values will make it not match.

## Example

    ...
    "Keywords": [ "ArmorMaterialLeather", "Survival_ArmorWarm", "-Survival_ArmorCold" ],
    "KeywordsOp": "AND",
    ...

## Available Advanced Filters

>**<font color="green">Factions</font>**: Only valid if Type set to only NPC. NPC will need to belong to one of the listed factions. Factions listed by FormID.  
>**<font color="green">FactionsOp</font>**: Operation to use if multiple factions listed. OR (default), AND, XOR.

>**<font color="green">Keywords</font>**: Keywords that must be assigned to record. Keywords listed by EditorID.  
>**<font color="green">KeywordsOp</font>**: Operation to use if multiple factions listed. OR (default), AND, XOR.

# Actions

At least 1 action must exist. If both provided then both will be applied to matching records.  
**NOTE:** While for the filters above you could either include a single entry without surrounding it with [ ]. For actions if multiple options possible you must always use [ ] even for single values.

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

>**<font color="green">ForwardType</font>**: ForwardType can change how Forwarding actions work. Following valid options:  
- **<font color="green">Default</font>**: Will replace winning with value from forwarding mod as described above.  
- **<font color="green">SelfMasterOnly</font>**: This is only relevant when forwarding changes from lists of FormIDs. This includes Keywords.  
It will only add new FormIDs to lists if FormID is from the same Forwarding Mod.  
It will not remove any current items from winning records.  
Other field types will just behave like they do in Default.  
Below example would make sure any cloaks that "Cloaks - Dawnguard.esp" added to outfits would be added to winning records if it was removed by another patch.
However the couple of changes it makes like adding back "Vampire Boots" [00B5DE:Dawnguard.esm] that the original Cloaks.esp removed would not be forwarded.

      {
        "types": [ "Outfit" ],
        "Masters": "Dawnguard.esm",
        "ForwardType": "SelfMasterOnly",
        "Forward": { "Cloaks - Dawnguard.esp": [ "Items" ] }
      }

Now with using a low Priority Default Forward rule, followed by 1 or more SelfMasterOnly Forward rules with a higher Priority you could achieve some complex results if other patches do not exist to do it.