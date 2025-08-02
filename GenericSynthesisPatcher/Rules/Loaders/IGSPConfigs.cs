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
    public interface IGSPConfigs : IEnumerable<GSPBase>
    {
        /// <summary>
        ///     Get number of top level rules loaded
        /// </summary>
        public int Count { get; }

        /// <summary>
        ///     Creates new instance.
        /// </summary>
        /// <param name="fileCount">
        ///     As configs could be loaded from multiple sources, this is the file count to start
        ///     at. So rules returned should have <see cref="GSPBase.ConfigFile" /> set to fileCount++.
        ///
        ///     Each file you load rules from should have a unique file count, starting at this value.
        /// </param>
        /// <param name="rules">
        ///     Rules to be added to patcher's list of rules. Maybe empty list if no critical
        ///     errors, but no rules found to load.
        /// </param>
        /// <returns>
        ///     True no rules loaded or ALL loaded rules passed <see cref="GSPBase.Validate" />,
        ///     else false.
        /// </returns>
        public static abstract bool TryLoadRules (int fileCount, [NotNullWhen(true)] out IGSPConfigs? rules);
    }
}