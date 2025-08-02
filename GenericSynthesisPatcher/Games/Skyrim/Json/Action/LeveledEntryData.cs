using GenericSynthesisPatcher.Games.Universal.Json.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Games.Skyrim.Json.Action
{
    public abstract class LeveledEntryData<TMajor, TData> (short count, short level) : BaseLeveledEntryData<TMajor, TData>(count, level)
        where TMajor : class, IMajorRecordQueryableGetter, IMajorRecordGetter
        where TData : class, IFormLinkContainer
    {
        private const int ClassLogCode = 0xAA;

        [JsonProperty(PropertyName = "ItemCondition")]
        public float ItemCondition { get; set; } = -1;

        [JsonProperty(PropertyName = "Owner")]
        public OwnerBase? Owner { get; set; }

        protected ExtraData? createExtraData ()
        {
            if (Owner is null && ItemCondition == -1)
            {
                Global.Logger.WriteLog(LogLevel.Trace, Helpers.LogType.RecordProcessing, "No extra data to add", ClassLogCode);
                return null;
            }

            var extraData = new ExtraData
            {
                ItemCondition = ItemCondition,
                Owner = Owner?.ToActionData() ?? new NoOwner(),
            };

            return extraData;
        }
    }
}