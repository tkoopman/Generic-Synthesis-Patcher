using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Games.Skyrim.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Action;
using GenericSynthesisPatcher.Helpers;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Games.Skyrim.Action
{
    public class PlayerSkillsAction : IRecordAction
    {
        public static readonly PlayerSkillsAction Instance = new();
        private const int ClassLogCode = 0x1B;

        protected PlayerSkillsAction ()
        {
        }

        // <inheritdoc />
        public bool AllowSubProperties => true;

        /// <summary>
        ///     Updates the skill values from one skill dictionary to another.
        /// </summary>
        /// <returns>Number of changes made</returns>
        public static int ForwardSkills (IReadOnlyDictionary<Skill, byte> from, IDictionary<Skill, byte> to)
        {
            int count = 0;

            foreach (var skill in Enum.GetValues<Skill>())
            {
                if (from[skill] != to[skill])
                {
                    count++;
                    to[skill] = from[skill];
                }
            }

            return count;
        }

        /// <summary>
        ///     Updates the stats (Health, Stamina, Magicka) from one IPlayerSkillsGetter to another IPlayerSkills.
        /// </summary>
        /// <returns>Number of changes made</returns>
        public static int ForwardStats (IPlayerSkillsGetter from, IPlayerSkills to)
        {
            int count = 0;

            if (from.Health != to.Health)
            {
                count++;
                to.Health = from.Health;
            }

            if (from.Stamina != to.Stamina)
            {
                count++;
                to.Stamina = from.Stamina;
            }

            if (from.Magicka != to.Magicka)
            {
                count++;
                to.Magicka = from.Magicka;
            }

            return count;
        }

        /// <summary>
        ///     Compares two skill dictionaries for equality.
        /// </summary>
        public static bool SkillsEqual (IReadOnlyDictionary<Skill, byte> l, IReadOnlyDictionary<Skill, byte> r)
        {
            foreach (var skill in Enum.GetValues<Skill>())
            {
                if (l[skill] != r[skill])
                    return false;
            }

            return true;
        }

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
        {
            if (!Mod.TryGetProperty<IPlayerSkillsGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                || !proKeys.TryGetFillValueAs(out PlayerSkillsData? newValue)
                || curValue is null
                || newValue is null)
            {
                return -1;
            }

            if (newValue.NonNullEquals(curValue))
                return 0;

            if (!Mod.TryGetPropertyValueForEditing<IPlayerSkills>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var updateValue))
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, "Error getting property to set", rule: proKeys.Rule, context: proKeys.Context, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            return newValue.UpdateRecord(updateValue);
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordGetter>? FindHPUIndex (ProcessingKeys proKeys, IEnumerable<IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? endNodes) => Mod.FindHPUIndex<IPlayerSkillsGetter>(proKeys, AllRecordMods, endNodes);

        /// <inheritdoc />
        public virtual int Forward (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext)
        {
            if (!Mod.TryGetProperty<IPlayerSkillsGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
                || !Mod.TryGetProperty<IPlayerSkillsGetter>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue)
                || curValue is null
                || newValue is null)
            {
                return -1;
            }

            if (curValue.Equals(newValue))
                return 0;

            if (!Mod.TryGetPropertyValueForEditing<IPlayerSkills>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var updateValue))
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, "Error getting property to set", rule: proKeys.Rule, context: proKeys.Context, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            int count = 0;

            if (curValue.FarAwayModelDistance != updateValue.FarAwayModelDistance)
            {
                count++;
                updateValue.FarAwayModelDistance = curValue.FarAwayModelDistance;
            }

            if (curValue.GearedUpWeapons != updateValue.GearedUpWeapons)
            {
                count++;
                updateValue.GearedUpWeapons = curValue.GearedUpWeapons;
            }

            count += ForwardStats(newValue, updateValue);
            count += ForwardSkills(newValue.SkillOffsets, updateValue.SkillOffsets);
            count += ForwardSkills(newValue.SkillValues, updateValue.SkillValues);
            return count;
        }

        /// <inheritdoc />
        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => !Mod.TryGetProperty<IPlayerSkillsGetter>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || curValue is null;

        /// <inheritdoc />
        public virtual bool MatchesOrigin (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => recordContext.IsMaster()
            || (Mod.TryGetProperty<IPlayerSkillsGetter>(recordContext.Record, proKeys.Property.PropertyName, out var curValue)
                && Mod.TryGetProperty<IPlayerSkillsGetter>(proKeys.GetOriginRecord(), proKeys.Property.PropertyName, out var originValue)
                && curValue is not null && originValue is not null
                && curValue.FarAwayModelDistance == originValue.FarAwayModelDistance
                && curValue.GearedUpWeapons == originValue.GearedUpWeapons
                && curValue.Health == originValue.Health
                && curValue.Stamina == originValue.Stamina
                && curValue.Magicka == originValue.Magicka
                && SkillsEqual(curValue.SkillOffsets, originValue.SkillOffsets)
                && SkillsEqual(curValue.SkillValues, originValue.SkillValues));

        /// <inheritdoc />
        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        /// <inheritdoc />
        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();

        // <inheritdoc />
        public virtual bool TryGetDocumentation (Type propertyType, string propertyName, [NotNullWhen(true)] out string? description, [NotNullWhen(true)] out string? example)
        {
            description = "JSON object containing the values under PlayerSkills you want to set";
            example = "See ../Examples/NPC Player Skills.json";

            return true;
        }
    }
}