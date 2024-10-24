using Mutagen.Bethesda.Plugins;

using Noggog;

namespace GenericSynthesisPatcher.Json.Operations
{
    public class ModKeyListOperation : ListOperationBase<ModKeyListOperation, ModKey>
    {
        public ModKeyListOperation ( string value ) : base(value)
        {
        }

        public ModKeyListOperation ( ListLogic operation, ModKey value ) : base(operation, value)
        {
        }

        protected override ModKey ConvertValue ( string? value ) => value == null ? ModKey.Null : ModKey.FromFileName(new FileName(value));
    }
}