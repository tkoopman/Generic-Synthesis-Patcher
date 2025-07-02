using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Helpers;

using Loqui;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericSynthesisPatcher.Games.Universal.Json.Data
{
    public class GSPDeepCopyIn
    {
        /// <summary>
        ///     Set to copy values from a different record into records matching the GSP rule.
        ///     Unless <see cref="FromMod" /> used will select winning override of the record to copy.
        ///
        ///     FormKey provided must be a valid record of the same type as the record being
        ///     processed by the GSP rule.
        /// </summary>
        public FormKey FromID { get; set; } = FormKey.Null;

        /// <summary>
        ///     Set to select which mod(s) to copy from. If <see cref="FromID" /> also provided then
        ///     will copy that <see cref="FromID" /> from the specified mod. Without FormID set,
        ///     will copy values from current record if it has instance in the specified mod(s).
        ///
        ///     When multiple mods are specified, it will select mod based on <see cref="GSPRule.ForwardOptions" />
        /// </summary>
        [JsonConverter(typeof(SingleOrArrayConverter<ModKey>))]
        public List<ModKey> FromMod { get; set; } = [];

        /// <summary>
        ///     Translation mask data to use. The type of mask is based on the record that was
        ///     matched by the GSP rule. When mask is not provided and FormID not set then default
        ///     mask will be created with DefaultOn set to true. Otherwise default value will have
        ///     DefaultOn set to false, meaning it will not copy anything from a different record
        ///     unless you also specify the mask.
        /// </summary>
        [JsonProperty(PropertyName = "Mask")]
        public JObject? MaskData { get; set; }

        /// <summary>
        ///     Gets the mask for the given ITranslationMask type, if available.
        ///
        ///     If no mask data is provided, it will:
        ///     - If no FromID set, try and create a mask with DefaultOn set to true.
        ///     - If FromID is set, null as must provide a mask if trying to copy from another record.
        /// </summary>
        /// <param name="type">ITranslationMask type</param>
        /// <returns>Mask or null if invalid</returns>
        public MajorRecord.TranslationMask? GetMask (Type type)
        {
            var mask = MaskData is null
                     ? FromID != FormKey.Null ? null : TranslationMaskFactory.TryCreate(type, true, out var allMask) ? allMask : null
                     : JsonSerializer.Create(Global.Game.SerializerSettings).Deserialize(MaskData.CreateReader(), type) as ITranslationMask;

            return mask as MajorRecord.TranslationMask;
        }
    }
}