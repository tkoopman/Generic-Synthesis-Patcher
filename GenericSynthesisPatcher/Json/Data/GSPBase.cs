using System.ComponentModel;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
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
        private const int ClassLogPrefix = 0xB00;

        [JsonProperty(PropertyName = "Masters", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKey>))]
        public List<ModKey>? Masters { get; private set; }

        [DefaultValue(0)]
        [JsonProperty(PropertyName = "Priority", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Priority { get; private set; }

        [JsonProperty(PropertyName = "Types", NullValueHandling = NullValueHandling.Ignore)]
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

            if (Global.Settings.Value.TraceFormKey != null && context.Record.FormKey.Equals(Global.Settings.Value.TraceFormKey))
            {
                LogHelper.Log(LogLevel.Trace, context, $"RecordType: {recordType}", ClassLogPrefix | 0x10);
                LogHelper.Log(LogLevel.Trace, context, $"Check Types: {Types.HasFlag(recordType)}", ClassLogPrefix | 0x10);
                LogHelper.Log(LogLevel.Trace, context, $"Check Masters: {Masters == null || Masters.Contains(context.Record.FormKey.ModKey)}", ClassLogPrefix | 0x10);
            }

            return recordType != RecordTypes.UNKNOWN
                && Types.HasFlag(recordType)
                && (Masters == null || Masters.Contains(context.Record.FormKey.ModKey));
        }

        public abstract bool Validate ();

        /// <summary>
        /// Validates Lists to make sure it is null if list contains 0 entries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>Cleaned List.</returns>
        protected static List<T>? ValidateList<T> ( List<T>? list ) => list.SafeAny() ? list : null;
    }
}