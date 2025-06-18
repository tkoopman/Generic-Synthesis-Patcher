using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Helpers;

using Loqui;

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
                    RecordTypes = getRecordTypes(typeof(Mutagen.Bethesda.Skyrim.TypeOptionSolidifierMixIns));
                    break;

                case GameRelease.Fallout4:
                    GameName = "Fallout4";
                    BaseGame = GenericSynthesisPatcher.Games.Fallout4.Fallout4Game.Constructor(null!);
                    RecordTypes = getRecordTypes(typeof(Mutagen.Bethesda.Fallout4.TypeOptionSolidifierMixIns));
                    break;

                case GameRelease.Oblivion:
                case GameRelease.OblivionRE:
                    GameName = "Oblivion";
                    BaseGame = GenericSynthesisPatcher.Games.Oblivion.OblivionGame.Constructor(null!);
                    RecordTypes = getRecordTypes(typeof(Mutagen.Bethesda.Oblivion.TypeOptionSolidifierMixIns));
                    break;

                default:
                    throw new NotSupportedException($"Game release {gameRelease} is not supported.");
            }
        }

        public BaseGame BaseGame { get; }

        public string GameName { get; }
        public GameRelease GameRelease { get; }
        public IReadOnlyCollection<ILoquiRegistration> RecordTypes { get; }

        private static IReadOnlyCollection<ILoquiRegistration> getRecordTypes (Type typeOptionSolidifierMixIns)
        {
            HashSet<ILoquiRegistration> types = [];

            foreach (var method in typeOptionSolidifierMixIns.GetMethods())
            {
                if (!method.ReturnType.IsGenericType || method.ReturnType.GenericTypeArguments.Length == 0)
                    continue;

                var returnType = method.ReturnType.GenericTypeArguments[^1];

                var regoProperty = returnType.GetProperty("StaticRegistration");
                if (regoProperty?.GetValue(null) is ILoquiRegistration rego && rego.IsValidRecordType())
                    _ = types.Add(rego);
            }

            var list = types.ToList();
            list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            return list.AsReadOnly();
        }
    }
}