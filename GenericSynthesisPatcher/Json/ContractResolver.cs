using GenericSynthesisPatcher.Json.Converters;

using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json.Serialization;

namespace GenericSynthesisPatcher.Json
{
    public class ContractResolver : DefaultContractResolver
    {
        public static readonly ContractResolver Instance = new();

        protected override JsonContract CreateContract ( Type objectType )
        {
            var contract = base.CreateContract(objectType);

            if (objectType == typeof(FormKey))
                contract.Converter = new FormKeyConverter();
            if (objectType == typeof(ModKey))
                contract.Converter = new ModKeyConverter();

            return contract;
        }
    }
}