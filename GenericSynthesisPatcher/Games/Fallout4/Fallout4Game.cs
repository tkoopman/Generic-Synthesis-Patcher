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
        /// <summary>
        ///     Create new Fallout 4 Game instance.
        /// </summary>
        /// <param name="gameState">
        ///     Patcher state. Can use null! in xUnit testing as long as you don't call anything
        ///     that requires State or LoadOrder.
        /// </param>
        public Fallout4Game (IPatcherState<IFallout4Mod, IFallout4ModGetter> gameState) : base(constructLoadOrder(gameState?.LoadOrder.Select(m => (IModListingGetter)m.Value))!)
        {
            State = gameState!;

            SerializerSettings.Converters.Add(new ObjectBoundsConverter());

            IgnoreSubPropertiesOnTypes.Add(
                [
                    typeof(Cell),
                    typeof(Destructible),
                    typeof(NavmeshGeometry),
                ]);

            addExactMatch(typeof(WorldspaceMaxHeight), WorldspaceMaxHeightAction.Instance);
            addExactMatch(typeof(CellMaxHeightData), CellMaxHeightDataAction.Instance);
        }

        public override GameCategory GameCategory => GameCategory.Fallout4;
        public override GameRelease GameRelease => State?.GameRelease ?? GameRelease.Fallout4;
        public override IPatcherState<IFallout4Mod, IFallout4ModGetter> State { get; }

        public override Type TypeOptionSolidifierMixIns => typeof(TypeOptionSolidifierMixIns);

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