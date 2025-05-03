using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Data.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Skyrim.Action
{
    public class EffectsAction : FormLinksWithDataAction<EffectsData, IMagicEffectGetter, Effect>
    {
        public static readonly EffectsAction Instance = new();
        private const int ClassLogCode = 0x17;

        public override Effect? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not IEffectGetter sourceRecord)
            {
                Global.Logger.Log(ClassLogCode, $"Failed to add effect. No Effects?", logLevel: LogLevel.Error);
                return null;
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

            return effect;
        }

        public override bool DataEquals (IFormLinkContainerGetter left, IFormLinkContainerGetter right)
            => left is IEffectGetter l
            && right is IEffectGetter r
            && l.BaseEffect.FormKey.Equals(r.BaseEffect.FormKey)
            && (l.Data != null && r.Data != null
            && l.Data.Area == r.Data.Area
            && l.Data.Duration == r.Data.Duration
            && l.Data.Magnitude == r.Data.Magnitude
            || l.Data == null && r.Data == null);

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