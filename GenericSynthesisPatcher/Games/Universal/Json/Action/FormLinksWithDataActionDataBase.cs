using GenericSynthesisPatcher.Games.Universal.Json.Operations;

using Mutagen.Bethesda.Plugins.Records;

namespace GenericSynthesisPatcher.Games.Universal.Json.Action
{
    /// <summary>
    ///     JSON Action Data base. Used to implement JSON Data classes for field actions that
    ///     contain list of FormKeys with extra data.
    /// </summary>
    /// <typeparam name="TMajor">
    ///     The major record class the FormKey in the list points to
    /// </typeparam>
    /// <typeparam name="TData">The class of entries in the list</typeparam>
    public abstract class FormLinksWithDataActionDataBase<TMajor, TData> : IEquatable<IFormLinkContainerGetter>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        public abstract FormKeyListOperation<TMajor> FormKey { get; }

        public abstract bool Equals (IFormLinkContainerGetter? other);

        public abstract override bool Equals (object? obj);

        public abstract override int GetHashCode ();

        public abstract TData ToActionData ();
    }
}