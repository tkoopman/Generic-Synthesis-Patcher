using GenericSynthesisPatcher.Json.Data;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public interface IAction
    {
        public static abstract bool CanFill ();

        public static abstract bool CanForward ();

        public static abstract bool CanForwardSelfOnly ();

        /// <summary>
        /// Static fill of value from JSON rule
        /// </summary>
        /// <param name="context">Context of record to update.</param>
        /// <param name="origin">Master of record if OnlyIfDefault = true and context record isn't the master already.</param>
        /// <param name="rule">Current rule being processed.</param>
        /// <param name="valueKey">Key to current action being taken. Used to get value data from JSON.</param>
        /// <param name="rcd">RCD used to process this action.</param>
        /// <param name="patchedRecord">If patch record created for current field it will be provided here. If provided should also ignore origin checks as this field has already been changed.</param>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to OnlyIfDefault = true and current record doesn't match origin.
        ///     0 if could update just nothing to update. Like value already equals what you were going to set.
        ///     >=1 The number of changes made. If single field value should never be >1, but for lists could include count of number of items added/removed.
        /// </returns>
        public static abstract int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context,
                                           GSPRule rule,
                                           ValueKey valueKey,
                                           RecordCallData rcd,
                                           ref ISkyrimMajorRecord? patchedRecord );

        /// <summary>
        /// Static fill of value from JSON rule
        /// </summary>
        /// <param name="context">Context of record to update.</param>
        /// <param name="origin">Master of record if OnlyIfDefault = true and context record isn't the master already.</param>
        /// <param name="rule">Current rule being processed.</param>
        /// <param name="valueKey">Key to current action being taken. Used to get value data from JSON.</param>
        /// <param name="rcd">RCD used to process this action.</param>
        /// <param name="patchedRecord">If patch record created for current field it will be provided here. If provided should also ignore origin checks as this field has already been changed.</param>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to OnlyIfDefault = true and current record doesn't match origin.
        ///     0 if could update just nothing to update. Like value already equals what you were going to set.
        ///     >=1 The number of changes made. If single field value should never be >1, but for lists could include count of number of items added/removed.
        /// </returns>
        public static abstract int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context,
                                              GSPRule rule,
                                              IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext,
                                              RecordCallData rcd,
                                              ref ISkyrimMajorRecord? patchedRecord );

        /// <summary>
        /// Static fill of value from JSON rule
        /// </summary>
        /// <param name="context">Context of record to update.</param>
        /// <param name="origin">Master of record if OnlyIfDefault = true and context record isn't the master already.</param>
        /// <param name="rule">Current rule being processed.</param>
        /// <param name="valueKey">Key to current action being taken. Used to get value data from JSON.</param>
        /// <param name="rcd">RCD used to process this action.</param>
        /// <param name="patchedRecord">If patch record created for current field it will be provided here. If provided should also ignore origin checks as this field has already been changed.</param>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to OnlyIfDefault = true and current record doesn't match origin.
        ///     0 if could update just nothing to update. Like value already equals what you were going to set.
        ///     >=1 The number of changes made. If single field value should never be >1, but for lists could include count of number of items added/removed.
        /// </returns>
        public static abstract int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context,
                                                      GSPRule rule,
                                                      IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext,
                                                      RecordCallData rcd,
                                                      ref ISkyrimMajorRecord? patchedRecord );

        /// <summary>
        /// Check if current rule matches. Excludes checking Basic checks.
        /// </summary>
        /// <param name="check">Record to check if it matches rule.</param>
        /// <param name="rule">Rule to check against</param>
        /// <param name="rcd">Field currently checking</param>
        /// <returns>True if this field matches</returns>
        public static abstract bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd );

        /// <summary>
        /// Check if field matches in both current context and origin.
        /// </summary>
        /// <param name="check">Record to check if it matches origin.</param>
        /// <param name="origin">Master record to check against.</param>
        /// <param name="rcd">Field currently checking</param>
        /// <returns>True if this field matches</returns>
        public static abstract bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd );
    }
}