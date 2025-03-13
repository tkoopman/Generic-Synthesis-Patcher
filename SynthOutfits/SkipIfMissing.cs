namespace SynthOutfits
{
    /// <summary>
    ///     If entries that are to be added to a list are missing, should the entire list be
    ///     skipped?
    /// </summary>
    public enum SkipIfMissing
    {
        /// <summary>
        ///     Skip if any entry is missing.
        /// </summary>
        Any = 0,

        /// <summary>
        ///     Only skip if all entries are missing which would make an empty list.
        /// </summary>
        All = 1,

        /// <summary>
        ///     Never skip.
        /// </summary>
        Never = 2,
    }
}