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

        /// <summary>
        /// Used to clear a property back to default instance of it's type.
        /// Good for clearing list field's by setting it back to an empty list.
        /// Does not check if current value already set to default instance.
        /// </summary>
        /// <param name="patchRecord">Editable version of the record to claer property on</param>
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
        /// Finds the master record of the current record context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Overwritten master record. Null if current record context is the master.</returns>
        public static IMajorRecordGetter? FindOrigin (IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context)
            => FindOriginContext(context)?.Record;

        /// <summary>
        /// Finds the master record context of the current record context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Overwritten master record's context. Null if current record context is the master.</returns>
        public static IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>? FindOriginContext (IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context)
            => !context.ModKey.Equals(context.Record.FormKey.ModKey)
            && Global.State.LinkCache.TryResolveContext(context.Record.FormKey, context.Record.Registration.GetterType, out var o, ResolveTarget.Origin)
            ? o : null;

        /// <summary>
        /// Adds 0 padding to String representation of a form key
        /// </summary>
        /// <param name="input">String representation of a form key that may not be padded</param>
        /// <returns>String representation of the form key with 0 padding added if required</returns>
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
            property = fromRecord.GetType().GetProperty(propertyName);
            if (property == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, LogHelper.MissingProperty, propertyName: propertyName);
                return false;
            }

            object? _value = property.GetValue(fromRecord);
            if (_value == null)
                return true;

            if (typeof(T) == typeof(string) && _value is ITranslatedStringGetter str)
            {
                if (str.String is T tValue)
                    _value = tValue;
            }
            else if (typeof(T) == typeof(int) && property.PropertyType.IsEnum)
            {
                Global.DebugLogger?.Log(ClassLogCode, $"{property.PropertyType}", propertyName: propertyName);
                _value = (int)_value;
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
    }
}