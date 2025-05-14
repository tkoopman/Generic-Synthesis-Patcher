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
            else if (objectType == typeof(P2Double))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P2Float))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P2Int))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P2Int16))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P2UInt8))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P3Double))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P3Float))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P3Int))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P3Int16))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P3UInt16))
                contract.Converter = new NoggogPxConverter();
            else if (objectType == typeof(P3UInt8))
                contract.Converter = new NoggogPxConverter();

            return contract;
        }
    }
}