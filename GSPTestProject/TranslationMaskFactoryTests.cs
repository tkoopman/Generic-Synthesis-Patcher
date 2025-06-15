using GenericSynthesisPatcher.Helpers;

using GSPTestProject.Helpers;
using GSPTestProject.StaticData;

using Loqui;

using Mutagen.Bethesda.Plugins.Records;

namespace GSPTestProject
{
    public class TranslationMaskFactoryTests
    {
        private readonly TranslationMaskConverter_Comparer _translationMaskComparer = new();

        [Theory]
        [ClassData(typeof(TranslationMaskFactory_TestData))]
        public void TryCreate_Tests (Type type, bool defaultOn, bool? onOverall, IEnumerable<string> toggleEntries, StringComparison comparison, bool expected, ITranslationMask? expectedMask)
        {
            // Act
            bool result = TranslationMaskFactory.tryCreateInternal(type, defaultOn, onOverall, toggleEntries, comparison, out var mask);

            // Assert
            Assert.Equal(expected, result);
            if (expectedMask is null)
            {
                Assert.Null(mask);
            }
            else
            {
                Assert.NotNull(mask);
                Assert.Equal(expectedMask, mask, _translationMaskComparer);
                Assert.IsAssignableFrom(typeof(MajorRecord.TranslationMask), mask);
            }
        }
    }
}