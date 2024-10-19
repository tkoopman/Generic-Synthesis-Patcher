using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

namespace GenericSynthesisPatcher.Helpers
{
    internal static partial class Mod
    {
        private const int ClassLogPrefix = 0x100;

        /// <summary>
        /// Finds the master record of the current context record.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Overwritten master record. Null if current record is the master.</returns>
        public static IMajorRecordGetter? FindOrigin ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context )
            => Global.State.LinkCache.TryResolve(context.Record.FormKey, context.Record.Registration.GetterType, out var o, ResolveTarget.Origin)
                ? context.Record.Equals(o)
                    ? null
                    : o
                : null;

        public static string FixFormKey ( string input ) => RegexFormKey().Replace(input, m => m.Value.PadLeft(6, '0'));

        public static bool GetProperty<T> ( IMajorRecordGetter fromRecord, string propertyName, out T? value ) => GetProperty<T>(fromRecord, propertyName, out value, out _);

        public static bool GetPropertyForEditing<T> ( IMajorRecord patchRecord, string propertyName, [NotNullWhen(true)] out T? value )
        {
            if (!GetProperty(patchRecord, propertyName, out value, out var property))
                return false;

            if (value != null)
                return true;

            object? _value = System.Activator.CreateInstance(property.PropertyType);

            if (_value == null || _value is not T outValue)
            {
                LogHelper.Log(LogLevel.Error, patchRecord, propertyName, $"Failed to construct new {property.PropertyType} value for editing.", ClassLogPrefix | 0x32);
                return false;
            }

            value = outValue;
            property.SetValue(patchRecord, _value);

            LogHelper.Log(LogLevel.Trace, patchRecord, propertyName, "Created new value for editing.", ClassLogPrefix | 0x33);
            return true;
        }

        public static bool SetProperty<T> ( IMajorRecord patch, string propertyName, T? value )
        {
            var property = patch.GetType().GetProperty(propertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, patch, propertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x51);
                return false;
            }

            if (value == null)
                property.SetValue(patch, null);
            else if (value is string strValue && property.PropertyType == typeof(TranslatedString))
                property.SetValue(patch, new TranslatedString(Language.English, strValue));
            else
                property.SetValue(patch, value);

            return true;
        }

        public static bool TryFindFormKey<TMajor> ( string input, out FormKey formKey, out IFormLinkGetter<TMajor>? link ) where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        {
            link = null;
            if (FormKey.TryFactory(FixFormKey(input), out formKey))
                return true;

            if (Global.State.LinkCache.TryResolve<TMajor>(input, out var record))
            {
                formKey = record.FormKey;
                link = record.ToLinkGetter();
                LogHelper.Log(LogLevel.Trace, $"Mapped EditorID \"{input}\" to FormKey {formKey}", ClassLogPrefix | 0x61);
                return true;
            }

            return false;
        }

        private static bool GetProperty<T> ( IMajorRecordGetter fromRecord, string propertyName, out T? value, [NotNullWhen(true)] out PropertyInfo? property )
        {
            value = default;
            property = fromRecord.GetType().GetProperty(propertyName);
            if (property == null)
            {
                LogHelper.Log(LogLevel.Debug, fromRecord, propertyName, LogHelper.MissingProperty, ClassLogPrefix | 0x41);
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

            if (_value is not T __value)
            {
                LogHelper.LogInvalidTypeFound(LogLevel.Debug, fromRecord, propertyName, typeof(T).Name, _value.GetType().Name, ClassLogPrefix | 0x42);
                return false;
            }

            value = __value;

            return true;
        }

        [GeneratedRegex(@"^[0-9A-Fa-f]{1,6}")]
        private static partial Regex RegexFormKey ();
    }
}