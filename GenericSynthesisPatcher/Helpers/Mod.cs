using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

using Noggog;

namespace GenericSynthesisPatcher.Helpers
{
    internal static class Mod
    {
        private const int ClassLogPrefix = 0x100;
        private static readonly Dictionary<(FileName, string), IModListing<ISkyrimModGetter>?> ModCache = [];

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

        /// <summary>
        /// Find overwritten version of context's record from provided mod file name if it exists.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="modFileName"></param>
        /// <returns>Record Getter with Context if found.</returns>
        public static IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>? GetModRecord ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string modFileName )
        {
            var modGetter = TryGetOverridingMod(context, modFileName);
            if (modGetter == null)
            {
                LogHelper.Log(LogLevel.Trace, context, $"Forward mod '{modFileName}' doesn't override '{context.Record.FormKey.ModKey.FileName}'.", ClassLogPrefix | 0x11);
                return null;
            }

            if ((modGetter.Mod?.ToImmutableLinkCache().TryResolveContext(context.Record.FormKey, context.Record.Registration.GetterType, out var forwardContext) ?? false) && forwardContext != null)
            {
                if (modFileName.Equals(forwardContext.ModKey.FileName.String, StringComparison.OrdinalIgnoreCase))
                {
                    LogHelper.Log(LogLevel.Trace, context, $"Found matching record in forwarding mod {modFileName}.", ClassLogPrefix | 0x12);
                    return forwardContext;
                }

                LogHelper.Log(LogLevel.Trace, context, $"Found incorrect matching record in forwarding mod {modFileName} != {forwardContext.ModKey.FileName.String}.", ClassLogPrefix | 0x13);
                return null;
            }

            LogHelper.Log(LogLevel.Trace, context, $"No matching record in forwarding mod {modFileName} of type {context.Record.Registration.GetterType.Name}.", ClassLogPrefix | 0x14);
            return null;
        }

        public static bool GetProperty<T> ( IMajorRecordGetter fromRecord, string propertyName, out T? value ) => GetProperty<T>(fromRecord, propertyName, out value, out _);

        public static bool GetPropertyForEditing<T> ( IMajorRecord patchRecord, string propertyName, [NotNullWhen(true)] out T? value )
        {
            if (!GetProperty(patchRecord, propertyName, out value, out var property))
                return false;

            if (value != null)
                return true;

            var constructor = property.PropertyType.GetConstructor([]);
            if (constructor == null)
            {
                LogHelper.Log(LogLevel.Error, patchRecord, propertyName, "Failed to construct new value for editing.", ClassLogPrefix | 0x31);
                return false;
            }

            object _value = constructor.Invoke([]);
            if (value == null)
            {
                LogHelper.Log(LogLevel.Error, patchRecord, propertyName, "Failed to construct new value for editing.", ClassLogPrefix | 0x32);
                return false;
            }

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

        /// <summary>
        /// Returns Mod Getter for provided mod's file name IF:
        ///   File exists in load order and
        ///   is after the Master mod for the context record
        ///   else returns null
        ///
        /// Caches results for context record's mod and file name combination.
        /// </summary>
        /// <param name="context">Current record's context to make sure master is before mod you looking for.</param>
        /// <param name="modFileName">File name of mod to find</param>
        /// <returns>Mod Getter is exists after master record for context. Null otherwise.</returns>
        private static IModListing<ISkyrimModGetter>? TryGetOverridingMod ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string modFileName )
        {
            if (ModCache.TryGetValue((context.Record.FormKey.ModKey.FileName, modFileName), out var getter))
            {
                if (getter == null)
                    LogHelper.Log(LogLevel.Trace, context, $"Returned null value for ('{context.Record.FormKey.ModKey.FileName}', '{modFileName}')", ClassLogPrefix | 0x21);
                else
                    LogHelper.Log(LogLevel.Trace, context, $"Returned cached mod for ('{context.Record.FormKey.ModKey.FileName}', '{modFileName}')", ClassLogPrefix | 0x22);
                return getter;
            }

            if (context.ModKey.FileName.Equals(context.Record.FormKey.ModKey))
            {
                ModCache.Add((context.Record.FormKey.ModKey.FileName, modFileName), null);
                LogHelper.Log(LogLevel.Information, context, $"Skipping. Can't forward to self.", ClassLogPrefix | 0x23);
                return null;
            }

            LogHelper.Log(LogLevel.Trace, context, $"Searching for forwarding record as not currently cached.", ClassLogPrefix | 0x24);

            IModListing<ISkyrimModGetter>? modGetter = null;
            bool foundRecordMaster = false;
            Global.State.LoadOrder.ForEach(m =>
            {
                if (!foundRecordMaster && m.Key.FileName.Equals(context.Record.FormKey.ModKey.FileName))
                    foundRecordMaster = true;

                if (m.Key.FileName.Equals(modFileName))
                {
                    if (foundRecordMaster)
                        modGetter = m.Value;
                    return;
                }
            });

            if (modGetter == null)
            {
                LogHelper.Log(LogLevel.Debug, $"Unable to find forwarding mod {modFileName} after master {context.Record.FormKey.ModKey.FileName}.", ClassLogPrefix | 0x25);
            }
            else
            {
                bool isMaster = false;
                foreach (var m in modGetter?.Mod?.MasterReferences ?? [])
                {
                    if (m.Master.Equals(context.Record.FormKey.ModKey))
                        isMaster = true;
                }

                if (!isMaster)
                {
                    LogHelper.Log(LogLevel.Debug, $"Forwarding mod {modFileName} doesn't contain master record for {context.Record.FormKey.ModKey.FileName}.", ClassLogPrefix | 0x26);
                    modGetter = null;
                }
                else
                {
                    LogHelper.Log(LogLevel.Debug, $"Found forwarding mod {modFileName} after master {context.Record.FormKey.ModKey.FileName}.", ClassLogPrefix | 0x27);
                }
            }

            ModCache.Add((context.Record.FormKey.ModKey.FileName, modFileName), modGetter);
            return modGetter;
        }
    }
}