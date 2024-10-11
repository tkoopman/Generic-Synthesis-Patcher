using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Json.Data
{
    public interface IFormLinksWithData<T>
        where T : class, IFormLinksWithData<T>
    {
        public FilterFormLinks FormKey { get; }

        public abstract static int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter source );

        public abstract static bool DataEquals ( IFormLinkContainerGetter left, IFormLinkContainerGetter right );

        public abstract static IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list, FormKey key );

        public abstract static FormKey GetFormKey ( IFormLinkContainerGetter from );

        public abstract static List<T>? GetValueAs ( GSPRule rule, ValueKey key );

        public abstract static int Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IFormLinkContainerGetter remove );

        public abstract static int Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch, IEnumerable<IFormLinkContainerGetter> newList );

        public int Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref ISkyrimMajorRecord? patch );

        public bool DataEquals ( IFormLinkContainerGetter other );

        public IFormLinkContainerGetter? Find ( IEnumerable<IFormLinkContainerGetter>? list );
    }
}