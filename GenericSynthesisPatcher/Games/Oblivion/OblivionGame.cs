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
        private readonly IEnumerable<IModListing<IOblivionModGetter>> OnlyEnabledAndExisting;

        public OblivionGame (IPatcherState<IOblivionMod, IOblivionModGetter> gameState) : base(new(gameState.LoadOrder.Select(m => (IModListingGetter)m.Value)))
        {
            State = gameState;
            OnlyEnabledAndExisting = State.LoadOrder.PriorityOrder.OnlyEnabledAndExisting();

            //SerializerSettings.Converters.Add(new ObjectBoundsConverter());

            IgnoreSubPropertiesOnTypes.Add(
                [
                typeof(Cell),
                typeof(Landscape),
                typeof(Model),
                typeof(RegionGrasses),
                typeof(RegionMap),
                typeof(RegionObjects),
                typeof(RegionSounds),
                typeof(RegionWeather),
                ]);
        }

        public override IPatcherState<IOblivionMod, IOblivionModGetter> State { get; }

        public override IEnumerable<IModContext<IMajorRecordGetter>> GetRecords (ILoquiRegistration recordType)
        {
            var m = typeof(TypeOptionSolidifierMixIns).GetMethod(recordType.Name, BindingFlags.Public | BindingFlags.Static, [typeof(IEnumerable<IModListingGetter<IOblivionModGetter>>)]) ?? throw new InvalidOperationException($"No method found for record type {recordType.Name}.");

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