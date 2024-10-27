using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

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
            if (objectType == typeof(RecordTypeMapping))
                contract.Converter = new RecordTypeConverter();
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition().IsAssignableTo(typeof(OperationBase<,>)))
                contract.Converter = new OperationsConverter();

            return contract;
        }
    }
}