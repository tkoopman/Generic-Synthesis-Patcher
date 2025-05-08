namespace GenericSynthesisPatcher.Games.Universal
{
    public interface IRecordProperty
    {
        public string PropertyName { get; }

        public Type? Type { get; }
    }
}