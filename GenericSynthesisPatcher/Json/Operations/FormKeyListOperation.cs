using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    public sealed class FormKeyListOperation<TMajor> : ListOperationBase<FormKeyListOperation<TMajor>, FormKey> where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        private IFormLinkGetter<TMajor>? formLinkGetter;
        private bool linked = false;

        public FormKeyListOperation ( string? value ) : base(value)
        {
        }

        public FormKeyListOperation ( ListLogic operation, FormKey value ) : base(operation, value)
        {
        }

        public IFormLinkGetter<TMajor>? ToLinkGetter ()
        {
            if (formLinkGetter == null && !linked)
            {
                if (!Global.State.LinkCache.TryResolve<TMajor>(Value, out var link))
                    Global.Logger.Log(0xFF, $"Unable to find {Value} to link to.", logLevel: LogLevel.Warning);

                formLinkGetter = link?.ToLinkGetter();
                linked = true;
            }

            return formLinkGetter;
        }

        protected override FormKey ConvertValue ( string? value )
        {
            if (value == null)
                return FormKey.Null;

            var _value = Mod.TryFindFormKey(value, out var formKey, out formLinkGetter) ? formKey
                  : throw new JsonSerializationException($"Unable to parse \"{value}\" into valid FormKey or EditorID");

            return _value;
        }
    }

    public class FormKeyListOperation : ListOperationBase<FormKeyListOperation, FormKey>
    {
        public FormKeyListOperation ( string? value ) : base(value)
        {
        }

        public FormKeyListOperation ( ListLogic operation, FormKey value ) : base(operation, value)
        {
        }

        protected override FormKey ConvertValue ( string? value )
            => value == null ? FormKey.Null
             : FormKey.TryFactory(Mod.FixFormKey(value), out var formKey) ? formKey
             : throw new JsonSerializationException($"Unable to parse \"{value}\" into valid FormKey");
    }
}