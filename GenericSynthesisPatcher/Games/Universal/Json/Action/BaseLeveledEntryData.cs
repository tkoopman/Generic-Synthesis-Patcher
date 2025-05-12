using System.ComponentModel;

using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Universal.Json.Action
{
    public abstract class BaseLeveledEntryData<TMajor, TData> (short count, short level) : FormLinksWithDataActionDataBase<TMajor, TData>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        [JsonProperty(PropertyName = "Count", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public short Count { get; set; } = count;

        [JsonProperty(PropertyName = "Level", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public short Level { get; } = level;

        public override string? ToString () => $"{Count}x{FormKey.Value}";
    }
}