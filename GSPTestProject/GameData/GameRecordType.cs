using System.Reflection;

using GenericSynthesisPatcher.Helpers;

using Loqui;

using Mutagen.Bethesda;

namespace GSPTestProject.GameData
{
    public sealed class GameRecordType (GameRelease gameRelease, ILoquiRegistration recordType, IEnumerable<ILoquiRegistration> subTypes) : Game(gameRelease)
    {
        public ILoquiRegistration RecordType { get; } = recordType;

        public IEnumerable<ILoquiRegistration> SubTypes { get; } = subTypes;

        public static List<PropertyInfo> GetProperties (Type type)
        {
            var list = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Where(p => p.IsValidPropertyType()).ToList();
            list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            return list;
        }

        public List<PropertyInfo> GetProperties () => GetProperties(RecordType.ClassType);
    }
}