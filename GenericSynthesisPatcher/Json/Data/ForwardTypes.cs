namespace GenericSynthesisPatcher.Json.Data
{
    public enum ForwardTypes
    {
        /// <summary>
        /// Will replace winning with value from forwarding mod.
        /// If field is a list then all winning values will be removed and forwarding record entries added. (No merging)
        /// </summary>
        Default = ForwardTypeFlags.Default,

        /// <summary>
        /// This  is only relevant when forwarding changes from lists of FormIDs. This includes Keywords.
        /// It will only add new FormIDs to lists if FormID is from the same Forwarding Mod.
        /// It will not remove any current items from winning records.
        /// Other field types will just behave like they do in Default.
        /// </summary>
        SelfMasterOnly = ForwardTypeFlags.SelfMasterOnly,

        /// <summary>
        /// This only works with ForwardIndexedByField = true
        /// For each field the first mod's field will forward using default method, then
        /// all other mods in the list will use the SelfMasterOnly forward method.
        /// If the first Default forward fails then non of the SelfMasterOnly will happen.
        /// Main failure reasons would be if you have OnlyIfDefault set to true and it doesn't match the default or
        /// The record doesn't exist in the first mod listed. OnlyIfDefault is only checked for the first mod listed.
        /// </summary>
        DefaultThenSelfMasterOnly = ForwardTypeFlags.DefaultThenSelfMasterOnly,

        /// <summary>
        /// This only works with ForwardIndexedByField = true
        /// Instead of picking first mod from list containing record it will pick random mod from list containing record.
        /// </summary>
        DefaultRandom = ForwardTypeFlags.DefaultRandom,
    }
}