using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSPTestProject.GameData.Stateless
{
    /// <summary>
    ///     Provides all games for testing purposes.
    ///
    ///     <see cref="GenericSynthesisPatcher.Global.Game" /> will not be set in any test using
    ///     classes in this namespace.
    /// </summary>
    public sealed class AllGames : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator ()
        {
            yield return new object[] { new Game(Mutagen.Bethesda.GameRelease.Fallout4) };
            yield return new object[] { new Game(Mutagen.Bethesda.GameRelease.OblivionRE) };
            yield return new object[] { new Game(Mutagen.Bethesda.GameRelease.SkyrimSE) };
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}