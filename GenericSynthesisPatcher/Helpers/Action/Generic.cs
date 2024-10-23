using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    internal class Generic<T> : IAction where T : IConvertible
    {
        private const int ClassLogCode = 0x10;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => false;

        public static bool CanMerge () => false;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (!Mod.GetProperty<T>(patchRecord ?? context.Record, rcd.PropertyName, out var curValue))
                return -1;

            var newValue = rule.GetFillValueAs<T>(valueKey);

            return Fill(context, rcd, curValue, newValue, ref patchRecord);
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
                    => (Mod.GetProperty<T>(patchRecord ?? context.Record, rcd.PropertyName, out var curValue)
                     && Mod.GetProperty<T>(forwardContext.Record, rcd.PropertyName, out var newValue)) ?
                        Fill(context, rcd, curValue, newValue, ref patchRecord)
                        : -1;

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord ) => throw new NotImplementedException();

        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, FilterOperation valueKey, RecordCallData rcd )
        {
            var values = rule.GetMatchValueAs<List<ListOperation<T>>>(valueKey);

            if (!values.SafeAny())
                return true;

            if (!Mod.GetProperty<T>(check, rcd.PropertyName, out var curValue) || curValue == null)
                return !values.Any(k => k.Operation != ListLogic.NOT);

            foreach (var v in values)
            {
                if (v is string str && str.StartsWith('/') && str.EndsWith('/') && curValue is string curStr)
                {
                    if (new Regex(str.Trim('/'), RegexOptions.IgnoreCase).IsMatch(curStr))
                        return v.Operation != ListLogic.NOT;
                }
                else if (curValue.Equals(v.Value))
                {
                    if (v.Operation == ListLogic.NOT)
                        return v.Operation != ListLogic.NOT;
                }
            }

            return !values.Any(k => k.Operation != ListLogic.NOT);
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => origin != null
                && Mod.GetProperty<T>(check, rcd.PropertyName, out var curValue)
                && Mod.GetProperty<T>(origin, rcd.PropertyName, out var originValue)
                && !(curValue == null ^ originValue == null)
                && !(curValue != null ^ originValue != null)
                && ((curValue == null && originValue == null)
                   || (curValue != null && originValue != null && curValue.Equals(originValue)));

        public static int Merge ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord ) => throw new NotImplementedException();

        private static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, T? curValue, T? newValue, ref ISkyrimMajorRecord? patchRecord )
        {
            if (curValue == null && newValue == null)
                return 0;
            if (curValue != null && newValue != null && curValue.Equals(newValue))
                return 0;

            patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);

            if (!Mod.SetProperty(patchRecord, rcd.PropertyName, newValue))
                return -1;

            LogHelper.Log(LogLevel.Debug, ClassLogCode, "Changed.", context: context, propertyName: rcd.PropertyName);
            return 1;
        }
    }
}