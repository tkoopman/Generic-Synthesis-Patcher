using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    public abstract class FormLinksWithDataActionDataBase<TMajor, TData> : IEquatable<IFormLinkContainerGetter>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        public abstract FormKeyListOperation<TMajor> FormKey { get; }

        public abstract bool Equals (IFormLinkContainerGetter? other);

        public abstract TData ToActionData ();
    }
}