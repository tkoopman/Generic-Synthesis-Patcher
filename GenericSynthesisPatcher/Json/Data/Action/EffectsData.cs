using System.ComponentModel;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public class EffectsData (FormKeyListOperation<IMagicEffectGetter> formKey, int area, int duration, float magnitude) : ActionDataBase<IMagicEffectGetter, Effect>, IEquatable<EffectsData>
    {
        [JsonProperty(PropertyName = "Area", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int Area = area;

        [JsonProperty(PropertyName = "Duration", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int Duration = duration;

        [JsonProperty(PropertyName = "Magnitude", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public float Magnitude = magnitude;

        [JsonProperty(PropertyName = "Effect", Required = Required.Always)]
        public override FormKeyListOperation<IMagicEffectGetter> FormKey { get; } = formKey ?? new(null);

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is IEffectGetter effect
            && effect.BaseEffect.FormKey.Equals(FormKey.Value)
            && effect.Data != null
            && effect.Data.Area == Area
            && effect.Data.Duration == Duration
            && effect.Data.Magnitude == Magnitude;

        public bool Equals (EffectsData? other) => other != null && Area == other.Area && Duration == other.Duration && Magnitude == other.Magnitude && FormKey == other.FormKey;

        public override bool Equals (object? obj) => Equals(obj as EffectsData);

        public override int GetHashCode () => HashCode.Combine(FormKey, Area, Duration, Magnitude);

        public override Effect ToActionData ()
        {
            var effect = new Effect();
            effect.BaseEffect.FormKey = FormKey.Value;
            effect.Data = new EffectData
            {
                Area = Area,
                Duration = Duration,
                Magnitude = Magnitude
            };

            return effect;
        }
    }
}