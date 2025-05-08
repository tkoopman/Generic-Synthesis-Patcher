using System.Text.RegularExpressions;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Games.Universal.Json.Operations
{
    public class FormKeyListOperationAdvanced<TMajor> : FormKeyListOperation<TMajor> where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        public FormKeyListOperationAdvanced (string value) : base(value)
        {
        }

        private FormKeyListOperationAdvanced (ListLogic operation, FormKey value, Regex? regex = null) : base(operation, value) => Regex = regex;

        public Regex? Regex { get; protected set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals (FormKeyListOperationAdvanced<TMajor>? l, FormKeyListOperationAdvanced<TMajor>? r)
        {
            if (ReferenceEquals(l, r))
                return true;

            if (l == null || r == null)
                return false;

            return l.Regex == r.Regex && l.Value == r.Value && l.Operation == r.Operation;
        }

        public override FormKeyListOperationAdvanced<TMajor> Inverse () => new(getInverse(Operation), Value, Regex);

        // Not just creating link, but confirming record exists
        public override IFormLinkGetter<TMajor>? ToLinkGetter () => Regex == null ? base.ToLinkGetter() : throw new InvalidOperationException("Unable to link to RegEx values.");

        public override bool ValueEquals (FormKey other)
        {
            if (Regex == null)
                return Value.Equals(other);

            if (!other.ToLinkGetter<TMajor>().TryResolve(Global.State.LinkCache, out var link) || link.EditorID == null)
                return false;

            bool result = Regex.IsMatch(link.EditorID);

            if (Global.Settings.Value.Logging.NoisyLogs.RegexMatchFailed && !result || Global.Settings.Value.Logging.NoisyLogs.RegexMatchSuccessful && result)
                Global.TraceLogger?.WriteLine($"Regex: {Regex} Value: {link.EditorID} IsMatch: {result}");

            return result;
        }

        protected override FormKey convertValue (string? value)
        {
            if (value == null)
                return FormKey.Null;

            if (value.StartsWith('/') && value.EndsWith('/'))
            {
                Regex = new Regex(value.Trim('/'), RegexOptions.IgnoreCase);
                return FormKey.Null;
            }

            return base.convertValue(value);
        }
    }
}