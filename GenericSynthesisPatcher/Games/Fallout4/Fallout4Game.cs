using System.Reflection;

using GenericSynthesisPatcher.Games.Fallout4.Action;
using GenericSynthesisPatcher.Games.Fallout4.Json.Converters;
using GenericSynthesisPatcher.Games.Universal.Action;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Synthesis;

using Noggog;

namespace GenericSynthesisPatcher.Games.Fallout4
{
    public class Fallout4Game : Universal.BaseGame
    {
        private readonly IEnumerable<IModListing<IFallout4ModGetter>> OnlyEnabledAndExisting;

        public Fallout4Game (IPatcherState<IFallout4Mod, IFallout4ModGetter> gameState) : base(new(gameState.LoadOrder.Select(m => (IModListingGetter)m.Value)))
        {
            State = gameState;
            OnlyEnabledAndExisting = State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting();
            SerializerSettings.Converters.Add(new ObjectBoundsConverter());

            IgnoreSubPropertiesOnTypes.Add(
                [
                typeof(AMagicEffectArchetype),
                typeof(Cell),
                typeof(CellMaxHeightData),
                typeof(DialogResponsesAdapter),
                typeof(FaceFxPhonemes),
                typeof(Landscape),
                typeof(LocationTargetRadius),
                typeof(Model),
                typeof(PackageAdapter),
                typeof(PerkAdapter),
                typeof(QuestAdapter),
                typeof(RegionGrasses),
                typeof(RegionLand),
                typeof(RegionMap),
                typeof(RegionObjects),
                typeof(RegionSounds),
                typeof(RegionWeather),
                typeof(SceneAdapter),
                typeof(VirtualMachineAdapter),
                typeof(WorldspaceMaxHeight),
                ]);

            addExactMatch(typeof(WorldspaceMaxHeight), WorldspaceMaxHeightAction.Instance);
            addExactMatch(typeof(CellMaxHeightData), CellMaxHeightDataAction.Instance);
        }

        public override IPatcherState<IFallout4Mod, IFallout4ModGetter> State { get; }

        public override IEnumerable<IModContext<IMajorRecordGetter>> GetRecords (ILoquiRegistration recordType)
        {
            var m = typeof(TypeOptionSolidifierMixIns).GetMethod(recordType.Name, BindingFlags.Public | BindingFlags.Static, [typeof(IEnumerable<IModListingGetter<IFallout4ModGetter>>)]) ?? throw new InvalidOperationException($"No method found for record type {recordType.Name}.");

            object records = m.Invoke(null, [OnlyEnabledAndExisting]) ?? throw new InvalidOperationException($"Failed to call method for record type {recordType.Name}.");

            m = records.GetType().GetMethod("WinningContextOverrides") ?? throw new InvalidOperationException($"No WinningContextOverrides method found for record type {recordType.Name}.");

            return m.GetParameters().Length == 1
                ? (IEnumerable<IModContext<IMajorRecordGetter>>)(m.Invoke(records, [false]) ?? throw new InvalidOperationException($"Failed to call WinningContextOverrides method found for record type {recordType.Name}."))
                : (IEnumerable<IModContext<IMajorRecordGetter>>)(m.Invoke(records, [State.LinkCache, false]) ?? throw new InvalidOperationException($"Failed to call WinningContextOverrides method found for record type {recordType.Name}."));
        }

        protected override IRecordAction? discoverAction (Type[] explodedType)
        {
            var result = base.discoverAction(explodedType);
            if (result is not null)
                return result;

            switch (explodedType.Length)
            {
                case 1:
                    if (explodedType[0].IsAssignableTo(typeof(IObjectBoundsGetter)))
                        return ObjectBoundsAction.Instance;

                    return null;

                case 2:
                    if (explodedType[0] == typeof(ExtendedList<>))
                    {
                        if (explodedType[1].IsAssignableTo(typeof(IContainerEntryGetter)))
                            return ContainerItemsAction.Instance;

                        if (explodedType[1].IsAssignableTo(typeof(IEffectGetter)))
                            return EffectsAction.Instance;
                    }

                    return null;

                default:
                    return null;
            }
        }

        protected override IEnumerable<ILoquiRegistration> getRecordTypes ()
        {
            List<ILoquiRegistration> types = [];

            foreach (var method in typeof(TypeOptionSolidifierMixIns).GetMethods())
            {
                if (!method.ReturnType.IsGenericType || method.ReturnType.GenericTypeArguments.Length == 0)
                    continue;

                var returnType = method.ReturnType.GenericTypeArguments[^1];

                var regoProperty = returnType.GetProperty("StaticRegistration");
                if (regoProperty?.GetValue(null) is ILoquiRegistration rego)
                    types.Add(rego);
            }

            return types;
        }
    }
}