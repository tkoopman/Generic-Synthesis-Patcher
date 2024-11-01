using System.Text.RegularExpressions;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Json.Operations
{
    public class FormKeyListOperationAdvanced<TMajor> : FormKeyListOperation<TMajor> where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        public Regex? Regex { get; protected set; }

        public FormKeyListOperationAdvanced (string value) : base(value)
        {
        }

        private FormKeyListOperationAdvanced (ListLogic operation, FormKey value, Regex? regex = null) : base(operation, value) => Regex = regex;

        public override FormKeyListOperationAdvanced<TMajor> Inverse () => new(Inverse(Operation), Value, Regex);

        // Not just creating link, but confirming record exists
        public override IFormLinkGetter<TMajor>? ToLinkGetter () => Regex == null ? base.ToLinkGetter() : throw new InvalidOperationException("Unable to link to RegEx values.");

        public override bool ValueEquals (FormKey other)
            => Regex == null
             ? Value.Equals(other)
             : other.ToLinkGetter<TMajor>().TryResolve(Global.State.LinkCache, out var link) && link.EditorID != null && Regex.IsMatch(link.EditorID);

        protected override FormKey ConvertValue (string? value)
        {
            if (value == null)
                return FormKey.Null;

            if (value.StartsWith('/') && value.EndsWith('/'))
            {
                Regex = new Regex(value.Trim('/'), RegexOptions.IgnoreCase);
                return FormKey.Null;
            }

            return base.ConvertValue(value);
        }
    }
}