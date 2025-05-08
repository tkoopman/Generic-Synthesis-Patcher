namespace GenericSynthesisPatcher.Games.Universal
{
    public readonly struct PropertyAliasMapping (Type? type, string propertyName, string? realPropertyName) : IRecordProperty
    {
        public string PropertyName { get; } = propertyName;
        public string? RealPropertyName { get; } = realPropertyName;
        public Type? Type { get; } = type;
    }
}