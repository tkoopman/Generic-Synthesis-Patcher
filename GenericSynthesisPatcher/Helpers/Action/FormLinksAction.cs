using System.Diagnostics.CodeAnalysis;

using GenericSynthesisPatcher.Helpers.Graph;
using GenericSynthesisPatcher.Json.Operations;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Noggog;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class FormLinksAction<TMajor> : IRecordAction
        where TMajor : class, IMajorRecordGetter
    {
        public static readonly FormLinksAction<TMajor> Instance = new();
        private const int ClassLogCode = 0x14;

        private FormLinksAction ()
        {
        }

        public bool CanFill () => true;

        public bool CanForward () => true;

        public bool CanForwardSelfOnly () => true;

        public bool CanMatch () => true;

        public bool CanMerge () => true;

        public int Fill (ProcessingKeys proKeys)
        {
            if (proKeys.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curList) || !proKeys.TryGetFillValueAs(out List<FormKeyListOperation<TMajor>>? links))
                    return -1;

                int changes = 0;
                foreach (var actionKey in links ?? [])
                {
                    if (actionKey.Value == FormKey.Null)
                    {
                        if (curList != null && curList.Count > 0)
                        {
                            if (!Mod.ClearProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName))
                            {
                                Global.Logger.Log(ClassLogCode, LogHelper.MissingProperty, logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                                return -1;
                            }

                            curList = [];
                            changes++;
                        }

                        continue;
                    }

                    var curKey = curList?.SingleOrDefault(k => k?.FormKey.Equals(actionKey.Value) ?? false, null);

                    if ((curKey != null && actionKey.Operation == ListLogic.DEL) || (curKey == null && actionKey.Operation == ListLogic.ADD))
                    {
                        if (!Mod.TryGetPropertyForEditing<List<IFormLinkGetter<TMajor>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var setList))
                        {
                            Global.Logger.Log(ClassLogCode, LogHelper.MissingProperty, logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                            return -1;
                        }

                        if (!Global.State.LinkCache.TryResolve(actionKey.Value, typeof(TMajor), out var link))
                        {
                            Global.Logger.Log(ClassLogCode, $"Unable to find {actionKey}", logLevel: LogLevel.Warning, propertyName: proKeys.Property.PropertyName);
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
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: proKeys.Property.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);
            return -1;
        }

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods) => throw new NotImplementedException("ForwardOption HPU invalid on this field.");

        public int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter record)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                    return -1;

                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue))
                    return -1;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
                    return 0;
                }

                if (!Mod.TryGetPropertyForEditing<ExtendedList<IFormLinkGetter<TMajor>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var patchValue))
                {
                    Global.Logger.Log(ClassLogCode, "Patch has null value.", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                    return -1;
                }

                int changes = patchValue.RemoveAll(_ => true);

                if (newValue != null)
                {
                    patchValue.AddRange(newValue);
                    changes += newValue.Count;
                }

                if (changes > 0)
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: proKeys.Property.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);

            return -1;
        }

        public int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
        {
            if (proKeys.Record is IFormLinkContainerGetter)
            {
                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curValue))
                    return -1;

                if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue))
                    return -1;

                if (!newValue.SafeAny())
                    return 0;

                if (curValue.SequenceEqualNullable(newValue))
                {
                    Global.TraceLogger?.Log(ClassLogCode, LogHelper.PropertyIsEqual, propertyName: proKeys.Property.PropertyName);
                    return 0;
                }

                ExtendedList<IFormLinkGetter<TMajor>>? patchValueLinks = null;
                int changes = 0;

                foreach (var item in newValue)
                {
                    if (item.FormKey.ModKey == forwardContext.ModKey)
                    {
                        if (curValue == null || !curValue.Contains(item))
                        {
                            if (patchValueLinks == null)
                            {
                                if (!Mod.TryGetPropertyForEditing<ExtendedList<IFormLinkGetter<TMajor>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var patchValue))
                                    return -1;

                                patchValueLinks = patchValue;
                            }

                            patchValueLinks.Add(item);
                            changes++;
                        }
                    }
                }

                if (changes > 0)
                    Global.DebugLogger?.Log(ClassLogCode, $"{changes} change(s).", propertyName: proKeys.Property.PropertyName);

                return changes;
            }

            Global.DebugLogger?.LogInvalidTypeFound(ClassLogCode, proKeys.Property.PropertyName, "IFormLinkContainerGetter", proKeys.Record.GetType().Name);

            return -1;
        }

        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || curValue is null || !curValue.Any();

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if all form keys match</returns>
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(recordContext.Record, proKeys.Property.PropertyName, out var checkValue)
                && Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue)
                && FormLinksAction<TMajor>.recordsMatch(checkValue, originValue));

        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        public bool MatchesRule (ProcessingKeys proKeys)
        {
            if (!proKeys.TryGetMatchValueAs(out bool fromCache, out List<FormKeyListOperation<TMajor>>? matches))
                return false;

            if (!matches.SafeAny())
                return true;

            if (!fromCache && !MatchesHelper.Validate(proKeys.RuleKey.Operation, matches))
                throw new InvalidDataException("Json data for matches invalid");

            if (!Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curLinks))
                return false; // Property must not exist for this record.

            return MatchesHelper.Matches(curLinks?.Select(l => l.FormKey), proKeys.RuleKey.Operation, matches, propertyName: proKeys.Property.PropertyName);
        }

        public int Merge (ProcessingKeys proKeys)
        {
            Global.UpdateLoggers(ClassLogCode);

            var root = RecordGraph<IFormLinkGetter<TMajor>>.Create(
                proKeys,
                record => Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(record, proKeys.Property.PropertyName, out var value) ? value : null,
                item => $"{item.FormKey}");

            return root != null && root.Merge(out var newList) ? Replace(proKeys, newList) : 0;
        }

        public int Replace (ProcessingKeys proKeys, IEnumerable<IFormLinkGetter<TMajor>>? inputList)
        {
            if (inputList is not IReadOnlyList<IFormLinkGetter<TMajor>> newList || !Mod.TryGetProperty<IReadOnlyList<IFormLinkGetter<TMajor>>>(proKeys.Record, proKeys.Property.PropertyName, out var curList))
            {
                Global.Logger.Log(ClassLogCode, "Failed to replace entries", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            var add = newList.WhereNotIn(curList);
            var del = curList.WhereNotIn(newList);

            if (!add.Any() && !del.Any())
                return 0;

            try
            {
                if (!Mod.TryGetPropertyForEditing<ExtendedList<IFormLinkGetter<TMajor>>>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var list))
                    return -1;

                foreach (var d in del)
                    _ = list.Remove(d);

                foreach (var a in add)
                    list.Add(a);
            }
            catch (RecordException ex)
            {
                Global.Logger.Log(ClassLogCode, ex.Message, logLevel: LogLevel.Critical, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            return add.Count() + del.Count();
        }

        // <inheritdoc />
        public bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "Form Keys or Editor IDs";
            example = "";

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