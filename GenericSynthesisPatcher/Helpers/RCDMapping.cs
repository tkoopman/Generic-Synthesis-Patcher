using GenericSynthesisPatcher.Helpers.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers
{
    public class RCDMapping
    {
        // This just to make sorting mappings easier by placing null after typeof alphabetically
        private const Type? zzNull = null;

        private static readonly RecordCallData RcdAcousticSpace = new RecordCallData<FormLink<IAcousticSpaceGetter>>("AcousticSpace");
        private static readonly RecordCallData RcdActorEffect = new RecordCallData<FormLinks<ISpellRecordGetter>>("ActorEffect");
        private static readonly RecordCallData RcdAddiction = new RecordCallData<FormLink<ISkyrimMajorRecordGetter>>("Addiction");
        private static readonly RecordCallData RcdAddictionChance = new RecordCallData<Generic<float>>("AddictionChance");
        private static readonly RecordCallData RcdAlternateBlockMaterial = new RecordCallData<FormLink<IMaterialTypeGetter>>("AlternateBlockMaterial");
        private static readonly RecordCallData RcdArmature = new RecordCallData<FormLinks<IArmorAddonGetter>>("Armature");
        private static readonly RecordCallData RcdArmorRating = new RecordCallData<Generic<float>>("ArmorRating");
        private static readonly RecordCallData RcdAttackFailSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("AttackFailSound");
        private static readonly RecordCallData RcdAttackLoopSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("AttackLoopSound");
        private static readonly RecordCallData RcdAttackRace = new RecordCallData<FormLink<IRaceGetter>>("AttackRace");
        private static readonly RecordCallData RcdAttackSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("AttackSound");
        private static readonly RecordCallData RcdAttackSound2D = new RecordCallData<FormLink<ISoundDescriptorGetter>>("AttackSound2D");
        private static readonly RecordCallData RcdBaseCost = new RecordCallData<Generic<uint>>("BaseCost");
        private static readonly RecordCallData RcdBashImpactDataSet = new RecordCallData<FormLink<IImpactDataSetGetter>>("BashImpactDataSet");
        private static readonly RecordCallData RcdBlockBashImpact = new RecordCallData<FormLink<IImpactDataSetGetter>>("BlockBashImpact");
        private static readonly RecordCallData RcdBookText = new RecordCallData<Generic<string>>("BookText");
        private static readonly RecordCallData RcdCastDuration = new RecordCallData<Generic<ushort>>("CastDuration");
        private static readonly RecordCallData RcdCastType = new RecordCallData<Enums>("CastType");
        private static readonly RecordCallData RcdChargeTime = new RecordCallData<Generic<ushort>>("ChargeTime");
        private static readonly RecordCallData RcdClass = new RecordCallData<FormLink<IClassGetter>>("Class");
        private static readonly RecordCallData RcdClimate = new RecordCallData<FormLink<IClimateGetter>>("Climate");
        private static readonly RecordCallData RcdCloseSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("CloseSound");
        private static readonly RecordCallData RcdCombatOverridePackageList = new RecordCallData<FormLink<IFormListGetter>>("CombatOverridePackageList");
        private static readonly RecordCallData RcdCombatStyle = new RecordCallData<FormLink<ICombatStyleGetter>>("CombatStyle");
        private static readonly RecordCallData RcdConsumeSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("ConsumeSound");
        private static readonly RecordCallData RcdContainerItems = new RecordCallData<FormLinksWithData<ContainerItemsAction, IItemGetter>>("Items");
        private static readonly RecordCallData RcdCrimeFaction = new RecordCallData<FormLink<IFactionGetter>>("CrimeFaction");
        private static readonly RecordCallData RcdDamage = new RecordCallData<Generic<float>>("Damage");
        private static readonly RecordCallData RcdDeathItem = new RecordCallData<FormLink<ILeveledItemGetter>>("DeathItem");
        private static readonly RecordCallData RcdDefaultOutfit = new RecordCallData<FormLink<IOutfitGetter>>("DefaultOutfit");
        private static readonly RecordCallData RcdDefaultPackageList = new RecordCallData<FormLink<IFormListGetter>>("DefaultPackageList");
        private static readonly RecordCallData RcdDescription = new RecordCallData<Generic<string>>("Description");
        private static readonly RecordCallData RcdDistantLodMultiplier = new RecordCallData<Generic<float>>("DistantLodMultiplier");
        private static readonly RecordCallData RcdEffects = new RecordCallData<FormLinksWithData<EffectsAction, IMagicEffectGetter>>("Effects");
        private static readonly RecordCallData RcdEnchantmentAmount = new RecordCallData<Generic<ushort>>("EnchantmentAmount");
        private static readonly RecordCallData RcdEncounterZone = new RecordCallData<FormLink<IEncounterZoneGetter>>("EncounterZone");
        private static readonly RecordCallData RcdEquipmentType = new RecordCallData<FormLink<IEquipTypeGetter>>("EquipmentType");
        private static readonly RecordCallData RcdEquipSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("EquipSound");
        private static readonly RecordCallData RcdEquipType = new RecordCallData<FormLink<IEquipTypeGetter>>("EquipType");
        private static readonly RecordCallData RcdExteriorJailMarker = new RecordCallData<FormLink<IPlacedObjectGetter>>("ExteriorJailMarker");
        private static readonly RecordCallData RcdFarAwayModel = new RecordCallData<FormLink<IArmorGetter>>("FarAwayModel");
        private static readonly RecordCallData RcdFirstPersonModel = new RecordCallData<FormLink<IStaticGetter>>("FirstPersonModel");
        private static readonly RecordCallData RcdFlags = new RecordCallData<Flags>("Flags");
        private static readonly RecordCallData RcdFollowerWaitMarker = new RecordCallData<FormLink<IPlacedObjectGetter>>("FollowerWaitMarker");
        private static readonly RecordCallData RcdGiftFilter = new RecordCallData<FormLink<IFormListGetter>>("GiftFilter");
        private static readonly RecordCallData RcdGuardWarnOverridePackageList = new RecordCallData<FormLink<IFormListGetter>>("GuardWarnOverridePackageList");
        private static readonly RecordCallData RcdHairColor = new RecordCallData<FormLink<IColorRecordGetter>>("HairColor");
        private static readonly RecordCallData RcdHeadParts = new RecordCallData<FormLinks<IHeadPartGetter>>("HeadParts");
        private static readonly RecordCallData RcdHeadTexture = new RecordCallData<FormLink<ITextureSetGetter>>("HeadTexture");
        private static readonly RecordCallData RcdHeight = new RecordCallData<Generic<float>>("Height");
        private static readonly RecordCallData RcdIdleSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("IdleSound");
        private static readonly RecordCallData RcdImageSpace = new RecordCallData<FormLink<IImageSpaceGetter>>("ImageSpace");
        private static readonly RecordCallData RcdImpactDataSet = new RecordCallData<FormLink<IImpactDataSetGetter>>("ImpactDataSet");
        private static readonly RecordCallData RcdIngredientValue = new RecordCallData<Generic<int>>("IngredientValue");
        private static readonly RecordCallData RcdInteriorLighting = new RecordCallData<FormLink<ILightingTemplateGetter>>("InteriorLighting");
        private static readonly RecordCallData RcdInventoryArt = new RecordCallData<FormLink<IStaticGetter>>("InventoryArt");
        private static readonly RecordCallData RcdItems = new RecordCallData<FormLinks<IOutfitTargetGetter>>("Items");
        private static readonly RecordCallData RcdJailOutfit = new RecordCallData<FormLink<IOutfitGetter>>("JailOutfit");
        private static readonly RecordCallData RcdKeywords = new RecordCallData<FormLinks<IKeywordGetter>>("Keywords");
        private static readonly RecordCallData RcdLightingTemplate = new RecordCallData<FormLink<ILightingTemplateGetter>>("LightingTemplate");
        private static readonly RecordCallData RcdLocation = new RecordCallData<FormLink<ILocationGetter>>("Location");
        private static readonly RecordCallData RcdLockList = new RecordCallData<FormLink<ILockListGetter>>("LockList");
        private static readonly RecordCallData RcdLodWater = new RecordCallData<FormLink<IWaterGetter>>("LodWater");
        private static readonly RecordCallData RcdLodWaterHeight = new RecordCallData<Generic<float>>("LodWaterHeight");
        private static readonly RecordCallData RcdMajorFlags = new RecordCallData<Flags>("MajorFlags");
        private static readonly RecordCallData RcdMenuDisplayObject = new RecordCallData<FormLink<IStaticGetter>>("MenuDisplayObject");
        private static readonly RecordCallData RcdMerchantContainer = new RecordCallData<FormLink<IPlacedObjectGetter>>("MerchantContainer");
        private static readonly RecordCallData RcdMusic = new RecordCallData<FormLink<IMusicTypeGetter>>("Music");
        private static readonly RecordCallData RcdName = new RecordCallData<Generic<string>>("Name");
        private static readonly RecordCallData RcdObjectEffect = new RecordCallData<FormLink<IEffectRecordGetter>>("ObjectEffect");
        private static readonly RecordCallData RcdObserveDeadBodyOverridePackageList = new RecordCallData<FormLink<IFormListGetter>>("ObserveDeadBodyOverridePackageList");
        private static readonly RecordCallData RcdOpenSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("OpenSound");
        private static readonly RecordCallData RcdOwner = new RecordCallData<FormLink<IOwnerGetter>>("Owner");
        private static readonly RecordCallData RcdPackages = new RecordCallData<FormLinks<IPackageGetter>>("Packages");
        private static readonly RecordCallData RcdPickUpSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("PickUpSound");
        private static readonly RecordCallData RcdPlayerInventoryContainer = new RecordCallData<FormLink<IPlacedObjectGetter>>("PlayerInventoryContainer");
        private static readonly RecordCallData RcdProjectile = new RecordCallData<FormLink<IProjectileGetter>>("Projectile");
        private static readonly RecordCallData RcdPutDownSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("PutDownSound");
        private static readonly RecordCallData RcdRace = new RecordCallData<FormLink<IRaceGetter>>("Race");
        private static readonly RecordCallData RcdRagdollConstraintTemplate = new RecordCallData<Generic<string>>("RagdollConstraintTemplate");
        private static readonly RecordCallData RcdRange = new RecordCallData<Generic<ushort>>("Range");
        private static readonly RecordCallData RcdSharedCrimeFactionList = new RecordCallData<FormLink<IFormListGetter>>("SharedCrimeFactionList");
        private static readonly RecordCallData RcdShortName = new RecordCallData<Generic<string>>("ShortName");
        private static readonly RecordCallData RcdSkyAndWeatherFromRegion = new RecordCallData<FormLink<IRegionGetter>>("SkyAndWeatherFromRegion");
        private static readonly RecordCallData RcdSleepingOutfit = new RecordCallData<FormLink<IOutfitGetter>>("SleepingOutfit");
        private static readonly RecordCallData RcdSoundLevel = new RecordCallData<Enums>("SoundLevel");
        private static readonly RecordCallData RcdSpectatorOverridePackageList = new RecordCallData<FormLink<IFormListGetter>>("SpectatorOverridePackageList");
        private static readonly RecordCallData RcdStolenGoodsContainer = new RecordCallData<FormLink<IPlacedObjectGetter>>("StolenGoodsContainer");
        private static readonly RecordCallData RcdTargetType = new RecordCallData<Enums>("TargetType");
        private static readonly RecordCallData RcdTemplate = new RecordCallData<FormLink<INpcSpawnGetter>>("Template");
        private static readonly RecordCallData RcdTemplateArmor = new RecordCallData<FormLink<IArmorGetter>>("TemplateArmor");
        private static readonly RecordCallData RcdType = new RecordCallData<Enums>("Type");
        private static readonly RecordCallData RcdUnequipSound = new RecordCallData<FormLink<ISoundDescriptorGetter>>("UnequipSound");
        private static readonly RecordCallData RcdValue = new RecordCallData<Generic<uint>>("Value");
        private static readonly RecordCallData RcdVendorBuySellList = new RecordCallData<FormLink<IFormListGetter>>("VendorBuySellList");
        private static readonly RecordCallData RcdVoice = new RecordCallData<FormLink<IVoiceTypeGetter>>("Voice");
        private static readonly RecordCallData RcdWater = new RecordCallData<FormLink<IWaterGetter>>("Water");
        private static readonly RecordCallData RcdWaterEnvironmentMap = new RecordCallData<Generic<string>>("WaterEnvironmentMap");
        private static readonly RecordCallData RcdWaterHeight = new RecordCallData<Generic<ushort>>("WaterHeight");
        private static readonly RecordCallData RcdWaterNoiseTexture = new RecordCallData<Generic<string>>("WaterNoiseTexture");
        private static readonly RecordCallData RcdWeight = new RecordCallData<Generic<float>>("Weight");
        private static readonly RecordCallData RcdWorldMapOffsetScale = new RecordCallData<Generic<ushort>>("WorldMapOffsetScale");
        private static readonly RecordCallData RcdWornArmor = new RecordCallData<FormLink<IArmorGetter>>("WornArmor");

        public static RecordCallData? FindRecordCallData ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string valueKey )
        {
            var rcd = RecordCallDataMapping.FirstOrDefault(r => r.Key.JsonKey.Equals(valueKey, StringComparison.OrdinalIgnoreCase)
                                                            && (r.Key.RecordType == null || r.Key.RecordType.IsAssignableFrom(context.Record.GetType()))).Value;

            if (rcd == null)
                LogHelper.Log(LogLevel.Trace, context, $"No RCD found - {valueKey}", 0xF41);

            return rcd;
        }

        #region RCD Mapping

        /// <summary>
        /// This list is what directs each field update to the correct function.
        /// List is sorted first by JSON key, then by record type, making sure null comes after any defined types.
        /// Having multiple entries with the exact same RecordCallKey will ignore all except the first one found in the list.
        /// </summary>
        public static readonly SortedList<RecordCallKey, RecordCallData> RecordCallDataMapping = new()
        {
                { new RecordCallKey("AcousticSpace"                      , typeof(ICellGetter)          ) , RcdAcousticSpace                      },
                { new RecordCallKey("ActorEffect"                        , typeof(INpcGetter)           ) , RcdActorEffect                        },
                { new RecordCallKey("Addiction"                          , typeof(IIngestibleGetter)    ) , RcdAddiction                          },
                { new RecordCallKey("AddictionChance"                    , typeof(IIngestibleGetter)    ) , RcdAddictionChance                    },
                { new RecordCallKey("AlternateBlockMaterial"             , zzNull                       ) , RcdAlternateBlockMaterial             },
                { new RecordCallKey("ANAM"                               , typeof(INpcGetter)           ) , RcdFarAwayModel                       },
                { new RecordCallKey("Armature"                           , typeof(IArmorGetter)         ) , RcdArmature                           },
                { new RecordCallKey("ArmorRating"                        , typeof(IArmorGetter)         ) , RcdArmorRating                        },
                { new RecordCallKey("ATKR"                               , typeof(INpcGetter)           ) , RcdAttackRace                         },
                { new RecordCallKey("AttackFailSound"                    , typeof(IWeaponGetter)        ) , RcdAttackFailSound                    },
                { new RecordCallKey("AttackLoopSound"                    , typeof(IWeaponGetter)        ) , RcdAttackLoopSound                    },
                { new RecordCallKey("AttackRace"                         , typeof(INpcGetter)           ) , RcdAttackRace                         },
                { new RecordCallKey("AttackSound"                        , typeof(IWeaponGetter)        ) , RcdAttackSound                        },
                { new RecordCallKey("AttackSound2D"                      , typeof(IWeaponGetter)        ) , RcdAttackSound2D                      },
                { new RecordCallKey("BAMT"                               , zzNull                       ) , RcdAlternateBlockMaterial             },
                { new RecordCallKey("BaseCost"                           , typeof(IScrollGetter)        ) , RcdBaseCost                           },
                { new RecordCallKey("BashImpactDataSet"                  , typeof(IArmorGetter)         ) , RcdBashImpactDataSet                  },
                { new RecordCallKey("BIDS"                               , typeof(IArmorGetter)         ) , RcdBashImpactDataSet                  },
                { new RecordCallKey("BIDS"                               , typeof(IWeaponGetter)        ) , RcdBlockBashImpact                    },
                { new RecordCallKey("BlockBashImpact"                    , typeof(IWeaponGetter)        ) , RcdBlockBashImpact                    },
                { new RecordCallKey("BMCT"                               , typeof(IArmorGetter)         ) , RcdRagdollConstraintTemplate          },
                { new RecordCallKey("BookText"                           , typeof(IBookGetter)          ) , RcdBookText                           },
                { new RecordCallKey("CastDuration"                       , typeof(IScrollGetter)        ) , RcdCastDuration                       },
                { new RecordCallKey("CastType"                           , typeof(IScrollGetter)        ) , RcdCastType                           },
                { new RecordCallKey("ChargeTime"                         , typeof(IScrollGetter)        ) , RcdChargeTime                         },
                { new RecordCallKey("Class"                              , typeof(INpcGetter)           ) , RcdClass                              },
                { new RecordCallKey("Climate"                            , typeof(IWorldspaceGetter)    ) , RcdClimate                            },
                { new RecordCallKey("CloseSound"                         , typeof(IContainerGetter)     ) , RcdCloseSound                         },
                { new RecordCallKey("CNAM"                               , typeof(IBookGetter)          ) , RcdDescription                        },
                { new RecordCallKey("CNAM"                               , typeof(INpcGetter)           ) , RcdClass                              },
                { new RecordCallKey("CNAM"                               , typeof(IWorldspaceGetter)    ) , RcdClimate                            },
                { new RecordCallKey("CombatOverridePackageList"          , typeof(INpcGetter)           ) , RcdCombatOverridePackageList          },
                { new RecordCallKey("CombatStyle"                        , typeof(INpcGetter)           ) , RcdCombatStyle                        },
                { new RecordCallKey("ConsumeSound"                       , typeof(IIngestibleGetter)    ) , RcdConsumeSound                       },
                { new RecordCallKey("CRGR"                               , typeof(IFactionGetter)       ) , RcdSharedCrimeFactionList             },
                { new RecordCallKey("CRIF"                               , typeof(INpcGetter)           ) , RcdCrimeFaction                       },
                { new RecordCallKey("CrimeFaction"                       , typeof(INpcGetter)           ) , RcdCrimeFaction                       },
                { new RecordCallKey("Damage"                             , typeof(IAmmunitionGetter)    ) , RcdDamage                             },
                { new RecordCallKey("DataFlags"                          , zzNull                       ) , RcdFlags                              },
                { new RecordCallKey("DeathItem"                          , typeof(INpcGetter)           ) , RcdDeathItem                          },
                { new RecordCallKey("DefaultOutfit"                      , typeof(INpcGetter)           ) , RcdDefaultOutfit                      },
                { new RecordCallKey("DefaultPackageList"                 , typeof(INpcGetter)           ) , RcdDefaultPackageList                 },
                { new RecordCallKey("DESC"                               , typeof(IBookGetter)          ) , RcdBookText                           },
                { new RecordCallKey("DESC"                               , zzNull                       ) , RcdDescription                        },
                { new RecordCallKey("Description"                        , zzNull                       ) , RcdDescription                        },
                { new RecordCallKey("DistantLodMultiplier"               , typeof(IWorldspaceGetter)    ) , RcdDistantLodMultiplier               },
                { new RecordCallKey("DMG"                                , typeof(IAmmunitionGetter)    ) , RcdDamage                             },
                { new RecordCallKey("DNAM"                               , typeof(IArmorGetter)         ) , RcdArmorRating                        },
                { new RecordCallKey("DOFT"                               , typeof(INpcGetter)           ) , RcdDefaultOutfit                      },
                { new RecordCallKey("DPLT"                               , typeof(INpcGetter)           ) , RcdDefaultPackageList                 },
                { new RecordCallKey("EAMT"                               , zzNull                       ) , RcdEnchantmentAmount                  },
                { new RecordCallKey("ECOR"                               , typeof(INpcGetter)           ) , RcdCombatOverridePackageList          },
                { new RecordCallKey("Effects"                            , typeof(IIngestibleGetter)    ) , RcdEffects                            },
                { new RecordCallKey("Effects"                            , typeof(IIngredientGetter)    ) , RcdEffects                            },
                { new RecordCallKey("Effects"                            , typeof(IScrollGetter)        ) , RcdEffects                            },
                { new RecordCallKey("EITM"                               , zzNull                       ) , RcdObjectEffect                       },
                { new RecordCallKey("EnchantmentAmount"                  , zzNull                       ) , RcdEnchantmentAmount                  },
                { new RecordCallKey("EncounterZone"                      , zzNull                       ) , RcdEncounterZone                      },
                { new RecordCallKey("EquipmentType"                      , zzNull                       ) , RcdEquipmentType                      },
                { new RecordCallKey("EquipSound"                         , typeof(IWeaponGetter)        ) , RcdEquipSound                         },
                { new RecordCallKey("EquipType"                          , typeof(IIngredientGetter)    ) , RcdEquipType                          },
                { new RecordCallKey("ETYP"                               , typeof(IIngredientGetter)    ) , RcdEquipType                          },
                { new RecordCallKey("ETYP"                               , zzNull                       ) , RcdEquipmentType                      },
                { new RecordCallKey("ExteriorJailMarker"                 , typeof(IFactionGetter)       ) , RcdExteriorJailMarker                 },
                { new RecordCallKey("FarAwayModel"                       , typeof(INpcGetter)           ) , RcdFarAwayModel                       },
                { new RecordCallKey("FirstPersonModel"                   , typeof(IWeaponGetter)        ) , RcdFirstPersonModel                   },
                { new RecordCallKey("Flags"                              , zzNull                       ) , RcdFlags                              },
                { new RecordCallKey("FollowerWaitMarker"                 , typeof(IFactionGetter)       ) , RcdFollowerWaitMarker                 },
                { new RecordCallKey("FTST"                               , typeof(INpcGetter)           ) , RcdHeadTexture                        },
                { new RecordCallKey("FULL"                               , typeof(INamedGetter)         ) , RcdName                               },
                { new RecordCallKey("FULL"                               , typeof(INamedRequiredGetter) ) , RcdName                               },
                { new RecordCallKey("GiftFilter"                         , typeof(INpcGetter)           ) , RcdGiftFilter                         },
                { new RecordCallKey("GNAM"                               , typeof(INpcGetter)           ) , RcdGiftFilter                         },
                { new RecordCallKey("GuardWarnOverridePackageList"       , typeof(INpcGetter)           ) , RcdGuardWarnOverridePackageList       },
                { new RecordCallKey("GWOR"                               , typeof(INpcGetter)           ) , RcdGuardWarnOverridePackageList       },
                { new RecordCallKey("HairColor"                          , typeof(INpcGetter)           ) , RcdHairColor                          },
                { new RecordCallKey("HCLF"                               , typeof(INpcGetter)           ) , RcdHairColor                          },
                { new RecordCallKey("HeadParts"                          , typeof(INpcGetter)           ) , RcdHeadParts                          },
                { new RecordCallKey("HeadTexture"                        , typeof(INpcGetter)           ) , RcdHeadTexture                        },
                { new RecordCallKey("Height"                             , typeof(INpcGetter)           ) , RcdHeight                             },
                { new RecordCallKey("IdleSound"                          , typeof(IWeaponGetter)        ) , RcdIdleSound                          },
                { new RecordCallKey("ImageSpace"                         , typeof(ICellGetter)          ) , RcdImageSpace                         },
                { new RecordCallKey("ImpactDataSet"                      , typeof(IWeaponGetter)        ) , RcdImpactDataSet                      },
                { new RecordCallKey("INAM"                               , typeof(IBookGetter)          ) , RcdInventoryArt                       },
                { new RecordCallKey("INAM"                               , typeof(INpcGetter)           ) , RcdDeathItem                          },
                { new RecordCallKey("INAM"                               , typeof(IWeaponGetter)        ) , RcdImpactDataSet                      },
                { new RecordCallKey("IngredientValue"                    , typeof(IIngredientGetter)    ) , RcdIngredientValue                    },
                { new RecordCallKey("InteriorLighting"                   , typeof(IWorldspaceGetter)    ) , RcdInteriorLighting                   },
                { new RecordCallKey("InventoryArt"                       , typeof(IBookGetter)          ) , RcdInventoryArt                       },
                { new RecordCallKey("Item"                               , typeof(IContainerGetter)     ) , RcdContainerItems                     },
                { new RecordCallKey("Item"                               , typeof(INpcGetter)           ) , RcdContainerItems                     },
                { new RecordCallKey("Item"                               , typeof(IOutfitGetter)        ) , RcdItems                              },
                { new RecordCallKey("Items"                              , typeof(IContainerGetter)     ) , RcdContainerItems                     },
                { new RecordCallKey("Items"                              , typeof(INpcGetter)           ) , RcdContainerItems                     },
                { new RecordCallKey("Items"                              , typeof(IOutfitGetter)        ) , RcdItems                              },
                { new RecordCallKey("JAIL"                               , typeof(IFactionGetter)       ) , RcdExteriorJailMarker                 },
                { new RecordCallKey("JailOutfit"                         , typeof(IFactionGetter)       ) , RcdJailOutfit                         },
                { new RecordCallKey("JOUT"                               , typeof(IFactionGetter)       ) , RcdJailOutfit                         },
                { new RecordCallKey("Keywords"                           , typeof(IKeywordedGetter)     ) , RcdKeywords                           },
                { new RecordCallKey("KWDA"                               , typeof(IKeywordedGetter)     ) , RcdKeywords                           },
                { new RecordCallKey("LightingTemplate"                   , typeof(ICellGetter)          ) , RcdLightingTemplate                   },
                { new RecordCallKey("Location"                           , zzNull                       ) , RcdLocation                           },
                { new RecordCallKey("LockList"                           , typeof(ICellGetter)          ) , RcdLockList                           },
                { new RecordCallKey("LodWater"                           , typeof(IWorldspaceGetter)    ) , RcdLodWater                           },
                { new RecordCallKey("LodWaterHeight"                     , typeof(IWorldspaceGetter)    ) , RcdLodWaterHeight                     },
                { new RecordCallKey("LTMP"                               , typeof(ICellGetter)          ) , RcdLightingTemplate                   },
                { new RecordCallKey("LTMP"                               , typeof(IWorldspaceGetter)    ) , RcdInteriorLighting                   },
                { new RecordCallKey("MajorFlags"                         , zzNull                       ) , RcdMajorFlags                         },
                { new RecordCallKey("MDOB"                               , typeof(IScrollGetter)        ) , RcdMenuDisplayObject                  },
                { new RecordCallKey("MenuDisplayObject"                  , typeof(IScrollGetter)        ) , RcdMenuDisplayObject                  },
                { new RecordCallKey("MerchantContainer"                  , typeof(IFactionGetter)       ) , RcdMerchantContainer                  },
                { new RecordCallKey("MODL"                               , typeof(IArmorGetter)         ) , RcdArmature                           },
                { new RecordCallKey("Music"                              , zzNull                       ) , RcdMusic                              },
                { new RecordCallKey("NAM3"                               , typeof(IWorldspaceGetter)    ) , RcdLodWater                           },
                { new RecordCallKey("NAM4"                               , typeof(IWorldspaceGetter)    ) , RcdLodWaterHeight                     },
                { new RecordCallKey("NAM6"                               , typeof(INpcGetter)           ) , RcdHeight                             },
                { new RecordCallKey("NAM7"                               , typeof(INpcGetter)           ) , RcdWeight                             },
                { new RecordCallKey("NAM7"                               , typeof(IWeaponGetter)        ) , RcdAttackLoopSound                    },
                { new RecordCallKey("NAM8"                               , typeof(INpcGetter)           ) , RcdSoundLevel                         },
                { new RecordCallKey("NAM8"                               , typeof(IWeaponGetter)        ) , RcdUnequipSound                       },
                { new RecordCallKey("NAM9"                               , typeof(IWeaponGetter)        ) , RcdEquipSound                         },
                { new RecordCallKey("NAMA"                               , typeof(IWorldspaceGetter)    ) , RcdDistantLodMultiplier               },
                { new RecordCallKey("Name"                               , typeof(INamedGetter)         ) , RcdName                               },
                { new RecordCallKey("Name"                               , typeof(INamedRequiredGetter) ) , RcdName                               },
                { new RecordCallKey("ObjectEffect"                       , zzNull                       ) , RcdObjectEffect                       },
                { new RecordCallKey("ObserveDeadBodyOverridePackageList" , typeof(INpcGetter)           ) , RcdObserveDeadBodyOverridePackageList },
                { new RecordCallKey("OCOR"                               , typeof(INpcGetter)           ) , RcdObserveDeadBodyOverridePackageList },
                { new RecordCallKey("ONAM"                               , zzNull                       ) , RcdShortName                          },
                { new RecordCallKey("OpenSound"                          , typeof(IContainerGetter)     ) , RcdOpenSound                          },
                { new RecordCallKey("Owner"                              , typeof(ICellGetter)          ) , RcdOwner                              },
                { new RecordCallKey("Packages"                           , typeof(INpcGetter)           ) , RcdPackages                           },
                { new RecordCallKey("PickUpSound"                        , zzNull                       ) , RcdPickUpSound                        },
                { new RecordCallKey("PKID"                               , typeof(INpcGetter)           ) , RcdPackages                           },
                { new RecordCallKey("PlayerInventoryContainer"           , typeof(IFactionGetter)       ) , RcdPlayerInventoryContainer           },
                { new RecordCallKey("PLCN"                               , typeof(IFactionGetter)       ) , RcdPlayerInventoryContainer           },
                { new RecordCallKey("PNAM"                               , typeof(INpcGetter)           ) , RcdHeadParts                          },
                { new RecordCallKey("Projectile"                         , typeof(IAmmunitionGetter)    ) , RcdProjectile                         },
                { new RecordCallKey("PutDownSound"                       , zzNull                       ) , RcdPutDownSound                       },
                { new RecordCallKey("QNAM"                               , typeof(IContainerGetter)     ) , RcdCloseSound                         },
                { new RecordCallKey("Race"                               , zzNull                       ) , RcdRace                               },
                { new RecordCallKey("RagdollConstraintTemplate"          , typeof(IArmorGetter)         ) , RcdRagdollConstraintTemplate          },
                { new RecordCallKey("Range"                              , typeof(IScrollGetter)        ) , RcdRange                              },
                { new RecordCallKey("RecordFlags"                        , zzNull                       ) , RcdMajorFlags                         },
                { new RecordCallKey("RNAM"                               , zzNull                       ) , RcdRace                               },
                { new RecordCallKey("SharedCrimeFactionList"             , typeof(IFactionGetter)       ) , RcdSharedCrimeFactionList             },
                { new RecordCallKey("ShortName"                          , zzNull                       ) , RcdShortName                          },
                { new RecordCallKey("SkyAndWeatherFromRegion"            , typeof(ICellGetter)          ) , RcdSkyAndWeatherFromRegion            },
                { new RecordCallKey("SleepingOutfit"                     , typeof(INpcGetter)           ) , RcdSleepingOutfit                     },
                { new RecordCallKey("SNAM"                               , typeof(IContainerGetter)     ) , RcdOpenSound                          },
                { new RecordCallKey("SNAM"                               , typeof(IWeaponGetter)        ) , RcdAttackSound                        },
                { new RecordCallKey("SOFT"                               , typeof(INpcGetter)           ) , RcdSleepingOutfit                     },
                { new RecordCallKey("SoundLevel"                         , typeof(INpcGetter)           ) , RcdSoundLevel                         },
                { new RecordCallKey("SpectatorOverridePackageList"       , typeof(INpcGetter)           ) , RcdSpectatorOverridePackageList       },
                { new RecordCallKey("SPLO"                               , typeof(INpcGetter)           ) , RcdActorEffect                        },
                { new RecordCallKey("SPOR"                               , typeof(INpcGetter)           ) , RcdSpectatorOverridePackageList       },
                { new RecordCallKey("STOL"                               , typeof(IFactionGetter)       ) , RcdStolenGoodsContainer               },
                { new RecordCallKey("StolenGoodsContainer"               , typeof(IFactionGetter)       ) , RcdStolenGoodsContainer               },
                { new RecordCallKey("TargetType"                         , typeof(IScrollGetter)        ) , RcdTargetType                         },
                { new RecordCallKey("Template"                           , typeof(INpcGetter)           ) , RcdTemplate                           },
                { new RecordCallKey("TemplateArmor"                      , typeof(IArmorGetter)         ) , RcdTemplateArmor                      },
                { new RecordCallKey("TNAM"                               , typeof(IArmorGetter)         ) , RcdTemplateArmor                      },
                { new RecordCallKey("TNAM"                               , typeof(IWeaponGetter)        ) , RcdAttackFailSound                    },
                { new RecordCallKey("TPLT"                               , typeof(INpcGetter)           ) , RcdTemplate                           },
                { new RecordCallKey("Type"                               , typeof(IBookGetter)          ) , RcdType                               },
                { new RecordCallKey("Type"                               , typeof(IScrollGetter)        ) , RcdType                               },
                { new RecordCallKey("UNAM"                               , typeof(IWeaponGetter)        ) , RcdIdleSound                          },
                { new RecordCallKey("UnequipSound"                       , typeof(IWeaponGetter)        ) , RcdUnequipSound                       },
                { new RecordCallKey("Value"                              , typeof(IWeightValueGetter)   ) , RcdValue                              },
                { new RecordCallKey("VENC"                               , typeof(IFactionGetter)       ) , RcdMerchantContainer                  },
                { new RecordCallKey("VEND"                               , typeof(IFactionGetter)       ) , RcdVendorBuySellList                  },
                { new RecordCallKey("VendorBuySellList"                  , typeof(IFactionGetter)       ) , RcdVendorBuySellList                  },
                { new RecordCallKey("Voice"                              , typeof(INpcGetter)           ) , RcdVoice                              },
                { new RecordCallKey("VTCK"                               , typeof(INpcGetter)           ) , RcdVoice                              },
                { new RecordCallKey("WAIT"                               , typeof(IFactionGetter)       ) , RcdFollowerWaitMarker                 },
                { new RecordCallKey("Water"                              , zzNull                       ) , RcdWater                              },
                { new RecordCallKey("WaterEnvironmentMap"                , typeof(ICellGetter)          ) , RcdWaterEnvironmentMap                },
                { new RecordCallKey("WaterHeight"                        , typeof(ICellGetter)          ) , RcdWaterHeight                        },
                { new RecordCallKey("WaterNoiseTexture"                  , typeof(ICellGetter)          ) , RcdWaterNoiseTexture                  },
                { new RecordCallKey("Weight"                             , typeof(IContainerGetter)     ) , RcdWeight                             },
                { new RecordCallKey("Weight"                             , typeof(INpcGetter)           ) , RcdWeight                             },
                { new RecordCallKey("Weight"                             , typeof(IWeightValueGetter)   ) , RcdWeight                             },
                { new RecordCallKey("WNAM"                               , typeof(INpcGetter)           ) , RcdWornArmor                          },
                { new RecordCallKey("WNAM"                               , typeof(IWeaponGetter)        ) , RcdFirstPersonModel                   },
                { new RecordCallKey("WorldMapOffsetScale"                , typeof(IWorldspaceGetter)    ) , RcdWorldMapOffsetScale                },
                { new RecordCallKey("WornArmor"                          , typeof(INpcGetter)           ) , RcdWornArmor                          },
                { new RecordCallKey("XCAS"                               , typeof(ICellGetter)          ) , RcdAcousticSpace                      },
                { new RecordCallKey("XCCM"                               , typeof(ICellGetter)          ) , RcdSkyAndWeatherFromRegion            },
                { new RecordCallKey("XCIM"                               , typeof(ICellGetter)          ) , RcdImageSpace                         },
                { new RecordCallKey("XCLW"                               , typeof(ICellGetter)          ) , RcdWaterHeight                        },
                { new RecordCallKey("XCWT"                               , zzNull                       ) , RcdWater                              },
                { new RecordCallKey("XEZN"                               , zzNull                       ) , RcdEncounterZone                      },
                { new RecordCallKey("XILL"                               , typeof(ICellGetter)          ) , RcdLockList                           },
                { new RecordCallKey("XLCN"                               , zzNull                       ) , RcdLocation                           },
                { new RecordCallKey("XNAM"                               , typeof(ICellGetter)          ) , RcdWaterNoiseTexture                  },
                { new RecordCallKey("XNAM"                               , typeof(IWeaponGetter)        ) , RcdAttackSound2D                      },
                { new RecordCallKey("XWEM"                               , typeof(ICellGetter)          ) , RcdWaterEnvironmentMap                },
                { new RecordCallKey("YNAM"                               , zzNull                       ) , RcdPickUpSound                        },
                { new RecordCallKey("ZNAM"                               , typeof(ICellGetter)          ) , RcdMusic                              },
                { new RecordCallKey("ZNAM"                               , typeof(INpcGetter)           ) , RcdCombatStyle                        },
                { new RecordCallKey("ZNAM"                               , typeof(IWorldspaceGetter)    ) , RcdMusic                              },
                { new RecordCallKey("ZNAM"                               , zzNull                       ) , RcdPutDownSound                       },
        };

        #endregion RCD Mapping
    }
}