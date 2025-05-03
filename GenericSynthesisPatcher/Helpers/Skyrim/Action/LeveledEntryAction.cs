using GenericSynthesisPatcher.Helpers.Action;
using GenericSynthesisPatcher.Json.Data.Action;

using Loqui;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Skyrim.Action
{
    public abstract class LeveledEntryAction<TActionData, TMajor, TData> : FormLinksWithDataAction<TActionData, TMajor, TData>
        where TActionData : FormLinksWithDataActionDataBase<TMajor, TData>
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        private const int ClassLogCode = 0x18;

        protected ExtraData? createExtraData (ILoquiObject source)
        {
            if (!Mod.TryGetProperty<IExtraDataGetter>(source, "ExtraData", out var sourceData) || sourceData == null)
            {
                Global.TraceLogger?.Log(ClassLogCode, $"No extra data to copy", logLevel: LogLevel.Error);
                return null;
            }

            var extraData = new ExtraData
            {
                ItemCondition = sourceData.ItemCondition
            };

            switch (sourceData.Owner)
            {
                case INpcOwnerGetter owner:
                    var npcOwner = new Mutagen.Bethesda.Skyrim.NpcOwner
                    {
                        Npc = owner.Npc.FormKey.ToLink<INpcGetter>(),
                        Global = owner.Global.FormKey.ToLink<IGlobalGetter>()
                    };
                    extraData.Owner = npcOwner;
                    break;

                case IFactionOwnerGetter owner:
                    var factionOwner = new Mutagen.Bethesda.Skyrim.FactionOwner
                    {
                        Faction = owner.Faction.FormKey.ToLink<IFactionGetter>(),
                        RequiredRank = owner.RequiredRank
                    };
                    extraData.Owner = factionOwner;
                    break;

                default:
                    extraData.Owner = new NoOwner();
                    break;
            }

            return extraData;
        }
    }
}