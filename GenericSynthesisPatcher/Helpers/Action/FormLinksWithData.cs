using GenericSynthesisPatcher.Json.Data.Action;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public abstract class FormLinksWithData<TActionData, TMajor, TData> : IRecordAction
        where TActionData : ActionDataBase<TMajor, TData>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        private const int ClassLogCode = 0x15;

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => true;

        #region Abstract

        /// <summary>
        /// Add entry as detailed by this IFormLinksWithData object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rcd"></param>
        /// <param name="patchRecord"></param>
        /// <returns>Number of changes made to add entry. Should be 1 if successful else 0. -1 if major failure.</returns>
        public abstract int Add (ProcessingKeys proKeys, TActionData data);

        /// <summary>
        /// Can this be used in merge actions
        /// </summary>
        /// <returns>True if Merge implemented</returns>
        public abstract bool CanMerge ();

        /// <summary>
        /// Check if 2 links and data are equal
        /// </summary>
        /// <returns>True only if both FormKey and any data match across both entries.</returns>
        public abstract bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right);

        /// <summary>
        /// Find record from list of records.
        /// </summary>
        /// <param name="list">List to search</param>
        /// <param name="key">FormKey to find</param>
        /// <returns>Getter of found entry or null if not found.</returns>
        public abstract IFormLinkContainerGetter? FindRecord (IEnumerable<IFormLinkContainerGetter>? list, FormKey key);

        /// <summary>
        /// Add source entry to patch record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="patchRecord"></param>
        /// <param name="source">Entry to add to list in patch record.</param>
        /// <returns>Number of changes made to add entry. Should be 1 if successful else 0. -1 if major failure.</returns>
        public abstract int Forward (ProcessingKeys proKeys, IFormLinkContainerGetter source);

        /// <summary>
        /// Get data from JSON value in rule.
        /// </summary>
        /// <param name="rule">Rule to get data from</param>
        /// <param name="key">Key to current action in rule</param>
        /// <returns>List of all data values for action.</returns>
        public abstract List<TActionData>? GetFillValueAs (ProcessingKeys proKeys);

        /// <summary>
        /// Get FormKey of entry.
        /// </summary>
        /// <param name="from">Form Link to get FormKey from</param>
        /// <returns>FormKey</returns>
        public abstract FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from);

        /// <summary>
        /// Preform merge of current field in current record.
        /// </summary>
        /// <returns>Number of changes to complete merge. Each entry removed / added counts as 1 change.</returns>
        public abstract int Merge (ProcessingKeys proKeys);

        /// <summary>
        /// Remove link entry from current record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rcd"></param>
        /// <param name="patchRecord"></param>
        /// <param name="remove"></param>
        /// <returns>Number of changes made to remove entry. Should be 1 if successful else 0. -1 if major failure.</returns>
        public abstract int Remove (ProcessingKeys proKeys, IFormLinkContainerGetter remove);

        /// <summary>
        /// Replace list of links in current record with new list.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rcd"></param>
        /// <param name="patchRecord"></param>
        /// <param name="newList">Patch record list should match this list after replace.</param>
        /// <returns>Number of changes to complete replace. Each entry removed / added counts as 1 change.</returns>
        public abstract int Replace (ProcessingKeys proKeys, IEnumerable<IFormLinkContainerGetter>? newList);

        #endregion Abstract

        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.GetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
                    return -1;

                int changes = 0;

                foreach (var actionData in GetFillValueAs(proKeys) ?? [])
                {
                    if (actionData?.FormKey == null || actionData.FormKey.Value == FormKey.Null)
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

                    var e = FindRecord(curList, actionData.FormKey.Value);

                    if (e != null && (actionData.FormKey.Operation == ListLogic.DEL || !actionData.Equals(e)))
                    {
                        _ = Remove(proKeys, e);
                        changes++;
                    }

                    if (actionData.FormKey.Operation == ListLogic.ADD && (e == null || (e != null && !actionData.Equals(e))))
                    {
                        _ = Add(proKeys, actionData);
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

                int changes = Replace(proKeys, newList);

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
                    var key = GetFormKeyFromRecord(item);
                    if (key.ModKey == forwardContext.ModKey)
                    {
                        var i = FindRecord(curList, key);

                        if (i != null && !DataEquals(item, i))
                        {
                            _ = Remove(proKeys, i);
                            _ = Forward(proKeys, item);
                            changes++;
                        }

                        if (i == null)
                        {
                            _ = Forward(proKeys, item);
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
                        && RecordsMatch(curList, originList);
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

                if (FindRecord(curLinks, link.Value) != null)
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

        private bool RecordsMatch (IReadOnlyList<IFormLinkContainerGetter>? left, IReadOnlyList<IFormLinkContainerGetter>? right)
        {
            if (!left.SafeAny() && !right.SafeAny())
                return true;

            if (!left.SafeAny() || !right.SafeAny() || left.Count != right.Count)
                return false;

            foreach (var l in left)
            {
                if (!right.Any(r => DataEquals(l, r)))
                    return false;
            }

            return true;
        }
    }
}