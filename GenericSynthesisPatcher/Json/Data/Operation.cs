namespace GenericSynthesisPatcher.Json.Data
{
    /// <summary>
    /// Operation to apply to an action.
    /// Assigning invalid operation for an action should just use Default instead.
    /// </summary>
    public enum Operation
    {
        Default,
        NOT, // !
        AND, // &
        XOR, // ^
        OR = Default, // |
        ADD = Default, // +
        Remove = NOT // -
    }
}