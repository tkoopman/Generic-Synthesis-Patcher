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

| Field              | Alt | MFFSM | Value Type            | Example |
| ------------------ | --- | ----- | --------------------- | ------- |
| AmbientSound       |     | MFF-- | Form Key or Editor ID |         |
| EnvironmentType    |     | MFF-- | Form Key or Editor ID |         |
| UseSoundFromRegion |     | MFF-- | Form Key or Editor ID |         |

[⬅ Back to Types](Types.md)

## ACTI - Activator

| Field                | Alt         | MFFSM | Value Type                                                                                                                                     | Example                                                    |
| -------------------- | ----------- | ----- | ---------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| ActivateTextOverride |             | MFF-- | A string value                                                                                                                                 | "ActivateTextOverride": "Hello"                            |
| ActivationSound      |             | MFF-- | Form Key or Editor ID                                                                                                                          |                                                            |
| Flags                | DataFlags   | MF--- | Flags (NoDisplacement, IgnoredBySandbox)                                                                                                       | "Flags": [ "NoDisplacement", "-IgnoredBySandbox" ]         |
| InteractionKeyword   |             | MFF-- | Form Key or Editor ID                                                                                                                          |                                                            |
| Keywords             | KWDA        | MFFSM | Form Keys or Editor IDs                                                                                                                        |                                                            |
| LoopingSound         |             | MFF-- | Form Key or Editor ID                                                                                                                          |                                                            |
| MajorFlags           | RecordFlags | MF--- | Flags (HasTreeLOD, MustUpdateAnims, HiddenFromLocalMap, HasDistantLOD, RandomAnimStart, Dangerous, IgnoreObjectInteraction, IsMarker, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, ChildCanUse, NavMeshGenerationGround) | "MajorFlags": [ "HasTreeLOD", "-NavMeshGenerationGround" ] |
| Name                 | FULL        | MFF-- | A string value                                                                                                                                 | "Name": "Hello"                                            |
| WaterType            |             | MFF-- | Form Key or Editor ID                                                                                                                          |                                                            |

[⬅ Back to Types](Types.md)

## AVIF - ActorValueInformation

| Field        | Alt  | MFFSM | Value Type     | Example                 |
| ------------ | ---- | ----- | -------------- | ----------------------- |
| Abbreviation |      | MFF-- | A string value | "Abbreviation": "Hello" |
| Description  | DESC | MFF-- | A string value | "Description": "Hello"  |
| Name         | FULL | MFF-- | A string value | "Name": "Hello"         |

[⬅ Back to Types](Types.md)

## ADDN - AddonNode

| Field                   | Alt | MFFSM | Value Type            | Example                      |
| ----------------------- | --- | ----- | --------------------- | ---------------------------- |
| MasterParticleSystemCap |     | MFF-- | A numeric value       | "MasterParticleSystemCap": 7 |
| NodeIndex               |     | MFF-- | A numeric value       | "NodeIndex": 7               |
| Sound                   |     | MFF-- | Form Key or Editor ID |                              |

[⬅ Back to Types](Types.md)

## APPA - AlchemicalApparatus

| Field        | Alt  | MFFSM | Value Type                                                       | Example                |
| ------------ | ---- | ----- | ---------------------------------------------------------------- | ---------------------- |
| Description  | DESC | MFF-- | A string value                                                   | "Description": "Hello" |
| Name         | FULL | MFF-- | A string value                                                   | "Name": "Hello"        |
| PickUpSound  | YNAM | MFF-- | Form Key or Editor ID                                            |                        |
| PutDownSound | ZNAM | MFF-- | Form Key or Editor ID                                            |                        |
| Quality      |      | MF--- | Possible values (Novice, Apprentice, Journeyman, Expert, Master) | "Quality": "Novice"    |
| Value        |      | MFF-- | A numeric value                                                  | "Value": 7             |
| Weight       |      | MFF-- | A decimal value                                                  | "Weight": 3.14         |

[⬅ Back to Types](Types.md)

## AMMO - Ammunition

| Field        | Alt         | MFFSM | Value Type                                                  | Example                                                  |
| ------------ | ----------- | ----- | ----------------------------------------------------------- | -------------------------------------------------------- |
| Damage       | DMG         | MFF-- | A decimal value                                             | "Damage": 3.14                                           |
| Description  | DESC        | MFF-- | A string value                                              | "Description": "Hello"                                   |
| Flags        | DataFlags   | MF--- | Flags (IgnoresNormalWeaponResistance, NonPlayable, NonBolt) | "Flags": [ "IgnoresNormalWeaponResistance", "-NonBolt" ] |
| Keywords     | KWDA        | MFFSM | Form Keys or Editor IDs                                     |                                                          |
| MajorFlags   | RecordFlags | MF--- | Flags (NonPlayable)                                         | "MajorFlags": "NonPlayable"                              |
| Name         | FULL        | MFF-- | A string value                                              | "Name": "Hello"                                          |
| PickUpSound  | YNAM        | MFF-- | Form Key or Editor ID                                       |                                                          |
| Projectile   |             | MFF-- | Form Key or Editor ID                                       |                                                          |
| PutDownSound | ZNAM        | MFF-- | Form Key or Editor ID                                       |                                                          |
| ShortName    | ONAM        | MFF-- | A string value                                              | "ShortName": "Hello"                                     |
| Value        |             | MFF-- | A numeric value                                             | "Value": 7                                               |
| Weight       |             | MFF-- | A decimal value                                             | "Weight": 3.14                                           |

[⬅ Back to Types](Types.md)

## ANIO - AnimatedObject

| Field       | Alt | MFFSM | Value Type     | Example                |
| ----------- | --- | ----- | -------------- | ---------------------- |
| UnloadEvent |     | MFF-- | A string value | "UnloadEvent": "Hello" |

[⬅ Back to Types](Types.md)

## ARMA - ArmorAddon

| Field               | Alt  | MFFSM | Value Type              | Example                  |
| ------------------- | ---- | ----- | ----------------------- | ------------------------ |
| AdditionalRaces     |      | MFFSM | Form Keys or Editor IDs |                          |
| ArtObject           |      | MFF-- | Form Key or Editor ID   |                          |
| DetectionSoundValue |      | MFF-- | A numeric value         | "DetectionSoundValue": 7 |
| FootstepSound       |      | MFF-- | Form Key or Editor ID   |                          |
| Race                | RNAM | MFF-- | Form Key or Editor ID   |                          |
| WeaponAdjust        |      | MFF-- | A decimal value         | "WeaponAdjust": 3.14     |

[⬅ Back to Types](Types.md)

## ARMO - Armor

| Field                     | Alt         | MFFSM | Value Type                  | Example                                    |
| ------------------------- | ----------- | ----- | --------------------------- | ------------------------------------------ |
| AlternateBlockMaterial    | BAMT        | MFF-- | Form Key or Editor ID       |                                            |
| Armature                  | MODL        | MFFSM | Form Keys or Editor IDs     |                                            |
| ArmorRating               | DNAM        | MFF-- | A decimal value             | "ArmorRating": 3.14                        |
| BashImpactDataSet         | BIDS        | MFF-- | Form Key or Editor ID       |                                            |
| Description               | DESC        | MFF-- | A string value              | "Description": "Hello"                     |
| EnchantmentAmount         | EAMT        | MFF-- | A numeric value             | "EnchantmentAmount": 7                     |
| EquipmentType             | ETYP        | MFF-- | Form Key or Editor ID       |                                            |
| Keywords                  | KWDA        | MFFSM | Form Keys or Editor IDs     |                                            |
| MajorFlags                | RecordFlags | MF--- | Flags (NonPlayable, Shield) | "MajorFlags": [ "NonPlayable", "-Shield" ] |
| Name                      | FULL        | MFF-- | A string value              | "Name": "Hello"                            |
| ObjectEffect              | EITM        | MFF-- | Form Key or Editor ID       |                                            |
| PickUpSound               | YNAM        | MFF-- | Form Key or Editor ID       |                                            |
| PutDownSound              | ZNAM        | MFF-- | Form Key or Editor ID       |                                            |
| Race                      | RNAM        | MFF-- | Form Key or Editor ID       |                                            |
| RagdollConstraintTemplate | BMCT        | MFF-- | A string value              | "RagdollConstraintTemplate": "Hello"       |
| TemplateArmor             | TNAM        | MFF-- | Form Key or Editor ID       |                                            |
| Value                     |             | MFF-- | A numeric value             | "Value": 7                                 |
| Weight                    |             | MFF-- | A decimal value             | "Weight": 3.14                             |

[⬅ Back to Types](Types.md)

## ARTO - ArtObject

| Field | Alt | MFFSM | Value Type                                              | Example                                          |
| ----- | --- | ----- | ------------------------------------------------------- | ------------------------------------------------ |
| Type  |     | MF--- | Flags (MagicCasting, MagicHitEffect, EnchantmentEffect) | "Type": [ "MagicCasting", "-EnchantmentEffect" ] |

[⬅ Back to Types](Types.md)

## BOOK - Book

| Field        | Alt       | MFFSM | Value Type                                 | Example                |
| ------------ | --------- | ----- | ------------------------------------------ | ---------------------- |
| BookText     | DESC      | MFF-- | A string value                             | "BookText": "Hello"    |
| Description  | CNAM;DESC | MFF-- | A string value                             | "Description": "Hello" |
| Flags        | DataFlags | MF--- | Flags (CantBeTaken)                        | "Flags": "CantBeTaken" |
| InventoryArt | INAM      | MFF-- | Form Key or Editor ID                      |                        |
| Keywords     | KWDA      | MFFSM | Form Keys or Editor IDs                    |                        |
| Name         | FULL      | MFF-- | A string value                             | "Name": "Hello"        |
| PickUpSound  | YNAM      | MFF-- | Form Key or Editor ID                      |                        |
| PutDownSound | ZNAM      | MFF-- | Form Key or Editor ID                      |                        |
| Type         |           | MF--- | Possible values (BookOrTome, NoteOrScroll) | "Type": "BookOrTome"   |
| Value        |           | MFF-- | A numeric value                            | "Value": 7             |
| Weight       |           | MFF-- | A decimal value                            | "Weight": 3.14         |

[⬅ Back to Types](Types.md)

## CPTH - CameraPath

| Field        | Alt | MFFSM | Value Type                         | Example                            |
| ------------ | --- | ----- | ---------------------------------- | ---------------------------------- |
| RelatedPaths |     | MFFSM | Form Keys or Editor IDs            |                                    |
| Shots        |     | MFFSM | Form Keys or Editor IDs            |                                    |
| Zoom         |     | MF--- | Flags (Default, Disable, ShotList) | "Zoom": [ "Default", "-ShotList" ] |

[⬅ Back to Types](Types.md)

## CAMS - CameraShot

| Field                      | Alt       | MFFSM | Value Type                                                                                                            | Example                                                    |
| -------------------------- | --------- | ----- | --------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| Action                     |           | MF--- | Possible values (Shoot, Fly, Hit, Zoom)                                                                               | "Action": "Shoot"                                          |
| Flags                      | DataFlags | MF--- | Flags (PositionFollowsLocation, RotationFollowsTarget, DoNotFollowBone, FirstPersonCamera, NoTracer, StartAtTimeZero) | "Flags": [ "PositionFollowsLocation", "-StartAtTimeZero" ] |
| ImageSpaceModifier         |           | MFF-- | Form Key or Editor ID                                                                                                 |                                                            |
| Location                   | XLCN      | MF--- | Possible values (Attacker, Projectile, Target, LeadActor)                                                             | "Location": "Attacker"                                     |
| MaxTime                    |           | MFF-- | A decimal value                                                                                                       | "MaxTime": 3.14                                            |
| MinTime                    |           | MFF-- | A decimal value                                                                                                       | "MinTime": 3.14                                            |
| NearTargetDistance         |           | MFF-- | A decimal value                                                                                                       | "NearTargetDistance": 3.14                                 |
| Target                     |           | MF--- | Possible values (Attacker, Projectile, Target, LeadActor)                                                             | "Target": "Attacker"                                       |
| TargetPercentBetweenActors |           | MFF-- | A decimal value                                                                                                       | "TargetPercentBetweenActors": 3.14                         |
| TimeMultiplierGlobal       |           | MFF-- | A decimal value                                                                                                       | "TimeMultiplierGlobal": 3.14                               |
| TimeMultiplierPlayer       |           | MFF-- | A decimal value                                                                                                       | "TimeMultiplierPlayer": 3.14                               |
| TimeMultiplierTarget       |           | MFF-- | A decimal value                                                                                                       | "TimeMultiplierTarget": 3.14                               |

[⬅ Back to Types](Types.md)

## CELL - Cell

| Field                   | Alt         | MFFSM | Value Type                                                                                                         | Example                                          |
| ----------------------- | ----------- | ----- | ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------ |
| AcousticSpace           | XCAS        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| EncounterZone           | XEZN        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| FactionRank             |             | MFF-- | A numeric value                                                                                                    | "FactionRank": 7                                 |
| Flags                   | DataFlags   | MF--- | Flags (IsInteriorCell, HasWater, CantTravelFromHere, NoLodWater, PublicArea, HandChanged, ShowSky, UseSkyLighting) | "Flags": [ "IsInteriorCell", "-UseSkyLighting" ] |
| ImageSpace              | XCIM        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| LightingTemplate        | LTMP        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| Location                | XLCN        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| LockList                | XILL        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| MajorFlags              | RecordFlags | MF--- | Flags (Persistent, OffLimits, CantWait)                                                                            | "MajorFlags": [ "Persistent", "-CantWait" ]      |
| Music                   | ZNAM        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| Name                    | FULL        | MFF-- | A string value                                                                                                     | "Name": "Hello"                                  |
| Owner                   |             | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| Regions                 |             | MFFSM | Form Keys or Editor IDs                                                                                            |                                                  |
| SkyAndWeatherFromRegion | XCCM        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| Water                   | XCWT        | MFF-- | Form Key or Editor ID                                                                                              |                                                  |
| WaterEnvironmentMap     | XWEM        | MFF-- | A string value                                                                                                     | "WaterEnvironmentMap": "Hello"                   |
| WaterHeight             | XCLW        | MFF-- | A decimal value                                                                                                    | "WaterHeight": 3.14                              |
| WaterNoiseTexture       | XNAM        | MFF-- | A string value                                                                                                     | "WaterNoiseTexture": "Hello"                     |

[⬅ Back to Types](Types.md)

## CLAS - Class

| Field            | Alt  | MFFSM | Value Type                                                                                                                                                                                   | Example                 |
| ---------------- | ---- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------- |
| BleedoutDefault  |      | MFF-- | A decimal value                                                                                                                                                                              | "BleedoutDefault": 3.14 |
| Description      | DESC | MFF-- | A string value                                                                                                                                                                               | "Description": "Hello"  |
| Icon             |      | MFF-- | A string value                                                                                                                                                                               | "Icon": "Hello"         |
| MaxTrainingLevel |      | MFF-- | A numeric value                                                                                                                                                                              | "MaxTrainingLevel": 7   |
| Name             | FULL | MFF-- | A string value                                                                                                                                                                               | "Name": "Hello"         |
| Teaches          |      | MF--- | Possible values (OneHanded, TwoHanded, Archery, Block, Smithing, HeavyArmor, LightArmor, Pickpocket, Lockpicking, Sneak, Alchemy, Speech, Alteration, Conjuration, Destruction, Illusion, Restoration, Enchanting) | "Teaches": "OneHanded"  |
| VoicePoints      |      | MFF-- | A numeric value                                                                                                                                                                              | "VoicePoints": 7        |

[⬅ Back to Types](Types.md)

## CLMT - Climate

| Field       | Alt | MFFSM | Value Type              | Example                           |
| ----------- | --- | ----- | ----------------------- | --------------------------------- |
| Moons       |     | MF--- | Flags (Masser, Secunda) | "Moons": [ "Masser", "-Secunda" ] |
| PhaseLength |     | MFF-- | A numeric value         | "PhaseLength": 7                  |
| Volatility  |     | MFF-- | A numeric value         | "Volatility": 7                   |

[⬅ Back to Types](Types.md)

## COLL - CollisionLayer

| Field        | Alt       | MFFSM | Value Type                                     | Example                                          |
| ------------ | --------- | ----- | ---------------------------------------------- | ------------------------------------------------ |
| CollidesWith |           | MFFSM | Form Keys or Editor IDs                        |                                                  |
| Description  | DESC      | MFF-- | A string value                                 | "Description": "Hello"                           |
| Flags        | DataFlags | MF--- | Flags (TriggerVolume, Sensor, NavmeshObstacle) | "Flags": [ "TriggerVolume", "-NavmeshObstacle" ] |
| Index        |           | MFF-- | A numeric value                                | "Index": 7                                       |
| Name         | FULL      | MFF-- | A string value                                 | "Name": "Hello"                                  |

[⬅ Back to Types](Types.md)

## CLFM - ColorRecord

| Field | Alt  | MFFSM | Value Type     | Example         |
| ----- | ---- | ----- | -------------- | --------------- |
| Name  | FULL | MFF-- | A string value | "Name": "Hello" |

[⬅ Back to Types](Types.md)

## CSTY - CombatStyle

| Field                     | Alt         | MFFSM | Value Type                                   | Example                                      |
| ------------------------- | ----------- | ----- | -------------------------------------------- | -------------------------------------------- |
| AvoidThreatChance         |             | MFF-- | A decimal value                              | "AvoidThreatChance": 3.14                    |
| CSGDDataTypeState         |             | MF--- | Flags (Break0, Break1)                       | "CSGDDataTypeState": [ "Break0", "-Break1" ] |
| DefensiveMult             |             | MFF-- | A decimal value                              | "DefensiveMult": 3.14                        |
| EquipmentScoreMultMagic   |             | MFF-- | A decimal value                              | "EquipmentScoreMultMagic": 3.14              |
| EquipmentScoreMultMelee   |             | MFF-- | A decimal value                              | "EquipmentScoreMultMelee": 3.14              |
| EquipmentScoreMultRanged  |             | MFF-- | A decimal value                              | "EquipmentScoreMultRanged": 3.14             |
| EquipmentScoreMultShout   |             | MFF-- | A decimal value                              | "EquipmentScoreMultShout": 3.14              |
| EquipmentScoreMultStaff   |             | MFF-- | A decimal value                              | "EquipmentScoreMultStaff": 3.14              |
| EquipmentScoreMultUnarmed |             | MFF-- | A decimal value                              | "EquipmentScoreMultUnarmed": 3.14            |
| Flags                     | DataFlags   | MF--- | Flags (Dueling, Flanking, AllowDualWielding) | "Flags": [ "Dueling", "-AllowDualWielding" ] |
| GroupOffensiveMult        |             | MFF-- | A decimal value                              | "GroupOffensiveMult": 3.14                   |
| LongRangeStrafeMult       |             | MFF-- | A decimal value                              | "LongRangeStrafeMult": 3.14                  |
| MajorFlags                | RecordFlags | MF--- | Flags (AllowDualWielding)                    | "MajorFlags": "AllowDualWielding"            |
| OffensiveMult             |             | MFF-- | A decimal value                              | "OffensiveMult": 3.14                        |

[⬅ Back to Types](Types.md)

## COBJ - ConstructibleObject

| Field              | Alt  | MFFSM | Value Type                                                      | Example                                              |
| ------------------ | ---- | ----- | --------------------------------------------------------------- | ---------------------------------------------------- |
| CreatedObject      |      | MFF-- | Form Key or Editor ID                                           |                                                      |
| CreatedObjectCount |      | MFF-- | A numeric value                                                 | "CreatedObjectCount": 7                              |
| Items              | Item | MFFSM | JSON objects containing item Form Key/Editor ID and Count (QTY) | "Items": { "Item": "021FED:Skyrim.esm", "Count": 3 } |
| WorkbenchKeyword   |      | MFF-- | Form Key or Editor ID                                           |                                                      |

[⬅ Back to Types](Types.md)

## CONT - Container

| Field      | Alt         | MFFSM | Value Type                                                                                                                       | Example                                                       |
| ---------- | ----------- | ----- | -------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------- |
| CloseSound | QNAM        | MFF-- | Form Key or Editor ID                                                                                                            |                                                               |
| Flags      | DataFlags   | MF--- | Flags (AllowSoundsWhenAnimation, Respawns, ShowOwner)                                                                            | "Flags": [ "AllowSoundsWhenAnimation", "-ShowOwner" ]         |
| Items      | Item        | MFFSM | JSON objects containing item Form Key/Editor ID and Count (QTY)                                                                  | "Items": { "Item": "021FED:Skyrim.esm", "Count": 3 }          |
| MajorFlags | RecordFlags | MF--- | Flags (HasDistantLOD, RandomAnimStart, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, NavMeshGenerationGround) | "MajorFlags": [ "HasDistantLOD", "-NavMeshGenerationGround" ] |
| Name       | FULL        | MFF-- | A string value                                                                                                                   | "Name": "Hello"                                               |
| OpenSound  | SNAM        | MFF-- | Form Key or Editor ID                                                                                                            |                                                               |
| Weight     |             | MFF-- | A decimal value                                                                                                                  | "Weight": 3.14                                                |

[⬅ Back to Types](Types.md)

## DLBR - DialogBranch

| Field         | Alt       | MFFSM | Value Type                            | Example                               |
| ------------- | --------- | ----- | ------------------------------------- | ------------------------------------- |
| Category      |           | MF--- | Possible values (Player, Command)     | "Category": "Player"                  |
| Flags         | DataFlags | MF--- | Flags (TopLevel, Blocking, Exclusive) | "Flags": [ "TopLevel", "-Exclusive" ] |
| Quest         |           | MFF-- | Form Key or Editor ID                 |                                       |
| StartingTopic |           | MFF-- | Form Key or Editor ID                 |                                       |

[⬅ Back to Types](Types.md)

## DIAL - DialogTopic

| Field      | Alt  | MFFSM | Value Type                                                                                                                                                                            | Example                              |
| ---------- | ---- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------ |
| Branch     |      | MFF-- | Form Key or Editor ID                                                                                                                                                                 |                                      |
| Category   |      | MF--- | Possible values (Topic, Favor, Scene, Combat, Favors, Detection, Service, Misc)                                                                                                       | "Category": "Topic"                  |
| Name       | FULL | MFF-- | A string value                                                                                                                                                                        | "Name": "Hello"                      |
| Priority   |      | MFF-- | A decimal value                                                                                                                                                                       | "Priority": 3.14                     |
| Quest      |      | MFF-- | Form Key or Editor ID                                                                                                                                                                 |                                      |
| Subtype    |      | MF--- | Possible values (Custom, ForceGreet, Rumors, Intimidate, Flatter, Bribe, AskGift, Gift, AskFavor, Favor, ShowRelationships, Follow, Reject, Scene, Show, Agree, Refuse, ExitFavorState, MoralRefusal, FlyingMountLand, FlyingMountCancelLand, FlyingMountAcceptTarget, FlyingMountRejectTarget, FlyingMountNoTarget, FlyingMountDestinationReached, Attack, PowerAttack, Bash, Hit, Flee, Bleedout, AvoidThreat, Death, GroupStrategy, Block, Taunt, AllyKilled, Steal, Yield, AcceptYield, PickpocketCombat, Assault, Murder, AssaultNC, MurderNC, PickpocketNC, StealFromNC, TrespassAgainstNC, Trespass, WerewolfTransformCrime, VoicePowerStartShort, VoicePowerStartLong, VoicePowerEndShort, VoicePowerEndLong, AlertIdle, LostIdle, NormalToAlert, AlertToCombat, NormalToCombat, AlertToNormal, CombatToNormal, CombatToLost, LostToNormal, LostToCombat, DetectFriendDie, ServiceRefusal, Repair, Travel, Training, BarterExit, RepairExit, Recharge, RechargeExit, TrainingExit, ObserveCombat, NoticeCorpse, TimeToGo, Goodbye, Hello, SwingMeleeWeapon, ShootBow, ZKeyObject, Jump, KnockOverObject, DestroyObject, StandOnFurniture, LockedObject, PickpocketTopic, PursueIdleTopic, SharedInfo, PlayerCastProjectileSpell, PlayerCastSelfSpell, PlayerShout, Idle, EnterSprintBreath, EnterBowZoomBreath, ExitBowZoomBreath, ActorCollideWithActor, PlayerInIronSights, OutOfBreath, CombatGrunt, LeaveWaterBreath) | "Subtype": "Custom"                  |
| TopicFlags |      | MF--- | Flags (DoAllBeforeRepeating)                                                                                                                                                          | "TopicFlags": "DoAllBeforeRepeating" |

[⬅ Back to Types](Types.md)

## DLVW - DialogView

| Field    | Alt | MFFSM | Value Type              | Example |
| -------- | --- | ----- | ----------------------- | ------- |
| Branches |     | MFFSM | Form Keys or Editor IDs |         |
| Quest    |     | MFF-- | Form Key or Editor ID   |         |

[⬅ Back to Types](Types.md)

## DOOR - Door

| Field      | Alt         | MFFSM | Value Type                                                              | Example                                              |
| ---------- | ----------- | ----- | ----------------------------------------------------------------------- | ---------------------------------------------------- |
| CloseSound |             | MFF-- | Form Key or Editor ID                                                   |                                                      |
| Flags      | DataFlags   | MF--- | Flags (Automatic, Hidden, MinimalUse, Sliding, DoNotOpenInCombatSearch) | "Flags": [ "Automatic", "-DoNotOpenInCombatSearch" ] |
| LoopSound  |             | MFF-- | Form Key or Editor ID                                                   |                                                      |
| MajorFlags | RecordFlags | MF--- | Flags (HasDistantLOD, RandomAnimStart, IsMarker)                        | "MajorFlags": [ "HasDistantLOD", "-IsMarker" ]       |
| Name       | FULL        | MFF-- | A string value                                                          | "Name": "Hello"                                      |
| OpenSound  |             | MFF-- | Form Key or Editor ID                                                   |                                                      |

[⬅ Back to Types](Types.md)

## DUAL - DualCastData

| Field         | Alt | MFFSM | Value Type                                  | Example                                          |
| ------------- | --- | ----- | ------------------------------------------- | ------------------------------------------------ |
| EffectShader  |     | MFF-- | Form Key or Editor ID                       |                                                  |
| Explosion     |     | MFF-- | Form Key or Editor ID                       |                                                  |
| HitEffectArt  |     | MFF-- | Form Key or Editor ID                       |                                                  |
| ImpactDataSet |     | MFF-- | Form Key or Editor ID                       |                                                  |
| InheritScale  |     | MF--- | Flags (HitEffectArt, Projectile, Explosion) | "InheritScale": [ "HitEffectArt", "-Explosion" ] |
| Projectile    |     | MFF-- | Form Key or Editor ID                       |                                                  |

[⬅ Back to Types](Types.md)

## EFSH - EffectShader

| Field                                      | Alt       | MFFSM | Value Type                                                                                                                       | Example                                              |
| ------------------------------------------ | --------- | ----- | -------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------- |
| AddonModels                                |           | MFF-- | Form Key or Editor ID                                                                                                            |                                                      |
| AddonModelsFadeInTime                      |           | MFF-- | A decimal value                                                                                                                  | "AddonModelsFadeInTime": 3.14                        |
| AddonModelsFadeOutTime                     |           | MFF-- | A decimal value                                                                                                                  | "AddonModelsFadeOutTime": 3.14                       |
| AddonModelsScaleEnd                        |           | MFF-- | A decimal value                                                                                                                  | "AddonModelsScaleEnd": 3.14                          |
| AddonModelsScaleInTime                     |           | MFF-- | A decimal value                                                                                                                  | "AddonModelsScaleInTime": 3.14                       |
| AddonModelsScaleOutTime                    |           | MFF-- | A decimal value                                                                                                                  | "AddonModelsScaleOutTime": 3.14                      |
| AddonModelsScaleStart                      |           | MFF-- | A decimal value                                                                                                                  | "AddonModelsScaleStart": 3.14                        |
| AmbientSound                               |           | MFF-- | Form Key or Editor ID                                                                                                            |                                                      |
| BirthPositionOffset                        |           | MFF-- | A decimal value                                                                                                                  | "BirthPositionOffset": 3.14                          |
| BirthPositionOffsetRangePlusMinus          |           | MFF-- | A decimal value                                                                                                                  | "BirthPositionOffsetRangePlusMinus": 3.14            |
| ColorKey1Alpha                             |           | MFF-- | A decimal value                                                                                                                  | "ColorKey1Alpha": 3.14                               |
| ColorKey1Time                              |           | MFF-- | A decimal value                                                                                                                  | "ColorKey1Time": 3.14                                |
| ColorKey2Alpha                             |           | MFF-- | A decimal value                                                                                                                  | "ColorKey2Alpha": 3.14                               |
| ColorKey2Time                              |           | MFF-- | A decimal value                                                                                                                  | "ColorKey2Time": 3.14                                |
| ColorKey3Alpha                             |           | MFF-- | A decimal value                                                                                                                  | "ColorKey3Alpha": 3.14                               |
| ColorKey3Time                              |           | MFF-- | A decimal value                                                                                                                  | "ColorKey3Time": 3.14                                |
| ColorScale                                 |           | MFF-- | A decimal value                                                                                                                  | "ColorScale": 3.14                                   |
| EdgeEffectAlphaFadeInTime                  |           | MFF-- | A decimal value                                                                                                                  | "EdgeEffectAlphaFadeInTime": 3.14                    |
| EdgeEffectAlphaFadeOutTime                 |           | MFF-- | A decimal value                                                                                                                  | "EdgeEffectAlphaFadeOutTime": 3.14                   |
| EdgeEffectAlphaPulseAmplitude              |           | MFF-- | A decimal value                                                                                                                  | "EdgeEffectAlphaPulseAmplitude": 3.14                |
| EdgeEffectAlphaPulseFrequency              |           | MFF-- | A decimal value                                                                                                                  | "EdgeEffectAlphaPulseFrequency": 3.14                |
| EdgeEffectFallOff                          |           | MFF-- | A decimal value                                                                                                                  | "EdgeEffectFallOff": 3.14                            |
| EdgeEffectFullAlphaRatio                   |           | MFF-- | A decimal value                                                                                                                  | "EdgeEffectFullAlphaRatio": 3.14                     |
| EdgeEffectFullAlphaTime                    |           | MFF-- | A decimal value                                                                                                                  | "EdgeEffectFullAlphaTime": 3.14                      |
| EdgeEffectPersistentAlphaRatio             |           | MFF-- | A decimal value                                                                                                                  | "EdgeEffectPersistentAlphaRatio": 3.14               |
| EdgeWidth                                  |           | MFF-- | A decimal value                                                                                                                  | "EdgeWidth": 3.14                                    |
| ExplosionWindSpeed                         |           | MFF-- | A decimal value                                                                                                                  | "ExplosionWindSpeed": 3.14                           |
| FillAlphaFadeInTime                        |           | MFF-- | A decimal value                                                                                                                  | "FillAlphaFadeInTime": 3.14                          |
| FillAlphaPulseAmplitude                    |           | MFF-- | A decimal value                                                                                                                  | "FillAlphaPulseAmplitude": 3.14                      |
| FillAlphaPulseFrequency                    |           | MFF-- | A decimal value                                                                                                                  | "FillAlphaPulseFrequency": 3.14                      |
| FillColorKey1Scale                         |           | MFF-- | A decimal value                                                                                                                  | "FillColorKey1Scale": 3.14                           |
| FillColorKey1Time                          |           | MFF-- | A decimal value                                                                                                                  | "FillColorKey1Time": 3.14                            |
| FillColorKey2Scale                         |           | MFF-- | A decimal value                                                                                                                  | "FillColorKey2Scale": 3.14                           |
| FillColorKey2Time                          |           | MFF-- | A decimal value                                                                                                                  | "FillColorKey2Time": 3.14                            |
| FillColorKey3Scale                         |           | MFF-- | A decimal value                                                                                                                  | "FillColorKey3Scale": 3.14                           |
| FillColorKey3Time                          |           | MFF-- | A decimal value                                                                                                                  | "FillColorKey3Time": 3.14                            |
| FillFadeOutTime                            |           | MFF-- | A decimal value                                                                                                                  | "FillFadeOutTime": 3.14                              |
| FillFullAlphaRatio                         |           | MFF-- | A decimal value                                                                                                                  | "FillFullAlphaRatio": 3.14                           |
| FillFullAlphaTime                          |           | MFF-- | A decimal value                                                                                                                  | "FillFullAlphaTime": 3.14                            |
| FillPersistentAlphaRatio                   |           | MFF-- | A decimal value                                                                                                                  | "FillPersistentAlphaRatio": 3.14                     |
| FillTextureAnimationSpeedU                 |           | MFF-- | A decimal value                                                                                                                  | "FillTextureAnimationSpeedU": 3.14                   |
| FillTextureAnimationSpeedV                 |           | MFF-- | A decimal value                                                                                                                  | "FillTextureAnimationSpeedV": 3.14                   |
| FillTextureScaleU                          |           | MFF-- | A decimal value                                                                                                                  | "FillTextureScaleU": 3.14                            |
| FillTextureScaleV                          |           | MFF-- | A decimal value                                                                                                                  | "FillTextureScaleV": 3.14                            |
| Flags                                      | DataFlags | MF--- | Flags (NoMembraneShader, MembraneGrayscaleColor, MembraneGrayscaleAlpha, NoParticleShader, EdgeEffectInverse, AffectSkinOnly, TextureEffectIgnoreAlpha, TextureEffectProjectUVs, IgnoreBaseGeometryAlpha, TextureEffectLighting, TextureEffectNoWeapons, ParticleAnimated, ParticleGrayscaleColor, ParticleGrayscaleAlpha, UseBloodGeometry) | "Flags": [ "NoMembraneShader", "-UseBloodGeometry" ] |
| HolesEndTime                               |           | MFF-- | A decimal value                                                                                                                  | "HolesEndTime": 3.14                                 |
| HolesEndValue                              |           | MFF-- | A decimal value                                                                                                                  | "HolesEndValue": 3.14                                |
| HolesStartTime                             |           | MFF-- | A decimal value                                                                                                                  | "HolesStartTime": 3.14                               |
| HolesStartValue                            |           | MFF-- | A decimal value                                                                                                                  | "HolesStartValue": 3.14                              |
| MembraneBlendOperation                     |           | MF--- | Possible values (Add, Subtract, ReverseSubtract, Minimum, Maximum)                                                               | "MembraneBlendOperation": "Add"                      |
| MembraneDestBlendMode                      |           | MF--- | Possible values (Zero, One, SourceColor, SourceInverseColor, SourceAlpha, SourceInvertedAlpha, DestAlpha, DestInvertedAlpha, DestColor, DestInverseColor, SourceAlphaSat) | "MembraneDestBlendMode": "Zero"                      |
| MembraneSourceBlendMode                    |           | MF--- | Possible values (Zero, One, SourceColor, SourceInverseColor, SourceAlpha, SourceInvertedAlpha, DestAlpha, DestInvertedAlpha, DestColor, DestInverseColor, SourceAlphaSat) | "MembraneSourceBlendMode": "Zero"                    |
| MembraneZTest                              |           | MF--- | Possible values (EqualTo, Normal, GreaterThan, GreaterThanOrEqualTo, AlwaysShow)                                                 | "MembraneZTest": "EqualTo"                           |
| ParticleAcceleration1                      |           | MFF-- | A decimal value                                                                                                                  | "ParticleAcceleration1": 3.14                        |
| ParticleAcceleration2                      |           | MFF-- | A decimal value                                                                                                                  | "ParticleAcceleration2": 3.14                        |
| ParticleAcceleration3                      |           | MFF-- | A decimal value                                                                                                                  | "ParticleAcceleration3": 3.14                        |
| ParticleAccelerationAlongNormal            |           | MFF-- | A decimal value                                                                                                                  | "ParticleAccelerationAlongNormal": 3.14              |
| ParticleAnimatedEndFrame                   |           | MFF-- | A numeric value                                                                                                                  | "ParticleAnimatedEndFrame": 7                        |
| ParticleAnimatedFrameCount                 |           | MFF-- | A numeric value                                                                                                                  | "ParticleAnimatedFrameCount": 7                      |
| ParticleAnimatedFrameCountVariation        |           | MFF-- | A numeric value                                                                                                                  | "ParticleAnimatedFrameCountVariation": 7             |
| ParticleAnimatedLoopStartFrame             |           | MFF-- | A numeric value                                                                                                                  | "ParticleAnimatedLoopStartFrame": 7                  |
| ParticleAnimatedLoopStartVariation         |           | MFF-- | A numeric value                                                                                                                  | "ParticleAnimatedLoopStartVariation": 7              |
| ParticleAnimatedStartFrame                 |           | MFF-- | A numeric value                                                                                                                  | "ParticleAnimatedStartFrame": 7                      |
| ParticleAnimatedStartFrameVariation        |           | MFF-- | A numeric value                                                                                                                  | "ParticleAnimatedStartFrameVariation": 7             |
| ParticleBirthRampDownTime                  |           | MFF-- | A decimal value                                                                                                                  | "ParticleBirthRampDownTime": 3.14                    |
| ParticleBirthRampUpTime                    |           | MFF-- | A decimal value                                                                                                                  | "ParticleBirthRampUpTime": 3.14                      |
| ParticleBlendOperation                     |           | MF--- | Possible values (Add, Subtract, ReverseSubtract, Minimum, Maximum)                                                               | "ParticleBlendOperation": "Add"                      |
| ParticleDestBlendMode                      |           | MF--- | Possible values (Zero, One, SourceColor, SourceInverseColor, SourceAlpha, SourceInvertedAlpha, DestAlpha, DestInvertedAlpha, DestColor, DestInverseColor, SourceAlphaSat) | "ParticleDestBlendMode": "Zero"                      |
| ParticleFullBirthRatio                     |           | MFF-- | A decimal value                                                                                                                  | "ParticleFullBirthRatio": 3.14                       |
| ParticleFullBirthTime                      |           | MFF-- | A decimal value                                                                                                                  | "ParticleFullBirthTime": 3.14                        |
| ParticleInitialRotationDegree              |           | MFF-- | A decimal value                                                                                                                  | "ParticleInitialRotationDegree": 3.14                |
| ParticleInitialRotationDegreePlusMinus     |           | MFF-- | A decimal value                                                                                                                  | "ParticleInitialRotationDegreePlusMinus": 3.14       |
| ParticleInitialSpeedAlongNormal            |           | MFF-- | A decimal value                                                                                                                  | "ParticleInitialSpeedAlongNormal": 3.14              |
| ParticleInitialSpeedAlongNormalPlusMinus   |           | MFF-- | A decimal value                                                                                                                  | "ParticleInitialSpeedAlongNormalPlusMinus": 3.14     |
| ParticleInitialVelocity1                   |           | MFF-- | A decimal value                                                                                                                  | "ParticleInitialVelocity1": 3.14                     |
| ParticleInitialVelocity2                   |           | MFF-- | A decimal value                                                                                                                  | "ParticleInitialVelocity2": 3.14                     |
| ParticleInitialVelocity3                   |           | MFF-- | A decimal value                                                                                                                  | "ParticleInitialVelocity3": 3.14                     |
| ParticleLifetime                           |           | MFF-- | A decimal value                                                                                                                  | "ParticleLifetime": 3.14                             |
| ParticleLifetimePlusMinus                  |           | MFF-- | A decimal value                                                                                                                  | "ParticleLifetimePlusMinus": 3.14                    |
| ParticlePeristentCount                     |           | MFF-- | A decimal value                                                                                                                  | "ParticlePeristentCount": 3.14                       |
| ParticleRotationSpeedDegreePerSec          |           | MFF-- | A decimal value                                                                                                                  | "ParticleRotationSpeedDegreePerSec": 3.14            |
| ParticleRotationSpeedDegreePerSecPlusMinus |           | MFF-- | A decimal value                                                                                                                  | "ParticleRotationSpeedDegreePerSecPlusMinus": 3.14   |
| ParticleScaleKey1                          |           | MFF-- | A decimal value                                                                                                                  | "ParticleScaleKey1": 3.14                            |
| ParticleScaleKey1Time                      |           | MFF-- | A decimal value                                                                                                                  | "ParticleScaleKey1Time": 3.14                        |
| ParticleScaleKey2                          |           | MFF-- | A decimal value                                                                                                                  | "ParticleScaleKey2": 3.14                            |
| ParticleScaleKey2Time                      |           | MFF-- | A decimal value                                                                                                                  | "ParticleScaleKey2Time": 3.14                        |
| ParticleSourceBlendMode                    |           | MF--- | Possible values (Zero, One, SourceColor, SourceInverseColor, SourceAlpha, SourceInvertedAlpha, DestAlpha, DestInvertedAlpha, DestColor, DestInverseColor, SourceAlphaSat) | "ParticleSourceBlendMode": "Zero"                    |
| ParticleZTest                              |           | MF--- | Possible values (EqualTo, Normal, GreaterThan, GreaterThanOrEqualTo, AlwaysShow)                                                 | "ParticleZTest": "EqualTo"                           |
| SceneGraphEmitDepthLimit                   |           | MFF-- | A numeric value                                                                                                                  | "SceneGraphEmitDepthLimit": 7                        |
| TextureCountU                              |           | MFF-- | A numeric value                                                                                                                  | "TextureCountU": 7                                   |
| TextureCountV                              |           | MFF-- | A numeric value                                                                                                                  | "TextureCountV": 7                                   |

[⬅ Back to Types](Types.md)

## ECZN - EncounterZone

| Field    | Alt       | MFFSM | Value Type                                                           | Example                                              |
| -------- | --------- | ----- | -------------------------------------------------------------------- | ---------------------------------------------------- |
| Flags    | DataFlags | MF--- | Flags (NeverResets, MatchPcBelowMinimumLevel, DisableCombatBoundary) | "Flags": [ "NeverResets", "-DisableCombatBoundary" ] |
| Location | XLCN      | MFF-- | Form Key or Editor ID                                                |                                                      |
| MaxLevel |           | MFF-- | A numeric value                                                      | "MaxLevel": 7                                        |
| MinLevel |           | MFF-- | A numeric value                                                      | "MinLevel": 7                                        |
| Owner    |           | MFF-- | Form Key or Editor ID                                                |                                                      |
| Rank     |           | MFF-- | A numeric value                                                      | "Rank": 7                                            |

[⬅ Back to Types](Types.md)

## EQUP - EquipType

| Field       | Alt | MFFSM | Value Type              | Example |
| ----------- | --- | ----- | ----------------------- | ------- |
| SlotParents |     | MFFSM | Form Keys or Editor IDs |         |

[⬅ Back to Types](Types.md)

## EXPL - Explosion

| Field              | Alt       | MFFSM | Value Type                                                                                                                                | Example                                                             |
| ------------------ | --------- | ----- | ----------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------- |
| Damage             |           | MFF-- | A decimal value                                                                                                                           | "Damage": 3.14                                                      |
| Flags              | DataFlags | MF--- | Flags (AlwaysUsesWorldOrientation, KnockDownAlways, KnockDownByFormula, IgnoreLosCheck, PushExplosionSourceRefOnly, IgnoreImageSpaceSwap, Chain, NoControllerVibration) | "Flags": [ "AlwaysUsesWorldOrientation", "-NoControllerVibration" ] |
| Force              |           | MFF-- | A decimal value                                                                                                                           | "Force": 3.14                                                       |
| ImageSpaceModifier |           | MFF-- | Form Key or Editor ID                                                                                                                     |                                                                     |
| ImpactDataSet      |           | MFF-- | Form Key or Editor ID                                                                                                                     |                                                                     |
| ISRadius           |           | MFF-- | A decimal value                                                                                                                           | "ISRadius": 3.14                                                    |
| Light              |           | MFF-- | Form Key or Editor ID                                                                                                                     |                                                                     |
| Name               | FULL      | MFF-- | A string value                                                                                                                            | "Name": "Hello"                                                     |
| ObjectEffect       | EITM      | MFF-- | Form Key or Editor ID                                                                                                                     |                                                                     |
| PlacedObject       |           | MFF-- | Form Key or Editor ID                                                                                                                     |                                                                     |
| Radius             |           | MFF-- | A decimal value                                                                                                                           | "Radius": 3.14                                                      |
| Sound1             |           | MFF-- | Form Key or Editor ID                                                                                                                     |                                                                     |
| Sound2             |           | MFF-- | Form Key or Editor ID                                                                                                                     |                                                                     |
| SoundLevel         |           | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)                                                                                          | "SoundLevel": "Loud"                                                |
| SpawnProjectile    |           | MFF-- | Form Key or Editor ID                                                                                                                     |                                                                     |
| VerticalOffsetMult |           | MFF-- | A decimal value                                                                                                                           | "VerticalOffsetMult": 3.14                                          |

[⬅ Back to Types](Types.md)

## EYES - Eyes

| Field      | Alt         | MFFSM | Value Type                           | Example                               |
| ---------- | ----------- | ----- | ------------------------------------ | ------------------------------------- |
| Flags      | DataFlags   | MF--- | Flags (Playable, NotMale, NotFemale) | "Flags": [ "Playable", "-NotFemale" ] |
| MajorFlags | RecordFlags | MF--- | Flags (NonPlayable)                  | "MajorFlags": "NonPlayable"           |
| Name       | FULL        | MFF-- | A string value                       | "Name": "Hello"                       |

[⬅ Back to Types](Types.md)

## FACT - Faction

| Field                    | Alt       | MFFSM | Value Type                                                                                                                                               | Example                                        |
| ------------------------ | --------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------- |
| ExteriorJailMarker       | JAIL      | MFF-- | Form Key or Editor ID                                                                                                                                    |                                                |
| Flags                    | DataFlags | MF--- | Flags (HiddenFromPC, SpecialCombat, TrackCrime, IgnoreMurder, IgnoreAssault, IgnoreStealing, IgnoreTrespass, DoNotReportCrimesAgainstMembers, CrimeGoldUseDefaults, IgnorePickpocket, Vendor, CanBeOwner, IgnoreWerewolf) | "Flags": [ "HiddenFromPC", "-IgnoreWerewolf" ] |
| FollowerWaitMarker       | WAIT      | MFF-- | Form Key or Editor ID                                                                                                                                    |                                                |
| JailOutfit               | JOUT      | MFF-- | Form Key or Editor ID                                                                                                                                    |                                                |
| MerchantContainer        | VENC      | MFF-- | Form Key or Editor ID                                                                                                                                    |                                                |
| Name                     | FULL      | MFF-- | A string value                                                                                                                                           | "Name": "Hello"                                |
| PlayerInventoryContainer | PLCN      | MFF-- | Form Key or Editor ID                                                                                                                                    |                                                |
| SharedCrimeFactionList   | CRGR      | MFF-- | Form Key or Editor ID                                                                                                                                    |                                                |
| StolenGoodsContainer     | STOL      | MFF-- | Form Key or Editor ID                                                                                                                                    |                                                |
| VendorBuySellList        | VEND      | MFF-- | Form Key or Editor ID                                                                                                                                    |                                                |

[⬅ Back to Types](Types.md)

## FLOR - Flora

| Field                | Alt  | MFFSM | Value Type              | Example                         |
| -------------------- | ---- | ----- | ----------------------- | ------------------------------- |
| ActivateTextOverride |      | MFF-- | A string value          | "ActivateTextOverride": "Hello" |
| HarvestSound         |      | MFF-- | Form Key or Editor ID   |                                 |
| Ingredient           |      | MFF-- | Form Key or Editor ID   |                                 |
| Keywords             | KWDA | MFFSM | Form Keys or Editor IDs |                                 |
| Name                 | FULL | MFF-- | A string value          | "Name": "Hello"                 |

[⬅ Back to Types](Types.md)

## FSTP - Footstep

| Field         | Alt | MFFSM | Value Type            | Example        |
| ------------- | --- | ----- | --------------------- | -------------- |
| ImpactDataSet |     | MFF-- | Form Key or Editor ID |                |
| Tag           |     | MFF-- | A string value        | "Tag": "Hello" |

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

| Field | Alt  | MFFSM | Value Type              | Example |
| ----- | ---- | ----- | ----------------------- | ------- |
| Items | Item | MFFSM | Form Keys or Editor IDs |         |

[⬅ Back to Types](Types.md)

## FURN - Furniture

| Field              | Alt         | MFFSM | Value Type                                                                             | Example                                            |
| ------------------ | ----------- | ----- | -------------------------------------------------------------------------------------- | -------------------------------------------------- |
| AssociatedSpell    |             | MFF-- | Form Key or Editor ID                                                                  |                                                    |
| Flags              | DataFlags   | MF--- | Flags (IgnoredBySandbox, DisablesActivation, IsPerch, MustExitToTalk)                  | "Flags": [ "IgnoredBySandbox", "-MustExitToTalk" ] |
| InteractionKeyword |             | MFF-- | Form Key or Editor ID                                                                  |                                                    |
| Keywords           | KWDA        | MFFSM | Form Keys or Editor IDs                                                                |                                                    |
| MajorFlags         | RecordFlags | MF--- | Flags (IsPerch, HasDistantLOD, RandomAnimStart, IsMarker, MustExitToTalk, ChildCanUse) | "MajorFlags": [ "IsPerch", "-ChildCanUse" ]        |
| Name               | FULL        | MFF-- | A string value                                                                         | "Name": "Hello"                                    |

[⬅ Back to Types](Types.md)

## GLOB - Global

| Field      | Alt         | MFFSM | Value Type       | Example                  |
| ---------- | ----------- | ----- | ---------------- | ------------------------ |
| MajorFlags | RecordFlags | MF--- | Flags (Constant) | "MajorFlags": "Constant" |
| TypeChar   |             | MFF-- | A numeric value  | "TypeChar": 7            |

[⬅ Back to Types](Types.md)

## GRAS - Grass

| Field              | Alt       | MFFSM | Value Type                                                                                                                                | Example                                      |
| ------------------ | --------- | ----- | ----------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| ColorRange         |           | MFF-- | A decimal value                                                                                                                           | "ColorRange": 3.14                           |
| Density            |           | MFF-- | A numeric value                                                                                                                           | "Density": 7                                 |
| Flags              | DataFlags | MF--- | Flags (VertexLighting, UniformScaling, FitToSlope)                                                                                        | "Flags": [ "VertexLighting", "-FitToSlope" ] |
| HeightRange        |           | MFF-- | A decimal value                                                                                                                           | "HeightRange": 3.14                          |
| MaxSlope           |           | MFF-- | A numeric value                                                                                                                           | "MaxSlope": 7                                |
| MinSlope           |           | MFF-- | A numeric value                                                                                                                           | "MinSlope": 7                                |
| PositionRange      |           | MFF-- | A decimal value                                                                                                                           | "PositionRange": 3.14                        |
| UnitsFromWater     |           | MFF-- | A numeric value                                                                                                                           | "UnitsFromWater": 7                          |
| UnitsFromWaterType |           | MF--- | Possible values (AboveAtLeast, AboveAtMost, BelowAtLeast, BelowAtMost, EitherAtLeast, EitherAtMost, EitherAtMostAbove, EitherAtMostBelow) | "UnitsFromWaterType": "AboveAtLeast"         |
| WavePeriod         |           | MFF-- | A decimal value                                                                                                                           | "WavePeriod": 3.14                           |

[⬅ Back to Types](Types.md)

## HAZD - Hazard

| Field              | Alt       | MFFSM | Value Type                                                                                                               | Example                                           |
| ------------------ | --------- | ----- | ------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------- |
| Flags              | DataFlags | MF--- | Flags (AffectsPlayerOnly, InheritDurationFromSpawnSpell, AlignToImpactNormal, InheritRadiusFromSpawnSpell, DropToGround) | "Flags": [ "AffectsPlayerOnly", "-DropToGround" ] |
| ImageSpaceModifier |           | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| ImageSpaceRadius   |           | MFF-- | A decimal value                                                                                                          | "ImageSpaceRadius": 3.14                          |
| ImpactDataSet      |           | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| Lifetime           |           | MFF-- | A decimal value                                                                                                          | "Lifetime": 3.14                                  |
| Light              |           | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| Limit              |           | MFF-- | A numeric value                                                                                                          | "Limit": 7                                        |
| Name               | FULL      | MFF-- | A string value                                                                                                           | "Name": "Hello"                                   |
| Radius             |           | MFF-- | A decimal value                                                                                                          | "Radius": 3.14                                    |
| Sound              |           | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| Spell              |           | MFF-- | Form Key or Editor ID                                                                                                    |                                                   |
| TargetInterval     |           | MFF-- | A decimal value                                                                                                          | "TargetInterval": 3.14                            |

[⬅ Back to Types](Types.md)

## HDPT - HeadPart

| Field      | Alt         | MFFSM | Value Type                                                            | Example                                  |
| ---------- | ----------- | ----- | --------------------------------------------------------------------- | ---------------------------------------- |
| Color      |             | MFF-- | Form Key or Editor ID                                                 |                                          |
| ExtraParts |             | MFFSM | Form Keys or Editor IDs                                               |                                          |
| Flags      | DataFlags   | MF--- | Flags (Playable, Male, Female, IsExtraPart, UseSolidTint)             | "Flags": [ "Playable", "-UseSolidTint" ] |
| MajorFlags | RecordFlags | MF--- | Flags (NonPlayable)                                                   | "MajorFlags": "NonPlayable"              |
| Name       | FULL        | MFF-- | A string value                                                        | "Name": "Hello"                          |
| TextureSet |             | MFF-- | Form Key or Editor ID                                                 |                                          |
| Type       |             | MF--- | Possible values (Misc, Face, Eyes, Hair, FacialHair, Scars, Eyebrows) | "Type": "Misc"                           |
| ValidRaces |             | MFF-- | Form Key or Editor ID                                                 |                                          |

[⬅ Back to Types](Types.md)

## IDLE - IdleAnimation

| Field                 | Alt       | MFFSM | Value Type                                      | Example                            |
| --------------------- | --------- | ----- | ----------------------------------------------- | ---------------------------------- |
| AnimationEvent        |           | MFF-- | A string value                                  | "AnimationEvent": "Hello"          |
| AnimationGroupSection |           | MFF-- | A numeric value                                 | "AnimationGroupSection": 7         |
| Flags                 | DataFlags | MF--- | Flags (Parent, Sequence, NoAttacking, Blocking) | "Flags": [ "Parent", "-Blocking" ] |
| LoopingSecondsMax     |           | MFF-- | A numeric value                                 | "LoopingSecondsMax": 7             |
| LoopingSecondsMin     |           | MFF-- | A numeric value                                 | "LoopingSecondsMin": 7             |
| RelatedIdles          |           | MFFSM | Form Keys or Editor IDs                         |                                    |
| ReplayDelay           |           | MFF-- | A numeric value                                 | "ReplayDelay": 7                   |

[⬅ Back to Types](Types.md)

## IDLM - IdleMarker

| Field      | Alt         | MFFSM | Value Type                                      | Example                                           |
| ---------- | ----------- | ----- | ----------------------------------------------- | ------------------------------------------------- |
| Animations |             | MFFSM | Form Keys or Editor IDs                         |                                                   |
| Flags      | DataFlags   | MF--- | Flags (RunInSequence, DoOnce, IgnoredBySandbox) | "Flags": [ "RunInSequence", "-IgnoredBySandbox" ] |
| IdleTimer  |             | MFF-- | A decimal value                                 | "IdleTimer": 3.14                                 |
| MajorFlags | RecordFlags | MF--- | Flags (ChildCanUse)                             | "MajorFlags": "ChildCanUse"                       |

[⬅ Back to Types](Types.md)

## IMAD - ImageSpaceAdapter

| Field             | Alt | MFFSM | Value Type                                                                                    | Example                                                 |
| ----------------- | --- | ----- | --------------------------------------------------------------------------------------------- | ------------------------------------------------------- |
| DepthOfFieldFlags |     | MF--- | Flags (UseTarget, ModeFront, ModeBack, NoSky, BlurRadiusBit2, BlurRadiusBit1, BlurRadiusBit0) | "DepthOfFieldFlags": [ "UseTarget", "-BlurRadiusBit0" ] |
| Duration          |     | MFF-- | A decimal value                                                                               | "Duration": 3.14                                        |

[⬅ Back to Types](Types.md)

## IPCT - Impact

| Field               | Alt | MFFSM | Value Type                                                              | Example                        |
| ------------------- | --- | ----- | ----------------------------------------------------------------------- | ------------------------------ |
| AngleThreshold      |     | MFF-- | A decimal value                                                         | "AngleThreshold": 3.14         |
| Duration            |     | MFF-- | A decimal value                                                         | "Duration": 3.14               |
| Hazard              |     | MFF-- | Form Key or Editor ID                                                   |                                |
| Orientation         |     | MF--- | Possible values (SurfaceNormal, ProjectileVector, ProjectileReflection) | "Orientation": "SurfaceNormal" |
| PlacementRadius     |     | MFF-- | A decimal value                                                         | "PlacementRadius": 3.14        |
| Result              |     | MF--- | Possible values (Default, Destroy, Bounce, Impale, Stick)               | "Result": "Default"            |
| SecondaryTextureSet |     | MFF-- | Form Key or Editor ID                                                   |                                |
| Sound1              |     | MFF-- | Form Key or Editor ID                                                   |                                |
| Sound2              |     | MFF-- | Form Key or Editor ID                                                   |                                |
| SoundLevel          |     | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)                        | "SoundLevel": "Loud"           |
| TextureSet          |     | MFF-- | Form Key or Editor ID                                                   |                                |

[⬅ Back to Types](Types.md)

## ALCH - Ingestible

| Field           | Alt         | MFFSM | Value Type                                                        | Example                                                                                |
| --------------- | ----------- | ----- | ----------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| Addiction       |             | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| AddictionChance |             | MFF-- | A decimal value                                                   | "AddictionChance": 3.14                                                                |
| ConsumeSound    |             | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Description     | DESC        | MFF-- | A string value                                                    | "Description": "Hello"                                                                 |
| Effects         |             | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipmentType   | ETYP        | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Flags           | DataFlags   | MF--- | Flags (NoAutoCalc, FoodItem, Medicine, Poison)                    | "Flags": [ "NoAutoCalc", "-Poison" ]                                                   |
| Keywords        | KWDA        | MFFSM | Form Keys or Editor IDs                                           |                                                                                        |
| MajorFlags      | RecordFlags | MF--- | Flags (Medicine)                                                  | "MajorFlags": "Medicine"                                                               |
| Name            | FULL        | MFF-- | A string value                                                    | "Name": "Hello"                                                                        |
| PickUpSound     | YNAM        | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| PutDownSound    | ZNAM        | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Value           |             | MFF-- | A numeric value                                                   | "Value": 7                                                                             |
| Weight          |             | MFF-- | A decimal value                                                   | "Weight": 3.14                                                                         |

[⬅ Back to Types](Types.md)

## INGR - Ingredient

| Field           | Alt       | MFFSM | Value Type                                                        | Example                                                                                |
| --------------- | --------- | ----- | ----------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| Effects         |           | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipType       | ETYP      | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Flags           | DataFlags | MF--- | Flags (NoAutoCalculation, FoodItem, ReferencesPersist)            | "Flags": [ "NoAutoCalculation", "-ReferencesPersist" ]                                 |
| IngredientValue |           | MFF-- | A numeric value                                                   | "IngredientValue": 7                                                                   |
| Keywords        | KWDA      | MFFSM | Form Keys or Editor IDs                                           |                                                                                        |
| Name            | FULL      | MFF-- | A string value                                                    | "Name": "Hello"                                                                        |
| PickUpSound     | YNAM      | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| PutDownSound    | ZNAM      | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| Value           |           | MFF-- | A numeric value                                                   | "Value": 7                                                                             |
| Weight          |           | MFF-- | A decimal value                                                   | "Weight": 3.14                                                                         |

[⬅ Back to Types](Types.md)

## KEYM - Key

| Field        | Alt         | MFFSM | Value Type              | Example                     |
| ------------ | ----------- | ----- | ----------------------- | --------------------------- |
| Keywords     | KWDA        | MFFSM | Form Keys or Editor IDs |                             |
| MajorFlags   | RecordFlags | MF--- | Flags (NonPlayable)     | "MajorFlags": "NonPlayable" |
| Name         | FULL        | MFF-- | A string value          | "Name": "Hello"             |
| PickUpSound  | YNAM        | MFF-- | Form Key or Editor ID   |                             |
| PutDownSound | ZNAM        | MFF-- | Form Key or Editor ID   |                             |
| Value        |             | MFF-- | A numeric value         | "Value": 7                  |
| Weight       |             | MFF-- | A decimal value         | "Weight": 3.14              |

[⬅ Back to Types](Types.md)

## LTEX - LandscapeTexture

| Field                   | Alt       | MFFSM | Value Type              | Example                      |
| ----------------------- | --------- | ----- | ----------------------- | ---------------------------- |
| Flags                   | DataFlags | MF--- | Flags (IsSnow)          | "Flags": "IsSnow"            |
| Grasses                 |           | MFFSM | Form Keys or Editor IDs |                              |
| HavokFriction           |           | MFF-- | A numeric value         | "HavokFriction": 7           |
| HavokRestitution        |           | MFF-- | A numeric value         | "HavokRestitution": 7        |
| MaterialType            |           | MFF-- | Form Key or Editor ID   |                              |
| TextureSet              |           | MFF-- | Form Key or Editor ID   |                              |
| TextureSpecularExponent |           | MFF-- | A numeric value         | "TextureSpecularExponent": 7 |

[⬅ Back to Types](Types.md)

## LVLI - LeveledItem

| Field  | Alt       | MFFSM | Value Type                                                                                            | Example                                                                    |
| ------ | --------- | ----- | ----------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| Flags  | DataFlags | MF--- | Flags (CalculateFromAllLevelsLessThanOrEqualPlayer, CalculateForEachItemInCount, UseAll, SpecialLoot) | "Flags": [ "CalculateFromAllLevelsLessThanOrEqualPlayer", "-SpecialLoot" ] |
| Global |           | MFF-- | Form Key or Editor ID                                                                                 |                                                                            |

[⬅ Back to Types](Types.md)

## LVLN - LeveledNpc

| Field  | Alt       | MFFSM | Value Type                                                                       | Example                                                                                    |
| ------ | --------- | ----- | -------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| Flags  | DataFlags | MF--- | Flags (CalculateFromAllLevelsLessThanOrEqualPlayer, CalculateForEachItemInCount) | "Flags": [ "CalculateFromAllLevelsLessThanOrEqualPlayer", "-CalculateForEachItemInCount" ] |
| Global |           | MFF-- | Form Key or Editor ID                                                            |                                                                                            |

[⬅ Back to Types](Types.md)

## LVSP - LeveledSpell

| Field | Alt       | MFFSM | Value Type                                                                                     | Example                                                                     |
| ----- | --------- | ----- | ---------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------- |
| Flags | DataFlags | MF--- | Flags (CalculateFromAllLevelsLessThanOrEqualPlayer, CalculateForEachItemInCount, UseAllSpells) | "Flags": [ "CalculateFromAllLevelsLessThanOrEqualPlayer", "-UseAllSpells" ] |

[⬅ Back to Types](Types.md)

## LIGH - Light

| Field                     | Alt         | MFFSM | Value Type                                                                                                                                          | Example                                          |
| ------------------------- | ----------- | ----- | --------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| FadeValue                 |             | MFF-- | A decimal value                                                                                                                                     | "FadeValue": 3.14                                |
| FalloffExponent           |             | MFF-- | A decimal value                                                                                                                                     | "FalloffExponent": 3.14                          |
| Flags                     | DataFlags   | MF--- | Flags (Dynamic, CanBeCarried, Negative, Flicker, OffByDefault, FlickerSlow, Pulse, PulseSlow, SpotLight, ShadowSpotlight, ShadowHemisphere, ShadowOmnidirectional, PortalStrict) | "Flags": [ "Dynamic", "-PortalStrict" ]          |
| FlickerIntensityAmplitude |             | MFF-- | A decimal value                                                                                                                                     | "FlickerIntensityAmplitude": 3.14                |
| FlickerMovementAmplitude  |             | MFF-- | A decimal value                                                                                                                                     | "FlickerMovementAmplitude": 3.14                 |
| FlickerPeriod             |             | MFF-- | A decimal value                                                                                                                                     | "FlickerPeriod": 3.14                            |
| FOV                       |             | MFF-- | A decimal value                                                                                                                                     | "FOV": 3.14                                      |
| Lens                      |             | MFF-- | Form Key or Editor ID                                                                                                                               |                                                  |
| MajorFlags                | RecordFlags | MF--- | Flags (RandomAnimStart, PortalStrict, Obstacle)                                                                                                     | "MajorFlags": [ "RandomAnimStart", "-Obstacle" ] |
| Name                      | FULL        | MFF-- | A string value                                                                                                                                      | "Name": "Hello"                                  |
| NearClip                  |             | MFF-- | A decimal value                                                                                                                                     | "NearClip": 3.14                                 |
| Radius                    |             | MFF-- | A numeric value                                                                                                                                     | "Radius": 7                                      |
| Sound                     |             | MFF-- | Form Key or Editor ID                                                                                                                               |                                                  |
| Time                      |             | MFF-- | A numeric value                                                                                                                                     | "Time": 7                                        |
| Value                     |             | MFF-- | A numeric value                                                                                                                                     | "Value": 7                                       |
| Weight                    |             | MFF-- | A decimal value                                                                                                                                     | "Weight": 3.14                                   |

[⬅ Back to Types](Types.md)

## LGTM - LightingTemplate

| Field                  | Alt | MFFSM | Value Type      | Example                        |
| ---------------------- | --- | ----- | --------------- | ------------------------------ |
| DirectionalFade        |     | MFF-- | A decimal value | "DirectionalFade": 3.14        |
| DirectionalRotationXY  |     | MFF-- | A numeric value | "DirectionalRotationXY": 7     |
| DirectionalRotationZ   |     | MFF-- | A numeric value | "DirectionalRotationZ": 7      |
| FogClipDistance        |     | MFF-- | A decimal value | "FogClipDistance": 3.14        |
| FogFar                 |     | MFF-- | A decimal value | "FogFar": 3.14                 |
| FogMax                 |     | MFF-- | A decimal value | "FogMax": 3.14                 |
| FogNear                |     | MFF-- | A decimal value | "FogNear": 3.14                |
| FogPower               |     | MFF-- | A decimal value | "FogPower": 3.14               |
| LightFadeEndDistance   |     | MFF-- | A decimal value | "LightFadeEndDistance": 3.14   |
| LightFadeStartDistance |     | MFF-- | A decimal value | "LightFadeStartDistance": 3.14 |

[⬅ Back to Types](Types.md)

## LSCR - LoadScreen

| Field            | Alt         | MFFSM | Value Type                 | Example                            |
| ---------------- | ----------- | ----- | -------------------------- | ---------------------------------- |
| Description      | DESC        | MFF-- | A string value             | "Description": "Hello"             |
| InitialScale     |             | MFF-- | A decimal value            | "InitialScale": 3.14               |
| LoadingScreenNif |             | MFF-- | Form Key or Editor ID      |                                    |
| MajorFlags       | RecordFlags | MF--- | Flags (DisplaysInMainMenu) | "MajorFlags": "DisplaysInMainMenu" |

[⬅ Back to Types](Types.md)

## LCTN - Location

| Field                             | Alt  | MFFSM | Value Type              | Example                     |
| --------------------------------- | ---- | ----- | ----------------------- | --------------------------- |
| ActorCellMarkerReference          |      | MFFSM | Form Keys or Editor IDs |                             |
| HorseMarkerRef                    |      | MFF-- | Form Key or Editor ID   |                             |
| Keywords                          | KWDA | MFFSM | Form Keys or Editor IDs |                             |
| LocationCellMarkerReference       |      | MFFSM | Form Keys or Editor IDs |                             |
| Music                             |      | MFF-- | Form Key or Editor ID   |                             |
| Name                              | FULL | MFF-- | A string value          | "Name": "Hello"             |
| ParentLocation                    |      | MFF-- | Form Key or Editor ID   |                             |
| ReferenceCellPersistentReferences |      | MFFSM | Form Keys or Editor IDs |                             |
| ReferenceCellStaticReferences     |      | MFFSM | Form Keys or Editor IDs |                             |
| ReferenceCellUnique               |      | MFFSM | Form Keys or Editor IDs |                             |
| UnreportedCrimeFaction            |      | MFF-- | Form Key or Editor ID   |                             |
| WorldLocationMarkerRef            |      | MFF-- | Form Key or Editor ID   |                             |
| WorldLocationRadius               |      | MFF-- | A decimal value         | "WorldLocationRadius": 3.14 |

[⬅ Back to Types](Types.md)

## MGEF - MagicEffect

| Field                   | Alt       | MFFSM | Value Type                                                                                                                                                      | Example                                  |
| ----------------------- | --------- | ----- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------- |
| BaseCost                |           | MFF-- | A decimal value                                                                                                                                                 | "BaseCost": 3.14                         |
| CastingArt              |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| CastingLight            |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| CastingSoundLevel       |           | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)                                                                                                                | "CastingSoundLevel": "Loud"              |
| CastType                |           | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration)                                                                                                  | "CastType": "ConstantEffect"             |
| CounterEffects          |           | MFFSM | Form Keys or Editor IDs                                                                                                                                         |                                          |
| Description             | DESC      | MFF-- | A string value                                                                                                                                                  | "Description": "Hello"                   |
| DualCastArt             |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| DualCastScale           |           | MFF-- | A decimal value                                                                                                                                                 | "DualCastScale": 3.14                    |
| EnchantArt              |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| EnchantShader           |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| EnchantVisuals          |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| EquipAbility            |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| Explosion               |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| Flags                   | DataFlags | MF--- | Flags (Hostile, Recover, Detrimental, SnapToNavmesh, NoHitEvent, DispelWithKeywords, NoDuration, NoMagnitude, NoArea, FXPersist, GoryVisuals, HideInUI, NoRecast, PowerAffectsMagnitude, PowerAffectsDuration, Painless, NoHitEffect, NoDeathDispel) | "Flags": [ "Hostile", "-NoDeathDispel" ] |
| HitEffectArt            |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| HitShader               |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| HitVisuals              |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| ImageSpaceModifier      |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| ImpactData              |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| Keywords                | KWDA      | MFFSM | Form Keys or Editor IDs                                                                                                                                         |                                          |
| MagicSkill              |           | MF--- | Possible values (Aggression, Confidence, Energy, Morality, Mood, Assistance, OneHanded, TwoHanded, Archery, Block, Smithing, HeavyArmor, LightArmor, Pickpocket, Lockpicking, Sneak, Alchemy, Speech, Alteration, Conjuration, Destruction, Illusion, Restoration, Enchanting, Health, Magicka, Stamina, HealRate, MagickaRate, StaminaRate, SpeedMult, InventoryWeight, CarryWeight, CriticalChance, MeleeDamage, UnarmedDamage, Mass, VoicePoints, VoiceRate, DamageResist, PoisonResist, ResistFire, ResistShock, ResistFrost, ResistMagic, ResistDisease, Paralysis, Invisibility, NightEye, DetectLifeRange, WaterBreathing, WaterWalking, Fame, Infamy, JumpingBonus, WardPower, RightItemCharge, ArmorPerks, ShieldPerks, WardDeflection, Variable01, Variable02, Variable03, Variable04, Variable05, Variable06, Variable07, Variable08, Variable09, Variable10, BowSpeedBonus, FavorActive, FavorsPerDay, FavorsPerDayTimer, LeftItemCharge, AbsorbChance, Blindness, WeaponSpeedMult, ShoutRecoveryMult, BowStaggerBonus, Telekinesis, FavorPointsBonus, LastBribedIntimidated, LastFlattered, MovementNoiseMult, BypassVendorStolenCheck, BypassVendorKeywordCheck, WaitingForPlayer, OneHandedModifier, TwoHandedModifier, MarksmanModifier, BlockModifier, SmithingModifier, HeavyArmorModifier, LightArmorModifier, PickpocketModifier, LockpickingModifier, SneakingModifier, AlchemyModifier, SpeechcraftModifier, AlterationModifier, ConjurationModifier, DestructionModifier, IllusionModifier, RestorationModifier, EnchantingModifier, OneHandedSkillAdvance, TwoHandedSkillAdvance, MarksmanSkillAdvance, BlockSkillAdvance, SmithingSkillAdvance, HeavyArmorSkillAdvance, LightArmorSkillAdvance, PickpocketSkillAdvance, LockpickingSkillAdvance, SneakingSkillAdvance, AlchemySkillAdvance, SpeechcraftSkillAdvance, AlterationSkillAdvance, ConjurationSkillAdvance, DestructionSkillAdvance, IllusionSkillAdvance, RestorationSkillAdvance, EnchantingSkillAdvance, LeftWeaponSpeedMultiply, DragonSouls, CombatHealthRegenMultiply, OneHandedPowerModifier, TwoHandedPowerModifier, MarksmanPowerModifier, BlockPowerModifier, SmithingPowerModifier, HeavyArmorPowerModifier, LightArmorPowerModifier, PickpocketPowerModifier, LockpickingPowerModifier, SneakingPowerModifier, AlchemyPowerModifier, SpeechcraftPowerModifier, AlterationPowerModifier, ConjurationPowerModifier, DestructionPowerModifier, IllusionPowerModifier, RestorationPowerModifier, EnchantingPowerModifier, DragonRend, AttackDamageMult, HealRateMult, MagickaRateMult, StaminaRateMult, WerewolfPerks, VampirePerks, GrabActorOffset, Grabbed, ReflectDamage, None) | "MagicSkill": "Aggression"               |
| MenuDisplayObject       |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| MinimumSkillLevel       |           | MFF-- | A numeric value                                                                                                                                                 | "MinimumSkillLevel": 7                   |
| Name                    | FULL      | MFF-- | A string value                                                                                                                                                  | "Name": "Hello"                          |
| PerkToApply             |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| Projectile              |           | MFF-- | Form Key or Editor ID                                                                                                                                           |                                          |
| ResistValue             |           | MF--- | Possible values (Aggression, Confidence, Energy, Morality, Mood, Assistance, OneHanded, TwoHanded, Archery, Block, Smithing, HeavyArmor, LightArmor, Pickpocket, Lockpicking, Sneak, Alchemy, Speech, Alteration, Conjuration, Destruction, Illusion, Restoration, Enchanting, Health, Magicka, Stamina, HealRate, MagickaRate, StaminaRate, SpeedMult, InventoryWeight, CarryWeight, CriticalChance, MeleeDamage, UnarmedDamage, Mass, VoicePoints, VoiceRate, DamageResist, PoisonResist, ResistFire, ResistShock, ResistFrost, ResistMagic, ResistDisease, Paralysis, Invisibility, NightEye, DetectLifeRange, WaterBreathing, WaterWalking, Fame, Infamy, JumpingBonus, WardPower, RightItemCharge, ArmorPerks, ShieldPerks, WardDeflection, Variable01, Variable02, Variable03, Variable04, Variable05, Variable06, Variable07, Variable08, Variable09, Variable10, BowSpeedBonus, FavorActive, FavorsPerDay, FavorsPerDayTimer, LeftItemCharge, AbsorbChance, Blindness, WeaponSpeedMult, ShoutRecoveryMult, BowStaggerBonus, Telekinesis, FavorPointsBonus, LastBribedIntimidated, LastFlattered, MovementNoiseMult, BypassVendorStolenCheck, BypassVendorKeywordCheck, WaitingForPlayer, OneHandedModifier, TwoHandedModifier, MarksmanModifier, BlockModifier, SmithingModifier, HeavyArmorModifier, LightArmorModifier, PickpocketModifier, LockpickingModifier, SneakingModifier, AlchemyModifier, SpeechcraftModifier, AlterationModifier, ConjurationModifier, DestructionModifier, IllusionModifier, RestorationModifier, EnchantingModifier, OneHandedSkillAdvance, TwoHandedSkillAdvance, MarksmanSkillAdvance, BlockSkillAdvance, SmithingSkillAdvance, HeavyArmorSkillAdvance, LightArmorSkillAdvance, PickpocketSkillAdvance, LockpickingSkillAdvance, SneakingSkillAdvance, AlchemySkillAdvance, SpeechcraftSkillAdvance, AlterationSkillAdvance, ConjurationSkillAdvance, DestructionSkillAdvance, IllusionSkillAdvance, RestorationSkillAdvance, EnchantingSkillAdvance, LeftWeaponSpeedMultiply, DragonSouls, CombatHealthRegenMultiply, OneHandedPowerModifier, TwoHandedPowerModifier, MarksmanPowerModifier, BlockPowerModifier, SmithingPowerModifier, HeavyArmorPowerModifier, LightArmorPowerModifier, PickpocketPowerModifier, LockpickingPowerModifier, SneakingPowerModifier, AlchemyPowerModifier, SpeechcraftPowerModifier, AlterationPowerModifier, ConjurationPowerModifier, DestructionPowerModifier, IllusionPowerModifier, RestorationPowerModifier, EnchantingPowerModifier, DragonRend, AttackDamageMult, HealRateMult, MagickaRateMult, StaminaRateMult, WerewolfPerks, VampirePerks, GrabActorOffset, Grabbed, ReflectDamage, None) | "ResistValue": "Aggression"              |
| ScriptEffectAIDelayTime |           | MFF-- | A decimal value                                                                                                                                                 | "ScriptEffectAIDelayTime": 3.14          |
| ScriptEffectAIScore     |           | MFF-- | A decimal value                                                                                                                                                 | "ScriptEffectAIScore": 3.14              |
| SecondActorValue        |           | MF--- | Possible values (Aggression, Confidence, Energy, Morality, Mood, Assistance, OneHanded, TwoHanded, Archery, Block, Smithing, HeavyArmor, LightArmor, Pickpocket, Lockpicking, Sneak, Alchemy, Speech, Alteration, Conjuration, Destruction, Illusion, Restoration, Enchanting, Health, Magicka, Stamina, HealRate, MagickaRate, StaminaRate, SpeedMult, InventoryWeight, CarryWeight, CriticalChance, MeleeDamage, UnarmedDamage, Mass, VoicePoints, VoiceRate, DamageResist, PoisonResist, ResistFire, ResistShock, ResistFrost, ResistMagic, ResistDisease, Paralysis, Invisibility, NightEye, DetectLifeRange, WaterBreathing, WaterWalking, Fame, Infamy, JumpingBonus, WardPower, RightItemCharge, ArmorPerks, ShieldPerks, WardDeflection, Variable01, Variable02, Variable03, Variable04, Variable05, Variable06, Variable07, Variable08, Variable09, Variable10, BowSpeedBonus, FavorActive, FavorsPerDay, FavorsPerDayTimer, LeftItemCharge, AbsorbChance, Blindness, WeaponSpeedMult, ShoutRecoveryMult, BowStaggerBonus, Telekinesis, FavorPointsBonus, LastBribedIntimidated, LastFlattered, MovementNoiseMult, BypassVendorStolenCheck, BypassVendorKeywordCheck, WaitingForPlayer, OneHandedModifier, TwoHandedModifier, MarksmanModifier, BlockModifier, SmithingModifier, HeavyArmorModifier, LightArmorModifier, PickpocketModifier, LockpickingModifier, SneakingModifier, AlchemyModifier, SpeechcraftModifier, AlterationModifier, ConjurationModifier, DestructionModifier, IllusionModifier, RestorationModifier, EnchantingModifier, OneHandedSkillAdvance, TwoHandedSkillAdvance, MarksmanSkillAdvance, BlockSkillAdvance, SmithingSkillAdvance, HeavyArmorSkillAdvance, LightArmorSkillAdvance, PickpocketSkillAdvance, LockpickingSkillAdvance, SneakingSkillAdvance, AlchemySkillAdvance, SpeechcraftSkillAdvance, AlterationSkillAdvance, ConjurationSkillAdvance, DestructionSkillAdvance, IllusionSkillAdvance, RestorationSkillAdvance, EnchantingSkillAdvance, LeftWeaponSpeedMultiply, DragonSouls, CombatHealthRegenMultiply, OneHandedPowerModifier, TwoHandedPowerModifier, MarksmanPowerModifier, BlockPowerModifier, SmithingPowerModifier, HeavyArmorPowerModifier, LightArmorPowerModifier, PickpocketPowerModifier, LockpickingPowerModifier, SneakingPowerModifier, AlchemyPowerModifier, SpeechcraftPowerModifier, AlterationPowerModifier, ConjurationPowerModifier, DestructionPowerModifier, IllusionPowerModifier, RestorationPowerModifier, EnchantingPowerModifier, DragonRend, AttackDamageMult, HealRateMult, MagickaRateMult, StaminaRateMult, WerewolfPerks, VampirePerks, GrabActorOffset, Grabbed, ReflectDamage, None) | "SecondActorValue": "Aggression"         |
| SecondActorValueWeight  |           | MFF-- | A decimal value                                                                                                                                                 | "SecondActorValueWeight": 3.14           |
| SkillUsageMultiplier    |           | MFF-- | A decimal value                                                                                                                                                 | "SkillUsageMultiplier": 3.14             |
| SpellmakingArea         |           | MFF-- | A numeric value                                                                                                                                                 | "SpellmakingArea": 7                     |
| SpellmakingCastingTime  |           | MFF-- | A decimal value                                                                                                                                                 | "SpellmakingCastingTime": 3.14           |
| TaperCurve              |           | MFF-- | A decimal value                                                                                                                                                 | "TaperCurve": 3.14                       |
| TaperDuration           |           | MFF-- | A decimal value                                                                                                                                                 | "TaperDuration": 3.14                    |
| TaperWeight             |           | MFF-- | A decimal value                                                                                                                                                 | "TaperWeight": 3.14                      |
| TargetType              |           | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation)                                                                                               | "TargetType": "Self"                     |
| Unknown1                |           | MFF-- | A numeric value                                                                                                                                                 | "Unknown1": 7                            |

[⬅ Back to Types](Types.md)

## MATO - MaterialObject

| Field           | Alt       | MFFSM | Value Type         | Example                 |
| --------------- | --------- | ----- | ------------------ | ----------------------- |
| FalloffBias     |           | MFF-- | A decimal value    | "FalloffBias": 3.14     |
| FalloffScale    |           | MFF-- | A decimal value    | "FalloffScale": 3.14    |
| Flags           | DataFlags | MF--- | Flags (SinglePass) | "Flags": "SinglePass"   |
| MaterialUvScale |           | MFF-- | A decimal value    | "MaterialUvScale": 3.14 |
| NoiseUvScale    |           | MFF-- | A decimal value    | "NoiseUvScale": 3.14    |
| NormalDampener  |           | MFF-- | A decimal value    | "NormalDampener": 3.14  |

[⬅ Back to Types](Types.md)

## MATT - MaterialType

| Field              | Alt       | MFFSM | Value Type                         | Example                                      |
| ------------------ | --------- | ----- | ---------------------------------- | -------------------------------------------- |
| Buoyancy           |           | MFF-- | A decimal value                    | "Buoyancy": 3.14                             |
| Flags              | DataFlags | MF--- | Flags (StairMaterial, ArrowsStick) | "Flags": [ "StairMaterial", "-ArrowsStick" ] |
| HavokImpactDataSet |           | MFF-- | Form Key or Editor ID              |                                              |
| Name               | FULL      | MFF-- | A string value                     | "Name": "Hello"                              |
| Parent             |           | MFF-- | Form Key or Editor ID              |                                              |

[⬅ Back to Types](Types.md)

## MESG - Message

| Field       | Alt       | MFFSM | Value Type                      | Example                                   |
| ----------- | --------- | ----- | ------------------------------- | ----------------------------------------- |
| Description | DESC      | MFF-- | A string value                  | "Description": "Hello"                    |
| DisplayTime |           | MFF-- | A numeric value                 | "DisplayTime": 7                          |
| Flags       | DataFlags | MF--- | Flags (MessageBox, AutoDisplay) | "Flags": [ "MessageBox", "-AutoDisplay" ] |
| Name        | FULL      | MFF-- | A string value                  | "Name": "Hello"                           |
| Quest       |           | MFF-- | Form Key or Editor ID           |                                           |

[⬅ Back to Types](Types.md)

## MISC - MiscItem

| Field        | Alt         | MFFSM | Value Type              | Example                     |
| ------------ | ----------- | ----- | ----------------------- | --------------------------- |
| Keywords     | KWDA        | MFFSM | Form Keys or Editor IDs |                             |
| MajorFlags   | RecordFlags | MF--- | Flags (NonPlayable)     | "MajorFlags": "NonPlayable" |
| Name         | FULL        | MFF-- | A string value          | "Name": "Hello"             |
| PickUpSound  | YNAM        | MFF-- | Form Key or Editor ID   |                             |
| PutDownSound | ZNAM        | MFF-- | Form Key or Editor ID   |                             |
| Value        |             | MFF-- | A numeric value         | "Value": 7                  |
| Weight       |             | MFF-- | A decimal value         | "Weight": 3.14              |

[⬅ Back to Types](Types.md)

## MSTT - MoveableStatic

| Field        | Alt         | MFFSM | Value Type                                                                                                                                        | Example                                                         |
| ------------ | ----------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------- |
| Flags        | DataFlags   | MF--- | Flags (OnLocalMap)                                                                                                                                | "Flags": "OnLocalMap"                                           |
| LoopingSound |             | MFF-- | Form Key or Editor ID                                                                                                                             |                                                                 |
| MajorFlags   | RecordFlags | MF--- | Flags (MustUpdateAnims, HiddenFromLocalMap, HasDistantLOD, RandomAnimStart, HasCurrents, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, NavMeshGenerationGround) | "MajorFlags": [ "MustUpdateAnims", "-NavMeshGenerationGround" ] |
| Name         | FULL        | MFF-- | A string value                                                                                                                                    | "Name": "Hello"                                                 |

[⬅ Back to Types](Types.md)

## MOVT - MovementType

| Field                | Alt  | MFFSM | Value Type      | Example                       |
| -------------------- | ---- | ----- | --------------- | ----------------------------- |
| BackRun              |      | MFF-- | A decimal value | "BackRun": 3.14               |
| BackWalk             |      | MFF-- | A decimal value | "BackWalk": 3.14              |
| ForwardRun           |      | MFF-- | A decimal value | "ForwardRun": 3.14            |
| ForwardWalk          |      | MFF-- | A decimal value | "ForwardWalk": 3.14           |
| LeftRun              |      | MFF-- | A decimal value | "LeftRun": 3.14               |
| LeftWalk             |      | MFF-- | A decimal value | "LeftWalk": 3.14              |
| Name                 | FULL | MFF-- | A string value  | "Name": "Hello"               |
| RightRun             |      | MFF-- | A decimal value | "RightRun": 3.14              |
| RightWalk            |      | MFF-- | A decimal value | "RightWalk": 3.14             |
| RotateInPlaceRun     |      | MFF-- | A decimal value | "RotateInPlaceRun": 3.14      |
| RotateInPlaceWalk    |      | MFF-- | A decimal value | "RotateInPlaceWalk": 3.14     |
| RotateWhileMovingRun |      | MFF-- | A decimal value | "RotateWhileMovingRun": 3.14  |
| SPEDDataTypeState    |      | MF--- | Flags (Break0)  | "SPEDDataTypeState": "Break0" |

[⬅ Back to Types](Types.md)

## MUST - MusicTrack

| Field    | Alt | MFFSM | Value Type                                          | Example           |
| -------- | --- | ----- | --------------------------------------------------- | ----------------- |
| Duration |     | MFF-- | A decimal value                                     | "Duration": 3.14  |
| FadeOut  |     | MFF-- | A decimal value                                     | "FadeOut": 3.14   |
| Tracks   |     | MFFSM | Form Keys or Editor IDs                             |                   |
| Type     |     | MF--- | Possible values (Palette, SingleTrack, SilentTrack) | "Type": "Palette" |

[⬅ Back to Types](Types.md)

## MUSC - MusicType

| Field        | Alt       | MFFSM | Value Type                                                                                                    | Example                                           |
| ------------ | --------- | ----- | ------------------------------------------------------------------------------------------------------------- | ------------------------------------------------- |
| FadeDuration |           | MFF-- | A decimal value                                                                                               | "FadeDuration": 3.14                              |
| Flags        | DataFlags | MF--- | Flags (PlaysOneSelection, AbruptTransition, CycleTracks, MaintainTrackOrder, DucksCurrentTrack, DoesNotQueue) | "Flags": [ "PlaysOneSelection", "-DoesNotQueue" ] |
| Tracks       |           | MFFSM | Form Keys or Editor IDs                                                                                       |                                                   |

[⬅ Back to Types](Types.md)

## NPC_ - Npc

| Field                              | Alt         | MFFSM | Value Type                                                      | Example                                              |
| ---------------------------------- | ----------- | ----- | --------------------------------------------------------------- | ---------------------------------------------------- |
| ActorEffect                        | SPLO        | MFFSM | Form Keys or Editor IDs                                         |                                                      |
| AttackRace                         | ATKR        | MFF-- | Form Key or Editor ID                                           |                                                      |
| Class                              | CNAM        | MFF-- | Form Key or Editor ID                                           |                                                      |
| CombatOverridePackageList          | ECOR        | MFF-- | Form Key or Editor ID                                           |                                                      |
| CombatStyle                        | ZNAM        | MFF-- | Form Key or Editor ID                                           |                                                      |
| CrimeFaction                       | CRIF        | MFF-- | Form Key or Editor ID                                           |                                                      |
| DeathItem                          | INAM        | MFF-- | Form Key or Editor ID                                           |                                                      |
| DefaultOutfit                      | DOFT        | MFF-- | Form Key or Editor ID                                           |                                                      |
| DefaultPackageList                 | DPLT        | MFF-- | Form Key or Editor ID                                           |                                                      |
| FarAwayModel                       | ANAM        | MFF-- | Form Key or Editor ID                                           |                                                      |
| GiftFilter                         | GNAM        | MFF-- | Form Key or Editor ID                                           |                                                      |
| GuardWarnOverridePackageList       | GWOR        | MFF-- | Form Key or Editor ID                                           |                                                      |
| HairColor                          | HCLF        | MFF-- | Form Key or Editor ID                                           |                                                      |
| HeadParts                          | PNAM        | MFFSM | Form Keys or Editor IDs                                         |                                                      |
| HeadTexture                        | FTST        | MFF-- | Form Key or Editor ID                                           |                                                      |
| Height                             | NAM6        | MFF-- | A decimal value                                                 | "Height": 3.14                                       |
| Items                              | Item        | MFFSM | JSON objects containing item Form Key/Editor ID and Count (QTY) | "Items": { "Item": "021FED:Skyrim.esm", "Count": 3 } |
| Keywords                           | KWDA        | MFFSM | Form Keys or Editor IDs                                         |                                                      |
| MajorFlags                         | RecordFlags | MF--- | Flags (BleedoutOverride)                                        | "MajorFlags": "BleedoutOverride"                     |
| NAM5                               |             | MFF-- | A numeric value                                                 | "NAM5": 7                                            |
| Name                               | FULL        | MFF-- | A string value                                                  | "Name": "Hello"                                      |
| ObserveDeadBodyOverridePackageList | OCOR        | MFF-- | Form Key or Editor ID                                           |                                                      |
| Packages                           | PKID        | MFFSM | Form Keys or Editor IDs                                         |                                                      |
| Race                               | RNAM        | MFF-- | Form Key or Editor ID                                           |                                                      |
| ShortName                          | ONAM        | MFF-- | A string value                                                  | "ShortName": "Hello"                                 |
| SleepingOutfit                     | SOFT        | MFF-- | Form Key or Editor ID                                           |                                                      |
| Sound                              |             | MFF-- | Form Key or Editor ID                                           |                                                      |
| SoundLevel                         | NAM8        | MF--- | Possible values (Loud, Normal, Silent, VeryLoud)                | "SoundLevel": "Loud"                                 |
| SpectatorOverridePackageList       | SPOR        | MFF-- | Form Key or Editor ID                                           |                                                      |
| Template                           | TPLT        | MFF-- | Form Key or Editor ID                                           |                                                      |
| Voice                              | VTCK        | MFF-- | Form Key or Editor ID                                           |                                                      |
| Weight                             | NAM7        | MFF-- | A decimal value                                                 | "Weight": 3.14                                       |
| WornArmor                          | WNAM        | MFF-- | Form Key or Editor ID                                           |                                                      |

[⬅ Back to Types](Types.md)

## ENCH - ObjectEffect

| Field             | Alt       | MFFSM | Value Type                                                        | Example                                                                                |
| ----------------- | --------- | ----- | ----------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| BaseEnchantment   |           | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| CastType          |           | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration)    | "CastType": "ConstantEffect"                                                           |
| ChargeTime        |           | MFF-- | A decimal value                                                   | "ChargeTime": 3.14                                                                     |
| Effects           |           | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EnchantmentAmount | EAMT      | MFF-- | A numeric value                                                   | "EnchantmentAmount": 7                                                                 |
| EnchantmentCost   |           | MFF-- | A numeric value                                                   | "EnchantmentCost": 7                                                                   |
| EnchantType       |           | MF--- | Possible values (Enchantment, StaffEnchantment)                   | "EnchantType": "Enchantment"                                                           |
| ENITDataTypeState |           | MF--- | Flags (Break0)                                                    | "ENITDataTypeState": "Break0"                                                          |
| Flags             | DataFlags | MF--- | Flags (NoAutoCalc, ExtendDurationOnRecast)                        | "Flags": [ "NoAutoCalc", "-ExtendDurationOnRecast" ]                                   |
| Name              | FULL      | MFF-- | A string value                                                    | "Name": "Hello"                                                                        |
| TargetType        |           | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation) | "TargetType": "Self"                                                                   |
| WornRestrictions  |           | MFF-- | Form Key or Editor ID                                             |                                                                                        |

[⬅ Back to Types](Types.md)

## OTFT - Outfit

| Field | Alt  | MFFSM | Value Type              | Example |
| ----- | ---- | ----- | ----------------------- | ------- |
| Items | Item | MFFSM | Form Keys or Editor IDs |         |

[⬅ Back to Types](Types.md)

## PACK - Package

| Field                     | Alt       | MFFSM | Value Type                                                                                                                                 | Example                                                     |
| ------------------------- | --------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------------------------- |
| CombatStyle               |           | MFF-- | Form Key or Editor ID                                                                                                                      |                                                             |
| DataInputVersion          |           | MFF-- | A numeric value                                                                                                                            | "DataInputVersion": 7                                       |
| Flags                     | DataFlags | MF--- | Flags (OffersServices, MustComplete, MaintainSpeedAtGoal, UnlockDoorsAtPackageStart, UnlockDoorsAtPackageEnd, ContinueIfPcNear, OncePerDay, PreferredSpeed, AlwaysSneak, AllowSwimming, IgnoreCombat, WeaponsUnequipped, WeaponDrawn, WearSleepOutfit, NoCombatAlert) | "Flags": [ "OffersServices", "-NoCombatAlert" ]             |
| InterruptOverride         |           | MF--- | Possible values (None, Spectator, ObserveDead, GuardWarn, Combat)                                                                          | "InterruptOverride": "None"                                 |
| InteruptFlags             |           | MF--- | Flags (HellosToPlayer, RandomConversations, ObserveCombatBehavior, GreetCorpseBehavior, ReactionToPlayerActions, FriendlyFireComments, AggroRadiusBehavior, AllowIdleChatter, WorldInteractions) | "InteruptFlags": [ "HellosToPlayer", "-WorldInteractions" ] |
| OwnerQuest                |           | MFF-- | Form Key or Editor ID                                                                                                                      |                                                             |
| PackageTemplate           |           | MFF-- | Form Key or Editor ID                                                                                                                      |                                                             |
| PreferredSpeed            |           | MF--- | Possible values (Walk, Jog, Run, FastWalk)                                                                                                 | "PreferredSpeed": "Walk"                                    |
| ScheduleDate              |           | MFF-- | A numeric value                                                                                                                            | "ScheduleDate": 7                                           |
| ScheduleDayOfWeek         |           | MF--- | Possible values (Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Weekdays, Weekends, MondayWednesdayFriday, TuesdayThursday, Any) | "ScheduleDayOfWeek": "Sunday"                               |
| ScheduleDurationInMinutes |           | MFF-- | A numeric value                                                                                                                            | "ScheduleDurationInMinutes": 7                              |
| ScheduleHour              |           | MFF-- | A numeric value                                                                                                                            | "ScheduleHour": 7                                           |
| ScheduleMinute            |           | MFF-- | A numeric value                                                                                                                            | "ScheduleMinute": 7                                         |
| ScheduleMonth             |           | MFF-- | A numeric value                                                                                                                            | "ScheduleMonth": 7                                          |
| Type                      |           | MF--- | Possible values (Package, PackageTemplate)                                                                                                 | "Type": "Package"                                           |

[⬅ Back to Types](Types.md)

## PERK - Perk

| Field       | Alt         | MFFSM | Value Type                                                        | Example                                                                                |
| ----------- | ----------- | ----- | ----------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| Description | DESC        | MFF-- | A string value                                                    | "Description": "Hello"                                                                 |
| Effects     |             | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| Level       |             | MFF-- | A numeric value                                                   | "Level": 7                                                                             |
| MajorFlags  | RecordFlags | MF--- | Flags (NonPlayable)                                               | "MajorFlags": "NonPlayable"                                                            |
| Name        | FULL        | MFF-- | A string value                                                    | "Name": "Hello"                                                                        |
| NextPerk    |             | MFF-- | Form Key or Editor ID                                             |                                                                                        |
| NumRanks    |             | MFF-- | A numeric value                                                   | "NumRanks": 7                                                                          |

[⬅ Back to Types](Types.md)

## PROJ - Projectile

| Field                        | Alt       | MFFSM | Value Type                                                                                                                                                     | Example                              |
| ---------------------------- | --------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------ |
| CollisionLayer               |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| CollisionRadius              |           | MFF-- | A decimal value                                                                                                                                                | "CollisionRadius": 3.14              |
| ConeSpread                   |           | MFF-- | A decimal value                                                                                                                                                | "ConeSpread": 3.14                   |
| CountdownSound               |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| DecalData                    |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| DefaultWeaponSource          |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| DisaleSound                  |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| Explosion                    |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| ExplosionAltTriggerProximity |           | MFF-- | A decimal value                                                                                                                                                | "ExplosionAltTriggerProximity": 3.14 |
| ExplosionAltTriggerTimer     |           | MFF-- | A decimal value                                                                                                                                                | "ExplosionAltTriggerTimer": 3.14     |
| FadeDuration                 |           | MFF-- | A decimal value                                                                                                                                                | "FadeDuration": 3.14                 |
| Flags                        | DataFlags | MF--- | Flags (Hitscan, Explosion, AltTrigger, MuzzleFlash, CanBeDisabled, CanBePickedUp, Supersonic, PinsLimbs, PassThroughSmallTransparent, DisableCombatAimCorrection, Rotation) | "Flags": [ "Hitscan", "-Rotation" ]  |
| Gravity                      |           | MFF-- | A decimal value                                                                                                                                                | "Gravity": 3.14                      |
| ImpactForce                  |           | MFF-- | A decimal value                                                                                                                                                | "ImpactForce": 3.14                  |
| Lifetime                     |           | MFF-- | A decimal value                                                                                                                                                | "Lifetime": 3.14                     |
| Light                        |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| MuzzleFlash                  |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| MuzzleFlashDuration          |           | MFF-- | A decimal value                                                                                                                                                | "MuzzleFlashDuration": 3.14          |
| Name                         | FULL      | MFF-- | A string value                                                                                                                                                 | "Name": "Hello"                      |
| Range                        |           | MFF-- | A decimal value                                                                                                                                                | "Range": 3.14                        |
| RelaunchInterval             |           | MFF-- | A decimal value                                                                                                                                                | "RelaunchInterval": 3.14             |
| Sound                        |           | MFF-- | Form Key or Editor ID                                                                                                                                          |                                      |
| SoundLevel                   |           | MFF-- | A numeric value                                                                                                                                                | "SoundLevel": 7                      |
| Speed                        |           | MFF-- | A decimal value                                                                                                                                                | "Speed": 3.14                        |
| TracerChance                 |           | MFF-- | A decimal value                                                                                                                                                | "TracerChance": 3.14                 |
| Type                         |           | MF--- | Possible values (Missile, Lobber, Beam, Flame, Cone, Barrier, Arrow)                                                                                           | "Type": "Missile"                    |

[⬅ Back to Types](Types.md)

## QUST - Quest

| Field              | Alt       | MFFSM | Value Type                                                                                                                                         | Example                                                    |
| ------------------ | --------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| Description        | DESC      | MFF-- | A string value                                                                                                                                     | "Description": "Hello"                                     |
| Filter             |           | MFF-- | A string value                                                                                                                                     | "Filter": "Hello"                                          |
| Flags              | DataFlags | MF--- | Flags (StartGameEnabled, AllowRepeatedStages, RunOnce, ExcludeFromDialogExport, WarnOnAliasFillFailure)                                            | "Flags": [ "StartGameEnabled", "-WarnOnAliasFillFailure" ] |
| Name               | FULL      | MFF-- | A string value                                                                                                                                     | "Name": "Hello"                                            |
| NextAliasID        |           | MFF-- | A numeric value                                                                                                                                    | "NextAliasID": 7                                           |
| Priority           |           | MFF-- | A numeric value                                                                                                                                    | "Priority": 7                                              |
| QuestFormVersion   |           | MFF-- | A numeric value                                                                                                                                    | "QuestFormVersion": 7                                      |
| TextDisplayGlobals |           | MFFSM | Form Keys or Editor IDs                                                                                                                            |                                                            |
| Type               |           | MF--- | Possible values (None, MainQuest, MageGuild, ThievesGuild, DarkBrotherhood, CompanionQuests, Misc, Daedric, SideQuest, CivilWar, Vampire, Dragonborn) | "Type": "None"                                             |

[⬅ Back to Types](Types.md)

## RACE - Race

| Field                     | Alt         | MFFSM | Value Type                                                                                                                                           | Example                                         |
| ------------------------- | ----------- | ----- | ---------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------- |
| AccelerationRate          |             | MFF-- | A decimal value                                                                                                                                      | "AccelerationRate": 3.14                        |
| ActorEffect               |             | MFFSM | Form Keys or Editor IDs                                                                                                                              |                                                 |
| AimAngleTolerance         |             | MFF-- | A decimal value                                                                                                                                      | "AimAngleTolerance": 3.14                       |
| AngularAccelerationRate   |             | MFF-- | A decimal value                                                                                                                                      | "AngularAccelerationRate": 3.14                 |
| AngularTolerance          |             | MFF-- | A decimal value                                                                                                                                      | "AngularTolerance": 3.14                        |
| ArmorRace                 |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| AttackRace                |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| BaseCarryWeight           |             | MFF-- | A decimal value                                                                                                                                      | "BaseCarryWeight": 3.14                         |
| BaseMass                  |             | MFF-- | A decimal value                                                                                                                                      | "BaseMass": 3.14                                |
| BaseMovementDefaultFly    |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| BaseMovementDefaultRun    |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| BaseMovementDefaultSneak  |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| BaseMovementDefaultSprint |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| BaseMovementDefaultSwim   |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| BaseMovementDefaultWalk   |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| BodyBipedObject           |             | MF--- | Possible values (Head, Hair, Body, Hands, Forearms, Amulet, Ring, Feet, Calves, Shield, Tail, LongHair, Circlet, Ears, DecapitateHead, Decapitate, FX01, None) | "BodyBipedObject": "Head"                       |
| BodyPartData              |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| CloseLootSound            |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| DecapitationFX            |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| DecelerationRate          |             | MFF-- | A decimal value                                                                                                                                      | "DecelerationRate": 3.14                        |
| Description               | DESC        | MFF-- | A string value                                                                                                                                       | "Description": "Hello"                          |
| EquipmentFlags            |             | MF--- | Flags (HandToHand, OneHandSword, OneHandDagger, OneHandAxe, OneHandMace, TwoHandSword, TwoHandAxe, Bow, Staff, Spell, Shield, Torch, Crossbow)       | "EquipmentFlags": [ "HandToHand", "-Crossbow" ] |
| EquipmentSlots            |             | MFFSM | Form Keys or Editor IDs                                                                                                                              |                                                 |
| Eyes                      |             | MFFSM | Form Keys or Editor IDs                                                                                                                              |                                                 |
| FacegenFaceClamp          |             | MFF-- | A decimal value                                                                                                                                      | "FacegenFaceClamp": 3.14                        |
| FacegenMainClamp          |             | MFF-- | A decimal value                                                                                                                                      | "FacegenMainClamp": 3.14                        |
| Flags                     | DataFlags   | MF--- | Flags (Playable, FaceGenHead, Child, TiltFrontBack, TiltLeftRight, NoShadow, Swims, Flies, Walks, Immobile, NotPunishable, NoCombatInWater, NoRotatingToHeadTrack, DontShowBloodSpray, DontShowBloodDecal, UsesHeadTrackAnims, SpellsAlignWithMagicNode, UseWorldRaycastsForFootIK, AllowRagdollCollision, RegenHpInCombat, CantOpenDoors, AllowPcDialog, NoKnockdowns, AllowPickpocket, AlwaysUseProxyController, DontShowWeaponBlood, OverlayHeadPartList, OverrideHeadPartList, CanPickupItems, AllowMultipleMembraneShaders, CanDualWield, AvoidsRoads, UseAdvancedAvoidance, NonHostile, AllowMountedCombat) | "Flags": [ "Playable", "-AllowMountedCombat" ]  |
| FlightRadius              |             | MFF-- | A decimal value                                                                                                                                      | "FlightRadius": 3.14                            |
| HairBipedObject           |             | MF--- | Possible values (Head, Hair, Body, Hands, Forearms, Amulet, Ring, Feet, Calves, Shield, Tail, LongHair, Circlet, Ears, DecapitateHead, Decapitate, FX01, None) | "HairBipedObject": "Head"                       |
| Hairs                     |             | MFFSM | Form Keys or Editor IDs                                                                                                                              |                                                 |
| HeadBipedObject           |             | MF--- | Possible values (Head, Hair, Body, Hands, Forearms, Amulet, Ring, Feet, Calves, Shield, Tail, LongHair, Circlet, Ears, DecapitateHead, Decapitate, FX01, None) | "HeadBipedObject": "Head"                       |
| ImpactDataSet             |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| InjuredHealthPercent      |             | MFF-- | A decimal value                                                                                                                                      | "InjuredHealthPercent": 3.14                    |
| Keywords                  | KWDA        | MFFSM | Form Keys or Editor IDs                                                                                                                              |                                                 |
| MajorFlags                | RecordFlags | MF--- | Flags (Critter)                                                                                                                                      | "MajorFlags": "Critter"                         |
| MaterialType              |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| MorphRace                 |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| Name                      | FULL        | MFF-- | A string value                                                                                                                                       | "Name": "Hello"                                 |
| NumberOfTintsInList       |             | MFF-- | A numeric value                                                                                                                                      | "NumberOfTintsInList": 7                        |
| OpenLootSound             |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| ShieldBipedObject         |             | MF--- | Possible values (Head, Hair, Body, Hands, Forearms, Amulet, Ring, Feet, Calves, Shield, Tail, LongHair, Circlet, Ears, DecapitateHead, Decapitate, FX01, None) | "ShieldBipedObject": "Head"                     |
| Size                      |             | MF--- | Possible values (Small, Medium, Large, ExtraLarge)                                                                                                   | "Size": "Small"                                 |
| Skin                      |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| UnarmedDamage             |             | MFF-- | A decimal value                                                                                                                                      | "UnarmedDamage": 3.14                           |
| UnarmedEquipSlot          |             | MFF-- | Form Key or Editor ID                                                                                                                                |                                                 |
| UnarmedReach              |             | MFF-- | A decimal value                                                                                                                                      | "UnarmedReach": 3.14                            |
| Weight                    |             | MFF-- | A decimal value                                                                                                                                      | "Weight": 3.14                                  |

[⬅ Back to Types](Types.md)

## REGN - Region

| Field      | Alt         | MFFSM | Value Type            | Example                      |
| ---------- | ----------- | ----- | --------------------- | ---------------------------- |
| MajorFlags | RecordFlags | MF--- | Flags (BorderRegion)  | "MajorFlags": "BorderRegion" |
| Worldspace |             | MFF-- | Form Key or Editor ID |                              |

[⬅ Back to Types](Types.md)

## RELA - Relationship

| Field           | Alt         | MFFSM | Value Type                                                                                     | Example                |
| --------------- | ----------- | ----- | ---------------------------------------------------------------------------------------------- | ---------------------- |
| AssociationType |             | MFF-- | Form Key or Editor ID                                                                          |                        |
| Child           |             | MFF-- | Form Key or Editor ID                                                                          |                        |
| Flags           | DataFlags   | MF--- | Flags (Secret)                                                                                 | "Flags": "Secret"      |
| MajorFlags      | RecordFlags | MF--- | Flags (Secret)                                                                                 | "MajorFlags": "Secret" |
| Parent          |             | MFF-- | Form Key or Editor ID                                                                          |                        |
| Rank            |             | MF--- | Possible values (Lover, Ally, Confidant, Friend, Acquaintance, Rival, Foe, Enemy, Archnemesis) | "Rank": "Lover"        |

[⬅ Back to Types](Types.md)

## REVB - ReverbParameters

| Field             | Alt | MFFSM | Value Type      | Example                |
| ----------------- | --- | ----- | --------------- | ---------------------- |
| DecayHfRatio      |     | MFF-- | A decimal value | "DecayHfRatio": 3.14   |
| DecayMilliseconds |     | MFF-- | A numeric value | "DecayMilliseconds": 7 |
| HfReferenceHertz  |     | MFF-- | A numeric value | "HfReferenceHertz": 7  |
| ReflectDelayMS    |     | MFF-- | A numeric value | "ReflectDelayMS": 7    |
| Reflections       |     | MFF-- | A numeric value | "Reflections": 7       |
| ReverbAmp         |     | MFF-- | A numeric value | "ReverbAmp": 7         |
| ReverbDelayMS     |     | MFF-- | A numeric value | "ReverbDelayMS": 7     |
| RoomFilter        |     | MFF-- | A numeric value | "RoomFilter": 7        |
| RoomHfFilter      |     | MFF-- | A numeric value | "RoomHfFilter": 7      |

[⬅ Back to Types](Types.md)

## SCEN - Scene

| Field           | Alt       | MFFSM | Value Type                                                                          | Example                                            |
| --------------- | --------- | ----- | ----------------------------------------------------------------------------------- | -------------------------------------------------- |
| Flags           | DataFlags | MF--- | Flags (BeginOnQuestStart, StopQuestOnEnd, RepeatConditionsWhileTrue, Interruptable) | "Flags": [ "BeginOnQuestStart", "-Interruptable" ] |
| LastActionIndex |           | MFF-- | A numeric value                                                                     | "LastActionIndex": 7                               |
| Quest           |           | MFF-- | Form Key or Editor ID                                                               |                                                    |

[⬅ Back to Types](Types.md)

## SCRL - Scroll

| Field             | Alt       | MFFSM | Value Type                                                                                                              | Example                                                                                |
| ----------------- | --------- | ----- | ----------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| BaseCost          |           | MFF-- | A numeric value                                                                                                         | "BaseCost": 7                                                                          |
| CastDuration      |           | MFF-- | A decimal value                                                                                                         | "CastDuration": 3.14                                                                   |
| CastType          |           | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration)                                                          | "CastType": "ConstantEffect"                                                           |
| ChargeTime        |           | MFF-- | A decimal value                                                                                                         | "ChargeTime": 3.14                                                                     |
| Description       | DESC      | MFF-- | A string value                                                                                                          | "Description": "Hello"                                                                 |
| Effects           |           | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data                                                       | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipmentType     | ETYP      | MFF-- | Form Key or Editor ID                                                                                                   |                                                                                        |
| Flags             | DataFlags | MF--- | Flags (ManualCostCalc, PCStartSpell, AreaEffectIgnoresLOS, IgnoreResistance, NoAbsorbOrReflect, NoDualCastModification) | "Flags": [ "ManualCostCalc", "-NoDualCastModification" ]                               |
| HalfCostPerk      |           | MFF-- | Form Key or Editor ID                                                                                                   |                                                                                        |
| Keywords          | KWDA      | MFFSM | Form Keys or Editor IDs                                                                                                 |                                                                                        |
| MenuDisplayObject | MDOB      | MFF-- | Form Key or Editor ID                                                                                                   |                                                                                        |
| Name              | FULL      | MFF-- | A string value                                                                                                          | "Name": "Hello"                                                                        |
| PickUpSound       | YNAM      | MFF-- | Form Key or Editor ID                                                                                                   |                                                                                        |
| PutDownSound      | ZNAM      | MFF-- | Form Key or Editor ID                                                                                                   |                                                                                        |
| Range             |           | MFF-- | A decimal value                                                                                                         | "Range": 3.14                                                                          |
| TargetType        |           | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation)                                                       | "TargetType": "Self"                                                                   |
| Type              |           | MF--- | Possible values (Spell, Disease, Power, LesserPower, Ability, Poison, Addiction, Voice)                                 | "Type": "Spell"                                                                        |
| Value             |           | MFF-- | A numeric value                                                                                                         | "Value": 7                                                                             |
| Weight            |           | MFF-- | A decimal value                                                                                                         | "Weight": 3.14                                                                         |

[⬅ Back to Types](Types.md)

## SPGD - ShaderParticleGeometry

| Field                | Alt | MFFSM | Value Type                   | Example                      |
| -------------------- | --- | ----- | ---------------------------- | ---------------------------- |
| BoxSize              |     | MFF-- | A numeric value              | "BoxSize": 7                 |
| CenterOffsetMax      |     | MFF-- | A decimal value              | "CenterOffsetMax": 3.14      |
| CenterOffsetMin      |     | MFF-- | A decimal value              | "CenterOffsetMin": 3.14      |
| GravityVelocity      |     | MFF-- | A decimal value              | "GravityVelocity": 3.14      |
| InitialRotationRange |     | MFF-- | A decimal value              | "InitialRotationRange": 3.14 |
| NumSubtexturesX      |     | MFF-- | A numeric value              | "NumSubtexturesX": 7         |
| NumSubtexturesY      |     | MFF-- | A numeric value              | "NumSubtexturesY": 7         |
| ParticleDensity      |     | MFF-- | A decimal value              | "ParticleDensity": 3.14      |
| ParticleSizeX        |     | MFF-- | A decimal value              | "ParticleSizeX": 3.14        |
| ParticleSizeY        |     | MFF-- | A decimal value              | "ParticleSizeY": 3.14        |
| RotationVelocity     |     | MFF-- | A decimal value              | "RotationVelocity": 3.14     |
| Type                 |     | MF--- | Possible values (Rain, Snow) | "Type": "Rain"               |

[⬅ Back to Types](Types.md)

## SHOU - Shout

| Field             | Alt         | MFFSM | Value Type                  | Example                             |
| ----------------- | ----------- | ----- | --------------------------- | ----------------------------------- |
| Description       | DESC        | MFF-- | A string value              | "Description": "Hello"              |
| MajorFlags        | RecordFlags | MF--- | Flags (TreatSpellsAsPowers) | "MajorFlags": "TreatSpellsAsPowers" |
| MenuDisplayObject |             | MFF-- | Form Key or Editor ID       |                                     |
| Name              | FULL        | MFF-- | A string value              | "Name": "Hello"                     |

[⬅ Back to Types](Types.md)

## SLGM - SoulGem

| Field           | Alt         | MFFSM | Value Type                                                    | Example                        |
| --------------- | ----------- | ----- | ------------------------------------------------------------- | ------------------------------ |
| ContainedSoul   |             | MF--- | Possible values (None, Petty, Lesser, Common, Greater, Grand) | "ContainedSoul": "None"        |
| Keywords        | KWDA        | MFFSM | Form Keys or Editor IDs                                       |                                |
| LinkedTo        |             | MFF-- | Form Key or Editor ID                                         |                                |
| MajorFlags      | RecordFlags | MF--- | Flags (CanHoldNpcSoul)                                        | "MajorFlags": "CanHoldNpcSoul" |
| MaximumCapacity |             | MF--- | Possible values (None, Petty, Lesser, Common, Greater, Grand) | "MaximumCapacity": "None"      |
| Name            | FULL        | MFF-- | A string value                                                | "Name": "Hello"                |
| PickUpSound     | YNAM        | MFF-- | Form Key or Editor ID                                         |                                |
| PutDownSound    | ZNAM        | MFF-- | Form Key or Editor ID                                         |                                |
| Value           |             | MFF-- | A numeric value                                               | "Value": 7                     |
| Weight          |             | MFF-- | A decimal value                                               | "Weight": 3.14                 |

[⬅ Back to Types](Types.md)

## SNCT - SoundCategory

| Field                  | Alt       | MFFSM | Value Type                                    | Example                                                 |
| ---------------------- | --------- | ----- | --------------------------------------------- | ------------------------------------------------------- |
| DefaultMenuVolume      |           | MFF-- | A decimal value                               | "DefaultMenuVolume": 3.14                               |
| Flags                  | DataFlags | MF--- | Flags (MuteWhenSubmerged, ShouldAppearOnMenu) | "Flags": [ "MuteWhenSubmerged", "-ShouldAppearOnMenu" ] |
| Name                   | FULL      | MFF-- | A string value                                | "Name": "Hello"                                         |
| Parent                 |           | MFF-- | Form Key or Editor ID                         |                                                         |
| StaticVolumeMultiplier |           | MFF-- | A decimal value                               | "StaticVolumeMultiplier": 3.14                          |

[⬅ Back to Types](Types.md)

## SNDR - SoundDescriptor

| Field             | Alt | MFFSM | Value Type                 | Example                   |
| ----------------- | --- | ----- | -------------------------- | ------------------------- |
| AlternateSoundFor |     | MFF-- | Form Key or Editor ID      |                           |
| Category          |     | MFF-- | Form Key or Editor ID      |                           |
| OutputModel       |     | MFF-- | Form Key or Editor ID      |                           |
| Priority          |     | MFF-- | A numeric value            | "Priority": 7             |
| StaticAttenuation |     | MFF-- | A decimal value            | "StaticAttenuation": 3.14 |
| String            |     | MFF-- | A string value             | "String": "Hello"         |
| Type              |     | MF--- | Possible values (Standard) | "Type": "Standard"        |
| Variance          |     | MFF-- | A numeric value            | "Variance": 7             |

[⬅ Back to Types](Types.md)

## SOUN - SoundMarker

| Field           | Alt | MFFSM | Value Type            | Example |
| --------------- | --- | ----- | --------------------- | ------- |
| SoundDescriptor |     | MFF-- | Form Key or Editor ID |         |

[⬅ Back to Types](Types.md)

## SOPM - SoundOutputModel

| Field | Alt | MFFSM | Value Type                                       | Example            |
| ----- | --- | ----- | ------------------------------------------------ | ------------------ |
| Type  |     | MF--- | Possible values (UsesHrtf, DefinedSpeakerOutput) | "Type": "UsesHrtf" |

[⬅ Back to Types](Types.md)

## SPEL - Spell

| Field             | Alt       | MFFSM | Value Type                                                                                                              | Example                                                                                |
| ----------------- | --------- | ----- | ----------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| BaseCost          |           | MFF-- | A numeric value                                                                                                         | "BaseCost": 7                                                                          |
| CastDuration      |           | MFF-- | A decimal value                                                                                                         | "CastDuration": 3.14                                                                   |
| CastType          |           | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration)                                                          | "CastType": "ConstantEffect"                                                           |
| ChargeTime        |           | MFF-- | A decimal value                                                                                                         | "ChargeTime": 3.14                                                                     |
| Description       | DESC      | MFF-- | A string value                                                                                                          | "Description": "Hello"                                                                 |
| Effects           |           | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data                                                       | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipmentType     | ETYP      | MFF-- | Form Key or Editor ID                                                                                                   |                                                                                        |
| Flags             | DataFlags | MF--- | Flags (ManualCostCalc, PCStartSpell, AreaEffectIgnoresLOS, IgnoreResistance, NoAbsorbOrReflect, NoDualCastModification) | "Flags": [ "ManualCostCalc", "-NoDualCastModification" ]                               |
| HalfCostPerk      |           | MFF-- | Form Key or Editor ID                                                                                                   |                                                                                        |
| Keywords          | KWDA      | MFFSM | Form Keys or Editor IDs                                                                                                 |                                                                                        |
| MenuDisplayObject |           | MFF-- | Form Key or Editor ID                                                                                                   |                                                                                        |
| Name              | FULL      | MFF-- | A string value                                                                                                          | "Name": "Hello"                                                                        |
| Range             |           | MFF-- | A decimal value                                                                                                         | "Range": 3.14                                                                          |
| TargetType        |           | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation)                                                       | "TargetType": "Self"                                                                   |
| Type              |           | MF--- | Possible values (Spell, Disease, Power, LesserPower, Ability, Poison, Addiction, Voice)                                 | "Type": "Spell"                                                                        |

[⬅ Back to Types](Types.md)

## STAT - Static

| Field             | Alt         | MFFSM | Value Type                                                                                                                                        | Example                                                    |
| ----------------- | ----------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| DNAMDataTypeState |             | MF--- | Flags (Break0)                                                                                                                                    | "DNAMDataTypeState": "Break0"                              |
| Flags             | DataFlags   | MF--- | Flags (ConsideredSnow)                                                                                                                            | "Flags": "ConsideredSnow"                                  |
| MajorFlags        | RecordFlags | MF--- | Flags (NeverFades, HasTreeLOD, AddOnLODObject, HiddenFromLocalMap, HasDistantLOD, UsesHdLodTexture, HasCurrents, IsMarker, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, ShowInWorldMap, NavMeshGenerationGround) | "MajorFlags": [ "NeverFades", "-NavMeshGenerationGround" ] |
| Material          |             | MFF-- | Form Key or Editor ID                                                                                                                             |                                                            |
| MaxAngle          |             | MFF-- | A decimal value                                                                                                                                   | "MaxAngle": 3.14                                           |

[⬅ Back to Types](Types.md)

## TACT - TalkingActivator

| Field        | Alt         | MFFSM | Value Type                                                | Example                                                 |
| ------------ | ----------- | ----- | --------------------------------------------------------- | ------------------------------------------------------- |
| FNAM         |             | MFF-- | A numeric value                                           | "FNAM": 7                                               |
| Keywords     | KWDA        | MFFSM | Form Keys or Editor IDs                                   |                                                         |
| LoopingSound |             | MFF-- | Form Key or Editor ID                                     |                                                         |
| MajorFlags   | RecordFlags | MF--- | Flags (HiddenFromLocalMap, RandomAnimStart, RadioStation) | "MajorFlags": [ "HiddenFromLocalMap", "-RadioStation" ] |
| Name         | FULL        | MFF-- | A string value                                            | "Name": "Hello"                                         |
| PNAM         |             | MFF-- | A numeric value                                           | "PNAM": 7                                               |
| VoiceType    |             | MFF-- | Form Key or Editor ID                                     |                                                         |

[⬅ Back to Types](Types.md)

## TXST - TextureSet

| Field | Alt       | MFFSM | Value Type                                                     | Example                                                 |
| ----- | --------- | ----- | -------------------------------------------------------------- | ------------------------------------------------------- |
| Flags | DataFlags | MF--- | Flags (NoSpecularMap, FaceGenTextures, HasModelSpaceNormalMap) | "Flags": [ "NoSpecularMap", "-HasModelSpaceNormalMap" ] |

[⬅ Back to Types](Types.md)

## TREE - Tree

| Field             | Alt         | MFFSM | Value Type            | Example                       |
| ----------------- | ----------- | ----- | --------------------- | ----------------------------- |
| BranchFlexibility |             | MFF-- | A decimal value       | "BranchFlexibility": 3.14     |
| HarvestSound      |             | MFF-- | Form Key or Editor ID |                               |
| Ingredient        |             | MFF-- | Form Key or Editor ID |                               |
| LeafAmplitude     |             | MFF-- | A decimal value       | "LeafAmplitude": 3.14         |
| LeafFrequency     |             | MFF-- | A decimal value       | "LeafFrequency": 3.14         |
| MajorFlags        | RecordFlags | MF--- | Flags (HasDistantLOD) | "MajorFlags": "HasDistantLOD" |
| Name              | FULL        | MFF-- | A string value        | "Name": "Hello"               |
| TrunkFlexibility  |             | MFF-- | A decimal value       | "TrunkFlexibility": 3.14      |

[⬅ Back to Types](Types.md)

## RFCT - VisualEffect

| Field     | Alt       | MFFSM | Value Type                                                  | Example                                               |
| --------- | --------- | ----- | ----------------------------------------------------------- | ----------------------------------------------------- |
| EffectArt |           | MFF-- | Form Key or Editor ID                                       |                                                       |
| Flags     | DataFlags | MF--- | Flags (RotateToFaceTarget, AttachToCamera, InheritRotation) | "Flags": [ "RotateToFaceTarget", "-InheritRotation" ] |
| Shader    |           | MFF-- | Form Key or Editor ID                                       |                                                       |

[⬅ Back to Types](Types.md)

## VTYP - VoiceType

| Field | Alt       | MFFSM | Value Type                         | Example                                      |
| ----- | --------- | ----- | ---------------------------------- | -------------------------------------------- |
| Flags | DataFlags | MF--- | Flags (AllowDefaultDialog, Female) | "Flags": [ "AllowDefaultDialog", "-Female" ] |

[⬅ Back to Types](Types.md)

## WATR - Water

| Field                          | Alt       | MFFSM | Value Type                                        | Example                                      |
| ------------------------------ | --------- | ----- | ------------------------------------------------- | -------------------------------------------- |
| DamagePerSecond                |           | MFF-- | A numeric value                                   | "DamagePerSecond": 7                         |
| DepthNormals                   |           | MFF-- | A decimal value                                   | "DepthNormals": 3.14                         |
| DepthReflections               |           | MFF-- | A decimal value                                   | "DepthReflections": 3.14                     |
| DepthRefraction                |           | MFF-- | A decimal value                                   | "DepthRefraction": 3.14                      |
| DepthSpecularLighting          |           | MFF-- | A decimal value                                   | "DepthSpecularLighting": 3.14                |
| DisplacementDampner            |           | MFF-- | A decimal value                                   | "DisplacementDampner": 3.14                  |
| DisplacementFalloff            |           | MFF-- | A decimal value                                   | "DisplacementFalloff": 3.14                  |
| DisplacementFoce               |           | MFF-- | A decimal value                                   | "DisplacementFoce": 3.14                     |
| DisplacementStartingSize       |           | MFF-- | A decimal value                                   | "DisplacementStartingSize": 3.14             |
| DisplacementVelocity           |           | MFF-- | A decimal value                                   | "DisplacementVelocity": 3.14                 |
| DNAMDataTypeState              |           | MF--- | Flags (Break0)                                    | "DNAMDataTypeState": "Break0"                |
| Flags                          | DataFlags | MF--- | Flags (CausesDamage, EnableFlowmap, BlendNormals) | "Flags": [ "CausesDamage", "-BlendNormals" ] |
| FogAboveWaterAmount            |           | MFF-- | A decimal value                                   | "FogAboveWaterAmount": 3.14                  |
| FogAboveWaterDistanceFarPlane  |           | MFF-- | A decimal value                                   | "FogAboveWaterDistanceFarPlane": 3.14        |
| FogAboveWaterDistanceNearPlane |           | MFF-- | A decimal value                                   | "FogAboveWaterDistanceNearPlane": 3.14       |
| FogUnderWaterAmount            |           | MFF-- | A decimal value                                   | "FogUnderWaterAmount": 3.14                  |
| FogUnderWaterDistanceFarPlane  |           | MFF-- | A decimal value                                   | "FogUnderWaterDistanceFarPlane": 3.14        |
| FogUnderWaterDistanceNearPlane |           | MFF-- | A decimal value                                   | "FogUnderWaterDistanceNearPlane": 3.14       |
| ImageSpace                     |           | MFF-- | Form Key or Editor ID                             |                                              |
| Material                       |           | MFF-- | Form Key or Editor ID                             |                                              |
| Name                           | FULL      | MFF-- | A string value                                    | "Name": "Hello"                              |
| NoiseFalloff                   |           | MFF-- | A decimal value                                   | "NoiseFalloff": 3.14                         |
| NoiseFlowmapScale              |           | MFF-- | A decimal value                                   | "NoiseFlowmapScale": 3.14                    |
| NoiseLayerOneAmplitudeScale    |           | MFF-- | A decimal value                                   | "NoiseLayerOneAmplitudeScale": 3.14          |
| NoiseLayerOneUvScale           |           | MFF-- | A decimal value                                   | "NoiseLayerOneUvScale": 3.14                 |
| NoiseLayerOneWindDirection     |           | MFF-- | A decimal value                                   | "NoiseLayerOneWindDirection": 3.14           |
| NoiseLayerOneWindSpeed         |           | MFF-- | A decimal value                                   | "NoiseLayerOneWindSpeed": 3.14               |
| NoiseLayerThreeAmplitudeScale  |           | MFF-- | A decimal value                                   | "NoiseLayerThreeAmplitudeScale": 3.14        |
| NoiseLayerThreeUvScale         |           | MFF-- | A decimal value                                   | "NoiseLayerThreeUvScale": 3.14               |
| NoiseLayerThreeWindDirection   |           | MFF-- | A decimal value                                   | "NoiseLayerThreeWindDirection": 3.14         |
| NoiseLayerThreeWindSpeed       |           | MFF-- | A decimal value                                   | "NoiseLayerThreeWindSpeed": 3.14             |
| NoiseLayerTwoAmplitudeScale    |           | MFF-- | A decimal value                                   | "NoiseLayerTwoAmplitudeScale": 3.14          |
| NoiseLayerTwoUvScale           |           | MFF-- | A decimal value                                   | "NoiseLayerTwoUvScale": 3.14                 |
| NoiseLayerTwoWindDirection     |           | MFF-- | A decimal value                                   | "NoiseLayerTwoWindDirection": 3.14           |
| NoiseLayerTwoWindSpeed         |           | MFF-- | A decimal value                                   | "NoiseLayerTwoWindSpeed": 3.14               |
| Opacity                        |           | MFF-- | A numeric value                                   | "Opacity": 7                                 |
| OpenSound                      |           | MFF-- | Form Key or Editor ID                             |                                              |
| SpecularBrightness             |           | MFF-- | A decimal value                                   | "SpecularBrightness": 3.14                   |
| SpecularPower                  |           | MFF-- | A decimal value                                   | "SpecularPower": 3.14                        |
| SpecularRadius                 |           | MFF-- | A decimal value                                   | "SpecularRadius": 3.14                       |
| SpecularSunPower               |           | MFF-- | A decimal value                                   | "SpecularSunPower": 3.14                     |
| SpecularSunSparkleMagnitude    |           | MFF-- | A decimal value                                   | "SpecularSunSparkleMagnitude": 3.14          |
| SpecularSunSparklePower        |           | MFF-- | A decimal value                                   | "SpecularSunSparklePower": 3.14              |
| SpecularSunSpecularMagnitude   |           | MFF-- | A decimal value                                   | "SpecularSunSpecularMagnitude": 3.14         |
| Spell                          |           | MFF-- | Form Key or Editor ID                             |                                              |
| WaterFresnel                   |           | MFF-- | A decimal value                                   | "WaterFresnel": 3.14                         |
| WaterReflectionMagnitude       |           | MFF-- | A decimal value                                   | "WaterReflectionMagnitude": 3.14             |
| WaterReflectivity              |           | MFF-- | A decimal value                                   | "WaterReflectivity": 3.14                    |
| WaterRefractionMagnitude       |           | MFF-- | A decimal value                                   | "WaterRefractionMagnitude": 3.14             |

[⬅ Back to Types](Types.md)

## WEAP - Weapon

| Field                  | Alt         | MFFSM | Value Type                                       | Example                       |
| ---------------------- | ----------- | ----- | ------------------------------------------------ | ----------------------------- |
| AlternateBlockMaterial | BAMT        | MFF-- | Form Key or Editor ID                            |                               |
| AttackFailSound        | TNAM        | MFF-- | Form Key or Editor ID                            |                               |
| AttackLoopSound        | NAM7        | MFF-- | Form Key or Editor ID                            |                               |
| AttackSound            | SNAM        | MFF-- | Form Key or Editor ID                            |                               |
| AttackSound2D          | XNAM        | MFF-- | Form Key or Editor ID                            |                               |
| BlockBashImpact        | BIDS        | MFF-- | Form Key or Editor ID                            |                               |
| Description            | DESC        | MFF-- | A string value                                   | "Description": "Hello"        |
| DetectionSoundLevel    |             | MF--- | Possible values (Loud, Normal, Silent, VeryLoud) | "DetectionSoundLevel": "Loud" |
| EnchantmentAmount      | EAMT        | MFF-- | A numeric value                                  | "EnchantmentAmount": 7        |
| EquipmentType          | ETYP        | MFF-- | Form Key or Editor ID                            |                               |
| EquipSound             | NAM9        | MFF-- | Form Key or Editor ID                            |                               |
| FirstPersonModel       |             | MFF-- | Form Key or Editor ID                            |                               |
| IdleSound              | UNAM        | MFF-- | Form Key or Editor ID                            |                               |
| ImpactDataSet          | INAM        | MFF-- | Form Key or Editor ID                            |                               |
| Keywords               | KWDA        | MFFSM | Form Keys or Editor IDs                          |                               |
| MajorFlags             | RecordFlags | MF--- | Flags (NonPlayable)                              | "MajorFlags": "NonPlayable"   |
| Name                   | FULL        | MFF-- | A string value                                   | "Name": "Hello"               |
| ObjectEffect           | EITM        | MFF-- | Form Key or Editor ID                            |                               |
| PickUpSound            | YNAM        | MFF-- | Form Key or Editor ID                            |                               |
| PutDownSound           | ZNAM        | MFF-- | Form Key or Editor ID                            |                               |
| Template               |             | MFF-- | Form Key or Editor ID                            |                               |
| UnequipSound           | NAM8        | MFF-- | Form Key or Editor ID                            |                               |

[⬅ Back to Types](Types.md)

## WTHR - Weather

| Field                 | Alt       | MFFSM | Value Type                                                                                   | Example                                                  |
| --------------------- | --------- | ----- | -------------------------------------------------------------------------------------------- | -------------------------------------------------------- |
| Flags                 | DataFlags | MF--- | Flags (Pleasant, Cloudy, Rainy, Snow, SkyStaticsAlwaysVisible, SkyStaticsFollowsSunPosition) | "Flags": [ "Pleasant", "-SkyStaticsFollowsSunPosition" ] |
| FogDistanceDayFar     |           | MFF-- | A decimal value                                                                              | "FogDistanceDayFar": 3.14                                |
| FogDistanceDayMax     |           | MFF-- | A decimal value                                                                              | "FogDistanceDayMax": 3.14                                |
| FogDistanceDayNear    |           | MFF-- | A decimal value                                                                              | "FogDistanceDayNear": 3.14                               |
| FogDistanceDayPower   |           | MFF-- | A decimal value                                                                              | "FogDistanceDayPower": 3.14                              |
| FogDistanceNightFar   |           | MFF-- | A decimal value                                                                              | "FogDistanceNightFar": 3.14                              |
| FogDistanceNightMax   |           | MFF-- | A decimal value                                                                              | "FogDistanceNightMax": 3.14                              |
| FogDistanceNightNear  |           | MFF-- | A decimal value                                                                              | "FogDistanceNightNear": 3.14                             |
| FogDistanceNightPower |           | MFF-- | A decimal value                                                                              | "FogDistanceNightPower": 3.14                            |
| NAM0DataTypeState     |           | MF--- | Flags (Break0, Break1)                                                                       | "NAM0DataTypeState": [ "Break0", "-Break1" ]             |
| Precipitation         |           | MFF-- | Form Key or Editor ID                                                                        |                                                          |
| SkyStatics            |           | MFFSM | Form Keys or Editor IDs                                                                      |                                                          |
| SunGlareLensFlare     |           | MFF-- | Form Key or Editor ID                                                                        |                                                          |
| TransDelta            |           | MFF-- | A decimal value                                                                              | "TransDelta": 3.14                                       |
| VisualEffect          |           | MFF-- | Form Key or Editor ID                                                                        |                                                          |
| WindDirection         |           | MFF-- | A decimal value                                                                              | "WindDirection": 3.14                                    |
| WindDirectionRange    |           | MFF-- | A decimal value                                                                              | "WindDirectionRange": 3.14                               |

[⬅ Back to Types](Types.md)

## WOOP - WordOfPower

| Field       | Alt  | MFFSM | Value Type     | Example                |
| ----------- | ---- | ----- | -------------- | ---------------------- |
| Name        | FULL | MFF-- | A string value | "Name": "Hello"        |
| Translation |      | MFF-- | A string value | "Translation": "Hello" |

[⬅ Back to Types](Types.md)

## WRLD - Worldspace

| Field                | Alt         | MFFSM | Value Type                                                                                     | Example                               |
| -------------------- | ----------- | ----- | ---------------------------------------------------------------------------------------------- | ------------------------------------- |
| Climate              | CNAM        | MFF-- | Form Key or Editor ID                                                                          |                                       |
| DistantLodMultiplier | NAMA        | MFF-- | A decimal value                                                                                | "DistantLodMultiplier": 3.14          |
| EncounterZone        | XEZN        | MFF-- | Form Key or Editor ID                                                                          |                                       |
| Flags                | DataFlags   | MF--- | Flags (SmallWorld, CannotFastTravel, NoLodWater, NoLandscape, NoSky, FixedDimensions, NoGrass) | "Flags": [ "SmallWorld", "-NoGrass" ] |
| InteriorLighting     | LTMP        | MFF-- | Form Key or Editor ID                                                                          |                                       |
| Location             | XLCN        | MFF-- | Form Key or Editor ID                                                                          |                                       |
| LodWater             | NAM3        | MFF-- | Form Key or Editor ID                                                                          |                                       |
| LodWaterHeight       | NAM4        | MFF-- | A decimal value                                                                                | "LodWaterHeight": 3.14                |
| MajorFlags           | RecordFlags | MF--- | Flags (CanNotWait)                                                                             | "MajorFlags": "CanNotWait"            |
| Music                | ZNAM        | MFF-- | Form Key or Editor ID                                                                          |                                       |
| Name                 | FULL        | MFF-- | A string value                                                                                 | "Name": "Hello"                       |
| Water                | XCWT        | MFF-- | Form Key or Editor ID                                                                          |                                       |
| WorldMapOffsetScale  |             | MFF-- | A decimal value                                                                                | "WorldMapOffsetScale": 3.14           |

[⬅ Back to Types](Types.md)