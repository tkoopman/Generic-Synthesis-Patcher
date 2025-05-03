using System.Diagnostics.CodeAnalysis;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public interface IRecordAction
    {
        public bool CanFill ();

        public bool CanForward ();

        public bool CanForwardSelfOnly ();

        public bool CanMatch ();

        public bool CanMerge ();

        /// <summary>
        ///     Static fill of value from JSON rule
        /// </summary>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to
        ///     OnlyIfDefault = true and current record doesn't match origin. 0 if could update just
        ///     nothing to update. Like value already equals what you were going to set. &gt;=1 The
        ///     number of changes made. If single field value should never be &gt;1, but for lists
        ///     could include count of number of items added/removed.
        /// </returns>
        public int Fill (ProcessingKeys proKeys);

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods);

        /// <summary>
        ///     Static fill of value from JSON rule
        /// </summary>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to
        ///     OnlyIfDefault = true and current record doesn't match origin. 0 if could update just
        ///     nothing to update. Like value already equals what you were going to set. &gt;=1 The
        ///     number of changes made. If single field value should never be &gt;1, but for lists
        ///     could include count of number of items added/removed.
        /// </returns>
        public int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext);

        /// <summary>
        ///     Static fill of value from JSON rule
        /// </summary>
        /// <returns>
        ///     -1 if couldn't update due to error. This would include not updating due to
        ///     OnlyIfDefault = true and current record doesn't match origin. 0 if could update just
        ///     nothing to update. Like value already equals what you were going to set. &gt;=1 The
        ///     number of changes made. If single field value should never be &gt;1, but for lists
        ///     could include count of number of items added/removed.
        /// </returns>
        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext);

        /// <summary>
        ///     Checks if property is currently set to Null or equivalent empty value.
        /// </summary>
        /// <param name="proKeys"></param>
        /// <param name="recordContext"></param>
        /// <returns></returns>
        public bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext);

        /// <summary>
        ///     Check if field matches in both current context and origin.
        /// </summary>
        /// <returns>True if this field matches</returns>
        public bool MatchesOrigin (ProcessingKeys proKeys);

        /// <summary>
        ///     Check if field matches in both this record and origin.
        /// </summary>
        /// <returns>True if this field matches</returns>
        public bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext);

        /// <summary>
        ///     Check Property matches current RuleKey both set in proKeys.
        /// </summary>
        /// <returns>True if this field matches</returns>
        public bool MatchesRule (ProcessingKeys proKeys);

        public int Merge (ProcessingKeys proKeys);

        /// <summary>
        ///     Used for generating Documentation
        /// </summary>
        /// <returns>If valid documentation for type found</returns>
        public bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example);
    }
}