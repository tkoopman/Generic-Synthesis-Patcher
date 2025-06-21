using GenericSynthesisPatcher.Games.Universal;

using Mutagen.Bethesda;

namespace GSPTestProject.GameData
{
    public sealed class Game
    {
        public Game (GameRelease gameRelease)
        {
            GameRelease = gameRelease;
            switch (gameRelease)
            {
                case GameRelease.SkyrimLE:
                case GameRelease.SkyrimSE:
                    GameName = "Skyrim";
                    BaseGame = GenericSynthesisPatcher.Games.Skyrim.SkyrimGame.Constructor(null!);
                    break;

                case GameRelease.Fallout4:
                    GameName = "Fallout4";
                    BaseGame = GenericSynthesisPatcher.Games.Fallout4.Fallout4Game.Constructor(null!);
                    break;

                case GameRelease.Oblivion:
                case GameRelease.OblivionRE:
                    GameName = "Oblivion";
                    BaseGame = GenericSynthesisPatcher.Games.Oblivion.OblivionGame.Constructor(null!);
                    break;

                default:
                    throw new NotSupportedException($"Game release {gameRelease} is not supported.");
            }
        }

        public BaseGame BaseGame { get; }

        public string GameName { get; }
        public GameRelease GameRelease { get; }
    }
}