using System.Collections.ObjectModel;

using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    [JsonConverter(typeof(OperationsConverter))]
    public class OperationFormLink : OperationBase<ListLogic>
    {
        public readonly FormKey FormKey;
        public readonly ListLogic Operation;

        private static readonly ReadOnlyDictionary<char, ListLogic> ValidPrefixes = new(new Dictionary<char, ListLogic>() {
            { '-', ListLogic.DEL },
            { '!', ListLogic.NOT },
            { '+', ListLogic.ADD } });

        public OperationFormLink ( string value )
        {
            var split = Split(value, ValidPrefixes);

            Operation = split.Item1;
            FormKey = FormKey.Factory(FormKeyConverter.FixFormKey(split.Item2));
        }

        public override bool Equals ( object? obj )
            => obj is OperationFormLink other
            && Operation == other.Operation
            && FormKey.Equals(other.FormKey);

        public override int GetHashCode () => HashCode.Combine(Operation, FormKey);

        public override string? ToString ()
        {
            char? prefix = Operation switch
            {
                ListLogic.DEL => '-',
                _ => null,
            };

            return prefix != null ? prefix + FormKey.ToString() : FormKey.ToString();
        }
    }
}