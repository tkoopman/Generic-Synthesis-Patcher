# Implemented Fields

Fields that are invalid for a record that was matched by your filter will just be ignored.  
You can use either the full field name or Alt name when available.  
**NOTE**: I have not personally tested every field / record combination. When adding new record or field type I did just add any others that would reuse the same code.  

## MFFSM

This just details where the field can currently be used.
- M: Matches
- F: Fill
- F: Forward
- S: Self-Only Forward
- M: Merge


## ASPC - AcousticSpace

| Field              | Alt  | MFFSM | Value Type                                               | Example                       |
| ------------------ | ---- | ----- | -------------------------------------------------------- | ----------------------------- |
| AmbientSound       | SNAM | MFF-- | Form Key or Editor ID                                    |                               |
| EnvironmentType    | BNAM | MFF-- | Form Key or Editor ID                                    |                               |
| ObjectBounds       | OBND | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9] |
| UseSoundFromRegion | RDAT | MFF-- | Form Key or Editor ID                                    |                               |

[⬅ Back to Types](Types.md)

## AACT - ActionRecord

| Field | Alt  | MFFSM | Value Type                                                                                                                                                                                                  | Example             |
| ----- | ---- | ----- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------- |
| Color | CNAM | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "Color": [40,50,60] |

[⬅ Back to Types](Types.md)

## ACTI - Activator

| Field                | Alt            | MFFSM | Value Type                                                                                                                                  | Example                                                    |
| -------------------- | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| ActivateTextOverride | RNAM           | MFF-- | String value                                                                                                                                | "ActivateTextOverride": "Hello"                            |
| ActivationSound      | VNAM           | MFF-- | Form Key or Editor ID                                                                                                                       |                                                            |
| Flags                | DataFlags;FNAM | MF--- | Flags (NoDisplacement, IgnoredBySandbox)                                                                                                    | "Flags": [ "NoDisplacement", "-IgnoredBySandbox" ]         |
| InteractionKeyword   | KNAM           | MFF-- | Form Key or Editor ID                                                                                                                       |                                                            |
| Keywords             | KWDA           | MFFSM | Form Keys or Editor IDs                                                                                                                     |                                                            |
| LoopingSound         | SNAM           | MFF-- | Form Key or Editor ID                                                                                                                       |                                                            |
| MajorFlags           | RecordFlags    | MF--- | Flags (HasTreeLOD, MustUpdateAnims, HiddenFromLocalMap, HasDistantLOD, RandomAnimStart, Dangerous, IgnoreObjectInteraction, IsMarker, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, ChildCanUse, NavMeshGenerationGround) | "MajorFlags": [ "HasTreeLOD", "-NavMeshGenerationGround" ] |
| MarkerColor          | PNAM           | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "MarkerColor": [40,50,60]                                  |
| Name                 | FULL           | MFF-- | String value                                                                                                                                | "Name": "Hello"                                            |
| ObjectBounds         | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                                    | "ObjectBounds": [6,6,6,9,9,9]                              |
| WaterType            | WNAM           | MFF-- | Form Key or Editor ID                                                                                                                       |                                                            |

[⬅ Back to Types](Types.md)

## AVIF - ActorValueInformation

| Field        | Alt  | MFFSM | Value Type   | Example                 |
| ------------ | ---- | ----- | ------------ | ----------------------- |
| Abbreviation | ANAM | MFF-- | String value | "Abbreviation": "Hello" |
| Description  | DESC | MFF-- | String value | "Description": "Hello"  |
| Name         | FULL | MFF-- | String value | "Name": "Hello"         |

[⬅ Back to Types](Types.md)

## ADDN - AddonNode

| Field                   | Alt  | MFFSM | Value Type                                               | Example                       |
| ----------------------- | ---- | ----- | -------------------------------------------------------- | ----------------------------- |
| AlwaysLoaded            |      | MFF-- | True / False                                             | "AlwaysLoaded": true          |
| MasterParticleSystemCap |      | MFF-- | Numeric value                                            | "MasterParticleSystemCap": 7  |
| NodeIndex               | DATA | MFF-- | Numeric value                                            | "NodeIndex": 7                |
| ObjectBounds            | OBND | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9] |
| Sound                   | SNAM | MFF-- | Form Key or Editor ID                                    |                               |

[⬅ Back to Types](Types.md)

## APPA - AlchemicalApparatus

| Field        | Alt  | MFFSM | Value Type                                                       | Example                       |
| ------------ | ---- | ----- | ---------------------------------------------------------------- | ----------------------------- |
| Description  | DESC | MFF-- | String value                                                     | "Description": "Hello"        |
| Name         | FULL | MFF-- | String value                                                     | "Name": "Hello"               |
| ObjectBounds | OBND | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]         | "ObjectBounds": [6,6,6,9,9,9] |
| PickUpSound  | YNAM | MFF-- | Form Key or Editor ID                                            |                               |
| PutDownSound | ZNAM | MFF-- | Form Key or Editor ID                                            |                               |
| Quality      | QUAL | MF--- | Possible values (Novice, Apprentice, Journeyman, Expert, Master) | "Quality": "Novice"           |
| Value        |      | MFF-- | Numeric value                                                    | "Value": 7                    |
| Weight       |      | MFF-- | Decimal value                                                    | "Weight": 3.14                |

[⬅ Back to Types](Types.md)

## AMMO - Ammunition

| Field        | Alt            | MFFSM | Value Type                                                  | Example                                                  |
| ------------ | -------------- | ----- | ----------------------------------------------------------- | -------------------------------------------------------- |
| Damage       | DMG            | MFF-- | Decimal value                                               | "Damage": 3.14                                           |
| Description  | DESC           | MFF-- | String value                                                | "Description": "Hello"                                   |
| Flags        | DataFlags;FNAM | MF--- | Flags (IgnoresNormalWeaponResistance, NonPlayable, NonBolt) | "Flags": [ "IgnoresNormalWeaponResistance", "-NonBolt" ] |
| Keywords     | KWDA           | MFFSM | Form Keys or Editor IDs                                     |                                                          |
| MajorFlags   | RecordFlags    | MF--- | Flags (NonPlayable)                                         | "MajorFlags": "NonPlayable"                              |
| Name         | FULL           | MFF-- | String value                                                | "Name": "Hello"                                          |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]    | "ObjectBounds": [6,6,6,9,9,9]                            |
| PickUpSound  | YNAM           | MFF-- | Form Key or Editor ID                                       |                                                          |
| Projectile   |                | MFF-- | Form Key or Editor ID                                       |                                                          |
| PutDownSound | ZNAM           | MFF-- | Form Key or Editor ID                                       |                                                          |
| ShortName    | ONAM           | MFF-- | String value                                                | "ShortName": "Hello"                                     |
| Value        |                | MFF-- | Numeric value                                               | "Value": 7                                               |
| Weight       |                | MFF-- | Decimal value                                               | "Weight": 3.14                                           |

[⬅ Back to Types](Types.md)

## ANIO - AnimatedObject

| Field       | Alt  | MFFSM | Value Type   | Example                |
| ----------- | ---- | ----- | ------------ | ---------------------- |
| UnloadEvent | BNAM | MFF-- | String value | "UnloadEvent": "Hello" |

[⬅ Back to Types](Types.md)

## ARMA - ArmorAddon

| Field               | Alt  | MFFSM | Value Type              | Example                  |
| ------------------- | ---- | ----- | ----------------------- | ------------------------ |
| AdditionalRaces     | MODL | MFFSM | Form Keys or Editor IDs |                          |
| ArtObject           | ONAM | MFF-- | Form Key or Editor ID   |                          |
| DetectionSoundValue |      | MFF-- | Numeric value           | "DetectionSoundValue": 7 |
| FootstepSound       | SNDD | MFF-- | Form Key or Editor ID   |                          |
| Race                | RNAM | MFF-- | Form Key or Editor ID   |                          |
| WeaponAdjust        |      | MFF-- | Decimal value           | "WeaponAdjust": 3.14     |

[⬅ Back to Types](Types.md)

## ARMO - Armor

| Field                     | Alt         | MFFSM | Value Type                                               | Example                                    |
| ------------------------- | ----------- | ----- | -------------------------------------------------------- | ------------------------------------------ |
| AlternateBlockMaterial    | BAMT        | MFF-- | Form Key or Editor ID                                    |                                            |
| Armature                  | MODL        | MFFSM | Form Keys or Editor IDs                                  |                                            |
| ArmorRating               | DNAM        | MFF-- | Decimal value                                            | "ArmorRating": 3.14                        |
| BashImpactDataSet         | BIDS        | MFF-- | Form Key or Editor ID                                    |                                            |
| Description               | DESC        | MFF-- | String value                                             | "Description": "Hello"                     |
| EnchantmentAmount         | EAMT        | MFF-- | Numeric value                                            | "EnchantmentAmount": 7                     |
| EquipmentType             | ETYP        | MFF-- | Form Key or Editor ID                                    |                                            |
| Keywords                  | KWDA        | MFFSM | Form Keys or Editor IDs                                  |                                            |
| MajorFlags                | RecordFlags | MF--- | Flags (NonPlayable, Shield)                              | "MajorFlags": [ "NonPlayable", "-Shield" ] |
| Name                      | FULL        | MFF-- | String value                                             | "Name": "Hello"                            |
| ObjectBounds              | OBND        | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9]              |
| ObjectEffect              | EITM        | MFF-- | Form Key or Editor ID                                    |                                            |
| PickUpSound               | YNAM        | MFF-- | Form Key or Editor ID                                    |                                            |
| PutDownSound              | ZNAM        | MFF-- | Form Key or Editor ID                                    |                                            |
| Race                      | RNAM        | MFF-- | Form Key or Editor ID                                    |                                            |
| RagdollConstraintTemplate | BMCT        | MFF-- | String value                                             | "RagdollConstraintTemplate": "Hello"       |
| TemplateArmor             | TNAM        | MFF-- | Form Key or Editor ID                                    |                                            |
| Value                     |             | MFF-- | Numeric value                                            | "Value": 7                                 |
| Weight                    |             | MFF-- | Decimal value                                            | "Weight": 3.14                             |

[⬅ Back to Types](Types.md)

## ARTO - ArtObject

| Field        | Alt  | MFFSM | Value Type                                               | Example                                          |
| ------------ | ---- | ----- | -------------------------------------------------------- | ------------------------------------------------ |
| ObjectBounds | OBND | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9]                    |
| Type         | DNAM | MF--- | Flags (MagicCasting, MagicHitEffect, EnchantmentEffect)  | "Type": [ "MagicCasting", "-EnchantmentEffect" ] |

[⬅ Back to Types](Types.md)

## ASTP - AssociationType

| Field    | Alt | MFFSM | Value Type   | Example          |
| -------- | --- | ----- | ------------ | ---------------- |
| IsFamily |     | MFF-- | True / False | "IsFamily": true |

[⬅ Back to Types](Types.md)

## BOOK - Book

| Field        | Alt            | MFFSM | Value Type                                               | Example                       |
| ------------ | -------------- | ----- | -------------------------------------------------------- | ----------------------------- |
| BookText     | DESC           | MFF-- | String value                                             | "BookText": "Hello"           |
| Description  | CNAM;DESC      | MFF-- | String value                                             | "Description": "Hello"        |
| Flags        | DataFlags;FNAM | MF--- | Flags (CantBeTaken)                                      | "Flags": "CantBeTaken"        |
| InventoryArt | INAM           | MFF-- | Form Key or Editor ID                                    |                               |
| Keywords     | KWDA           | MFFSM | Form Keys or Editor IDs                                  |                               |
| Name         | FULL           | MFF-- | String value                                             | "Name": "Hello"               |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9] |
| PickUpSound  | YNAM           | MFF-- | Form Key or Editor ID                                    |                               |
| PutDownSound | ZNAM           | MFF-- | Form Key or Editor ID                                    |                               |
| Type         |                | MF--- | Possible values (BookOrTome, NoteOrScroll)               | "Type": "BookOrTome"          |
| Value        |                | MFF-- | Numeric value                                            | "Value": 7                    |
| Weight       |                | MFF-- | Decimal value                                            | "Weight": 3.14                |

[⬅ Back to Types](Types.md)

## CPTH - CameraPath

| Field                   | Alt | MFFSM | Value Type                         | Example                            |
| ----------------------- | --- | ----- | ---------------------------------- | ---------------------------------- |
| RelatedPaths            |     | MFFSM | Form Keys or Editor IDs            |                                    |
| Shots                   |     | MFFSM | Form Keys or Editor IDs            |                                    |
| Zoom                    |     | MF--- | Flags (Default, Disable, ShotList) | "Zoom": [ "Default", "-ShotList" ] |
| ZoomMustHaveCameraShots |     | MFF-- | True / False                       | "ZoomMustHaveCameraShots": true    |

[⬅ Back to Types](Types.md)

## CAMS - CameraShot

| Field                      | Alt            | MFFSM | Value Type                                                                                                            | Example                                                    |
| -------------------------- | -------------- | ----- | --------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| Action                     |                | MF--- | Possible values (Shoot, Fly, Hit, Zoom)                                                                               | "Action": "Shoot"                                          |
| Flags                      | DataFlags;FNAM | MF--- | Flags (PositionFollowsLocation, RotationFollowsTarget, DoNotFollowBone, FirstPersonCamera, NoTracer, StartAtTimeZero) | "Flags": [ "PositionFollowsLocation", "-StartAtTimeZero" ] |
| ImageSpaceModifier         | MNAM           | MFF-- | Form Key or Editor ID                                                                                                 |                                                            |
| Location                   | XLCN           | MF--- | Possible values (Attacker, Projectile, Target, LeadActor)                                                             | "Location": "Attacker"                                     |
| MaxTime                    |                | MFF-- | Decimal value                                                                                                         | "MaxTime": 3.14                                            |
| MinTime                    |                | MFF-- | Decimal value                                                                                                         | "MinTime": 3.14                                            |
| NearTargetDistance         |                | MFF-- | Decimal value                                                                                                         | "NearTargetDistance": 3.14                                 |
| Target                     |                | MF--- | Possible values (Attacker, Projectile, Target, LeadActor)                                                             | "Target": "Attacker"                                       |
| TargetPercentBetweenActors |                | MFF-- | Decimal value                                                                                                         | "TargetPercentBetweenActors": 3.14                         |
| TimeMultiplierGlobal       |                | MFF-- | Decimal value                                                                                                         | "TimeMultiplierGlobal": 3.14                               |
| TimeMultiplierPlayer       |                | MFF-- | Decimal value                                                                                                         | "TimeMultiplierPlayer": 3.14                               |
| TimeMultiplierTarget       |                | MFF-- | Decimal value                                                                                                         | "TimeMultiplierTarget": 3.14                               |

[⬅ Back to Types](Types.md)

## CELL - Cell

| Field                   | Alt            | MFFSM | Value Type                                                                                                         | Example                                          |
| ----------------------- | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------ |
| AcousticSpace           | XCAS           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| EncounterZone           | XEZN           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| FactionRank             |                | MFF-- | Numeric value                                                                                                      | "FactionRank": 7                                 |
| Flags                   | DataFlags;FNAM | MF--- | Flags (IsInteriorCell, HasWater, CantTravelFromHere, NoLodWater, PublicArea, HandChanged, ShowSky, UseSkyLighting) | "Flags": [ "IsInteriorCell", "-UseSkyLighting" ] |
| ImageSpace              | XCIM           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| LightingTemplate        | LTMP           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| Location                | XLCN           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| LockList                | XILL           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| MajorFlags              | RecordFlags    | MF--- | Flags (Persistent, OffLimits, CantWait)                                                                            | "MajorFlags": [ "Persistent", "-CantWait" ]      |
| Music                   | XCMO           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| Name                    | FULL           | MFF-- | String value                                                                                                       | "Name": "Hello"                                  |
| Owner                   |                | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| Regions                 | XCLR           | MFFSM | Form Keys or Editor IDs                                                                                            |                                                  |
| SkyAndWeatherFromRegion | XCCM           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| Water                   | XCWT           | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| WaterEnvironmentMap     | XWEM           | MFF-- | String value                                                                                                       | "WaterEnvironmentMap": "Hello"                   |
| WaterHeight             | XCLW           | MFF-- | Decimal value                                                                                                      | "WaterHeight": 3.14                              |
| WaterNoiseTexture       | XNAM           | MFF-- | String value                                                                                                       | "WaterNoiseTexture": "Hello"                     |

[⬅ Back to Types](Types.md)

## CLAS - Class

| Field            | Alt  | MFFSM | Value Type                                                                                                                                                                                   | Example                 |
| ---------------- | ---- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------- |
| BleedoutDefault  |      | MFF-- | Decimal value                                                                                                                                                                                | "BleedoutDefault": 3.14 |
| Description      | DESC | MFF-- | String value                                                                                                                                                                                 | "Description": "Hello"  |
| Icon             |      | MFF-- | String value                                                                                                                                                                                 | "Icon": "Hello"         |
| MaxTrainingLevel |      | MFF-- | Numeric value                                                                                                                                                                                | "MaxTrainingLevel": 7   |
| Name             | FULL | MFF-- | String value                                                                                                                                                                                 | "Name": "Hello"         |
| Teaches          |      | MF--- | Possible values (OneHanded, TwoHanded, Archery, Block, Smithing, HeavyArmor, LightArmor, Pickpocket, Lockpicking, Sneak, Alchemy, Speech, Alteration, Conjuration, Destruction, Illusion, Restoration, Enchanting) | "Teaches": "OneHanded"  |
| VoicePoints      |      | MFF-- | Numeric value                                                                                                                                                                                | "VoicePoints": 7        |

[⬅ Back to Types](Types.md)

## CLMT - Climate

| Field       | Alt | MFFSM | Value Type              | Example                           |
| ----------- | --- | ----- | ----------------------- | --------------------------------- |
| Moons       |     | MF--- | Flags (Masser, Secunda) | "Moons": [ "Masser", "-Secunda" ] |
| PhaseLength |     | MFF-- | Numeric value           | "PhaseLength": 7                  |
| Volatility  |     | MFF-- | Numeric value           | "Volatility": 7                   |

[⬅ Back to Types](Types.md)

## COLL - CollisionLayer

| Field        | Alt                 | MFFSM | Value Type                                                                                                                                               | Example                                          |
| ------------ | ------------------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| CollidesWith | CNAM                | MFFSM | Form Keys or Editor IDs                                                                                                                                  |                                                  |
| DebugColor   | FNAM                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "DebugColor": [40,50,60]                         |
| Description  | DESC                | MFF-- | String value                                                                                                                                             | "Description": "Hello"                           |
| Flags        | DataFlags;FNAM;GNAM | MF--- | Flags (TriggerVolume, Sensor, NavmeshObstacle)                                                                                                           | "Flags": [ "TriggerVolume", "-NavmeshObstacle" ] |
| Index        | BNAM                | MFF-- | Numeric value                                                                                                                                            | "Index": 7                                       |
| Name         | FULL;MNAM           | MFF-- | String value                                                                                                                                             | "Name": "Hello"                                  |

[⬅ Back to Types](Types.md)

## CLFM - ColorRecord

| Field    | Alt  | MFFSM | Value Type                                                                                                                                                                                               | Example             |
| -------- | ---- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------- |
| Color    |      | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "Color": [40,50,60] |
| Name     | FULL | MFF-- | String value                                                                                                                                                                                             | "Name": "Hello"     |
| Playable | FNAM | MFF-- | True / False                                                                                                                                                                                             | "Playable": true    |

[⬅ Back to Types](Types.md)

## CSTY - CombatStyle

| Field                     | Alt            | MFFSM | Value Type                                   | Example                                      |
| ------------------------- | -------------- | ----- | -------------------------------------------- | -------------------------------------------- |
| AvoidThreatChance         |                | MFF-- | Decimal value                                | "AvoidThreatChance": 3.14                    |
| CSGDDataTypeState         |                | MF--- | Flags (Break0, Break1)                       | "CSGDDataTypeState": [ "Break0", "-Break1" ] |
| DefensiveMult             |                | MFF-- | Decimal value                                | "DefensiveMult": 3.14                        |
| EquipmentScoreMultMagic   |                | MFF-- | Decimal value                                | "EquipmentScoreMultMagic": 3.14              |
| EquipmentScoreMultMelee   |                | MFF-- | Decimal value                                | "EquipmentScoreMultMelee": 3.14              |
| EquipmentScoreMultRanged  |                | MFF-- | Decimal value                                | "EquipmentScoreMultRanged": 3.14             |
| EquipmentScoreMultShout   |                | MFF-- | Decimal value                                | "EquipmentScoreMultShout": 3.14              |
| EquipmentScoreMultStaff   |                | MFF-- | Decimal value                                | "EquipmentScoreMultStaff": 3.14              |
| EquipmentScoreMultUnarmed |                | MFF-- | Decimal value                                | "EquipmentScoreMultUnarmed": 3.14            |
| Flags                     | DataFlags;FNAM | MF--- | Flags (Dueling, Flanking, AllowDualWielding) | "Flags": [ "Dueling", "-AllowDualWielding" ] |
| GroupOffensiveMult        |                | MFF-- | Decimal value                                | "GroupOffensiveMult": 3.14                   |
| LongRangeStrafeMult       | CSLR           | MFF-- | Decimal value                                | "LongRangeStrafeMult": 3.14                  |
| MajorFlags                | RecordFlags    | MF--- | Flags (AllowDualWielding)                    | "MajorFlags": "AllowDualWielding"            |
| OffensiveMult             |                | MFF-- | Decimal value                                | "OffensiveMult": 3.14                        |

[⬅ Back to Types](Types.md)

## COBJ - ConstructibleObject

| Field              | Alt  | MFFSM | Value Type                                                      | Example                                              |
| ------------------ | ---- | ----- | --------------------------------------------------------------- | ---------------------------------------------------- |
| CreatedObject      | CNAM | MFF-- | Form Key or Editor ID                                           |                                                      |
| CreatedObjectCount | NAM1 | MFF-- | Numeric value                                                   | "CreatedObjectCount": 7                              |
| Items              | Item | MFFSM | JSON objects containing item Form Key/Editor ID and Count (QTY) | "Items": { "Item": "021FED:Skyrim.esm", "Count": 3 } |
| WorkbenchKeyword   | BNAM | MFF-- | Form Key or Editor ID                                           |                                                      |

[⬅ Back to Types](Types.md)

## CONT - Container

| Field        | Alt            | MFFSM | Value Type                                                                                                                       | Example                                                       |
| ------------ | -------------- | ----- | -------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------- |
| CloseSound   | QNAM           | MFF-- | Form Key or Editor ID                                                                                                            |                                                               |
| Flags        | DataFlags;FNAM | MF--- | Flags (AllowSoundsWhenAnimation, Respawns, ShowOwner)                                                                            | "Flags": [ "AllowSoundsWhenAnimation", "-ShowOwner" ]         |
| Items        | Item           | MFFSM | JSON objects containing item Form Key/Editor ID and Count (QTY)                                                                  | "Items": { "Item": "021FED:Skyrim.esm", "Count": 3 }          |
| MajorFlags   | RecordFlags    | MF--- | Flags (HasDistantLOD, RandomAnimStart, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, NavMeshGenerationGround) | "MajorFlags": [ "HasDistantLOD", "-NavMeshGenerationGround" ] |
| Name         | FULL           | MFF-- | String value                                                                                                                     | "Name": "Hello"                                               |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                         | "ObjectBounds": [6,6,6,9,9,9]                                 |
| OpenSound    | SNAM           | MFF-- | Form Key or Editor ID                                                                                                            |                                                               |
| Weight       |                | MFF-- | Decimal value                                                                                                                    | "Weight": 3.14                                                |

[⬅ Back to Types](Types.md)

## DLBR - DialogBranch

| Field         | Alt            | MFFSM | Value Type                            | Example                               |
| ------------- | -------------- | ----- | ------------------------------------- | ------------------------------------- |
| Category      | TNAM           | MF--- | Possible values (Player, Command)     | "Category": "Player"                  |
| Flags         | DataFlags;FNAM | MF--- | Flags (TopLevel, Blocking, Exclusive) | "Flags": [ "TopLevel", "-Exclusive" ] |
| Quest         | QNAM           | MFF-- | Form Key or Editor ID                 |                                       |
| StartingTopic | SNAM           | MFF-- | Form Key or Editor ID                 |                                       |

[⬅ Back to Types](Types.md)

## DIAL - DialogTopic

| Field      | Alt  | MFFSM | Value Type                                                                                                                                                                            | Example                              |
| ---------- | ---- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------ |
| Branch     | BNAM | MFF-- | Form Key or Editor ID                                                                                                                                                                 |                                      |
| Category   |      | MF--- | Possible values (Topic, Favor, Scene, Combat, Favors, Detection, Service, Misc)                                                                                                       | "Category": "Topic"                  |
| Name       | FULL | MFF-- | String value                                                                                                                                                                          | "Name": "Hello"                      |
| Priority   | PNAM | MFF-- | Decimal value                                                                                                                                                                         | "Priority": 3.14                     |
| Quest      | QNAM | MFF-- | Form Key or Editor ID                                                                                                                                                                 |                                      |
| Subtype    | SNAM | MF--- | Possible values (Custom, ForceGreet, Rumors, Intimidate, Flatter, Bribe, AskGift, Gift, AskFavor, Favor, ShowRelationships, Follow, Reject, Scene, Show, Agree, Refuse, ExitFavorState, MoralRefusal, FlyingMountLand, FlyingMountCancelLand, FlyingMountAcceptTarget, FlyingMountRejectTarget, FlyingMountNoTarget, FlyingMountDestinationReached, Attack, PowerAttack, Bash, Hit, Flee, Bleedout, AvoidThreat, Death, GroupStrategy, Block, Taunt, AllyKilled, Steal, Yield, AcceptYield, PickpocketCombat, Assault, Murder, AssaultNC, MurderNC, PickpocketNC, StealFromNC, TrespassAgainstNC, Trespass, WerewolfTransformCrime, VoicePowerStartShort, VoicePowerStartLong, VoicePowerEndShort, VoicePowerEndLong, AlertIdle, LostIdle, NormalToAlert, AlertToCombat, NormalToCombat, AlertToNormal, CombatToNormal, CombatToLost, LostToNormal, LostToCombat, DetectFriendDie, ServiceRefusal, Repair, Travel, Training, BarterExit, RepairExit, Recharge, RechargeExit, TrainingExit, ObserveCombat, NoticeCorpse, TimeToGo, Goodbye, Hello, SwingMeleeWeapon, ShootBow, ZKeyObject, Jump, KnockOverObject, DestroyObject, StandOnFurniture, LockedObject, PickpocketTopic, PursueIdleTopic, SharedInfo, PlayerCastProjectileSpell, PlayerCastSelfSpell, PlayerShout, Idle, EnterSprintBreath, EnterBowZoomBreath, ExitBowZoomBreath, ActorCollideWithActor, PlayerInIronSights, OutOfBreath, CombatGrunt, LeaveWaterBreath) | "Subtype": "Custom"                  |
| TopicFlags |      | MF--- | Flags (DoAllBeforeRepeating)                                                                                                                                                          | "TopicFlags": "DoAllBeforeRepeating" |

[⬅ Back to Types](Types.md)

## DLVW - DialogView

| Field    | Alt  | MFFSM | Value Type              | Example |
| -------- | ---- | ----- | ----------------------- | ------- |
| Branches | BNAM | MFFSM | Form Keys or Editor IDs |         |
| Quest    | QNAM | MFF-- | Form Key or Editor ID   |         |

[⬅ Back to Types](Types.md)

## DOOR - Door

| Field        | Alt            | MFFSM | Value Type                                                              | Example                                              |
| ------------ | -------------- | ----- | ----------------------------------------------------------------------- | ---------------------------------------------------- |
| CloseSound   | ANAM           | MFF-- | Form Key or Editor ID                                                   |                                                      |
| Flags        | DataFlags;FNAM | MF--- | Flags (Automatic, Hidden, MinimalUse, Sliding, DoNotOpenInCombatSearch) | "Flags": [ "Automatic", "-DoNotOpenInCombatSearch" ] |
| LoopSound    | BNAM           | MFF-- | Form Key or Editor ID                                                   |                                                      |
| MajorFlags   | RecordFlags    | MF--- | Flags (HasDistantLOD, RandomAnimStart, IsMarker)                        | "MajorFlags": [ "HasDistantLOD", "-IsMarker" ]       |
| Name         | FULL           | MFF-- | String value                                                            | "Name": "Hello"                                      |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                | "ObjectBounds": [6,6,6,9,9,9]                        |
| OpenSound    | SNAM           | MFF-- | Form Key or Editor ID                                                   |                                                      |

[⬅ Back to Types](Types.md)

## DUAL - DualCastData

| Field         | Alt  | MFFSM | Value Type                                               | Example                                          |
| ------------- | ---- | ----- | -------------------------------------------------------- | ------------------------------------------------ |
| EffectShader  |      | MFF-- | Form Key or Editor ID                                    |                                                  |
| Explosion     |      | MFF-- | Form Key or Editor ID                                    |                                                  |
| HitEffectArt  |      | MFF-- | Form Key or Editor ID                                    |                                                  |
| ImpactDataSet |      | MFF-- | Form Key or Editor ID                                    |                                                  |
| InheritScale  |      | MF--- | Flags (HitEffectArt, Projectile, Explosion)              | "InheritScale": [ "HitEffectArt", "-Explosion" ] |
| ObjectBounds  | OBND | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9]                    |
| Projectile    |      | MFF-- | Form Key or Editor ID                                    |                                                  |

[⬅ Back to Types](Types.md)

## EFSH - EffectShader

| Field                                      | Alt            | MFFSM | Value Type                                                                                                                  | Example                                              |
| ------------------------------------------ | -------------- | ----- | --------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------- |
| AddonModels                                |                | MFF-- | Form Key or Editor ID                                                                                                       |                                                      |
| AddonModelsFadeInTime                      |                | MFF-- | Decimal value                                                                                                               | "AddonModelsFadeInTime": 3.14                        |
| AddonModelsFadeOutTime                     |                | MFF-- | Decimal value                                                                                                               | "AddonModelsFadeOutTime": 3.14                       |
| AddonModelsScaleEnd                        |                | MFF-- | Decimal value                                                                                                               | "AddonModelsScaleEnd": 3.14                          |
| AddonModelsScaleInTime                     |                | MFF-- | Decimal value                                                                                                               | "AddonModelsScaleInTime": 3.14                       |
| AddonModelsScaleOutTime                    |                | MFF-- | Decimal value                                                                                                               | "AddonModelsScaleOutTime": 3.14                      |
| AddonModelsScaleStart                      |                | MFF-- | Decimal value                                                                                                               | "AddonModelsScaleStart": 3.14                        |
| AmbientSound                               |                | MFF-- | Form Key or Editor ID                                                                                                       |                                                      |
| BirthPositionOffset                        |                | MFF-- | Decimal value                                                                                                               | "BirthPositionOffset": 3.14                          |
| BirthPositionOffsetRangePlusMinus          |                | MFF-- | Decimal value                                                                                                               | "BirthPositionOffsetRangePlusMinus": 3.14            |
| ColorKey1                                  |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "ColorKey1": [40,50,60]                              |
| ColorKey1Alpha                             |                | MFF-- | Decimal value                                                                                                               | "ColorKey1Alpha": 3.14                               |
| ColorKey1Time                              |                | MFF-- | Decimal value                                                                                                               | "ColorKey1Time": 3.14                                |
| ColorKey2                                  |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "ColorKey2": [40,50,60]                              |
| ColorKey2Alpha                             |                | MFF-- | Decimal value                                                                                                               | "ColorKey2Alpha": 3.14                               |
| ColorKey2Time                              |                | MFF-- | Decimal value                                                                                                               | "ColorKey2Time": 3.14                                |
| ColorKey3                                  |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "ColorKey3": [40,50,60]                              |
| ColorKey3Alpha                             |                | MFF-- | Decimal value                                                                                                               | "ColorKey3Alpha": 3.14                               |
| ColorKey3Time                              |                | MFF-- | Decimal value                                                                                                               | "ColorKey3Time": 3.14                                |
| ColorScale                                 |                | MFF-- | Decimal value                                                                                                               | "ColorScale": 3.14                                   |
| EdgeColor                                  |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "EdgeColor": [40,50,60]                              |
| EdgeEffectAlphaFadeInTime                  |                | MFF-- | Decimal value                                                                                                               | "EdgeEffectAlphaFadeInTime": 3.14                    |
| EdgeEffectAlphaFadeOutTime                 |                | MFF-- | Decimal value                                                                                                               | "EdgeEffectAlphaFadeOutTime": 3.14                   |
| EdgeEffectAlphaPulseAmplitude              |                | MFF-- | Decimal value                                                                                                               | "EdgeEffectAlphaPulseAmplitude": 3.14                |
| EdgeEffectAlphaPulseFrequency              |                | MFF-- | Decimal value                                                                                                               | "EdgeEffectAlphaPulseFrequency": 3.14                |
| EdgeEffectColor                            |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "EdgeEffectColor": [40,50,60]                        |
| EdgeEffectFallOff                          |                | MFF-- | Decimal value                                                                                                               | "EdgeEffectFallOff": 3.14                            |
| EdgeEffectFullAlphaRatio                   |                | MFF-- | Decimal value                                                                                                               | "EdgeEffectFullAlphaRatio": 3.14                     |
| EdgeEffectFullAlphaTime                    |                | MFF-- | Decimal value                                                                                                               | "EdgeEffectFullAlphaTime": 3.14                      |
| EdgeEffectPersistentAlphaRatio             |                | MFF-- | Decimal value                                                                                                               | "EdgeEffectPersistentAlphaRatio": 3.14               |
| EdgeWidth                                  |                | MFF-- | Decimal value                                                                                                               | "EdgeWidth": 3.14                                    |
| ExplosionWindSpeed                         |                | MFF-- | Decimal value                                                                                                               | "ExplosionWindSpeed": 3.14                           |
| FillAlphaFadeInTime                        |                | MFF-- | Decimal value                                                                                                               | "FillAlphaFadeInTime": 3.14                          |
| FillAlphaPulseAmplitude                    |                | MFF-- | Decimal value                                                                                                               | "FillAlphaPulseAmplitude": 3.14                      |
| FillAlphaPulseFrequency                    |                | MFF-- | Decimal value                                                                                                               | "FillAlphaPulseFrequency": 3.14                      |
| FillColorKey1                              |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "FillColorKey1": [40,50,60]                          |
| FillColorKey1Scale                         |                | MFF-- | Decimal value                                                                                                               | "FillColorKey1Scale": 3.14                           |
| FillColorKey1Time                          |                | MFF-- | Decimal value                                                                                                               | "FillColorKey1Time": 3.14                            |
| FillColorKey2                              |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "FillColorKey2": [40,50,60]                          |
| FillColorKey2Scale                         |                | MFF-- | Decimal value                                                                                                               | "FillColorKey2Scale": 3.14                           |
| FillColorKey2Time                          |                | MFF-- | Decimal value                                                                                                               | "FillColorKey2Time": 3.14                            |
| FillColorKey3                              |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "FillColorKey3": [40,50,60]                          |
| FillColorKey3Scale                         |                | MFF-- | Decimal value                                                                                                               | "FillColorKey3Scale": 3.14                           |
| FillColorKey3Time                          |                | MFF-- | Decimal value                                                                                                               | "FillColorKey3Time": 3.14                            |
| FillFadeOutTime                            |                | MFF-- | Decimal value                                                                                                               | "FillFadeOutTime": 3.14                              |
| FillFullAlphaRatio                         |                | MFF-- | Decimal value                                                                                                               | "FillFullAlphaRatio": 3.14                           |
| FillFullAlphaTime                          |                | MFF-- | Decimal value                                                                                                               | "FillFullAlphaTime": 3.14                            |
| FillPersistentAlphaRatio                   |                | MFF-- | Decimal value                                                                                                               | "FillPersistentAlphaRatio": 3.14                     |
| FillTextureAnimationSpeedU                 |                | MFF-- | Decimal value                                                                                                               | "FillTextureAnimationSpeedU": 3.14                   |
| FillTextureAnimationSpeedV                 |                | MFF-- | Decimal value                                                                                                               | "FillTextureAnimationSpeedV": 3.14                   |
| FillTextureScaleU                          |                | MFF-- | Decimal value                                                                                                               | "FillTextureScaleU": 3.14                            |
| FillTextureScaleV                          |                | MFF-- | Decimal value                                                                                                               | "FillTextureScaleV": 3.14                            |
| Flags                                      | DataFlags;FNAM | MF--- | Flags (NoMembraneShader, MembraneGrayscaleColor, MembraneGrayscaleAlpha, NoParticleShader, EdgeEffectInverse, AffectSkinOnly, TextureEffectIgnoreAlpha, TextureEffectProjectUVs, IgnoreBaseGeometryAlpha, TextureEffectLighting, TextureEffectNoWeapons, ParticleAnimated, ParticleGrayscaleColor, ParticleGrayscaleAlpha, UseBloodGeometry) | "Flags": [ "NoMembraneShader", "-UseBloodGeometry" ] |
| HolesEndTime                               |                | MFF-- | Decimal value                                                                                                               | "HolesEndTime": 3.14                                 |
| HolesEndValue                              |                | MFF-- | Decimal value                                                                                                               | "HolesEndValue": 3.14                                |
| HolesStartTime                             |                | MFF-- | Decimal value                                                                                                               | "HolesStartTime": 3.14                               |
| HolesStartValue                            |                | MFF-- | Decimal value                                                                                                               | "HolesStartValue": 3.14                              |
| MembraneBlendOperation                     |                | MF--- | Possible values (Add, Subtract, ReverseSubtract, Minimum, Maximum)                                                          | "MembraneBlendOperation": "Add"                      |
| MembraneDestBlendMode                      |                | MF--- | Possible values (Zero, One, SourceColor, SourceInverseColor, SourceAlpha, SourceInvertedAlpha, DestAlpha, DestInvertedAlpha, DestColor, DestInverseColor, SourceAlphaSat) | "MembraneDestBlendMode": "Zero"                      |
| MembraneSourceBlendMode                    |                | MF--- | Possible values (Zero, One, SourceColor, SourceInverseColor, SourceAlpha, SourceInvertedAlpha, DestAlpha, DestInvertedAlpha, DestColor, DestInverseColor, SourceAlphaSat) | "MembraneSourceBlendMode": "Zero"                    |
| MembraneZTest                              |                | MF--- | Possible values (EqualTo, Normal, GreaterThan, GreaterThanOrEqualTo, AlwaysShow)                                            | "MembraneZTest": "EqualTo"                           |
| ParticleAcceleration1                      |                | MFF-- | Decimal value                                                                                                               | "ParticleAcceleration1": 3.14                        |
| ParticleAcceleration2                      |                | MFF-- | Decimal value                                                                                                               | "ParticleAcceleration2": 3.14                        |
| ParticleAcceleration3                      |                | MFF-- | Decimal value                                                                                                               | "ParticleAcceleration3": 3.14                        |
| ParticleAccelerationAlongNormal            |                | MFF-- | Decimal value                                                                                                               | "ParticleAccelerationAlongNormal": 3.14              |
| ParticleAnimatedEndFrame                   |                | MFF-- | Numeric value                                                                                                               | "ParticleAnimatedEndFrame": 7                        |
| ParticleAnimatedFrameCount                 |                | MFF-- | Numeric value                                                                                                               | "ParticleAnimatedFrameCount": 7                      |
| ParticleAnimatedFrameCountVariation        |                | MFF-- | Numeric value                                                                                                               | "ParticleAnimatedFrameCountVariation": 7             |
| ParticleAnimatedLoopStartFrame             |                | MFF-- | Numeric value                                                                                                               | "ParticleAnimatedLoopStartFrame": 7                  |
| ParticleAnimatedLoopStartVariation         |                | MFF-- | Numeric value                                                                                                               | "ParticleAnimatedLoopStartVariation": 7              |
| ParticleAnimatedStartFrame                 |                | MFF-- | Numeric value                                                                                                               | "ParticleAnimatedStartFrame": 7                      |
| ParticleAnimatedStartFrameVariation        |                | MFF-- | Numeric value                                                                                                               | "ParticleAnimatedStartFrameVariation": 7             |
| ParticleBirthRampDownTime                  |                | MFF-- | Decimal value                                                                                                               | "ParticleBirthRampDownTime": 3.14                    |
| ParticleBirthRampUpTime                    |                | MFF-- | Decimal value                                                                                                               | "ParticleBirthRampUpTime": 3.14                      |
| ParticleBlendOperation                     |                | MF--- | Possible values (Add, Subtract, ReverseSubtract, Minimum, Maximum)                                                          | "ParticleBlendOperation": "Add"                      |
| ParticleDestBlendMode                      |                | MF--- | Possible values (Zero, One, SourceColor, SourceInverseColor, SourceAlpha, SourceInvertedAlpha, DestAlpha, DestInvertedAlpha, DestColor, DestInverseColor, SourceAlphaSat) | "ParticleDestBlendMode": "Zero"                      |
| ParticleFullBirthRatio                     |                | MFF-- | Decimal value                                                                                                               | "ParticleFullBirthRatio": 3.14                       |
| ParticleFullBirthTime                      |                | MFF-- | Decimal value                                                                                                               | "ParticleFullBirthTime": 3.14                        |
| ParticleInitialRotationDegree              |                | MFF-- | Decimal value                                                                                                               | "ParticleInitialRotationDegree": 3.14                |
| ParticleInitialRotationDegreePlusMinus     |                | MFF-- | Decimal value                                                                                                               | "ParticleInitialRotationDegreePlusMinus": 3.14       |
| ParticleInitialSpeedAlongNormal            |                | MFF-- | Decimal value                                                                                                               | "ParticleInitialSpeedAlongNormal": 3.14              |
| ParticleInitialSpeedAlongNormalPlusMinus   |                | MFF-- | Decimal value                                                                                                               | "ParticleInitialSpeedAlongNormalPlusMinus": 3.14     |
| ParticleInitialVelocity1                   |                | MFF-- | Decimal value                                                                                                               | "ParticleInitialVelocity1": 3.14                     |
| ParticleInitialVelocity2                   |                | MFF-- | Decimal value                                                                                                               | "ParticleInitialVelocity2": 3.14                     |
| ParticleInitialVelocity3                   |                | MFF-- | Decimal value                                                                                                               | "ParticleInitialVelocity3": 3.14                     |
| ParticleLifetime                           |                | MFF-- | Decimal value                                                                                                               | "ParticleLifetime": 3.14                             |
| ParticleLifetimePlusMinus                  |                | MFF-- | Decimal value                                                                                                               | "ParticleLifetimePlusMinus": 3.14                    |
| ParticlePeristentCount                     |                | MFF-- | Decimal value                                                                                                               | "ParticlePeristentCount": 3.14                       |
| ParticleRotationSpeedDegreePerSec          |                | MFF-- | Decimal value                                                                                                               | "ParticleRotationSpeedDegreePerSec": 3.14            |
| ParticleRotationSpeedDegreePerSecPlusMinus |                | MFF-- | Decimal value                                                                                                               | "ParticleRotationSpeedDegreePerSecPlusMinus": 3.14   |
| ParticleScaleKey1                          |                | MFF-- | Decimal value                                                                                                               | "ParticleScaleKey1": 3.14                            |
| ParticleScaleKey1Time                      |                | MFF-- | Decimal value                                                                                                               | "ParticleScaleKey1Time": 3.14                        |
| ParticleScaleKey2                          |                | MFF-- | Decimal value                                                                                                               | "ParticleScaleKey2": 3.14                            |
| ParticleScaleKey2Time                      |                | MFF-- | Decimal value                                                                                                               | "ParticleScaleKey2Time": 3.14                        |
| ParticleSourceBlendMode                    |                | MF--- | Possible values (Zero, One, SourceColor, SourceInverseColor, SourceAlpha, SourceInvertedAlpha, DestAlpha, DestInvertedAlpha, DestColor, DestInverseColor, SourceAlphaSat) | "ParticleSourceBlendMode": "Zero"                    |
| ParticleZTest                              |                | MF--- | Possible values (EqualTo, Normal, GreaterThan, GreaterThanOrEqualTo, AlwaysShow)                                            | "ParticleZTest": "EqualTo"                           |
| SceneGraphEmitDepthLimit                   |                | MFF-- | Numeric value                                                                                                               | "SceneGraphEmitDepthLimit": 7                        |
| TextureCountU                              |                | MFF-- | Numeric value                                                                                                               | "TextureCountU": 7                                   |
| TextureCountV                              |                | MFF-- | Numeric value                                                                                                               | "TextureCountV": 7                                   |

[⬅ Back to Types](Types.md)

## ECZN - EncounterZone

| Field    | Alt            | MFFSM | Value Type                                                           | Example                                              |
| -------- | -------------- | ----- | -------------------------------------------------------------------- | ---------------------------------------------------- |
| Flags    | DataFlags;FNAM | MF--- | Flags (NeverResets, MatchPcBelowMinimumLevel, DisableCombatBoundary) | "Flags": [ "NeverResets", "-DisableCombatBoundary" ] |
| Location | XLCN           | MFF-- | Form Key or Editor ID                                                |                                                      |
| MaxLevel |                | MFF-- | Numeric value                                                        | "MaxLevel": 7                                        |
| MinLevel |                | MFF-- | Numeric value                                                        | "MinLevel": 7                                        |
| Owner    |                | MFF-- | Form Key or Editor ID                                                |                                                      |
| Rank     |                | MFF-- | Numeric value                                                        | "Rank": 7                                            |

[⬅ Back to Types](Types.md)

## EQUP - EquipType

| Field         | Alt  | MFFSM | Value Type              | Example               |
| ------------- | ---- | ----- | ----------------------- | --------------------- |
| SlotParents   | PNAM | MFFSM | Form Keys or Editor IDs |                       |
| UseAllParents | DATA | MFF-- | True / False            | "UseAllParents": true |

[⬅ Back to Types](Types.md)

## EXPL - Explosion

| Field              | Alt            | MFFSM | Value Type                                                                                                                           | Example                                                             |
| ------------------ | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------- |
| Damage             |                | MFF-- | Decimal value                                                                                                                        | "Damage": 3.14                                                      |
| Flags              | DataFlags;FNAM | MF--- | Flags (AlwaysUsesWorldOrientation, KnockDownAlways, KnockDownByFormula, IgnoreLosCheck, PushExplosionSourceRefOnly, IgnoreImageSpaceSwap, Chain, NoControllerVibration) | "Flags": [ "AlwaysUsesWorldOrientation", "-NoControllerVibration" ] |
| Force              |                | MFF-- | Decimal value                                                                                                                        | "Force": 3.14                                                       |
| ImageSpaceModifier | MNAM           | MFF-- | Form Key or Editor ID                                                                                                                |                                                                     |
| ImpactDataSet      |                | MFF-- | Form Key or Editor ID                                                                                                                |                                                                     |
| ISRadius           |                | MFF-- | Decimal value                                                                                                                        | "ISRadius": 3.14                                                    |
| Light              |                | MFF-- | Form Key or Editor ID                                                                                                                |                                                                     |
| Name               | FULL           | MFF-- | String value                                                                                                                         | "Name": "Hello"                                                     |
| ObjectBounds       | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                             | "ObjectBounds": [6,6,6,9,9,9]                                       |
| ObjectEffect       | EITM           | MFF-- | Form Key or Editor ID                                                                                                                |                                                                     |
| PlacedObject       |                | MFF-- | Form Key or Editor ID                                                                                                                |                                                                     |
| Radius             |                | MFF-- | Decimal value                                                                                                                        | "Radius": 3.14                                                      |
| Sound1             |                | MFF-- | Form Key or Editor ID                                                                                                                |                                                                     |
| Sound2             |                | MFF-- | Form Key or Editor ID                                                                                                                |                                                                     |
| SoundLevel         |                | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)                                                                                     | "SoundLevel": "Loud"                                                |
| SpawnProjectile    |                | MFF-- | Form Key or Editor ID                                                                                                                |                                                                     |
| VerticalOffsetMult |                | MFF-- | Decimal value                                                                                                                        | "VerticalOffsetMult": 3.14                                          |

[⬅ Back to Types](Types.md)

## EYES - Eyes

| Field      | Alt            | MFFSM | Value Type                           | Example                               |
| ---------- | -------------- | ----- | ------------------------------------ | ------------------------------------- |
| Flags      | DataFlags;FNAM | MF--- | Flags (Playable, NotMale, NotFemale) | "Flags": [ "Playable", "-NotFemale" ] |
| MajorFlags | RecordFlags    | MF--- | Flags (NonPlayable)                  | "MajorFlags": "NonPlayable"           |
| Name       | FULL           | MFF-- | String value                         | "Name": "Hello"                       |

[⬅ Back to Types](Types.md)

## FACT - Faction

| Field                    | Alt            | MFFSM | Value Type                                                                                                                                          | Example                                        |
| ------------------------ | -------------- | ----- | --------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------- |
| ExteriorJailMarker       | JAIL           | MFF-- | Form Key or Editor ID                                                                                                                               |                                                |
| Flags                    | DataFlags;FNAM | MF--- | Flags (HiddenFromPC, SpecialCombat, TrackCrime, IgnoreMurder, IgnoreAssault, IgnoreStealing, IgnoreTrespass, DoNotReportCrimesAgainstMembers, CrimeGoldUseDefaults, IgnorePickpocket, Vendor, CanBeOwner, IgnoreWerewolf) | "Flags": [ "HiddenFromPC", "-IgnoreWerewolf" ] |
| FollowerWaitMarker       | WAIT           | MFF-- | Form Key or Editor ID                                                                                                                               |                                                |
| JailOutfit               | JOUT           | MFF-- | Form Key or Editor ID                                                                                                                               |                                                |
| MerchantContainer        | VENC           | MFF-- | Form Key or Editor ID                                                                                                                               |                                                |
| Name                     | FULL           | MFF-- | String value                                                                                                                                        | "Name": "Hello"                                |
| PlayerInventoryContainer | PLCN           | MFF-- | Form Key or Editor ID                                                                                                                               |                                                |
| SharedCrimeFactionList   | CRGR           | MFF-- | Form Key or Editor ID                                                                                                                               |                                                |
| StolenGoodsContainer     | STOL           | MFF-- | Form Key or Editor ID                                                                                                                               |                                                |
| VendorBuySellList        | VEND           | MFF-- | Form Key or Editor ID                                                                                                                               |                                                |

[⬅ Back to Types](Types.md)

## FLOR - Flora

| Field                | Alt  | MFFSM | Value Type                                               | Example                         |
| -------------------- | ---- | ----- | -------------------------------------------------------- | ------------------------------- |
| ActivateTextOverride | RNAM | MFF-- | String value                                             | "ActivateTextOverride": "Hello" |
| HarvestSound         | SNAM | MFF-- | Form Key or Editor ID                                    |                                 |
| Ingredient           | PFIG | MFF-- | Form Key or Editor ID                                    |                                 |
| Keywords             | KWDA | MFFSM | Form Keys or Editor IDs                                  |                                 |
| Name                 | FULL | MFF-- | String value                                             | "Name": "Hello"                 |
| ObjectBounds         | OBND | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9]   |

[⬅ Back to Types](Types.md)

## FSTP - Footstep

| Field         | Alt  | MFFSM | Value Type            | Example        |
| ------------- | ---- | ----- | --------------------- | -------------- |
| ImpactDataSet | DATA | MFF-- | Form Key or Editor ID |                |
| Tag           | ANAM | MFF-- | String value          | "Tag": "Hello" |

[⬅ Back to Types](Types.md)

## FSTS - FootstepSet

| Field                          | Alt | MFFSM | Value Type              | Example |
| ------------------------------ | --- | ----- | ----------------------- | ------- |
| RunForwardAlternateFootsteps   |     | MFFSM | Form Keys or Editor IDs |         |
| RunForwardFootsteps            |     | MFFSM | Form Keys or Editor IDs |         |
| WalkForwardAlternateFootsteps  |     | MFFSM | Form Keys or Editor IDs |         |
| WalkForwardAlternateFootsteps2 |     | MFFSM | Form Keys or Editor IDs |         |
| WalkForwardFootsteps           |     | MFFSM | Form Keys or Editor IDs |         |

[⬅ Back to Types](Types.md)

## FLST - FormList

| Field | Alt                      | MFFSM | Value Type              | Example |
| ----- | ------------------------ | ----- | ----------------------- | ------- |
| Items | FormID;FormIDs;Item;LNAM | MFFSM | Form Keys or Editor IDs |         |

[⬅ Back to Types](Types.md)

## FURN - Furniture

| Field              | Alt            | MFFSM | Value Type                                                                             | Example                                            |
| ------------------ | -------------- | ----- | -------------------------------------------------------------------------------------- | -------------------------------------------------- |
| AssociatedSpell    | NAM1           | MFF-- | Form Key or Editor ID                                                                  |                                                    |
| Flags              | DataFlags;FNAM | MF--- | Flags (IgnoredBySandbox, DisablesActivation, IsPerch, MustExitToTalk)                  | "Flags": [ "IgnoredBySandbox", "-MustExitToTalk" ] |
| InteractionKeyword | KNAM           | MFF-- | Form Key or Editor ID                                                                  |                                                    |
| Keywords           | KWDA           | MFFSM | Form Keys or Editor IDs                                                                |                                                    |
| MajorFlags         | RecordFlags    | MF--- | Flags (IsPerch, HasDistantLOD, RandomAnimStart, IsMarker, MustExitToTalk, ChildCanUse) | "MajorFlags": [ "IsPerch", "-ChildCanUse" ]        |
| Name               | FULL           | MFF-- | String value                                                                           | "Name": "Hello"                                    |
| ObjectBounds       | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                               | "ObjectBounds": [6,6,6,9,9,9]                      |

[⬅ Back to Types](Types.md)

## GLOB - Global

| Field      | Alt         | MFFSM | Value Type       | Example                  |
| ---------- | ----------- | ----- | ---------------- | ------------------------ |
| MajorFlags | RecordFlags | MF--- | Flags (Constant) | "MajorFlags": "Constant" |
| TypeChar   |             | MFF-- | Numeric value    | "TypeChar": 7            |

[⬅ Back to Types](Types.md)

## GRAS - Grass

| Field              | Alt            | MFFSM | Value Type                                                                                                                                | Example                                      |
| ------------------ | -------------- | ----- | ----------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| ColorRange         |                | MFF-- | Decimal value                                                                                                                             | "ColorRange": 3.14                           |
| Density            |                | MFF-- | Numeric value                                                                                                                             | "Density": 7                                 |
| Flags              | DataFlags;FNAM | MF--- | Flags (VertexLighting, UniformScaling, FitToSlope)                                                                                        | "Flags": [ "VertexLighting", "-FitToSlope" ] |
| HeightRange        |                | MFF-- | Decimal value                                                                                                                             | "HeightRange": 3.14                          |
| MaxSlope           |                | MFF-- | Numeric value                                                                                                                             | "MaxSlope": 7                                |
| MinSlope           |                | MFF-- | Numeric value                                                                                                                             | "MinSlope": 7                                |
| ObjectBounds       | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                                  | "ObjectBounds": [6,6,6,9,9,9]                |
| PositionRange      |                | MFF-- | Decimal value                                                                                                                             | "PositionRange": 3.14                        |
| UnitsFromWater     |                | MFF-- | Numeric value                                                                                                                             | "UnitsFromWater": 7                          |
| UnitsFromWaterType |                | MF--- | Possible values (AboveAtLeast, AboveAtMost, BelowAtLeast, BelowAtMost, EitherAtLeast, EitherAtMost, EitherAtMostAbove, EitherAtMostBelow) | "UnitsFromWaterType": "AboveAtLeast"         |
| WavePeriod         |                | MFF-- | Decimal value                                                                                                                             | "WavePeriod": 3.14                           |

[⬅ Back to Types](Types.md)

## HAZD - Hazard

| Field              | Alt            | MFFSM | Value Type                                                                                                               | Example                                           |
| ------------------ | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------- |
| Flags              | DataFlags;FNAM | MF--- | Flags (AffectsPlayerOnly, InheritDurationFromSpawnSpell, AlignToImpactNormal, InheritRadiusFromSpawnSpell, DropToGround) | "Flags": [ "AffectsPlayerOnly", "-DropToGround" ] |
| ImageSpaceModifier | MNAM           | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| ImageSpaceRadius   |                | MFF-- | Decimal value                                                                                                            | "ImageSpaceRadius": 3.14                          |
| ImpactDataSet      |                | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| Lifetime           |                | MFF-- | Decimal value                                                                                                            | "Lifetime": 3.14                                  |
| Light              |                | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| Limit              |                | MFF-- | Numeric value                                                                                                            | "Limit": 7                                        |
| Name               | FULL           | MFF-- | String value                                                                                                             | "Name": "Hello"                                   |
| ObjectBounds       | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                 | "ObjectBounds": [6,6,6,9,9,9]                     |
| Radius             |                | MFF-- | Decimal value                                                                                                            | "Radius": 3.14                                    |
| Sound              |                | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| Spell              |                | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| TargetInterval     |                | MFF-- | Decimal value                                                                                                            | "TargetInterval": 3.14                            |

[⬅ Back to Types](Types.md)

## HDPT - HeadPart

| Field      | Alt             | MFFSM | Value Type                                                            | Example                                  |
| ---------- | --------------- | ----- | --------------------------------------------------------------------- | ---------------------------------------- |
| Color      | CNAM            | MFF-- | Form Key or Editor ID                                                 |                                          |
| ExtraParts |                 | MFFSM | Form Keys or Editor IDs                                               |                                          |
| Flags      | DataFlags;FNAM  | MF--- | Flags (Playable, Male, Female, IsExtraPart, UseSolidTint)             | "Flags": [ "Playable", "-UseSolidTint" ] |
| MajorFlags | RecordFlags     | MF--- | Flags (NonPlayable)                                                   | "MajorFlags": "NonPlayable"              |
| Name       | FULL            | MFF-- | String value                                                          | "Name": "Hello"                          |
| TextureSet | TNAM            | MFF-- | Form Key or Editor ID                                                 |                                          |
| Type       | PNAM            | MF--- | Possible values (Misc, Face, Eyes, Hair, FacialHair, Scars, Eyebrows) | "Type": "Misc"                           |
| ValidRaces | Race;Races;RNAM | MFF-- | Form Key or Editor ID                                                 |                                          |

[⬅ Back to Types](Types.md)

## IDLE - IdleAnimation

| Field                 | Alt            | MFFSM | Value Type                                      | Example                            |
| --------------------- | -------------- | ----- | ----------------------------------------------- | ---------------------------------- |
| AnimationEvent        | ENAM           | MFF-- | String value                                    | "AnimationEvent": "Hello"          |
| AnimationGroupSection |                | MFF-- | Numeric value                                   | "AnimationGroupSection": 7         |
| Flags                 | DataFlags;FNAM | MF--- | Flags (Parent, Sequence, NoAttacking, Blocking) | "Flags": [ "Parent", "-Blocking" ] |
| LoopingSecondsMax     |                | MFF-- | Numeric value                                   | "LoopingSecondsMax": 7             |
| LoopingSecondsMin     |                | MFF-- | Numeric value                                   | "LoopingSecondsMin": 7             |
| RelatedIdles          |                | MFFSM | Form Keys or Editor IDs                         |                                    |
| ReplayDelay           |                | MFF-- | Numeric value                                   | "ReplayDelay": 7                   |

[⬅ Back to Types](Types.md)

## IDLM - IdleMarker

| Field        | Alt                 | MFFSM | Value Type                                               | Example                                           |
| ------------ | ------------------- | ----- | -------------------------------------------------------- | ------------------------------------------------- |
| Animations   | IDLA                | MFFSM | Form Keys or Editor IDs                                  |                                                   |
| Flags        | DataFlags;FNAM;IDLF | MF--- | Flags (RunInSequence, DoOnce, IgnoredBySandbox)          | "Flags": [ "RunInSequence", "-IgnoredBySandbox" ] |
| IdleTimer    | IDLT                | MFF-- | Decimal value                                            | "IdleTimer": 3.14                                 |
| MajorFlags   | RecordFlags         | MF--- | Flags (ChildCanUse)                                      | "MajorFlags": "ChildCanUse"                       |
| ObjectBounds | OBND                | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9]                     |

[⬅ Back to Types](Types.md)

## IMAD - ImageSpaceAdapter

| Field               | Alt | MFFSM | Value Type                                                                                    | Example                                                 |
| ------------------- | --- | ----- | --------------------------------------------------------------------------------------------- | ------------------------------------------------------- |
| Animatable          |     | MFF-- | True / False                                                                                  | "Animatable": true                                      |
| DepthOfFieldFlags   |     | MF--- | Flags (UseTarget, ModeFront, ModeBack, NoSky, BlurRadiusBit2, BlurRadiusBit1, BlurRadiusBit0) | "DepthOfFieldFlags": [ "UseTarget", "-BlurRadiusBit0" ] |
| Duration            |     | MFF-- | Decimal value                                                                                 | "Duration": 3.14                                        |
| RadialBlurUseTarget |     | MFF-- | True / False                                                                                  | "RadialBlurUseTarget": true                             |

[⬅ Back to Types](Types.md)

## IPCT - Impact

| Field               | Alt | MFFSM | Value Type                                                              | Example                        |
| ------------------- | --- | ----- | ----------------------------------------------------------------------- | ------------------------------ |
| AngleThreshold      |     | MFF-- | Decimal value                                                           | "AngleThreshold": 3.14         |
| Duration            |     | MFF-- | Decimal value                                                           | "Duration": 3.14               |
| Hazard              |     | MFF-- | Form Key or Editor ID                                                   |                                |
| NoDecalData         |     | MFF-- | True / False                                                            | "NoDecalData": true            |
| Orientation         |     | MF--- | Possible values (SurfaceNormal, ProjectileVector, ProjectileReflection) | "Orientation": "SurfaceNormal" |
| PlacementRadius     |     | MFF-- | Decimal value                                                           | "PlacementRadius": 3.14        |
| Result              |     | MF--- | Possible values (Default, Destroy, Bounce, Impale, Stick)               | "Result": "Default"            |
| SecondaryTextureSet |     | MFF-- | Form Key or Editor ID                                                   |                                |
| Sound1              |     | MFF-- | Form Key or Editor ID                                                   |                                |
| Sound2              |     | MFF-- | Form Key or Editor ID                                                   |                                |
| SoundLevel          |     | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)                        | "SoundLevel": "Loud"           |
| TextureSet          |     | MFF-- | Form Key or Editor ID                                                   |                                |

[⬅ Back to Types](Types.md)

## ALCH - Ingestible

| Field           | Alt            | MFFSM | Value Type                                                        | Example                                                                                |
| --------------- | -------------- | ----- | ----------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| Addiction       |                | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| AddictionChance |                | MFF-- | Decimal value                                                     | "AddictionChance": 3.14                                                                |
| ConsumeSound    |                | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Description     | DESC           | MFF-- | String value                                                      | "Description": "Hello"                                                                 |
| Effects         |                | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipmentType   | ETYP           | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Flags           | DataFlags;FNAM | MF--- | Flags (NoAutoCalc, FoodItem, Medicine, Poison)                    | "Flags": [ "NoAutoCalc", "-Poison" ]                                                   |
| Keywords        | KWDA           | MFFSM | Form Keys or Editor IDs                                           |                                                                                        |
| MajorFlags      | RecordFlags    | MF--- | Flags (Medicine)                                                  | "MajorFlags": "Medicine"                                                               |
| Name            | FULL           | MFF-- | String value                                                      | "Name": "Hello"                                                                        |
| ObjectBounds    | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]          | "ObjectBounds": [6,6,6,9,9,9]                                                          |
| PickUpSound     | YNAM           | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| PutDownSound    | ZNAM           | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Value           |                | MFF-- | Numeric value                                                     | "Value": 7                                                                             |
| Weight          |                | MFF-- | Decimal value                                                     | "Weight": 3.14                                                                         |

[⬅ Back to Types](Types.md)

## INGR - Ingredient

| Field           | Alt            | MFFSM | Value Type                                                        | Example                                                                                |
| --------------- | -------------- | ----- | ----------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| Effects         |                | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipType       | ETYP           | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Flags           | DataFlags;FNAM | MF--- | Flags (NoAutoCalculation, FoodItem, ReferencesPersist)            | "Flags": [ "NoAutoCalculation", "-ReferencesPersist" ]                                 |
| IngredientValue |                | MFF-- | Numeric value                                                     | "IngredientValue": 7                                                                   |
| Keywords        | KWDA           | MFFSM | Form Keys or Editor IDs                                           |                                                                                        |
| Name            | FULL           | MFF-- | String value                                                      | "Name": "Hello"                                                                        |
| ObjectBounds    | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]          | "ObjectBounds": [6,6,6,9,9,9]                                                          |
| PickUpSound     | YNAM           | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| PutDownSound    | ZNAM           | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Value           |                | MFF-- | Numeric value                                                     | "Value": 7                                                                             |
| Weight          |                | MFF-- | Decimal value                                                     | "Weight": 3.14                                                                         |

[⬅ Back to Types](Types.md)

## KEYM - Key

| Field        | Alt         | MFFSM | Value Type                                               | Example                       |
| ------------ | ----------- | ----- | -------------------------------------------------------- | ----------------------------- |
| Keywords     | KWDA        | MFFSM | Form Keys or Editor IDs                                  |                               |
| MajorFlags   | RecordFlags | MF--- | Flags (NonPlayable)                                      | "MajorFlags": "NonPlayable"   |
| Name         | FULL        | MFF-- | String value                                             | "Name": "Hello"               |
| ObjectBounds | OBND        | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9] |
| PickUpSound  | YNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| PutDownSound | ZNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| Value        |             | MFF-- | Numeric value                                            | "Value": 7                    |
| Weight       |             | MFF-- | Decimal value                                            | "Weight": 3.14                |

[⬅ Back to Types](Types.md)

## KYWD - Keyword

| Field | Alt | MFFSM | Value Type                                                                                                                                                                                                   | Example             |
| ----- | --- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------- |
| Color |     | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "Color": [40,50,60] |

[⬅ Back to Types](Types.md)

## LTEX - LandscapeTexture

| Field                   | Alt            | MFFSM | Value Type              | Example                      |
| ----------------------- | -------------- | ----- | ----------------------- | ---------------------------- |
| Flags                   | DataFlags;FNAM | MF--- | Flags (IsSnow)          | "Flags": "IsSnow"            |
| Grasses                 |                | MFFSM | Form Keys or Editor IDs |                              |
| HavokFriction           |                | MFF-- | Numeric value           | "HavokFriction": 7           |
| HavokRestitution        |                | MFF-- | Numeric value           | "HavokRestitution": 7        |
| MaterialType            |                | MFF-- | Form Key or Editor ID   |                              |
| TextureSet              |                | MFF-- | Form Key or Editor ID   |                              |
| TextureSpecularExponent |                | MFF-- | Numeric value           | "TextureSpecularExponent": 7 |

[⬅ Back to Types](Types.md)

## LVLI - LeveledItem

| Field        | Alt            | MFFSM | Value Type                                                                                            | Example                                                                    |
| ------------ | -------------- | ----- | ----------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| ChanceNone   |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                              | "ChanceNone": "30.5%"                                                      |
| Entries      |                | MFFSM | Array of JSON objects containing Item Form Key/Editor ID and level/count data                         | "Entries": [{ "Item": "000ABC:Skyrim.esm", "Level": 36, "Count": 1 }]      |
| Flags        | DataFlags;FNAM | MF--- | Flags (CalculateFromAllLevelsLessThanOrEqualPlayer, CalculateForEachItemInCount, UseAll, SpecialLoot) | "Flags": [ "CalculateFromAllLevelsLessThanOrEqualPlayer", "-SpecialLoot" ] |
| Global       |                | MFF-- | Form Key or Editor ID                                                                                 |                                                                            |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                              | "ObjectBounds": [6,6,6,9,9,9]                                              |

[⬅ Back to Types](Types.md)

## LVLN - LeveledNpc

| Field        | Alt            | MFFSM | Value Type                                                                       | Example                                                                                    |
| ------------ | -------------- | ----- | -------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| ChanceNone   |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                         | "ChanceNone": "30.5%"                                                                      |
| Entries      |                | MFFSM | Array of JSON objects containing NPC Form Key/Editor ID and level/count data     | "Entries": [{ "NPC": "000ABC:Skyrim.esm", "Level": 36, "Count": 1 }]                       |
| Flags        | DataFlags;FNAM | MF--- | Flags (CalculateFromAllLevelsLessThanOrEqualPlayer, CalculateForEachItemInCount) | "Flags": [ "CalculateFromAllLevelsLessThanOrEqualPlayer", "-CalculateForEachItemInCount" ] |
| Global       |                | MFF-- | Form Key or Editor ID                                                            |                                                                                            |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                         | "ObjectBounds": [6,6,6,9,9,9]                                                              |

[⬅ Back to Types](Types.md)

## LVSP - LeveledSpell

| Field        | Alt            | MFFSM | Value Type                                                                                     | Example                                                                     |
| ------------ | -------------- | ----- | ---------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------- |
| ChanceNone   |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                       | "ChanceNone": "30.5%"                                                       |
| Entries      |                | MFFSM | Array of JSON objects containing Item Form Key/Editor ID and level/count data                  | "Entries": [{ "Spell": "000ABC:Skyrim.esm", "Level": 36, "Count": 1 }]      |
| Flags        | DataFlags;FNAM | MF--- | Flags (CalculateFromAllLevelsLessThanOrEqualPlayer, CalculateForEachItemInCount, UseAllSpells) | "Flags": [ "CalculateFromAllLevelsLessThanOrEqualPlayer", "-UseAllSpells" ] |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                       | "ObjectBounds": [6,6,6,9,9,9]                                               |

[⬅ Back to Types](Types.md)

## LIGH - Light

| Field                     | Alt            | MFFSM | Value Type                                                                                                                                       | Example                                          |
| ------------------------- | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------ |
| Color                     |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "Color": [40,50,60]                              |
| FadeValue                 |                | MFF-- | Decimal value                                                                                                                                    | "FadeValue": 3.14                                |
| FalloffExponent           |                | MFF-- | Decimal value                                                                                                                                    | "FalloffExponent": 3.14                          |
| Flags                     | DataFlags;FNAM | MF--- | Flags (Dynamic, CanBeCarried, Negative, Flicker, OffByDefault, FlickerSlow, Pulse, PulseSlow, SpotLight, ShadowSpotlight, ShadowHemisphere, ShadowOmnidirectional, PortalStrict) | "Flags": [ "Dynamic", "-PortalStrict" ]          |
| FlickerIntensityAmplitude |                | MFF-- | Decimal value                                                                                                                                    | "FlickerIntensityAmplitude": 3.14                |
| FlickerMovementAmplitude  |                | MFF-- | Decimal value                                                                                                                                    | "FlickerMovementAmplitude": 3.14                 |
| FlickerPeriod             |                | MFF-- | Decimal value                                                                                                                                    | "FlickerPeriod": 3.14                            |
| FOV                       |                | MFF-- | Decimal value                                                                                                                                    | "FOV": 3.14                                      |
| Lens                      |                | MFF-- | Form Key or Editor ID                                                                                                                            |                                                  |
| MajorFlags                | RecordFlags    | MF--- | Flags (RandomAnimStart, PortalStrict, Obstacle)                                                                                                  | "MajorFlags": [ "RandomAnimStart", "-Obstacle" ] |
| Name                      | FULL           | MFF-- | String value                                                                                                                                     | "Name": "Hello"                                  |
| NearClip                  |                | MFF-- | Decimal value                                                                                                                                    | "NearClip": 3.14                                 |
| ObjectBounds              | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                                         | "ObjectBounds": [6,6,6,9,9,9]                    |
| Radius                    |                | MFF-- | Numeric value                                                                                                                                    | "Radius": 7                                      |
| Sound                     |                | MFF-- | Form Key or Editor ID                                                                                                                            |                                                  |
| Time                      |                | MFF-- | Numeric value                                                                                                                                    | "Time": 7                                        |
| Value                     |                | MFF-- | Numeric value                                                                                                                                    | "Value": 7                                       |
| Weight                    |                | MFF-- | Decimal value                                                                                                                                    | "Weight": 3.14                                   |

[⬅ Back to Types](Types.md)

## LGTM - LightingTemplate

| Field                  | Alt | MFFSM | Value Type                                                                                                                                                                       | Example                        |
| ---------------------- | --- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------ |
| AmbientColor           |     | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "AmbientColor": [40,50,60]     |
| DirectionalColor       |     | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "DirectionalColor": [40,50,60] |
| DirectionalFade        |     | MFF-- | Decimal value                                                                                                                                                                    | "DirectionalFade": 3.14        |
| DirectionalRotationXY  |     | MFF-- | Numeric value                                                                                                                                                                    | "DirectionalRotationXY": 7     |
| DirectionalRotationZ   |     | MFF-- | Numeric value                                                                                                                                                                    | "DirectionalRotationZ": 7      |
| FogClipDistance        |     | MFF-- | Decimal value                                                                                                                                                                    | "FogClipDistance": 3.14        |
| FogFar                 |     | MFF-- | Decimal value                                                                                                                                                                    | "FogFar": 3.14                 |
| FogFarColor            |     | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "FogFarColor": [40,50,60]      |
| FogMax                 |     | MFF-- | Decimal value                                                                                                                                                                    | "FogMax": 3.14                 |
| FogNear                |     | MFF-- | Decimal value                                                                                                                                                                    | "FogNear": 3.14                |
| FogNearColor           |     | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "FogNearColor": [40,50,60]     |
| FogPower               |     | MFF-- | Decimal value                                                                                                                                                                    | "FogPower": 3.14               |
| LightFadeEndDistance   |     | MFF-- | Decimal value                                                                                                                                                                    | "LightFadeEndDistance": 3.14   |
| LightFadeStartDistance |     | MFF-- | Decimal value                                                                                                                                                                    | "LightFadeStartDistance": 3.14 |

[⬅ Back to Types](Types.md)

## LSCR - LoadScreen

| Field            | Alt         | MFFSM | Value Type                 | Example                            |
| ---------------- | ----------- | ----- | -------------------------- | ---------------------------------- |
| Description      | DESC        | MFF-- | String value               | "Description": "Hello"             |
| InitialScale     |             | MFF-- | Decimal value              | "InitialScale": 3.14               |
| LoadingScreenNif |             | MFF-- | Form Key or Editor ID      |                                    |
| MajorFlags       | RecordFlags | MF--- | Flags (DisplaysInMainMenu) | "MajorFlags": "DisplaysInMainMenu" |

[⬅ Back to Types](Types.md)

## LCTN - Location

| Field                             | Alt  | MFFSM | Value Type                                                                                                                                                              | Example                     |
| --------------------------------- | ---- | ----- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------- |
| ActorCellMarkerReference          |      | MFFSM | Form Keys or Editor IDs                                                                                                                                                 |                             |
| Color                             |      | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "Color": [40,50,60]         |
| HorseMarkerRef                    |      | MFF-- | Form Key or Editor ID                                                                                                                                                   |                             |
| Keywords                          | KWDA | MFFSM | Form Keys or Editor IDs                                                                                                                                                 |                             |
| LocationCellMarkerReference       |      | MFFSM | Form Keys or Editor IDs                                                                                                                                                 |                             |
| Music                             |      | MFF-- | Form Key or Editor ID                                                                                                                                                   |                             |
| Name                              | FULL | MFF-- | String value                                                                                                                                                            | "Name": "Hello"             |
| ParentLocation                    |      | MFF-- | Form Key or Editor ID                                                                                                                                                   |                             |
| ReferenceCellPersistentReferences |      | MFFSM | Form Keys or Editor IDs                                                                                                                                                 |                             |
| ReferenceCellStaticReferences     |      | MFFSM | Form Keys or Editor IDs                                                                                                                                                 |                             |
| ReferenceCellUnique               |      | MFFSM | Form Keys or Editor IDs                                                                                                                                                 |                             |
| UnreportedCrimeFaction            |      | MFF-- | Form Key or Editor ID                                                                                                                                                   |                             |
| WorldLocationMarkerRef            |      | MFF-- | Form Key or Editor ID                                                                                                                                                   |                             |
| WorldLocationRadius               |      | MFF-- | Decimal value                                                                                                                                                           | "WorldLocationRadius": 3.14 |

[⬅ Back to Types](Types.md)

## LCRT - LocationReferenceType

| Field | Alt | MFFSM | Value Type                                                                                                                                                                                                   | Example             |
| ----- | --- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------- |
| Color |     | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "Color": [40,50,60] |

[⬅ Back to Types](Types.md)

## MGEF - MagicEffect

| Field                   | Alt            | MFFSM | Value Type                                                                                                                                                 | Example                                  |
| ----------------------- | -------------- | ----- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------- |
| BaseCost                |                | MFF-- | Decimal value                                                                                                                                              | "BaseCost": 3.14                         |
| CastingArt              |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| CastingLight            |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| CastingSoundLevel       |                | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)                                                                                                           | "CastingSoundLevel": "Loud"              |
| CastType                |                | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration)                                                                                             | "CastType": "ConstantEffect"             |
| CounterEffects          |                | MFFSM | Form Keys or Editor IDs                                                                                                                                    |                                          |
| Description             | DESC           | MFF-- | String value                                                                                                                                               | "Description": "Hello"                   |
| DualCastArt             |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| DualCastScale           |                | MFF-- | Decimal value                                                                                                                                              | "DualCastScale": 3.14                    |
| EnchantArt              |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| EnchantShader           |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| EnchantVisuals          |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| EquipAbility            |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| Explosion               |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| Flags                   | DataFlags;FNAM | MF--- | Flags (Hostile, Recover, Detrimental, SnapToNavmesh, NoHitEvent, DispelWithKeywords, NoDuration, NoMagnitude, NoArea, FXPersist, GoryVisuals, HideInUI, NoRecast, PowerAffectsMagnitude, PowerAffectsDuration, Painless, NoHitEffect, NoDeathDispel) | "Flags": [ "Hostile", "-NoDeathDispel" ] |
| HitEffectArt            |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| HitShader               |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| HitVisuals              |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| ImageSpaceModifier      |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| ImpactData              |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| Keywords                | KWDA           | MFFSM | Form Keys or Editor IDs                                                                                                                                    |                                          |
| MagicSkill              |                | MF--- | Possible values (Aggression, Confidence, Energy, Morality, Mood, Assistance, OneHanded, TwoHanded, Archery, Block, Smithing, HeavyArmor, LightArmor, Pickpocket, Lockpicking, Sneak, Alchemy, Speech, Alteration, Conjuration, Destruction, Illusion, Restoration, Enchanting, Health, Magicka, Stamina, HealRate, MagickaRate, StaminaRate, SpeedMult, InventoryWeight, CarryWeight, CriticalChance, MeleeDamage, UnarmedDamage, Mass, VoicePoints, VoiceRate, DamageResist, PoisonResist, ResistFire, ResistShock, ResistFrost, ResistMagic, ResistDisease, Paralysis, Invisibility, NightEye, DetectLifeRange, WaterBreathing, WaterWalking, Fame, Infamy, JumpingBonus, WardPower, RightItemCharge, ArmorPerks, ShieldPerks, WardDeflection, Variable01, Variable02, Variable03, Variable04, Variable05, Variable06, Variable07, Variable08, Variable09, Variable10, BowSpeedBonus, FavorActive, FavorsPerDay, FavorsPerDayTimer, LeftItemCharge, AbsorbChance, Blindness, WeaponSpeedMult, ShoutRecoveryMult, BowStaggerBonus, Telekinesis, FavorPointsBonus, LastBribedIntimidated, LastFlattered, MovementNoiseMult, BypassVendorStolenCheck, BypassVendorKeywordCheck, WaitingForPlayer, OneHandedModifier, TwoHandedModifier, MarksmanModifier, BlockModifier, SmithingModifier, HeavyArmorModifier, LightArmorModifier, PickpocketModifier, LockpickingModifier, SneakingModifier, AlchemyModifier, SpeechcraftModifier, AlterationModifier, ConjurationModifier, DestructionModifier, IllusionModifier, RestorationModifier, EnchantingModifier, OneHandedSkillAdvance, TwoHandedSkillAdvance, MarksmanSkillAdvance, BlockSkillAdvance, SmithingSkillAdvance, HeavyArmorSkillAdvance, LightArmorSkillAdvance, PickpocketSkillAdvance, LockpickingSkillAdvance, SneakingSkillAdvance, AlchemySkillAdvance, SpeechcraftSkillAdvance, AlterationSkillAdvance, ConjurationSkillAdvance, DestructionSkillAdvance, IllusionSkillAdvance, RestorationSkillAdvance, EnchantingSkillAdvance, LeftWeaponSpeedMultiply, DragonSouls, CombatHealthRegenMultiply, OneHandedPowerModifier, TwoHandedPowerModifier, MarksmanPowerModifier, BlockPowerModifier, SmithingPowerModifier, HeavyArmorPowerModifier, LightArmorPowerModifier, PickpocketPowerModifier, LockpickingPowerModifier, SneakingPowerModifier, AlchemyPowerModifier, SpeechcraftPowerModifier, AlterationPowerModifier, ConjurationPowerModifier, DestructionPowerModifier, IllusionPowerModifier, RestorationPowerModifier, EnchantingPowerModifier, DragonRend, AttackDamageMult, HealRateMult, MagickaRateMult, StaminaRateMult, WerewolfPerks, VampirePerks, GrabActorOffset, Grabbed, ReflectDamage, None) | "MagicSkill": "Aggression"               |
| MenuDisplayObject       |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| MinimumSkillLevel       |                | MFF-- | Numeric value                                                                                                                                              | "MinimumSkillLevel": 7                   |
| Name                    | FULL           | MFF-- | String value                                                                                                                                               | "Name": "Hello"                          |
| PerkToApply             |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| Projectile              |                | MFF-- | Form Key or Editor ID                                                                                                                                      |                                          |
| ResistValue             |                | MF--- | Possible values (Aggression, Confidence, Energy, Morality, Mood, Assistance, OneHanded, TwoHanded, Archery, Block, Smithing, HeavyArmor, LightArmor, Pickpocket, Lockpicking, Sneak, Alchemy, Speech, Alteration, Conjuration, Destruction, Illusion, Restoration, Enchanting, Health, Magicka, Stamina, HealRate, MagickaRate, StaminaRate, SpeedMult, InventoryWeight, CarryWeight, CriticalChance, MeleeDamage, UnarmedDamage, Mass, VoicePoints, VoiceRate, DamageResist, PoisonResist, ResistFire, ResistShock, ResistFrost, ResistMagic, ResistDisease, Paralysis, Invisibility, NightEye, DetectLifeRange, WaterBreathing, WaterWalking, Fame, Infamy, JumpingBonus, WardPower, RightItemCharge, ArmorPerks, ShieldPerks, WardDeflection, Variable01, Variable02, Variable03, Variable04, Variable05, Variable06, Variable07, Variable08, Variable09, Variable10, BowSpeedBonus, FavorActive, FavorsPerDay, FavorsPerDayTimer, LeftItemCharge, AbsorbChance, Blindness, WeaponSpeedMult, ShoutRecoveryMult, BowStaggerBonus, Telekinesis, FavorPointsBonus, LastBribedIntimidated, LastFlattered, MovementNoiseMult, BypassVendorStolenCheck, BypassVendorKeywordCheck, WaitingForPlayer, OneHandedModifier, TwoHandedModifier, MarksmanModifier, BlockModifier, SmithingModifier, HeavyArmorModifier, LightArmorModifier, PickpocketModifier, LockpickingModifier, SneakingModifier, AlchemyModifier, SpeechcraftModifier, AlterationModifier, ConjurationModifier, DestructionModifier, IllusionModifier, RestorationModifier, EnchantingModifier, OneHandedSkillAdvance, TwoHandedSkillAdvance, MarksmanSkillAdvance, BlockSkillAdvance, SmithingSkillAdvance, HeavyArmorSkillAdvance, LightArmorSkillAdvance, PickpocketSkillAdvance, LockpickingSkillAdvance, SneakingSkillAdvance, AlchemySkillAdvance, SpeechcraftSkillAdvance, AlterationSkillAdvance, ConjurationSkillAdvance, DestructionSkillAdvance, IllusionSkillAdvance, RestorationSkillAdvance, EnchantingSkillAdvance, LeftWeaponSpeedMultiply, DragonSouls, CombatHealthRegenMultiply, OneHandedPowerModifier, TwoHandedPowerModifier, MarksmanPowerModifier, BlockPowerModifier, SmithingPowerModifier, HeavyArmorPowerModifier, LightArmorPowerModifier, PickpocketPowerModifier, LockpickingPowerModifier, SneakingPowerModifier, AlchemyPowerModifier, SpeechcraftPowerModifier, AlterationPowerModifier, ConjurationPowerModifier, DestructionPowerModifier, IllusionPowerModifier, RestorationPowerModifier, EnchantingPowerModifier, DragonRend, AttackDamageMult, HealRateMult, MagickaRateMult, StaminaRateMult, WerewolfPerks, VampirePerks, GrabActorOffset, Grabbed, ReflectDamage, None) | "ResistValue": "Aggression"              |
| ScriptEffectAIDelayTime |                | MFF-- | Decimal value                                                                                                                                              | "ScriptEffectAIDelayTime": 3.14          |
| ScriptEffectAIScore     |                | MFF-- | Decimal value                                                                                                                                              | "ScriptEffectAIScore": 3.14              |
| SecondActorValue        |                | MF--- | Possible values (Aggression, Confidence, Energy, Morality, Mood, Assistance, OneHanded, TwoHanded, Archery, Block, Smithing, HeavyArmor, LightArmor, Pickpocket, Lockpicking, Sneak, Alchemy, Speech, Alteration, Conjuration, Destruction, Illusion, Restoration, Enchanting, Health, Magicka, Stamina, HealRate, MagickaRate, StaminaRate, SpeedMult, InventoryWeight, CarryWeight, CriticalChance, MeleeDamage, UnarmedDamage, Mass, VoicePoints, VoiceRate, DamageResist, PoisonResist, ResistFire, ResistShock, ResistFrost, ResistMagic, ResistDisease, Paralysis, Invisibility, NightEye, DetectLifeRange, WaterBreathing, WaterWalking, Fame, Infamy, JumpingBonus, WardPower, RightItemCharge, ArmorPerks, ShieldPerks, WardDeflection, Variable01, Variable02, Variable03, Variable04, Variable05, Variable06, Variable07, Variable08, Variable09, Variable10, BowSpeedBonus, FavorActive, FavorsPerDay, FavorsPerDayTimer, LeftItemCharge, AbsorbChance, Blindness, WeaponSpeedMult, ShoutRecoveryMult, BowStaggerBonus, Telekinesis, FavorPointsBonus, LastBribedIntimidated, LastFlattered, MovementNoiseMult, BypassVendorStolenCheck, BypassVendorKeywordCheck, WaitingForPlayer, OneHandedModifier, TwoHandedModifier, MarksmanModifier, BlockModifier, SmithingModifier, HeavyArmorModifier, LightArmorModifier, PickpocketModifier, LockpickingModifier, SneakingModifier, AlchemyModifier, SpeechcraftModifier, AlterationModifier, ConjurationModifier, DestructionModifier, IllusionModifier, RestorationModifier, EnchantingModifier, OneHandedSkillAdvance, TwoHandedSkillAdvance, MarksmanSkillAdvance, BlockSkillAdvance, SmithingSkillAdvance, HeavyArmorSkillAdvance, LightArmorSkillAdvance, PickpocketSkillAdvance, LockpickingSkillAdvance, SneakingSkillAdvance, AlchemySkillAdvance, SpeechcraftSkillAdvance, AlterationSkillAdvance, ConjurationSkillAdvance, DestructionSkillAdvance, IllusionSkillAdvance, RestorationSkillAdvance, EnchantingSkillAdvance, LeftWeaponSpeedMultiply, DragonSouls, CombatHealthRegenMultiply, OneHandedPowerModifier, TwoHandedPowerModifier, MarksmanPowerModifier, BlockPowerModifier, SmithingPowerModifier, HeavyArmorPowerModifier, LightArmorPowerModifier, PickpocketPowerModifier, LockpickingPowerModifier, SneakingPowerModifier, AlchemyPowerModifier, SpeechcraftPowerModifier, AlterationPowerModifier, ConjurationPowerModifier, DestructionPowerModifier, IllusionPowerModifier, RestorationPowerModifier, EnchantingPowerModifier, DragonRend, AttackDamageMult, HealRateMult, MagickaRateMult, StaminaRateMult, WerewolfPerks, VampirePerks, GrabActorOffset, Grabbed, ReflectDamage, None) | "SecondActorValue": "Aggression"         |
| SecondActorValueWeight  |                | MFF-- | Decimal value                                                                                                                                              | "SecondActorValueWeight": 3.14           |
| SkillUsageMultiplier    |                | MFF-- | Decimal value                                                                                                                                              | "SkillUsageMultiplier": 3.14             |
| SpellmakingArea         |                | MFF-- | Numeric value                                                                                                                                              | "SpellmakingArea": 7                     |
| SpellmakingCastingTime  |                | MFF-- | Decimal value                                                                                                                                              | "SpellmakingCastingTime": 3.14           |
| TaperCurve              |                | MFF-- | Decimal value                                                                                                                                              | "TaperCurve": 3.14                       |
| TaperDuration           |                | MFF-- | Decimal value                                                                                                                                              | "TaperDuration": 3.14                    |
| TaperWeight             |                | MFF-- | Decimal value                                                                                                                                              | "TaperWeight": 3.14                      |
| TargetType              |                | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation)                                                                                          | "TargetType": "Self"                     |
| Unknown1                |                | MFF-- | Numeric value                                                                                                                                              | "Unknown1": 7                            |

[⬅ Back to Types](Types.md)

## MATO - MaterialObject

| Field           | Alt            | MFFSM | Value Type                                                                                                                                                                    | Example                       |
| --------------- | -------------- | ----- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------- |
| FalloffBias     |                | MFF-- | Decimal value                                                                                                                                                                 | "FalloffBias": 3.14           |
| FalloffScale    |                | MFF-- | Decimal value                                                                                                                                                                 | "FalloffScale": 3.14          |
| Flags           | DataFlags;FNAM | MF--- | Flags (SinglePass)                                                                                                                                                            | "Flags": "SinglePass"         |
| HasSnow         |                | MFF-- | True / False                                                                                                                                                                  | "HasSnow": true               |
| MaterialUvScale |                | MFF-- | Decimal value                                                                                                                                                                 | "MaterialUvScale": 3.14       |
| NoiseUvScale    |                | MFF-- | Decimal value                                                                                                                                                                 | "NoiseUvScale": 3.14          |
| NormalDampener  |                | MFF-- | Decimal value                                                                                                                                                                 | "NormalDampener": 3.14        |
| SinglePassColor |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "SinglePassColor": [40,50,60] |

[⬅ Back to Types](Types.md)

## MATT - MaterialType

| Field              | Alt            | MFFSM | Value Type                                                                                                                                                  | Example                                      |
| ------------------ | -------------- | ----- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| Buoyancy           |                | MFF-- | Decimal value                                                                                                                                               | "Buoyancy": 3.14                             |
| Flags              | DataFlags;FNAM | MF--- | Flags (StairMaterial, ArrowsStick)                                                                                                                          | "Flags": [ "StairMaterial", "-ArrowsStick" ] |
| HavokDisplayColor  |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "HavokDisplayColor": [40,50,60]              |
| HavokImpactDataSet |                | MFF-- | Form Key or Editor ID                                                                                                                                       |                                              |
| Name               | FULL           | MFF-- | String value                                                                                                                                                | "Name": "Hello"                              |
| Parent             |                | MFF-- | Form Key or Editor ID                                                                                                                                       |                                              |

[⬅ Back to Types](Types.md)

## MESG - Message

| Field       | Alt            | MFFSM | Value Type                      | Example                                   |
| ----------- | -------------- | ----- | ------------------------------- | ----------------------------------------- |
| Description | DESC           | MFF-- | String value                    | "Description": "Hello"                    |
| DisplayTime |                | MFF-- | Numeric value                   | "DisplayTime": 7                          |
| Flags       | DataFlags;FNAM | MF--- | Flags (MessageBox, AutoDisplay) | "Flags": [ "MessageBox", "-AutoDisplay" ] |
| Name        | FULL           | MFF-- | String value                    | "Name": "Hello"                           |
| Quest       |                | MFF-- | Form Key or Editor ID           |                                           |

[⬅ Back to Types](Types.md)

## MISC - MiscItem

| Field        | Alt         | MFFSM | Value Type                                               | Example                       |
| ------------ | ----------- | ----- | -------------------------------------------------------- | ----------------------------- |
| Keywords     | KWDA        | MFFSM | Form Keys or Editor IDs                                  |                               |
| MajorFlags   | RecordFlags | MF--- | Flags (NonPlayable)                                      | "MajorFlags": "NonPlayable"   |
| Name         | FULL        | MFF-- | String value                                             | "Name": "Hello"               |
| ObjectBounds | OBND        | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9] |
| PickUpSound  | YNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| PutDownSound | ZNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| Value        |             | MFF-- | Numeric value                                            | "Value": 7                    |
| Weight       |             | MFF-- | Decimal value                                            | "Weight": 3.14                |

[⬅ Back to Types](Types.md)

## MSTT - MoveableStatic

| Field        | Alt            | MFFSM | Value Type                                                                                                                                     | Example                                                         |
| ------------ | -------------- | ----- | ---------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------- |
| Flags        | DataFlags;FNAM | MF--- | Flags (OnLocalMap)                                                                                                                             | "Flags": "OnLocalMap"                                           |
| LoopingSound |                | MFF-- | Form Key or Editor ID                                                                                                                          |                                                                 |
| MajorFlags   | RecordFlags    | MF--- | Flags (MustUpdateAnims, HiddenFromLocalMap, HasDistantLOD, RandomAnimStart, HasCurrents, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, NavMeshGenerationGround) | "MajorFlags": [ "MustUpdateAnims", "-NavMeshGenerationGround" ] |
| Name         | FULL           | MFF-- | String value                                                                                                                                   | "Name": "Hello"                                                 |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                                       | "ObjectBounds": [6,6,6,9,9,9]                                   |

[⬅ Back to Types](Types.md)

## MOVT - MovementType

| Field                | Alt  | MFFSM | Value Type     | Example                       |
| -------------------- | ---- | ----- | -------------- | ----------------------------- |
| BackRun              |      | MFF-- | Decimal value  | "BackRun": 3.14               |
| BackWalk             |      | MFF-- | Decimal value  | "BackWalk": 3.14              |
| ForwardRun           |      | MFF-- | Decimal value  | "ForwardRun": 3.14            |
| ForwardWalk          |      | MFF-- | Decimal value  | "ForwardWalk": 3.14           |
| LeftRun              |      | MFF-- | Decimal value  | "LeftRun": 3.14               |
| LeftWalk             |      | MFF-- | Decimal value  | "LeftWalk": 3.14              |
| Name                 | FULL | MFF-- | String value   | "Name": "Hello"               |
| RightRun             |      | MFF-- | Decimal value  | "RightRun": 3.14              |
| RightWalk            |      | MFF-- | Decimal value  | "RightWalk": 3.14             |
| RotateInPlaceRun     |      | MFF-- | Decimal value  | "RotateInPlaceRun": 3.14      |
| RotateInPlaceWalk    |      | MFF-- | Decimal value  | "RotateInPlaceWalk": 3.14     |
| RotateWhileMovingRun |      | MFF-- | Decimal value  | "RotateWhileMovingRun": 3.14  |
| SPEDDataTypeState    |      | MF--- | Flags (Break0) | "SPEDDataTypeState": "Break0" |

[⬅ Back to Types](Types.md)

## MUST - MusicTrack

| Field    | Alt | MFFSM | Value Type                                          | Example           |
| -------- | --- | ----- | --------------------------------------------------- | ----------------- |
| Duration |     | MFF-- | Decimal value                                       | "Duration": 3.14  |
| FadeOut  |     | MFF-- | Decimal value                                       | "FadeOut": 3.14   |
| Tracks   |     | MFFSM | Form Keys or Editor IDs                             |                   |
| Type     |     | MF--- | Possible values (Palette, SingleTrack, SilentTrack) | "Type": "Palette" |

[⬅ Back to Types](Types.md)

## MUSC - MusicType

| Field        | Alt            | MFFSM | Value Type                                                                                                    | Example                                           |
| ------------ | -------------- | ----- | ------------------------------------------------------------------------------------------------------------- | ------------------------------------------------- |
| FadeDuration |                | MFF-- | Decimal value                                                                                                 | "FadeDuration": 3.14                              |
| Flags        | DataFlags;FNAM | MF--- | Flags (PlaysOneSelection, AbruptTransition, CycleTracks, MaintainTrackOrder, DucksCurrentTrack, DoesNotQueue) | "Flags": [ "PlaysOneSelection", "-DoesNotQueue" ] |
| Tracks       |                | MFFSM | Form Keys or Editor IDs                                                                                       |                                                   |

[⬅ Back to Types](Types.md)

## NPC_ - Npc

| Field                              | Alt         | MFFSM | Value Type                                                                                                                             | Example                                              |
| ---------------------------------- | ----------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------- |
| ActorEffect                        | SPLO        | MFFSM | Form Keys or Editor IDs                                                                                                                |                                                      |
| AttackRace                         | ATKR        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| Class                              | CNAM        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| CombatOverridePackageList          | ECOR        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| CombatStyle                        | ZNAM        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| CrimeFaction                       | CRIF        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| DeathItem                          | INAM        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| DefaultOutfit                      | DOFT        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| DefaultPackageList                 | DPLT        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| FarAwayModel                       | ANAM        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| GiftFilter                         | GNAM        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| GuardWarnOverridePackageList       | GWOR        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| HairColor                          | HCLF        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| HeadParts                          | PNAM        | MFFSM | Form Keys or Editor IDs                                                                                                                |                                                      |
| HeadTexture                        | FTST        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| Height                             | NAM6        | MFF-- | Decimal value                                                                                                                          | "Height": 3.14                                       |
| Items                              | Item        | MFFSM | JSON objects containing item Form Key/Editor ID and Count (QTY)                                                                        | "Items": { "Item": "021FED:Skyrim.esm", "Count": 3 } |
| Keywords                           | KWDA        | MFFSM | Form Keys or Editor IDs                                                                                                                |                                                      |
| MajorFlags                         | RecordFlags | MF--- | Flags (BleedoutOverride)                                                                                                               | "MajorFlags": "BleedoutOverride"                     |
| NAM5                               |             | MFF-- | Numeric value                                                                                                                          | "NAM5": 7                                            |
| Name                               | FULL        | MFF-- | String value                                                                                                                           | "Name": "Hello"                                      |
| ObjectBounds                       | OBND        | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                               | "ObjectBounds": [6,6,6,9,9,9]                        |
| ObserveDeadBodyOverridePackageList | OCOR        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| Packages                           | PKID        | MFFSM | Form Keys or Editor IDs                                                                                                                |                                                      |
| Race                               | RNAM        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| ShortName                          | ONAM        | MFF-- | String value                                                                                                                           | "ShortName": "Hello"                                 |
| SleepingOutfit                     | SOFT        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| Sound                              |             | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| SoundLevel                         | NAM8        | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)                                                                                       | "SoundLevel": "Loud"                                 |
| SpectatorOverridePackageList       | SPOR        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| Template                           | TPLT        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| TextureLighting                    |             | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "TextureLighting": [40,50,60]                        |
| Voice                              | VTCK        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |
| Weight                             | NAM7        | MFF-- | Decimal value                                                                                                                          | "Weight": 3.14                                       |
| WornArmor                          | WNAM        | MFF-- | Form Key or Editor ID                                                                                                                  |                                                      |

[⬅ Back to Types](Types.md)

## ENCH - ObjectEffect

| Field             | Alt            | MFFSM | Value Type                                                        | Example                                                                                |
| ----------------- | -------------- | ----- | ----------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| BaseEnchantment   |                | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| CastType          |                | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration)    | "CastType": "ConstantEffect"                                                           |
| ChargeTime        |                | MFF-- | Decimal value                                                     | "ChargeTime": 3.14                                                                     |
| Effects           |                | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EnchantmentAmount | EAMT           | MFF-- | Numeric value                                                     | "EnchantmentAmount": 7                                                                 |
| EnchantmentCost   |                | MFF-- | Numeric value                                                     | "EnchantmentCost": 7                                                                   |
| EnchantType       |                | MF--- | Possible values (Enchantment, StaffEnchantment)                   | "EnchantType": "Enchantment"                                                           |
| ENITDataTypeState |                | MF--- | Flags (Break0)                                                    | "ENITDataTypeState": "Break0"                                                          |
| Flags             | DataFlags;FNAM | MF--- | Flags (NoAutoCalc, ExtendDurationOnRecast)                        | "Flags": [ "NoAutoCalc", "-ExtendDurationOnRecast" ]                                   |
| Name              | FULL           | MFF-- | String value                                                      | "Name": "Hello"                                                                        |
| ObjectBounds      | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]          | "ObjectBounds": [6,6,6,9,9,9]                                                          |
| TargetType        |                | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation) | "TargetType": "Self"                                                                   |
| WornRestrictions  |                | MFF-- | Form Key or Editor ID                                             |                                                                                        |

[⬅ Back to Types](Types.md)

## OTFT - Outfit

| Field | Alt  | MFFSM | Value Type              | Example |
| ----- | ---- | ----- | ----------------------- | ------- |
| Items | Item | MFFSM | Form Keys or Editor IDs |         |

[⬅ Back to Types](Types.md)

## PACK - Package

| Field                     | Alt            | MFFSM | Value Type                                                                                                                            | Example                                                     |
| ------------------------- | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------- |
| CombatStyle               |                | MFF-- | Form Key or Editor ID                                                                                                                 |                                                             |
| DataInputVersion          |                | MFF-- | Numeric value                                                                                                                         | "DataInputVersion": 7                                       |
| Flags                     | DataFlags;FNAM | MF--- | Flags (OffersServices, MustComplete, MaintainSpeedAtGoal, UnlockDoorsAtPackageStart, UnlockDoorsAtPackageEnd, ContinueIfPcNear, OncePerDay, PreferredSpeed, AlwaysSneak, AllowSwimming, IgnoreCombat, WeaponsUnequipped, WeaponDrawn, WearSleepOutfit, NoCombatAlert) | "Flags": [ "OffersServices", "-NoCombatAlert" ]             |
| InterruptOverride         |                | MF--- | Possible values (None, Spectator, ObserveDead, GuardWarn, Combat)                                                                     | "InterruptOverride": "None"                                 |
| InteruptFlags             |                | MF--- | Flags (HellosToPlayer, RandomConversations, ObserveCombatBehavior, GreetCorpseBehavior, ReactionToPlayerActions, FriendlyFireComments, AggroRadiusBehavior, AllowIdleChatter, WorldInteractions) | "InteruptFlags": [ "HellosToPlayer", "-WorldInteractions" ] |
| OwnerQuest                |                | MFF-- | Form Key or Editor ID                                                                                                                 |                                                             |
| PackageTemplate           |                | MFF-- | Form Key or Editor ID                                                                                                                 |                                                             |
| PreferredSpeed            |                | MF--- | Possible values (Walk, Jog, Run, FastWalk)                                                                                            | "PreferredSpeed": "Walk"                                    |
| ScheduleDate              |                | MFF-- | Numeric value                                                                                                                         | "ScheduleDate": 7                                           |
| ScheduleDayOfWeek         |                | MF--- | Possible values (Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Weekdays, Weekends, MondayWednesdayFriday, TuesdayThursday, Any) | "ScheduleDayOfWeek": "Sunday"                               |
| ScheduleDurationInMinutes |                | MFF-- | Numeric value                                                                                                                         | "ScheduleDurationInMinutes": 7                              |
| ScheduleHour              |                | MFF-- | Numeric value                                                                                                                         | "ScheduleHour": 7                                           |
| ScheduleMinute            |                | MFF-- | Numeric value                                                                                                                         | "ScheduleMinute": 7                                         |
| ScheduleMonth             |                | MFF-- | Numeric value                                                                                                                         | "ScheduleMonth": 7                                          |
| Type                      |                | MF--- | Possible values (Package, PackageTemplate)                                                                                            | "Type": "Package"                                           |

[⬅ Back to Types](Types.md)

## PERK - Perk

| Field       | Alt         | MFFSM | Value Type                                                        | Example                                                                                |
| ----------- | ----------- | ----- | ----------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| Description | DESC        | MFF-- | String value                                                      | "Description": "Hello"                                                                 |
| Effects     |             | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| Hidden      |             | MFF-- | True / False                                                      | "Hidden": true                                                                         |
| Level       |             | MFF-- | Numeric value                                                     | "Level": 7                                                                             |
| MajorFlags  | RecordFlags | MF--- | Flags (NonPlayable)                                               | "MajorFlags": "NonPlayable"                                                            |
| Name        | FULL        | MFF-- | String value                                                      | "Name": "Hello"                                                                        |
| NextPerk    |             | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| NumRanks    |             | MFF-- | Numeric value                                                     | "NumRanks": 7                                                                          |
| Playable    |             | MFF-- | True / False                                                      | "Playable": true                                                                       |
| Trait       |             | MFF-- | True / False                                                      | "Trait": true                                                                          |

[⬅ Back to Types](Types.md)

## PROJ - Projectile

| Field                        | Alt            | MFFSM | Value Type                                                                                                                                                | Example                              |
| ---------------------------- | -------------- | ----- | --------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------ |
| CollisionLayer               |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| CollisionRadius              |                | MFF-- | Decimal value                                                                                                                                             | "CollisionRadius": 3.14              |
| ConeSpread                   |                | MFF-- | Decimal value                                                                                                                                             | "ConeSpread": 3.14                   |
| CountdownSound               |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| DecalData                    |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| DefaultWeaponSource          |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| DisaleSound                  |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| Explosion                    |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| ExplosionAltTriggerProximity |                | MFF-- | Decimal value                                                                                                                                             | "ExplosionAltTriggerProximity": 3.14 |
| ExplosionAltTriggerTimer     |                | MFF-- | Decimal value                                                                                                                                             | "ExplosionAltTriggerTimer": 3.14     |
| FadeDuration                 |                | MFF-- | Decimal value                                                                                                                                             | "FadeDuration": 3.14                 |
| Flags                        | DataFlags;FNAM | MF--- | Flags (Hitscan, Explosion, AltTrigger, MuzzleFlash, CanBeDisabled, CanBePickedUp, Supersonic, PinsLimbs, PassThroughSmallTransparent, DisableCombatAimCorrection, Rotation) | "Flags": [ "Hitscan", "-Rotation" ]  |
| Gravity                      |                | MFF-- | Decimal value                                                                                                                                             | "Gravity": 3.14                      |
| ImpactForce                  |                | MFF-- | Decimal value                                                                                                                                             | "ImpactForce": 3.14                  |
| Lifetime                     |                | MFF-- | Decimal value                                                                                                                                             | "Lifetime": 3.14                     |
| Light                        |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| MuzzleFlash                  |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| MuzzleFlashDuration          |                | MFF-- | Decimal value                                                                                                                                             | "MuzzleFlashDuration": 3.14          |
| Name                         | FULL           | MFF-- | String value                                                                                                                                              | "Name": "Hello"                      |
| ObjectBounds                 | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                                                  | "ObjectBounds": [6,6,6,9,9,9]        |
| Range                        |                | MFF-- | Decimal value                                                                                                                                             | "Range": 3.14                        |
| RelaunchInterval             |                | MFF-- | Decimal value                                                                                                                                             | "RelaunchInterval": 3.14             |
| Sound                        |                | MFF-- | Form Key or Editor ID                                                                                                                                     |                                      |
| SoundLevel                   |                | MFF-- | Numeric value                                                                                                                                             | "SoundLevel": 7                      |
| Speed                        |                | MFF-- | Decimal value                                                                                                                                             | "Speed": 3.14                        |
| TracerChance                 |                | MFF-- | Decimal value                                                                                                                                             | "TracerChance": 3.14                 |
| Type                         |                | MF--- | Possible values (Missile, Lobber, Beam, Flame, Cone, Barrier, Arrow)                                                                                      | "Type": "Missile"                    |

[⬅ Back to Types](Types.md)

## QUST - Quest

| Field              | Alt            | MFFSM | Value Type                                                                                                                                    | Example                                                    |
| ------------------ | -------------- | ----- | --------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| Description        | DESC           | MFF-- | String value                                                                                                                                  | "Description": "Hello"                                     |
| Filter             |                | MFF-- | String value                                                                                                                                  | "Filter": "Hello"                                          |
| Flags              | DataFlags;FNAM | MF--- | Flags (StartGameEnabled, AllowRepeatedStages, RunOnce, ExcludeFromDialogExport, WarnOnAliasFillFailure)                                       | "Flags": [ "StartGameEnabled", "-WarnOnAliasFillFailure" ] |
| Name               | FULL           | MFF-- | String value                                                                                                                                  | "Name": "Hello"                                            |
| NextAliasID        |                | MFF-- | Numeric value                                                                                                                                 | "NextAliasID": 7                                           |
| Priority           |                | MFF-- | Numeric value                                                                                                                                 | "Priority": 7                                              |
| QuestFormVersion   |                | MFF-- | Numeric value                                                                                                                                 | "QuestFormVersion": 7                                      |
| TextDisplayGlobals |                | MFFSM | Form Keys or Editor IDs                                                                                                                       |                                                            |
| Type               |                | MF--- | Possible values (None, MainQuest, MageGuild, ThievesGuild, DarkBrotherhood, CompanionQuests, Misc, Daedric, SideQuest, CivilWar, Vampire, Dragonborn) | "Type": "None"                                             |

[⬅ Back to Types](Types.md)

## RACE - Race

| Field                     | Alt            | MFFSM | Value Type                                                                                                                                        | Example                                         |
| ------------------------- | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------- |
| AccelerationRate          |                | MFF-- | Decimal value                                                                                                                                     | "AccelerationRate": 3.14                        |
| ActorEffect               |                | MFFSM | Form Keys or Editor IDs                                                                                                                           |                                                 |
| AimAngleTolerance         |                | MFF-- | Decimal value                                                                                                                                     | "AimAngleTolerance": 3.14                       |
| AngularAccelerationRate   |                | MFF-- | Decimal value                                                                                                                                     | "AngularAccelerationRate": 3.14                 |
| AngularTolerance          |                | MFF-- | Decimal value                                                                                                                                     | "AngularTolerance": 3.14                        |
| ArmorRace                 |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| AttackRace                |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| BaseCarryWeight           |                | MFF-- | Decimal value                                                                                                                                     | "BaseCarryWeight": 3.14                         |
| BaseMass                  |                | MFF-- | Decimal value                                                                                                                                     | "BaseMass": 3.14                                |
| BaseMovementDefaultFly    |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| BaseMovementDefaultRun    |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| BaseMovementDefaultSneak  |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| BaseMovementDefaultSprint |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| BaseMovementDefaultSwim   |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| BaseMovementDefaultWalk   |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| BodyBipedObject           |                | MF--- | Possible values (Head, Hair, Body, Hands, Forearms, Amulet, Ring, Feet, Calves, Shield, Tail, LongHair, Circlet, Ears, DecapitateHead, Decapitate, FX01, None) | "BodyBipedObject": "Head"                       |
| BodyPartData              |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| CloseLootSound            |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| DecapitationFX            |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| DecelerationRate          |                | MFF-- | Decimal value                                                                                                                                     | "DecelerationRate": 3.14                        |
| Description               | DESC           | MFF-- | String value                                                                                                                                      | "Description": "Hello"                          |
| EquipmentFlags            |                | MF--- | Flags (HandToHand, OneHandSword, OneHandDagger, OneHandAxe, OneHandMace, TwoHandSword, TwoHandAxe, Bow, Staff, Spell, Shield, Torch, Crossbow)    | "EquipmentFlags": [ "HandToHand", "-Crossbow" ] |
| EquipmentSlots            |                | MFFSM | Form Keys or Editor IDs                                                                                                                           |                                                 |
| ExportingExtraNam2        |                | MFF-- | True / False                                                                                                                                      | "ExportingExtraNam2": true                      |
| Eyes                      |                | MFFSM | Form Keys or Editor IDs                                                                                                                           |                                                 |
| FacegenFaceClamp          |                | MFF-- | Decimal value                                                                                                                                     | "FacegenFaceClamp": 3.14                        |
| FacegenMainClamp          |                | MFF-- | Decimal value                                                                                                                                     | "FacegenMainClamp": 3.14                        |
| Flags                     | DataFlags;FNAM | MF--- | Flags (Playable, FaceGenHead, Child, TiltFrontBack, TiltLeftRight, NoShadow, Swims, Flies, Walks, Immobile, NotPunishable, NoCombatInWater, NoRotatingToHeadTrack, DontShowBloodSpray, DontShowBloodDecal, UsesHeadTrackAnims, SpellsAlignWithMagicNode, UseWorldRaycastsForFootIK, AllowRagdollCollision, RegenHpInCombat, CantOpenDoors, AllowPcDialog, NoKnockdowns, AllowPickpocket, AlwaysUseProxyController, DontShowWeaponBlood, OverlayHeadPartList, OverrideHeadPartList, CanPickupItems, AllowMultipleMembraneShaders, CanDualWield, AvoidsRoads, UseAdvancedAvoidance, NonHostile, AllowMountedCombat) | "Flags": [ "Playable", "-AllowMountedCombat" ]  |
| FlightRadius              |                | MFF-- | Decimal value                                                                                                                                     | "FlightRadius": 3.14                            |
| HairBipedObject           |                | MF--- | Possible values (Head, Hair, Body, Hands, Forearms, Amulet, Ring, Feet, Calves, Shield, Tail, LongHair, Circlet, Ears, DecapitateHead, Decapitate, FX01, None) | "HairBipedObject": "Head"                       |
| Hairs                     |                | MFFSM | Form Keys or Editor IDs                                                                                                                           |                                                 |
| HeadBipedObject           |                | MF--- | Possible values (Head, Hair, Body, Hands, Forearms, Amulet, Ring, Feet, Calves, Shield, Tail, LongHair, Circlet, Ears, DecapitateHead, Decapitate, FX01, None) | "HeadBipedObject": "Head"                       |
| ImpactDataSet             |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| InjuredHealthPercent      |                | MFF-- | Decimal value                                                                                                                                     | "InjuredHealthPercent": 3.14                    |
| Keywords                  | KWDA           | MFFSM | Form Keys or Editor IDs                                                                                                                           |                                                 |
| MajorFlags                | RecordFlags    | MF--- | Flags (Critter)                                                                                                                                   | "MajorFlags": "Critter"                         |
| MaterialType              |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| MorphRace                 |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| Name                      | FULL           | MFF-- | String value                                                                                                                                      | "Name": "Hello"                                 |
| NumberOfTintsInList       |                | MFF-- | Numeric value                                                                                                                                     | "NumberOfTintsInList": 7                        |
| OpenLootSound             |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| ShieldBipedObject         |                | MF--- | Possible values (Head, Hair, Body, Hands, Forearms, Amulet, Ring, Feet, Calves, Shield, Tail, LongHair, Circlet, Ears, DecapitateHead, Decapitate, FX01, None) | "ShieldBipedObject": "Head"                     |
| Size                      |                | MF--- | Possible values (Small, Medium, Large, ExtraLarge)                                                                                                | "Size": "Small"                                 |
| Skin                      |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| UnarmedDamage             |                | MFF-- | Decimal value                                                                                                                                     | "UnarmedDamage": 3.14                           |
| UnarmedEquipSlot          |                | MFF-- | Form Key or Editor ID                                                                                                                             |                                                 |
| UnarmedReach              |                | MFF-- | Decimal value                                                                                                                                     | "UnarmedReach": 3.14                            |
| Weight                    |                | MFF-- | Decimal value                                                                                                                                     | "Weight": 3.14                                  |

[⬅ Back to Types](Types.md)

## REGN - Region

| Field      | Alt         | MFFSM | Value Type                                                                                                                                                                             | Example                      |
| ---------- | ----------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------- |
| MajorFlags | RecordFlags | MF--- | Flags (BorderRegion)                                                                                                                                                                   | "MajorFlags": "BorderRegion" |
| MapColor   |             | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "MapColor": [40,50,60]       |
| Worldspace |             | MFF-- | Form Key or Editor ID                                                                                                                                                                  |                              |

[⬅ Back to Types](Types.md)

## RELA - Relationship

| Field           | Alt            | MFFSM | Value Type                                                                                     | Example                |
| --------------- | -------------- | ----- | ---------------------------------------------------------------------------------------------- | ---------------------- |
| AssociationType |                | MFF-- | Form Key or Editor ID                                                                          |                        |
| Child           |                | MFF-- | Form Key or Editor ID                                                                          |                        |
| Flags           | DataFlags;FNAM | MF--- | Flags (Secret)                                                                                 | "Flags": "Secret"      |
| MajorFlags      | RecordFlags    | MF--- | Flags (Secret)                                                                                 | "MajorFlags": "Secret" |
| Parent          |                | MFF-- | Form Key or Editor ID                                                                          |                        |
| Rank            |                | MF--- | Possible values (Lover, Ally, Confidant, Friend, Acquaintance, Rival, Foe, Enemy, Archnemesis) | "Rank": "Lover"        |

[⬅ Back to Types](Types.md)

## REVB - ReverbParameters

| Field             | Alt | MFFSM | Value Type                                               | Example                     |
| ----------------- | --- | ----- | -------------------------------------------------------- | --------------------------- |
| DecayHfRatio      |     | MFF-- | Decimal value                                            | "DecayHfRatio": 3.14        |
| DecayMilliseconds |     | MFF-- | Numeric value                                            | "DecayMilliseconds": 7      |
| DensityPercent    |     | -FF-- | Decimal value between 0.00 - 1.00, or string ending in % | "DensityPercent": "30.5%"   |
| DiffusionPercent  |     | -FF-- | Decimal value between 0.00 - 1.00, or string ending in % | "DiffusionPercent": "30.5%" |
| HfReferenceHertz  |     | MFF-- | Numeric value                                            | "HfReferenceHertz": 7       |
| ReflectDelayMS    |     | MFF-- | Numeric value                                            | "ReflectDelayMS": 7         |
| Reflections       |     | MFF-- | Numeric value                                            | "Reflections": 7            |
| ReverbAmp         |     | MFF-- | Numeric value                                            | "ReverbAmp": 7              |
| ReverbDelayMS     |     | MFF-- | Numeric value                                            | "ReverbDelayMS": 7          |
| RoomFilter        |     | MFF-- | Numeric value                                            | "RoomFilter": 7             |
| RoomHfFilter      |     | MFF-- | Numeric value                                            | "RoomHfFilter": 7           |

[⬅ Back to Types](Types.md)

## SCEN - Scene

| Field           | Alt            | MFFSM | Value Type                                                                          | Example                                            |
| --------------- | -------------- | ----- | ----------------------------------------------------------------------------------- | -------------------------------------------------- |
| Flags           | DataFlags;FNAM | MF--- | Flags (BeginOnQuestStart, StopQuestOnEnd, RepeatConditionsWhileTrue, Interruptable) | "Flags": [ "BeginOnQuestStart", "-Interruptable" ] |
| LastActionIndex |                | MFF-- | Numeric value                                                                       | "LastActionIndex": 7                               |
| Quest           |                | MFF-- | Form Key or Editor ID                                                               |                                                    |

[⬅ Back to Types](Types.md)

## SCRL - Scroll

| Field             | Alt            | MFFSM | Value Type                                                                                                         | Example                                                                                |
| ----------------- | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------- |
| BaseCost          |                | MFF-- | Numeric value                                                                                                      | "BaseCost": 7                                                                          |
| CastDuration      |                | MFF-- | Decimal value                                                                                                      | "CastDuration": 3.14                                                                   |
| CastType          |                | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration)                                                     | "CastType": "ConstantEffect"                                                           |
| ChargeTime        |                | MFF-- | Decimal value                                                                                                      | "ChargeTime": 3.14                                                                     |
| Description       | DESC           | MFF-- | String value                                                                                                       | "Description": "Hello"                                                                 |
| Effects           |                | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data                                                  | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipmentType     | ETYP           | MFF-- | Form Key or Editor ID                                                                                              |                                                                                        |
| Flags             | DataFlags;FNAM | MF--- | Flags (ManualCostCalc, PCStartSpell, AreaEffectIgnoresLOS, IgnoreResistance, NoAbsorbOrReflect, NoDualCastModification) | "Flags": [ "ManualCostCalc", "-NoDualCastModification" ]                               |
| HalfCostPerk      |                | MFF-- | Form Key or Editor ID                                                                                              |                                                                                        |
| Keywords          | KWDA           | MFFSM | Form Keys or Editor IDs                                                                                            |                                                                                        |
| MenuDisplayObject | MDOB           | MFF-- | Form Key or Editor ID                                                                                              |                                                                                        |
| Name              | FULL           | MFF-- | String value                                                                                                       | "Name": "Hello"                                                                        |
| ObjectBounds      | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                           | "ObjectBounds": [6,6,6,9,9,9]                                                          |
| PickUpSound       | YNAM           | MFF-- | Form Key or Editor ID                                                                                              |                                                                                        |
| PutDownSound      | ZNAM           | MFF-- | Form Key or Editor ID                                                                                              |                                                                                        |
| Range             |                | MFF-- | Decimal value                                                                                                      | "Range": 3.14                                                                          |
| TargetType        |                | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation)                                                  | "TargetType": "Self"                                                                   |
| Type              |                | MF--- | Possible values (Spell, Disease, Power, LesserPower, Ability, Poison, Addiction, Voice)                            | "Type": "Spell"                                                                        |
| Value             |                | MFF-- | Numeric value                                                                                                      | "Value": 7                                                                             |
| Weight            |                | MFF-- | Decimal value                                                                                                      | "Weight": 3.14                                                                         |

[⬅ Back to Types](Types.md)

## SPGD - ShaderParticleGeometry

| Field                | Alt | MFFSM | Value Type                   | Example                      |
| -------------------- | --- | ----- | ---------------------------- | ---------------------------- |
| BoxSize              |     | MFF-- | Numeric value                | "BoxSize": 7                 |
| CenterOffsetMax      |     | MFF-- | Decimal value                | "CenterOffsetMax": 3.14      |
| CenterOffsetMin      |     | MFF-- | Decimal value                | "CenterOffsetMin": 3.14      |
| GravityVelocity      |     | MFF-- | Decimal value                | "GravityVelocity": 3.14      |
| InitialRotationRange |     | MFF-- | Decimal value                | "InitialRotationRange": 3.14 |
| NumSubtexturesX      |     | MFF-- | Numeric value                | "NumSubtexturesX": 7         |
| NumSubtexturesY      |     | MFF-- | Numeric value                | "NumSubtexturesY": 7         |
| ParticleDensity      |     | MFF-- | Decimal value                | "ParticleDensity": 3.14      |
| ParticleSizeX        |     | MFF-- | Decimal value                | "ParticleSizeX": 3.14        |
| ParticleSizeY        |     | MFF-- | Decimal value                | "ParticleSizeY": 3.14        |
| RotationVelocity     |     | MFF-- | Decimal value                | "RotationVelocity": 3.14     |
| Type                 |     | MF--- | Possible values (Rain, Snow) | "Type": "Rain"               |

[⬅ Back to Types](Types.md)

## SHOU - Shout

| Field             | Alt         | MFFSM | Value Type                  | Example                             |
| ----------------- | ----------- | ----- | --------------------------- | ----------------------------------- |
| Description       | DESC        | MFF-- | String value                | "Description": "Hello"              |
| MajorFlags        | RecordFlags | MF--- | Flags (TreatSpellsAsPowers) | "MajorFlags": "TreatSpellsAsPowers" |
| MenuDisplayObject |             | MFF-- | Form Key or Editor ID       |                                     |
| Name              | FULL        | MFF-- | String value                | "Name": "Hello"                     |

[⬅ Back to Types](Types.md)

## SLGM - SoulGem

| Field           | Alt         | MFFSM | Value Type                                                    | Example                        |
| --------------- | ----------- | ----- | ------------------------------------------------------------- | ------------------------------ |
| ContainedSoul   |             | MF--- | Possible values (None, Petty, Lesser, Common, Greater, Grand) | "ContainedSoul": "None"        |
| Keywords        | KWDA        | MFFSM | Form Keys or Editor IDs                                       |                                |
| LinkedTo        |             | MFF-- | Form Key or Editor ID                                         |                                |
| MajorFlags      | RecordFlags | MF--- | Flags (CanHoldNpcSoul)                                        | "MajorFlags": "CanHoldNpcSoul" |
| MaximumCapacity |             | MF--- | Possible values (None, Petty, Lesser, Common, Greater, Grand) | "MaximumCapacity": "None"      |
| Name            | FULL        | MFF-- | String value                                                  | "Name": "Hello"                |
| ObjectBounds    | OBND        | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]      | "ObjectBounds": [6,6,6,9,9,9]  |
| PickUpSound     | YNAM        | MFF-- | Form Key or Editor ID                                         |                                |
| PutDownSound    | ZNAM        | MFF-- | Form Key or Editor ID                                         |                                |
| Value           |             | MFF-- | Numeric value                                                 | "Value": 7                     |
| Weight          |             | MFF-- | Decimal value                                                 | "Weight": 3.14                 |

[⬅ Back to Types](Types.md)

## SNCT - SoundCategory

| Field                  | Alt            | MFFSM | Value Type                                    | Example                                                 |
| ---------------------- | -------------- | ----- | --------------------------------------------- | ------------------------------------------------------- |
| DefaultMenuVolume      |                | MFF-- | Decimal value                                 | "DefaultMenuVolume": 3.14                               |
| Flags                  | DataFlags;FNAM | MF--- | Flags (MuteWhenSubmerged, ShouldAppearOnMenu) | "Flags": [ "MuteWhenSubmerged", "-ShouldAppearOnMenu" ] |
| Name                   | FULL           | MFF-- | String value                                  | "Name": "Hello"                                         |
| Parent                 |                | MFF-- | Form Key or Editor ID                         |                                                         |
| StaticVolumeMultiplier |                | MFF-- | Decimal value                                 | "StaticVolumeMultiplier": 3.14                          |

[⬅ Back to Types](Types.md)

## SNDR - SoundDescriptor

| Field                    | Alt | MFFSM | Value Type                                               | Example                             |
| ------------------------ | --- | ----- | -------------------------------------------------------- | ----------------------------------- |
| AlternateSoundFor        |     | MFF-- | Form Key or Editor ID                                    |                                     |
| Category                 |     | MFF-- | Form Key or Editor ID                                    |                                     |
| OutputModel              |     | MFF-- | Form Key or Editor ID                                    |                                     |
| PercentFrequencyShift    |     | -FF-- | Decimal value between 0.00 - 1.00, or string ending in % | "PercentFrequencyShift": "30.5%"    |
| PercentFrequencyVariance |     | -FF-- | Decimal value between 0.00 - 1.00, or string ending in % | "PercentFrequencyVariance": "30.5%" |
| Priority                 |     | MFF-- | Numeric value                                            | "Priority": 7                       |
| StaticAttenuation        |     | MFF-- | Decimal value                                            | "StaticAttenuation": 3.14           |
| String                   |     | MFF-- | String value                                             | "String": "Hello"                   |
| Type                     |     | MF--- | Possible values (Standard)                               | "Type": "Standard"                  |
| Variance                 |     | MFF-- | Numeric value                                            | "Variance": 7                       |

[⬅ Back to Types](Types.md)

## SOUN - SoundMarker

| Field           | Alt  | MFFSM | Value Type                                               | Example                       |
| --------------- | ---- | ----- | -------------------------------------------------------- | ----------------------------- |
| ObjectBounds    | OBND | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9] |
| SoundDescriptor |      | MFF-- | Form Key or Editor ID                                    |                               |

[⬅ Back to Types](Types.md)

## SOPM - SoundOutputModel

| Field | Alt | MFFSM | Value Type                                       | Example            |
| ----- | --- | ----- | ------------------------------------------------ | ------------------ |
| Type  |     | MF--- | Possible values (UsesHrtf, DefinedSpeakerOutput) | "Type": "UsesHrtf" |

[⬅ Back to Types](Types.md)

## SPEL - Spell

| Field             | Alt            | MFFSM | Value Type                                                                                                         | Example                                                                                |
| ----------------- | -------------- | ----- | ------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------- |
| BaseCost          |                | MFF-- | Numeric value                                                                                                      | "BaseCost": 7                                                                          |
| CastDuration      |                | MFF-- | Decimal value                                                                                                      | "CastDuration": 3.14                                                                   |
| CastType          |                | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration)                                                     | "CastType": "ConstantEffect"                                                           |
| ChargeTime        |                | MFF-- | Decimal value                                                                                                      | "ChargeTime": 3.14                                                                     |
| Description       | DESC           | MFF-- | String value                                                                                                       | "Description": "Hello"                                                                 |
| Effects           |                | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data                                                  | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipmentType     | ETYP           | MFF-- | Form Key or Editor ID                                                                                              |                                                                                        |
| Flags             | DataFlags;FNAM | MF--- | Flags (ManualCostCalc, PCStartSpell, AreaEffectIgnoresLOS, IgnoreResistance, NoAbsorbOrReflect, NoDualCastModification) | "Flags": [ "ManualCostCalc", "-NoDualCastModification" ]                               |
| HalfCostPerk      |                | MFF-- | Form Key or Editor ID                                                                                              |                                                                                        |
| Keywords          | KWDA           | MFFSM | Form Keys or Editor IDs                                                                                            |                                                                                        |
| MenuDisplayObject |                | MFF-- | Form Key or Editor ID                                                                                              |                                                                                        |
| Name              | FULL           | MFF-- | String value                                                                                                       | "Name": "Hello"                                                                        |
| ObjectBounds      | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                           | "ObjectBounds": [6,6,6,9,9,9]                                                          |
| Range             |                | MFF-- | Decimal value                                                                                                      | "Range": 3.14                                                                          |
| TargetType        |                | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation)                                                  | "TargetType": "Self"                                                                   |
| Type              |                | MF--- | Possible values (Spell, Disease, Power, LesserPower, Ability, Poison, Addiction, Voice)                            | "Type": "Spell"                                                                        |

[⬅ Back to Types](Types.md)

## STAT - Static

| Field             | Alt            | MFFSM | Value Type                                                                                                                                     | Example                                                    |
| ----------------- | -------------- | ----- | ---------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| DNAMDataTypeState |                | MF--- | Flags (Break0)                                                                                                                                 | "DNAMDataTypeState": "Break0"                              |
| Flags             | DataFlags;FNAM | MF--- | Flags (ConsideredSnow)                                                                                                                         | "Flags": "ConsideredSnow"                                  |
| MajorFlags        | RecordFlags    | MF--- | Flags (NeverFades, HasTreeLOD, AddOnLODObject, HiddenFromLocalMap, HasDistantLOD, UsesHdLodTexture, HasCurrents, IsMarker, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, ShowInWorldMap, NavMeshGenerationGround) | "MajorFlags": [ "NeverFades", "-NavMeshGenerationGround" ] |
| Material          |                | MFF-- | Form Key or Editor ID                                                                                                                          |                                                            |
| MaxAngle          |                | MFF-- | Decimal value                                                                                                                                  | "MaxAngle": 3.14                                           |
| ObjectBounds      | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]                                                                                       | "ObjectBounds": [6,6,6,9,9,9]                              |

[⬅ Back to Types](Types.md)

## TACT - TalkingActivator

| Field        | Alt         | MFFSM | Value Type                                                | Example                                                 |
| ------------ | ----------- | ----- | --------------------------------------------------------- | ------------------------------------------------------- |
| FNAM         |             | MFF-- | Numeric value                                             | "FNAM": 7                                               |
| Keywords     | KWDA        | MFFSM | Form Keys or Editor IDs                                   |                                                         |
| LoopingSound |             | MFF-- | Form Key or Editor ID                                     |                                                         |
| MajorFlags   | RecordFlags | MF--- | Flags (HiddenFromLocalMap, RandomAnimStart, RadioStation) | "MajorFlags": [ "HiddenFromLocalMap", "-RadioStation" ] |
| Name         | FULL        | MFF-- | String value                                              | "Name": "Hello"                                         |
| ObjectBounds | OBND        | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]  | "ObjectBounds": [6,6,6,9,9,9]                           |
| PNAM         |             | MFF-- | Numeric value                                             | "PNAM": 7                                               |
| VoiceType    |             | MFF-- | Form Key or Editor ID                                     |                                                         |

[⬅ Back to Types](Types.md)

## TXST - TextureSet

| Field        | Alt            | MFFSM | Value Type                                                     | Example                                                 |
| ------------ | -------------- | ----- | -------------------------------------------------------------- | ------------------------------------------------------- |
| Flags        | DataFlags;FNAM | MF--- | Flags (NoSpecularMap, FaceGenTextures, HasModelSpaceNormalMap) | "Flags": [ "NoSpecularMap", "-HasModelSpaceNormalMap" ] |
| ObjectBounds | OBND           | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2]       | "ObjectBounds": [6,6,6,9,9,9]                           |

[⬅ Back to Types](Types.md)

## TREE - Tree

| Field             | Alt         | MFFSM | Value Type                                               | Example                       |
| ----------------- | ----------- | ----- | -------------------------------------------------------- | ----------------------------- |
| BranchFlexibility |             | MFF-- | Decimal value                                            | "BranchFlexibility": 3.14     |
| HarvestSound      |             | MFF-- | Form Key or Editor ID                                    |                               |
| Ingredient        |             | MFF-- | Form Key or Editor ID                                    |                               |
| LeafAmplitude     |             | MFF-- | Decimal value                                            | "LeafAmplitude": 3.14         |
| LeafFrequency     |             | MFF-- | Decimal value                                            | "LeafFrequency": 3.14         |
| MajorFlags        | RecordFlags | MF--- | Flags (HasDistantLOD)                                    | "MajorFlags": "HasDistantLOD" |
| Name              | FULL        | MFF-- | String value                                             | "Name": "Hello"               |
| ObjectBounds      | OBND        | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9] |
| TrunkFlexibility  |             | MFF-- | Decimal value                                            | "TrunkFlexibility": 3.14      |

[⬅ Back to Types](Types.md)

## RFCT - VisualEffect

| Field     | Alt            | MFFSM | Value Type                                                  | Example                                               |
| --------- | -------------- | ----- | ----------------------------------------------------------- | ----------------------------------------------------- |
| EffectArt |                | MFF-- | Form Key or Editor ID                                       |                                                       |
| Flags     | DataFlags;FNAM | MF--- | Flags (RotateToFaceTarget, AttachToCamera, InheritRotation) | "Flags": [ "RotateToFaceTarget", "-InheritRotation" ] |
| Shader    |                | MFF-- | Form Key or Editor ID                                       |                                                       |

[⬅ Back to Types](Types.md)

## VTYP - VoiceType

| Field | Alt            | MFFSM | Value Type                         | Example                                      |
| ----- | -------------- | ----- | ---------------------------------- | -------------------------------------------- |
| Flags | DataFlags;FNAM | MF--- | Flags (AllowDefaultDialog, Female) | "Flags": [ "AllowDefaultDialog", "-Female" ] |

[⬅ Back to Types](Types.md)

## WATR - Water

| Field                          | Alt            | MFFSM | Value Type                                                                                                                                      | Example                                      |
| ------------------------------ | -------------- | ----- | ----------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| DamagePerSecond                |                | MFF-- | Numeric value                                                                                                                                   | "DamagePerSecond": 7                         |
| DeepColor                      |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "DeepColor": [40,50,60]                      |
| DepthNormals                   |                | MFF-- | Decimal value                                                                                                                                   | "DepthNormals": 3.14                         |
| DepthReflections               |                | MFF-- | Decimal value                                                                                                                                   | "DepthReflections": 3.14                     |
| DepthRefraction                |                | MFF-- | Decimal value                                                                                                                                   | "DepthRefraction": 3.14                      |
| DepthSpecularLighting          |                | MFF-- | Decimal value                                                                                                                                   | "DepthSpecularLighting": 3.14                |
| DisplacementDampner            |                | MFF-- | Decimal value                                                                                                                                   | "DisplacementDampner": 3.14                  |
| DisplacementFalloff            |                | MFF-- | Decimal value                                                                                                                                   | "DisplacementFalloff": 3.14                  |
| DisplacementFoce               |                | MFF-- | Decimal value                                                                                                                                   | "DisplacementFoce": 3.14                     |
| DisplacementStartingSize       |                | MFF-- | Decimal value                                                                                                                                   | "DisplacementStartingSize": 3.14             |
| DisplacementVelocity           |                | MFF-- | Decimal value                                                                                                                                   | "DisplacementVelocity": 3.14                 |
| DNAMDataTypeState              |                | MF--- | Flags (Break0)                                                                                                                                  | "DNAMDataTypeState": "Break0"                |
| Flags                          | DataFlags;FNAM | MF--- | Flags (CausesDamage, EnableFlowmap, BlendNormals)                                                                                               | "Flags": [ "CausesDamage", "-BlendNormals" ] |
| FogAboveWaterAmount            |                | MFF-- | Decimal value                                                                                                                                   | "FogAboveWaterAmount": 3.14                  |
| FogAboveWaterDistanceFarPlane  |                | MFF-- | Decimal value                                                                                                                                   | "FogAboveWaterDistanceFarPlane": 3.14        |
| FogAboveWaterDistanceNearPlane |                | MFF-- | Decimal value                                                                                                                                   | "FogAboveWaterDistanceNearPlane": 3.14       |
| FogUnderWaterAmount            |                | MFF-- | Decimal value                                                                                                                                   | "FogUnderWaterAmount": 3.14                  |
| FogUnderWaterDistanceFarPlane  |                | MFF-- | Decimal value                                                                                                                                   | "FogUnderWaterDistanceFarPlane": 3.14        |
| FogUnderWaterDistanceNearPlane |                | MFF-- | Decimal value                                                                                                                                   | "FogUnderWaterDistanceNearPlane": 3.14       |
| ImageSpace                     |                | MFF-- | Form Key or Editor ID                                                                                                                           |                                              |
| Material                       |                | MFF-- | Form Key or Editor ID                                                                                                                           |                                              |
| Name                           | FULL           | MFF-- | String value                                                                                                                                    | "Name": "Hello"                              |
| NoiseFalloff                   |                | MFF-- | Decimal value                                                                                                                                   | "NoiseFalloff": 3.14                         |
| NoiseFlowmapScale              |                | MFF-- | Decimal value                                                                                                                                   | "NoiseFlowmapScale": 3.14                    |
| NoiseLayerOneAmplitudeScale    |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerOneAmplitudeScale": 3.14          |
| NoiseLayerOneUvScale           |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerOneUvScale": 3.14                 |
| NoiseLayerOneWindDirection     |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerOneWindDirection": 3.14           |
| NoiseLayerOneWindSpeed         |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerOneWindSpeed": 3.14               |
| NoiseLayerThreeAmplitudeScale  |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerThreeAmplitudeScale": 3.14        |
| NoiseLayerThreeUvScale         |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerThreeUvScale": 3.14               |
| NoiseLayerThreeWindDirection   |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerThreeWindDirection": 3.14         |
| NoiseLayerThreeWindSpeed       |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerThreeWindSpeed": 3.14             |
| NoiseLayerTwoAmplitudeScale    |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerTwoAmplitudeScale": 3.14          |
| NoiseLayerTwoUvScale           |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerTwoUvScale": 3.14                 |
| NoiseLayerTwoWindDirection     |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerTwoWindDirection": 3.14           |
| NoiseLayerTwoWindSpeed         |                | MFF-- | Decimal value                                                                                                                                   | "NoiseLayerTwoWindSpeed": 3.14               |
| Opacity                        |                | MFF-- | Numeric value                                                                                                                                   | "Opacity": 7                                 |
| OpenSound                      |                | MFF-- | Form Key or Editor ID                                                                                                                           |                                              |
| ReflectionColor                |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "ReflectionColor": [40,50,60]                |
| ShallowColor                   |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "ShallowColor": [40,50,60]                   |
| SpecularBrightness             |                | MFF-- | Decimal value                                                                                                                                   | "SpecularBrightness": 3.14                   |
| SpecularPower                  |                | MFF-- | Decimal value                                                                                                                                   | "SpecularPower": 3.14                        |
| SpecularRadius                 |                | MFF-- | Decimal value                                                                                                                                   | "SpecularRadius": 3.14                       |
| SpecularSunPower               |                | MFF-- | Decimal value                                                                                                                                   | "SpecularSunPower": 3.14                     |
| SpecularSunSparkleMagnitude    |                | MFF-- | Decimal value                                                                                                                                   | "SpecularSunSparkleMagnitude": 3.14          |
| SpecularSunSparklePower        |                | MFF-- | Decimal value                                                                                                                                   | "SpecularSunSparklePower": 3.14              |
| SpecularSunSpecularMagnitude   |                | MFF-- | Decimal value                                                                                                                                   | "SpecularSunSpecularMagnitude": 3.14         |
| Spell                          |                | MFF-- | Form Key or Editor ID                                                                                                                           |                                              |
| WaterFresnel                   |                | MFF-- | Decimal value                                                                                                                                   | "WaterFresnel": 3.14                         |
| WaterReflectionMagnitude       |                | MFF-- | Decimal value                                                                                                                                   | "WaterReflectionMagnitude": 3.14             |
| WaterReflectivity              |                | MFF-- | Decimal value                                                                                                                                   | "WaterReflectivity": 3.14                    |
| WaterRefractionMagnitude       |                | MFF-- | Decimal value                                                                                                                                   | "WaterRefractionMagnitude": 3.14             |

[⬅ Back to Types](Types.md)

## WEAP - Weapon

| Field                  | Alt         | MFFSM | Value Type                                               | Example                       |
| ---------------------- | ----------- | ----- | -------------------------------------------------------- | ----------------------------- |
| AlternateBlockMaterial | BAMT        | MFF-- | Form Key or Editor ID                                    |                               |
| AttackFailSound        | TNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| AttackLoopSound        | NAM7        | MFF-- | Form Key or Editor ID                                    |                               |
| AttackSound            | SNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| AttackSound2D          | XNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| BlockBashImpact        | BIDS        | MFF-- | Form Key or Editor ID                                    |                               |
| Description            | DESC        | MFF-- | String value                                             | "Description": "Hello"        |
| DetectionSoundLevel    |             | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)         | "DetectionSoundLevel": "Loud" |
| EnchantmentAmount      | EAMT        | MFF-- | Numeric value                                            | "EnchantmentAmount": 7        |
| EquipmentType          | ETYP        | MFF-- | Form Key or Editor ID                                    |                               |
| EquipSound             | NAM9        | MFF-- | Form Key or Editor ID                                    |                               |
| FirstPersonModel       |             | MFF-- | Form Key or Editor ID                                    |                               |
| IdleSound              | UNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| ImpactDataSet          | INAM        | MFF-- | Form Key or Editor ID                                    |                               |
| Keywords               | KWDA        | MFFSM | Form Keys or Editor IDs                                  |                               |
| MajorFlags             | RecordFlags | MF--- | Flags (NonPlayable)                                      | "MajorFlags": "NonPlayable"   |
| Name                   | FULL        | MFF-- | String value                                             | "Name": "Hello"               |
| ObjectBounds           | OBND        | -FF-- | Object bounds array of numbers. [x1, y1, z1, x2, y2, z2] | "ObjectBounds": [6,6,6,9,9,9] |
| ObjectEffect           | EITM        | MFF-- | Form Key or Editor ID                                    |                               |
| PickUpSound            | YNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| PutDownSound           | ZNAM        | MFF-- | Form Key or Editor ID                                    |                               |
| Template               |             | MFF-- | Form Key or Editor ID                                    |                               |
| UnequipSound           | NAM8        | MFF-- | Form Key or Editor ID                                    |                               |

[⬅ Back to Types](Types.md)

## WTHR - Weather

| Field                       | Alt            | MFFSM | Value Type                                                                                                                             | Example                                                  |
| --------------------------- | -------------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------- |
| Flags                       | DataFlags;FNAM | MF--- | Flags (Pleasant, Cloudy, Rainy, Snow, SkyStaticsAlwaysVisible, SkyStaticsFollowsSunPosition)                                           | "Flags": [ "Pleasant", "-SkyStaticsFollowsSunPosition" ] |
| FogDistanceDayFar           |                | MFF-- | Decimal value                                                                                                                          | "FogDistanceDayFar": 3.14                                |
| FogDistanceDayMax           |                | MFF-- | Decimal value                                                                                                                          | "FogDistanceDayMax": 3.14                                |
| FogDistanceDayNear          |                | MFF-- | Decimal value                                                                                                                          | "FogDistanceDayNear": 3.14                               |
| FogDistanceDayPower         |                | MFF-- | Decimal value                                                                                                                          | "FogDistanceDayPower": 3.14                              |
| FogDistanceNightFar         |                | MFF-- | Decimal value                                                                                                                          | "FogDistanceNightFar": 3.14                              |
| FogDistanceNightMax         |                | MFF-- | Decimal value                                                                                                                          | "FogDistanceNightMax": 3.14                              |
| FogDistanceNightNear        |                | MFF-- | Decimal value                                                                                                                          | "FogDistanceNightNear": 3.14                             |
| FogDistanceNightPower       |                | MFF-- | Decimal value                                                                                                                          | "FogDistanceNightPower": 3.14                            |
| LightningColor              |                | -FF-- | Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted. | "LightningColor": [40,50,60]                             |
| NAM0DataTypeState           |                | MF--- | Flags (Break0, Break1)                                                                                                                 | "NAM0DataTypeState": [ "Break0", "-Break1" ]             |
| Precipitation               |                | MFF-- | Form Key or Editor ID                                                                                                                  |                                                          |
| PrecipitationBeginFadeIn    |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "PrecipitationBeginFadeIn": "30.5%"                      |
| PrecipitationEndFadeOut     |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "PrecipitationEndFadeOut": "30.5%"                       |
| SkyStatics                  |                | MFFSM | Form Keys or Editor IDs                                                                                                                |                                                          |
| SunDamage                   |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "SunDamage": "30.5%"                                     |
| SunGlare                    |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "SunGlare": "30.5%"                                      |
| SunGlareLensFlare           |                | MFF-- | Form Key or Editor ID                                                                                                                  |                                                          |
| ThunderLightningBeginFadeIn |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "ThunderLightningBeginFadeIn": "30.5%"                   |
| ThunderLightningEndFadeOut  |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "ThunderLightningEndFadeOut": "30.5%"                    |
| ThunderLightningFrequency   |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "ThunderLightningFrequency": "30.5%"                     |
| TransDelta                  |                | MFF-- | Decimal value                                                                                                                          | "TransDelta": 3.14                                       |
| VisualEffect                |                | MFF-- | Form Key or Editor ID                                                                                                                  |                                                          |
| VisualEffectBegin           |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "VisualEffectBegin": "30.5%"                             |
| VisualEffectEnd             |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "VisualEffectEnd": "30.5%"                               |
| WindDirection               |                | MFF-- | Decimal value                                                                                                                          | "WindDirection": 3.14                                    |
| WindDirectionRange          |                | MFF-- | Decimal value                                                                                                                          | "WindDirectionRange": 3.14                               |
| WindSpeed                   |                | -FF-- | Decimal value between 0.00 - 1.00, or string ending in %                                                                               | "WindSpeed": "30.5%"                                     |

[⬅ Back to Types](Types.md)

## WOOP - WordOfPower

| Field       | Alt  | MFFSM | Value Type   | Example                |
| ----------- | ---- | ----- | ------------ | ---------------------- |
| Name        | FULL | MFF-- | String value | "Name": "Hello"        |
| Translation |      | MFF-- | String value | "Translation": "Hello" |

[⬅ Back to Types](Types.md)

## WRLD - Worldspace

| Field                | Alt            | MFFSM | Value Type                                                                                     | Example                               |
| -------------------- | -------------- | ----- | ---------------------------------------------------------------------------------------------- | ------------------------------------- |
| Climate              | CNAM           | MFF-- | Form Key or Editor ID                                                                          |                                       |
| DistantLodMultiplier | NAMA           | MFF-- | Decimal value                                                                                  | "DistantLodMultiplier": 3.14          |
| EncounterZone        | XEZN           | MFF-- | Form Key or Editor ID                                                                          |                                       |
| Flags                | DataFlags;FNAM | MF--- | Flags (SmallWorld, CannotFastTravel, NoLodWater, NoLandscape, NoSky, FixedDimensions, NoGrass) | "Flags": [ "SmallWorld", "-NoGrass" ] |
| InteriorLighting     | LTMP           | MFF-- | Form Key or Editor ID                                                                          |                                       |
| Location             | XLCN           | MFF-- | Form Key or Editor ID                                                                          |                                       |
| LodWater             | NAM3           | MFF-- | Form Key or Editor ID                                                                          |                                       |
| LodWaterHeight       | NAM4           | MFF-- | Decimal value                                                                                  | "LodWaterHeight": 3.14                |
| MajorFlags           | RecordFlags    | MF--- | Flags (CanNotWait)                                                                             | "MajorFlags": "CanNotWait"            |
| Music                | ZNAM           | MFF-- | Form Key or Editor ID                                                                          |                                       |
| Name                 | FULL           | MFF-- | String value                                                                                   | "Name": "Hello"                       |
| Water                | XCWT           | MFF-- | Form Key or Editor ID                                                                          |                                       |
| WorldMapOffsetScale  |                | MFF-- | Decimal value                                                                                  | "WorldMapOffsetScale": 3.14           |

[⬅ Back to Types](Types.md)