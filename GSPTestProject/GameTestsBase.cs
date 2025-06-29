using Common;

using GenericSynthesisPatcher;
using GenericSynthesisPatcher.Helpers;

using GSPTestProject.GameData;
using GSPTestProject.GameData.GlobalGame;
using GSPTestProject.Helpers;

using Mutagen.Bethesda;

using Xunit.Abstractions;

namespace GSPTestProject
{
    [Collection("Sequential")]
    public abstract class GameTestsBase
    {
        public readonly ITestOutputHelper Output;

        public GameTestsBase (ITestOutputHelper output)
        {
            Output = output;
            LogHelper.Out = new TestOutputTextWritter(output);
        }

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
                    Output.WriteLine($"{property.Name}: {action.Action.GetType().GetClassName()}");
                }
                else
                {
                    Output.WriteLine($"{property.Name}: !!! No Action Found !!!");
                }
            }

            Assert.Equal(count, valid);
            Output.WriteLine($"{recordType.Name}: {valid}/{count}");
        }
    }
}