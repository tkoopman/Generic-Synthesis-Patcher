using GenericSynthesisPatcher.Games.Universal.Action;

namespace GenericSynthesisPatcher.Games.Universal
{
    public readonly struct RecordPropertyMapping (Type? type, string propertyName, IRecordAction action) : IRecordProperty
    {
        public IRecordAction Action { get; } = action;

        public string PropertyName { get; } = propertyName;
        public Type? Type { get; } = type;
    }
}