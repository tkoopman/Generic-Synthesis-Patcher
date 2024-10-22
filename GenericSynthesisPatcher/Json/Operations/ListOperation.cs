using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Json.Converters;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Operations
{
    [JsonConverter(typeof(OperationsConverter))]
    public class ListOperation : ListOperation<string>
    {
        public Regex? Regex { get; private set; }

        public ListOperation ( string value ) : base(value)
        {
            if (Value.StartsWith('/') && Value.EndsWith('/'))
                Regex = new Regex(Value.Trim('/'), RegexOptions.IgnoreCase);
        }

        public ListOperation ( ListLogic operation, string value ) : base(operation, value)
        {
            if (Value.StartsWith('/') && Value.EndsWith('/'))
                Regex = new Regex(Value.Trim('/'), RegexOptions.IgnoreCase);
        }

        public override ListOperation Inverse () => new(InverseOperation(), Value);

        public bool MatchesValue ( string check )
        {
            if (Regex == null)
                Global.TraceLogger?.WriteLine($"String inspecting: {Value} Check: {check} IsMatch: {Value.Equals(check, StringComparison.OrdinalIgnoreCase)}");
            else
                Global.TraceLogger?.WriteLine($"Regex: {Regex?.ToString()} Check: {check} IsMatch: {Regex?.IsMatch(check)}");

            return Regex == null ? Value.Equals(check, StringComparison.OrdinalIgnoreCase) : Regex.IsMatch(check);
        }
    }

    [JsonConverter(typeof(OperationsConverter))]
    public class ListOperation<T> : ListOperationBase<ListOperation<T>, T> where T : IConvertible
    {
        public override ListLogic Operation { get; protected set; }
        public override T Value { get; protected set; }

        public ListOperation ( string value )
        {
            (Operation, string? v) = Split(value, ValidPrefixes);
            Value = (T)((IConvertible)v).ToType(typeof(T), null);
        }

        public ListOperation ( ListLogic operation, T value )
        {
            Operation = operation;
            Value = value;
        }

        public override ListOperation<T> Inverse () => new(InverseOperation(), Value);
    }
}