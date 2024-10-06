using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

using static GenericSynthesisPatcher.Json.Data.GSPRule;
using static GenericSynthesisPatcher.Program;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x4xx
    public class Effects : IAction
    {
        public static bool CanFill () => true;
        public static bool CanForward () => true;

        // Log Codes: 0x41x
        public static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd )
        {
            IIngestible? patch = null;

            if (context.Record is IIngestibleGetter record)
            {
                var effectActions = rule.GetValueAs<List<GSPRule.EffectAction>>(valueKey);
                if (rule.OnlyIfDefault && origin != null && origin is IIngestibleGetter originIG && !record.Effects.SequenceEqualNullable(originIG.Effects))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Skipping as effects don't match origin");
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
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IIngestibleGetter", context.Record.GetType().Name, 0x412);
            }

            if (patch != null)
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");
            return patch != null;
        }

        // Log Codes: 0x42x
        public static bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd )
        {
            if (context.Record is IIngestibleGetter record && forwardContext.Record is IIngestibleGetter forward)
            {
                if (forward.Effects.SequenceEqualNullable(record.Effects))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Skipping as already matches forwarding record");
                    return false;
                }

                if (rule.OnlyIfDefault && origin != null && origin is IIngestibleGetter originGetter && !record.Effects.SequenceEqualNullable(originGetter.Effects))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Skipping as keywords don't match origin");
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

                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");
                return true;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IIngestibleGetter", context.Record.GetType().Name, 0x422);
            return false;
        }
    }
}