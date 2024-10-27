using Loqui;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers
{
    public readonly struct RecordTypeMapping : IEquatable<RecordTypeMapping>, IEquatable<RecordType>, IEquatable<ILoquiRegistration>, IEquatable<string>
    {
        public readonly WinningContextOverridesDelegate WinningContextOverrides;
        public string FullName => StaticRegistration.Name;
        public string Name => Type.CheckedType;
        public ILoquiRegistration StaticRegistration { get; }
        public RecordType Type { get; }

        public RecordTypeMapping (ILoquiRegistration staticRegistration, WinningContextOverridesDelegate winningContextOverrides)
        {
            if (staticRegistration.GetType().GetField("TriggeringRecordType")?.GetValue(StaticRegistration) is not RecordType recordType)
                throw new ArgumentException("Doesn't map to RecordType", nameof(staticRegistration));
            Type = recordType;
            StaticRegistration = staticRegistration;
            WinningContextOverrides = winningContextOverrides;
        }

        public delegate IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>> WinningContextOverridesDelegate ();

        public static bool operator != (RecordTypeMapping r1, RecordTypeMapping r2) => !r1.Equals(r2);

        public static bool operator == (RecordTypeMapping r1, RecordTypeMapping r2) => r1.Equals(r2);

        public bool Equals (RecordTypeMapping other) => Type.Equals(other.Type);

        public bool Equals (RecordType other) => Type.Equals(other);

        public bool Equals (ILoquiRegistration? other) => StaticRegistration == other;

        public bool Equals (string? other) => Type.Equals(other);

        public override bool Equals (object? obj) => obj is RecordTypeMapping other && Equals(other);

        public override int GetHashCode () => Type.TypeInt;
    }
}