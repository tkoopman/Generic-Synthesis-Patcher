using GenericSynthesisPatcher.Games.Universal;

using Mutagen.Bethesda;

namespace GSPTestProject.GameData
{
    public class Game
    {
        public Game (GameRelease gameRelease)
        {
            // Initialize the base game based on the provided game release.
            BaseGame = gameRelease switch
            {
                GameRelease.SkyrimLE or GameRelease.SkyrimSE => new GenericSynthesisPatcher.Games.Skyrim.SkyrimGame(null!),
                GameRelease.Fallout4 => new GenericSynthesisPatcher.Games.Fallout4.Fallout4Game(null!),
                GameRelease.Oblivion or GameRelease.OblivionRE => new GenericSynthesisPatcher.Games.Oblivion.OblivionGame(null!),
                _ => throw new NotSupportedException($"Game release {gameRelease} is not supported."),
            };
        }

        public BaseGame BaseGame { get; }

        public string GameName => BaseGame.GameCategory.ToString();
    }
}