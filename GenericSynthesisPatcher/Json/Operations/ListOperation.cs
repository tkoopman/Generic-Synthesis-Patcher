using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public sealed class ListOperation : ListOperation<string>
    {
        public ListOperation (string? value) : base(value)
        {
        }

        public ListOperation (ListLogic operation, string? value) : base(operation, value)
        {
        }

        public Regex? Regex { get; private set; }

        public override ListOperation Inverse () => new(Operation, Value);

        public override bool ValueEquals (string? check)
        {
            if (Value == null && check == null)
                return true;

            if (Value == null || check == null)
                return false;

            if (Regex == null)
                return string.Equals(Value, check, StringComparison.OrdinalIgnoreCase);

            bool result = Regex.IsMatch(check);

            if ((Global.Settings.Value.Logging.NoisyLogs.RegexMatchFailed && !result) || (Global.Settings.Value.Logging.NoisyLogs.RegexMatchSuccessful && result))
                Global.TraceLogger?.WriteLine($"Regex: {Regex} Value: {check} IsMatch: {result}");

            return result;
        }

        protected override void valueUpdated ()
        {
            if (Value != null && Value.StartsWith('/') && Value.EndsWith('/'))
                Regex = new Regex(Value.Trim('/'), RegexOptions.IgnoreCase);
        }
    }

    [JsonConverter(typeof(OperationsConverter))]
    public class ListOperation<T> : ListOperationBase<T> where T : IConvertible
    {
        public ListOperation (string? value) : base(value)
        {
        }

        public ListOperation (ListLogic operation, T? value) : base(operation, value)
        {
        }

        protected override T? convertValue (string? value) => value != null ? (T)((IConvertible)value).ToType(typeof(T), null) : default;
    }
}