using GenericSynthesisPatcher.Json.Data.Action;

using Microsoft.Extensions.Logging;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class PlayerSkillsAction : IRecordAction
    {
        public static readonly PlayerSkillsAction Instance = new();
        private const int ClassLogCode = 0x1B;

        protected PlayerSkillsAction ()
        {
        }
        public static bool SkillsEqual (IReadOnlyDictionary<Skill, byte> l, IReadOnlyDictionary<Skill, byte> r)
        {
            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                if (l[skill] != r[skill])
                    return false;
            }

            return true;
        }

        public static int ForwardSkills (IReadOnlyDictionary<Skill, byte> from, IDictionary<Skill, byte> to)
        {
            int count = 0;

            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
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

        public virtual bool CanFill () => true;

        public virtual bool CanForward () => true;

        public virtual bool CanForwardSelfOnly () => false;

        public virtual bool CanMatch () => false;

        public virtual bool CanMerge () => false;

        public virtual int Fill (ProcessingKeys proKeys)
        {
            if (!Mod.TryGetProperty<IPlayerSkillsGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue)
             || !proKeys.TryGetFillValueAs(out PlayerSkillsData? newValue)
             || curValue == null || newValue == null)
                return -1;

            if (newValue.Equals(curValue))
                return 0;

            if (!Mod.TryGetPropertyForEditing<IPlayerSkills>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var updateValue))
            {
                LogHelper.WriteLog(LogLevel.Error, ClassLogCode, "Error getting property to set", rule: proKeys.Rule, context: proKeys.Context, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            return newValue.Update(updateValue);
        }

        public virtual int Forward (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext)
        {
            if (!Mod.TryGetProperty<IPlayerSkillsGetter>(proKeys.Record, proKeys.Property.PropertyName, out var curValue) ||
                !Mod.TryGetProperty<IPlayerSkillsGetter>(forwardContext.Record, proKeys.Property.PropertyName, out var newValue) ||
                curValue == null || newValue == null)
            { return -1; }

            if (curValue.Equals(newValue))
                return 0;

            if (!Mod.TryGetPropertyForEditing<IPlayerSkills>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var updateValue))
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

        public virtual int ForwardSelfOnly (ProcessingKeys proKeys, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> forwardContext) => throw new NotImplementedException();

        public virtual bool MatchesOrigin (ProcessingKeys proKeys)
        {
            var origin = proKeys.GetOriginRecord();
            return origin != null
                        && Mod.TryGetProperty<IPlayerSkillsGetter>(proKeys.Context.Record, proKeys.Property.PropertyName, out var curValue)
                        && Mod.TryGetProperty<IPlayerSkillsGetter>(origin, proKeys.Property.PropertyName, out var originValue)
                        && curValue != null && originValue != null
                        && curValue.FarAwayModelDistance == originValue.FarAwayModelDistance
                        && curValue.GearedUpWeapons == originValue.GearedUpWeapons
                        && curValue.Health == originValue.Health
                        && curValue.Stamina == originValue.Stamina
                        && curValue.Magicka == originValue.Magicka
                        && SkillsEqual(curValue.SkillOffsets, originValue.SkillOffsets)
                        && SkillsEqual(curValue.SkillValues, originValue.SkillValues);
        }

        public virtual bool MatchesRule (ProcessingKeys proKeys) => throw new NotImplementedException();

        public virtual int Merge (ProcessingKeys proKeys) => throw new NotImplementedException();
    }
}