namespace GenericSynthesisPatcher.Helpers
{
    /// <summary>
    /// Sort key for sorted list of record call mappings.
    /// </summary>
    /// <param name="jsonKey">The field value found in the JSON configuration files. Case insensitive.</param>
    /// <param name="recordType">Set record getter type if this should only be mapped for a single record type. Can be null in which case will call for any matching records.</param>
    public class RecordCallKey ( string jsonKey, Type? recordType ) : IEquatable<RecordCallKey>, IComparable<RecordCallKey>
    {
        public string JsonKey { get; } = jsonKey;
        public Type? RecordType { get; } = recordType;

        /// <summary>
        /// So sorted list can sort itself the way it needs to be for this program to work.
        /// </summary>
        public int CompareTo ( RecordCallKey? other )
        {
            if (other == null)
                return -1;

            int c = string.Compare(JsonKey, other.JsonKey, StringComparison.OrdinalIgnoreCase);

            return c switch
            {
                0 when RecordType == null && other.RecordType == null => 0,
                0 when RecordType == null || other.RecordType == null => other.RecordType == null ? -1 : 1,
                0 => RecordType.FullName?.CompareTo(other.RecordType.FullName) ?? 0,
                _ => c
            };
        }

        public override bool Equals ( object? obj ) => obj is RecordCallKey key && Equals(key);

        public bool Equals ( RecordCallKey? other ) => other != null && JsonKey.Equals(other.JsonKey, StringComparison.OrdinalIgnoreCase) && RecordType == other.RecordType;

        public override int GetHashCode () => HashCode.Combine(JsonKey.GetHashCode(StringComparison.OrdinalIgnoreCase), RecordType);
    }
}