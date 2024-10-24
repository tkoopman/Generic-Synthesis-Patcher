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

## ALCH - Ingestible

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| Addiction                      |                | MFF-- | Form Key or Editor ID          |                                |
| AddictionChance                |                | MFF-- | A decimal value                | "AddictionChance": 3.14        |
| ConsumeSound                   |                | MFF-- | Form Key or Editor ID          |                                |
| Description                    | DESC           | MFF-- | A string value                 | "Description": "Hello"         |
| Effects                        |                | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipmentType                  | ETYP           | MFF-- | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | MF--- | Flags (NoAutoCalc, FoodItem, Medicine, Poison) | "Flags": [ "NoAutoCalc", "-Poison" ] |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | MF--- | Flags (Medicine)               | "MajorFlags": "Medicine"       |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Value                          |                | MFF-- | A numeric value                | "Value": 7                     |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## AMMO - Ammunition

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| Damage                         | DMG            | MFF-- | A decimal value                | "Damage": 3.14                 |
| Description                    | DESC           | MFF-- | A string value                 | "Description": "Hello"         |
| Flags                          | DataFlags      | MF--- | Flags (IgnoresNormalWeaponResistance, NonPlayable, NonBolt) | "Flags": [ "IgnoresNormalWeaponResistance", "-NonBolt" ] |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | MF--- | Flags (NonPlayable)            | "MajorFlags": "NonPlayable"    |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| Projectile                     |                | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| ShortName                      | ONAM           | MFF-- | A string value                 | "ShortName": "Hello"           |
| Value                          |                | MFF-- | A numeric value                | "Value": 7                     |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## ARMO - Armor

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| AlternateBlockMaterial         | BAMT           | MFF-- | Form Key or Editor ID          |                                |
| Armature                       | MODL           | MFFSM | Form Keys or Editor IDs        |                                |
| ArmorRating                    | DNAM           | MFF-- | A decimal value                | "ArmorRating": 3.14            |
| BashImpactDataSet              | BIDS           | MFF-- | Form Key or Editor ID          |                                |
| Description                    | DESC           | MFF-- | A string value                 | "Description": "Hello"         |
| EnchantmentAmount              | EAMT           | MFF-- | A numeric value                | "EnchantmentAmount": 7         |
| EquipmentType                  | ETYP           | MFF-- | Form Key or Editor ID          |                                |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | MF--- | Flags (NonPlayable, Shield)    | "MajorFlags": [ "NonPlayable", "-Shield" ] |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| ObjectEffect                   | EITM           | MFF-- | Form Key or Editor ID          |                                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Race                           | RNAM           | MFF-- | Form Key or Editor ID          |                                |
| RagdollConstraintTemplate      | BMCT           | MFF-- | A string value                 | "RagdollConstraintTemplate": "Hello" |
| TemplateArmor                  | TNAM           | MFF-- | Form Key or Editor ID          |                                |
| Value                          |                | MFF-- | A numeric value                | "Value": 7                     |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## BOOK

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| BookText                       | DESC           | MFF-- | A string value                 | "BookText": "Hello"            |
| Description                    | CNAM;DESC      | MFF-- | A string value                 | "Description": "Hello"         |
| Flags                          | DataFlags      | MF--- | Flags (CantBeTaken)            | "Flags": "CantBeTaken"         |
| InventoryArt                   | INAM           | MFF-- | Form Key or Editor ID          |                                |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Type                           |                | MF--- | Possible values (BookOrTome, NoteOrScroll) | "Type": "BookOrTome"           |
| Value                          |                | MFF-- | A numeric value                | "Value": 7                     |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## CELL

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| AcousticSpace                  | XCAS           | MFF-- | Form Key or Editor ID          |                                |
| EncounterZone                  | XEZN           | MFF-- | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | MF--- | Flags (IsInteriorCell, HasWater, CantTravelFromHere, NoLodWater, PublicArea, HandChanged, ShowSky, UseSkyLighting) | "Flags": [ "IsInteriorCell", "-UseSkyLighting" ] |
| ImageSpace                     | XCIM           | MFF-- | Form Key or Editor ID          |                                |
| LightingTemplate               | LTMP           | MFF-- | Form Key or Editor ID          |                                |
| Location                       | XLCN           | MFF-- | Form Key or Editor ID          |                                |
| LockList                       | XILL           | MFF-- | Form Key or Editor ID          |                                |
| MajorFlags                     | RecordFlags    | MF--- | Flags (Persistent, OffLimits, CantWait) | "MajorFlags": [ "Persistent", "-CantWait" ] |
| Music                          | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| Owner                          |                | MFF-- | Form Key or Editor ID          |                                |
| SkyAndWeatherFromRegion        | XCCM           | MFF-- | Form Key or Editor ID          |                                |
| Water                          | XCWT           | MFF-- | Form Key or Editor ID          |                                |
| WaterEnvironmentMap            | XWEM           | MFF-- | A string value                 | "WaterEnvironmentMap": "Hello" |
| WaterHeight                    | XCLW           | MFF-- | A numeric value                | "WaterHeight": 7               |
| WaterNoiseTexture              | XNAM           | MFF-- | A string value                 | "WaterNoiseTexture": "Hello"   |

## CONT - Container

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| CloseSound                     | QNAM           | MFF-- | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | MF--- | Flags (AllowSoundsWhenAnimation, Respawns, ShowOwner) | "Flags": [ "AllowSoundsWhenAnimation", "-ShowOwner" ] |
| Items                          | Item           | MFFSM | JSON objects containing item Form Key/Editor ID and Count (QTY) | "Items": { "Item": "021FED:Skyrim.esm", "Count": 3 } |
| MajorFlags                     | RecordFlags    | MF--- | Flags (HasDistantLOD, RandomAnimStart, Obstacle, NavMeshGenerationFilter, NavMeshGenerationBoundingBox, NavMeshGenerationGround) | "MajorFlags": [ "HasDistantLOD", "-NavMeshGenerationGround" ] |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| OpenSound                      | SNAM           | MFF-- | Form Key or Editor ID          |                                |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## FACT - Faction

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| ExteriorJailMarker             | JAIL           | MFF-- | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | MF--- | Flags (HiddenFromPC, SpecialCombat, TrackCrime, IgnoreMurder, IgnoreAssault, IgnoreStealing, IgnoreTrespass, DoNotReportCrimesAgainstMembers, CrimeGoldUseDefaults, IgnorePickpocket, Vendor, CanBeOwner, IgnoreWerewolf) | "Flags": [ "HiddenFromPC", "-IgnoreWerewolf" ] |
| FollowerWaitMarker             | WAIT           | MFF-- | Form Key or Editor ID          |                                |
| JailOutfit                     | JOUT           | MFF-- | Form Key or Editor ID          |                                |
| MerchantContainer              | VENC           | MFF-- | Form Key or Editor ID          |                                |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| PlayerInventoryContainer       | PLCN           | MFF-- | Form Key or Editor ID          |                                |
| SharedCrimeFactionList         | CRGR           | MFF-- | Form Key or Editor ID          |                                |
| StolenGoodsContainer           | STOL           | MFF-- | Form Key or Editor ID          |                                |
| VendorBuySellList              | VEND           | MFF-- | Form Key or Editor ID          |                                |

## INGR - Ingredient

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| Effects                        |                | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipType                      | ETYP           | MFF-- | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | MF--- | Flags (NoAutoCalculation, FoodItem, ReferencesPersist) | "Flags": [ "NoAutoCalculation", "-ReferencesPersist" ] |
| IngredientValue                |                | MFF-- | A numeric value                | "IngredientValue": 7           |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Value                          |                | MFF-- | A numeric value                | "Value": 7                     |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## KEYM - Key

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | MF--- | Flags (NonPlayable)            | "MajorFlags": "NonPlayable"    |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Value                          |                | MFF-- | A numeric value                | "Value": 7                     |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## MISC - MiscItem

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | MF--- | Flags (NonPlayable)            | "MajorFlags": "NonPlayable"    |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Value                          |                | MFF-- | A numeric value                | "Value": 7                     |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## NPC

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| ActorEffect                    | SPLO           | MFFSM | Form Keys or Editor IDs        |                                |
| AttackRace                     | ATKR           | MFF-- | Form Key or Editor ID          |                                |
| Class                          | CNAM           | MFF-- | Form Key or Editor ID          |                                |
| CombatOverridePackageList      | ECOR           | MFF-- | Form Key or Editor ID          |                                |
| CombatStyle                    | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| CrimeFaction                   | CRIF           | MFF-- | Form Key or Editor ID          |                                |
| DeathItem                      | INAM           | MFF-- | Form Key or Editor ID          |                                |
| DefaultOutfit                  | DOFT           | MFF-- | Form Key or Editor ID          |                                |
| DefaultPackageList             | DPLT           | MFF-- | Form Key or Editor ID          |                                |
| FarAwayModel                   | ANAM           | MFF-- | Form Key or Editor ID          |                                |
| GiftFilter                     | GNAM           | MFF-- | Form Key or Editor ID          |                                |
| GuardWarnOverridePackageList   | GWOR           | MFF-- | Form Key or Editor ID          |                                |
| HairColor                      | HCLF           | MFF-- | Form Key or Editor ID          |                                |
| HeadParts                      | PNAM           | MFFSM | Form Keys or Editor IDs        |                                |
| HeadTexture                    | FTST           | MFF-- | Form Key or Editor ID          |                                |
| Height                         | NAM6           | MFF-- | A decimal value                | "Height": 3.14                 |
| Items                          | Item           | MFFSM | JSON objects containing item Form Key/Editor ID and Count (QTY) | "Items": { "Item": "021FED:Skyrim.esm", "Count": 3 } |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | MF--- | Flags (BleedoutOverride)       | "MajorFlags": "BleedoutOverride" |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| ObserveDeadBodyOverridePackageList | OCOR           | MFF-- | Form Key or Editor ID          |                                |
| Packages                       | PKID           | MFFSM | Form Keys or Editor IDs        |                                |
| Race                           | RNAM           | MFF-- | Form Key or Editor ID          |                                |
| ShortName                      | ONAM           | MFF-- | A string value                 | "ShortName": "Hello"           |
| SleepingOutfit                 | SOFT           | MFF-- | Form Key or Editor ID          |                                |
| SoundLevel                     | NAM8           | MF--- | Possible values (Loud, Normal, Silent, VeryLoud) | "SoundLevel": "Loud"           |
| SpectatorOverridePackageList   | SPOR           | MFF-- | Form Key or Editor ID          |                                |
| Template                       | TPLT           | MFF-- | Form Key or Editor ID          |                                |
| Voice                          | VTCK           | MFF-- | Form Key or Editor ID          |                                |
| Weight                         | NAM7           | MFF-- | A decimal value                | "Weight": 3.14                 |
| WornArmor                      | WNAM           | MFF-- | Form Key or Editor ID          |                                |

## OTFT - Outfit

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| Items                          | Item           | MFFSM | Form Keys or Editor IDs        |                                |

## SCRL - Scroll

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| BaseCost                       |                | MFF-- | A numeric value                | "BaseCost": 7                  |
| CastDuration                   |                | MFF-- | A numeric value                | "CastDuration": 7              |
| CastType                       |                | MF--- | Possible values (ConstantEffect, FireAndForget, Concentration) | "CastType": "ConstantEffect"   |
| ChargeTime                     |                | MFF-- | A numeric value                | "ChargeTime": 7                |
| Description                    | DESC           | MFF-- | A string value                 | "Description": "Hello"         |
| Effects                        |                | MFFSM | JSON objects containing effect Form Key/Editor ID and effect data | "Effects": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 } |
| EquipmentType                  | ETYP           | MFF-- | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | MF--- | Flags (ManualCostCalc, PCStartSpell, AreaEffectIgnoresLOS, IgnoreResistance, NoAbsorbOrReflect, NoDualCastModification) | "Flags": [ "ManualCostCalc", "-NoDualCastModification" ] |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| MenuDisplayObject              | MDOB           | MFF-- | Form Key or Editor ID          |                                |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Range                          |                | MFF-- | A numeric value                | "Range": 7                     |
| TargetType                     |                | MF--- | Possible values (Self, Touch, Aimed, TargetActor, TargetLocation) | "TargetType": "Self"           |
| Type                           |                | MF--- | Possible values (Spell, Disease, Power, LesserPower, Ability, Poison, Addiction, Voice) | "Type": "Spell"                |
| Value                          |                | MFF-- | A numeric value                | "Value": 7                     |
| Weight                         |                | MFF-- | A decimal value                | "Weight": 3.14                 |

## WEAP - Weapon

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| AlternateBlockMaterial         | BAMT           | MFF-- | Form Key or Editor ID          |                                |
| AttackFailSound                | TNAM           | MFF-- | Form Key or Editor ID          |                                |
| AttackLoopSound                | NAM7           | MFF-- | Form Key or Editor ID          |                                |
| AttackSound                    | SNAM           | MFF-- | Form Key or Editor ID          |                                |
| AttackSound2D                  | XNAM           | MFF-- | Form Key or Editor ID          |                                |
| BlockBashImpact                | BIDS           | MFF-- | Form Key or Editor ID          |                                |
| Description                    | DESC           | MFF-- | A string value                 | "Description": "Hello"         |
| EnchantmentAmount              | EAMT           | MFF-- | A numeric value                | "EnchantmentAmount": 7         |
| EquipmentType                  | ETYP           | MFF-- | Form Key or Editor ID          |                                |
| EquipSound                     | NAM9           | MFF-- | Form Key or Editor ID          |                                |
| FirstPersonModel               | WNAM           | MFF-- | Form Key or Editor ID          |                                |
| IdleSound                      | UNAM           | MFF-- | Form Key or Editor ID          |                                |
| ImpactDataSet                  | INAM           | MFF-- | Form Key or Editor ID          |                                |
| Keywords                       | KWDA           | MFFSM | Form Keys or Editor IDs        |                                |
| MajorFlags                     | RecordFlags    | MF--- | Flags (NonPlayable)            | "MajorFlags": "NonPlayable"    |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| ObjectEffect                   | EITM           | MFF-- | Form Key or Editor ID          |                                |
| PickUpSound                    | YNAM           | MFF-- | Form Key or Editor ID          |                                |
| PutDownSound                   | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| UnequipSound                   | NAM8           | MFF-- | Form Key or Editor ID          |                                |

## WRLD - Worldspace

| Field                          | Alt            | MFFSM | Value Type                     | Example                        |
| ------------------------------ | -------------- | ----- | ------------------------------ | ------------------------------ |
| Climate                        | CNAM           | MFF-- | Form Key or Editor ID          |                                |
| DistantLodMultiplier           | NAMA           | MFF-- | A decimal value                | "DistantLodMultiplier": 3.14   |
| EncounterZone                  | XEZN           | MFF-- | Form Key or Editor ID          |                                |
| Flags                          | DataFlags      | MF--- | Flags (SmallWorld, CannotFastTravel, NoLodWater, NoLandscape, NoSky, FixedDimensions, NoGrass) | "Flags": [ "SmallWorld", "-NoGrass" ] |
| InteriorLighting               | LTMP           | MFF-- | Form Key or Editor ID          |                                |
| Location                       | XLCN           | MFF-- | Form Key or Editor ID          |                                |
| LodWater                       | NAM3           | MFF-- | Form Key or Editor ID          |                                |
| LodWaterHeight                 | NAM4           | MFF-- | A decimal value                | "LodWaterHeight": 3.14         |
| MajorFlags                     | RecordFlags    | MF--- | Flags (CanNotWait)             | "MajorFlags": "CanNotWait"     |
| Music                          | ZNAM           | MFF-- | Form Key or Editor ID          |                                |
| Name                           | FULL           | MFF-- | A string value                 | "Name": "Hello"                |
| Water                          | XCWT           | MFF-- | Form Key or Editor ID          |                                |
| WorldMapOffsetScale            |                | MFF-- | A numeric value                | "WorldMapOffsetScale": 7       |