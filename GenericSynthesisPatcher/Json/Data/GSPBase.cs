using System.ComponentModel;

using DynamicData;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Json.Data
{
    [JsonConverter(typeof(GSPBaseConverter))]
    public abstract class GSPBase : IComparable<GSPBase>
    {
        private const int ClassLogCode = 0x03;

        // Only works due to knowing only will be set by deserialization as will always just add to list
        private List<ListOperation>? patched = null;

        [JsonIgnore]
        public int ConfigFile { get; internal set; }

        [JsonIgnore]
        public int ConfigRule { get; internal set; }

        /// <summary>
        /// Set to true to enable Debug/Trace logging for this rule
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty(PropertyName = "Debug")]
        public bool Debug { get; set; }

        /// <summary>
        /// Sets order of processing rules. Rules with matching priority can be executed in any order.
        /// Processed lowest to highest, so if rules with priority of 1 & 100 both touch the same field,
        /// while both still run the 100 priority will run last so may overwrite changes the other made.
        /// </summary>
        [DefaultValue(0)]
        [JsonProperty(PropertyName = "Priority")]
        public int Priority { get; private set; }

        /// <summary>
        /// List of record types this rule should match
        /// </summary>
        [JsonProperty(PropertyName = "Types")]
        [JsonConverter(typeof(SingleOrArrayConverter<RecordTypeMapping>))]
        public IReadOnlyList<RecordTypeMapping> Types { get; protected set; } = [];

        #region Masters

        // Only works due to knowing only one will be set by deserialization as will always just add to list
        // If some puts in multiple it will probably generate errors.
        private List<ModKeyListOperation>? masters = null;

        /// <summary>
        /// List of masters that should be either included or excluded from matching
        /// </summary>
        [JsonProperty(PropertyName = "Masters")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? Masters
        {
            get => masters;
            set
            {
                if (!value.SafeAny())
                    return;

                masters ??= [];
                masters.Add(value);
            }
        }

        [JsonProperty(PropertyName = "-Masters")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? MastersDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                masters ??= [];
                foreach (var v in value ?? [])
                    masters.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!Masters")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? MastersNot { set => MastersDel = value; }

        #endregion Masters

        #region PatchedBy

        // Only works due to knowing only will be set by deserialization as will always just add to list
        private List<ModKeyListOperation>? patchedBy = null;

        private FilterLogic patchedByLogic = FilterLogic.Default;

        [JsonProperty(PropertyName = "&PatchedBy")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedAnd
        {
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.AND;
                patchedBy ??= [];
                patchedBy.Add(value);
            }
        }

        [JsonProperty(PropertyName = "PatchedBy")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedBy
        {
            get => patchedBy;
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.Default;
                patchedBy ??= [];
                patchedBy.Add(value);
            }
        }

        [JsonProperty(PropertyName = "-PatchedBy")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedByDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.Default;
                patchedBy ??= [];
                foreach (var v in value ?? [])
                    patchedBy.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!PatchedBy")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedByNot { set => PatchedByDel = value; }

        [JsonProperty(PropertyName = "^PatchedBy")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? PatchedXOR
        {
            set
            {
                if (!value.SafeAny())
                    return;

                patchedByLogic = FilterLogic.XOR;
                patchedBy ??= [];
                patchedBy.Add(value);
            }
        }

        #endregion PatchedBy

        #region Patched

        /// <summary>
        /// List of mods that if this record was patched by will either include or exclude it from matching.
        /// Note: Record will never matched PatchedBy of it's master mod. Use Masters for that.
        /// </summary>
        [JsonProperty(PropertyName = "Patched")]
        [JsonConverter(typeof(SingleOrArrayConverter<ListOperation>))]
        public List<ListOperation>? Patched
        {
            get => patched;
            set
            {
                if (!value.SafeAny())
                    return;

                patched ??= [];
                patched.Add(value);
            }
        }

        [JsonProperty(PropertyName = "-Patched")]
        [JsonConverter(typeof(SingleOrArrayConverter<ListOperation>))]
        public List<ListOperation>? PatchedDel
        {
            set
            {
                if (!value.SafeAny())
                    return;

                patched ??= [];
                foreach (var v in value ?? [])
                    patched.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!Patched")]
        [JsonConverter(typeof(SingleOrArrayConverter<ListOperation>))]
        public List<ListOperation>? PatchedNot { set => PatchedDel = value; }

        #endregion Patched

        /// <summary>
        /// Should only be used for sorting by Priority
        /// </summary>
        public int CompareTo (GSPBase? other)
            => other == null ? 1
             : other == this ? 0
             : Priority.CompareTo(other.Priority);

        public override int GetHashCode ()
        {
            var hash = new HashCode();

            hash.Add(Priority);
            hash.Add(Types);
            if (Masters != null)
                hash.AddEnumerable(Masters);

            return hash.ToHashCode();
        }

        /// <summary>
        /// Checks if current context matches filters.
        /// When overridden should always check base matches first, and only do extra checks if base returned true, for best performance.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns true if context matches filters.</returns>
        public virtual bool Matches (ProcessingKeys proKeys)
        {
            if (!Types.Contains(proKeys.Type))
            {
                if (!Global.Settings.Value.Logging.DisabledLogs.FailedTypeMatch)
                    Global.TraceLogger?.Log(ClassLogCode, "Match Types: No");
                return false;
            }

            if (!Global.Settings.Value.Logging.DisabledLogs.SuccessfulTypeMatch)
                Global.TraceLogger?.Log(ClassLogCode, "Matched Types: Yes");

            // Due to SafeAny checks in property setters should be null or a list with at least 1 entry, never just an empty list. So process with this assumption
            if (Masters != null && !MatchesHelper.Matches(proKeys.Record.FormKey.ModKey, Masters, "Masters: "))
                return false;

            // Due to SafeAny checks in property setters should be null or a list with at least 1 entry, never just an empty list. So process with this assumption
            if (PatchedBy != null)
            {
                var all = Global.State.LinkCache.ResolveAllContexts(proKeys.Record.FormKey, proKeys.Record.Registration.GetterType).Select(m => m.ModKey);
                if (!MatchesHelper.Matches(all, patchedByLogic, PatchedBy, debugPrefix: "PatchedBy: "))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for anything that may make this rule invalid and possibly cause issues.
        /// Any rule failing validation will mean the patcher will never start any patching,
        /// and just end with an error.
        /// </summary>
        /// <returns>True if rule passes validation</returns>
        public virtual bool Validate ()
        {
            if (Debug)
                LogHelper.WriteLog(LogLevel.Debug, ClassLogCode, "Debug / Trace logging enabled for this rule.", rule: this);

            if (Masters.SafeAny() && !MatchesHelper.Validate(Masters, $"Masters: "))
                return false;

            if (patchedByLogic != FilterLogic.AND && PatchedBy.SafeAny() && !MatchesHelper.Validate(patchedByLogic, PatchedBy, "PatchedBy: "))
                return false;

            return true;
        }

        /// <summary>
        /// Validates Lists to make sure it is null if list contains 0 entries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>Cleaned List.</returns>
        protected static List<T>? ValidateList<T> (List<T>? list) => list.SafeAny() ? list : null;
    }
}