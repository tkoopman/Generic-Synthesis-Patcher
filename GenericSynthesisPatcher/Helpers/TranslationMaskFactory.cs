using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Common;

using Loqui;

using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers
{
    public static class TranslationMaskFactory
    {
        /// <inheritdoc cref="TryCreate(Type, bool, bool, IEnumerable{string}, out ITranslationMask?, StringComparison)" />
        public static bool TryCreate (ILoquiRegistration recordType, bool defaultOn, IEnumerable<string> toggleEntries, [NotNullWhen(true)] out ITranslationMask? mask, StringComparison comparer = StringComparison.Ordinal)
        {
            mask = null;
            return TryGetTranslationMaskType(recordType, out var type) && tryCreateInternal(type, defaultOn, null, toggleEntries, comparer, out mask);
        }

        /// <inheritdoc cref="TryCreate(Type, bool, bool, IEnumerable{string}, out ITranslationMask?, StringComparison)" />
        public static bool TryCreate (Type type, bool defaultOn, IEnumerable<string> toggleEntries, [NotNullWhen(true)] out ITranslationMask? mask, StringComparison comparer = StringComparison.Ordinal)
                    => tryCreateInternal(type, defaultOn, null, toggleEntries, comparer, out mask);

        /// <summary>
        ///     Creates a TranslationMask of specified type and sets entries as specified.
        /// </summary>
        /// <param name="type">TranslationMask class type to create</param>
        /// <param name="defaultOn">Passed to the TranslationMask constructor.</param>
        /// <param name="onOverall">Passed to the TranslationMask constructor.</param>
        /// <param name="toggleEntries">
        ///     List of mask entries that will be set to non-defaultOn value.
        /// </param>
        /// <param name="mask">
        ///     Created translation mask. Can still be set if returned result is false, but if it
        ///     does exist then means not all toggleEntries were valid.
        /// </param>
        /// <param name="comparer">
        ///     Set to comparison used to match toggleEntries field names. Default =
        ///     StringComparison.Ordinal, so case-sensitive by default.
        /// </param>
        /// <returns>
        ///     True if mask was able to be created, and all toggleEntries were able to be set, else
        ///     false.
        /// </returns>
        public static bool TryCreate (Type type, bool defaultOn, bool onOverall, IEnumerable<string> toggleEntries, [NotNullWhen(true)] out ITranslationMask? mask, StringComparison comparer = StringComparison.Ordinal)
            => tryCreateInternal(type, defaultOn, onOverall, toggleEntries, comparer, out mask);

        /// <inheritdoc cref="TryCreate(Type, bool, bool, IEnumerable{string}, out ITranslationMask?, StringComparison)" />
        public static bool TryCreate (Type type, bool defaultOn, bool onOverall, [NotNullWhen(true)] out ITranslationMask? mask)
            => tryCreateInternal(type, defaultOn, onOverall, [], StringComparison.Ordinal, out mask);

        /// <inheritdoc cref="TryCreate(Type, bool, bool, out ITranslationMask?)" />
        public static bool TryCreate (Type type, bool defaultOn, [NotNullWhen(true)] out ITranslationMask? mask)
            => tryCreateInternal(type, defaultOn, null, [], StringComparison.Ordinal, out mask);

        /// <inheritdoc cref="TryCreate(Type, bool, IEnumerable{string}, out ITranslationMask?, StringComparison)" />
        /// <typeparam name="T">TranslationMask class type to create</typeparam>
        public static bool TryCreate<T> (bool defaultOn, IEnumerable<string> toggleEntries, [NotNullWhen(true)] out T? mask, StringComparison comparer = StringComparison.Ordinal)
            where T : class, ITranslationMask
        {
            _ = tryCreateInternal(typeof(T), defaultOn, null, toggleEntries, comparer, out var m);
            mask = m as T;
            return mask is not null;
        }

        /// <inheritdoc cref="TryCreate(Type, bool, bool, IEnumerable{string}, out ITranslationMask?, StringComparison)" />
        /// <typeparam name="T">TranslationMask class type to create</typeparam>
        public static bool TryCreate<T> (bool defaultOn, bool onOverall, IEnumerable<string> toggleEntries, [NotNullWhen(true)] out T? mask, StringComparison comparer = StringComparison.Ordinal)
            where T : class, ITranslationMask
        {
            _ = tryCreateInternal(typeof(T), defaultOn, onOverall, toggleEntries, comparer, out var m);
            mask = m as T;
            return mask is not null;
        }

        /// <inheritdoc cref="TryCreate(Type, bool, bool, out ITranslationMask?)" />
        /// <typeparam name="T">TranslationMask class type to create</typeparam>
        public static bool TryCreate<T> (bool defaultOn, bool onOverall, [NotNullWhen(true)] out T? mask)
            where T : class, ITranslationMask
        {
            _ = tryCreateInternal(typeof(T), defaultOn, onOverall, [], StringComparison.Ordinal, out var m);
            mask = m as T;
            return mask is not null;
        }

        /// <inheritdoc cref="TryCreate(Type, bool, out ITranslationMask?)" />
        /// <typeparam name="T">TranslationMask class type to create</typeparam>
        public static bool TryCreate<T> (bool defaultOn, [NotNullWhen(true)] out T? mask)
            where T : class, ITranslationMask
        {
            _ = tryCreateInternal(typeof(T), defaultOn, null, [], StringComparison.Ordinal, out var m);
            mask = m as T;
            return mask is not null;
        }

        /// <summary>
        ///     Returns the TranslationMask type for the given registration.
        /// </summary>
        /// <param name="registration">ILoquiRegistration to get translation mask for.</param>
        /// <returns>ITranslationMask type if found, else null.</returns>
        public static bool TryGetTranslationMaskType (this ILoquiRegistration registration, [NotNullWhen(true)] out Type? translationMaskType)
        {
            translationMaskType = registration.ClassType.GetNestedType("TranslationMask");
            return translationMaskType is not null && translationMaskType.IsAssignableTo(typeof(ITranslationMask));
        }

        /// <summary>
        ///     Tries to set a property on the translation mask. If the property is of type
        ///     ITranslationMask, it will attempt to create a new instance of that mask type with
        ///     the DefaultOn set to value.
        /// </summary>
        /// <param name="mask">TranslationMask to set property on</param>
        /// <param name="maskEntry">Name of the mask entry to set</param>
        /// <param name="value">Value to set</param>
        /// <param name="comparer">
        ///     StringComparison to use to match propertyName. Exact match always done first.
        /// </param>
        /// <returns>True if property exists and was able to be set.</returns>
        public static bool TrySetValue (this ITranslationMask mask, string maskEntry, bool value, StringComparison comparer = StringComparison.Ordinal)
        {
            if (mask is null || string.IsNullOrWhiteSpace(maskEntry))
                return false;

            var field = mask.GetType().GetField(maskEntry, BindingFlags.Public | BindingFlags.Instance);
            if (field is null)
            {
                if (comparer == StringComparison.OrdinalIgnoreCase)
                {
                    // Try case-insensitive search
                    field = mask.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(f => string.Equals(f.Name, maskEntry, comparer));
                }

                if (field is null)
                    return false;
            }

            if (field.IsInitOnly)
                return false; // Can set readonly field

            if (field.FieldType == typeof(bool))
            {
                field.SetValue(mask, value);
                return true;
            }
            else if (field.FieldType.IsAssignableTo(typeof(ITranslationMask)))
            {
                // As we converting a bool to TranslationMask, we follow the implicit operator and
                // set both defaultOn and onOverall to value.
                if (tryCreateInternal(field.FieldType, value, value, [], StringComparison.Ordinal, out var subMask))
                {
                    field.SetValue(mask, subMask);
                    return true;
                }
            }
            else
            {
                var properties = field.FieldType.Explode(2);
                if (properties.Length == 2 && (properties[0] == typeof(IGenderedItem<>) || properties[0] == typeof(GenderedItem<>)))
                {
                    // TODO: Currently only support setting both genders to same value
                    if (properties[1].IsAssignableTo(typeof(bool)))
                    {
                        field.SetValue(mask, new GenderedItem<bool>(value, value));
                        return true;
                    }

                    if (properties[1].IsAssignableTo(typeof(ITranslationMask)) && tryCreateInternal(properties[1], value, value, [], StringComparison.Ordinal, out var subMask))
                    {
                        var con = field.FieldType.GetConstructor([properties[1], properties[1]]);
                        object? genderObj = con?.Invoke([subMask, subMask]);
                        if (genderObj is null)
                            return false; // Constructor returned null

                        field.SetValue(mask, genderObj);
                        return true;
                    }
                }
            }

            return false; // Unsupported field type
        }

        /// <summary>
        ///     Primary Method. Returns the TranslationMask type for the given type. Type must
        ///     implement using Loqui, and have a StaticRegistration property.
        /// </summary>
        /// <param name="onOverall">
        ///     Passed to the TranslationMask constructor. If null will use the constructor's
        ///     default value.
        /// </param>
        /// <inheritdoc cref="TryCreate(Type, bool, bool, IEnumerable{string}, out ITranslationMask?, StringComparison)" />
        internal static bool tryCreateInternal (Type type, bool defaultOn, bool? onOverall, IEnumerable<string> toggleEntries, StringComparison comparer, [NotNullWhen(true)] out ITranslationMask? mask)
        {
            var con = type.GetConstructor([typeof(bool), typeof(bool)]);
            if (con is null)
            {
                mask = null;
                return false; // No suitable constructor found
            }

            // If onOverall is null, use the constructor's default value for it
            onOverall ??= (bool)(con.GetParameters()[1].DefaultValue ?? true);

            mask = (ITranslationMask)con.Invoke([defaultOn, onOverall]);
            if (mask is null)
                return false;  // Constructor returned null

            bool setAll = true;
            foreach (string param in toggleEntries)
                setAll = mask.TrySetValue(param, !defaultOn, comparer) && setAll;

            return setAll;
        }
    }
}