using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Universal.Json.Converters
{
    public partial class OperationsConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert (Type objectType) => objectType.GetType().IsAssignableTo(typeof(OperationBase<,>));

        public override object? ReadJson (JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            string? key = reader.Value?.ToString();

            var constructor = objectType.GetConstructor([typeof(string)]);
            if (constructor is null)
            {
                Global.Logger.Log(0xFF, "Failed to construct new value form JSON.", logLevel: LogLevel.Error);
                return false;
            }

            return constructor.Invoke([key]);
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}