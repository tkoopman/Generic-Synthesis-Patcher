using System.Diagnostics.CodeAnalysis;

using Common;

using Loqui;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Games.Universal
{
    public abstract class RecordTypeMappings
    {
        private const int ClassLogCode = 0x06;
        private readonly List<RecordTypeMapping> all = [];
        private readonly Dictionary<string, RecordTypeMapping> ByName = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Type, RecordTypeMapping> ByType = [];

        public IReadOnlyList<RecordTypeMapping> All => all.AsReadOnly();

        public RecordTypeMapping FindByName (string? value)
            => TryFindByName(value, out var mapping) ? mapping
             : throw new InvalidCastException($"Record Type not found by name: \"{value}\"");

        public RecordTypeMapping FindByObjectType (object obj)
            => FindByType(obj.GetType());

        public RecordTypeMapping FindByType (Type type)
                    => TryFindByType(type, out var mapping) ? mapping
             : throw new InvalidCastException($"No Record Type implements: \"{type.Name}\"");

        public RecordTypeMapping FindByType (RecordType type)
            => TryFindByName(type.Type, out var mapping) ? mapping
             : throw new InvalidCastException($"No Record Type found: \"{type.Type}\"");

        public bool TryFindByName (string? value, out RecordTypeMapping mapping)
        {
            mapping = default;
            return value is not null && ByName.TryGetValue(value, out mapping);
        }

        public bool TryFindByType (IMajorRecordGetter record, out RecordTypeMapping mapping)
        {
            if (!TryFindByType(record.GetType(), out mapping))
            {
                Global.TraceLogger?.Log(ClassLogCode, "Find Types: Type Unknown");
                return false;
            }

            return true;
        }

        public bool TryFindByType (Type type, out RecordTypeMapping mapping)
            => ByType.TryGetValue(type, out mapping);

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        protected void Add (ILoquiRegistration staticRegistration, RecordTypeMapping.WinningContextOverridesDelegate getWinningContexts)
        {
            var map = new RecordTypeMapping(staticRegistration, getWinningContexts);
            ByName.Add(map.Type.Type, map);
            if (!map.Type.Type.Equals(map.StaticRegistration.Name, StringComparison.OrdinalIgnoreCase))
            {
                ByName.Add(map.StaticRegistration.Name, map);
                if (!map.StaticRegistration.Name.Equals(map.StaticRegistration.Name.SeparateWords(), StringComparison.OrdinalIgnoreCase))
                    ByName.Add(map.StaticRegistration.Name.SeparateWords(), map);
            }

            ByType.Add(map.StaticRegistration.GetterType, map);
            ByType.Add(map.StaticRegistration.SetterType, map);

            all.Add(map);
        }
    }
}