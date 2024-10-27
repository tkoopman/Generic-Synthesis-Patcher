using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLinksWithData<T, TMajor> : IRecordAction
        where T : class, IFormLinksWithData<T, TMajor>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        public static readonly FormLinksWithData<T, TMajor> Instance = new();
        private const int ClassLogCode = 0x15;

        private FormLinksWithData ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => true;

        public bool CanMerge () => T.CanMerge();

        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
                    return -1;

                int changes = 0;

                foreach (var action in T.GetFillValueAs(proKeys) ?? [])
                {
                    if (action?.FormKey == null || action.FormKey.Value == FormKey.Null)
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

                    var e = action.FindFormKey(curList);

                    if (e != null && (action.FormKey.Operation == ListLogic.DEL || !action.DataEquals(e)))
                    {
                        _ = T.Remove(proKeys, e);
                        changes++;
                    }

                    if (action.FormKey.Operation == ListLogic.ADD && (e == null || (e != null && !action.DataEquals(e))))
                    {
                        _ = action.Add(proKeys);
                        changes++;
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
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(forwardContext.Record, proKeys.Property.PropertyName, out var newList))
                    return -1;

                if (curList.SequenceEqualNullable(newList))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
                    return 0;
                }

                int changes = T.Replace(proKeys, newList);

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
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
                    return -1;

                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(forwardContext.Record, proKeys.Property.PropertyName, out var newList))
                    return -1;

                if (!newList.SafeAny())
                    return 0;

                if (curList.SequenceEqualNullable(newList))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
                    return 0;
                }

                int changes = 0;
                foreach (var item in newList)
                {
                    var key = T.GetFormKeyFromRecord(item);
                    if (key.ModKey == forwardContext.ModKey)
                    {
                        var i = T.FindRecord(curList, key);

                        if (i != null && !T.DataEquals(item, i))
                        {
                            _ = T.Remove(proKeys, i);
                            _ = T.Forward(proKeys, item);
                            changes++;
                        }

                        if (i == null)
                        {
                            _ = T.Forward(proKeys, item);
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

        /// <summary>
        /// Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if all form keys and data matches</returns>
        public bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curList)
                        && Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(origin, proKeys.Property.PropertyName, out var originList)
                        && FormLinksWithData<T, TMajor>.RecordsMatch(curList, originList);
        }

        /// <summary>
        /// Only checks the FormKeys not the Data
        /// </summary>
        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (proKeys is not IFormLinkContainerGetter)
                return false;

            var links = proKeys.GetMatchValueAs<List<FormKeyListOperation<TMajor>>>();

            if (!links.SafeAny())
                return true;

            if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curLinks) || !curLinks.SafeAny())
                return !links.Any(k => k.Operation != ListLogic.NOT);

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            foreach (var link in links)
            {
                if (link.Operation != ListLogic.NOT)
                    includesChecked++;

                if (T.FindRecord(curLinks, link.Value) != null)
                {
                    // Doesn't matter what overall Operation is we always fail on a NOT match
                    if (link.Operation == ListLogic.NOT)
                        return false;

                    if (proKeys.RuleKey.Operation == FilterLogic.OR)
                        return true;

                    matchedCount++;
                }
                else if (link.Operation != ListLogic.NOT && proKeys.RuleKey.Operation == FilterLogic.AND)
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

        public int Merge (ProcessingKeys proKeys) => T.Merge(proKeys);

        private static bool RecordsMatch (IReadOnlyList<IFormLinkContainerGetter>? left, IReadOnlyList<IFormLinkContainerGetter>? right)
        {
            if (!left.SafeAny() && !right.SafeAny())
                return true;

            if (!left.SafeAny() || !right.SafeAny() || left.Count != right.Count)
                return false;

            foreach (var l in left)
            {
                if (!right.Any(r => T.DataEquals(l, r)))
                    return false;
            }

            return true;
        }
    }
}