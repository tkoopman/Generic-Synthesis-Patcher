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

using static NexusMods.Paths.Delegates;

namespace GenericSynthesisPatcher.Helpers.Action
{
    // Log Codes: 0x5xx
    internal static class FormLink
    {
        // 0x51x
        private static bool GetFormLinks<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, string propertyName, out T? value ) where T : class, IEnumerable<IFormLinkGetter<IMajorRecordGetter>>
        {
            value = null;
            var property = record.GetType().GetProperty(propertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, propertyName, $"Failed to find property. Skipping.", 0x511);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null)
                return true;

            if (_value is not T __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, typeof(T).Name, _value.GetType().Name, 0x512);
                return false;
            }

            value = __value;

            return true;
        }

        // Log Codes: 0x52x
        public static bool FillFormLinks<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, string propertyName ) where T : class, IMajorRecordGetter
        {
            IFormLinkContainerGetter? patch = null;

            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, context.Record, propertyName, out var curValue))
                    return false;

                if (rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, origin, propertyName, out var originValue))
                    {
                        if (!curValue.SequenceEqualNullable(originValue))
                        {
                            LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as property doesn't match origin");
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find origin property to check.", 0x521);
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
                        var setProperty = patch.GetType().GetProperty(propertyName);
                        if (setProperty == null)
                        {
                            LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find property to set new value to.", 0x522);
                            return false;
                        }

                        object? _setList = setProperty.GetValue(patch);
                        if (_setList == null)
                        {
                            LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find property to set new value to.", 0x523);
                            return false;
                        }

                        if (_setList is not List<IFormLinkGetter<T>> setList)
                        {
                            LogHelper.LogInvalidTypeFound(LogLevel.Error, context, propertyName, "List", _setList.GetType().Name, 0x524);
                            return false;
                        }

                        if (!Global.State.LinkCache.TryResolve(actionKey, typeof(T), out var link))
                        {
                            LogHelper.Log(LogLevel.Warning, context, propertyName, $"Unable to find {actionKey}");
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
                    LogHelper.Log(LogLevel.Debug, context, propertyName, "Updated.");
            }
            else
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x525);
            }

            return patch != null;
        }

        // Log Codes: 0x53x
        public static bool ForwardFormLinks<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IMajorRecordGetter forwardRecord, string propertyName ) where T : class, IMajorRecordGetter
        {
            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, context.Record, propertyName, out var curValue))
                    return false;

                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, forwardRecord, propertyName, out var newValue))
                    return false;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as already matches forwarding record");
                    return false;
                }

                if (rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, origin, propertyName, out var originValue))
                    {
                        if (!curValue.SequenceEqualNullable(originValue))
                        {
                            LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as property doesn't match origin");
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find origin property to check.", 0x531);
                        return false;
                    }
                }

                var patch = context.GetOrAddAsOverride(Global.State.PatchMod);

                if (!GetFormLinks<IReadOnlyList<IFormLinkGetter<T>>>(context, patch, propertyName, out var patchValue) || patchValue is not ExtendedList<IFormLinkGetter<T>> patchValueLinks)
                    return false;

                _ = patchValueLinks.RemoveAll(_ => true);

                if (newValue != null)
                    patchValueLinks.AddRange(newValue);

                LogHelper.Log(LogLevel.Debug, context, propertyName, "Updated.");
                return true;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x535);

            return false;
        }

        // 0x54x
        private static bool GetFormLink<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, string propertyName, out T? value ) where T : class, IFormLinkGetter<IMajorRecordGetter>
        {
            value = null;
            var property = record.GetType().GetProperty(propertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, propertyName, $"Failed to find property. Skipping.", 0x541);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null)
                return true;

            if (_value is not T __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, typeof(T).Name, _value.GetType().Name, 0x542);
                return false;
            }

            value = __value;

            return true;
        }

        // Log Codes: 0x55x
        private static bool FillFormLink<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, string propertyName, IFormLinkGetter<T>? curValue, IFormLinkGetter<T>? newValue ) where T : class, IMajorRecordGetter
        {
            if (curValue != null && newValue != null && curValue.FormKey.Equals(newValue.FormKey))
            {
                LogHelper.Log(LogLevel.Trace, context, propertyName, "Skipping as property already matching");
                return false;
            }

            if (rule.OnlyIfDefault && origin != null)
            {
                if (GetFormLink<IFormLinkGetter<T>>(context, origin, propertyName, out var originValue))
                {
                    if (!(curValue != null && originValue != null)
                        && ((curValue == null && originValue != null)
                             || (curValue != null && originValue == null)
                             || (!curValue?.FormKey.Equals(originValue?.FormKey) ?? throw new Exception("Impossible???"))))
                    {
                        LogHelper.Log(LogLevel.Debug, context, propertyName, "Skipping as property doesn't match origin");
                        return false;
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find origin property to check.", 0x551);
                    return false;
                }
            }

            var patch = context.GetOrAddAsOverride(Global.State.PatchMod);
            var setProperty = patch.GetType().GetProperty(propertyName);
            if (setProperty == null)
            {
                LogHelper.Log(LogLevel.Error, context, propertyName, $"Unable to find property to set new value to.", 0x552);
                return false;
            }

            setProperty.SetValue(patch, newValue);
            LogHelper.Log(LogLevel.Debug, context, propertyName, "Updated.");

            return true;
        }

        // Log Codes: 0x56x
        public static bool FillFormLink<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, string propertyName ) where T : class, IMajorRecordGetter
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!GetFormLink<IFormLinkGetter<T>>(context, context.Record, propertyName, out var curValue))
                {
                    LogHelper.Log(LogLevel.Debug, context, propertyName, $"Unable to get current value");
                    return false;
                }

                var newValue = rule.GetValueAs<FormKey>(valueKey);
                if (!Global.State.LinkCache.TryResolve(newValue, typeof(T), out var link))
                {
                    LogHelper.Log(LogLevel.Warning, context, propertyName, $"Unable to find {newValue}");
                    return false;
                }

                return FillFormLink(context, origin, rule, propertyName, curValue, link.ToLinkGetter<T>());
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x561);
            return false;
        }

        // Log Codes: 0x57x
        public static bool ForwardFormLink<T> ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IMajorRecordGetter forwardRecord, string propertyName ) where T : class, IMajorRecordGetter
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!GetFormLink<IFormLinkGetter<T>>(context, context.Record, propertyName, out var curValue))
                    return false;

                if (!GetFormLink<IFormLinkGetter<T>>(context, forwardRecord, propertyName, out var newValue))
                    return false;

                return FillFormLink(context, origin, rule, propertyName, curValue, newValue);
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, propertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x571);
            return false;
        }
    }
}