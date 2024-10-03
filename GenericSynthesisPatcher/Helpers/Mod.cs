using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

using Global = GenericSynthesisPatcher.Global;

namespace GenericSynthesisPatcher.Helpers
{
    // Log Code: 0x7xx
    internal static class Mod
    {
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

        private static readonly Dictionary<(FileName, string), IModListing<ISkyrimModGetter>?> ModCache = [];

        /// <summary>
        /// Find overwritten version of context's record from provided mod file name if it exists.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="modFileName"></param>
        /// <returns>Record Getter if found.</returns>
        /// Log Code: 0x71x
        public static IMajorRecordGetter? GetModRecord ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, string modFileName )
        {
            var modGetter = TryGetOverridingMod(context, modFileName);
            if (modGetter == null)
            {
                LogHelper.Log(LogLevel.Trace, context, $"Forward mod '{modFileName}' doesn't override '{context.Record.FormKey.ModKey.FileName}'.", 0x711);
                return null;
            }

            if ((modGetter.Mod?.ToImmutableLinkCache().TryResolveContext(context.Record.FormKey, context.Record.Registration.GetterType, out var forwardContext) ?? false) && forwardContext != null)
            {
                LogHelper.Log(LogLevel.Trace, context, $"Found matching record in forwarding mod {modFileName}.", 0x712);
                return forwardContext.Record;
            }

            LogHelper.Log(LogLevel.Trace, context, $"No matching record in forwarding mod {modFileName} of type {context.Record.Registration.GetterType.Name}.", 0x713);
            return null;
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
                    LogHelper.Log(LogLevel.Trace, context, $"Returned null value for ('{context.Record.FormKey.ModKey.FileName}', '{modFileName}')");
                else
                    LogHelper.Log(LogLevel.Trace, context, $"Returned cached mod for ('{context.Record.FormKey.ModKey.FileName}', '{modFileName}')");
                return getter;
            }

            LogHelper.Log(LogLevel.Trace, context, $"Searching for forwarding record as not currently cached.");

            if (context.Record.FormKey.ModKey.FileName.Equals(modFileName))
            {
                ModCache.Add((context.Record.FormKey.ModKey.FileName, modFileName), null);
                LogHelper.Log(LogLevel.Information, context, $"Skipping. Can't forward to self.");
                return null;
            }

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

            bool isMaster = false;
            foreach (var m in modGetter?.Mod?.MasterReferences ?? [])
            {
                if (m.Master.Equals(context.Record.FormKey.ModKey))
                    isMaster = true;
            }

            if (!isMaster)
            {
                LogHelper.Log(LogLevel.Debug, $"Forwarding mod {modFileName} doesn't contain master record for {context.Record.FormKey.ModKey.FileName}.");
                modGetter = null;
            }
            else
            {
                ModCache.Add((context.Record.FormKey.ModKey.FileName, modFileName), modGetter);

                if (modGetter == null)
                    LogHelper.Log(LogLevel.Debug, $"Unable to find forwarding mod {modFileName} after master {context.Record.FormKey.ModKey.FileName}.");
                else
                    LogHelper.Log(LogLevel.Debug, $"Found forwarding mod {modFileName} after master {context.Record.FormKey.ModKey.FileName}.");
            }

            return modGetter;
        }
    }
}