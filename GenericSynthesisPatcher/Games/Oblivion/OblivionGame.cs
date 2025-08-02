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
        /// <summary>
        ///     Create new Oblivion Game instance.
        /// </summary>
        /// <param name="gameState">
        ///     Patcher state. Can use null! in xUnit testing as long as you don't call anything
        ///     that requires State or LoadOrder.
        /// </param>

        public OblivionGame (IPatcherState<IOblivionMod, IOblivionModGetter> gameState) : base(constructLoadOrder(gameState?.LoadOrder.Select(m => (IModListingGetter)m.Value))!)
        {
            State = gameState!;

            //SerializerSettings.Converters.Add(new ObjectBoundsConverter());

            IgnoreSubPropertiesOnTypes.Add(
                [
                    typeof(Cell),
                ]);
        }

        public override GameCategory GameCategory => GameCategory.Oblivion;
        public override GameRelease GameRelease => State?.GameRelease ?? GameRelease.OblivionRE;
        public override IPatcherState<IOblivionMod, IOblivionModGetter> State { get; }

        public override Type TypeOptionSolidifierMixIns => typeof(TypeOptionSolidifierMixIns);

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