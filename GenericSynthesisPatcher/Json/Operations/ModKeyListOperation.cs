using Mutagen.Bethesda.Plugins;

using Noggog;

namespace GenericSynthesisPatcher.Json.Operations
{
    public class ModKeyListOperation : ListOperationBase<ModKeyListOperation, ModKey>
    {
        public override ListLogic Operation { get; protected set; }

        public override ModKey Value { get; protected set; }

        public ModKeyListOperation ( string value )
        {
            (Operation, string? v) = Split(value, ValidPrefixes);

            Value = ModKey.FromFileName(new FileName(v));
        }

        public ModKeyListOperation ( ListLogic operation, ModKey value )
        {
            Operation = operation;
            Value = value;
        }

        public override ModKeyListOperation Inverse () => new(InverseOperation(), Value);
    }
}