using System.Collections;

using GSPTestProject.GameData.GlobalGame.Fixtures;

using Global = GenericSynthesisPatcher.Global;

namespace GSPTestProject.GameData.GlobalGame
{
    /// <summary>
    ///     Returns all record types for <see cref="Global.Game" />.
    ///
    ///     Requires <see cref="BaseFixture{TModSetter, TModGetter}" /> to be used.
    ///
    ///     If <see cref="Global.Game" /> is not required use
    ///     <see cref="All.AllGames_RecordTypes" /> instead.
    /// </summary>
    public class RecordTypes : IEnumerable<object?[]>
    {
        public IEnumerator<object?[]> GetEnumerator ()
        {
            foreach (var recordType in Global.Game.AllRecordTypes())
                yield return [new GameRecordType(Global.Game.State.GameRelease, recordType)];
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}