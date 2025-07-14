using GenericSynthesisPatcher.Games.Universal.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Rules
{
    [Flags]
    [JsonConverter(typeof(FlagConverter))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Underscores part of logic in this case, marking hidden flags not documented.")]
    public enum ForwardOptions
    {
        /// <summary>
        ///     The most basic of forward of from a single mod. If multiple mods listed first mod
        ///     that contains record will be used.
        /// </summary>
        Default = 1 << 1,

        /// <summary>
        ///     Can be used when forwarding fields that contain list of FormKeys. It will not
        ///     overwrite current value, but add any entries that point to items in the same mod as
        ///     being forwarded. If multiple mods all mods will be processed with each adding it's
        ///     own entries to the list.
        ///     <para>
        ///         If combined with Default flag, the first mod in the list will do a default
        ///         forward replacing current value, while all other mods in list will do SelfMasterOnly
        ///     </para>
        /// </summary>
        SelfMasterOnly = 1 << 2,

        /// <summary>
        ///     Changes the configuration from being indexed by mod name, with potentially multiple
        ///     fields per mod, to being indexed by field name with potentially multiple mods per field.
        /// </summary>
        IndexedByField = 1 << 3,

        _randomMod = 1 << 4,
        _nonDefaultMod = 1 << 5,
        _nonNullMod = 1 << 6,
        _sortMods = 1 << 7,
        _hpu = 1 << 8,

        /// <summary>
        ///     Makes it so when multiple mods listed, filters out any that have the same value as
        ///     the master record.
        ///     <para>Enables Default and IndexByField flags as well.</para>
        ///     <para>Ignored if used with SelfMasterOnly flag.</para>
        /// </summary>
        NonDefault = Default | _nonDefaultMod | IndexedByField,

        /// <summary>
        ///     Makes it so when multiple mods listed, filters out any that have the null or empty value.
        ///     <para>Enables Default and IndexByField flags as well.</para>
        ///     <para>Ignored if used with SelfMasterOnly flag.</para>
        /// </summary>
        NonNull = Default | _nonNullMod | IndexedByField,

        /// <summary>
        ///     Sorts mods listed by load order priority, with higher priority at the start. Useful
        ///     when leaving mods empty, which would add every mod in load order to the list.
        ///     <para>Enables IndexByField flags as well.</para>
        /// </summary>
        Sort = _sortMods | IndexedByField,

        /// <summary>
        ///     Includes Default, SelfMasterOnly and IndexByField flags.
        /// </summary>
        DefaultThenSelfMasterOnly = Default | SelfMasterOnly | IndexedByField,

        /// <summary>
        ///     Makes it so if multiple mods listed, instead of using first mod that contains the
        ///     record, it will pick a random mod from the list of mods containing the record.
        ///     <para>If used without Sort then random mod is picked per record from valid mods.</para>
        ///     <para>
        ///         However if used with Sort then will instead sort mods into randomized order, but
        ///         that same order will be used for all records, leaving actual selection down to
        ///         other Mod selection options like record existing in mod, NonDefault, NonNull.
        ///     </para>
        ///     <para>Enables Default and IndexByField flags as well.</para>
        ///     <para>Crazy if used with SelfMasterOnly.</para>
        /// </summary>
        Random = Default | _randomMod | IndexedByField,

        /// <summary>
        ///     Deprecated in v2.0 here so ForwardType="DefaultRandom" works. Once it fully removed
        ///     we should remove this as well.
        /// </summary>
        DefaultRandom = Random,

        /// <summary>
        ///     Couldn't come up with better name so this is it for now. Highest Priority Unique
        ///     (HPU). It is basically the "Merge" for non-list fields. This looks at all records
        ///     for the field, and finds the highest priority unique value to forward. Best used
        ///     with Sort if manually entering the mods, else priority is based on the order you
        ///     enter them start highest to lowest, else leave mod list empty so it auto populates
        ///     with all mods in the sorted order. It is designed to be used on records that are
        ///     often validly included in patches but not updated, like Worldspace and Cell records.
        ///     <para>
        ///         One way to look at this is you look at each value starting at lowest priority
        ///         and each time you find a value that hasn't been used before it becomes the HPU.
        ///         When you finished checking all what ever is the last HPU is the value that will
        ///         be forward if not already set.
        ///     </para>
        ///     <para>
        ///         However it is a bit more complex than that as it does honor Masters. Meaning if
        ///         a patch lists a mod as a parent, then HPU will accept the patched value even if
        ///         it sets it back to an already seen value.
        ///     </para>
        ///     <para>
        ///         Could be used with NonNull to make sure value from Master record not overridden
        ///         with a null value if that happens to be HPU. NonDefault pointless as would only
        ///         "try" and set Default as HPU if value was never overwritten.
        ///     </para>
        ///     <para>Enables Default and IndexByField flags as well.</para>
        ///     <para>
        ///         Invalid on list types that implement the merge action. Currently will just throw
        ///         an error and stop the whole process.
        ///     </para>
        /// </summary>
        HPU = Default | IndexedByField | _hpu,
    }
}