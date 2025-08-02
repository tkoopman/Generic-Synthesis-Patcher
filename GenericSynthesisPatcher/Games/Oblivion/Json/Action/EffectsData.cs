using System.ComponentModel;

using GenericSynthesisPatcher.Games.Universal.Json.Action;
using GenericSynthesisPatcher.Rules.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Oblivion.Json.Action
{
    public class EffectsData (FormKeyListOperation<IMagicEffectGetter> formKey, uint area, uint duration, uint magnitude) : FormLinksWithDataActionDataBase<IMagicEffectGetter, Effect>
    {
        private const int ClassLogCode = 0xC3;

        [JsonProperty(PropertyName = "Area", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public uint Area { get; set; } = area;

        [JsonProperty(PropertyName = "Duration", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public uint Duration { get; set; } = duration;

        [JsonProperty(PropertyName = "Effect", Required = Required.Always)]
        public override FormKeyListOperation<IMagicEffectGetter> FormKey { get; } = formKey;

        [JsonProperty(PropertyName = "Magnitude", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public uint Magnitude { get; set; } = magnitude;

        public override bool Equals (IFormLinkContainerGetter? other)
            => other is IEffectGetter effect
            && (effect.ScriptEffect?.Data?.VisualEffect.TryResolveFormKey(Global.Game.State.LinkCache, out var key) ?? false)
            && FormKey.ValueEquals(key)
            && effect.Data is not null
            && effect.Data.Area == Area
            && effect.Data.Duration == Duration
            && effect.Data.Magnitude == Magnitude;

        public bool Equals (EffectsData? other)
            => other is not null
            && Area == other.Area
            && Duration == other.Duration
            && Magnitude == other.Magnitude
            && FormKey == other.FormKey;

        public override bool Equals (object? obj) => Equals(obj as EffectsData);

        public override int GetHashCode () => HashCode.Combine(FormKey, Area, Duration, Magnitude);

        public override Effect ToActionData ()
        {
            var effect = new Effect();
            if (!Global.Game.State.LinkCache.TryResolve<IMagicEffect>(FormKey.Value, out var rec))
            {
                Global.Logger.WriteLog(LogLevel.Error, Helpers.LogType.RecordNotFound, $"Unable to find Magic Effect {FormKey.Value}", ClassLogCode);
                return null!;
            }

            var rt = new RecordType(rec.EditorID);
            effect.ScriptEffect ??= new ScriptEffect();
            effect.ScriptEffect.Data ??= new ScriptEffectData();
            effect.ScriptEffect.Data.VisualEffect.SetTo(rt);
            effect.Data = new EffectData
            {
                Area = Area,
                Duration = Duration,
                Magnitude = Magnitude
            };

            return effect;
        }

        public override string? ToString () => $"{FormKey.Value} (Area: {Area}, Duration: {Duration}, Magnitude: {Magnitude})";
    }
}