using System;

using GenericSynthesisPatcher.Json.Data;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public interface IAction
    {
        public static abstract bool CanFill ();
        public static abstract bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, RecordCallData rcd );

        public static abstract bool CanForward ();
        public static abstract bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IMajorRecordGetter forwardRecord, RecordCallData rcd );
    }
}