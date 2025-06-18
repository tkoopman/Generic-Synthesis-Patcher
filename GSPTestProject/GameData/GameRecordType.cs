using System.Reflection;

using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Helpers;

using Loqui;

using Mutagen.Bethesda;

namespace GSPTestProject.GameData
{
    public sealed class GameRecordType
    {
        public GameRecordType (GameRelease gameRelease, ILoquiRegistration recordType)
        {
            GameRelease = gameRelease;
            RecordType = recordType;
            switch (gameRelease)
            {
                case GameRelease.SkyrimLE:
                case GameRelease.SkyrimSE:
                    GameName = "Skyrim";
                    BaseGame = GenericSynthesisPatcher.Games.Skyrim.SkyrimGame.Constructor(null!);
                    Properties = getProperties(recordType);
                    break;

                case GameRelease.Fallout4:
                    GameName = "Fallout4";
                    BaseGame = GenericSynthesisPatcher.Games.Fallout4.Fallout4Game.Constructor(null!);
                    Properties = getProperties(recordType);
                    break;

                case GameRelease.Oblivion:
                case GameRelease.OblivionRE:
                    GameName = "Oblivion";
                    BaseGame = GenericSynthesisPatcher.Games.Oblivion.OblivionGame.Constructor(null!);
                    Properties = getProperties(recordType);
                    break;

                default:
                    throw new NotSupportedException($"Game release {gameRelease} is not supported.");
            }
        }

        public BaseGame BaseGame { get; }

        public string GameName { get; }
        public GameRelease GameRelease { get; }
        public IReadOnlyCollection<PropertyInfo> Properties { get; }
        public ILoquiRegistration RecordType { get; }

        private static IReadOnlyCollection<PropertyInfo> getProperties (ILoquiRegistration recordType)
        {
            HashSet<PropertyInfo> properties = [];

            foreach (var property in recordType.ClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Where(p => p.IsValidPropertyType()))
                _ = properties.Add(property);

            var list = properties.ToList();
            list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            return list.AsReadOnly();
        }
    }
}