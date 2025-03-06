﻿using System.Drawing;

using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers
{
    public static partial class RecordPropertyMappings
    {
        private static void populateSubAliases ()
        {
#pragma warning disable format
            AddAlias(typeof(IActorValueInformationGetter), "AVSK.ImproveMult"             , "Skill.ImproveMult");
            AddAlias(typeof(IActorValueInformationGetter), "AVSK.ImproveOffset"           , "Skill.ImproveOffset");
            AddAlias(typeof(IActorValueInformationGetter), "AVSK.OffsetMult"              , "Skill.OffsetMult");
            AddAlias(typeof(IActorValueInformationGetter), "AVSK.UseMult"                 , "Skill.UseMult");
            AddAlias(typeof(IArmorAddonGetter)           , "BODT.ArmorType"               , "BodyTemplate.ArmorType");
            AddAlias(typeof(IArmorAddonGetter)           , "BODT.FirstPersonFlags"        , "BodyTemplate.FirstPersonFlags");
            AddAlias(typeof(IArmorAddonGetter)           , "BODT.Flags"                   , "BodyTemplate.Flags");
            AddAlias(typeof(IArmorGetter)                , "BOD2.ArmorType"               , "BodyTemplate.ArmorType");
            AddAlias(typeof(IArmorGetter)                , "BOD2.FirstPersonFlags"        , "BodyTemplate.FirstPersonFlags");
            AddAlias(typeof(IArmorGetter)                , "BOD2.Flags"                   , "BodyTemplate.Flags");
            AddAlias(typeof(ICellGetter)                 , "XCLL.AmbientColor"            , "Lighting.AmbientColor");
            AddAlias(typeof(ICellGetter)                 , "XCLL.AmbientColors"           , "Lighting.AmbientColors");
            AddAlias(typeof(ICellGetter)                 , "XCLL.DirectionalColor"        , "Lighting.DirectionalColor");
            AddAlias(typeof(ICellGetter)                 , "XCLL.DirectionalFade"         , "Lighting.DirectionalFade");
            AddAlias(typeof(ICellGetter)                 , "XCLL.DirectionalRotationXY"   , "Lighting.DirectionalRotationXY");
            AddAlias(typeof(ICellGetter)                 , "XCLL.DirectionalRotationZ"    , "Lighting.DirectionalRotationZ");
            AddAlias(typeof(ICellGetter)                 , "XCLL.FogClipDistance"         , "Lighting.FogClipDistance");
            AddAlias(typeof(ICellGetter)                 , "XCLL.FogFar"                  , "Lighting.FogFar");
            AddAlias(typeof(ICellGetter)                 , "XCLL.FogFarColor"             , "Lighting.FogFarColor");
            AddAlias(typeof(ICellGetter)                 , "XCLL.FogMax"                  , "Lighting.FogMax");
            AddAlias(typeof(ICellGetter)                 , "XCLL.FogNear"                 , "Lighting.FogNear");
            AddAlias(typeof(ICellGetter)                 , "XCLL.FogNearColor"            , "Lighting.FogNearColor");
            AddAlias(typeof(ICellGetter)                 , "XCLL.FogPower"                , "Lighting.FogPower");
            AddAlias(typeof(ICellGetter)                 , "XCLL.Inherits"                , "Lighting.Inherits");
            AddAlias(typeof(ICellGetter)                 , "XCLL.LightFadeBegin"          , "Lighting.LightFadeBegin");
            AddAlias(typeof(ICellGetter)                 , "XCLL.LightFadeEnd"            , "Lighting.LightFadeEnd");
            AddAlias(typeof(ICellGetter)                 , "XCLX.Flags"                   , "Grid.Flags");
            AddAlias(typeof(ICellGetter)                 , "XCLX.Point"                   , "Grid.Point");
            AddAlias(typeof(ICellGetter)                 , "XCLX.Point.X"                 , "Grid.Point.X");
            AddAlias(typeof(ICellGetter)                 , "XCLX.Point.Y"                 , "Grid.Point.Y");
            AddAlias(typeof(ICombatStyleGetter)          , "CSCR.CircleMult"              , "CloseRange.CircleMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSCR.FallbackMult"            , "CloseRange.FallbackMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSCR.FlankDistance"           , "CloseRange.FlankDistance");
            AddAlias(typeof(ICombatStyleGetter)          , "CSCR.StalkTime"               , "CloseRange.StalkTime");
            AddAlias(typeof(ICombatStyleGetter)          , "CSFL.DiveBombChance"          , "Flight.DiveBombChance");
            AddAlias(typeof(ICombatStyleGetter)          , "CSFL.FlyingAttackChance"      , "Flight.FlyingAttackChance");
            AddAlias(typeof(ICombatStyleGetter)          , "CSFL.GroundAttackChance"      , "Flight.GroundAttackChance");
            AddAlias(typeof(ICombatStyleGetter)          , "CSFL.GroundAttackTime"        , "Flight.GroundAttackTime");
            AddAlias(typeof(ICombatStyleGetter)          , "CSFL.HoverChance"             , "Flight.HoverChance");
            AddAlias(typeof(ICombatStyleGetter)          , "CSFL.HoverTime"               , "Flight.HoverTime");
            AddAlias(typeof(ICombatStyleGetter)          , "CSFL.PerchAttackChance"       , "Flight.PerchAttackChance");
            AddAlias(typeof(ICombatStyleGetter)          , "CSFL.PerchAttackTime"         , "Flight.PerchAttackTime");
            AddAlias(typeof(ICombatStyleGetter)          , "CSME.AttackStaggeredMult"     , "Melee.AttackStaggeredMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSME.BashAttackMult"          , "Melee.BashAttackMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSME.BashMult"                , "Melee.BashMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSME.BashPowerAttackMult"     , "Melee.BashPowerAttackMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSME.BashRecoilMult"          , "Melee.BashRecoilMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSME.PowerAttackBlockingMult" , "Melee.PowerAttackBlockingMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSME.PowerAttackStaggeredMult", "Melee.PowerAttackStaggeredMult");
            AddAlias(typeof(ICombatStyleGetter)          , "CSME.SpecialAttackMult"       , "Melee.SpecialAttackMult");
            AddAlias(typeof(IFactionGetter)              , "CRVA.Arrest"                  , "CrimeValues.Arrest");
            AddAlias(typeof(IFactionGetter)              , "CRVA.Assault"                 , "CrimeValues.Assault");
            AddAlias(typeof(IFactionGetter)              , "CRVA.AttackOnSight"           , "CrimeValues.AttackOnSight");
            AddAlias(typeof(IFactionGetter)              , "CRVA.Escape"                  , "CrimeValues.Escape");
            AddAlias(typeof(IFactionGetter)              , "CRVA.Murder"                  , "CrimeValues.Murder");
            AddAlias(typeof(IFactionGetter)              , "CRVA.Pickpocket"              , "CrimeValues.Pickpocket");
            AddAlias(typeof(IFactionGetter)              , "CRVA.StealMult"               , "CrimeValues.StealMult");
            AddAlias(typeof(IFactionGetter)              , "CRVA.Trespass"                , "CrimeValues.Trespass");
            AddAlias(typeof(IFactionGetter)              , "CRVA.Werewolf"                , "CrimeValues.Werewolf");
            AddAlias(typeof(IFactionGetter)              , "VENV.EndHour"                 , "VendorValues.EndHour");
            AddAlias(typeof(IFactionGetter)              , "VENV.NotSellBuy"              , "VendorValues.NotSellBuy");
            AddAlias(typeof(IFactionGetter)              , "VENV.OnlyBuysStolenItems"     , "VendorValues.OnlyBuysStolenItems");
            AddAlias(typeof(IFactionGetter)              , "VENV.Radius"                  , "VendorValues.Radius");
            AddAlias(typeof(IFactionGetter)              , "VENV.StartHour"               , "VendorValues.StartHour");
            AddAlias(typeof(IFloraGetter)                , "PFPC.Fall"                    , "Production.Fall");
            AddAlias(typeof(IFloraGetter)                , "PFPC.Spring"                  , "Production.Spring");
            AddAlias(typeof(IFloraGetter)                , "PFPC.Summer"                  , "Production.Summer");
            AddAlias(typeof(IFloraGetter)                , "PFPC.Winter"                  , "Production.Winter");
            AddAlias(typeof(IFurnitureGetter)            , "WBDT.BenchType"               , "WorkbenchData.BenchType");
            AddAlias(typeof(IFurnitureGetter)            , "WBDT.UsesSkill"               , "WorkbenchData.UsesSkill");
            AddAlias(typeof(IImageSpaceGetter)           , "CNAM.Brightness"              , "Cinematic.Brightness");
            AddAlias(typeof(IImageSpaceGetter)           , "CNAM.Contrast"                , "Cinematic.Contrast");
            AddAlias(typeof(IImageSpaceGetter)           , "CNAM.Saturation"              , "Cinematic.Saturation");
            AddAlias(typeof(IImageSpaceGetter)           , "DNAM.BlurRadius"              , "DepthOfField.BlurRadius");
            AddAlias(typeof(IImageSpaceGetter)           , "DNAM.Distance"                , "DepthOfField.Distance");
            AddAlias(typeof(IImageSpaceGetter)           , "DNAM.Range"                   , "DepthOfField.Range");
            AddAlias(typeof(IImageSpaceGetter)           , "DNAM.Sky"                     , "DepthOfField.Sky");
            AddAlias(typeof(IImageSpaceGetter)           , "DNAM.Strength"                , "DepthOfField.Strength");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.BloomBlurRadius"         , "Hdr.BloomBlurRadius");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.BloomScale"              , "Hdr.BloomScale");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.BloomThreshold"          , "Hdr.BloomThreshold");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.EyeAdaptSpeed"           , "Hdr.EyeAdaptSpeed");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.EyeAdaptStrength"        , "Hdr.EyeAdaptStrength");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.ReceiveBloomThreshold"   , "Hdr.ReceiveBloomThreshold");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.SkyScale"                , "Hdr.SkyScale");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.SunlightScale"           , "Hdr.SunlightScale");
            AddAlias(typeof(IImageSpaceGetter)           , "HNAM.White"                   , "Hdr.White");
            AddAlias(typeof(IImageSpaceGetter)           , "TNAM.Amount"                  , "Tint.Amount");
            AddAlias(typeof(IImageSpaceGetter)           , "TNAM.Color"                   , "Tint.Color");
            AddAlias(typeof(IImpactGetter)               , "DODT.Color"                   , "Decal.Color");
            AddAlias(typeof(IImpactGetter)               , "DODT.Depth"                   , "Decal.Depth");
            AddAlias(typeof(IImpactGetter)               , "DODT.Flags"                   , "Decal.Flags");
            AddAlias(typeof(IImpactGetter)               , "DODT.MaxHeight"               , "Decal.MaxHeight");
            AddAlias(typeof(IImpactGetter)               , "DODT.MaxWidth"                , "Decal.MaxWidth");
            AddAlias(typeof(IImpactGetter)               , "DODT.MinHeight"               , "Decal.MinHeight");
            AddAlias(typeof(IImpactGetter)               , "DODT.MinWidth"                , "Decal.MinWidth");
            AddAlias(typeof(IImpactGetter)               , "DODT.ParallaxPasses"          , "Decal.ParallaxPasses");
            AddAlias(typeof(IImpactGetter)               , "DODT.ParallaxScale"           , "Decal.ParallaxScale");
            AddAlias(typeof(IImpactGetter)               , "DODT.Shininess"               , "Decal.Shininess");
            AddAlias(typeof(ILightingTemplateGetter)     , "DALC.DirectionalXMinus"       , "DirectionalAmbientColors.DirectionalXMinus");
            AddAlias(typeof(ILightingTemplateGetter)     , "DALC.DirectionalXPlus"        , "DirectionalAmbientColors.DirectionalXPlus");
            AddAlias(typeof(ILightingTemplateGetter)     , "DALC.DirectionalYMinus"       , "DirectionalAmbientColors.DirectionalYMinus");
            AddAlias(typeof(ILightingTemplateGetter)     , "DALC.DirectionalYPlus"        , "DirectionalAmbientColors.DirectionalYPlus");
            AddAlias(typeof(ILightingTemplateGetter)     , "DALC.DirectionalZMinus"       , "DirectionalAmbientColors.DirectionalZMinus");
            AddAlias(typeof(ILightingTemplateGetter)     , "DALC.DirectionalZPlus"        , "DirectionalAmbientColors.DirectionalZPlus");
            AddAlias(typeof(ILightingTemplateGetter)     , "DALC.Scale"                   , "DirectionalAmbientColors.Scale");
            AddAlias(typeof(ILightingTemplateGetter)     , "DALC.Specular"                , "DirectionalAmbientColors.Specular");
            AddAlias(typeof(ILoadScreenGetter)           , "ONAM.Max"                     , "RotationOffsetConstraints.Max");
            AddAlias(typeof(ILoadScreenGetter)           , "ONAM.Min"                     , "RotationOffsetConstraints.Min");
            AddAlias(typeof(ILoadScreenGetter)           , "RNAM.X"                       , "InitialRotation.X");
            AddAlias(typeof(ILoadScreenGetter)           , "RNAM.Y"                       , "InitialRotation.Y");
            AddAlias(typeof(ILoadScreenGetter)           , "RNAM.Z"                       , "InitialRotation.Z");
            AddAlias(typeof(ILoadScreenGetter)           , "XNAM.X"                       , "InitialTranslationOffset.X");
            AddAlias(typeof(ILoadScreenGetter)           , "XNAM.Y"                       , "InitialTranslationOffset.Y");
            AddAlias(typeof(ILoadScreenGetter)           , "XNAM.Z"                       , "InitialTranslationOffset.Z");
            AddAlias(typeof(IMovementTypeGetter)         , "INAM.Directional"             , "AnimationChangeThresholds.Directional");
            AddAlias(typeof(IMovementTypeGetter)         , "INAM.MovementSpeed"           , "AnimationChangeThresholds.MovementSpeed");
            AddAlias(typeof(IMovementTypeGetter)         , "INAM.RotationSpeed"           , "AnimationChangeThresholds.RotationSpeed");
            AddAlias(typeof(IMusicTrackGetter)           , "LNAM.Begins"                  , "LoopData.Begins");
            AddAlias(typeof(IMusicTrackGetter)           , "LNAM.Count"                   , "LoopData.Count");
            AddAlias(typeof(IMusicTrackGetter)           , "LNAM.Ends"                    , "LoopData.Ends");
            AddAlias(typeof(INpcGetter)                  , "AIDT.Aggression"              , "AIData.Aggression");
            AddAlias(typeof(INpcGetter)                  , "AIDT.AggroRadiusBehavior"     , "AIData.AggroRadiusBehavior");
            AddAlias(typeof(INpcGetter)                  , "AIDT.Assistance"              , "AIData.Assistance");
            AddAlias(typeof(INpcGetter)                  , "AIDT.Attack"                  , "AIData.Attack");
            AddAlias(typeof(INpcGetter)                  , "AIDT.Confidence"              , "AIData.Confidence");
            AddAlias(typeof(INpcGetter)                  , "AIDT.EnergyLevel"             , "AIData.EnergyLevel");
            AddAlias(typeof(INpcGetter)                  , "AIDT.Mood"                    , "AIData.Mood");
            AddAlias(typeof(INpcGetter)                  , "AIDT.Responsibility"          , "AIData.Responsibility");
            AddAlias(typeof(INpcGetter)                  , "AIDT.Warn"                    , "AIData.Warn");
            AddAlias(typeof(INpcGetter)                  , "AIDT.WarnOrAttack"            , "AIData.WarnOrAttack");
            AddAlias(typeof(INpcGetter)                  , "DNAM.FarAwayModelDistance"    , "PlayerSkills.FarAwayModelDistance");
            AddAlias(typeof(INpcGetter)                  , "DNAM.GearedUpWeapons"         , "PlayerSkills.GearedUpWeapons");
            AddAlias(typeof(INpcGetter)                  , "DNAM.Health"                  , "PlayerSkills.Health");
            AddAlias(typeof(INpcGetter)                  , "DNAM.Magicka"                 , "PlayerSkills.Magicka");
            AddAlias(typeof(INpcGetter)                  , "DNAM.Stamina"                 , "PlayerSkills.Stamina");
            AddAlias(typeof(INpcGetter)                  , "NAM9.BrowsForwardVsBack"      , "FaceMorph.BrowsForwardVsBack");
            AddAlias(typeof(INpcGetter)                  , "NAM9.BrowsInVsOut"            , "FaceMorph.BrowsInVsOut");
            AddAlias(typeof(INpcGetter)                  , "NAM9.BrowsUpVsDown"           , "FaceMorph.BrowsUpVsDown");
            AddAlias(typeof(INpcGetter)                  , "NAM9.CheeksForwardVsBack"     , "FaceMorph.CheeksForwardVsBack");
            AddAlias(typeof(INpcGetter)                  , "NAM9.CheeksUpVsDown"          , "FaceMorph.CheeksUpVsDown");
            AddAlias(typeof(INpcGetter)                  , "NAM9.ChinNarrowVsWide"        , "FaceMorph.ChinNarrowVsWide");
            AddAlias(typeof(INpcGetter)                  , "NAM9.ChinUnderbiteVsOverbite" , "FaceMorph.ChinUnderbiteVsOverbite");
            AddAlias(typeof(INpcGetter)                  , "NAM9.ChinUpVsDown"            , "FaceMorph.ChinUpVsDown");
            AddAlias(typeof(INpcGetter)                  , "NAM9.EyesForwardVsBack"       , "FaceMorph.EyesForwardVsBack");
            AddAlias(typeof(INpcGetter)                  , "NAM9.EyesInVsOut"             , "FaceMorph.EyesInVsOut");
            AddAlias(typeof(INpcGetter)                  , "NAM9.EyesUpVsDown"            , "FaceMorph.EyesUpVsDown");
            AddAlias(typeof(INpcGetter)                  , "NAM9.JawForwardVsBack"        , "FaceMorph.JawForwardVsBack");
            AddAlias(typeof(INpcGetter)                  , "NAM9.JawNarrowVsWide"         , "FaceMorph.JawNarrowVsWide");
            AddAlias(typeof(INpcGetter)                  , "NAM9.JawUpVsDown"             , "FaceMorph.JawUpVsDown");
            AddAlias(typeof(INpcGetter)                  , "NAM9.LipsInVsOut"             , "FaceMorph.LipsInVsOut");
            AddAlias(typeof(INpcGetter)                  , "NAM9.LipsUpVsDown"            , "FaceMorph.LipsUpVsDown");
            AddAlias(typeof(INpcGetter)                  , "NAM9.NoseLongVsShort"         , "FaceMorph.NoseLongVsShort");
            AddAlias(typeof(INpcGetter)                  , "NAM9.NoseUpVsDown"            , "FaceMorph.NoseUpVsDown");
            AddAlias(typeof(INpcGetter)                  , "NAMA.Eyes"                    , "FaceParts.Eyes");
            AddAlias(typeof(INpcGetter)                  , "NAMA.Mouth"                   , "FaceParts.Mouth");
            AddAlias(typeof(INpcGetter)                  , "NAMA.Nose"                    , "FaceParts.Nose");
            AddAlias(typeof(IRaceGetter)                 , "ANAM.Female"                  , "SkeletalModel.Female");
            AddAlias(typeof(IRaceGetter)                 , "ANAM.Male"                    , "SkeletalModel.Male");
            AddAlias(typeof(IRaceGetter)                 , "BOD2.ArmorType"               , "BodyTemplate.ArmorType");
            AddAlias(typeof(IRaceGetter)                 , "BOD2.FirstPersonFlags"        , "BodyTemplate.FirstPersonFlags");
            AddAlias(typeof(IRaceGetter)                 , "BOD2.Flags"                   , "BodyTemplate.Flags");
            AddAlias(typeof(IRaceGetter)                 , "DNAM.Female"                  , "DecapitateArmors.Female");
            AddAlias(typeof(IRaceGetter)                 , "DNAM.Male"                    , "DecapitateArmors.Male");
            AddAlias(typeof(IRaceGetter)                 , "HCLF.Female"                  , "DefaultHairColors.Female");
            AddAlias(typeof(IRaceGetter)                 , "HCLF.Male"                    , "DefaultHairColors.Male");
            AddAlias(typeof(IRaceGetter)                 , "VTCK.Female"                  , "Voices.Female");
            AddAlias(typeof(IRaceGetter)                 , "VTCK.Male"                    , "Voices.Male");
            AddAlias(typeof(ISoundOutputModelGetter)     , "ONAM.Channel0"                , "OutputChannels.Channel0");
            AddAlias(typeof(ISoundOutputModelGetter)     , "ONAM.Channel1"                , "OutputChannels.Channel1");
            AddAlias(typeof(ISoundOutputModelGetter)     , "ONAM.Channel2"                , "OutputChannels.Channel2");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.Color"                   , "Decal.Color");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.Depth"                   , "Decal.Depth");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.Flags"                   , "Decal.Flags");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.MaxHeight"               , "Decal.MaxHeight");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.MaxWidth"                , "Decal.MaxWidth");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.MinHeight"               , "Decal.MinHeight");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.MinWidth"                , "Decal.MinWidth");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.ParallaxPasses"          , "Decal.ParallaxPasses");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.ParallaxScale"           , "Decal.ParallaxScale");
            AddAlias(typeof(ITextureSetGetter)           , "DODT.Shininess"               , "Decal.Shininess");
            AddAlias(typeof(IWaterGetter)                , "NAM0.X"                       , "LinearVelocity.X");
            AddAlias(typeof(IWaterGetter)                , "NAM0.Y"                       , "LinearVelocity.Y");
            AddAlias(typeof(IWaterGetter)                , "NAM0.Z"                       , "LinearVelocity.Z");
            AddAlias(typeof(IWaterGetter)                , "NAM1.X"                       , "AngularVelocity.X");
            AddAlias(typeof(IWaterGetter)                , "NAM1.Y"                       , "AngularVelocity.Y");
            AddAlias(typeof(IWaterGetter)                , "NAM1.Z"                       , "AngularVelocity.Z");
            AddAlias(typeof(IWeatherGetter)              , "DALC.Day"                     , "DirectionalAmbientLightingColors.Day");
            AddAlias(typeof(IWeatherGetter)              , "DALC.Night"                   , "DirectionalAmbientLightingColors.Night");
            AddAlias(typeof(IWeatherGetter)              , "DALC.Sunrise"                 , "DirectionalAmbientLightingColors.Sunrise");
            AddAlias(typeof(IWeatherGetter)              , "DALC.Sunset"                  , "DirectionalAmbientLightingColors.Sunset");
            AddAlias(typeof(IWeatherGetter)              , "HNAM.Day"                     , "VolumetricLighting.Day");
            AddAlias(typeof(IWeatherGetter)              , "HNAM.Night"                   , "VolumetricLighting.Night");
            AddAlias(typeof(IWeatherGetter)              , "HNAM.Sunrise"                 , "VolumetricLighting.Sunrise");
            AddAlias(typeof(IWeatherGetter)              , "HNAM.Sunset"                  , "VolumetricLighting.Sunset");
            AddAlias(typeof(IWeatherGetter)              , "IMSP.Day"                     , "ImageSpaces.Day");
            AddAlias(typeof(IWeatherGetter)              , "IMSP.Night"                   , "ImageSpaces.Night");
            AddAlias(typeof(IWeatherGetter)              , "IMSP.Sunrise"                 , "ImageSpaces.Sunrise");
            AddAlias(typeof(IWeatherGetter)              , "IMSP.Sunset"                  , "ImageSpaces.Sunset");
            AddAlias(typeof(IWorldspaceGetter)           , "DNAM.DefaultLandHeight"       , "LandDefaults.DefaultLandHeight");
            AddAlias(typeof(IWorldspaceGetter)           , "DNAM.DefaultWaterHeight"      , "LandDefaults.DefaultWaterHeight");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.CameraInitialPitch"      , "MapData.CameraInitialPitch");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.CameraMaxHeight"         , "MapData.CameraMaxHeight");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.CameraMinHeight"         , "MapData.CameraMinHeight");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.NorthwestCellCoords"     , "MapData.NorthwestCellCoords");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.NorthwestCellCoords.X"   , "MapData.NorthwestCellCoords.X");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.NorthwestCellCoords.Y"   , "MapData.NorthwestCellCoords.Y");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.SoutheastCellCoords"     , "MapData.SoutheastCellCoords");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.SoutheastCellCoords.X"   , "MapData.SoutheastCellCoords.X");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.SoutheastCellCoords.Y"   , "MapData.SoutheastCellCoords.Y");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.UsableDimensions"        , "MapData.UsableDimensions");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.UsableDimensions.X"      , "MapData.UsableDimensions.X");
            AddAlias(typeof(IWorldspaceGetter)           , "MNAM.UsableDimensions.Y"      , "MapData.UsableDimensions.Y");
            AddAlias(typeof(IWorldspaceGetter)           , "NAM0.X"                       , "ObjectBoundsMin.X");
            AddAlias(typeof(IWorldspaceGetter)           , "NAM0.Y"                       , "ObjectBoundsMin.Y");
            AddAlias(typeof(IWorldspaceGetter)           , "NAM9.X"                       , "ObjectBoundsMax.X");
            AddAlias(typeof(IWorldspaceGetter)           , "NAM9.Y"                       , "ObjectBoundsMax.Y");
            AddAlias(typeof(IWorldspaceGetter)           , "WCTR.X"                       , "FixedDimensionsCenterCell.X");
            AddAlias(typeof(IWorldspaceGetter)           , "WCTR.Y"                       , "FixedDimensionsCenterCell.Y");
#pragma warning restore format
        }
    }
}