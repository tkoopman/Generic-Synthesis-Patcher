using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    public class FormKeyListOperation : ListOperationBase<FormKey>
    {
        public override ListLogic Operation { get; protected set; }

        public override FormKey Value { get; protected set; }

        public FormKeyListOperation ( string value )
        {
            (Operation, string? v) = Split(value, ValidPrefixes);

            Value = FormKey.TryFactory(Mod.FixFormKey(v), out var formKey) ? formKey
                  : throw new JsonSerializationException($"Unable to parse \"{v}\" into valid FormKey");
        }

        protected FormKeyListOperation ()
        { }
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

        public IFormLinkGetter<TMajor>? ToLinkGetter ()
        {
            if (formLinkGetter == null && !linked)
            {
                if (!Global.State.LinkCache.TryResolve<TMajor>(Value, out var link))
                    LogHelper.Log(LogLevel.Warning, $"Unable to find {Value} to link to.", 0xF12);

                formLinkGetter = link?.ToLinkGetter();
                linked = true;
            }

            return formLinkGetter;
        }
    }
}