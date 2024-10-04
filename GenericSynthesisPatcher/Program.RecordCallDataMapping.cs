using System;
using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Skyrim;

using String = GenericSynthesisPatcher.Helpers.Action.String;

namespace GenericSynthesisPatcher
{
    public partial class Program
    {
        private static readonly RecordCallData RcdActorEffect               = new RecordCallData<FormLinks<ISpellRecordGetter>>  ( "ActorEffect"               );
        private static readonly RecordCallData RcdAlternateBlockMaterial    = new RecordCallData<FormLink<IMaterialTypeGetter>>  ( "AlternateBlockMaterial"    );
        private static readonly RecordCallData RcdArmature                  = new RecordCallData<FormLinks<IArmorAddonGetter>>   ( "Armature"                  );
        private static readonly RecordCallData RcdArmorRating               = new RecordCallData<Generic<float>>                 ( "ArmorRating"               );
        private static readonly RecordCallData RcdAttackRace                = new RecordCallData<FormLink<IRaceGetter>>          ( "AttackRace"                );
        private static readonly RecordCallData RcdBashImpactDataSet         = new RecordCallData<FormLink<IImpactDataSetGetter>> ( "BashImpactDataSet"         );
        private static readonly RecordCallData RcdBookText                  = new RecordCallData<String>                         ( "BookText"                  );
        private static readonly RecordCallData RcdClass                     = new RecordCallData<FormLink<IClassGetter>>         ( "Class"                     );
        private static readonly RecordCallData RcdCombatOverridePackageList = new RecordCallData<FormLink<IFormListGetter>>      ( "CombatOverridePackageList" );
        private static readonly RecordCallData RcdCombatStyle               = new RecordCallData<FormLink<ICombatStyleGetter>>   ( "CombatStyle"               );
        private static readonly RecordCallData RcdCrimeFaction              = new RecordCallData<FormLink<IFactionGetter>>       ( "CrimeFaction"              );
        private static readonly RecordCallData RcdDamage                    = new RecordCallData<Generic<float>>                 ( "Damage"                    );
        private static readonly RecordCallData RcdDeathItem                 = new RecordCallData<FormLink<ILeveledItemGetter>>   ( "DeathItem"                 );
        private static readonly RecordCallData RcdDefaultOutfit             = new RecordCallData<FormLink<IOutfitGetter>>        ( "DefaultOutfit"             );
        private static readonly RecordCallData RcdDefaultPackageList        = new RecordCallData<FormLink<IFormListGetter>>      ( "DefaultPackageList"        );
        private static readonly RecordCallData RcdDescription               = new RecordCallData<String>                         ( "Description"               );
        private static readonly RecordCallData RcdEffects                   = new RecordCallData<Effects>                        ( "Effects"                   );
        private static readonly RecordCallData RcdEnchantmentAmount         = new RecordCallData<Generic<ushort>>                ( "EnchantmentAmount"         );
        private static readonly RecordCallData RcdEquipmentType             = new RecordCallData<FormLink<IEquipTypeGetter>>     ( "EquipmentType"             );
        private static readonly RecordCallData RcdEquipType                 = new RecordCallData<FormLink<IEquipTypeGetter>>     ( "EquipType"                 );
        private static readonly RecordCallData RcdFarAwayModel              = new RecordCallData<FormLink<IArmorGetter>>         ( "FarAwayModel"              );
        private static readonly RecordCallData RcdFlags                     = new RecordCallData<Flags>                          ( "Flags"                     );
        private static readonly RecordCallData RcdIngredientValue           = new RecordCallData<Generic<int>>                   ( "IngredientValue"           );
        private static readonly RecordCallData RcdInventoryArt              = new RecordCallData<FormLink<IStaticGetter>>        ( "InventoryArt"              );
        private static readonly RecordCallData RcdItems                     = new RecordCallData<FormLinks<IOutfitTargetGetter>> ( "Items"                     );
        private static readonly RecordCallData RcdKeywords                  = new RecordCallData<Keywords>                       ( "Keywords"                  );
        private static readonly RecordCallData RcdLocation                  = new RecordCallData<FormLink<ILocationGetter>>      ( "Location"                  );
        private static readonly RecordCallData RcdMajorFlags                = new RecordCallData<Flags>                          ( "MajorFlags"                );
        private static readonly RecordCallData RcdName                      = new RecordCallData<String>                         ( "Name"                      );
        private static readonly RecordCallData RcdObjectEffect              = new RecordCallData<FormLink<IEffectRecordGetter>>  ( "ObjectEffect"              );
        private static readonly RecordCallData RcdProjectile                = new RecordCallData<FormLink<IProjectileGetter>>    ( "Projectile"                );
        private static readonly RecordCallData RcdRace                      = new RecordCallData<FormLink<IRaceGetter>>          ( "Race"                      );
        private static readonly RecordCallData RcdRagdollConstraintTemplate = new RecordCallData<String>                         ( "RagdollConstraintTemplate" );
        private static readonly RecordCallData RcdShortName                 = new RecordCallData<String>                         ( "ShortName"                 );
        private static readonly RecordCallData RcdTemplateArmor             = new RecordCallData<FormLink<IArmorGetter>>         ( "TemplateArmor"             );
        private static readonly RecordCallData RcdValue                     = new RecordCallData<Generic<uint>>                  ( "Value"                     );
        private static readonly RecordCallData RcdWater                     = new RecordCallData<FormLink<IWaterGetter>>         ( "Water"                     );
        private static readonly RecordCallData RcdWeight                    = new RecordCallData<Generic<float>>                 ( "Weight"                    );

        /// <summary>
        /// This list is what directs each field update to the correct function.
        /// List is sorted first by JSON key, then by record type, making sure null comes after any defined types.
        /// Having multiple entries with the exact same RecordCallKey will ignore all except the first one found in the list.
        /// </summary>
        public static readonly SortedList<RecordCallKey, RecordCallData> RecordCallDataMapping = new()
        {
                { new RecordCallKey("ActorEffect"               , typeof(INpcGetter)        ) , RcdActorEffect               },
                { new RecordCallKey("AlternateBlockMaterial"    , null                      ) , RcdAlternateBlockMaterial    },
                { new RecordCallKey("ANAM"                      , typeof(INpcGetter)        ) , RcdFarAwayModel              },
                { new RecordCallKey("Armature"                  , null                      ) , RcdArmature                  },
                { new RecordCallKey("ArmorRating"               , typeof(IArmorGetter)      ) , RcdArmorRating               },
                { new RecordCallKey("AttackRace"                , typeof(INpcGetter)        ) , RcdAttackRace                },
                { new RecordCallKey("BAMT"                      , null                      ) , RcdAlternateBlockMaterial    },
                { new RecordCallKey("BashImpactDataSet"         , typeof(IArmorGetter)      ) , RcdBashImpactDataSet         },
                { new RecordCallKey("BIDS"                      , typeof(IArmorGetter)      ) , RcdBashImpactDataSet         },
                { new RecordCallKey("BMCT"                      , null                      ) , RcdRagdollConstraintTemplate },
                { new RecordCallKey("BookText"                  , typeof(IBookGetter)       ) , RcdBookText                  },
                { new RecordCallKey("Class"                     , typeof(INpcGetter)        ) , RcdClass                     },
                { new RecordCallKey("CNAM"                      , typeof(IBookGetter)       ) , RcdDescription               },
                { new RecordCallKey("CNAM"                      , typeof(INpcGetter)        ) , RcdClass                     },
                { new RecordCallKey("CombatOverridePackageList" , typeof(INpcGetter)        ) , RcdCombatOverridePackageList },
                { new RecordCallKey("CombatStyle"               , typeof(INpcGetter)        ) , RcdCombatStyle               },
                { new RecordCallKey("CRIF"                      , typeof(INpcGetter)        ) , RcdCrimeFaction              },
                { new RecordCallKey("CrimeFaction"              , typeof(INpcGetter)        ) , RcdCrimeFaction              },
                { new RecordCallKey("Damage"                    , typeof(IAmmunitionGetter) ) , RcdDamage                    },
                { new RecordCallKey("DataFlags"                 , null                      ) , RcdFlags                     },
                { new RecordCallKey("DeathItem"                 , typeof(INpcGetter)        ) , RcdDeathItem                 },
                { new RecordCallKey("DefaultOutfit"             , typeof(INpcGetter)        ) , RcdDefaultOutfit             },
                { new RecordCallKey("DefaultPackageList"        , typeof(INpcGetter)        ) , RcdDefaultPackageList        },
                { new RecordCallKey("Desc"                      , typeof(IBookGetter)       ) , RcdBookText                  },
                { new RecordCallKey("Desc"                      , null                      ) , RcdDescription               },
                { new RecordCallKey("Description"               , null                      ) , RcdDescription               },
                { new RecordCallKey("DMG"                       , typeof(IAmmunitionGetter) ) , RcdDamage                    },
                { new RecordCallKey("DNAM"                      , typeof(IArmorGetter)      ) , RcdArmorRating               },
                { new RecordCallKey("DOFT"                      , typeof(INpcGetter)        ) , RcdDefaultOutfit             },
                { new RecordCallKey("DPLT"                      , typeof(INpcGetter)        ) , RcdDefaultPackageList        },
                { new RecordCallKey("EAMT"                      , null                      ) , RcdEnchantmentAmount         },
                { new RecordCallKey("ECOR"                      , typeof(INpcGetter)        ) , RcdCombatOverridePackageList },
                { new RecordCallKey("Effects"                   , null                      ) , RcdEffects                   },
                { new RecordCallKey("EITM"                      , null                      ) , RcdObjectEffect              },
                { new RecordCallKey("EnchantmentAmount"         , null                      ) , RcdEnchantmentAmount         },
                { new RecordCallKey("EquipmentType"             , null                      ) , RcdEquipmentType             },
                { new RecordCallKey("EquipType"                 , typeof(IIngredientGetter) ) , RcdEquipType                 },
                { new RecordCallKey("ETYP"                      , null                      ) , RcdEquipmentType             },
                { new RecordCallKey("FarAwayModel"              , typeof(INpcGetter)        ) , RcdFarAwayModel              },
                { new RecordCallKey("Flags"                     , null                      ) , RcdFlags                     },
                { new RecordCallKey("Full"                      , typeof(INamedGetter)      ) , RcdName                      },
                { new RecordCallKey("INAM"                      , typeof(INpcGetter)        ) , RcdDeathItem                 },
                { new RecordCallKey("IngredientValue"           , typeof(IIngredientGetter) ) , RcdIngredientValue           },
                { new RecordCallKey("InventoryArt"              , null                      ) , RcdInventoryArt              },
                { new RecordCallKey("Items"                     , typeof(IOutfitGetter)     ) , RcdItems                     },
                { new RecordCallKey("Keywords"                  , typeof(IKeywordedGetter)  ) , RcdKeywords                  },
                { new RecordCallKey("KWDA"                      , typeof(IKeywordedGetter)  ) , RcdKeywords                  },
                { new RecordCallKey("Location"                  , null                      ) , RcdLocation                  },
                { new RecordCallKey("MajorFlags"                , null                      ) , RcdMajorFlags                },
                { new RecordCallKey("Name"                      , typeof(INamedGetter)      ) , RcdName                      },
                { new RecordCallKey("ObjectEffect"              , null                      ) , RcdObjectEffect              },
                { new RecordCallKey("ONAM"                      , null                      ) , RcdShortName                 },
                { new RecordCallKey("Projectile"                , null                      ) , RcdProjectile                },
                { new RecordCallKey("Race"                      , null                      ) , RcdRace                      },
                { new RecordCallKey("RagdollConstraintTemplate" , null                      ) , RcdRagdollConstraintTemplate },
                { new RecordCallKey("RecordFlags"               , null                      ) , RcdMajorFlags                },
                { new RecordCallKey("RNAM"                      , null                      ) , RcdRace                      },
                { new RecordCallKey("ShortName"                 , null                      ) , RcdShortName                 },
                { new RecordCallKey("TemplateArmor"             , typeof(IArmorGetter)      ) , RcdTemplateArmor             },
                { new RecordCallKey("Value"                     , typeof(IWeightValueGetter)) , RcdValue                     },
                { new RecordCallKey("Water"                     , null                      ) , RcdWater                     },
                { new RecordCallKey("Weight"                    , typeof(IWeightValueGetter)) , RcdWeight                    },
                { new RecordCallKey("XCWT"                      , null                      ) , RcdWater                     },
                { new RecordCallKey("XLCN"                      , null                      ) , RcdLocation                  },
                { new RecordCallKey("ZNAM"                      , typeof(INpcGetter)        ) , RcdCombatStyle               },
        };
    }
}