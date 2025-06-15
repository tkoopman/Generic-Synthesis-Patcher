using Common;

using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Universal.Json.Operations
{
    public class FormKeyListOperation<TMajor> : FormKeyListOperation where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        private bool exists;
        private bool existsChecked;

        // Required for OperationsConverter
        public FormKeyListOperation (string? value) : base(value) { }

        public FormKeyListOperation (ListLogic operation, FormKey value) : base(operation, value)
        {
        }

        public override FormKeyListOperation<TMajor> Inverse () => (FormKeyListOperation<TMajor>)base.Inverse();

        /// <summary>
        ///     Returns link getter but only if to a valid record.
        /// </summary>
        /// <returns>Null if FormKey doesn't resolve to a record of type TMajor</returns>
        public virtual IFormLinkGetter<TMajor>? ToLinkGetter ()
        {
            if (!existsChecked)
            {
                exists = Global.Game.State.LinkCache.TryResolve<TMajor>(Value, out _);

                if (!exists)
                    Global.Logger.Log(0xFF, $"Unable to find {Value} to link to.", logLevel: LogLevel.Warning);

                existsChecked = true;
            }

            return exists ? Value.ToLinkGetter<TMajor>() : null;
        }

        protected override FormKey convertValue (string? value)
        {
            if (value is null)
                return FormKey.Null;

            if (!Mod.TryFindFormKey<TMajor>(value, out var formKey, out existsChecked))
                throw new JsonSerializationException($"Unable to parse \"{value}\" into valid FormKey or EditorID");

            exists = existsChecked;

            return formKey;
        }
    }

    public class FormKeyListOperation : ListOperationBase<FormKey>
    {
        // Required for OperationsConverter
        public FormKeyListOperation (string? value) : base(value) { }

        public FormKeyListOperation (ListLogic operation, FormKey value) : base(operation, value)
        {
        }

        public override FormKeyListOperation Inverse () => (FormKeyListOperation)base.Inverse();

        protected override FormKey convertValue (string? value)
            => value is null ? FormKey.Null
             : FormKey.TryFactory(SynthCommon.FixFormKey(value), out var formKey) ? formKey
             : throw new JsonSerializationException($"Unable to parse \"{value}\" into valid FormKey");
    }
}