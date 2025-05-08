using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Games.Universal.Action;
using GenericSynthesisPatcher.Games.Universal.Json.Data;
using GenericSynthesisPatcher.Games.Universal.Json.Operations;
using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher
{
    /// <summary>
    ///     Stores details about current record, rule and property being processed. Used as single
    ///     variable to pass around between functions.
    /// </summary>
    /// <param name="rtm">Current record type mapping</param>
    /// <param name="context">Current record context</param>
    /// <param name="parent">
    ///     Parent key for when processing groups. Parent is always for the same context.
    /// </param>
    public class ProcessingKeys (RecordTypeMapping rtm, IModContext<IMajorRecordGetter> context, ProcessingKeys? parent = null)
    {
        private const int ClassLogCode = 0xFF;
        private IMajorRecordGetter? origin;
        private IMajorRecord? patchRecord;
        private Random? random;
        private FilterOperation? ruleKey;

        /// <summary>
        ///     Current record context being processed
        /// </summary>
        public IModContext<IMajorRecordGetter> Context { get; } = context;

        /// <summary>
        ///     Current rule group being processed if current rule belongs to a group, else null
        /// </summary>
        public GSPGroup? Group => RuleBase is GSPGroup group ? group : Rule.Group;

        /// <summary>
        ///     Checks if we have created a patched record for the current context yet. If parent
        ///     exists will call against parent.
        /// </summary>
        public bool HasPatchRecord => Parent == null ? patchRecord != null : Parent.HasPatchRecord;

        /// <summary>
        ///     Is current rule being processed a group of rules
        /// </summary>
        [MemberNotNullWhen(true, nameof(Group))]
        public bool IsGroup => RuleBase is GSPGroup;

        /// <summary>
        ///     Is current rule being processed a normal rule and not a group. May be standard rule
        ///     inside a group.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Rule))]
        public bool IsRule => RuleBase is GSPRule;

        /// <summary>
        ///     Current property being processed on current context.
        /// </summary>
        public RecordPropertyMapping Property { get; private set; }

        /// <summary>
        ///     Gets current record but if a patch record already exists it will return the patched
        ///     record, so any checks are done against updated values.
        /// </summary>
        public IMajorRecordGetter Record => HasPatchRecord ? (patchRecord ?? GetPatchRecord()) : (IMajorRecordGetter)(Context.Record ?? throw new Exception());

        /// <summary>
        ///     Gets current rule being processed. If current is a group and not a rule will throw
        ///     InvalidOperationException
        /// </summary>
        public GSPRule Rule => RuleBase as GSPRule ?? throw new InvalidOperationException("Rule not currently set");

        /// <summary>
        ///     Current key for current rule. Rule could be keyed by property name or mod.
        /// </summary>
        public FilterOperation RuleKey { get => ruleKey ?? throw new InvalidOperationException("Property not set"); private set => ruleKey = value; }

        /// <summary>
        ///     Gets current record type mapping being processed
        /// </summary>
        public RecordTypeMapping Type { get; } = rtm;

        /// <summary>
        ///     Parent processing key
        /// </summary>
        private ProcessingKeys? Parent { get; } = parent;

        /// <summary>
        ///     Rule which could be a group or standard rule
        /// </summary>
        private GSPBase? RuleBase { get; set; }

        /// <summary>
        ///     If rule is set to OnlyIfDefault, check if current record matches origin.
        /// </summary>
        /// <returns>
        ///     True if rule is set to OnlyIfDefault and current record matches origin, else false
        /// </returns>
        public bool CheckOnlyIfDefault ()
        {
            if (!Rule.OnlyIfDefault)
                return false;

            bool result = false;

            Global.TraceLogger?.LogAction(ClassLogCode, $"{Property.Action.GetType().GetClassName()}.{nameof(IRecordAction.MatchesOrigin)}", propertyName: Property.PropertyName);

            if (!Property.Action.MatchesOrigin(this))
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.OriginMismatch, propertyName: Property.PropertyName);
                result = true;
            }

            return result;
        }

        /// <summary>
        ///     Gets the master of the current record, and caches result
        /// </summary>
        /// <returns>
        ///     Overwritten master record. Null if current record context is the master.
        /// </returns>
        public IMajorRecordGetter GetOriginRecord ()
        {
            origin ??= Mod.FindOriginContext(Context).Record;
            return origin;
        }

        /// <summary>
        ///     Gets cached patched record or creates and caches if first time called for this
        ///     current record.
        /// </summary>
        /// <returns>Editable version of current record</returns>
        public IMajorRecord GetPatchRecord ()
        {
            if (Parent == null)
            {
                patchRecord = Context switch
                {
                    //TODO Add other games
                    IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> skyrimContext => skyrimContext.GetOrAddAsOverride((ISkyrimMod)Global.State.PatchMod),
                    _ => throw new InvalidCastException(),
                };
            }
            else
            {
                patchRecord ??= Parent.GetPatchRecord();
            }

            return patchRecord;
        }

        /// <summary>
        ///     Create a seeded pseudo-random number generator, which is an algorithm that produces
        ///     a sequence of numbers that meet certain statistical requirements for randomness.
        ///
        ///     Seeded based on current record type, form key, rule and property.
        /// </summary>
        /// <returns>Seeded Random class</returns>
        public Random GetRandom ()
        {
            random ??= new(Type.Type.TypeInt ^ Record.FormKey.GetHashCode() ^ (RuleBase?.GetHashCode() ?? 0) ^ Property.PropertyName.GetHashCode());
            return random;
        }

        /// <summary>
        ///     Sets the current property being looked at by either Matches or Action.
        /// </summary>
        /// <returns>True if RPM for property found for current record type</returns>
        [MemberNotNullWhen(true, nameof(RuleKey))]
        [MemberNotNullWhen(true, nameof(Property))]
        [MemberNotNullWhen(true, nameof(RuleBase))]
        public bool SetProperty (FilterOperation ruleKey, string name)
        {
            if (RuleBase == null)
                throw new InvalidOperationException("Must set rule first.");

            if (Global.RecordPropertyMappings.TryFind(Type.StaticRegistration.GetterType, name, out var property))
            {
                Property = property;
                RuleKey = ruleKey;
                return true;
            }

            Property = default;
            this.ruleKey = null;
            return false;
        }

        /// <summary>
        ///     Sets the current rule being looked and resets Properties and RuleKey.
        /// </summary>
        /// <returns>True if RPM for property found for current record type</returns>
        [MemberNotNullWhen(true, nameof(RuleBase))]
        public bool SetRule (GSPBase rule)
        {
            RuleBase = rule;
            Property = default;
            random = null;
            ruleKey = null;
            return true;
        }

        /// <summary>
        ///     Get the fill data for a current rule's action value key parsed to selected class
        ///     type.
        /// </summary>
        /// <typeparam name="T">Type of the output data to read from JSON</typeparam>
        /// <param name="valueAs">Output data</param>
        /// <returns>Current rule's fill data</returns>
        /// <exception cref="InvalidOperationException">
        ///     If property not set, then unable to get value.
        /// </exception>
        public bool TryGetFillValueAs<T> (out T? valueAs) => RuleKey is not null ? Rule.TryGetFillValueAs(RuleKey, out valueAs) : throw new InvalidOperationException("Property not set");

        /// <summary>
        ///     Get the match data for a current rule's action value key parsed to selected class
        ///     type.
        /// </summary>
        /// <typeparam name="T">Type of the output data to read from JSON</typeparam>
        /// <param name="valueAs">Output data</param>
        /// <returns>Current rule's match data</returns>
        /// <exception cref="InvalidOperationException">
        ///     If property not set, then unable to get value.
        /// </exception>
        public bool TryGetMatchValueAs<T> (out bool fromCache, out T? valueAs) => RuleKey is not null ? Rule.TryGetMatchValueAs(RuleKey, out fromCache, out valueAs) : throw new InvalidOperationException("Property not set");
    }
}