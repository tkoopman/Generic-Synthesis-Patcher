using System.Collections.ObjectModel;

namespace GenericSynthesisPatcher.Games.Universal.Json.Operations
{
    public enum ListLogic
    {
        Default,
        DEL,
        ADD = Default,
        NOT = DEL
    }

    internal static class ListLogicPrefixes
    {
        internal static readonly ReadOnlyDictionary<char, ListLogic> Prefixes = new(new Dictionary<char, ListLogic>()
        {
            { '-', ListLogic.DEL },
            { '!', ListLogic.NOT },
            { '+', ListLogic.ADD }
        });
    }
}