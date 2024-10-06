using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

using static GenericSynthesisPatcher.Program;
using static NexusMods.Paths.Delegates;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x5xx
    public class FormLink<T> : IAction where T : class, IMajorRecordGetter
    {
        // 0x54x
        private static bool GetFormLink<LT> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out LT? value ) where LT : class, IFormLinkGetter<IMajorRecordGetter>
        {
            value = null;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"Failed to find property. Skipping.", 0x541);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null)
                return true;

            if (_value is not LT __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, typeof(LT).Name, _value.GetType().Name, 0x542);
                return false;
            }

            value = __value;

            return true;
        }

        // Log Codes: 0x55x
        private static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, RecordCallData rcd, IFormLinkGetter<T>? curValue, IFormLinkGetter<T>? newValue )
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                LogHelper.Log(LogLevel.Trace, context, rcd.PropertyName, "Skipping as property already matching");
                return false;
            }

            if (rule.OnlyIfDefault && origin != null)
            {
                if (GetFormLink<IFormLinkGetter<T>>(context, origin, rcd, out var originValue))
                {
                    if (!(curValue != null && originValue != null)
                        && ((curValue == null && originValue != null)
                             || (curValue != null && originValue == null)
                             || (!curValue?.FormKey.Equals(originValue?.FormKey) ?? throw new Exception("Impossible???"))))
                    {
                        LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Skipping as property doesn't match origin");
                        return false;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, $"Unable to find origin property to check.", 0x551);
                    return false;
                }
            }

            var patch = context.GetOrAddAsOverride(Global.State.PatchMod);
            var setProperty = patch.GetType().GetProperty(rcd.PropertyName);
            if (setProperty == null)
            {
                LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, $"Unable to find property to set new value to.", 0x552);
                return false;
            }

            setProperty.SetValue(patch, newValue);
            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");

            return true;
        }

        // Log Codes: 0x56x
        public static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!GetFormLink<IFormLinkGetter<T>>(context, context.Record, rcd, out var curValue))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"Unable to get current value");
                    return false;
                }

                var newValue = rule.GetValueAs<FormKey>(valueKey);
                if (!Global.State.LinkCache.TryResolve(newValue, typeof(T), out var link))
                {
                    LogHelper.Log(LogLevel.Warning, context, rcd.PropertyName, $"Unable to find {newValue}");
                    return false;
                }

                return Fill(context, origin, rule, rcd, curValue, link.ToLinkGetter<T>());
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x561);
            return false;
        }

        // Log Codes: 0x57x
        public static bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!GetFormLink<IFormLinkGetter<T>>(context, context.Record, rcd, out var curValue))
                    return false;

                if (!GetFormLink<IFormLinkGetter<T>>(context, forwardContext.Record, rcd, out var newValue))
                    return false;

                return Fill(context, origin, rule, rcd, curValue, newValue);
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x571);
            return false;
        }
        public static bool CanFill () => true;
        public static bool CanForward () => true;
    }
}