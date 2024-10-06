using System;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    public partial class GSPRule
    {
        [Flags]
        public enum ForwardTypes
        {
            /// <summary>
            /// Will replace winning with value from forwarding mod.
            /// If field is a list then all winning values will be removed and forwarding record entries added. (No merging)
            /// </summary>
            Default,
            /// <summary>
            /// This  is only relevant when forwarding changes from lists of FormIDs. This includes Keywords.
            /// It will only add new FormIDs to lists if FormID is from the same Forwarding Mod.
            /// It will not remove any current items from winning records.
            /// Other field types will just behave like they do in Default.
            /// </summary>
            SelfMasterOnly
        }
    }
}
