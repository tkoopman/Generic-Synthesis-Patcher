using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Common;

using Loqui;

using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Helpers
{
    /// <summary>
    ///     Methods for working with <see cref="Loqui.ITranslationMask" /> classes used by Mutagen.
    ///
    ///     NOTE: Probably won't work with non-Mutagen ITranslationMask classes, unless they follow
    ///     the same implementation of DefaultOn and OnOverall which isn't part of the
    ///     ITranslationMask interface.
    /// </summary>
    public static class TranslationMaskFactory
    {
        /// <summary>
        ///     Get all valid fields from mask. Excludes non-writable (DefaultOn) and OnOverall.
        /// </summary>
        public static IEnumerable<FieldInfo> GetAllFields (this ITranslationMask mask)
            => mask.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => IsValidMaskField(f) && !f.Name.Equals(nameof(MajorRecord.TranslationMask.OnOverall), StringComparison.Ordinal));

        /// <summary>
        ///     Gets the DefaultOn value of a mask
        /// </summary>
        /// <exception cref="ArgumentException">DefaultOn doesn't exist</exception>
        public static bool GetDefaultOn (this ITranslationMask mask)
        {
            var field = mask.GetType().GetField(nameof(MajorRecord.TranslationMask.DefaultOn), BindingFlags.Public | BindingFlags.Instance);
            return field is not null && field.GetValue(mask) is bool value
                ? value
                : throw new ArgumentException("Mask doesn't contain DefaultOn field");
        }

        /// <summary>
        ///     Return list of field names that are enabled on a mask
        /// </summary>
        public static List<string> GetEnabled (this ITranslationMask mask)
        {
            bool defaultOn = mask.GetDefaultOn();

            List<string> enabled = [];

            var fields = mask.GetAllFields().ToList();
            fields.Sort((lhs, rhs) => string.Compare(lhs?.Name, rhs?.Name, StringComparison.OrdinalIgnoreCase));

            foreach (var field in fields)
            {
                object? value = field.GetValue(mask);
                if (isEnabled(value, defaultOn))
                    enabled.Add(field.Name);
            }

            return enabled;
        }

        /// <summary>
        ///     Gets all fields and values that are not set to DefaultOn.
        /// </summary>
        public static IEnumerable<(string name, object value)> GetNonDefault (this ITranslationMask mask)
        {
            bool defaultOn = mask.GetDefaultOn();
            List<(string name, object value)> fields = [];

            foreach (var field in mask.GetAllFields())
            {
                object? value = field.GetValue(mask);
                switch (value)
                {
                    case null:
                        break;

                    case bool boolValue:
                        if (boolValue != defaultOn)
                            fields.Add((field.Name, boolValue));
                        break;

                    default:
                        fields.Add((field.Name, value));
                        break;
                }
            }

            return fields;
        }

        public static bool GetOnOverall (this ITranslationMask mask)
        {
            var field = mask.GetType().GetField(nameof(MajorRecord.TranslationMask.OnOverall), BindingFlags.Public | BindingFlags.Instance);
            return field is not null && field.GetValue(mask) is bool value
                ? value
                : throw new ArgumentException("Mask doesn't contain OnOverall field");
        }

        public static bool IsValidMaskField (FieldInfo? field) => field is not null && !field.IsInitOnly && IsValidMaskField(field.FieldType);

        public static bool IsValidMaskField (Type FieldType, bool allowGendered = true)
        {
            if (FieldType == typeof(bool) || FieldType.IsAssignableTo(typeof(ITranslationMask)))
                return true;

            if (allowGendered)
            {
                var properties = FieldType.Explode(2);
                if (properties.Length == 2 && (properties[0] == typeof(IGenderedItem<>) || properties[0] == typeof(GenderedItem<>)))
                {
                    return IsValidMaskField(properties[1], false);
                }
            }

            // Must be false
            return false;
        }

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
        ///     True if mask was able to be created, and all toggleEntries were able to be set, else false.
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
        ///     Attempts to get a field from the translation mask by its name.
        ///
        ///     In order will attempt to find by:
        ///     - Exact match
        ///     - Match using string comparison (e.g. OrdinalIgnoreCase)
        ///     - Checks if it is a known alias
        /// </summary>
        /// <param name="mask">Mask to get field from.</param>
        /// <param name="maskEntry">Name of field to get.</param>
        /// <param name="comparer">Comparer to try if exact match not found.</param>
        /// <param name="field">FieldInfo output if found.</param>
        /// <returns>True if field info found.</returns>
        public static bool TryGetMaskField (this ITranslationMask mask, string maskEntry, StringComparison comparer, [NotNullWhen(true)] out FieldInfo? field)
        {
            field = null;
            if (mask is null || string.IsNullOrWhiteSpace(maskEntry))
                return false;

            field = mask.GetType().GetField(maskEntry, BindingFlags.Public | BindingFlags.Instance);
            if (field is null)
            {
                if (comparer == StringComparison.OrdinalIgnoreCase)
                {
                    // Try case-insensitive search
                    field = mask.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(f => string.Equals(f.Name, maskEntry, comparer));
                }

                if (field is null)
                {
                    if (Global.Game is not null && TryGetParentClass(mask.GetType(), out var rego))
                    {
                        // Try to get the real property name from the game context
                        string? realName = Global.Game.GetRealPropertyName(rego, maskEntry);
                        if (realName is not null)
                            field = mask.GetType().GetField(realName, BindingFlags.Public | BindingFlags.Instance);
                    }
                }
            }

            return IsValidMaskField(field);
        }

        /// <summary>
        ///     Returns the ILoquiRegistration for the given ITranslationMask type.
        /// </summary>
        /// <returns>True if parent found.</returns>
        public static bool TryGetParentClass (Type translationMaskType, [NotNullWhen(true)] out ILoquiRegistration? registration)
        {
            registration = null;
            var parent = translationMaskType.DeclaringType;
            if (!translationMaskType.IsClass || parent is null || !translationMaskType.IsAssignableTo(typeof(ITranslationMask)))
                return false;

            registration = SynthCommon.GetStaticRegistration(parent);

            return registration is not null;
        }

        /// <summary>
        ///     Returns the TranslationMask type for the given registration.
        /// </summary>
        /// <param name="registration">ILoquiRegistration to get translation mask for.</param>
        /// <returns>true if mask was found.</returns>
        public static bool TryGetTranslationMaskType (this ILoquiRegistration registration, [NotNullWhen(true)] out Type? translationMaskType)
        {
            translationMaskType = registration.ClassType.GetNestedType("TranslationMask");
            return translationMaskType is not null && translationMaskType.IsAssignableTo(typeof(ITranslationMask));
        }

        /// <summary>
        ///     Tries to set a field on the translation mask.
        ///
        ///     If the field is of type ITranslationMask, it will attempt to create a new instance
        ///     of that mask type with the DefaultOn and OnOverall values set to value.
        ///
        ///     If the field is of type GenderedItem, it will create a new instance with both male
        ///     and female values set to value.
        /// </summary>
        /// <param name="mask">TranslationMask to set field on</param>
        /// <param name="maskEntry">Name of the mask entry to set</param>
        /// <param name="value">Value to set</param>
        /// <param name="comparer">
        ///     StringComparison to use to match field name. Exact match always done first.
        /// </param>
        /// <returns>True if field exists and was able to be set.</returns>
        public static bool TrySetValue (this ITranslationMask mask, string maskEntry, bool value, StringComparison comparer = StringComparison.Ordinal)
        {
            if (!TryGetMaskField(mask, maskEntry, comparer, out var field))
                return false;

            if (field.FieldType == typeof(bool))
            {
                field.SetValue(mask, value);
                return true;
            }

            if (field.FieldType.IsAssignableTo(typeof(ITranslationMask)))
            {
                // As we converting a bool to TranslationMask, we follow the implicit operator and
                // set both defaultOn and onOverall to value.
                if (!tryCreateInternal(field.FieldType, value, value, [], StringComparison.Ordinal, out var subMask))
                    return false;

                field.SetValue(mask, subMask);
                return true;
            }

            var properties = field.FieldType.Explode(2);
            if (properties.Length == 2 && (properties[0] == typeof(IGenderedItem<>) || properties[0] == typeof(GenderedItem<>)))
            {
                if (properties[1].IsAssignableTo(typeof(bool)))
                {
                    field.SetValue(mask, new GenderedItem<bool>(value, value));
                    return true;
                }

                if (properties[1].IsAssignableTo(typeof(ITranslationMask)))
                {
                    var con = field.FieldType.GetConstructor([properties[1], properties[1]]);
                    if (con is null)
                        return false; // No suitable constructor found

                    if (!tryCreateInternal(properties[1], value, value, [], StringComparison.Ordinal, out var subMask))
                        return false;

                    object? genderObj = con?.Invoke([subMask, subMask]);
                    if (genderObj is null)
                        return false; // Constructor returned null

                    field.SetValue(mask, genderObj);
                    return true;
                }
            }

            return false; // Unsupported field type
        }

        /// <summary>
        ///     Tries to set a field on the translation mask.
        ///
        ///     If mask field value is GenderedItem will create a new instance with both male and
        ///     female values set to value.
        /// </summary>
        /// <param name="mask">TranslationMask to set field on</param>
        /// <param name="maskEntry">Name of the mask entry to set</param>
        /// <param name="value">Value to set</param>
        /// <param name="comparer">
        ///     StringComparison to use to match field name. Exact match always done first.
        /// </param>
        /// <returns>True if field exists and was able to be set.</returns>
        public static bool TrySetValue (this ITranslationMask mask, string maskEntry, ITranslationMask? value, StringComparison comparer = StringComparison.Ordinal)
        {
            if (!TryGetMaskField(mask, maskEntry, comparer, out var field))
                return false;

            if (field.FieldType == typeof(bool))
                return false;

            // Handle null value case
            if (value is null)
            {
                field.SetValue(mask, null);
                return true;
            }

            // Handle exact match case - Valid ITranslationMask value would hit here
            if (value.GetType() == field.FieldType)
            {
                field.SetValue(mask, value);
                return true;
            }

            // Handle gendered mask case
            var properties = field.FieldType.Explode(2);
            if (properties.Length == 2 && (properties[0] == typeof(IGenderedItem<>) || properties[0] == typeof(GenderedItem<>)))
            {
                if (properties[1] == value.GetType())
                {
                    var con = field.FieldType.GetConstructor([properties[1], properties[1]]);
                    if (con is null)
                        return false; // No suitable constructor found

                    object? genderObj = con?.Invoke([value, value]);

                    if (genderObj is null)
                        return false; // Constructor returned null

                    field.SetValue(mask, genderObj);
                    return true;
                }
            }

            return false; // Unsupported field type
        }

        /// <summary>
        ///     Tries to set a field on the translation mask.
        /// </summary>
        /// <param name="mask">TranslationMask to set field on</param>
        /// <param name="maskEntry">Name of the mask entry to set</param>
        /// <param name="value">Value to set</param>
        /// <param name="comparer">
        ///     StringComparison to use to match field name. Exact match always done first.
        /// </param>
        /// <returns>True if field exists and was able to be set.</returns>
        public static bool TrySetValue<T> (this ITranslationMask mask, string maskEntry, IGenderedItem<T>? value, StringComparison comparer = StringComparison.Ordinal)
            where T : notnull, ITranslationMask
        {
            if (!TryGetMaskField(mask, maskEntry, comparer, out var field))
                return false;

            // While TryGetMaskField already confirmed isValidMaskField, use again but don't allow
            // Gendered and return false, so only Gendered continues
            if (IsValidMaskField(field.FieldType, false))
                return false;

            // Handle null value case
            if (value is null)
            {
                field.SetValue(mask, null);
                return true;
            }

            // Handle assignable match case - Valid value type would hit here Using IsAssignableTo
            // to allow for when FieldType is IGenderedItem<T>
            if (value.GetType().IsAssignableTo(field.FieldType))
            {
                field.SetValue(mask, value);
                return true;
            }

            return false; // Unsupported field type
        }

        /// <summary>
        ///     Tries to set a field on the translation mask.
        /// </summary>
        /// <param name="mask">TranslationMask to set field on</param>
        /// <param name="maskEntry">Name of the mask entry to set</param>
        /// <param name="value">Value to set</param>
        /// <param name="comparer">
        ///     StringComparison to use to match field name. Exact match always done first.
        /// </param>
        /// <returns>True if field exists and was able to be set.</returns>
        public static bool TrySetValue (this ITranslationMask mask, string maskEntry, IGenderedItem<bool>? value, StringComparison comparer = StringComparison.Ordinal)
        {
            if (!TryGetMaskField(mask, maskEntry, comparer, out var field))
                return false;

            // While TryGetMaskField already confirmed isValidMaskField, use again but don't allow
            // Gendered and return false, so only Gendered continues
            if (IsValidMaskField(field.FieldType, false))
                return false;

            // Handle null value case
            if (value is null)
            {
                field.SetValue(mask, null);
                return true;
            }

            // Handle assignable match case - Valid value type would hit here Using IsAssignableTo
            // to allow for when FieldType is IGenderedItem<T>
            if (value.GetType().IsAssignableTo(field.FieldType))
            {
                field.SetValue(mask, value);
                return true;
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
            if (con is null || !TryGetParentClass(type, out var registration))
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
            {
                if (!mask.TrySetValue(param, !defaultOn, comparer))
                    setAll = false; // Unable to set value for this entry
            }

            return setAll;
        }

        private static bool isEnabled (object? value, bool defaultOn)
        {
            switch (value)
            {
                case null:
                    return defaultOn;

                case bool boolValue:
                    return boolValue;

                case ITranslationMask mask:
                    return (!mask.GetCrystal()?.CopyNothing) ?? defaultOn;

                default:
                    var properties = value.GetType().Explode(2);
                    return properties.Length == 2 && (properties[0] == typeof(IGenderedItem<>) || properties[0] == typeof(GenderedItem<>)) ? true : throw new ArgumentException($"Invalid mask value type: {value.GetType().GetClassName()}");
            }
        }
    }
}