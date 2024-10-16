namespace GenericSynthesisPatcher.Json.Data
{
    [Flags]
    public enum ForwardTypeFlags
    {
        Default = 1 << 1,
        SelfMasterOnly = 1 << 2,
        Random = 1 << 3,
        IndexedByField = 1 << 4,
        DefaultThenSelfMasterOnly = Default | SelfMasterOnly | IndexedByField,
        DefaultRandom = Default | Random | IndexedByField,
    }
}