using EnumsNET;

using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLinksWithData<T> : IAction
        where T : class, IFormLinksWithData<T>
    {
        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd )
        {
            IMajorRecordGetter? patch = null;

            if (context.Record is IFormLinkContainerGetter)
            {
                if (!GetFormLinks(context, context.Record, rcd, out var curList))
                    return false;

                if (rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks(context, origin, rcd, out var originValue))
                    {
                        if (!curList.SequenceEqualNullable(originValue))
                        {
                            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch);
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, 0x821);
                        return false;
                    }
                }

                foreach (var action in T.GetValueAs(rule, valueKey) ?? [])
                {
                    var e = action.Find(curList);

                    if (e != null && (action.FormKey.Neg || !action.DataEquals(e)))
                        _ = T.Remove(context, ref patch, e);

                    if (!action.FormKey.Neg && (e == null || (e != null && !action.DataEquals(e))))
                        _ = action.Add(context, ref patch);
                }

                if (patch == null)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");
            }
            else
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x825);
            }

            return patch != null;
        }

        // Log Codes: 0x83x
        public static bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd )
        {
            IMajorRecordGetter? patch = null;

            if (context.Record is IFormLinkContainerGetter)
            {
                if (!GetFormLinks(context, context.Record, rcd, out var curList))
                    return false;

                if (!GetFormLinks(context, forwardContext.Record, rcd, out var newList))
                    return false;

                if (curList.SequenceEqualNullable(newList))
                {
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.PropertyIsEqual);
                    return false;
                }

                if (rule.OnlyIfDefault && origin != null)
                {
                    if (GetFormLinks(context, origin, rcd, out var orgList))
                    {
                        if (!curList.SequenceEqualNullable(orgList))
                        {
                            LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.OriginMismatch);
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, context, rcd.PropertyName, LogHelper.MissingProperty, 0x831);
                        return false;
                    }
                }

                if (rule.ForwardType.GetFlags().Contains(ForwardTypes.SelfMasterOnly))
                {
                    if (newList == null)
                        return false;

                    foreach (var item in newList)
                    {
                        var key = T.GetFormKey(item);
                        if (key.ModKey == forwardContext.ModKey)
                        {
                            var i = T.Find(curList, key);
                            bool eq = i != null && T.DataEquals(item, i);

                            if (i != null && !eq)
                            {
                                _ = T.Remove(context, ref patch, i);
                                _ = T.Add(context, ref patch, item);
                            }

                            if (i == null)
                                _ = T.Add(context, ref patch, item);
                        }
                    }
                }
                else
                {
                    _ = newList != null && T.Replace(context, ref patch, newList);
                }

                if (patch == null)
                    LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, "Updated.");

                return patch != null;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name, 0x835);

            return false;
        }

        // Log Codes: 0x81x
        public static bool GetFormLinks ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter record, RecordCallData rcd, out IReadOnlyList<IFormLinkContainerGetter>? value )
        {
            value = null;
            var property = record.GetType().GetProperty(rcd.PropertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, context, rcd.PropertyName, LogHelper.MissingProperty, 0x811);
                return false;
            }

            object? _value = property.GetValue(record);
            if (_value == null)
                return true;

            if (_value is not IReadOnlyList<IFormLinkContainerGetter> __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, context, rcd.PropertyName, "IReadOnlyList<IFormLinkContainerGetter>", _value.GetType().FullName ?? _value.GetType().Name, 0x812);
                return false;
            }

            value = __value;
            return true;
        }
    }
}