using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Games.Skyrim.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Games.Skyrim.Action
{
    public class EffectsAction : FormLinksWithDataAction<EffectsData, IMagicEffectGetter, Effect>
    {
        public static readonly EffectsAction Instance = new();
        private const int ClassLogCode = 0xA2;

        public override Effect? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not IEffectGetter sourceRecord)
            {
                Global.Logger.WriteLog(LogLevel.Error, Helpers.LogType.RecordUpdateFailure, $"Failed to add effect. No Effects?", ClassLogCode);
                return null;
            }

            var effect = new Effect();
            effect.BaseEffect.FormKey = sourceRecord.BaseEffect.FormKey;
            if (sourceRecord.Data is not null)
            {
                effect.Data = new EffectData
                {
                    Area = sourceRecord.Data.Area,
                    Duration = sourceRecord.Data.Duration,
                    Magnitude = sourceRecord.Data.Magnitude
                };
            }

            return effect;
        }

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IEffectGetter l
            && right is IEffectGetter r
            && l.BaseEffect.FormKey.Equals(r.BaseEffect.FormKey)
            && ((l.Data is not null && r.Data is not null
                && l.Data.Area == r.Data.Area
                && l.Data.Duration == r.Data.Duration
                && l.Data.Magnitude == r.Data.Magnitude)
                || (l.Data is null && r.Data is null));

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from)
            => from is IEffectGetter record
            ? record.BaseEffect.FormKey
            : throw new ArgumentNullException(nameof(from));

        public override string ToString (IFormLinkContainerGetter source)
            => source is IEffectGetter item
            ? $"{item.BaseEffect.FormKey} (Area: {item.Data?.Area}, Duration: {item.Data?.Duration}, Magnitude: {item.Data?.Magnitude})"
            : throw new InvalidCastException();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "JSON objects containing effect Form Key/Editor ID and effect data";
            example = $$"""
                        "{{propertyName}}": { "Effect": "021FED:Skyrim.esm", "Area": 3, "Duration": 3, "Magnitude": 3 }
                        """;

            return true;
        }
    }
}