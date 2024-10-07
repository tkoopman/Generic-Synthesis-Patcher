using GenericSynthesisPatcher.Helpers.Action;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using static GenericSynthesisPatcher.Json.Data.GSPRule;

namespace GenericSynthesisPatcher.Json.Data
{
    public interface IFormLinksWithData<T, S>
        where T : class, IFormLinksWithData<T, S>
        where S : class, IFormLinkContainerGetter
    {
        public FilterFormLinks FormKey { get; }

        public abstract static bool Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IFormLinkContainerGetter source );

        public abstract static FormKey GetFormKey ( IFormLinkContainerGetter from );

        public abstract static List<T>? GetValueAs ( GSPRule rule, ValueKey key );

        public abstract static bool Remove ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IFormLinkContainerGetter remove );

        public abstract static bool Replace ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch, IEnumerable<IFormLinkContainerGetter> newList );

        public bool Add ( IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context, ref IMajorRecordGetter? patch );

        public bool DataEquals ( IFormLinkContainerGetter other );

        public S? Find ( IEnumerable<IFormLinkContainerGetter>? list );
    }
}