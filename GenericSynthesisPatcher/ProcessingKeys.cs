using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher
{
    public class ProcessingKeys (RecordTypeMapping rtm, GSPBase rule, IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context)
    {
        private readonly GSPBase rule = rule;
        private IMajorRecordGetter? origin;
        private bool originSaved = false;
        private IMajorRecord? patchRecord;
        private FilterOperation? ruleKey = null;

        public IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> Context { get; } = context;

        public GSPGroup? Group => rule is GSPGroup group ? group : Rule.Group;

        public bool HasPatchRecord => patchRecord != null;

        [MemberNotNullWhen(true, nameof(Group))]
        public bool IsGroup => rule is GSPGroup;

        public bool IsRule => rule is GSPRule;

        public RecordPropertyMapping Property { get; private set; }

        public IMajorRecordGetter Record => (IMajorRecordGetter?)patchRecord ?? Context.Record;

        public GSPRule Rule => rule as GSPRule ?? throw new InvalidOperationException("Rule not currently set");

        public FilterOperation RuleKey { get => ruleKey ?? throw new InvalidOperationException("Property not set"); private set => ruleKey = value; }

        public RecordTypeMapping Type { get; } = rtm;

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
            patchRecord ??= Context.GetOrAddAsOverride(Global.State.PatchMod);
            return patchRecord;
        }

        public Random GetRandom () => new(Type.Type.TypeInt ^ Record.FormKey.GetHashCode() ^ rule.GetHashCode() ^ Property.PropertyName.GetHashCode());

        [MemberNotNullWhen(true, nameof(RuleKey))]
        [MemberNotNullWhen(true, nameof(Property))]
        public bool SetProperty (FilterOperation ruleKey, string name)
        {
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

        public bool TryGetFillValueAs<T> (out T? valueAs) => RuleKey is not null ? Rule.TryGetFillValueAs(RuleKey, out valueAs) : throw new InvalidOperationException("Property not set");

        public bool TryGetMatchValueAs<T> (out bool fromCache, out T? valueAs) => RuleKey is not null ? Rule.TryGetMatchValueAs(RuleKey, out fromCache, out valueAs) : throw new InvalidOperationException("Property not set");
    }
}