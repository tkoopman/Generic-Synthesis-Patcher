using System.Reflection;

using Common;

using GenericSynthesisPatcher.Helpers;

using GSPTestProject.GameData.GlobalGame.Fixtures;
using GSPTestProject.Helpers;
using GSPTestProject.StaticData;

using Loqui;

using Mutagen.Bethesda.Plugins.Records;

namespace GSPTestProject
{
    public class TranslationMaskFactoryTests : IClassFixture<SkyrimSEFixture>
    {
        private readonly TranslationMaskConverter_Comparer _translationMaskComparer = new();

        [Theory]
        [ClassData(typeof(TranslationMaskFactory_TestData.SetValueAdvanced_TestData))]
        public void SetValueAdvanced_Tests (ITranslationMask startingMask, Dictionary<string, (object?, bool)> setFields, ITranslationMask? expectedResult)
        {
            foreach (var (fieldName, (value, expected)) in setFields)
            {
                switch (value)
                {
                    case bool boolValue:
                        Assert.Equal(expected, startingMask.TrySetValue(fieldName, boolValue));
                        break;

                    case ITranslationMask maskValue:
                        Assert.Equal(expected, startingMask.TrySetValue(fieldName, maskValue));
                        break;

                    case GenderedItem<bool> gbValue:
                        Assert.Equal(expected, startingMask.TrySetValue(fieldName, gbValue));
                        break;

                    case null:
                        Assert.Equal(expected, startingMask.TrySetValue(fieldName, (ITranslationMask?)null));
                        break;

                    default:
                        var exploded = value.GetType().Explode(2);
                        Assert.Equal(2, exploded.Length);

                        var methods = typeof(TranslationMaskFactory).GetMethods(BindingFlags.Static | BindingFlags.Public);
                        methods = [.. methods.Where(m => m.IsGenericMethod && m.ContainsGenericParameters && m.Name.Equals("TrySetValue"))];

                        var setMethod = Assert.Single(methods).MakeGenericMethod([exploded[1]]);

                        Assert.Equal(expected, setMethod.Invoke(null, [startingMask, fieldName, value, StringComparison.Ordinal]));
                        break;
                }
            }

            Assert.Equal(expectedResult, startingMask, _translationMaskComparer);
        }

        [Theory]
        [ClassData(typeof(TranslationMaskFactory_TestData.TryCreate_TestData))]
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