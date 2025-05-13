using GenericSynthesisPatcher.Games.Fallout4.Json.Converters;

using Mutagen.Bethesda.Fallout4;

using Newtonsoft.Json.Serialization;

namespace GenericSynthesisPatcher.Games.Fallout4.Json
{
    public class ContractResolver : Universal.Json.ContractResolver
    {
        public static readonly ContractResolver Instance = new();

        protected override JsonContract CreateContract (Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (objectType.IsAssignableTo(typeof(IObjectBoundsGetter)))
                contract.Converter = new ObjectBoundsConverter();

            return contract;
        }
    }
}