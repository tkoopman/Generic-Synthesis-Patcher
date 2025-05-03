namespace GenericSynthesisPatcher.Helpers
{
    public interface IRecordProperty
    {
        public string PropertyName { get; }

        public Type? Type { get; }
    }
}