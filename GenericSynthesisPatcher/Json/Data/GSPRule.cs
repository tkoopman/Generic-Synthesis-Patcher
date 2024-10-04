using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Noggog;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace GenericSynthesisPatcher.Json.Data
{
    public partial class GSPRule
    {
        /// <summary>
        /// What action to take.
        /// </summary>
        public enum ActionType
        {
            Fill,
            Forward
        }

        /// <summary>
        /// Key used for storing actions.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="key"></param>
        public readonly struct ValueKey ( ActionType actionType, string key )
        {
            public readonly ActionType ActionType = actionType;
            public readonly string Key = key.ToLower();

            public override bool Equals ( [NotNullWhen(true)] object? obj )
                => obj is ValueKey key
                   && ActionType == key.ActionType
                   && Key.Equals(key.Key);

            public override readonly int GetHashCode () => Key.GetHashCode();

            public static bool operator == ( ValueKey left, ValueKey right ) => left.Equals(right);

            public static bool operator != ( ValueKey left, ValueKey right ) => !(left == right);
        }

        internal Dictionary<ValueKey, JToken> jsonValues;
        private Dictionary<ValueKey, object>? cache = null;

        [DefaultValue(0)]
        [JsonProperty(PropertyName = "priority", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Priority;

        [JsonProperty(PropertyName = "Types", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<GSPRule.Type>))]
        public List<GSPRule.Type>? Types;

        [JsonProperty(PropertyName = "formID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKey>))]
        public List<FormKey>? FormID;

        [JsonProperty(PropertyName = "editorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string>? EditorID;

        [JsonProperty(PropertyName = "masters", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string>? Masters;

        [JsonProperty(PropertyName = "-formID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKey>))]
        public List<FormKey>? NotFormID;

        [JsonProperty(PropertyName = "-editorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string>? NotEditorID;

        [JsonProperty(PropertyName = "inFaction", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKey>))]
        public List<FormKey>? InFaction;

        [JsonProperty(PropertyName = "inFactionAnd", NullValueHandling = NullValueHandling.Ignore)]
        public bool InFactionAnd;

        [JsonProperty(PropertyName = "OnlyIfDefault", NullValueHandling = NullValueHandling.Ignore)]
        public bool OnlyIfDefault { get; set; }

        /// <summary>
        /// Add Forward action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "forward", NullValueHandling = NullValueHandling.Ignore)]
        public JObject? Forward
        {
            set
            {
                foreach (var x in value ?? [])
                {
                    if (x.Value != null)
                        jsonValues.Add(new ValueKey(ActionType.Forward, x.Key), x.Value);
                }
            }
        }

        /// <summary>
        /// Add Fill action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "fill", NullValueHandling = NullValueHandling.Ignore)]
        public JObject? Fill
        {
            set
            {
                foreach (var x in value ?? [])
                {
                    if (x.Value != null)
                        jsonValues.Add(new ValueKey(ActionType.Fill, x.Key), x.Value);
                }
            }
        }

        /// <summary>
        /// Validates Lists to make sure it is null if list contains 0 entries and no duplicates.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>Cleaned List.</returns>
        private static List<T>? ValidateList<T> ( List<T>? list ) => list != null && list.Count != 0 ? list.Distinct().ToList() : null;

        /// <summary>
        /// Create new rule, and validates it meets basic needs.
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="types"></param>
        /// <param name="formID"></param>
        /// <param name="editorID"></param>
        /// <param name="notFormID"></param>
        /// <param name="notEditorID"></param>
        /// <param name="fill"></param>
        /// <param name="forward"></param>
        /// <param name="inFaction"></param>
        /// <param name="inFactionAnd"></param>
        /// <exception cref="JsonSerializationException"></exception>
        public GSPRule ( int priority, List<GSPRule.Type>? types, List<FormKey>? formID, List<string>? editorID, List<FormKey>? notFormID, List<string>? notEditorID, JObject? fill, JObject? forward, List<FormKey>? inFaction, bool? inFactionAnd, List<string>? masters )
        {
            jsonValues = [];
            Priority = priority;
            Types = ValidateList(types);
            FormID = ValidateList(formID);
            EditorID = ValidateList(editorID);
            NotFormID = ValidateList(notFormID);
            NotEditorID = ValidateList(notEditorID);
            Fill = fill;
            Forward = forward;
            InFaction = ValidateList(inFaction);
            InFactionAnd = inFactionAnd != null && (bool)inFactionAnd;
            Masters = ValidateList(masters);

            if (types == null && formID == null && editorID == null)
                throw new JsonSerializationException("Each Json record must contain at least one filter (types, editorID or formID)");

            if (inFaction != null && (types == null || types.Count != 1 || !types.Contains(GSPRule.Type.NPC)))
                throw new JsonSerializationException("When using inFaction Type must = \"NPC\" only.");
        }

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
            IFactionGetter => "FACT",
            IIngredientGetter => "INGR",
            IKeyGetter => "KEYM",
            IMiscItemGetter => "MISC",
            INpcGetter => "NPC",
            IOutfitGetter => "OTFT",
            IScrollGetter => "SCRL",
            IWeaponGetter => "WEAP",
            _ => "UNKNOWN"
        };

        /// <summary>
        /// Return GSP rule type for a record.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static GSPRule.Type GetGSPRuleType ( IMajorRecordGetter record ) => record switch
        {
            IIngestibleGetter => GSPRule.Type.ALCH,
            IAmmunitionGetter => GSPRule.Type.AMMO,
            IArmorGetter => GSPRule.Type.ARMO,
            IBookGetter => GSPRule.Type.BOOK,
            ICellGetter => GSPRule.Type.CELL,
            IFactionGetter => GSPRule.Type.FACT,
            IIngredientGetter => GSPRule.Type.INGR,
            IKeyGetter => GSPRule.Type.KEYM,
            IMiscItemGetter => GSPRule.Type.MISC,
            INpcGetter => GSPRule.Type.NPC,
            IOutfitGetter => GSPRule.Type.OTFT,
            IScrollGetter => GSPRule.Type.SCRL,
            IWeaponGetter => GSPRule.Type.WEAP,
            _ => GSPRule.Type.UNKNOWN
        };

        /// <summary>
        /// Checks if rule filter(s) match current context's record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        public bool Matches ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, out IMajorRecordGetter? Origin )
        {
            bool matches = MatchesRefined(context);

            Origin = (matches && OnlyIfDefault) ? Mod.FindOrigin(context) : null;

            return matches;
        }

        /// <summary>
        /// Checks if more refined rule filter(s) match current context's record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        private bool MatchesRefined ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            bool matches = MatchesBaseFilters(context);

            if (!matches || InFaction == null)
                return matches;

            // Must have only matched NPCs due to validation in constructor
            var npc = (INpcGetter)context.Record;
            int inAnd = 0;
            foreach (var faction in npc.Factions)
            {
                if (InFaction.Contains(faction.Faction.FormKey))
                {
                    if (InFactionAnd)
                        inAnd++;
                    else
                        return true;
                }
            }

            return InFactionAnd && inAnd == InFaction.Count;
        }

        /// <summary>
        /// Checks if base rule filter(s) match current context's record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        private bool MatchesBaseFilters ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            var recordType = GetGSPRuleType(context.Record);
            if (recordType == GSPRule.Type.UNKNOWN)
                return false;

            if (Types != null && !Types.Contains(recordType))
                return false;

            if (Masters != null && !Masters.Contains(context.Record.FormKey.ModKey.FileName.String))
                return false;

            if (FormID != null && !FormID.Contains(context.Record.FormKey))
                return false;

            if (NotFormID != null && NotFormID.Contains(context.Record.FormKey))
                return false;

            if (NotEditorID != null)
            {
                foreach (string editorID in NotEditorID)
                {
                    if (editorID.StartsWith('/') && editorID.EndsWith('/'))
                    {
                        var matchedRegex = new Regex(editorID.Trim('/'));
                        if (matchedRegex.IsMatch(context.Record.EditorID ?? ""))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (editorID.Equals(context.Record.EditorID))
                        {
                            return false;
                        }
                    }
                }
            }

            // Can assume true if no EditorID filter as GSPRule MUST contain 1 filter
            // Which means one of the above checks must of have contained a matching value
            if (EditorID == null)
                return true;

            foreach (string editorID in EditorID)
            {
                if (editorID.StartsWith('/') && editorID.EndsWith('/'))
                {
                    var matchedRegex = new Regex(editorID.Trim('/'));
                    if (matchedRegex.IsMatch(context.Record.EditorID ?? ""))
                        return true;
                }
                else if (editorID.Equals(context.Record.EditorID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the raw string value data for a selected rule's action value key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string? GetValueAsString ( ValueKey key )
            => jsonValues.TryGetValue(key, out var jsonValue)
                  ? jsonValue.Type == JTokenType.String
                      ? jsonValue.ToString()
                      : throw new InvalidOperationException($"Invalid value type returned for {key.Key}")
                  : null;

        /// <summary>
        /// Get the value data for a selected rule's action value key parsed to selected class type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T? GetValueAs<T> ( ValueKey key )
        {
            cache ??= [];
            if (cache.TryGetValue(key, out object? value))
            {
                return value is T v ? v : throw new InvalidOperationException($"Invalid value type returned for {key.Key}");
            }

            if (jsonValues.TryGetValue(key, out var jsonValue))
            {
                if (JsonConvert.DeserializeObject<T>(jsonValue.ToString(), Global.SerializerSettings) is T value2)
                {
                    cache.Add(key, value2);
                    return value2;
                }
            }

            return default;
        }
    }
}