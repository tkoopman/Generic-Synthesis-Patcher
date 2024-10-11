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
    public class IngestibleEffectsAction ( FilterFormLinks formKey, int area, int duration, float magnitude ) : IFormLinksWithData<IngestibleEffectsAction>
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

        private const int ClassLogPrefix = 0xB00;

        [JsonProperty(PropertyName = "effect", Required = Required.Always)]
        public FilterFormLinks FormKey { get; set; } = formKey;

        public static int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter source )
        {
            if (context.Record is not IIngestibleGetter || source is not IEffectGetter entry)
                return -1;

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
            {
                p.Effects?.Add(effect);
                return 1;
            }

            return -1;
        }

        public static bool DataEquals ( IFormLinkContainerGetter left, IFormLinkContainerGetter right )
            => left is IEffectGetter l
            && right is IEffectGetter r
            && l.BaseEffect.FormKey.Equals(r.BaseEffect.FormKey)
            && ((l.Data != null && r.Data != null
            && l.Data.Area == r.Data.Area
            && l.Data.Duration == r.Data.Duration
            && l.Data.Magnitude == r.Data.Magnitude)
            || (l.Data == null && r.Data == null));

        public static IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list, FormKey key ) => list?.SingleOrDefault(s => (s != null) && GetFormKey(s).Equals(key), null);

        public static FormKey GetFormKey ( IFormLinkContainerGetter from ) => from is IEffectGetter record ? record.BaseEffect.FormKey : throw new ArgumentNullException(nameof(from));

        public static List<IngestibleEffectsAction>? GetValueAs ( GSPRule rule, GSPRule.ValueKey key ) => rule.GetValueAs<List<IngestibleEffectsAction>>(key);

        public static int Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter remove )
        {
            if (!((patch == null || (patch is IIngestible)) && remove is IEffectGetter entry))
                return -1;

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
            {
                LogHelper.Log(Microsoft.Extensions.Logging.LogLevel.Error, context, $"Failed to remove effect [{entry.BaseEffect.FormKey}] from container.", ClassLogPrefix | 0X11);
                return -1;
            }

            return 1;
        }

        public static int Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IEnumerable<IFormLinkContainerGetter> newList )
        {
            if (context.Record is not IContainerGetter record || newList is not IReadOnlyList<IContainerEntryGetter> list)
                return -1;

            patch ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (patch is IIngestible p)
            {
                int changes = p.Effects?.RemoveAll(_ => true) ?? 0;

                foreach (var add in list)
                {
                    _ = Add(context, ref patch, add);
                    changes++;
                }
            }

            return -1;
        }

        public int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch )
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
            {
                p.Effects?.Add(effect);
                return 1;
            }

            return -1;
        }

        public bool DataEquals ( IFormLinkContainerGetter other )
            => other is IEffectGetter effect
            && effect.BaseEffect.FormKey.Equals(FormKey.FormKey)
            && effect.Data != null
            && effect.Data.Area == Area
            && effect.Data.Duration == Duration
            && effect.Data.Magnitude == Magnitude;

        public IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list ) => Find(list, FormKey.FormKey);
    }
}