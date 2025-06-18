using Mutagen.Bethesda.Plugins;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Json.Operations
{
    public class ModKeyListOperation : ListOperationBase<ModKey>
    {
        // Required for OperationsConverter
        public ModKeyListOperation (string value) : base(value) { }

        public ModKeyListOperation (ListLogic operation, ModKey value) : base(operation, value)
        {
        }

        public override ModKeyListOperation Inverse () => (ModKeyListOperation)base.Inverse();

        public override bool ValueEquals (ModKey other) => Value.Equals(other);

        protected override ModKey convertValue (string? value) => value is null ? ModKey.Null : ModKey.FromFileName(new FileName(value));
    }
}