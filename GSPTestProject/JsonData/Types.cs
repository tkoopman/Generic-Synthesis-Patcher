using Newtonsoft.Json;

namespace GSPTestProject.JsonData
{
    public readonly struct Types (string name, string id)
    {
        [JsonProperty(nameof(id), Order = 2)]
        public string ID { get; } = id;

        [JsonProperty(nameof(name), Order = 1)]
        public string Name { get; } = name;
    }
}