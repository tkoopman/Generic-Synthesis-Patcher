using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "Just used for sorting")]
    public abstract class GSPBase : IComparable<GSPBase>
    {
        private const int ClassLogCode = 0x03;

        [JsonIgnore]
        public int ConfigFile { get; internal set; }

        [JsonIgnore]
        public int ConfigRule { get; internal set; }

        /// <summary>
        ///     Set to true to enable Debug/Trace logging for this rule
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty(PropertyName = "Debug")]
        public bool Debug { get; set; }

        /// <summary>
        ///     If set will check if record has already been patched by another GSP rule or not. If
        ///     you don't care exclude or set to null
        /// </summary>
        [JsonProperty(PropertyName = "Patched")]
        public bool? Patched { get; set; }

        /// <summary> Sets order of processing rules. Rules with matching priority can be executed
        /// in any order. Processed lowest to highest, so if rules with priority of 1 & 100 both
        /// touch the same field, while both still run the 100 priority will run last so may
        /// overwrite changes the other made. </summary>
        [DefaultValue(0)]
        [JsonProperty(PropertyName = "Priority")]
        public int Priority { get; private set; }

        /// <summary>
        ///     List of record types this rule should match
        /// </summary>
        [JsonProperty(PropertyName = "Types")]
        [JsonConverter(typeof(SingleOrArrayConverter<RecordTypeMapping>))]
        public IReadOnlyList<RecordTypeMapping> Types { get; protected set; } = [];

        #region Masters

        private List<ModKeyListOperation>? masters;

        /// <summary>
        ///     List of masters that should be either included or excluded from matching
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

                masters = value;
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

                masters = [];
                foreach (var v in value ?? [])
                    masters.Add(v.Inverse());
            }
        }

        [JsonProperty(PropertyName = "!Masters")]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKeyListOperation>))]
        public List<ModKeyListOperation>? MastersNot { set => MastersDel = value; }

        #endregion Masters

        #region PatchedBy

        private List<ModKeyListOperation>? patchedBy;

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

        /// <summary>
        ///     List of mods that if this record was patched by will either include or exclude it
        ///     from matching.
        ///     Note: Record will never matched PatchedBy of it's master mod. Use Masters for that.
        /// </summary>
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
                patchedBy = value;
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
                patchedBy = [];
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
                patchedBy = value;
            }
        }

        #endregion PatchedBy

        /// <summary>
        ///     Should only be used for sorting by Priority
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
        ///     Checks if current context matches filters. When overridden should always check base
        ///     matches first, and only do extra checks if base returned true, for best performance.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns true if context matches filters.</returns>
        public virtual bool Matches (ProcessingKeys proKeys)
        {
            if (!Types.Contains(proKeys.Type))
            {
                if (Global.Settings.Value.Logging.NoisyLogs.TypeMatchFailed)
                    Global.TraceLogger?.Log(ClassLogCode, "Matched: False", propertyName: "Record Type");
                return false;
            }

            if (Global.Settings.Value.Logging.NoisyLogs.TypeMatchSuccessful)
                Global.TraceLogger?.Log(ClassLogCode, "Matched: True", propertyName: "Record Type");

            if (Masters != null && !MatchesHelper.Matches(proKeys.Record.FormKey.ModKey, Masters, nameof(Masters), Global.Settings.Value.Logging.NoisyLogs.MastersMatchSuccessful, Global.Settings.Value.Logging.NoisyLogs.MastersMatchFailed))
                return false;

            if (PatchedBy != null)
            {
                var all = Global.State.LinkCache.ResolveAllSimpleContexts(proKeys.Record.FormKey, proKeys.Record.Registration.GetterType).Select(m => m.ModKey);
                if (!MatchesHelper.Matches(all, patchedByLogic, PatchedBy, nameof(PatchedBy)))
                    return false;
            }

            if (Patched.HasValue)
            {
                bool result = Patched.Value ? proKeys.HasPatchRecord : !proKeys.HasPatchRecord;
                Global.TraceLogger?.Log(ClassLogCode, $"Matched: {result} Has patch record: {proKeys.HasPatchRecord}", propertyName: nameof(Patched));
                return result;
            }

            return true;
        }

        /// <summary>
        ///     Checks for anything that may make this rule invalid and possibly cause issues. Any
        ///     rule failing validation will mean the patcher will never start any patching, and
        ///     just end with an error.
        /// </summary>
        /// <returns>True if rule passes validation</returns>
        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
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
    }
}