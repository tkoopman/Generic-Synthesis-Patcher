using Loqui;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Helpers
{
    [JsonObject(MemberSerialization.OptIn)]
    public readonly struct RecordTypeMapping : IEquatable<RecordTypeMapping>, IEquatable<RecordType>, IEquatable<ILoquiRegistration>, IEquatable<string>
    {
        public RecordTypeMapping (ILoquiRegistration staticRegistration, WinningContextOverridesDelegate winningContextOverrides)
        {
            if (staticRegistration.GetType().GetField("TriggeringRecordType")?.GetValue(StaticRegistration) is not RecordType recordType)
                throw new ArgumentException("Doesn't map to RecordType", nameof(staticRegistration));
            Type = recordType;
            StaticRegistration = staticRegistration;
            WinningContextOverrides = winningContextOverrides;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "<Pending>")]
        public delegate IEnumerable<IModContext<IMajorRecordGetter>> WinningContextOverridesDelegate ();

        [JsonProperty(propertyName: "name")]
        public string DisplayName => StaticRegistration.Name.SeparateWords();

        public string FullName => StaticRegistration.Name;

        [JsonProperty(propertyName: "id")]
        public string Name => Type.CheckedType;

        public ILoquiRegistration StaticRegistration { get; }
        public RecordType Type { get; }
        public readonly WinningContextOverridesDelegate WinningContextOverrides { get; }

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