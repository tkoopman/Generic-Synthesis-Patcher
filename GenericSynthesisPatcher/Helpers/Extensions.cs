using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Common;

using Loqui;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Helpers
{
    public static class Extensions
    {
        /// <summary>
        ///     Deserialize JSON token with my standard stuff, depending on output type and JSON
        ///     token type.
        /// </summary>
        public static T? Deserialize<T> (this JToken token)
            => typeof(T) == typeof(string) && token.Type == JTokenType.String ? (T?)(object)token.ToString()
             : typeof(T).IsAssignableTo(typeof(IEnumerable)) && token.Type != JTokenType.Array ? JsonSerializer.Create(Global.Game.SerializerSettings).Deserialize<T>(new JArray(token).CreateReader())
             : JsonSerializer.Create(Global.Game.SerializerSettings).Deserialize<T>(token.CreateReader());

        /// <summary>
        ///     Gets the value of the TriggeringRecordType field of a registration.
        /// </summary>
        /// <param name="registration">Registration to get value from.</param>
        /// <returns>Record Type</returns>
        /// <exception cref="InvalidOperationException">
        ///     If TriggeringRecordType doesn't exist.
        /// </exception>
        public static RecordType GetRecordType (this ILoquiRegistration registration)
            => registration.TryGetRecordType(out var recordType)
            ? recordType
            : throw new InvalidOperationException($"Registration {registration.GetType().Name} does not have a valid RecordType.");

        /// <summary>
        ///     Gets the singleton instance of a type. Type requires a public static field named "Instance".
        /// </summary>
        /// <param name="type">Type to get singleton of.</param>
        /// <returns>Singleton instance of type.</returns>
        /// <exception cref="InvalidOperationException">
        ///     If type does not contain the required Instance field.
        /// </exception>
        public static object GetSingletonInstance (this Type type) => type.GetField("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null) ??
                    throw new InvalidOperationException($"Type {type.Name} is not a singleton.");

        /// <summary>
        ///     Gets the singleton instance of a type. Type requires a public static field named "Instance".
        /// </summary>
        /// <typeparam name="T">Type singleton must assignable to.</typeparam>
        /// <param name="type">Type to get singleton of.</param>
        /// <returns>Singleton instance of type.</returns>
        /// <exception cref="InvalidOperationException">
        ///     If type does not contain the required Instance field, or if not assignable to type T.
        /// </exception>
        public static T GetSingletonInstance<T> (this Type type) where T : class => type.GetSingletonInstance() as T ??
            throw new InvalidOperationException($"Type {type.Name} is not of type {typeof(T).GetClassName()}.");

        /// <summary>
        ///     Confirms property is valid for setting property via an action.
        /// </summary>
        public static bool IsValidPropertyType (this PropertyInfo propertyInfo) => propertyInfo.CanWrite && propertyInfo.PropertyType != typeof(FormKey);

        /// <inheritdoc cref="IsValidRecordType(ILoquiRegistration, out RecordType, out Type?)" />
        public static bool IsValidRecordType (this ILoquiRegistration registration) => registration.TryGetRecordType(out _) && registration.TryGetTranslationMaskType(out _);

        /// <summary>
        ///     Confirms static registration of a record type is valid. To be valid both below
        ///     methods must return true:
        ///     - <see cref="TryGetRecordType(ILoquiRegistration, out RecordType)" />
        ///     - <see cref="TranslationMaskFactory.TryGetTranslationMaskType(ILoquiRegistration, out Type?)" />
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="type">
        ///     Actual record type. Will still return associated record type even if no translation mask.
        /// </param>
        /// <param name="translationMaskType">
        ///     Actual translation mask. Will still return associated translation mask type, even if
        ///     no record type.
        /// </param>
        /// <returns>
        ///     True if StaticRegistration has both a record type and translation mask associated
        ///     with it.
        /// </returns>
        public static bool IsValidRecordType (this ILoquiRegistration registration, out RecordType type, [NotNullWhen(true)] out Type? translationMaskType)
        {
            bool hasRecordType = registration.TryGetRecordType(out type);
            return registration.TryGetTranslationMaskType(out translationMaskType) && hasRecordType;
        }

        /// <inheritdoc cref="IsValidRecordType(ILoquiRegistration, out RecordType, out Type?)" />
        public static bool IsValidRecordType (this ILoquiRegistration registration, out RecordType type) => registration.TryGetRecordType(out type) && registration.TryGetTranslationMaskType(out _);

        /// <summary>
        ///     Gets the value of the TriggeringRecordType field of a registration.
        /// </summary>
        /// <param name="registration">Registration to get value from.</param>
        /// <returns>Value if field exists else <see cref="RecordType.Null" /></returns>
        public static bool TryGetRecordType (this ILoquiRegistration registration, out RecordType recordType)
        {
            if (registration.GetType().GetField("TriggeringRecordType")?.GetValue(registration) is RecordType type)
            {
                recordType = type;
                return recordType != RecordType.Null;
            }

            recordType = RecordType.Null;
            return false;
        }
    }
}