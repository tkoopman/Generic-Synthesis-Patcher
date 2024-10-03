using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x4xx
    internal static class Effects
    {
        // Log Codes: 0x41x
        public static bool FillEffects ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, string propertyName )
        {
            IIngestible? patch = null;

            if (context.Record is IIngestibleGetter record)
            {
                var effectActions = rule.GetValueAs<List<GSPRule.EffectAction>>(valueKey);
                if (rule.OnlyIfDefault && origin != null && origin is IIngestibleGetter originIG && !record.Effects.SequenceEqualNullable(originIG.Effects))
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as effects don't match origin");
                    return false;
                }

                foreach (var effectAction in effectActions ?? [])
                {
                    var e = record.Effects.SingleOrDefault(e => e?.BaseEffect.FormKey.Equals(effectAction.Effect) ?? false, null);
                    if ((e != null && effectAction.Remove) || (e == null && !effectAction.Remove))
                    {
                        var actionEffect = new Effect();
                        actionEffect.BaseEffect.FormKey = effectAction.Effect;
                        patch ??= (IIngestible)context.GetOrAddAsOverride(Global.State.PatchMod);

                        if (effectAction.Remove)
                        {
                            if (e?.Data != null)
                            {
                                actionEffect.Data = new EffectData
                                {
                                    Area = e.Data.Area,
                                    Duration = e.Data.Duration,
                                    Magnitude = e.Data.Magnitude
                                };
                            }

                            _ = (patch.Effects?.Remove(actionEffect));
                        }
                        else
                        {
                            actionEffect.Data = new EffectData
                            {
                                Area = effectAction.Area,
                                Duration = effectAction.Duration,
                                Magnitude = effectAction.Magnitude
                            };

                            patch.Effects?.Add(actionEffect);
                        }
                    }
                }
            }
            else
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "IIngestibleGetter", context.Record.GetType().Name, 0x412);
            }

            if (patch != null)
                LogHelper.Log(LogLevel.Debug, context, propertyName, "Updated.");
            return patch != null;
        }

        // Log Codes: 0x42x
        public static bool ForwardEffects ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IMajorRecordGetter forwardRecord, string propertyName )
        {
            if (context.Record is IIngestibleGetter record && forwardRecord is IIngestibleGetter forward)
            {
                if (forward.Effects.SequenceEqualNullable(record.Effects))
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as already matches forwarding record");
                    return false;
                }

                if (rule.OnlyIfDefault && origin != null && origin is IIngestibleGetter originGetter && !record.Effects.SequenceEqualNullable(originGetter.Effects))
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as keywords don't match origin");
                    return false;
                }

                var patch = (IIngestible)context.GetOrAddAsOverride(Global.State.PatchMod);

                _ = patch.Effects?.RemoveAll(_ => true);

                if (forward.Effects != null && patch.Effects != null)
                {
                    foreach (var e in forward.Effects)
                    {
                        var actionEffect = new Effect();
                        actionEffect.BaseEffect.FormKey = e.BaseEffect.FormKey;

                        if (e.Data != null)
                        {
                            actionEffect.Data = new EffectData
                            {
                                Area = e.Data.Area,
                                Duration = e.Data.Duration,
                                Magnitude = e.Data.Magnitude
                            };
                        }

                        patch.Effects.Add(actionEffect);
                    }
                }

                LogHelper.Log(LogLevel.Debug, context, propertyName, "Updated.");
                return true;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "IIngestibleGetter", context.Record.GetType().Name, 0x422);
            return false;
        }
    }
}