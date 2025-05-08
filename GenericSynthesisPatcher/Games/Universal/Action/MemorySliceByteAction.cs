using System.Diagnostics.CodeAnalysis;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    public class MemorySliceByteAction : BasicGetterSetterAction<ReadOnlyMemorySlice<byte>, MemorySlice<byte>>
    {
        public static readonly MemorySliceByteAction Instance = new();

        // <inheritdoc />
        public override bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Memory slice in form of array of bytes";
            example = $$"""
                        "{{propertyName}}": [0x1A,0x00,0x3F]
                        """;

            return true;
        }

        protected override bool compareValues (ReadOnlyMemorySlice<byte> lhs, MemorySlice<byte> rhs) => lhs.SequenceEqual(rhs);

        protected override MemorySlice<byte> getSetter (ReadOnlyMemorySlice<byte> getter) => new([.. getter]);
    }
}