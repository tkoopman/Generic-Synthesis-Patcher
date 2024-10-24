using System.ComponentModel;

using DynamicData;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Json.Data
{
    [JsonConverter(typeof(GSPBaseConverter))]
    public abstract class GSPBase : IComparable<GSPBase>
    {
        private const int ClassLogCode = 0x03;

        // Only works due to knowing only will be set by deserialization as will always just add to list
        private List<ModKeyListOperation>? masters = null;

        [JsonIgnore]
        public int ConfigFile { get; internal set; }

        [JsonIgnore]
        public int ConfigRule { get; internal set; }

        [DefaultValue(false)]
        [JsonProperty(PropertyName = "Debug")]
        public bool Debug { get; set; }

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

        [DefaultValue(0)]
        [JsonProperty(PropertyName = "Priority")]
        public int Priority { get; private set; }

        [JsonProperty(PropertyName = "Types")]
        [JsonConverter(typeof(FlagConverter))]
        public RecordTypes Types { get; protected set; }

        /// <summary>
        /// Return GSP rule type for a record.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static RecordTypes GetGSPRuleType ( IMajorRecordGetter record ) => record switch
        {
            IIngestibleGetter => RecordTypes.ALCH,
            IAmmunitionGetter => RecordTypes.AMMO,
            IArmorGetter => RecordTypes.ARMO,
            IBookGetter => RecordTypes.BOOK,
            ICellGetter => RecordTypes.CELL,
            IContainerGetter => RecordTypes.CONT,
            IFactionGetter => RecordTypes.FACT,
            IIngredientGetter => RecordTypes.INGR,
            IKeyGetter => RecordTypes.KEYM,
            IMiscItemGetter => RecordTypes.MISC,
            INpcGetter => RecordTypes.NPC,
            IOutfitGetter => RecordTypes.OTFT,
            IScrollGetter => RecordTypes.SCRL,
            IWeaponGetter => RecordTypes.WEAP,
            IWorldspaceGetter => RecordTypes.WRLD,
            _ => RecordTypes.UNKNOWN
        };

        /// <summary>
        /// Return GSP rule type as string for a record.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static string GetGSPRuleTypeAsString ( IMajorRecordGetter record ) => record switch
        {
            IIngestibleGetter => "ALCH",
            IAmmunitionGetter => "AMMO",
            IArmorGetter => "ARMO",
            IBookGetter => "BOOK",
            ICellGetter => "CELL",
            IContainerGetter => "CONT",
            IFactionGetter => "FACT",
            IIngredientGetter => "INGR",
            IKeyGetter => "KEYM",
            IMiscItemGetter => "MISC",
            INpcGetter => "NPC",
            IOutfitGetter => "OTFT",
            IScrollGetter => "SCRL",
            IWeaponGetter => "WEAP",
            IWorldspaceGetter => "WRLD",
            _ => "UNKNOWN"
        };

        /// <summary>
        /// Should only be used for sorting by Priority
        /// </summary>
        public int CompareTo ( GSPBase? other )
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
        public virtual bool Matches ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            var recordType = GetGSPRuleType(context.Record);
            Global.TraceLogger?.Log(ClassLogCode, $"Check Types: {Types.HasFlag(recordType)} RecordType: {recordType}");

            if (recordType == RecordTypes.UNKNOWN || !Types.HasFlag(recordType))
                return false;

            if (Masters != null)
            {
                bool hasEntry = Masters?.Any(m => m.Value == context.Record.FormKey.ModKey) ?? false;
                bool isNeg = Masters != null && Masters.First().Operation == ListLogic.NOT;
                bool result = isNeg ? !hasEntry : (Masters == null || hasEntry);

                Global.TraceLogger?.Log(ClassLogCode, $"Check Masters: {result} Has Masters: {Masters != null} Record Found: {hasEntry} Not: {isNeg}");

                if (!result)
                    return false;
            }

            return true;
        }

        public virtual bool Validate ()
        {
            if (Debug)
                LogHelper.Log(LogLevel.Trace, ClassLogCode, "Trace logging enabled for this rule.", rule: this);

            if (Masters.SafeAny() && Masters.Any(m => m.Operation == ListLogic.NOT) && Masters.Any(m => m.Operation != ListLogic.NOT))
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, "Rule includes both include and exclude masters, which does not compute.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates Lists to make sure it is null if list contains 0 entries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>Cleaned List.</returns>
        protected static List<T>? ValidateList<T> ( List<T>? list ) => list.SafeAny() ? list : null;
    }
}