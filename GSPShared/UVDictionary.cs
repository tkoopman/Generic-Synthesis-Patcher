using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GSPShared
{
    /// <summary>
    ///     A unique value dictionary that allows for reverse lookups.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class UVDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        private readonly Dictionary<TKey, TValue> _byKey;
        private readonly Dictionary<TValue, TKey> _byValue;

        public UVDictionary ()
        {
            _byKey = [];
            _byValue = [];
        }

        public UVDictionary (IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer)
        {
            _byKey = new(keyComparer);
            _byValue = new(valueComparer);
        }

        public int Count => _byKey.Count;
        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        bool ICollection.IsSynchronized => false;
        public ICollection<TKey> Keys => _byKey.Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ((IReadOnlyDictionary<TKey, TValue>)_byKey).Keys;
        ICollection IDictionary.Keys => _byKey.Keys;
        object ICollection.SyncRoot => throw new NotImplementedException();
        public ICollection<TValue> Values => _byValue.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => ((IReadOnlyDictionary<TKey, TValue>)_byKey).Values;
        ICollection IDictionary.Values => _byValue.Values;

        object? IDictionary.this[object key]
        {
            get => (key is TKey _key) ? _byKey[_key] : null;
            set
            {
                if (key is TKey _key && value is TValue _value)
                    UpdateOrAdd(_key, _value);
                else
                    throw new InvalidCastException("Either key and/or value of incorrect type");
            }
        }

        public TValue this[TKey key] { get => _byKey[key]; set => UpdateOrAdd(key, value); }

        public void Add (TKey key, TValue value)
        {
            if (_byKey.TryAdd(key, value))
            {
                if (_byValue.TryAdd(value, key))
                    return;

                _ = _byKey.Remove(key);
            }

            throw new ArgumentException("An item with the same key or value already exists in the dictionary.");
        }

        public void Add (KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        void IDictionary.Add (object key, object? value)
        {
            if (key is TKey _key && value is TValue _value)
                Add(_key, _value);
            else
                throw new InvalidCastException("Either key and/or value of incorrect type");
        }

        public void Clear ()
        {
            _byKey.Clear();
            _byValue.Clear();
        }

        public bool Contains (KeyValuePair<TKey, TValue> item) => _byKey.Contains(item);

        bool IDictionary.Contains (object key) => key is TKey _key && _byKey.ContainsKey(_key);

        public bool ContainsKey (TKey key) => _byKey.ContainsKey(key);

        public bool ContainsValue (TValue value) => _byValue.ContainsKey(value);

        public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();

        public void CopyTo (Array array, int index) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator () => _byKey.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator () => ((IDictionary)_byKey).GetEnumerator();

        public bool Remove (TKey key)
        {
            if (_byKey.TryGetValue(key, out var value))
            {
                _ = _byKey.Remove(key);
                _ = _byValue.Remove(value);
                return true;
            }

            return false;
        }

        public bool Remove (KeyValuePair<TKey, TValue> item)
        {
            if (_byKey.TryGetValue(item.Key, out var value) && ReferenceEquals(value, item.Value))
            {
                _ = _byKey.Remove(item.Key);
                _ = _byValue.Remove(value);
                return true;
            }

            return false;
        }

        void IDictionary.Remove (object key)
        {
            if (key is TKey _key)
                _ = Remove(_key);
        }

        public bool TryAdd (TKey key, TValue value)
        {
            if (_byKey.TryAdd(key, value))
            {
                if (_byValue.TryAdd(value, key))
                    return true;
                _ = _byKey.Remove(key);
            }

            return false;
        }

        public bool TryGetKey (TValue value, [MaybeNullWhen(false)] out TKey key) => _byValue.TryGetValue(value, out key);

        public bool TryGetValue (TKey key, [MaybeNullWhen(false)] out TValue value) => _byKey.TryGetValue(key, out value);

        /// <summary>
        ///     Will update existing key-value pair if key exists, or add a new key-value pair if
        ///     key does not exist. If Value already exists with a different key, this will delete
        ///     that current key-value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryUpdateOrAdd (TKey key, TValue value)
        {
            bool keyFound = _byKey.TryGetValue(key, out var existingKeyValue);
            bool valueFound = _byValue.TryGetValue(value, out var existingValueKey);

            if (keyFound && valueFound && key.Equals(existingValueKey) && value.Equals(existingKeyValue))
                return true;

            if (valueFound && existingValueKey is not null)
                _ = Remove(existingValueKey);

            if (keyFound)
            {
                if (existingKeyValue is not null)
                    _ = _byValue.Remove(existingKeyValue);

                _byKey[key] = value;
                _byValue.Add(value, key);

                return true;
            }
            else
            {
                _byKey.Add(key, value);
                _byValue.Add(value, key);
                return true;
            }
        }

        public void UpdateOrAdd (TKey key, TValue value)
        {
            if (!TryUpdateOrAdd(key, value))
                throw new ArgumentException("An item with the same value already exists in the dictionary.");
        }
    }
}