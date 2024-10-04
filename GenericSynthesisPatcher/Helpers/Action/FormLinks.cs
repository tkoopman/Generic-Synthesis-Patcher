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
    public class FormLinks<T>: IAction where T : class, IMajorRecordGetter
    {
        // 0x51x
        private static bool GetFormLinks<LT> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out LT? value ) where LT : class, IEnumerable<IFormLinkGetter<IMajorRecordGetter>>
        {
            value = null;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, $"Failed to find property. Skipping.", 0x511);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null)
                return true;

            if (_value is not LT __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, typeof(LT).Name, _value.GetType().Name, 0x512);
                return false;
            }

            value = __value;

            return true;
        }

        // Log Codes: 0x52x
        public static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd )
        {
            IFormLinkContainerGetter? patch = null;

            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, context.Record, rcd, out var curValue))
                    return false;

                if (rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, origin, rcd, out var originValue))
                    {
                        if (!curValue.SequenceEqualNullable(originValue))
                        {
                            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Skipping as property doesn't match origin");
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, $"Unable to find origin property to check.", 0x521);
                        return false;
                    }
                }

                foreach (string c in rule.GetValueAs<List<string>>(valueKey) ?? [])
                {
                    bool actionRemove = c.StartsWith('-');
                    var actionKey = FormKey.Factory(actionRemove || c.StartsWith('+') ? c[1..] : c);
                    var curKey = curValue?.SingleOrDefault(flg => flg?.FormKey.Equals(actionKey) ?? false, null);

                    if ((curKey != null && actionRemove) || (curKey == null && !actionRemove))
                    {
                        patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                        var setProperty = patch.GetType().GetProperty(rcd.PropertyName);
                        if (setProperty == null)
                        {
                            LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, $"Unable to find property to set new value to.", 0x522);
                            return false;
                        }

                        object? _setList = setProperty.GetValue(patch);
                        if (_setList == null)
                        {
                            LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, $"Unable to find property to set new value to.", 0x523);
                            return false;
                        }

                        if (_setList is not List<IFormLinkGetter<T>> setList)
                        {
                            LogHelper.LogInvalidTypeFound(LogLevel.Error, context, rcd.PropertyName, "List", _setList.GetType().Name, 0x524);
                            return false;
                        }

                        if (!Global.State.LinkCache.TryResolve(actionKey, typeof(T), out var link))
                        {
                            LogHelper.Log(LogLevel.Warning, context, rcd.PropertyName, $"Unable to find {actionKey}");
                        }
                        else if (actionRemove)
                        {
                            _ = setList.Remove(link.ToLinkGetter<T>());
                        }
                        else
                        {
                            setList.Add(link.ToLinkGetter<T>());
                        }
                    }
                }

                if (patch != null)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");
            }
            else
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x525);
            }

            return patch != null;
        }

        // Log Codes: 0x53x
        public static bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IMajorRecordGetter forwardRecord, RecordCallData rcd )
        {
            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, context.Record, rcd, out var curValue))
                    return false;

                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, forwardRecord, rcd, out var newValue))
                    return false;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Skipping as already matches forwarding record");
                    return false;
                }

                if (rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, origin, rcd, out var originValue))
                    {
                        if (!curValue.SequenceEqualNullable(originValue))
                        {
                            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Skipping as property doesn't match origin");
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, $"Unable to find origin property to check.", 0x531);
                        return false;
                    }
                }

                var patch = context.GetOrAddAsOverride(Global.State.PatchMod);

                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, patch, rcd, out var patchValue) || patchValue is not ExtendedList<IFormLinkGetter<T>> patchValueLinks)
                    return false;

                _ = patchValueLinks.RemoveAll(_ => true);

                if (newValue != null)
                    patchValueLinks.AddRange(newValue);

                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");
                return true;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x535);

            return false;
        }
        public static bool CanFill () => true;
        public static bool CanForward () => true;
    }
}