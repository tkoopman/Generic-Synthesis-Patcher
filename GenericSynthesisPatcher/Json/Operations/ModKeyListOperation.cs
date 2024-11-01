using Mutagen.Bethesda.Plugins;

using Noggog;

namespace GenericSynthesisPatcher.Json.Operations
{
    public class ModKeyListOperation : ListOperationBase<ModKey>
    {
        public ModKeyListOperation (string value) : base(value)
        {
        }

        public ModKeyListOperation (ListLogic operation, ModKey value) : base(operation, value)
        {
        }

        public override ModKeyListOperation Inverse () => (ModKeyListOperation)base.Inverse();

        public override bool ValueEquals (ModKey other) => Value.Equals(other);

        protected override ModKey ConvertValue (string? value) => value == null ? ModKey.Null : ModKey.FromFileName(new FileName(value));
    }
}