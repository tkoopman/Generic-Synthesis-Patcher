using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

using Loqui;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

namespace GenericSynthesisPatcher.Helpers
{
    internal static partial class Mod
    {
        private const int ClassLogCode = 0x02;

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
        /// Finds the master record of the current context record.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Overwritten master record. Null if current record is the master.</returns>
        public static IMajorRecordGetter? FindOrigin (IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context)
            => Global.State.LinkCache.TryResolve(context.Record.FormKey, context.Record.Registration.GetterType, out var o, ResolveTarget.Origin)
                ? context.Record.Equals(o)
                    ? null
                    : o
                : null;

        public static string FixFormKey (string input) => RegexFormKey().Replace(input, m => m.Value.PadLeft(6, '0'));

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

        public static bool TryGetProperty<T> (ILoquiObject fromRecord, string propertyName, out T? value, [NotNullWhen(true)] out PropertyInfo? property)
        {
            value = default;

            if (!TryGetPropertyFromHierarchy(fromRecord, propertyName, out object? _value, out property))
                return false;

            if (_value == null)
                return true;

            if (typeof(T) == typeof(string) && _value is ITranslatedStringGetter str)
            {
                if (str.String is T tValue)
                    _value = tValue;
            }

            if (_value is not T __value)
            {
                Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, propertyName, typeof(T).FullName ?? typeof(T).Name, _value.GetType().FullName ?? _value.GetType().Name);
                return false;
            }

            value = __value;

            return true;
        }

        public static bool TryGetProperty<T> (ILoquiObject fromRecord, string propertyName, out T? value) => TryGetProperty(fromRecord, propertyName, out value, out _);

        public static bool TryGetPropertyForEditing<T> (IMajorRecord patchRecord, string propertyName, [NotNullWhen(true)] out T? value)
        {
            if (!TryGetProperty(patchRecord, propertyName, out value, out var property))
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Failed to find property.", propertyName: propertyName);
                return false;
            }

            if (value != null)
                return true;

            // TODO - Update following code to support Hierarchy
            object? _value = System.Activator.CreateInstance(property.PropertyType);

            if (_value == null || _value is not T outValue)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to construct new {property.PropertyType} value for editing.", logLevel: LogLevel.Error, propertyName: propertyName);
                return false;
            }

            value = outValue;
            property.SetValue(patchRecord, _value);

            Global.TraceLogger?.Log(ClassLogCode, "Created new value for editing.", propertyName: propertyName);
            return true;
        }

        public static bool TrySetProperty<T> (IMajorRecord patchRecord, string propertyName, T? value)
        {
            var property = patchRecord.GetType().GetProperty(propertyName);
            if (property == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                return false;
            }

            if (value == null)
                property.SetValue(patchRecord, null);
            else if (value is string strValue && property.PropertyType == typeof(TranslatedString))
                property.SetValue(patchRecord, new TranslatedString(Language.English, strValue));
            else
                property.SetValue(patchRecord, value);

            return true;
        }

        [GeneratedRegex(@"^[0-9A-Fa-f]{1,6}")]
        private static partial Regex RegexFormKey ();

        /// <summary>
        /// Attempt to get a property from input object. Supports property name hierarchy.
        /// </summary>
        /// <param name="from">         Object to get property value from </param>
        /// <param name="propertyName"> Name of property, using period (.) to seperate </param>
        /// <param name="value">
        /// Property value. Can be null, even if true returned, if property name was valid but
        /// returned null. When using property hierarchy, this will also be the case if any parent
        /// in the hierarchy return null.
        /// </param>
        /// <param name="property">     Property Info of resulting value </param>
        /// <returns> True if property name existed, even if result of valid property was null </returns>
        private static bool TryGetPropertyFromHierarchy (object from, string propertyName, out object? value, [NotNullWhen(true)] out PropertyInfo? property) => TryGetPropertyFromHierarchy(from, from.GetType(), propertyName, out value, out property);

        /// <summary>
        /// Attempt to get a property from input object. Supports property name hierarchy.
        /// Include type of from for property name validation when from is null.
        /// </summary>
        /// <param name="from">         Object to get property value from </param>
        /// <param name="fromType">     Type of from</param>
        /// <param name="propertyName"> Name of property, using period (.) to seperate </param>
        /// <param name="value">
        /// Property value. Can be null, even if true returned, if property name was valid but
        /// returned null value. When using property hierarchy, this will also be the case if any parent
        /// in the hierarchy return null, however children still checked to make sure valid property name.
        /// </param>
        /// <param name="property">     Property Info of resulting value </param>
        /// <returns> True if property name existed, even if result of valid property was null </returns>
        private static bool TryGetPropertyFromHierarchy (object? from, Type fromType, string propertyName, out object? value, [NotNullWhen(true)] out PropertyInfo? property)
        {
            value = default;
            property = null;
            string[] propertyNames = propertyName.Split('.');

            for (int i = 0; i < propertyNames.Length; i++)
            {
                string name = propertyNames[i];
                property = fromType.GetProperty(name);
                if (property == null)
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                    return false;
                }

                value = from is not null ? property.GetValue(from) : null;
                if (i < propertyNames.Length - 1)
                {
                    value = default;
                    fromType = property.PropertyType;
                }
            }

            // Check property is not null one last time just for off chance propertyNames had no entries
            if (property == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                return false;
            }

            return true;
        }
    }
}