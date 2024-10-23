using System.ComponentModel;

using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Json.Data;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class EffectsAction ( FormKeyListOperation<IMagicEffectGetter> formKey, int area, int duration, float magnitude ) : IFormLinksWithData<EffectsAction, IMagicEffectGetter>
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

        private const int ClassLogCode = 0x17;

        [JsonProperty(PropertyName = "Effect", Required = Required.Always)]
        public FormKeyListOperation<IMagicEffectGetter> FormKey { get; private set; } = formKey;

        FormKeyListOperation IFormLinksWithData<EffectsAction>.FormKey => FormKey;

        public static int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord, IFormLinkContainerGetter source )
        {
            if (source is not IEffectGetter sourceRecord)
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, $"Failed to add effect. No Effects?", context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            var effect = new Effect();
            effect.BaseEffect.FormKey = sourceRecord.BaseEffect.FormKey;
            if (sourceRecord.Data != null)
            {
                effect.Data = new EffectData
                {
                    Area = sourceRecord.Data.Area,
                    Duration = sourceRecord.Data.Duration,
                    Magnitude = sourceRecord.Data.Magnitude
                };
            }

            patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.GetPropertyForEditing<ExtendedList<Effect>>(patchRecord, rcd.PropertyName, out var items))
                return -1;

            items.Add(effect);
            Global.TraceLogger?.Log(ClassLogCode, $"Added effect {effect.BaseEffect.FormKey} ({effect.Data?.Area}, {effect.Data?.Duration}, {effect.Data?.Magnitude})");

            return 1;
        }

        public static bool CanMerge () => true;

        public static bool DataEquals ( IFormLinkContainerGetter left, IFormLinkContainerGetter right )
            => left is IEffectGetter l
            && right is IEffectGetter r
            && l.BaseEffect.FormKey.Equals(r.BaseEffect.FormKey)
            && ((l.Data != null && r.Data != null
            && l.Data.Area == r.Data.Area
            && l.Data.Duration == r.Data.Duration
            && l.Data.Magnitude == r.Data.Magnitude)
            || (l.Data == null && r.Data == null));

        public static IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list, FormKey key ) => list?.FirstOrDefault(s => s != null && GetFormKey(s).Equals(key), null);

        public static List<EffectsAction>? GetFillValueAs ( GSPRule rule, FilterOperation key ) => rule.GetFillValueAs<List<EffectsAction>>(key);

        public static FormKey GetFormKey ( IFormLinkContainerGetter from ) => from is IEffectGetter record ? record.BaseEffect.FormKey : throw new ArgumentNullException(nameof(from));

        public static int Merge ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, GSPRule rule, FilterOperation valueKey, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            _ = Global.UpdateTrace(ClassLogCode);

            var root = RecordGraph<IEffectGetter>.Create(
                context.Record.FormKey,
                context.Record.Registration.GetterType,
                rule.Merge[valueKey],
                list => Mod.GetProperty<IReadOnlyList<IEffectGetter>>(list.Record, rcd.PropertyName, out var value) ? value : null,
                item => $"{item.BaseEffect.FormKey} ({item.Data?.Area}, {item.Data?.Duration}, {item.Data?.Magnitude})");

            if (root == null)
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, "Failed to generate graph for merge", rule: rule, context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            return root.Merge(out var newList) ? Replace(context, rcd, ref patchRecord, newList) : 0;
        }

        public static int Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord, IFormLinkContainerGetter remove )
        {
            if (remove is not IEffectGetter sourceRecord)
                return -1;

            var effect = new Effect();
            effect.BaseEffect.FormKey = sourceRecord.BaseEffect.FormKey;
            if (sourceRecord.Data != null)
            {
                effect.Data = new EffectData
                {
                    Area = sourceRecord.Data.Area,
                    Duration = sourceRecord.Data.Duration,
                    Magnitude = sourceRecord.Data.Magnitude
                };
            }

            patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.GetPropertyForEditing<ExtendedList<Effect>>(patchRecord, rcd.PropertyName, out var items))
                return -1;

            if (items.Remove(effect))
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Removed effect {effect.BaseEffect.FormKey} ({effect.Data?.Area}, {effect.Data?.Duration}, {effect.Data?.Magnitude})");
                return 1;
            }

            LogHelper.Log(LogLevel.Error, ClassLogCode, $"Failed to remove effect [{sourceRecord.BaseEffect.FormKey}].", context: context, propertyName: rcd.PropertyName);
            return 0;
        }

        public static int Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord, IEnumerable<IFormLinkContainerGetter>? _newList )
        {
            if (_newList is not IReadOnlyList<IEffectGetter> newList || !Mod.GetProperty<IReadOnlyList<IEffectGetter>>(patchRecord ?? context.Record, rcd.PropertyName, out var curList))
            {
                LogHelper.Log(LogLevel.Error, ClassLogCode, "Failed to replace effects", context: context, propertyName: rcd.PropertyName);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.GetPropertyForEditing<ExtendedList<Effect>>(patchRecord, rcd.PropertyName, out _))
                return -1;

            foreach (var d in del)
                _ = Remove(context, rcd, ref patchRecord, d);

            foreach (var a in add)
                _ = Add(context, rcd, ref patchRecord, a);

            return add.Count() + del.Count();
        }

        public int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, RecordCallData rcd, ref ISkyrimMajorRecord? patchRecord )
        {
            var effect = new Effect();
            effect.BaseEffect.FormKey = FormKey.Value;
            effect.Data = new EffectData
            {
                Area = Area,
                Duration = Duration,
                Magnitude = Magnitude
            };

            patchRecord ??= context.GetOrAddAsOverride(Global.State.PatchMod);
            if (!Mod.GetPropertyForEditing<ExtendedList<Effect>>(patchRecord, rcd.PropertyName, out var effects))
                return -1;

            effects.Add(effect);
            return 1;
        }

        public bool DataEquals ( IFormLinkContainerGetter other )
            => other is IEffectGetter effect
            && effect.BaseEffect.FormKey.Equals(FormKey.Value)
            && effect.Data != null
            && effect.Data.Area == Area
            && effect.Data.Duration == Duration
            && effect.Data.Magnitude == Magnitude;

        public IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list ) => Find(list, FormKey.Value);
    }
}