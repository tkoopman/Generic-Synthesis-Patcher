using System.Diagnostics.CodeAnalysis;

using Common;

using GenericSynthesisPatcher.Games.Skyrim.Json.Action;
using GenericSynthesisPatcher.Games.Universal.Action;
using GenericSynthesisPatcher.Games.Universal.Json.Data;
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

        public static bool SkillsEqual (IReadOnlyDictionary<Skill, byte> l, IReadOnlyDictionary<Skill, byte> r)
        {
            foreach (var skill in Enum.GetValues<Skill>())
            {
                if (l[skill] != r[skill])
                    return false;
            }

            return true;
        }

        public virtual bool CanFill () => true;

        public virtual bool CanForward () => true;

        public virtual bool CanForwardSelfOnly () => false;

        public virtual bool CanMatch () => false;

        public virtual bool CanMerge () => false;

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

        public int FindHPUIndex (ProcessingKeys proKeys, IEnumerable<ModKey> mods, IEnumerable<int> indexes, Dictionary<ModKey, IModContext<IMajorRecordGetter>> AllRecordMods, IEnumerable<ModKey>? validMods)
        {
            bool nonNull = proKeys.Rule.HasForwardType(ForwardOptions._nonNullMod);
            List<IPlayerSkillsGetter?> history = [];
            int hpu = -1;
            int hpuHistory = -1;

            foreach (int i in indexes.Reverse())
            {
                var mc = AllRecordMods[mods.ElementAt(i)];

                if (Mod.TryGetProperty<IPlayerSkillsGetter>(mc.Record, proKeys.Property.PropertyName, out var curValue)
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

        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public virtual bool IsNullOrEmpty (ProcessingKeys proKeys, IModContext<IMajorRecordGetter> recordContext)
            => !Mod.TryGetProperty<IPlayerSkillsGetter>(recordContext.Record, proKeys.Property.PropertyName, out var curValue) || curValue is null;

        /// <summary>
        ///     Called when GSPRule.OnlyIfDefault is true
        /// </summary>
        /// <returns>True if all player skills data matches</returns>
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

        public bool MatchesOrigin (ProcessingKeys proKeys) => MatchesOrigin(proKeys, proKeys.Context);

        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

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