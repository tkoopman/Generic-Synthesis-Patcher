using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Data;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers
{
    public class RecordCallData<T> ( string propertyName ) : RecordCallData(propertyName) where T : class, IAction
    {
        public override bool CanFill () => T.CanFill();

        public override bool CanForward () => T.CanForward();

        public override bool CanForwardSelfOnly () => T.CanForwardSelfOnly();

        public override int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, Helpers.RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => T.Fill(context, rule, valueKey, rcd, ref patchedRecord);

        public override int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, Helpers.RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => T.Forward(context, rule, forwardContext, rcd, ref patchedRecord);

        public override int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, Helpers.RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord ) => T.ForwardSelfOnly(context, rule, forwardContext, rcd, ref patchedRecord);

        public override bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd ) => T.Matches(check, rule, valueKey, rcd);

        public override bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd ) => T.Matches(check, origin, rcd);
    }

    public abstract class RecordCallData ( string propertyName )
    {
        public string PropertyName { get; } = propertyName;

        public abstract bool CanFill ();

        public abstract bool CanForward ();

        public abstract bool CanForwardSelfOnly ();

        public abstract int Fill ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, ValueKey valueKey, Helpers.RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord );

        public abstract int Forward ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord );

        public abstract int ForwardSelfOnly ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext, RecordCallData rcd, ref ISkyrimMajorRecord? patchedRecord );

        public abstract bool Matches ( ISkyrimMajorRecordGetter check, GSPRule rule, ValueKey valueKey, RecordCallData rcd );

        public abstract bool Matches ( ISkyrimMajorRecordGetter check, IMajorRecordGetter? origin, RecordCallData rcd );
    }
}