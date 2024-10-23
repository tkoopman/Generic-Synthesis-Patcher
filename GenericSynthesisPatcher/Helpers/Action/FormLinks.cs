using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLinks<T> : IAction
        where T : class, IMajorRecordGetter
    {
        private const int ClassLogCode = 0x14;

        public static bool CanFill () => true;

        public static bool CanForward () => true;

        public static bool CanForwardSelfOnly () => true;

        public static bool CanMerge () => true;

        public static int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(patchRecord ?? context.Record, rcd.PropertyName, out var curValue))
                    return -1;

                int changes = 0;
                foreach (var actionKey in rule.GetFillValueAs<List<FormKeyListOperation<T>>>(valueKey) ?? [])
                {
                    var curKey = curValue?.SingleOrDefault(k => k?.FormKey.Equals(actionKey.Value) ?? false, null);

                    if ((curKey != null && actionKey.Operation == ListLogic.DEL) || (curKey == null && actionKey.Operation == ListLogic.ADD))
                    {
                        patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);

                        if (!Mod.GetPropertyForEditing<List<IFormLinkGetter<T>>>(patchRecord, rcd.PropertyName, out var setList))
                        {
                            LogHelper.Log(LogLevel.Error, ClassLogCode, LogHelper.MissingProperty, rule: rule, context: context, propertyName: rcd.PropertyName);
                            return -1;
                        }

                        if (!Global.State.LinkCache.TryResolve(actionKey.Value, typeof(T), out var link))
                        {
                            LogHelper.Log(LogLevel.Warning, ClassLogCode, $"Unable to find {actionKey}", rule: rule, context: context, propertyName: rcd.PropertyName);
                            continue;
                        }

                        changes++;
                        if (actionKey.Operation == ListLogic.DEL)
                            _ = setList.Remove(link.ToLinkGetter<T>());
                        else
                            setList.Add(link.ToLinkGetter<T>());
                    }
                }

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, ClassLogCode, $"{changes} change(s).", rule: rule, context: context, propertyName: rcd.PropertyName);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, ClassLogCode, rule, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name);
            return -1;
        }

        public static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (context.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(patchRecord ?? context.Record, rcd.PropertyName, out var curValue))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(forwardContext.Record, rcd.PropertyName, out var newValue))
                    return -1;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    LogHelper.Log(LogLevel.Debug, ClassLogCode, LogHelper.PropertyIsEqual, rule: rule, context: context, propertyName: rcd.PropertyName);
                    return 0;
                }

                patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);

                if (!Mod.GetPropertyForEditing<ExtendedList<IFormLinkGetter<T>>>(patchRecord, rcd.PropertyName, out var patchValue))
                {
                    LogHelper.Log(LogLevel.Error, ClassLogCode, "Patch has null value.", rule: rule, context: context, propertyName: rcd.PropertyName);
                    return -1;
                }

                int changes = patchValue.RemoveAll(_ => true);

                if (newValue != null)
                {
                    patchValue.AddRange(newValue);
                    changes += newValue.Count;
                }

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, ClassLogCode, $"{changes} change(s).", rule: rule, context: context, propertyName: rcd.PropertyName);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, ClassLogCode, rule, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name);

            return -1;
        }

        public static int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            if (context.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(patchRecord ?? context.Record, rcd.PropertyName, out var curValue))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(forwardContext.Record, rcd.PropertyName, out var newValue))
                    return -1;

                if (!newValue.SafeAny())
                    return 0;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: rcd.PropertyName);
                    return 0;
                }

                ExtendedList<IFormLinkGetter<T>>? patchValueLinks = null;
                int changes = 0;

                foreach (var item in newValue)
                {
                    if (item.FormKey.ModKey == forwardContext.ModKey)
                    {
                        if (curValue == null || !curValue.Contains(item))
                        {
                            if (patchValueLinks == null)
                            {
                                patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                                if (!Mod.GetPropertyForEditing<ExtendedList<IFormLinkGetter<T>>>(patchRecord, rcd.PropertyName, out var patchValue))
                                    return -1;

                                patchValueLinks = patchValue;
                            }

                            patchValueLinks.Add(item);
                            changes++;
                        }
                    }
                }

                if (changes > 0)
                    LogHelper.Log(LogLevel.Debug, ClassLogCode, $"{changes} change(s).", rule: rule, context: context, propertyName: rcd.PropertyName);

                return changes;
            }

            LogHelper.LogInvalidTypeFound(LogLevel.Debug, ClassLogCode, rule, context, rcd.PropertyName, "IFormLinkContainerGetter", context.Record.GetType().Name);

            return -1;
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, FilterOperation valueKey, RecordCallData rcd )
        {
            if (check is not IFormLinkContainerGetter)
                return false;

            var values = rule.GetMatchValueAs<List<FormKeyListOperationAdvanced<T>>>(valueKey);

            if (!values.SafeAny())
                return true;

            if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(check, rcd.PropertyName, out var curValue) || !curValue.SafeAny())
                return !values.Any(k => k.Operation != ListLogic.NOT);

            List<string> EditorIDs = [];
            if (values.Any(v => v.Regex != null))
            {
                foreach (var v in curValue)
                {
                    var link = v.TryResolve(Global.State.LinkCache);
                    if (link != null && link.EditorID != null)
                        EditorIDs.Add(link.EditorID);
                }
            }

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            foreach (var v in values)
            {
                if (v.Operation != ListLogic.NOT)
                    includesChecked++;

                bool match = (v.Regex != null)
                    ? EditorIDs.Any(v.Regex.IsMatch)
                    : curValue.Any(l => l.FormKey.Equals(v.Value));

                if (match)
                {
                    // Doesn't matter what overall Operation is we always fail on a NOT match
                    if (v.Operation == ListLogic.NOT)
                        return false;

                    if (valueKey.Operation == FilterLogic.OR)
                        return true;

                    matchedCount++;
                }
                else if (v.Operation != ListLogic.NOT && valueKey.Operation == FilterLogic.AND)
                {
                    return false;
                }
            }

            return valueKey.Operation switch
            {
                FilterLogic.AND => true,
                FilterLogic.XOR => matchedCount == 1,
                _ => includesChecked == 0 // OR
            };
        }

        public static bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd )
                => origin != null
                && Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(check, rcd.PropertyName, out var checkValue)
                && Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(origin, rcd.PropertyName, out var originValue)
                && RecordsMatch(checkValue, originValue);

        public static int Merge ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            _ = Global.UpdateTrace(ClassLogCode);

            var root = RecordGraph<IFormLinkGetter<T>>.Create(
                context.Record.FormKey,
                context.Record.Registration.GetterType,
                rule.Merge[valueKey],
                list => Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(list.Record, rcd.PropertyName, out var value) ? value : null,
                item => $"{item.FormKey}");

            if (root == null)
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, "Failed to generate graph for merge", rule: rule, context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            return root.Merge(out var newList) ? Replace(context, rcd, ref patchRecord, newList) : 0;
        }

        public static int Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord, IEnumerable<IFormLinkGetter<T>>? _newList )
        {
            if (_newList is not IReadOnlyList<IFormLinkGetter<T>> newList || !Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(patchRecord ?? context.Record, rcd.PropertyName, out var curList))
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, "Failed to replace entries", context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            try
            {
                patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
                if (!Mod.GetPropertyForEditing<ExtendedList<IFormLinkGetter<T>>>(patchRecord, rcd.PropertyName, out var list))
                    return -1;

                foreach (var d in del)
                    _ = list.Remove(d);

                foreach (var a in add)
                    list.Add(a);
            }
            catch (RecordException ex)
            {
                LogHelper.Log(LogLevel.Critical, ClassLogCode, ex.Message, context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            return add.Count() + del.Count();
        }

        private static bool RecordsMatch ( IReadOnlyList<IFormLinkGetter<T>>? left, IReadOnlyList<IFormLinkGetter<T>>? right )
        {
            if (!left.SafeAny() && !right.SafeAny())
                return true;

            if (!left.SafeAny() || !right.SafeAny() || left.Count != right.Count)
                return false;

            foreach (var l in left)
            {
                if (!right.Any(r => l.FormKey.Equals(r.FormKey)))
                    return false;
            }

            return true;
        }
    }
}