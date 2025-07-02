using System.Reflection;

using Common;

using GenericSynthesisPatcher;
using GenericSynthesisPatcher.Helpers;

using GSPTestProject.GameData;
using GSPTestProject.Helpers;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records.Mapping;

using Noggog;

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

        protected abstract Type ModGetterType { get; }

        [Theory]
        [ClassData(typeof(GameData.GlobalGame.RecordTypes))]
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

        /// <summary>
        ///     I do know I could get all record types using MajorRecordTypeEnumerator but in my
        ///     testing faster doing the way BaseGame.RecordTypes does it, as need to check certain
        ///     things anyway.
        ///
        ///     Added this test however just to make sure nothing changes.
        /// </summary>
        [Fact]
        public void ConfirmDetectedRecordTypesComparedToMajorRecordTypeEnumerator ()
        {
            var recordTypes = new Dictionary<string, ILoquiRegistration>(StringComparer.OrdinalIgnoreCase);

            foreach (var recordType in MajorRecordTypeEnumerator.GetMajorRecordTypesFor(Global.Game.State.GameRelease.ToCategory()))
            {
                if (!SynthCommon.TryGetStaticRegistration(recordType.GetterType, out var registration))
                    continue;

                if (!registration.IsValidRecordType(out var type))
                    continue;

                if (Global.Game.TypeOptionSolidifierMixIns.GetMethod(registration.Name, BindingFlags.Public | BindingFlags.Static, [ModGetterType]) is null)
                {
                    Output.WriteLine($"No TypeOptionSolidifierMixIns for {registration.Name} - {registration.ClassType.Name} - {registration.GetterType.Name} - {registration.SetterType.Name}");
                    continue;
                }

                _ = recordTypes.TryAdd(type.Type, registration);
                _ = recordTypes.TryAdd(registration.Name, registration);
                _ = recordTypes.TryAdd(registration.Name.SeparateWords(), registration);
            }

            var list = new HashSet<ILoquiRegistration>(recordTypes.Values);

            var missing = Global.Game.AllRecordTypes().WhereNotIn(list);
            var extra = list.WhereNotIn(Global.Game.AllRecordTypes());

            foreach (var item in list)
            {
                string prefix = missing.Contains(item) ? "--- " : string.Empty;
                prefix += extra.Contains(item) ? "+++ " : string.Empty;

                _ = Global.Game.GetRecords(item); // Just run to make sure it doesn't throw exception. Can't actually get records in this test.

                Output.WriteLine($"{prefix}{item.ClassType.Name} - {item.GetterType.Name} - {item.SetterType.Name} - {item.IsValidRecordType()}");
            }

            foreach (var item in missing)
            {
                Output.WriteLine($"--- {item.ClassType.Name} - {item.GetterType.Name} - {item.SetterType.Name}");
            }

            Assert.Empty(missing);
            Assert.Empty(extra);
        }
    }
}