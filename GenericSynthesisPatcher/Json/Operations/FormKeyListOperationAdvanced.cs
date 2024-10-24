using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    public class FormKeyListOperationAdvanced : ListOperationBase<FormKeyListOperationAdvanced, FormKey>
    {
        public Regex? Regex { get; protected set; }

        public FormKeyListOperationAdvanced ( string value ) : base(value)
        {
        }

        public FormKeyListOperationAdvanced ( ListLogic operation, FormKey value, Regex? regex = null ) : base(operation, value) => Regex = regex;

        public override FormKeyListOperationAdvanced Inverse () => new(Inverse(Operation), Value, Regex);

        protected override FormKey ConvertValue ( string? value )
        {
            if (value == null)
                return FormKey.Null;

            if (value.StartsWith('/') && value.EndsWith('/'))
            {
                Regex = new Regex(value.Trim('/'), RegexOptions.IgnoreCase);
                return FormKey.Null;
            }

            return FormKey.TryFactory(Mod.FixFormKey(value), out var formKey) ? formKey
                   : throw new JsonSerializationException($"Unable to parse \"{value}\" into valid FormKey");
        }
    }

    public class FormKeyListOperationAdvanced<TMajor> : ListOperationBase<FormKeyListOperationAdvanced<TMajor>, FormKey> where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        private IFormLinkGetter<TMajor>? formLinkGetter;
        private bool linked = false;
        public Regex? Regex { get; protected set; }

        public FormKeyListOperationAdvanced ( string value ) : base(value)
        {
        }

        private FormKeyListOperationAdvanced ( ListLogic operation, FormKey value, Regex? regex = null ) : base(operation, value) => Regex = regex;

        public override FormKeyListOperationAdvanced<TMajor> Inverse () => new(Inverse(Operation), Value, Regex);

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

            if (value.StartsWith('/') && value.EndsWith('/'))
            {
                Regex = new Regex(value.Trim('/'), RegexOptions.IgnoreCase);
                return FormKey.Null;
            }

            return Mod.TryFindFormKey(value, out var formKey, out formLinkGetter) ? formKey
                 : throw new JsonSerializationException($"Unable to parse \"{value}\" into valid FormKey or EditorID");
        }
    }
}