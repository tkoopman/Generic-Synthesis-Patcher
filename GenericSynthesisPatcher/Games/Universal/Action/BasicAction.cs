using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using Common;

using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal.Action
{
    /// <summary>
    ///     Most basic of action where the value can easily be set to a single value of type T. Type
    ///     T must be a type that the defined <see cref="BaseGame.SerializerSettings" /> can auto read.
    /// </summary>
    /// <typeparam name="T">
    ///     Type T must be a type that the defined <see cref="BaseGame.SerializerSettings" /> can
    ///     auto read.
    /// </typeparam>
    public class BasicAction<T> : IRecordAction
    {
        public static readonly BasicAction<T> Instance = new();
        private const int ClassLogCode = 0x11;

        protected BasicAction ()
        {
        }

        // <inheritdoc />
        public bool AllowSubProperties => string.Equals(typeof(T).Namespace, "Noggog", StringComparison.Ordinal);

        /// <inheritdoc />
        public virtual bool CanFill () => true;

        /// <inheritdoc />
        public virtual bool CanForward () => true;

        /// <inheritdoc />
        public virtual bool CanForwardSelfOnly () => false;

        /// <inheritdoc />
        public virtual bool CanMatch () => false;

        /// <inheritdoc />
        public virtual bool CanMerge () => false;

        /// <inheritdoc />
        public virtual int Fill (ProcessingKeys proKeys)
                    => !Mod.TryGetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode)
                    || !proKeys.TryGetFillValueAs(out T? newValue)
                    ? -1
                    : performFill(proKeys, curValue, newValue);

        /// <inheritdoc />
        public IModContext<IMajorRecordGetter>? FindHPUIndex (ProcessingKeys proKeys, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? endNodes) => Mod.FindHPUIndex<T>(proKeys, AllRecordMods, endNodes, ClassLogCode);

        /// <inheritdoc />
        public virtual int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
            => Mod.TryGetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode)
             && Mod.TryGetProperty<T>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue, ClassLogCode)
             ? performFill(proKeys, curValue, newValue)
             : -1;

        /// <inheritdoc />
        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
                    => !Mod.TryGetProperty<T>(recordContext.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode) || Mod.IsNullOrEmpty(curValue);

        /// <inheritdoc />
        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if value matches</returns>
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<T>(recordContext.Record, proKeys.Property.PropertyName, out var curValue, ClassLogCode)
            && Mod.TryGetProperty<T>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue, ClassLogCode)
            && Equals(curValue, originValue));

        /// <inheritdoc />
        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        /// <inheritdoc />
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
            else if (string.Equals(typeof(T).Namespace, "Noggog", StringComparison.Ordinal))
            {
                string? typeName = typeof(T).GetProperty("X")?.PropertyType.Name;
                if (typeName is not null)
                {
                    if (typeof(T).GetProperty("Z") is not null)
                    {
                        description = $"Array of 3 {typeName} values";
                        example = $"""
                                   "{propertyName}": [1, 2, 3]
                                   """;
                    }
                    else
                    {
                        description = $"Array of 2 {typeName} values";
                        example = $"""
                                   "{propertyName}": [1, 2]
                                   """;
                    }
                }
            }

            return description is not null && example is not null;
        }

        protected virtual int performFill (ProcessingKeys proKeys, T? curValue, T? newValue)
        {
            if (Equals(curValue, newValue))
                return 0;

            if (!Mod.TrySetProperty(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, newValue, ClassLogCode))
                return -1;

            Global.Logger.WriteLog(LogLevel.Debug, LogType.RecordUpdated, LogWriter.RecordUpdated, ClassLogCode);
            return 1;
        }
    }
}