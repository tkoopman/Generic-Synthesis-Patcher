using GenericSynthesisPatcher.Helpers.Graph;
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
    public class FormLinks<T> : IRecordAction
        where T : class, IMajorRecordGetter
    {
        public static readonly FormLinks<T> Instance = new();
        private const int ClassLogCode = 0x14;

        private FormLinks ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => true;

        public bool CanMerge () => true;

        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
                    return -1;

                int changes = 0;
                foreach (var actionKey in proKeys.GetFillValueAs<List<FormKeyListOperation<T>>>() ?? [])
                {
                    if (actionKey.Value == FormKey.Null)
                    {
                        if (curList != null && curList.Count > 0)
                        {
                            if (!Mod.ClearProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName))
                            {
                                Global.Logger.Log(ClassLogCode, LogHelper.MissingProperty, logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                                return -1;
                            }

                            curList = [];
                            changes++;
                        }

                        continue;
                    }

                    var curKey = curList?.SingleOrDefault(k => k?.FormKey.Equals(actionKey.Value) ?? false, null);

                    if ((curKey != null && actionKey.Operation == ListLogic.DEL) || (curKey == null && actionKey.Operation == ListLogic.ADD))
                    {
                        if (!Mod.GetPropertyForEditing<List<IFormLinkGetter<T>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var setList))
                        {
                            Global.Logger.Log(ClassLogCode, LogHelper.MissingProperty, logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                            return -1;
                        }

                        if (!Global.State.LinkCache.TryResolve(actionKey.Value, typeof(T), out var link))
                        {
                            Global.Logger.Log(ClassLogCode, $"Unable to find {actionKey}", logLevel: LogLevel.Warning, propertyName: proKeys.Property.PropertyName);
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
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: proKeys.Property.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);
            return -1;
        }

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue))
                    return -1;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
                    return 0;
                }

                if (!Mod.GetPropertyForEditing<ExtendedList<IFormLinkGetter<T>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var patchValue))
                {
                    Global.Logger.Log(ClassLogCode, "Patch has null value.", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                    return -1;
                }

                int changes = patchValue.RemoveAll(_ => true);

                if (newValue != null)
                {
                    patchValue.AddRange(newValue);
                    changes += newValue.Count;
                }

                if (changes > 0)
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: proKeys.Property.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);

            return -1;
        }

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue))
                    return -1;

                if (!newValue.SafeAny())
                    return 0;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
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
                                if (!Mod.GetPropertyForEditing<ExtendedList<IFormLinkGetter<T>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var patchValue))
                                    return -1;

                                patchValueLinks = patchValue;
                            }

                            patchValueLinks.Add(item);
                            changes++;
                        }
                    }
                }

                if (changes > 0)
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: proKeys.Property.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);

            return -1;
        }

        public bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(proKeys.Context.Record, proKeys.Property.PropertyName, out var checkValue)
                        && Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(origin, proKeys.Property.PropertyName, out var originValue)
                        && FormLinks<T>.RecordsMatch(checkValue, originValue);
        }

        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (proKeys.Record is not IFormLinkContainerGetter)
                return false;

            var values = proKeys.GetMatchValueAs<List<FormKeyListOperationAdvanced<T>>>();

            if (!values.SafeAny())
                return true;

            if (!Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue) || !curValue.SafeAny())
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

                    if (proKeys.RuleKey.Operation == FilterLogic.OR)
                        return true;

                    matchedCount++;
                }
                else if (v.Operation != ListLogic.NOT && proKeys.RuleKey.Operation == FilterLogic.AND)
                {
                    return false;
                }
            }

            return proKeys.RuleKey.Operation switch
            {
                FilterLogic.AND => true,
                FilterLogic.XOR => matchedCount == 1,
                _ => includesChecked == 0 // OR
            };
        }

        public int Merge (ProcessingKeys proKeys)
        {
            Global.UpdateLoggers(ClassLogCode);

            var root = RecordGraph<IFormLinkGetter<T>>.Create(
                proKeys.Record.FormKey,
                proKeys.Type.StaticRegistration.GetterType,
                proKeys.Rule.Merge[proKeys.RuleKey],
                list => Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(list.Record, proKeys.Property.PropertyName, out var value) ? value : null,
                item => $"{item.FormKey}");

            if (root == null)
            {
                Global.Logger.Log(ClassLogCode, "Failed to generate graph for merge", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            return root.Merge(out var newList) ? Replace(proKeys, newList) : 0;
        }

        public int Replace (ProcessingKeys proKeys, IEnumerable<IFormLinkGetter<T>>? _newList)
        {
            if (_newList is not IReadOnlyList<IFormLinkGetter<T>> newList || !Mod.GetProperty<IReadOnlyList<IFormLinkGetter<T>>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
            {
                Global.Logger.Log(ClassLogCode, "Failed to replace entries", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            try
            {
                if (!Mod.GetPropertyForEditing<ExtendedList<IFormLinkGetter<T>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var list))
                    return -1;

                foreach (var d in del)
                    _ = list.Remove(d);

                foreach (var a in add)
                    list.Add(a);
            }
            catch (RecordException ex)
            {
                Global.Logger.Log(ClassLogCode, ex.Message, logLevel: LogLevel.Critical, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            return add.Count() + del.Count();
        }

        private static bool RecordsMatch (IReadOnlyList<IFormLinkGetter<T>>? left, IReadOnlyList<IFormLinkGetter<T>>? right)
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