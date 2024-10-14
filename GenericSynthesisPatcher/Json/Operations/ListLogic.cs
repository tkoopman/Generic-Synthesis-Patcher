using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericSynthesisPatcher.Json.Operations
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
        internal static readonly ReadOnlyDictionary<char, ListLogic> Prefixes = new(new Dictionary<char, ListLogic>() {
            { '-', ListLogic.DEL },
            { '!', ListLogic.NOT },
            { '+', ListLogic.ADD } });
    }
}