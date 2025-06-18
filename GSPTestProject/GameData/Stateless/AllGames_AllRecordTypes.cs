using System.Collections;

namespace GSPTestProject.GameData.Stateless
{
    public class AllGames_AllRecordTypes : IEnumerable<object[]>
    {
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
                foreach (var recordType in gameData.RecordTypes)
                    yield return new object[] { new GameRecordType(gameData.GameRelease, recordType) };
            }
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}