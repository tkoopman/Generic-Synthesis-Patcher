using Newtonsoft.Json;

namespace GSPTestProject.JsonData
{
    public class Property (string name, string aliases, PropertyFlags flags, string description, string example)
    {
        public string Aliases { get; } = aliases;
        public string Description { get; } = description;
        public string Example { get; } = example;

        [JsonIgnore]
        public PropertyFlags Flags { get; } = flags;

        [JsonProperty(nameof(Flags))]
        public int JsonFlags => (int)Flags;

        public string Name { get; } = name;
    }
}