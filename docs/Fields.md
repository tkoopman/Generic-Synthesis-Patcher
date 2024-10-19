# Implemented Fields - WIP

Fields that are invalid for a record that was matched by your filter will just be ignored.  
You can use either the full field name or Alt name when available.  
**NOTE**: I have not personally tested every field / record combination. When adding new record or field type I did just add any others that would reuse the same code.  

## ALCH - Ingestible

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| Addiction                      |                | Form Key or Editor ID          |                                |
| AddictionChance                |                | A decimal value                | "AddictionChance": 3.14        |
| ConsumeSound                   |                | Form Key or Editor ID          |                                |
| Description                    | DESC           | A string value                 | "Description": "Hello"         |
| Effects                        |                | !!!                            | !!!                            |
| EquipmentType                  | ETYP           | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | Flags (NoAutoCalc, FoodItem, Medicine, Poison) | "Flags": [ "NoAutoCalc", "-Poison" ] |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | Flags (Medicine)               | "MajorFlags": "Medicine"       |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| Value                          |                | A numeric value                | "Value": 7                     |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## AMMO - Ammunition

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| Damage                         | DMG            | A decimal value                | "Damage": 3.14                 |
| Description                    | DESC           | A string value                 | "Description": "Hello"         |
| Flags                          | DataFlags      | Flags (IgnoresNormalWeaponResistance, NonPlayable, NonBolt) | "Flags": [ "IgnoresNormalWeaponResistance", "-NonBolt" ] |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | Flags (NonPlayable)            | "MajorFlags": "NonPlayable"    |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| Projectile                     |                | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| ShortName                      | ONAM           | A string value                 | "ShortName": "Hello"           |
| Value                          |                | A numeric value                | "Value": 7                     |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## ARMO - Armor

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| AlternateBlockMaterial         | BAMT           | Form Key or Editor ID          |                                |
| Armature                       | MODL           | Form Keys or Editor IDs        |                                |
| ArmorRating                    | DNAM           | A decimal value                | "ArmorRating": 3.14            |
| BashImpactDataSet              | BIDS           | Form Key or Editor ID          |                                |
| Description                    | DESC           | A string value                 | "Description": "Hello"         |
| EnchantmentAmount              | EAMT           | A numeric value                | "EnchantmentAmount": 7         |
| EquipmentType                  | ETYP           | Form Key or Editor ID          |                                |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | Flags (NonPlayable, Shield)    | "MajorFlags": [ "NonPlayable", "-Shield" ] |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| ObjectEffect                   | EITM           | Form Key or Editor ID          |                                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| Race                           | RNAM           | Form Key or Editor ID          |                                |
| RagdollConstraintTemplate      | BMCT           | A string value                 | "RagdollConstraintTemplate": "Hello" |
| TemplateArmor                  | TNAM           | Form Key or Editor ID          |                                |
| Value                          |                | A numeric value                | "Value": 7                     |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## BOOK

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| BookText                       | DESC           | A string value                 | "BookText": "Hello"            |
| Description                    | CNAM;DESC      | A string value                 | "Description": "Hello"         |
| Flags                          | DataFlags      | Flags (CantBeTaken)            | "Flags": "CantBeTaken"         |
| InventoryArt                   | INAM           | Form Key or Editor ID          |                                |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| Type                           |                | Possible values (BookOrTome, NoteOrScroll) | "Type": "BookOrTome"           |
| Value                          |                | A numeric value                | "Value": 7                     |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## CELL

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| AcousticSpace                  | XCAS           | Form Key or Editor ID          |                                |
| EncounterZone                  | XEZN           | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | Flags (IsInteriorCell, HasWater, CantTravelFromHere, NoLodWater, PublicArea, HandChanged, ShowSky, UseSkyLighting) | "Flags": [ "IsInteriorCell", "-UseSkyLighting" ] |
| ImageSpace                     | XCIM           | Form Key or Editor ID          |                                |
| LightingTemplate               | LTMP           | Form Key or Editor ID          |                                |
| Location                       | XLCN           | Form Key or Editor ID          |                                |
| LockList                       | XILL           | Form Key or Editor ID          |                                |
| MajorFlags                     | RecordFlags    | Flags (Persistent, OffLimits, CantWait) | "MajorFlags": [ "Persistent", "-CantWait" ] |
| Music                          | ZNAM           | Form Key or Editor ID          |                                |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| Owner                          |                | Form Key or Editor ID          |                                |
| SkyAndWeatherFromRegion        | XCCM           | Form Key or Editor ID          |                                |
| Water                          | XCWT           | Form Key or Editor ID          |                                |
| WaterEnvironmentMap            | XWEM           | A string value                 | "WaterEnvironmentMap": "Hello" |
| WaterHeight                    | XCLW           | A numeric value                | "WaterHeight": 7               |
| WaterNoiseTexture              | XNAM           | A string value                 | "WaterNoiseTexture": "Hello"   |

## CONT - Container

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| CloseSound                     | QNAM           | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | Flags (AllowSoundsWhenAnimation, Respawns, ShowOwner) | "Flags": [ "AllowSoundsWhenAnimation", "-ShowOwner" ] |
| Items                          | Item           | !!!                            | !!!                            |
| MajorFlags                     | RecordFlags    | Flags (HasDistantLOD, RandomAnimStart, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, NavMeshGenerationGround) | "MajorFlags": [ "HasDistantLOD", "-NavMeshGenerationGround" ] |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| OpenSound                      | SNAM           | Form Key or Editor ID          |                                |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## FACT - Faction

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| ExteriorJailMarker             | JAIL           | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | Flags (HiddenFromPC, SpecialCombat, TrackCrime, IgnoreMurder, IgnoreAssault, IgnoreStealing, IgnoreTrespass, DoNotReportCrimesAgainstMembers, CrimeGoldUseDefaults, IgnorePickpocket, Vendor, CanBeOwner, IgnoreWerewolf) | "Flags": [ "HiddenFromPC", "-IgnoreWerewolf" ] |
| FollowerWaitMarker             | WAIT           | Form Key or Editor ID          |                                |
| JailOutfit                     | JOUT           | Form Key or Editor ID          |                                |
| MerchantContainer              | VENC           | Form Key or Editor ID          |                                |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| PlayerInventoryContainer       | PLCN           | Form Key or Editor ID          |                                |
| SharedCrimeFactionList         | CRGR           | Form Key or Editor ID          |                                |
| StolenGoodsContainer           | STOL           | Form Key or Editor ID          |                                |
| VendorBuySellList              | VEND           | Form Key or Editor ID          |                                |

## INGR - Ingredient

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| Effects                        |                | !!!                            | !!!                            |
| EquipType                      | ETYP           | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | Flags (NoAutoCalculation, FoodItem, ReferencesPersist) | "Flags": [ "NoAutoCalculation", "-ReferencesPersist" ] |
| IngredientValue                |                | A numeric value                | "IngredientValue": 7           |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| Value                          |                | A numeric value                | "Value": 7                     |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## KEYM - Key

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | Flags (NonPlayable)            | "MajorFlags": "NonPlayable"    |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| Value                          |                | A numeric value                | "Value": 7                     |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## MISC - MiscItem

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | Flags (NonPlayable)            | "MajorFlags": "NonPlayable"    |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| Value                          |                | A numeric value                | "Value": 7                     |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## NPC

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| ActorEffect                    | SPLO           | Form Keys or Editor IDs        |                                |
| AttackRace                     | ATKR           | Form Key or Editor ID          |                                |
| Class                          | CNAM           | Form Key or Editor ID          |                                |
| CombatOverridePackageList      | ECOR           | Form Key or Editor ID          |                                |
| CombatStyle                    | ZNAM           | Form Key or Editor ID          |                                |
| CrimeFaction                   | CRIF           | Form Key or Editor ID          |                                |
| DeathItem                      | INAM           | Form Key or Editor ID          |                                |
| DefaultOutfit                  | DOFT           | Form Key or Editor ID          |                                |
| DefaultPackageList             | DPLT           | Form Key or Editor ID          |                                |
| FarAwayModel                   | ANAM           | Form Key or Editor ID          |                                |
| GiftFilter                     | GNAM           | Form Key or Editor ID          |                                |
| GuardWarnOverridePackageList   | GWOR           | Form Key or Editor ID          |                                |
| HairColor                      | HCLF           | Form Key or Editor ID          |                                |
| HeadParts                      | PNAM           | Form Keys or Editor IDs        |                                |
| HeadTexture                    | FTST           | Form Key or Editor ID          |                                |
| Height                         | NAM6           | A decimal value                | "Height": 3.14                 |
| Items                          | Item           | !!!                            | !!!                            |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | Flags (BleedoutOverride)       | "MajorFlags": "BleedoutOverride" |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| ObserveDeadBodyOverridePackageList | OCOR           | Form Key or Editor ID          |
|
| Packages                       | PKID           | Form Keys or Editor IDs        |                                |
| Race                           | RNAM           | Form Key or Editor ID          |                                |
| ShortName                      | ONAM           | A string value                 | "ShortName": "Hello"           |
| SleepingOutfit                 | SOFT           | Form Key or Editor ID          |                                |
| SoundLevel                     | NAM8           | Possible values (Loud, Normal, Silent, VeryLoud) | "SoundLevel": "Loud"           |
| SpectatorOverridePackageList   | SPOR           | Form Key or Editor ID          |                                |
| Template                       | TPLT           | Form Key or Editor ID          |                                |
| Voice                          | VTCK           | Form Key or Editor ID          |                                |
| Weight                         | NAM7           | A decimal value                | "Weight": 3.14                 |
| WornArmor                      | WNAM           | Form Key or Editor ID          |                                |

## OTFT - Outfit

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| Items                          | Item           | Form Keys or Editor IDs        |                                |

## SCRL - Scroll

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| BaseCost                       |                | A numeric value                | "BaseCost": 7                  |
| CastDuration                   |                | A numeric value                | "CastDuration": 7              |
| CastType                       |                | Possible values (ConstantEffect, FireAndForget, Concentration) | "CastType": "ConstantEffect"   |
| ChargeTime                     |                | A numeric value                | "ChargeTime": 7                |
| Description                    | DESC           | A string value                 | "Description": "Hello"         |
| Effects                        |                | !!!                            | !!!                            |
| EquipmentType                  | ETYP           | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | Flags (ManualCostCalc, PCStartSpell, AreaEffectIgnoresLOS, IgnoreResistance, NoAbsorbOrReflect, NoDualCastModification) | "Flags": [ "ManualCostCalc", "-NoDualCastModification" ] |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| MenuDisplayObject              | MDOB           | Form Key or Editor ID          |                                |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| Range                          |                | A numeric value                | "Range": 7                     |
| TargetType                     |                | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation) | "TargetType": "Self"           |
| Type                           |                | Possible values (Spell, Disease, Power, LesserPower, Ability, Poison, Addiction, Voice) | "Type": "Spell"                |
| Value                          |                | A numeric value                | "Value": 7                     |
| Weight                         |                | A decimal value                | "Weight": 3.14                 |

## WEAP - Weapon

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| AlternateBlockMaterial         | BAMT           | Form Key or Editor ID          |                                |
| AttackFailSound                | TNAM           | Form Key or Editor ID          |                                |
| AttackLoopSound                | NAM7           | Form Key or Editor ID          |                                |
| AttackSound                    | SNAM           | Form Key or Editor ID          |                                |
| AttackSound2D                  | XNAM           | Form Key or Editor ID          |                                |
| BlockBashImpact                | BIDS           | Form Key or Editor ID          |                                |
| Description                    | DESC           | A string value                 | "Description": "Hello"         |
| EnchantmentAmount              | EAMT           | A numeric value                | "EnchantmentAmount": 7         |
| EquipmentType                  | ETYP           | Form Key or Editor ID          |                                |
| EquipSound                     | NAM9           | Form Key or Editor ID          |                                |
| FirstPersonModel               | WNAM           | Form Key or Editor ID          |                                |
| IdleSound                      | UNAM           | Form Key or Editor ID          |                                |
| ImpactDataSet                  | INAM           | Form Key or Editor ID          |                                |
| Keywords                       | KWDA           | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | Flags (NonPlayable)            | "MajorFlags": "NonPlayable"    |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| ObjectEffect                   | EITM           | Form Key or Editor ID          |                                |
| PickUpSound                    | YNAM           | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | Form Key or Editor ID          |                                |
| UnequipSound                   | NAM8           | Form Key or Editor ID          |                                |

## WRLD - Worldspace

| Field                          | Alt            | Value Type                     | Example                        |
| ------------------------------ | -------------- | ------------------------------ | ------------------------------ |
| Climate                        | CNAM           | Form Key or Editor ID          |                                |
| DistantLodMultiplier           | NAMA           | A decimal value                | "DistantLodMultiplier": 3.14   |
| EncounterZone                  | XEZN           | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | Flags (SmallWorld, CannotFastTravel, NoLodWater, NoLandscape, NoSky, FixedDimensions, NoGrass) | "Flags": [ "SmallWorld", "-NoGrass" ] |
| InteriorLighting               | LTMP           | Form Key or Editor ID          |                                |
| Location                       | XLCN           | Form Key or Editor ID          |                                |
| LodWater                       | NAM3           | Form Key or Editor ID          |                                |
| LodWaterHeight                 | NAM4           | A decimal value                | "LodWaterHeight": 3.14         |
| MajorFlags                     | RecordFlags    | Flags (CanNotWait)             | "MajorFlags": "CanNotWait"     |
| Music                          | ZNAM           | Form Key or Editor ID          |                                |
| Name                           | FULL           | A string value                 | "Name": "Hello"                |
| Water                          | XCWT           | Form Key or Editor ID          |                                |
| WorldMapOffsetScale            |                | A numeric value                | "WorldMapOffsetScale": 7       |