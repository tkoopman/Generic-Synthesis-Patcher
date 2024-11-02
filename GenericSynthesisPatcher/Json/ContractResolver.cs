using System.Drawing;

using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Converters;
using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json.Serialization;

using Noggog;

using ColorConverter = GenericSynthesisPatcher.Json.Converters.ColorConverter;

namespace GenericSynthesisPatcher.Json
{
    public class ContractResolver : DefaultContractResolver
    {
        public static readonly ContractResolver Instance = new();

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
            else if (objectType.IsAssignableTo(typeof(IObjectBoundsGetter)))
                contract.Converter = new ObjectBoundsConverter();
            else if (objectType == typeof(Color))
                contract.Converter = new ColorConverter();

            return contract;
        }
    }
}