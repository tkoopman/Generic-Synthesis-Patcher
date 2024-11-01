using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher
{
    public class ProcessingKeys (RecordTypeMapping rtm, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ProcessingKeys? parent = null)
    {
        private IMajorRecordGetter? origin;
        private bool originSaved = false;
        private IMajorRecord? patchRecord;
        private FilterOperation? ruleKey = null;

        public IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> Context { get; } = context;
        public GSPGroup? Group => RuleBase is GSPGroup group ? group : Rule.Group;

        public bool HasPatchRecord => Parent == null ? patchRecord != null : Parent.HasPatchRecord;

        [MemberNotNullWhen(true, nameof(Group))]
        public bool IsGroup => RuleBase is GSPGroup;

        [MemberNotNullWhen(true, nameof(Rule))]
        public bool IsRule => RuleBase is GSPRule;

        public RecordPropertyMapping Property { get; private set; }

        public IMajorRecordGetter Record => HasPatchRecord ? (patchRecord ?? GetPatchRecord()) : Context.Record;

        public GSPRule Rule => RuleBase as GSPRule ?? throw new InvalidOperationException("Rule not currently set");
        public FilterOperation RuleKey { get => ruleKey ?? throw new InvalidOperationException("Property not set"); private set => ruleKey = value; }
        public RecordTypeMapping Type { get; } = rtm;
        private ProcessingKeys? Parent { get; } = parent;
        private GSPBase? RuleBase { get; set; } = null;

        public IMajorRecordGetter? GetOriginRecord ()
        {
            if (!Rule.OnlyIfDefault || originSaved)
                return origin;

            origin = Mod.FindOrigin(Context);
            originSaved = true;
            return origin;
        }

        public IMajorRecord GetPatchRecord ()
        {
            if (Parent == null)
                patchRecord ??= Context.GetOrAddAsOverride(Global.State.PatchMod);
            else
                patchRecord ??= Parent.GetPatchRecord();

            return patchRecord;
        }

        public Random GetRandom () => new(Type.Type.TypeInt ^ Record.FormKey.GetHashCode() ^ (RuleBase?.GetHashCode() ?? 0) ^ Property.PropertyName.GetHashCode());

        /// <summary>
        /// Sets the current property being looked at by either Matches or Action.
        /// </summary>
        /// <returns>True if RPM for property found for current record type</returns>
        [MemberNotNullWhen(true, nameof(RuleKey))]
        [MemberNotNullWhen(true, nameof(Property))]
        [MemberNotNullWhen(true, nameof(RuleBase))]
        public bool SetProperty (FilterOperation ruleKey, string name)
        {
            if (RuleBase == null)
                throw new InvalidOperationException("Must set rule first.");

            if (RecordPropertyMappings.TryFind(Type.StaticRegistration.GetterType, name, out var property))
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
        /// Sets the current rule being looked and resets Properties and RuleKey.
        /// </summary>
        /// <returns>True if RPM for property found for current record type</returns>
        [MemberNotNullWhen(true, nameof(RuleBase))]
        public bool SetRule (GSPBase rule)
        {
            RuleBase = rule;
            Property = default;
            ruleKey = null;
            return true;
        }

        public bool TryGetFillValueAs<T> (out T? valueAs) => RuleKey is not null ? Rule.TryGetFillValueAs(RuleKey, out valueAs) : throw new InvalidOperationException("Property not set");

        public bool TryGetMatchValueAs<T> (out bool fromCache, out T? valueAs) => RuleKey is not null ? Rule.TryGetMatchValueAs(RuleKey, out fromCache, out valueAs) : throw new InvalidOperationException("Property not set");
    }
}