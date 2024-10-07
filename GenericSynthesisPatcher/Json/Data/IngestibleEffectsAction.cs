using System.ComponentModel;
using System.Data;

using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

using static GenericSynthesisPatcher.Json.Data.GSPRule;
using static Mutagen.Bethesda.Skyrim.Furniture;

namespace GenericSynthesisPatcher.Json.Data
{
    public class IngestibleEffectsAction ( FilterFormLinks formKey, int area, int duration, float magnitude ) : IFormLinksWithData<IngestibleEffectsAction, IEffectGetter>, IFormLinksWithData<IngestibleEffectsAction, IFormLinkContainerGetter>
    {
        [JsonProperty(PropertyName = "area", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int Area = area;

        [JsonProperty(PropertyName = "duration", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int Duration = duration;

        [JsonProperty(PropertyName = "magnitude", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public float Magnitude = magnitude;

        [JsonProperty(PropertyName = "effect", Required = Required.Always)]
        public FilterFormLinks FormKey { get; set; } = formKey;

        public static bool Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IFormLinkContainerGetter source )
        {
            if (context.Record is not IIngestibleGetter || source is not IEffectGetter entry)
                return false;

            var effect = new Effect();
            effect.BaseEffect.FormKey = entry.BaseEffect.FormKey;
            if (entry.Data != null)
            {
                effect.Data = new EffectData
                {
                    Area = entry.Data.Area,
                    Duration = entry.Data.Duration,
                    Magnitude = entry.Data.Magnitude
                };
            }

            patch ??= (IIngestible)context.GetOrAddAsOverride(Global.State.PatchMod);

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IIngestible p)
                p.Effects?.Add(effect);

            return true;
        }

        public static FormKey GetFormKey ( IFormLinkContainerGetter from ) => from is IEffectGetter record ? record.BaseEffect.FormKey : throw new ArgumentNullException(nameof(from));

        public static List<IngestibleEffectsAction>? GetValueAs ( GSPRule rule, GSPRule.ValueKey key ) => rule.GetValueAs<List<IngestibleEffectsAction>>(key);

        public static bool Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IFormLinkContainerGetter remove )
        {
            if (!((patch == null || (patch is IIngestible)) && remove is IEffectGetter entry))
                return false;

            var effect = new Effect();
            effect.BaseEffect.FormKey = entry.BaseEffect.FormKey;
            if (entry.Data != null)
            {
                effect.Data = new EffectData
                {
                    Area = entry.Data.Area,
                    Duration = entry.Data.Duration,
                    Magnitude = entry.Data.Magnitude
                };
            }

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is not IIngestible p || (!p.Effects?.Remove(effect) ?? false))
                LogHelper.Log(Microsoft.Extensions.Logging.LogLevel.Error, context, $"Failed to remove effect [{entry.BaseEffect.FormKey}] from container.");

            return true;
        }

        public static bool Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IEnumerable<IFormLinkContainerGetter> newList )
        {
            if (context.Record is not IContainerGetter record || newList is not IReadOnlyList<IContainerEntryGetter> list)
                return false;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IIngestible p)
            {
                _ = p.Effects?.RemoveAll(_ => true);

                foreach (var add in list)
                    _ = Add(context, ref patch, add);
            }

            return true;
        }

        public bool Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch )
        {
            var effect = new Effect();
            effect.BaseEffect.FormKey = FormKey.FormKey;
            effect.Data = new EffectData
            {
                Area = Area,
                Duration = Duration,
                Magnitude = Magnitude
            };

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IIngestible p)
                p.Effects?.Add(effect);

            return true;
        }

        public bool DataEquals ( IFormLinkContainerGetter other )
            => other is IEffectGetter effect
            && effect.BaseEffect.Equals(FormKey.FormKey)
            && effect.Data != null
            && effect.Data.Area == Area
            && effect.Data.Duration == Duration
            && effect.Data.Magnitude == Magnitude;

        public IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list ) => list?.SingleOrDefault(s => (s != null) && GetFormKey(s).Equals(FormKey.FormKey), null);

        IEffectGetter? IFormLinksWithData<IngestibleEffectsAction, IEffectGetter>.Find ( IEnumerable<IFormLinkContainerGetter>? list ) => (IEffectGetter?)list?.SingleOrDefault(s => (s != null) && GetFormKey(s).Equals(FormKey.FormKey), null);
    }
}