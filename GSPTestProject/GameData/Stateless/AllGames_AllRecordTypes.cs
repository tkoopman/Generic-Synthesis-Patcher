using System.Collections;
using System.Collections.Immutable;

using Common;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records.Mapping;

namespace GSPTestProject.GameData.Stateless
{
    public class AllGames_AllRecordTypes : IEnumerable<object[]>
    {
        public static ImmutableArray<ILoquiRegistration> GetSubTypes (GameCategory gameCategory, ILoquiRegistration recordType)
        {
            List<ILoquiRegistration> subTypes = [];
            foreach (var type in MajorRecordTypeEnumerator.GetMajorRecordTypesFor(gameCategory))
            {
                if (type.ClassType != recordType.ClassType
                    && SynthCommon.TryGetStaticRegistration(type.GetterType, out var registration)
                    && registration.ClassType.IsAssignableTo(recordType.ClassType))
                {
                    subTypes.Add(registration);
                }
            }

            return [.. subTypes];
        }

        /// <summary>
        ///     Provides all record types across all games for testing purposes.
        ///
        ///     <see cref="GenericSynthesisPatcher.Global.Game" /> will not be set in any test using
        ///     classes in this namespace.
        /// </summary>
        public IEnumerator<object[]> GetEnumerator ()
        {
            foreach (object[] obj in new AllGames())
            {
                var gameData = (Game)obj[0];
                foreach (var recordType in gameData.BaseGame.AllRecordTypes())
                    yield return new object[] { new GameRecordType(gameData.BaseGame.GameRelease, recordType, GetSubTypes(gameData.BaseGame.GameCategory, recordType)) };
            }
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}