using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Skyrim.Action
{
    public class ModelAction : BasicGetterSetterAction<IModelGetter, Model>
    {
        public static readonly ModelAction Instance = new();

        public override bool CanFill () => false;

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Forward Model data.";
            example = $$"""[{ "types": ["Book"], "Forward": { "SomeMod.esp": ["{{propertyName}}"] } }]""";

            return true;
        }

        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        protected override bool compareValues (IModelGetter? lhs, Model? rhs)
        {
            if (lhs == null && rhs == null)
                return true;

            if (lhs == null || rhs == null)
                return false;

            return
                lhs.File.Equals(rhs.File) &&
                lhs.Data.SequenceEqual(rhs.Data) &&
                lhs.AlternateTextures.SequenceEqualNullable(rhs.AlternateTextures);
        }

        protected override Model? getSetter (IModelGetter? getter)
            => getter == null
            ? null
            : new Model
            {
                AlternateTextures = getAlternateTextures(getter.AlternateTextures),
                Data = getter.Data is null ? null : new([.. getter.Data]),
                File = new(getter.File)
            };

        private static ExtendedList<AlternateTexture>? getAlternateTextures (IReadOnlyList<IAlternateTextureGetter>? alternateTextures)
        {
            if (alternateTextures is null)
                return null;

            var result = new ExtendedList<AlternateTexture>();

            foreach (var t in alternateTextures)
            {
                var texture = new AlternateTexture
                {
                    Index = t.Index,
                    Name = t.Name,
                };
                texture.NewTexture.SetTo(t.NewTexture.FormKey);
                result.Add(texture);
            }

            return result;
        }
    }
}