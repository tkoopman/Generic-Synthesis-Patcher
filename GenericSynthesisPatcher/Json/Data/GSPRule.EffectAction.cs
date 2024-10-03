using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json;

using System.ComponentModel;

namespace GenericSynthesisPatcher.Json.Data
{
    public partial class GSPRule
    {
        public class EffectAction ( FormKey effect, bool remove, int area, int duration, float magnitude )
        {
            [JsonProperty(PropertyName = "remove", DefaultValueHandling = DefaultValueHandling.Populate)]
            [DefaultValue(false)]
            public bool Remove = remove;

            [JsonProperty(PropertyName = "effect", Required = Required.Always)]
            public FormKey Effect = effect;

            [JsonProperty(PropertyName = "area", DefaultValueHandling = DefaultValueHandling.Populate)]
            [DefaultValue(0)]
            public int Area = area;

            [JsonProperty(PropertyName = "duration", DefaultValueHandling = DefaultValueHandling.Populate)]
            [DefaultValue(0)]
            public int Duration = duration;

            [JsonProperty(PropertyName = "magnitude", DefaultValueHandling = DefaultValueHandling.Populate)]
            [DefaultValue(0)]
            public float Magnitude = magnitude;
        }
        internal class NotEffectAction ( FormKey effect, bool remove, int area, int duration, float magnitude ) : EffectAction(effect, remove, area, duration, magnitude)
        {
        }
    }
}