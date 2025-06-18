using System.Diagnostics.CodeAnalysis;

namespace GenericSynthesisPatcher.Games.Universal
{
    public readonly struct PropertyAliasMapping (Type? type, string propertyName, string? realPropertyName)
    {
        public string PropertyName { get; } = propertyName;
        public string? RealPropertyName { get; } = realPropertyName;
        public Type? Type { get; } = type;

        public static bool operator != (PropertyAliasMapping left, PropertyAliasMapping right) => !(left == right);

        public static bool operator == (PropertyAliasMapping left, PropertyAliasMapping right) => left.Equals(right);

        public override bool Equals ([NotNullWhen(true)] object? obj) => obj is PropertyAliasMapping p && Equals(p);

        public bool Equals (PropertyAliasMapping other) => Type == other.Type && PropertyName.Equals(other.PropertyName, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode () => (Type?.GetHashCode() ?? 0) ^ PropertyName.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}