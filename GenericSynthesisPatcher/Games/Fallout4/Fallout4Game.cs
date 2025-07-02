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
        private Fallout4Game () => State = null!;

        private Fallout4Game (IPatcherState<IFallout4Mod, IFallout4ModGetter> gameState) : base(new(gameState.LoadOrder.Select(m => (IModListingGetter)m.Value))) => State = gameState;

        public override IPatcherState<IFallout4Mod, IFallout4ModGetter> State { get; }

        public override Type TypeOptionSolidifierMixIns => typeof(TypeOptionSolidifierMixIns);

        public static Fallout4Game Constructor (IPatcherState<IFallout4Mod, IFallout4ModGetter> gameState)
        {
            var game = gameState is null ? new Fallout4Game () : new Fallout4Game(gameState);
            game.SerializerSettings.Converters.Add(new ObjectBoundsConverter());

            game.IgnoreSubPropertiesOnTypes.Add(
                [
                    typeof(Cell),
                    typeof(Destructible),
                    typeof(NavmeshGeometry),
                ]);

            game.addExactMatch(typeof(WorldspaceMaxHeight), WorldspaceMaxHeightAction.Instance);
            game.addExactMatch(typeof(CellMaxHeightData), CellMaxHeightDataAction.Instance);

            return game;
        }

        public override IEnumerable<IModContext<IMajorRecordGetter>> GetRecords (ILoquiRegistration recordType)
        {
            var m = typeof(TypeOptionSolidifierMixIns).GetMethod(recordType.Name, BindingFlags.Public | BindingFlags.Static, [typeof(IEnumerable<IModListingGetter<IFallout4ModGetter>>)]) ?? throw new InvalidOperationException($"No method found for record type {recordType.Name}.");

            object records = m.Invoke(null, [State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting()]) ?? throw new InvalidOperationException($"Failed to call method for record type {recordType.Name}.");

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
    }
}