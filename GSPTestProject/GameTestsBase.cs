using System.Reflection;

using Common;

using GenericSynthesisPatcher;
using GenericSynthesisPatcher.Helpers;

using GSPTestProject.GameData.Universal;

using Loqui;

using Xunit.Abstractions;

namespace GSPTestProject
{
    [Collection("Sequential")]
    public abstract class GameTestsBase (ITestOutputHelper output)
    {
        [Theory]
        [ClassData(typeof(AllRecordTypes_TestData))]
        public void ConfirmActionForEveryEditableProperty (ILoquiRegistration recordType)
        {
            int count = 0;
            int valid = 0;

            _ = TranslationMaskFactory.TryGetTranslationMaskType(recordType, out var mask);

            var properties = recordType.ClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).ToList();
            properties.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var property in properties)
            {
                if (!property.CanWrite && mask?.GetField(property.Name, BindingFlags.Public | BindingFlags.Instance) is null)
                {
                    output.WriteLine($"{property.Name}: *** Read Only ***");
                    continue;
                }

                count++;

                var action = Global.Game.GetAction(recordType, property.Name);
                if (action.IsValid)
                {
                    valid++;
                    output.WriteLine($"{property.Name}: {action.Action.GetType().GetClassName()}");
                }
                else
                {
                    output.WriteLine($"{property.Name}: !!! No Action Found !!!");
                }
            }

            Assert.Equal(count, valid);
            output.WriteLine($"{recordType.Name}: {valid}/{count}");
        }
    }
}