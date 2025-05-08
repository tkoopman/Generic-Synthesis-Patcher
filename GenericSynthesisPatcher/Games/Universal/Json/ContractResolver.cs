using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json.Serialization;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Json
{
    public abstract class ContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract (Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (objectType == typeof(FormKey))
                contract.Converter = new FormKeyConverter();
            else if (objectType == typeof(ModKey))
                contract.Converter = new ModKeyConverter();
            else if (objectType == typeof(RecordTypeMapping))
                contract.Converter = new RecordTypeConverter();
            else if (objectType.IsGenericType && objectType.GetGenericTypeDefinition().IsAssignableTo(typeof(OperationBase<,>)))
                contract.Converter = new OperationsConverter();
            else if (objectType == typeof(Percent))
                contract.Converter = new PercentConverter();
            else if (objectType == typeof(System.Drawing.Color))
                contract.Converter = new ColorConverter();

            return contract;
        }
    }
}