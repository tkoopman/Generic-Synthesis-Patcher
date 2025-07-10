using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Operations;
using GenericSynthesisPatcher.Helpers;
using GenericSynthesisPatcher.Helpers.Graph;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    /// <summary>
    ///     Action that handles form links pointing to a list of major records.
    /// </summary>
    public class FormLinksAction<TMajor> : IRecordAction
        where TMajor : class, IMajorRecordGetter
    {
        public static readonly FormLinksAction<TMajor> Instance = new();
        private const int ClassLogCode = 0x18;

        private FormLinksAction ()
        {
        }

        // <inheritdoc />
        public bool AllowSubProperties => false;

        // <inheritdoc />
        public bool CanFill () => true;

        // <inheritdoc />
        public bool CanForward () => true;

        // <inheritdoc />
        public bool CanForwardSelfOnly () => true;

        // <inheritdoc />
        public bool CanMatch () => true;

        // <inheritdoc />
        public bool CanMerge () => true;

        // <inheritdoc />
        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curList, ClassLogCode) || !proKeys.TryGetFillValueAs(out List<FormKeyListOperation<TMajor>>? links))
                    return -1;

                int changes = 0;
                foreach (var actionKey in links ?? [])
                {
                    if (actionKey.Value == FormKey.Null)
                    {
                        if (curList is not null && curList.Count > 0)
                        {
                            if (!Mod.ClearProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, ClassLogCode))
                                return -1;

                            curList = [];
                            changes++;
                        }

                        continue;
                    }

                    var curKey = curList?.SingleOrDefault(k => k?.FormKey.Equals(actionKey.Value) ?? false, null);

                    if ((curKey is not null && actionKey.Operation == ListLogic.DEL) || (curKey is null && actionKey.Operation == ListLogic.ADD))
                    {
                        if (!Mod.TryGetPropertyValueForEditing<List<IFormLinkGetter<TMajor>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var setList, ClassLogCode))
                            return -1;

                        if (!Global.Game.State.LinkCache.TryResolve(actionKey.Value, typeof(TMajor), out var link))
                        {
                            Global.Logger.WriteLog(LogLevel.Error, LogType.RecordActionInvalid, $"Unable to find {actionKey}", ClassLogCode);
                            continue;
                        }

                        changes++;
                        if (actionKey.Operation == ListLogic.DEL)
                            _ = setList.Remove(link.ToLinkGetter<TMajor>());
                        else
                            setList.Add(link.ToLinkGetter<TMajor>());
                    }
                }

                if (changes > 0)
                    Global.Logger.WriteLog(LogLevel.Debug, LogType.RecordUpdated, $"{changes} {LogWriter.RecordUpdatedChanges}", ClassLogCode);

                return changes;
            }

            Global.Logger.LogInvalidTypeFound("IFormLinkContainerGetter", proKeys.Record.GetType().Name, ClassLogCode);
            return -1;
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordGetter>? FindHPUIndex (ProcessingKeys proKeys, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? endNodes) => throw new NotImplementedException("ForwardOption HPU invalid on this field.");

        // <inheritdoc />
        public int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode))
                    return -1;

                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue, ClassLogCode))
                    return -1;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    Global.Logger.WriteLog(LogLevel.Trace, LogType.NoUpdateAlreadyMatches, LogWriter.PropertyIsEqual, ClassLogCode);
                    return 0;
                }

                if (!Mod.TryGetPropertyValueForEditing<ExtendedList<IFormLinkGetter<TMajor>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var patchValue, ClassLogCode))
                    return -1;

                int changes = patchValue.RemoveAll(_ => true);

                if (newValue is not null)
                {
                    patchValue.AddRange(newValue);
                    changes += newValue.Count;
                }

                if (changes > 0)
                    Global.Logger.WriteLog(LogLevel.Debug, LogType.RecordUpdated, $"{changes} {LogWriter.RecordUpdatedChanges}", ClassLogCode);

                return changes;
            }

            Global.Logger.LogInvalidTypeFound("IFormLinkContainerGetter", proKeys.Record.GetType().Name, ClassLogCode);

            return -1;
        }

        // <inheritdoc />
        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode))
                    return -1;

                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue, ClassLogCode))
                    return -1;

                if (!newValue.SafeAny())
                    return 0;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    Global.Logger.WriteLog(LogLevel.Trace, LogType.NoUpdateAlreadyMatches, LogWriter.PropertyIsEqual, ClassLogCode);
                    return 0;
                }

                ExtendedList<IFormLinkGetter<TMajor>>? patchValueLinks = null;
                int changes = 0;

                foreach (var item in newValue)
                {
                    if (item.FormKey.ModKey == forwardContext.ModKey)
                    {
                        if (curValue is null || !curValue.Contains(item))
                        {
                            if (patchValueLinks is null)
                            {
                                if (!Mod.TryGetPropertyValueForEditing<ExtendedList<IFormLinkGetter<TMajor>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var patchValue, ClassLogCode))
                                    return -1;

                                patchValueLinks = patchValue;
                            }

                            patchValueLinks.Add(item);
                            changes++;
                        }
                    }
                }

                if (changes > 0)
                    Global.Logger.WriteLog(LogLevel.Debug, LogType.RecordUpdated, $"{changes} {LogWriter.RecordUpdatedChanges}", ClassLogCode);

                return changes;
            }

            Global.Logger.LogInvalidTypeFound("IFormLinkContainerGetter", proKeys.Record.GetType().Name, ClassLogCode);

            return -1;
        }

        // <inheritdoc />
        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(recordContext.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode) || curValue is null || !curValue.Any();

        // <inheritdoc />
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(recordContext.Record, proKeys.Property.PropertyName, out var checkValue, ClassLogCode)
                && Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue, ClassLogCode)
                && FormLinksAction<TMajor>.recordsMatch(checkValue, originValue));

        // <inheritdoc />
        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        // <inheritdoc />
        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetMatchValueAs(out bool fromCache, out List<FormKeyListOperation<TMajor>>? matches))
                return false;

            if (!matches.SafeAny())
                return true;

            if (!fromCache && !MatchesHelper.Validate(proKeys.RuleKey.Operation, matches))
                throw new InvalidDataException("Json data for matches invalid");

            if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curLinks, ClassLogCode))
                return false; // Property must not exist for this record.

            return MatchesHelper.Matches(curLinks?.Select(l => l.FormKey), proKeys.RuleKey.Operation, matches);
        }

        // <inheritdoc />
        public int Merge (ProcessingKeys proKeys)
        {
            Global.Logger.UpdateDefaultCallingLocation(ClassLogCode);

            var root = RecordGraph<IFormLinkGetter<TMajor>>.Create(
                proKeys,
                record => Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(record, proKeys.Property.PropertyName, out var value, ClassLogCode) ? value : null,
                item => $"{item.FormKey}");

            return root is not null && root.Merge(out var newList) ? Replace(proKeys, newList) : 0;
        }

        // <inheritdoc />
        public int Replace (ProcessingKeys proKeys, IEnumerable<IFormLinkGetter<TMajor>>? inputList)
        {
            if (inputList is not IReadOnlyList<IFormLinkGetter<TMajor>> newList || !Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curList, ClassLogCode))
            {
                Global.Logger.WriteLog(LogLevel.Error, LogType.RecordUpdateFailure, "Failed to replace entries", ClassLogCode);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            try
            {
                if (!Mod.TryGetPropertyValueForEditing<ExtendedList<IFormLinkGetter<TMajor>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var list, ClassLogCode))
                    return -1;

                foreach (var d in del)
                    _ = list.Remove(d);

                foreach (var a in add)
                    list.Add(a);
            }
            catch (RecordException ex)
            {
                Global.Logger.WriteLog(LogLevel.Critical, LogType.RecordUpdateFailure, ex.Message, ClassLogCode);
                return -1;
            }

            return add.Count() + del.Count();
        }

        // <inheritdoc />
        public bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Form Keys or Editor IDs";
            example = $"""
                        "{propertyName}": [ "123:Skyrim.esm", "MyEditorID" ]
                        """;

            return true;
        }

        private static bool recordsMatch (IReadOnlyList<IFormLinkGetter<TMajor>>? left, IReadOnlyList<IFormLinkGetter<TMajor>>? right)
        {
            if (!left.SafeAny() && !right.SafeAny())
                return true;

            if (!left.SafeAny() || !right.SafeAny() || left.Count != right.Count)
                return false;

            foreach (var l in left)
            {
                if (!right.Any(r => l.FormKey.Equals(r.FormKey)))
                    return false;
            }

            return true;
        }
    }
}