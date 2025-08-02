using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Helpers;

using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace GenericSynthesisPatcher
{
    internal static class Global
    {
        internal static Lazy<GSPSettings> LazySettings = null!;

        /// <summary>
        ///     Global Game instance that contains all game specific information.
        /// </summary>
        public static BaseGame Game { get; private set; } = null!;

        public static LogWriter Logger { get; private set; } = null!;

        public static GSPSettings Settings { get; private set; } = null!;

        /// <summary>
        ///     Fully initializes the global state of the patcher. MUST be called prior to using
        ///     this class.
        /// </summary>
        /// <param name="state">Sate object for session.</param>
        /// <param name="settings">Used for testing only when settings not provided by LazySettings.</param>
        /// <exception cref="Exception">LazySettings not set and didn't provide test settings.</exception>
        /// <exception cref="InvalidCastException">
        ///     When state is not a valid supported game state.
        /// </exception>
        public static void Initialize (IPatcherState state, GSPSettings? settings = null)
        {
            Settings = (settings is null ? LazySettings?.Value : settings) ?? throw new Exception("Settings not provided.");
            Logger = new LogWriter();

            //TODO Add games
            Game = state switch
            {
                IPatcherState<ISkyrimMod, ISkyrimModGetter> gameState => new Games.Skyrim.SkyrimGame(gameState),
                IPatcherState<IFallout4Mod, IFallout4ModGetter> gameState => new Games.Fallout4.Fallout4Game(gameState),
                IPatcherState<IOblivionMod, IOblivionModGetter> gameState => new Games.Oblivion.OblivionGame(gameState),
                _ => throw new InvalidCastException(),
            };
        }
    }
}