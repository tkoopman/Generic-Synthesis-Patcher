using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Data;

using Loqui;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

using Noggog;

namespace GenericSynthesisPatcher.Helpers
{
    internal static class Mod
    {
        /// <summary>
        ///     Used to clear a property back to default instance of it's type. Good for clearing
        ///     list field's by setting it back to an empty list. Does not check if current value
        ///     already set to default instance.
        /// </summary>
        /// <param name="patchRecord">Editable version of the record to clear property on</param>
        /// <param name="propertyName">Name of property to clear</param>
        /// <returns>True if successfully cleared.</returns>
        public static bool ClearProperty (IMajorRecord patchRecord, string propertyName, int callerClassCode, [CallerLineNumber] int line = 0)
        {
            var property = patchRecord.GetType().GetProperty(propertyName);
            if (property is null)
            {
                Global.Logger.LogMissingProperty(propertyName, callerClassCode, line: line);
                return false;
            }

            object? value = System.Activator.CreateInstance(property.PropertyType);

            if (value is null)
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, $"Failed to construct new {property.PropertyType} value for clear.", callerClassCode, line: line);
                return false;
            }

            property.SetValue(patchRecord, value);

            Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordUpdated, "Cleared value.", callerClassCode, line: line);
            return true;
        }

        /// <inheritdoc cref="GenericSynthesisPatcher.Games.Universal.Action.IRecordAction.FindHPUIndex(ProcessingKeys, IEnumerable{IModContext{IMajorRecordGetter}}, IEnumerable{ModKey}?)" />
        public static IModContext<IMajorRecordGetter>? FindHPUIndex<T> (ProcessingKeys proKeys, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? endNodes, int callerClassCode, [CallerLineNumber] int line = 0)
        {
            bool nonNull = proKeys.Rule.HasForwardOption(ForwardOptions._nonNullMod);

            /// Get value from origin record as it shouldn't be included in AllRecordMods as would
            /// of been filtered out in Program.getAvailableMods due to NonDefault being set by HPU
            if (!Mod.TryGetProperty<T>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var defaultValue, callerClassCode, line))
                return null;

            List<T?> history = [defaultValue];
            IModContext<IMajorRecordGetter>? hpu = null;
            int hpuHistory = -1;

            // We process in reverse as AllRecordMods is highest to lowest, and we want lowest to highest
            foreach (var mc in AllRecordMods.Reverse())
            {
                if (Mod.TryGetProperty<T>(mc.Record, proKeys.Property.PropertyName, out var curValue, callerClassCode, line)
                    && (!nonNull || !Mod.IsNullOrEmpty(curValue)))
                {
                    int historyIndex = history.IndexOf(curValue);
                    if (historyIndex == -1)
                    {
                        historyIndex = history.Count;
                        history.Add(curValue);
                        Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Forwarding Type: {nameof(ForwardOptions.HPU)}. Added value from {mc.ModKey} to history", callerClassCode, line: line);
                    }

                    // Keep out of history check as could of been added to history by a non-end node mod
                    if (endNodes is null || endNodes.Contains(mc.ModKey))
                    {
                        // If this a valid mod to be selected then check when it's value was added
                        // to history and if higher or equal we found new HPU
                        if (hpuHistory <= historyIndex)
                        {
                            hpu = mc;
                            hpuHistory = historyIndex;
                            Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordProcessing, $"Forwarding Type: {nameof(ForwardOptions.HPU)}. Updated HPU to {mc.ModKey}.", callerClassCode, line: line);
                        }
                    }
                }
            }

            return hpu;
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
            && Global.Game.State.LinkCache.TryResolveSimpleContext(context.Record.FormKey, context.Record.Registration.GetterType, out var o, ResolveTarget.Origin)
            ? o : context;

        /// <summary>
        ///     Checks if random object equals null or default value.
        /// </summary>
        public static bool IsNullOrEmpty<T> (T value)
            => value is string valueString ? string.IsNullOrEmpty(valueString)
            : value is IEnumerable valueList ? !valueList.Any()
            : value is null || value.Equals(default(T));

        /// <summary>
        ///     Attempts to find a record from the input string.
        /// </summary>
        /// <typeparam name="TMajor">Record type trying to find.</typeparam>
        /// <param name="input">FormKey or EditorID of record to find.</param>
        /// <param name="formKey">FormKey of found record.</param>
        /// <param name="wasEditorID">Was input an EditorID. False if was FormKey.</param>
        /// <returns>True if record found.</returns>
        public static bool TryFindFormKey<TMajor> (string input, out FormKey formKey, out bool wasEditorID, int callerClassCode, [CallerLineNumber] int line = 0) where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        {
            wasEditorID = false;
            if (FormKey.TryFactory(SynthCommon.FixFormKey(input), out formKey))
                return true;

            if (Global.Game.State.LinkCache.TryResolve<TMajor>(input, out var record))
            {
                wasEditorID = true;
                formKey = record.FormKey;
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordFound, $"Mapped EditorID \"{input}\" to FormKey {formKey}", callerClassCode, line: line);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Attempts to get a property from the a record.
        /// </summary>
        /// <typeparam name="T">
        ///     Property type to return. Property must be assignable to this else will fail.
        /// </typeparam>
        /// <param name="fromRecord">Record to retrieve property from</param>
        /// <param name="propertyName">Name of property to get</param>
        /// <param name="value">
        ///     Output of the value assigned to the property. Can be null even if successfully
        ///     retrieved if property it self is nullable.
        /// </param>
        /// <param name="propertyType">Actual property type.</param>
        /// <returns>
        ///     True if property exists, is successfully gotten and is assignable to T, else false.
        /// </returns>
        public static bool TryGetProperty<T> (object? fromRecord, string propertyName, out T? value, [NotNullWhen(true)] out Type? propertyType, int callerClassCode, [CallerLineNumber] int line = 0)
        {
            if (fromRecord is null)
            {
                value = default;
                propertyType = null;
                return false;
            }

            if (tryGetPropertyFromHierarchy(fromRecord, propertyName, false, out object? _value, out _, out var property, callerClassCode, line) && convertPropertyNullable(property, _value, out value, callerClassCode, line))
            {
                propertyType = property.PropertyType;
                return true;
            }

            value = default;
            propertyType = null;
            return false;
        }

        /// <inheritdoc cref="Mod.TryGetProperty{T}(object?, string, out T?, out Type?)" />
        public static bool TryGetProperty<T> (object? fromRecord, string propertyName, out T? value, int callerClassCode, [CallerLineNumber] int line = 0) => TryGetProperty(fromRecord, propertyName, out value, out _, callerClassCode, line);

        /// <summary>
        ///     Attempts to get a property value for editing. This means if currently the property
        ///     is set to null, it will try to create a new default value for the property.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="patchRecord">The writable patch record.</param>
        /// <param name="propertyName">Name of the property to set.</param>
        /// <param name="value">Output of the property value.</param>
        /// <param name="parent">Output of the parent object to call set on.</param>
        /// <param name="property">PropertyInfo of the property to set.</param>
        /// <returns>True if property exists, and is valid property you can set.</returns>
        public static bool TryGetPropertyForSetting<T> (ILoquiObject patchRecord, string propertyName, out T? value, [NotNullWhen(true)] out object? parent, [NotNullWhen(true)] out PropertyInfo? property, int callerClassCode, [CallerLineNumber] int line = 0)
        {
            value = default;
            return tryGetPropertyFromHierarchy(patchRecord, propertyName, true, out object? _value, out parent, out property, callerClassCode, line) && property.IsValidPropertyType() && convertPropertyNullable(property, _value, out value, callerClassCode, line);
        }

        /// <summary>
        ///     Attempts to get a property value for editing. This means if currently the property
        ///     is set to null, it will try to create a new default value for the property.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="patchRecord">The writable patch record.</param>
        /// <param name="propertyName">Name of the property to set.</param>
        /// <param name="value">Output of the property value.</param>
        /// <returns>
        ///     True if property exists, was not null, or non-null default value for it was able to
        ///     be set and returned.
        /// </returns>
        public static bool TryGetPropertyValueForEditing<T> (IMajorRecord patchRecord, string propertyName, [NotNullWhen(true)] out T? value, int callerClassCode, [CallerLineNumber] int line = 0)
        {
            value = default;
            if (!tryGetPropertyFromHierarchy(patchRecord, propertyName, true, out object? _value, out object? parent, out var property, callerClassCode, line))
                return false;

            if (!property.IsValidPropertyType() || parent is null)
            {
                Global.Logger.LogMissingProperty(propertyName, callerClassCode, line: line);
                return false;
            }

            if (convertProperty(property, _value, out value, callerClassCode, line))
                return true;

            _value = setDefaultPropertyValue(parent, property, callerClassCode, line);

            return convertProperty(property, _value, out value, callerClassCode, line);
        }

        /// <summary>
        ///     Attempts to set a property on a record to the specified value.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="patchRecord">The writable patch record.</param>
        /// <param name="propertyName">Name of the property to set.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>True if property exists and was set to the new value.</returns>
        public static bool TrySetProperty<T> (IMajorRecord patchRecord, string propertyName, T? value, int callerClassCode, [CallerLineNumber] int line = 0)
        {
            if (!TryGetPropertyForSetting<T>(patchRecord, propertyName, out _, out object? parent, out var property, callerClassCode, line))
                return false;

            try
            {
                if (value is null)
                    property.SetValue(parent, null);
                else if (value is string strValue && property.PropertyType == typeof(TranslatedString))
                    property.SetValue(parent, new TranslatedString(Language.English, strValue));
                else
                    property.SetValue(parent, value);

                return true;
            }
            catch (Exception ex)
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, $"Failed to set property to new value. {ex.Message}", callerClassCode, line: line);
                return false;
            }
        }

        /// <summary>
        ///     Converts the input value to the specified type T.
        /// </summary>
        /// <typeparam name="T">Output type</typeparam>
        /// <param name="propertyName">Property name. Just used for logging.</param>
        /// <param name="property">PropertyInfo of the input value.</param>
        /// <param name="input">Input property value</param>
        /// <param name="output">
        ///     Output of converted value. If input is null and T is struct will return default
        ///     struct value.
        /// </param>
        /// <returns>
        ///     True if input was successfully converted to output type and output is not null.
        /// </returns>
        private static bool convertProperty<T> (PropertyInfo property, object? input, [NotNullWhen(true)] out T? output, int callerClassCode, [CallerLineNumber] int line = 0) => convertPropertyNullable(property, input, out output, callerClassCode, line) && output is not null;

        /// <summary>
        ///     Converts the input value to the specified type T, if possible supporting nullable values.
        /// </summary>
        /// <typeparam name="T">Output type</typeparam>
        /// <param name="propertyName">Property name. Just used for logging.</param>
        /// <param name="property">PropertyInfo of the input value.</param>
        /// <param name="input">Input property value</param>
        /// <param name="output">
        ///     Output of converted value. If input is null and T is struct will return default
        ///     struct value.
        /// </param>
        /// <returns>True if input was successfully converted to output type.</returns>
        private static bool convertPropertyNullable<T> (PropertyInfo property, object? input, out T? output, int callerClassCode, [CallerLineNumber] int line = 0)
        {
            output = default;

            if (input is null)
                return true;

            if (typeof(T) == typeof(string) && input is ITranslatedStringGetter translatedString)
            {
                input = translatedString.String;
                if (input is null)
                    return true;
            }
            else if (typeof(T) == typeof(int) && property.PropertyType.IsEnum)
            {
                input = (int)input;
            }

            if (input is not T value)
            {
                Global.Logger.LogInvalidTypeFound(typeof(T).FullName ?? typeof(T).Name, input.GetType().FullName ?? input.GetType().Name, callerClassCode, line: line);
                return false;
            }

            output = value;
            return true;
        }

        /// <summary>
        ///     Sets property to the default non-null value of the property type.
        /// </summary>
        /// <param name="parent">Parent object that property exists on.</param>
        /// <param name="property">PropertyInfo of the property to set to default.</param>
        /// <returns>
        ///     Default value that property was just set to. Null if couldn't find property or
        ///     failed to set to default value.
        /// </returns>
        private static object? setDefaultPropertyValue (object? parent, PropertyInfo property, int callerClassCode, [CallerLineNumber] int line = 0)
        {
            if (!property.IsValidPropertyType())
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, $"Failed to construct new {property.PropertyType} as invalid property type.", callerClassCode, line: line);
                return null;
            }

            try
            {
                object? _value = System.Activator.CreateInstance(property.PropertyType);

                property.SetValue(parent, _value);
                Global.Logger.WriteLog(LogLevel.Trace, LogType.RecordUpdated, "Created new value for editing.", callerClassCode, line: line);
                return _value;
            }
            catch { }

            Global.Logger.WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, $"Failed to construct new {property.PropertyType} value for editing.", callerClassCode, line: line);
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
        private static bool tryGetPropertyFromHierarchy (object from, string propertyName, bool createParents, out object? value, out object? parent, [NotNullWhen(true)] out PropertyInfo? property, int callerClassCode, [CallerLineNumber] int line = 0)
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
                if (property is null)
                {
                    Global.Logger.LogMissingProperty(propertyName, callerClassCode, line: line);
                    return false;
                }

                // We still continue processing if null to make sure the full property name is valid
                object? propertyValue = parent is not null ? property.GetValue(parent) : null;
                if (i < propertyNames.Length - 1)
                {
                    if (propertyValue is null && createParents)
                    {
                        propertyValue = setDefaultPropertyValue(parent, property, callerClassCode, line);
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

            // Check property is not null one last time just for off chance propertyNames had no entries
            if (property is null)
            {
                Global.Logger.LogMissingProperty(propertyName, callerClassCode, line: line);
                return false;
            }

            return true;
        }
    }
}