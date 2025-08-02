using System.Diagnostics.CodeAnalysis;

namespace GenericSynthesisPatcher.Rules.Loaders
{
    /// <summary>
    ///     List of rules patcher will use. Could be loaded from multiple sources.
    ///
    ///     Rules returned MUST be validated, however don't need to be sorted, as that will be done
    ///     once all rules are loaded if multiple sources.
    /// </summary>
    /// <returns>List of rules</returns>
    public interface IGSPConfigs
    {
        /// <summary>
        ///     Get number of top level rules loaded
        /// </summary>
        public int Count { get; }

        /// <summary>
        ///     Access loaded rules. Null if you have not called <see cref="LoadRules(int)" /> yet,
        ///     or that call failed.
        /// </summary>
        public IEnumerable<GSPBase>? Rules { get; }

        /// <summary>
        ///     Run after GSP has finished processing, and is about to close.
        /// </summary>
        /// <param name="successful">True if no critical errors during the run.</param>
        public void Close (bool successful);

        /// <summary>
        ///     Load rules if possible. Can access rules via <see cref="Rules" /> after loaded.
        /// </summary>
        /// <param name="fileCount">
        ///     As configs could be loaded from multiple sources, this is the file count to start
        ///     at. So rules returned should have <see cref="GSPBase.ConfigFile" /> set to fileCount++.
        ///
        ///     Each file you load rules from should have a unique file count, starting at this value.
        /// </param>
        /// <returns>
        ///     True no rules loaded or ALL loaded rules passed <see cref="GSPBase.Validate" />,
        ///     else false.
        /// </returns>
        [MemberNotNullWhen(true, nameof(Rules))]
        public bool LoadRules (int fileCount);
    }
}