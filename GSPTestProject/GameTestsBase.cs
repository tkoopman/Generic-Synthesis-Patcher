using Common;

using GenericSynthesisPatcher;
using GenericSynthesisPatcher.Helpers;

using GSPTestProject.GameData;
using GSPTestProject.GameData.GlobalGame;

using Mutagen.Bethesda;

using Xunit.Abstractions;

namespace GSPTestProject
{
    [Collection("Sequential")]
    public abstract class GameTestsBase (ITestOutputHelper output)
    {
        protected abstract GameRelease GameRelease { get; }

        [Theory]
        [ClassData(typeof(RecordTypes))]
        public void ConfirmActionForEveryEditableProperty (GameRecordType gameRecordType)
        {
            int count = 0;
            int valid = 0;

            var recordType = gameRecordType.RecordType;
            _ = TranslationMaskFactory.TryGetTranslationMaskType(recordType, out var mask);

            var properties = gameRecordType.GetProperties();

            foreach (var property in properties)
            {
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