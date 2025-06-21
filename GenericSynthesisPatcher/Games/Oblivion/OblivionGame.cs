using System.Reflection;

using GenericSynthesisPatcher.Games.Oblivion.Action;
using GenericSynthesisPatcher.Games.Universal.Action;

using Loqui;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Synthesis;

using Noggog;

namespace GenericSynthesisPatcher.Games.Oblivion
{
    public class OblivionGame : Universal.BaseGame
    {
        private OblivionGame () => State = null!;

        private OblivionGame (IPatcherState<IOblivionMod, IOblivionModGetter> gameState) : base(new(gameState.LoadOrder.Select(m => (IModListingGetter)m.Value))) => State = gameState;

        public override IPatcherState<IOblivionMod, IOblivionModGetter> State { get; }

        protected override Type TypeOptionSolidifierMixIns => typeof(TypeOptionSolidifierMixIns);

        public static OblivionGame Constructor (IPatcherState<IOblivionMod, IOblivionModGetter> gameState)
        {
            var game = gameState is null ? new OblivionGame () : new OblivionGame(gameState);

            //game.SerializerSettings.Converters.Add(new ObjectBoundsConverter());

            game.IgnoreSubPropertiesOnTypes.Add(
                [
                    typeof(Cell),
                ]);

            return game;
        }

        public override IEnumerable<IModContext<IMajorRecordGetter>> GetRecords (ILoquiRegistration recordType)
        {
            var m = typeof(TypeOptionSolidifierMixIns).GetMethod(recordType.Name, BindingFlags.Public | BindingFlags.Static, [typeof(IEnumerable<IModListingGetter<IOblivionModGetter>>)]) ?? throw new InvalidOperationException($"No method found for record type {recordType.Name}.");

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
                case 2:
                    if (explodedType[0] == typeof(ExtendedList<>))
                    {
                        if (explodedType[1].IsAssignableTo(typeof(IContainerItemGetter)))
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