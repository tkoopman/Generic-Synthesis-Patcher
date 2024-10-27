using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Json.Data.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class EffectsAction : FormLinksWithData<EffectsData, IMagicEffectGetter, Effect>
    {
        public static readonly EffectsAction Instance = new();
        private const int ClassLogCode = 0x17;

        public override int Add (ProcessingKeys proKeys, EffectsData data)
        {
            if (!Mod.GetPropertyForEditing<ExtendedList<Effect>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var effects))
                return -1;

            effects.Add(data.ToActionData());
            return 1;
        }

        public override bool CanMerge () => true;

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IEffectGetter l
            && right is IEffectGetter r
            && l.BaseEffect.FormKey.Equals(r.BaseEffect.FormKey)
            && ((l.Data != null && r.Data != null
            && l.Data.Area == r.Data.Area
            && l.Data.Duration == r.Data.Duration
            && l.Data.Magnitude == r.Data.Magnitude)
            || (l.Data == null && r.Data == null));

        public override IFormLinkContainerGetter? FindRecord (IEnumerable<IFormLinkContainerGetter>? list, FormKey key) => list?.FirstOrDefault(s => s != null && GetFormKeyFromRecord(s).Equals(key), null);

        public override int Forward (ProcessingKeys proKeys, IFormLinkContainerGetter source)
        {
            if (source is not IEffectGetter sourceRecord)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to add effect. No Effects?", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
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

            if (!Mod.GetPropertyForEditing<ExtendedList<Effect>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var items))
                return -1;

            items.Add(effect);
            Global.TraceLogger?.Log(ClassLogCode, $"Added effect {effect.BaseEffect.FormKey} ({effect.Data?.Area}, {effect.Data?.Duration}, {effect.Data?.Magnitude})");

            return 1;
        }

        public override List<EffectsData>? GetFillValueAs (ProcessingKeys proKeys) => proKeys.GetFillValueAs<List<EffectsData>>();

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from) => from is IEffectGetter record ? record.BaseEffect.FormKey : throw new ArgumentNullException(nameof(from));

        public override int Merge (ProcessingKeys proKeys)
        {
            Global.UpdateLoggers(ClassLogCode);

            var root = RecordGraph<IEffectGetter>.Create(
                proKeys.Record.FormKey,
                proKeys.Type.StaticRegistration.GetterType,
                proKeys.Rule.Merge[proKeys.RuleKey],
                list => Mod.GetProperty<IReadOnlyList<IEffectGetter>>(list.Record, proKeys.Property.PropertyName, out var value) ? value : null,
                item => $"{item.BaseEffect.FormKey} ({item.Data?.Area}, {item.Data?.Duration}, {item.Data?.Magnitude})");

            if (root == null)
            {
                Global.Logger.Log(ClassLogCode, "Failed to generate graph for merge", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            return root.Merge(out var newList) ? Replace(proKeys, newList) : 0;
        }

        public override int Remove (ProcessingKeys proKeys, IFormLinkContainerGetter remove)
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

            if (!Mod.GetPropertyForEditing<ExtendedList<Effect>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var items))
                return -1;

            if (items.Remove(effect))
            {
                Global.TraceLogger?.Log(ClassLogCode, $"Removed effect {effect.BaseEffect.FormKey} ({effect.Data?.Area}, {effect.Data?.Duration}, {effect.Data?.Magnitude})");
                return 1;
            }

            Global.Logger.Log(ClassLogCode, $"Failed to remove effect [{sourceRecord.BaseEffect.FormKey}].", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
            return 0;
        }

        public override int Replace (ProcessingKeys proKeys, IEnumerable<IFormLinkContainerGetter>? _newList)
        {
            if (_newList is not IReadOnlyList<IEffectGetter> newList || !Mod.GetProperty<IReadOnlyList<IEffectGetter>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
            {
                Global.Logger.Log(ClassLogCode, "Failed to replace effects", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            if (!Mod.GetPropertyForEditing<ExtendedList<Effect>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out _))
                return -1;

            foreach (var d in del)
                _ = Remove(proKeys, d);

            foreach (var a in add)
                _ = Forward(proKeys, a);

            return add.Count() + del.Count();
        }
    }
}