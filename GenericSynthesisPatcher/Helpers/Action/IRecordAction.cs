using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public interface IRecordAction
    {
        public bool CanFill ();

        public bool CanForward ();

        public bool CanForwardSelfOnly ();

        public bool CanMerge ();

        /// <summary>
        /// Static fill of value from JSON rule
        /// </summary>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to OnlyIfDefault = true and current record doesn't match origin.
        ///     0 if could update just nothing to update. Like value already equals what you were going to set.
        ///     >=1 The number of changes made. If single field value should never be >1, but for lists could include count of number of items added/removed.
        /// </returns>
        public int Fill (ProcessingKeys proKeys);

        /// <summary>
        /// Static fill of value from JSON rule
        /// </summary>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to OnlyIfDefault = true and current record doesn't match origin.
        ///     0 if could update just nothing to update. Like value already equals what you were going to set.
        ///     >=1 The number of changes made. If single field value should never be >1, but for lists could include count of number of items added/removed.
        /// </returns>
        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext);

        /// <summary>
        /// Static fill of value from JSON rule
        /// </summary>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to OnlyIfDefault = true and current record doesn't match origin.
        ///     0 if could update just nothing to update. Like value already equals what you were going to set.
        ///     >=1 The number of changes made. If single field value should never be >1, but for lists could include count of number of items added/removed.
        /// </returns>
        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext);

        /// <summary>
        /// Check if field matches in both current context and origin.
        /// </summary>
        /// <returns>True if this field matches</returns>
        public bool MatchesOrigin (ProcessingKeys proKeys);

        /// <summary>
        /// Check if current rule matches. Excludes checking Basic checks.
        /// </summary>
        /// <returns>True if this field matches</returns>
        public bool MatchesRule (ProcessingKeys proKeys);

        public int Merge (ProcessingKeys proKeys);
    }
}