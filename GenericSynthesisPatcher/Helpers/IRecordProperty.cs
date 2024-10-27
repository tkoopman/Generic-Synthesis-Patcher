namespace GenericSynthesisPatcher.Helpers
{
    internal interface IRecordProperty
    {
        public string PropertyName { get; }

        public Type? Type { get; }
    }
}