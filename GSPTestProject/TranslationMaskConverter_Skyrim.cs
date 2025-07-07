using GenericSynthesisPatcher.Games.Universal.Json.Converters;

using GSPTestProject.GameData.GlobalGame.Fixtures;
using GSPTestProject.Helpers;
using GSPTestProject.StaticData;

using Loqui;

using Newtonsoft.Json;

namespace GSPTestProject
{
    public class TranslationMaskConverter_Skyrim : IClassFixture<SkyrimSEFixture>
    {
        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            Converters = [new TranslationMaskConverter(), new GenderedItemConverter()],
        };

        private readonly TranslationMaskConverter_Comparer _translationMaskComparer = new();

        [Theory]
        [ClassData(typeof(TranslationMaskConverter_SkyrimData))]
        public void TranslationMaskConverter_Theory (string testName, string json, ITranslationMask expected)
        {
            // Test boolean input
            var mask = JsonConvert.DeserializeObject(json, expected.GetType(), _serializerSettings) as ITranslationMask;
            Assert.NotNull(mask);

            Assert.Equal(expected, mask, _translationMaskComparer);
        }
    }
}