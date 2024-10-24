using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Json.Converters;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public sealed class ListOperation : ListOperation<string>
    {
        public Regex? Regex { get; private set; }

        public ListOperation ( string? value ) : base(value)
        {
        }

        public ListOperation ( ListLogic operation, string? value ) : base(operation, value)
        {
        }

        public override ListOperation Inverse () => new(Operation, Value);

        public bool MatchesValue ( string check )
        {
            if (Regex == null)
                Global.TraceLogger?.WriteLine($"String inspecting: {Value} Check: {check} IsMatch: {string.Equals(Value, check, StringComparison.OrdinalIgnoreCase)}");
            else
                Global.TraceLogger?.WriteLine($"Regex: {Regex?.ToString()} Check: {check} IsMatch: {Regex?.IsMatch(check)}");

            return Regex == null ? string.Equals(Value, check, StringComparison.OrdinalIgnoreCase) : Regex.IsMatch(check);
        }

        protected override void ValueUpdated ()
        {
            if (Value != null && Value.StartsWith('/') && Value.EndsWith('/'))
                Regex = new Regex(Value.Trim('/'), RegexOptions.IgnoreCase);
        }
    }

    [JsonConverter(typeof(OperationsConverter))]
    public class ListOperation<T> : ListOperationBase<ListOperation<T>, T> where T : IConvertible
    {
        public ListOperation ( string? value ) : base(value)
        {
        }

        public ListOperation ( ListLogic operation, T? value ) : base(operation, value)
        {
        }

        protected override T? ConvertValue ( string? value ) => value != null ? (T)((IConvertible)value).ToType(typeof(T), null) : default;
    }
}