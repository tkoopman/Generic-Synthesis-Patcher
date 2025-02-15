using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class MemorySliceByteAction : BasicGetterSetterAction<ReadOnlyMemorySlice<byte>, MemorySlice<byte>>
    {
        public static readonly MemorySliceByteAction Instance = new();

        protected override bool CompareValues (ReadOnlyMemorySlice<byte> lhs, MemorySlice<byte> rhs) => lhs.SequenceEqual(rhs);

        protected override MemorySlice<byte> GetSetter (ReadOnlyMemorySlice<byte> getter) => new MemorySlice<byte>([.. getter]);
    }
}