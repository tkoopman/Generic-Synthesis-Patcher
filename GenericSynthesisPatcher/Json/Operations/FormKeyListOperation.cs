using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    public class FormKeyListOperation : ListOperationBase<FormKeyListOperation, FormKey>
    {
        public override ListLogic Operation { get; protected set; }

        public override FormKey Value { get; protected set; }

        public FormKeyListOperation ( string value )
        {
            (Operation, string? v) = Split(value, ValidPrefixes);

            Value = FormKey.TryFactory(Mod.FixFormKey(v), out var formKey) ? formKey
                  : throw new JsonSerializationException($"Unable to parse \"{v}\" into valid FormKey");
        }

        public FormKeyListOperation ( ListLogic operation, FormKey value )
        {
            Operation = operation;
            Value = value;
        }

        protected FormKeyListOperation ()
        { }

        public override FormKeyListOperation Inverse () => new(InverseOperation(), Value);
    }

    public class FormKeyListOperation<TMajor> : FormKeyListOperation where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        private IFormLinkGetter<TMajor>? formLinkGetter;
        private bool linked = false;
        public override ListLogic Operation { get; protected set; }

        public override FormKey Value { get; protected set; }

        public FormKeyListOperation ( string value ) : base()
        {
            (Operation, string? v) = Split(value, ValidPrefixes);

            Value = Mod.TryFindFormKey<TMajor>(v, out var formKey, out formLinkGetter) ? formKey
                  : throw new JsonSerializationException($"Unable to parse \"{v}\" into valid FormKey or EditorID");
        }

        public FormKeyListOperation ( ListLogic operation, FormKey value ) : base(operation, value)
        {
        }

        public override FormKeyListOperation<TMajor> Inverse () => new(InverseOperation(), Value);

        public IFormLinkGetter<TMajor>? ToLinkGetter ()
        {
            if (formLinkGetter == null && !linked)
            {
                if (!Global.State.LinkCache.TryResolve<TMajor>(Value, out var link))
                    LogHelper.Log(LogLevel.Warning, 0xFF, $"Unable to find {Value} to link to.");

                formLinkGetter = link?.ToLinkGetter();
                linked = true;
            }

            return formLinkGetter;
        }
    }
}