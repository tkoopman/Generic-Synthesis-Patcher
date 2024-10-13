using GenericSynthesisPatcher.Json.Converters;

using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
    [JsonConverter(typeof(OperationsConverter))]
    public class OperationFormLink : OperationValue
    {
        [JsonIgnore]
        public readonly FormKey FormKey;

        public OperationFormLink ( string input ) : base(input) => FormKey = FormKey.Factory(FormKeyConverter.FixFormKey(Value));
    }
}