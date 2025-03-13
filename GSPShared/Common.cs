using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GSPShared
{
    public static partial class Common
    {
        /// <summary>
        ///     Adds 0 padding to String representation of a form key. Doesn't actually validate the
        ///     rest of the string.
        /// </summary>
        /// <param name="input">
        ///     String representation of a form key that may not be padded
        /// </param>
        /// <returns>
        ///     String representation of the form key with 0 padding added if required.
        /// </returns>
        public static string FixFormKey (string input) => RegexFormKey().Replace(input, m => m.Value.PadLeft(6, '0'));

        /// <summary>
        ///     Checks that string meets the requirements for a valid EditorID
        /// </summary>
        public static bool IsValidEditorID (this string? editorID) => !string.IsNullOrWhiteSpace(editorID) && ValidEditorID().IsMatch(editorID);

        /// <summary>
        ///     Attempts to convert a string to a FormKey or EditorID
        /// </summary>
        /// <param name="input">Input string that is either FormKey or EditorID</param>
        /// <param name="formKey">FormKey if return value is SkyrimIDType.FormKey</param>
        /// <param name="editorID">
        ///     Null if return value != SkyrimIDType.EditorID, else the EditorID as a string
        /// </param>
        /// <returns>
        ///     SkyrimIDType value depending on if input was FormKey or EditorID.
        ///     SkyrimIDType.Invalid if neither.
        /// </returns>
        public static SkyrimIDType TryConvertToSkyrimID (string input, out FormKey formKey, out string editorID)
        {
            editorID = null!;

            if (FormKey.TryFactory(FixFormKey(input), out formKey))
                return SkyrimIDType.FormKey;

            formKey = default;

            if (input.IsValidEditorID())
            {
                editorID = input;
                return SkyrimIDType.EditorID;
            }

            return SkyrimIDType.Invalid;
        }

        /// <summary>
        ///     Attempt to return the record identified by either FormKey or EditorID
        /// </summary>
        /// <typeparam name="T">Record Type</typeparam>
        /// <param name="id">String of FormKey or EditorID</param>
        /// <param name="linkCache">Used to search for record</param>
        /// <param name="record">The record if found</param>
        /// <returns>true if found else false</returns>
        public static bool TryGetRecord<T> (string id, ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, [NotNullWhen(true)] out T? record)
            where T : class, IMajorRecordQueryableGetter
        {
            record = null;

            return Common.TryConvertToSkyrimID(id, out var formKey, out string editorID) switch
            {
                SkyrimIDType.FormKey => linkCache.TryResolve<T>(formKey, out record),
                SkyrimIDType.EditorID => linkCache.TryResolve<T>(editorID, out record),
                _ => false,
            };
        }

        /// <summary>
        ///     Attempt to return the winning record context identified by either FormKey or
        ///     EditorID
        /// </summary>
        /// <typeparam name="T">Record Type</typeparam>
        /// <param name="id">String of FormKey or EditorID</param>
        /// <param name="linkCache">Used to search for record</param>
        /// <param name="record">The record if found</param>
        /// <returns>true if found else false</returns>
        public static bool TryGetRecordContext<TMajor, TMajorGetter> (string id, ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, [NotNullWhen(true)] out IModContext<ISkyrimMod, ISkyrimModGetter, TMajor, TMajorGetter>? record)
            where TMajor : class, IMajorRecordQueryable, TMajorGetter
            where TMajorGetter : class, IMajorRecordQueryableGetter
        {
            record = null;

            return Common.TryConvertToSkyrimID(id, out var formKey, out string editorID) switch
            {
                SkyrimIDType.FormKey => linkCache.TryResolveContext(formKey, out record),
                SkyrimIDType.EditorID => linkCache.TryResolveContext(editorID, out record),
                _ => false,
            };
        }

        [GeneratedRegex(@"^[0-9A-Fa-f]{1,6}")]
        private static partial Regex RegexFormKey ();

        [GeneratedRegex(@"^[a-zA-Z0-9_]+$")]
        private static partial Regex ValidEditorID ();
    }
}