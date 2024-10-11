using GenericSynthesisPatcher.Json;

using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher
{
    internal static class Global
    {
        internal static Lazy<GSPSettings> settings = null!;
        private static IPatcherState<ISkyrimMod, ISkyrimModGetter>? state;

        public static JsonSerializerSettings SerializerSettings { get; } = new() { MissingMemberHandling = MissingMemberHandling.Ignore, ContractResolver = ContractResolver.Instance };
        public static Lazy<GSPSettings> Settings { get => settings; private set => settings = value; }
        public static IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get => state ?? throw new Exception("Oh boy this shouldn't happen!"); set => state = value; }
    }
}