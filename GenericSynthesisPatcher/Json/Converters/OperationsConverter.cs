using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Converters
{
    public partial class OperationsConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert ( Type objectType ) => objectType.GetType().IsAssignableTo(typeof(OperationBase<>));

        public override object? ReadJson ( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer )
        {
            string? key = reader.Value?.ToString();
            if (key == null)
                return null;

            var constructor = objectType.GetConstructor([typeof(string)]);
            if (constructor == null)
            {
                LogHelper.Log(LogLevel.Error, "Failed to construct new value form JSON.", 0xF00);
                return false;
            }

            return constructor.Invoke([key]);
        }

        public override void WriteJson ( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotImplementedException();
    }
}