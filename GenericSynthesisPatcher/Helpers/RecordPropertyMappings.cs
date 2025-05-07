using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Helpers.Action;

using Noggog;

namespace GenericSynthesisPatcher.Helpers
{
    public abstract class RecordPropertyMappings
    {
        private readonly HashSet<IRecordProperty> propertyAliases = new(comparer: new RecordPropertyComparer());
        private readonly HashSet<IRecordProperty> propertyMappings = new(comparer: new RecordPropertyComparer());

        public IReadOnlyList<PropertyAliasMapping> AllAliases => propertyAliases.Select(a => (PropertyAliasMapping)a).ToList().AsReadOnly();
        public IReadOnlyList<RecordPropertyMapping> AllRPMs => propertyMappings.Select(a => (RecordPropertyMapping)a).ToList().AsReadOnly();

        public IReadOnlyList<string> GetAllAliases (Type type, string propertyName)
        {
            var list = propertyAliases.Where(p => p is PropertyAliasMapping pam && (pam.RealPropertyName?.Equals(propertyName, StringComparison.Ordinal) ?? true) && (pam.Type == null || pam.Type == type)).Select(p => (PropertyAliasMapping)p).ToList();

            foreach (var item in list.ToArray().Where(l => l.Type == null && list.Count(i => i.PropertyName == l.PropertyName) > 1))
                _ = list.Remove(item);

            return list.Where(i => i.RealPropertyName is not null).Select(l => l.PropertyName).ToList().AsReadOnly();
        }

        public IReadOnlyList<string> GetNullAliases (string propertyName)
        {
            var list = propertyAliases.Where(p => p is PropertyAliasMapping pam && (pam.RealPropertyName?.Equals(propertyName, StringComparison.Ordinal) ?? true) && pam.Type == null).Select(p => (PropertyAliasMapping)p).ToList();

            return list.Where(i => i.RealPropertyName is not null).Select(l => l.PropertyName).ToList().AsReadOnly();
        }

        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public bool TryFind (Type? type, string propertyName, out RecordPropertyMapping rpm)
        {
            if (tryFindMapping(type, propertyName, out rpm))
                return true;

            if (tryFindAlias(type, propertyName, out var pam) && pam.RealPropertyName is not null)
                return tryFindMapping(type, pam.RealPropertyName, out rpm);

            return false;
        }

        internal bool tryFindMapping (Type? type, string key, out RecordPropertyMapping rpm)
        {
            if (type != null && propertyMappings.TryGetValue(new RecordProperty(type, key), out var _rpm) && _rpm is RecordPropertyMapping _RPM)
            {
                rpm = _RPM;
                return true;
            }
            else if (propertyMappings.TryGetValue(new RecordProperty(key), out var _rpmNoType) && _rpmNoType is RecordPropertyMapping _RPMNoType)
            {
                rpm = _RPMNoType;
                return true;
            }

            rpm = default;
            return false;
        }

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        protected void Add (Type? type, string propertyName, IRecordAction action) => _ = propertyMappings.Add(new RecordPropertyMapping(type, propertyName, action));

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        protected void AddAlias (Type? type, string propertyName, string? realPropertyName)
        {
            if (type is not null)
            {
                foreach (var prop in propertyAliases.Where(p => p.Type is null && p is PropertyAliasMapping pam && (pam.RealPropertyName?.Equals(realPropertyName, StringComparison.Ordinal) ?? false)).ToArray())
                {
                    if (propertyName.Equals(prop.PropertyName, StringComparison.Ordinal))
                        throw new Exception($"Invalid alias that matches null entry. Type: {type.GetClassName()}. Alias: {propertyName}");

                    _ = propertyAliases.Add(new PropertyAliasMapping(type, prop.PropertyName, null));
                }
            }

            _ = propertyAliases.Add(new PropertyAliasMapping(type, propertyName, realPropertyName));
        }

        private bool tryFindAlias (Type? type, string key, out PropertyAliasMapping pam)
        {
            if (type != null && propertyAliases.TryGetValue(new RecordProperty(type, key), out var _rpm) && _rpm is PropertyAliasMapping _PAM)
            {
                pam = _PAM;
                return true;
            }
            else if (propertyAliases.TryGetValue(new RecordProperty(key), out var _rpmNoType) && _rpmNoType is PropertyAliasMapping _PAMNoType)
            {
                pam = _PAMNoType;
                return true;
            }

            pam = default;
            return false;
        }
    }

    #region Support Classes

    public readonly struct RecordProperty (Type? type, string propertyName) : IRecordProperty
    {
        public RecordProperty (string propertyName) : this(null, propertyName)
        {
        }

        public string PropertyName { get; } = propertyName;

        public Type? Type { get; } = type;
    }

    public sealed class RecordPropertyComparer : IEqualityComparer<IRecordProperty>
    {
        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals (IRecordProperty? x, IRecordProperty? y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            return x.Type == y.Type && x.PropertyName.Equals(y.PropertyName, StringComparison.OrdinalIgnoreCase);
        }

        public static int GetHashCode ([DisallowNull] IRecordProperty obj) => (obj.Type?.GetHashCode() ?? 0) ^ obj.PropertyName.GetHashCode(StringComparison.OrdinalIgnoreCase);

        bool IEqualityComparer<IRecordProperty>.Equals (IRecordProperty? x, IRecordProperty? y) => Equals(x, y);

        int IEqualityComparer<IRecordProperty>.GetHashCode ([DisallowNull] IRecordProperty obj) => GetHashCode(obj);
    }

    #endregion Support Classes
}