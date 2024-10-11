using System;

using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Data;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Helpers
{
    public class RecordCallData<T> ( string propertyName ) : RecordCallData(propertyName) where T : class, IAction
    {
        public override bool CanFill () => T.CanFill();

        public override bool CanForward () => T.CanForward();

        public override bool CanForwardSelfOnly () => T.CanForwardSelfOnly();

        public override int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, Helpers.RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => T.Fill(context, origin, rule, valueKey, rcd, ref patchedRecord);

        public override int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, Helpers.RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => T.Forward(context, origin, rule, forwardContext, rcd, ref patchedRecord);

        public override int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, Helpers.RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => T.ForwardSelfOnly(context, origin, rule, forwardContext, rcd, ref patchedRecord);
    }

    public abstract class RecordCallData ( string propertyName )
    {
        public string PropertyName { get; } = propertyName;

        public abstract bool CanFill ();

        public abstract bool CanForward ();

        public abstract bool CanForwardSelfOnly ();

        public abstract int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, GSPRule.ValueKey valueKey, Helpers.RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord );

        public abstract int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord );

        public abstract int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, IMajorRecordGetter? origin, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord );
    }
}