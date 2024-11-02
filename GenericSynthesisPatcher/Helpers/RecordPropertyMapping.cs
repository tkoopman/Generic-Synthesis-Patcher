using GenericSynthesisPatcher.Helpers.Action;

namespace GenericSynthesisPatcher.Helpers
{
    public readonly struct RecordPropertyMapping (Type? type, string propertyName, IRecordAction action) : IRecordProperty
    {
        public IRecordAction Action { get; } = action;

        public string PropertyName { get; } = propertyName;
        public Type? Type { get; } = type;
    }
}