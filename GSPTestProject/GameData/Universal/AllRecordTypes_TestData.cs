using System.Collections;

using Global = GenericSynthesisPatcher.Global;

namespace GSPTestProject.GameData.Universal
{
    internal class AllRecordTypes_TestData : IEnumerable<object?[]>
    {
        public IEnumerator<object?[]> GetEnumerator ()
        {
            foreach (var recordType in Global.Game.AllRecordTypes())
                yield return [recordType];
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}