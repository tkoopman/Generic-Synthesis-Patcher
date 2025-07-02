using GenericSynthesisPatcher.Games.Universal;

using Mutagen.Bethesda;

namespace GSPTestProject.GameData
{
    public class Game
    {
        public Game (GameRelease gameRelease)
        {
            GameRelease = gameRelease;
            GameCategory = gameRelease.ToCategory();
            GameName = GameCategory.ToString();

            BaseGame = gameRelease switch
            {
                GameRelease.SkyrimLE or GameRelease.SkyrimSE => GenericSynthesisPatcher.Games.Skyrim.SkyrimGame.Constructor(null!),
                GameRelease.Fallout4 => GenericSynthesisPatcher.Games.Fallout4.Fallout4Game.Constructor(null!),
                GameRelease.Oblivion or GameRelease.OblivionRE => GenericSynthesisPatcher.Games.Oblivion.OblivionGame.Constructor(null!),
                _ => throw new NotSupportedException($"Game release {gameRelease} is not supported."),
            };
        }

        public BaseGame BaseGame { get; }

        public GameCategory GameCategory { get; }
        public string GameName { get; }
        public GameRelease GameRelease { get; }
    }
}