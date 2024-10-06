using System;

using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Data;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Helpers
{
    public class RecordCallData<T> ( string propertyName ) : RecordCallData( propertyName ) where T : class, IAction
    {
        public override bool CanFill () => T.CanFill();
        public override bool CanForward () => T.CanForward();
        public override bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, Helpers.RecordCallData rcd ) => T.Fill(context, origin, rule, valueKey, rcd);
        public override bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, Helpers.RecordCallData rcd ) => T.Forward(context, origin, rule, forwardContext, rcd);
    }
    public abstract class RecordCallData ( string propertyName )
    {
        public string PropertyName { get; } = propertyName;

        public abstract bool CanFill ();
        public abstract bool CanForward ();
        public abstract bool Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, Helpers.RecordCallData rcd );
        public abstract bool Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd );
    }
}