using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Games.Oblivion.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Games.Oblivion.Action
{
    public class EffectsAction : FormLinksWithDataAction<EffectsData, IMagicEffectGetter, Effect>
    {
        public static readonly EffectsAction Instance = new();
        private const int ClassLogCode = 0xC2;

        public override Effect? CreateFrom (IFormLinkContainerGetter source)
        {
            if (source is not IEffectGetter sourceRecord)
            {
                Global.Logger.WriteLog(LogLevel.Error, Helpers.LogType.RecordUpdateFailure, "Failed to add effect. No Effects?", ClassLogCode);
                return null;
            }

            var effect = sourceRecord.DeepCopy();

            return effect;
        }

        public override FormKey GetFormKeyFromRecord (IFormLinkContainerGetter from)
            => from is IEffectGetter record
            ? (record.Data.MagicEffect.TryResolveFormKey(Global.Game.State.LinkCache, out var key) ? key : throw new ArgumentNullException(nameof(from)))
            : throw new ArgumentNullException(nameof(from));

        public override string ToString (IFormLinkContainerGetter source)
            => source is IEffectGetter item
            ? $"{item.Data.MagicEffect} (Area: {item.Data.Area}, Duration: {item.Data.Duration}, Magnitude: {item.Data.Magnitude})"
            : throw new InvalidCastException();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "JSON objects containing effect Form Key/Editor ID and effect data";
            example = $$"""
                        "{{propertyName}}": { "Effect": "021FED:Oblivion.esm", "Area": 3, "Duration": 3, "Magnitude": 3 }
                        """;

            return true;
        }
    }
}