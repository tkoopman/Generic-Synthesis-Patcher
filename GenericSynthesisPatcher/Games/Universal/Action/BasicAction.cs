using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using Common;

using GenericSynthesisPatcher.Games.Universal.Json.Data;
using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    public class BasicAction<T> : IRecordAction
    {
        public static readonly BasicAction<T> Instance = new();
        private const int ClassLogCode = 0x1B;

        protected BasicAction ()
        {
        }

        public virtual bool CanFill () => true;

        public virtual bool CanForward () => true;

        public virtual bool CanForwardSelfOnly () => false;

        public virtual bool CanMatch () => false;

        public virtual bool CanMerge () => false;

        public virtual int Fill (ProcessingKeys proKeys)
                    => !Mod.TryGetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                    || !proKeys.TryGetFillValueAs(out T? newValue)
                    ? -1
                    : performFill(proKeys, curValue, newValue);

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods)
        {
            bool nonNull = proKeys.Rule.HasForwardType(ForwardOptions._nonNullMod);
            List<T?> history = [];
            int hpu = -1;
            int hpuHistory = -1;

            foreach (int i in indexes.Reverse())
            {
                var mc = AllRecordMods[mods.ElementAt(i)];

                if (Mod.TryGetProperty<T>(mc.Record, proKeys.Property.PropertyName, out var curValue)
                    && (!nonNull || !Mod.IsNullOrEmpty(curValue)))
                {
                    int historyIndex = history.IndexOf(curValue);
                    if (historyIndex == -1)
                    {
                        historyIndex = history.Count;
                        history.Add(curValue);
                        Global.TraceLogger?.Log(ClassLogCode, $"Added value from {mc.ModKey} to history", propertyName: proKeys.Property.PropertyName);
                    }

                    if (validMods is null || validMods.Contains(mc.ModKey))
                    {
                        // If this a valid mod to be selected then check when it's value was added
                        // to history and if higher or equal we found new HPU.
                        if (hpuHistory <= historyIndex)
                        {
                            hpu = i;
                            hpuHistory = historyIndex;
                            Global.TraceLogger?.Log(ClassLogCode, $"Updated HPU value to {mc.ModKey} with index of {i} and history index of {historyIndex}", propertyName: proKeys.Property.PropertyName);
                        }
                    }
                }
            }

            return hpu;
        }

        public virtual int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
            => Mod.TryGetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
             && Mod.TryGetProperty<T>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)
             ? performFill(proKeys, curValue, newValue)
             : -1;

        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual string GetDocumentationDescription (Type propertyType)
            => typeof(T).Equals(typeof(Percent)) ? "Decimal value between 0.00 - 1.00, or string ending in %"
             : typeof(T).Equals(typeof(Color)) ? """Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted."""
             : throw new NotImplementedException();

        // <inheritdoc />
        public virtual string GetDocumentationExample (Type propertyType, string propertyName)
            => typeof(T).Equals(typeof(Percent)) ? $""" "{propertyName}": "30.5%" """
             : typeof(T).Equals(typeof(Color)) ? $""" "{propertyName}": [40,50,60] """
             : throw new NotImplementedException();

        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty<T>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || Mod.IsNullOrEmpty(curValue);

        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if value matches</returns>
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || Mod.TryGetProperty<T>(recordContext.Record, proKeys.Property.PropertyName, out var curValue)
            && Mod.TryGetProperty<T>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue)
            && Equals(curValue, originValue);

        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        public virtual int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = null;
            example = null;

            if (typeof(T).Equals(typeof(Percent)))
            {
                description = "Decimal value between 0.00 - 1.00, or string ending in %";
                example = $"""
                           "{propertyName}": "30.5%"
                           """;
            }
            else if (typeof(T).Equals(typeof(Color)))
            {
                description = """Color value as number, array of 3 or 4 numbers, or a string in the format of either Hex value ("#0A0A0A0A") or named color "Blue". Array and Hex are ARGB. Alpha portion of ARGB may be ignored for some fields and can be omitted.""";
                example = $"""
                           "{propertyName}": [40,50,60]
                           """;
            }

            return description is not null && example is not null;
        }

        protected virtual int performFill (ProcessingKeys proKeys, T? curValue, T? newValue)
        {
            if (Equals(curValue, newValue))
                return 0;

            if (!Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue))
                return -1;

            Global.DebugLogger?.Log(ClassLogCode, "Changed.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }
    }
}