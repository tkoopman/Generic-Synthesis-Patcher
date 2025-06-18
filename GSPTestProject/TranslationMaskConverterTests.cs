using GenericSynthesisPatcher.Games.Universal.Json.Converters;

using GSPTestProject.Helpers;
using GSPTestProject.StaticData;

using Loqui;

using Newtonsoft.Json;

namespace GSPTestProject
{
    public class TranslationMaskConverterTests
    {
        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            Converters = [new TranslationMaskConverter()],
        };

        private readonly TranslationMaskConverter_Comparer _translationMaskComparer = new();

        [Theory]
        [ClassData(typeof(TranslationMaskConverter_TestData))]
        public void TranslationMaskConverter_Theory (string json, ITranslationMask expected)
        {
            // Test boolean input
            var mask = JsonConvert.DeserializeObject(json, expected.GetType(), _serializerSettings) as ITranslationMask;
            Assert.NotNull(mask);

            Assert.Equal(expected, mask, _translationMaskComparer);
        }
    }
}