using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Rules.Operations
{
    public class FormKeyListOperationAdvanced<TMajor> : FormKeyListOperation<TMajor> where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
    {
        private const int ClassLogCode = -1;

        // Required for OperationsConverter
        public FormKeyListOperationAdvanced (string value) : base(value) { }

        private FormKeyListOperationAdvanced (ListLogic operation, FormKey value, Regex? regex = null) : base(operation, value) => Regex = regex;

        public Regex? Regex { get; protected set; }

        public override bool Equals (object? obj) => Equals(obj as FormKeyListOperationAdvanced<TMajor>);

        public bool Equals (FormKeyListOperationAdvanced<TMajor>? other) => other is not null && Regex == Regex && Value == Value && Operation == other.Operation;

        public override int GetHashCode ()
        {
            var hash = new HashCode ();
            hash.Add(base.GetHashCode());
            hash.Add(Regex);
            return hash.ToHashCode();
        }

        public override FormKeyListOperationAdvanced<TMajor> Inverse () => new(getInverse(Operation), Value, Regex);

        // Not just creating link, but confirming record exists
        public override IFormLinkGetter<TMajor>? ToLinkGetter () => Regex is null ? base.ToLinkGetter() : throw new InvalidOperationException("Unable to link to RegEx values.");

        public override bool ValueEquals (FormKey other)
        {
            if (Regex is null)
                return Value.Equals(other);

            if (!other.ToLinkGetter<TMajor>().TryResolve(Global.Game.State.LinkCache, out var link) || link.EditorID is null)
                return false;

            bool result = Regex.IsMatch(link.EditorID);

            if (Global.Settings.Logging.NoisyLogs.MatchLogs.IncludeRegex)
                Global.Logger.WriteLog(LogLevel.Trace, result ? Helpers.LogType.MatchSuccess : Helpers.LogType.MatchFailure, $"Regex: {Regex} Value: {link.EditorID} IsMatch: {result}", ClassLogCode);

            return result;
        }

        protected override FormKey convertValue (string? value)
        {
            if (value is null)
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