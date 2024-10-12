using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Noggog;

namespace GenericSynthesisPatcher.Json.Data
{
    public partial class GSPRule
    {
        internal Dictionary<ValueKey, JToken> jsonValues;
        private const int ClassLogPrefix = 0xA00;
        private Dictionary<ValueKey, object>? cache = null;
        private bool forwardIndexedByField;
        private ForwardTypes forwardType;

        [JsonProperty(PropertyName = "EditorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string>? EditorID { get; set; }

        [JsonProperty(PropertyName = "Factions", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FilterFormLinks>))]
        public List<FilterFormLinks>? Factions { get; set; }

        [JsonProperty(PropertyName = "FactionsOp", NullValueHandling = NullValueHandling.Ignore)]
        public Operation FactionsOp { get; set; }

        /// <summary>
        /// Add Fill action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Fill", NullValueHandling = NullValueHandling.Ignore)]
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

        [JsonProperty(PropertyName = "FormID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKey>))]
        public List<FormKey>? FormID { get; set; }

        /// <summary>
        /// Add Forward action(s) to this rule.
        /// This is only used for adding to joint Fill/Forward store.
        /// </summary>
        [JsonProperty(PropertyName = "Forward", NullValueHandling = NullValueHandling.Ignore)]
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
        /// Changes JSON format for Forward from being the default "mod.esp" : [ "field1", "field2" ]
        /// to be "field": [ "Mod1.esp", "Mod2.esp" ]
        /// </summary>
        [JsonProperty(PropertyName = "ForwardIndexedByField", NullValueHandling = NullValueHandling.Ignore)]
        public bool ForwardIndexedByField
        {
            get => forwardIndexedByField;
            set
            {
                if (HasForwardType(ForwardTypeFlags.IndexedByField) && !value)
                    throw new ArgumentException("", "ForwardIndexedByField");
                forwardIndexedByField = value;
            }
        }

        /// <summary>
        /// ForwardType can change how Forwarding actions work.
        /// </summary>
        [JsonProperty(PropertyName = "ForwardType", NullValueHandling = NullValueHandling.Ignore)]
        public ForwardTypes ForwardType
        {
            get => forwardType;
            set
            {
                forwardType = value;
                if (HasForwardType(ForwardTypeFlags.IndexedByField))
                    forwardIndexedByField = true;
            }
        }

        [JsonProperty(PropertyName = "Keywords", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string>? Keywords { get; set; }

        [JsonProperty(PropertyName = "KeywordsOp", NullValueHandling = NullValueHandling.Ignore)]
        public Operation KeywordsOp { get; set; }

        [JsonProperty(PropertyName = "Masters", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<ModKey>))]
        public List<ModKey>? Masters { get; set; }

        [JsonProperty(PropertyName = "-EditorID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string>? NotEditorID { get; set; }

        [JsonProperty(PropertyName = "-FormID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FormKey>))]
        public List<FormKey>? NotFormID { get; set; }

        [JsonProperty(PropertyName = "OnlyIfDefault", NullValueHandling = NullValueHandling.Ignore)]
        public bool OnlyIfDefault { get; set; }

        [DefaultValue(0)]
        [JsonProperty(PropertyName = "Priority", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Priority { get; set; }

        [JsonProperty(PropertyName = "Types", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<GSPRule.Type>))]
        public List<GSPRule.Type>? Types { get; set; }

        /// <summary>
        /// Create new rule, and validates it meets basic needs.
        /// </summary>
        public GSPRule ( int priority, List<GSPRule.Type>? types, List<FormKey>? formID, List<string>? editorID, List<FormKey>? notFormID, List<string>? notEditorID, JObject? fill, JObject? forward, List<FilterFormLinks>? factions, Operation? factionsOp, List<ModKey>? masters, ForwardTypes forwardType, List<string>? keywords, Operation? keywordsOp )
        {
            jsonValues = [];

            Priority = priority;
            Types = ValidateList(types);
            FormID = ValidateList(formID);
            EditorID = ValidateList(editorID);
            NotFormID = ValidateList(notFormID);
            NotEditorID = ValidateList(notEditorID);
            Masters = ValidateList(masters);
            Keywords = ValidateList(keywords);
            Factions = ValidateList(factions);
            KeywordsOp = (keywordsOp == null) ? Operation.OR : (Operation)keywordsOp;
            FactionsOp = (factionsOp == null) ? Operation.OR : (Operation)factionsOp;
            Fill = fill;
            Forward = forward;
            ForwardType = forwardType;

            if (types == null && formID == null && editorID == null)
                throw new JsonSerializationException("Each Json record must contain at least one basic filter (types, editorID or formID)");

            if (factions != null && (types == null || types.Count != 1 || !types.Contains(GSPRule.Type.NPC)))
                throw new JsonSerializationException("When using inFaction Type must = \"NPC\" only.");
        }

        /// <summary>
        /// What action to take.
        /// </summary>
        public enum ActionType
        {
            Fill,
            Forward
        }

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

        public bool HasForwardType ( ForwardTypeFlags flag ) => ((ForwardTypeFlags)ForwardType).HasFlag(flag);

        /// <summary>
        /// Checks if rule filter(s) match current context's record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        public bool Matches ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, out IMajorRecordGetter? Origin )
        {
            bool matches = MatchesBasicFilters(context)
                        && MatchesExtraFilters(context)
                        && MatchesFactions(context)
                        && MatchesKeywords(context);

            Origin = (matches && OnlyIfDefault) ? Mod.FindOrigin(context) : null;

            if (Global.Settings.Value.TraceFormKey != null && context.Record.FormKey.Equals(Global.Settings.Value.TraceFormKey))
            {
                LogHelper.Log(LogLevel.Trace, context, $"MatchesBasicFilters: {MatchesBasicFilters(context)} HasTypes: {Types != null} HasEditorID: {EditorID != null} HasFormID: {FormID != null}", ClassLogPrefix | 0x11);
                LogHelper.Log(LogLevel.Trace, context, $"MatchesExtraFilters: {MatchesExtraFilters(context)} HasNotEditorID: {NotEditorID != null} HasNotFormID: {NotFormID != null} HasMasters: {Masters != null}", ClassLogPrefix | 0x12);
                LogHelper.Log(LogLevel.Trace, context, $"MatchesFactions: {MatchesFactions(context)} HasFactions: {Factions != null}", ClassLogPrefix | 0x13);
                LogHelper.Log(LogLevel.Trace, context, $"MatchesKeywords: {MatchesKeywords(context)} HasKeywords: {Keywords != null}", ClassLogPrefix | 0x14);
                LogHelper.Log(LogLevel.Trace, context, $"HasOrigin: {Origin != null} OnlyIfDefault: {OnlyIfDefault}", ClassLogPrefix | 0x15);
            }

            return matches;
        }

        private static bool MatchesFaction ( INpcGetter record, FilterFormLinks check )
        {
            // If no factions on record return result based on if +/-
            if (!record.Factions.SafeAny())
                return false;

            foreach (var f in record.Factions)
            {
                if (f.Faction.Equals(check.FormKey))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MatchesKeyword ( IKeywordedGetter record, string check, bool neg )
        {
            // If no keywords on record return result based on if +/-
            if (!record.Keywords.SafeAny())
                return false;

            string keywordStr = (neg || check.StartsWith('+'))? check[1..] : check;
            var keyword = Helpers.Action.Keywords.GetKeyword(keywordStr);

            return keyword != null && record.Keywords.Contains(keyword);
        }

        /// <summary>
        /// Validates Lists to make sure it is null if list contains 0 entries and no duplicates.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>Cleaned List.</returns>
        private static List<T>? ValidateList<T> ( List<T>? list ) => list.SafeAny() ? list.Distinct().ToList() : null;

        /// <summary>
        /// Checks basic filters.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns>Returns true if context matches basic filters.</returns>
        private bool MatchesBasicFilters ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            var recordType = GetGSPRuleType(context.Record);
            if (recordType == GSPRule.Type.UNKNOWN)
                return false;

            if (Types != null && !Types.Contains(recordType))
                return false;

            if (FormID != null && !FormID.Contains(context.Record.FormKey))
                return false;

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
        /// Checks Extra filters.
        /// Excludes OnlyIfDefault as that must be checked by the appropriate Actions if all other filters match.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Return true if context matches any defined extra filters.</returns>
        private bool MatchesExtraFilters ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            if (Masters != null && !Masters.Contains(context.Record.FormKey.ModKey))
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

            return true;
        }

        /// <summary>
        /// Checks is matches and faction filters.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        private bool MatchesFactions ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            if (Factions == null)
                return true;

            // Must have only matched NPCs due to validation in constructor
            if (context.Record is not INpcGetter record)
                return false;

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            foreach (var check in Factions)
            {
                if (!check.Neg)
                    includesChecked++;

                if (MatchesFaction(record, check))
                {
                    if (check.Neg)
                        return false;

                    if (FactionsOp == Operation.OR)
                        return true;

                    matchedCount++;
                }
                else if (!check.Neg && FactionsOp == Operation.AND)
                {
                    return false;
                }
            }

            return FactionsOp switch
            {
                Operation.OR => includesChecked == 0,
                Operation.AND => true,
                _ => matchedCount == 1 // XOR
            };
        }

        /// <summary>
        /// Checks is matches and keyword filters.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Origin"></param>
        /// <returns></returns>
        private bool MatchesKeywords ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
        {
            if (Keywords == null)
                return true;

            // Must be Keyworded for Keywords to match
            if (context.Record is not IKeywordedGetter record)
                return false;

            int matchedCount = 0;
            int includesChecked = 0; // Only count !Neg

            foreach (string check in Keywords)
            {
                bool neg = check.StartsWith('-');
                if (!neg)
                    includesChecked++;

                if (MatchesKeyword(record, check, neg))
                {
                    if (neg)
                        return false;

                    if (KeywordsOp == Operation.OR)
                        return true;

                    matchedCount++;
                }
                else if (!neg && KeywordsOp == Operation.AND)
                {
                    return false;
                }
            }

            return KeywordsOp switch
            {
                Operation.OR => includesChecked == 0,
                Operation.AND => true,
                _ => matchedCount == 1 // XOR
            };
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

            public static bool operator != ( ValueKey left, ValueKey right ) => !(left == right);

            public static bool operator == ( ValueKey left, ValueKey right ) => left.Equals(right);

            public override bool Equals ( [NotNullWhen(true)] object? obj )
                                        => obj is ValueKey key
                   && ActionType == key.ActionType
                   && Key.Equals(key.Key);

            public override readonly int GetHashCode () => Key.GetHashCode();
        }
    }
}