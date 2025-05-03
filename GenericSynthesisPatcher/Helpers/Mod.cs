using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

using Loqui;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

using Noggog;

namespace GenericSynthesisPatcher.Helpers
{
    internal static partial class Mod
    {
        private const int ClassLogCode = 0x02;

        /// <summary>
        ///     Used to clear a property back to default instance of it's type. Good for clearing
        ///     list field's by setting it back to an empty list. Does not check if current value
        ///     already set to default instance.
        /// </summary>
        /// <param name="patchRecord">Editable version of the record to clear property on</param>
        /// <param name="propertyName">Name of property to clear</param>
        /// <returns>True if successfully cleared.</returns>
        public static bool ClearProperty (IMajorRecord patchRecord, string propertyName)
        {
            var property = patchRecord.GetType().GetProperty(propertyName);
            if (property == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                return false;
            }

            object? value = System.Activator.CreateInstance(property.PropertyType);

            if (value == null)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to construct new {property.PropertyType} value for clear.", logLevel: LogLevel.Error, propertyName: propertyName);
                return false;
            }

            property.SetValue(patchRecord, value);

            Global.TraceLogger?.Log(ClassLogCode, "Cleared value.", propertyName: propertyName);
            return true;
        }

        /// <summary>
        ///     Finds the master record context of the current record context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        ///     Overwritten master record's context. Null if current record context is the master.
        /// </returns>
        public static IModContext<IMajorRecordGetter> FindOriginContext (IModContext<IMajorRecordGetter> context)
            => !context.IsMaster()
            && Global.State.LinkCache.TryResolveSimpleContext(context.Record.FormKey, context.Record.Registration.GetterType, out var o, ResolveTarget.Origin)
            ? o : context;

        /// <summary>
        ///     Adds 0 padding to String representation of a form key
        /// </summary>
        /// <param name="input">
        ///     String representation of a form key that may not be padded
        /// </param>
        /// <returns>
        ///     String representation of the form key with 0 padding added if required
        /// </returns>
        public static string FixFormKey (string input) => RegexFormKey().Replace(input, m => m.Value.PadLeft(6, '0'));

        /// <summary>
        ///     Checks if random object equals null or default value.
        /// </summary>
        public static bool IsNullOrEmpty<T> (T value)
            => value is string valueString ? string.IsNullOrEmpty(valueString)
            : value is IEnumerable valueList ? !valueList.Any()
            : value is null || value.Equals(default(T));

        public static bool TryFindFormKey<TMajor> (string input, out FormKey formKey, out bool wasEditorID) where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        {
            wasEditorID = false;
            if (FormKey.TryFactory(FixFormKey(input), out formKey))
                return true;

            if (Global.State.LinkCache.TryResolve<TMajor>(input, out var record))
            {
                wasEditorID = true;
                formKey = record.FormKey;
                Global.TraceLogger?.Log(ClassLogCode, $"Mapped EditorID \"{input}\" to FormKey {formKey}");
                return true;
            }

            return false;
        }

        public static bool TryGetProperty<T> (object? fromRecord, string propertyName, out T? value, [NotNullWhen(true)] out Type? propertyType)
        {
            if (fromRecord == null)
            {
                value = default;
                propertyType = null;
                return false;
            }

            if (tryGetPropertyFromHierarchy(fromRecord, propertyName, false, out object? _value, out _, out var property) && convertPropertyNullable(propertyName, property, _value, out value))
            {
                propertyType = property.PropertyType;
                return true;
            }

            value = default;
            propertyType = null;
            return false;
        }

        public static bool TryGetProperty<T> (object? fromRecord, string propertyName, out T? value) => TryGetProperty(fromRecord, propertyName, out value, out _);

        public static bool TryGetPropertyForSetting<T> (ILoquiObject patchRecord, string propertyName, out T? value, [NotNullWhen(true)] out object? parent, [NotNullWhen(true)] out PropertyInfo? property)
        {
            value = default;
            return tryGetPropertyFromHierarchy(patchRecord, propertyName, true, out object? _value, out parent, out property) && property.CanWrite && convertPropertyNullable(propertyName, property, _value, out value);
        }

        public static bool TryGetPropertyValueForEditing<T> (IMajorRecord patchRecord, string propertyName, [NotNullWhen(true)] out T? value)
        {
            if (!tryGetPropertyFromHierarchy(patchRecord, propertyName, true, out object? _value, out object? parent, out var property) || !property.CanWrite || parent is null)
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                value = default;
                return false;
            }

            if (convertProperty(propertyName, property, _value, out value))
                return true;

            _value = setDefaultPropertyValue(parent, property);

            if (convertProperty(propertyName, property, _value, out value))
            {
                Global.TraceLogger?.Log(ClassLogCode, "Created new value for editing.", propertyName: propertyName);
                return true;
            }

            Global.Logger.Log(ClassLogCode, $"Failed to construct new {property.PropertyType} value for editing.", logLevel: LogLevel.Error, propertyName: propertyName);
            return false;
        }

        public static bool TrySetProperty<T> (IMajorRecord patchRecord, string propertyName, T? value)
        {
            if (!TryGetPropertyForSetting<T>(patchRecord, propertyName, out _, out object? parent, out var property))
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                return false;
            }

            if (value == null)
                property.SetValue(parent, null);
            else if (value is string strValue && property.PropertyType == typeof(TranslatedString))
                property.SetValue(parent, new TranslatedString(Language.English, strValue));
            else
                property.SetValue(parent, value);

            return true;
        }

        private static bool convertProperty<T> (string propertyName, PropertyInfo property, object? input, [NotNullWhen(true)] out T? output) => convertPropertyNullable(propertyName, property, input, out output) && output is not null;

        private static bool convertPropertyNullable<T> (string propertyName, PropertyInfo property, object? input, out T? output)
        {
            output = default;

            if (input == null)
                return true;

            if (typeof(T) == typeof(string) && input is ITranslatedStringGetter translatedString)
            {
                input = translatedString.String;
                if (input == null)
                    return true;
            }
            else if (typeof(T) == typeof(int) && property.PropertyType.IsEnum)
            {
                input = (int)input;
            }

            if (input is not T value)
            {
                Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, propertyName, typeof(T).FullName ?? typeof(T).Name, input.GetType().FullName ?? input.GetType().Name);
                return false;
            }

            output = value;
            return true;
        }

        [GeneratedRegex(@"^[0-9A-Fa-f]{1,6}")]
        private static partial Regex RegexFormKey ();

        private static object? setDefaultPropertyValue (object? parent, PropertyInfo property)
        {
            if (!property.CanWrite)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to construct new {property.PropertyType} parent value for editing as not writable.", logLevel: LogLevel.Error, propertyName: property.Name);
                return null;
            }

            try
            {
                object? _value = System.Activator.CreateInstance(property.PropertyType);

                property.SetValue(parent, _value);
                Global.TraceLogger?.Log(ClassLogCode, "Created new value for editing.", propertyName: property.Name);
                return _value;
            }
            catch { }

            Global.Logger.Log(ClassLogCode, $"Failed to construct new {property.PropertyType} value for editing.", logLevel: LogLevel.Error, propertyName: property.Name);
            return null;
        }

        /// <summary>
        ///     Attempt to get a property from input object. Supports property name hierarchy.
        ///     Include type of from for property name validation when from is null.
        /// </summary>
        /// <param name="from">Object to get property value from</param>
        /// <param name="propertyName">Name of property, using period (.) to separate</param>
        /// <param name="value">
        ///     Property value. Can be null, even if true returned, if property name was valid but
        ///     returned null value. When using property hierarchy, this will also be the case if
        ///     any parent in the hierarchy return null, however children still checked to make sure
        ///     valid property name.
        /// </param>
        /// <param name="property">Property Info of resulting value</param>
        /// <returns>
        ///     True if property name existed, even if result of valid property was null
        /// </returns>
        private static bool tryGetPropertyFromHierarchy (object from, string propertyName, bool createParents, out object? value, out object? parent, [NotNullWhen(true)] out PropertyInfo? property)
        {
            parent = from;
            var parentType = from.GetType();

            value = null;
            property = null;
            string[] propertyNames = propertyName.Split('.');

            for (int i = 0; i < propertyNames.Length; i++)
            {
                string name = propertyNames[i];
                property = parentType.GetProperty(name);
                if (property == null)
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                    return false;
                }

                // We still continue processing if null to make sure the full property name is valid
                object? propertyValue = parent is not null ? property.GetValue(parent) : null;
                if (i < propertyNames.Length - 1)
                {
                    if (propertyValue is null && createParents)
                    {
                        propertyValue = setDefaultPropertyValue(parent, property);
                        if (propertyValue is null)
                            return false;
                    }

                    parent = propertyValue;
                    parentType = property.PropertyType;
                }
                else
                {
                    value = propertyValue;
                }
            }

            // Check property is not null one last time just for off chance propertyNames had no
            // entries
            if (property == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                return false;
            }

            return true;
        }
    }
}