using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Json.Data
{
    public interface IFormLinksWithData<T, TMajor>
        where T : class, IFormLinksWithData<T, TMajor>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        /// <summary>
        /// The FormKey and Operation for this action
        /// </summary>
        public FormKeyListOperation<TMajor> FormKey { get; }

        /// <summary>
        /// Can this be used in merge actions
        /// </summary>
        /// <returns>True if Merge implemented</returns>
        public static abstract bool CanMerge ();

        /// <summary>
        /// Check if 2 links and data are equal
        /// </summary>
        /// <returns>True only if both FormKey and any data match across both entries.</returns>
        public abstract static bool DataEquals ( IFormLinkContainerGetter left, IFormLinkContainerGetter right );

        /// <summary>
        /// Find record from list of records.
        /// </summary>
        /// <param name="list">List to search</param>
        /// <param name="key">FormKey to find</param>
        /// <returns>Getter of found entry or null if not found.</returns>
        public abstract static IFormLinkContainerGetter? FindRecord ( IEnumerable<IFormLinkContainerGetter>? list, FormKey key );

        /// <summary>
        /// Add source entry to patch record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rcd"></param>
        /// <param name="patchRecord"></param>
        /// <param name="source">Entry to add to list in patch record.</param>
        /// <returns>Number of changes made to add entry. Should be 1 if successful else 0. -1 if major failure.</returns>
        public abstract static int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord, IFormLinkContainerGetter source );

        /// <summary>
        /// Get data from JSON value in rule.
        /// </summary>
        /// <param name="rule">Rule to get data from</param>
        /// <param name="key">Key to current action in rule</param>
        /// <returns>List of all data values for action.</returns>
        public abstract static List<T>? GetFillValueAs ( GSPRule rule, FilterOperation key );

        /// <summary>
        /// Get FormKey of entry.
        /// </summary>
        /// <param name="from">Form Link to get FormKey from</param>
        /// <returns>FormKey</returns>
        public abstract static FormKey GetFormKeyFromRecord ( IFormLinkContainerGetter from );

        /// <summary>
        /// Preform merge of current field in current record.
        /// </summary>
        /// <param name="context">Context fo current record</param>
        /// <param name="rule">Rule merge action from.</param>
        /// <param name="valueKey">Key of merge action entry to perform.</param>
        /// <param name="rcd">RCD to use</param>
        /// <param name="patchRecord">Current patch record if already been patched.</param>
        /// <returns>Number of changes to complete merge. Each entry removed / added counts as 1 change.</returns>
        public static abstract int Merge ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord );

        /// <summary>
        /// Remove link entry from current record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rcd"></param>
        /// <param name="patchRecord"></param>
        /// <param name="remove"></param>
        /// <returns>Number of changes made to remove entry. Should be 1 if successful else 0. -1 if major failure.</returns>
        public abstract static int Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord, IFormLinkContainerGetter remove );

        /// <summary>
        /// Replace list of links in current record with new list.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rcd"></param>
        /// <param name="patchRecord"></param>
        /// <param name="newList">Patch record list should match this list after replace.</param>
        /// <returns>Number of changes to complete replace. Each entry removed / added counts as 1 change.</returns>
        public abstract static int Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord, IEnumerable<IFormLinkContainerGetter>? newList );

        /// <summary>
        /// Add entry as detailed by this IFormLinksWithData object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rcd"></param>
        /// <param name="patchRecord"></param>
        /// <returns>Number of changes made to add entry. Should be 1 if successful else 0. -1 if major failure.</returns>
        public int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord );

        /// <summary>
        /// Checks if supplied Link matches FormKey and Data in this IFormLinksWithData object.
        /// </summary>
        /// <param name="other">Current entry to check.</param>
        /// <returns>True only if both FormKey and any data match.</returns>
        public bool DataEquals ( IFormLinkContainerGetter other );

        /// <summary>
        /// Find record in supplied list that matches this FormKey.
        /// Data is not compared just FormKey.
        /// </summary>
        /// <param name="list">List to search.</param>
        /// <returns>Record from list that matches FormKey or null if not found.</returns>
        public IFormLinkContainerGetter? FindFormKey ( IEnumerable<IFormLinkContainerGetter>? list );
    }
}