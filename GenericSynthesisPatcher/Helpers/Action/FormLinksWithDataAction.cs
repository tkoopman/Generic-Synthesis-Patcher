using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Json.Data.Action;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    /// <summary>
    ///     Abstract class used to implement actions that modify fields that contain list of
    ///     FormLinks with extra data
    /// </summary>
    /// <typeparam name="TActionData">The JSON Data class</typeparam>
    /// <typeparam name="TMajor">
    ///     The major class that FormKey on an entry in the list points to
    /// </typeparam>
    /// <typeparam name="TData">
    ///     The class of the items contained in the list. This would include the FormKey pointing to
    ///     TMajor and the extra data.
    /// </typeparam>
    public abstract class FormLinksWithDataAction<TActionData, TMajor, TData> : IRecordAction
        where TActionData : FormLinksWithDataActionDataBase<TMajor, TData>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        private const int ClassLogCode = 0x15;

        /// <summary>
        ///     Add entry as detailed by this data object.
        /// </summary>
        /// <returns>
        ///     Number of changes made to add entry. Should be 1 if successful else 0. -1 if major
        ///     failure.
        /// </returns>
        public int Add (ProcessingKeys proKeys, TActionData data)
        {
            if (!Mod.TryGetPropertyValueForEditing<ExtendedList<TData>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var items))
                return -1;

            items.Add(data.ToActionData());
            return 1;
        }

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => true;

        public bool CanMatch () => true;

        public bool CanMerge () => true;

        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList) || !TryGetFillValueAs(proKeys, out var links))
                    return -1;

                int changes = 0;

                foreach (var actionData in links ?? [])
                {
                    // Check if action wanting to clear all
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

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods) => throw new NotImplementedException("ForwardOption HPU invalid on this field.");

        /// <summary>
        ///     Find record from list of records.
        /// </summary>
        /// <param name="list">List to search</param>
        /// <param name="key">FormKey to find</param>
        /// <returns>Getter of found entry or null if not found.</returns>
        public IFormLinkContainerGetter? FindRecord (IEnumerable<IFormLinkContainerGetter>? list, FormKey key) => list?.FirstOrDefault(s => s != null && GetFormKeyFromRecord(s).Equals(key), null);

        /// <summary>
        ///     Add source entry to patch record.
        /// </summary>
        /// <param name="source">Entry to add to list in patch record.</param>
        /// <returns>
        ///     Number of changes made to add entry. Should be 1 if successful else 0. -1 if major
        ///     failure.
        /// </returns>
        public int Forward (ProcessingKeys proKeys, IFormLinkContainerGetter source)
        {
            var newEntry = CreateFrom(source);

            if (newEntry == null || !Mod.TryGetPropertyValueForEditing<ExtendedList<TData>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var items))
                return -1;

            items.Add(newEntry);

            Global.TraceLogger?.Log(ClassLogCode, $"Added {newEntry}");

            return 1;
        }

        public int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
                    return -1;

                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(forwardContext.Record, proKeys.Property.PropertyName, out var newList))
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

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
                    return -1;

                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(forwardContext.Record, proKeys.Property.PropertyName, out var newList))
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
                    if (key != null && key.ModKey == forwardContext.ModKey)
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

        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || curValue is null || !curValue.Any();

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if all form keys and data matches</returns>
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(recordContext.Record, proKeys.Property.PropertyName, out var curList)
                && Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originList)
                && recordsMatch(curList, originList));

        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        /// <summary>
        ///     Only checks the FormKeys not the Data
        /// </summary>
        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetMatchValueAs(out bool fromCache, out List<FormKeyListOperation<TMajor>>? matches))
                return false;

            if (!matches.SafeAny())
                return true;

            if (!fromCache && !MatchesHelper.Validate(proKeys.RuleKey.Operation, matches))
                throw new InvalidDataException("Json data for matches invalid");

            if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curLinks))
                return false; // Property must not exist for this record.

            return MatchesHelper.Matches(curLinks?.Select(GetFormKeyFromRecord), proKeys.RuleKey.Operation, matches, propertyName: proKeys.Property.PropertyName);
        }

        /// <summary>
        ///     Preform merge of current field in current record.
        /// </summary>
        /// <returns>
        ///     Number of changes to complete merge. Each entry removed / added counts as 1 change.
        /// </returns>
        public int Merge (ProcessingKeys proKeys)
        {
            Global.UpdateLoggers(ClassLogCode);

            var root = RecordGraph<IFormLinkContainerGetter>.Create(
                proKeys,
                record => Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(record, proKeys.Property.PropertyName, out var value) ? value : null,
                ToString);

            return root != null && root.Merge(out var newList) ? Replace(proKeys, newList) : 0;
        }

        /// <summary>
        ///     Remove link entry from current record.
        /// </summary>
        /// <param name="remove"></param>
        /// <returns>
        ///     Number of changes made to remove entry. Should be 1 if successful else 0. -1 if
        ///     major failure.
        /// </returns>
        public int Remove (ProcessingKeys proKeys, IFormLinkContainerGetter remove)
        {
            var entry = CreateFrom(remove);

            if (entry == null || !Mod.TryGetPropertyValueForEditing<ExtendedList<TData>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var items))
                return -1;

            if (items.Remove(entry))
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Removed {entry}");
                return 1;
            }

            Global.Logger.Log(ClassLogCode, $"Failed to remove {ToString(remove)}", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
            return 0;
        }

        /// <summary>
        ///     Replace list of links in current record with new list.
        /// </summary>
        /// <param name="newList">Patch record list should match this list after replace.</param>
        /// <returns>
        ///     Number of changes to complete replace. Each entry removed / added counts as 1
        ///     change.
        /// </returns>
        public int Replace (ProcessingKeys proKeys, IEnumerable<IFormLinkContainerGetter>? newList)
        {
            if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkContainerGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
            {
                Global.Logger.Log(ClassLogCode, "Failed to replace entries", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            if (!Mod.TryGetPropertyValueForEditing<ExtendedList<TData>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out _))
                return -1;

            foreach (var d in del)
                _ = Remove(proKeys, d);

            foreach (var a in add)
                _ = Forward(proKeys, a);

            return add.Count() + del.Count();
        }

        // <inheritdoc />
        public abstract bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example);

        #region Abstract

        /// <summary>
        ///     Create new TData coping values from existing Getter.
        /// </summary>
        /// <param name="source">Source to copy values from.</param>
        /// <returns>New TData with Source values</returns>
        public abstract TData? CreateFrom (IFormLinkContainerGetter source);

        /// <summary>
        ///     Check if 2 links and data are equal
        /// </summary>
        /// <returns>True only if both FormKey and any data match across both entries.</returns>
        public abstract bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right);

        /// <summary>
        ///     Get FormKey of entry.
        /// </summary>
        /// <param name="from">Form Link to get FormKey from</param>
        /// <returns>FormKey</returns>
        public abstract FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from);

        public abstract string ToString (IFormLinkContainerGetter source);

        #endregion Abstract

        /// <summary>
        ///     Get data from JSON value in rule.
        /// </summary>
        /// <param name="rule">Rule to get data from</param>
        /// <param name="key">Key to current action in rule</param>
        /// <returns>List of all data values for action.</returns>
        public bool TryGetFillValueAs (ProcessingKeys proKeys, out List<TActionData>? data) => proKeys.TryGetFillValueAs(out data);

        private bool recordsMatch (IReadOnlyList<IFormLinkContainerGetter>? left, IReadOnlyList<IFormLinkContainerGetter>? right)
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