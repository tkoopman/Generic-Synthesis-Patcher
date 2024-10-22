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
        public override ListLogic Operation { get; protected set; }

        public Regex? Regex { get; protected set; }
        public override FormKey Value { get; protected set; } = FormKey.Null;

        public FormKeyListOperationAdvanced ( string value )
        {
            (Operation, string? v) = Split(value, ValidPrefixes);

            if (v.StartsWith('/') && v.EndsWith('/'))
            {
                Regex = new Regex(v.Trim('/'), RegexOptions.IgnoreCase);
                return;
            }

            Value = FormKey.TryFactory(Mod.FixFormKey(v), out var formKey) ? formKey
                  : throw new JsonSerializationException($"Unable to parse \"{v}\" into valid FormKey");
        }

        public FormKeyListOperationAdvanced ( ListLogic operation, FormKey value, Regex? regex )
        {
            Operation = operation;
            Value = value;
            Regex = regex;
        }

        protected FormKeyListOperationAdvanced ()
        { }

        public override FormKeyListOperationAdvanced Inverse () => new(InverseOperation(), Value, Regex);
    }

    public class FormKeyListOperationAdvanced<TMajor> : FormKeyListOperationAdvanced where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        private IFormLinkGetter<TMajor>? formLinkGetter;
        private bool linked = false;

        public FormKeyListOperationAdvanced ( string value ) : base()
        {
            (Operation, string? v) = Split(value, ValidPrefixes);

            if (v.StartsWith('/') && v.EndsWith('/'))
            {
                Regex = new Regex(v.Trim('/'), RegexOptions.IgnoreCase);
                return;
            }

            Value = Mod.TryFindFormKey(v, out var formKey, out formLinkGetter) ? formKey
                  : throw new JsonSerializationException($"Unable to parse \"{v}\" into valid FormKey or EditorID");
        }

        private FormKeyListOperationAdvanced ( ListLogic operation, FormKey value, Regex? regex )
        {
            Operation = operation;
            Value = value;
            Regex = regex;
        }

        public override FormKeyListOperationAdvanced<TMajor> Inverse () => new(InverseOperation(), Value, Regex);

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