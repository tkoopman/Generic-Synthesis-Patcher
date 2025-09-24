using System.Reflection;

using GenericSynthesisPatcher.Games.Universal;
using GenericSynthesisPatcher.Helpers;

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

        /// <summary>
        ///     GSP Log Writer.
        /// </summary>
        public static LogWriter Logger { get; private set; } = null!;

        /// <summary>
        ///     Version of Mutagen.
        /// </summary>
        public static string MutagenVersion { get; } = typeof(Mutagen.Bethesda.GameRelease).Assembly.GetName().Version?.ToString() ?? "Error getting version number.";

        /// <summary>
        ///     Synthesis Settings.
        /// </summary>
        public static GSPSettings Settings { get; private set; } = null!;

        /// <summary>
        ///     Version of Synthesis.
        /// </summary>
        public static string SynthesisVersion { get; } = typeof(IPatcherState).Assembly.GetName().Version?.ToString() ?? "Error getting version number.";

        /// <summary>
        ///     Version of GSP Build.
        /// </summary>
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Error getting version number.";

        /// <inheritdoc cref="initialize(GSPSettings?)" />
        /// <param name="state">Skyrim state object for session.</param>
        public static void Initialize (IPatcherState<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter> state, GSPSettings? settings = null)
        {
            initialize(settings);
            Game = new Games.Skyrim.SkyrimGame(state);
            Console.WriteLine($"GSP initialized for Skyrim.");
        }

        /// <inheritdoc cref="initialize(GSPSettings?)" />
        /// <param name="state">Fallout4 state object for session.</param>
        public static void Initialize (IPatcherState<Mutagen.Bethesda.Fallout4.IFallout4Mod, Mutagen.Bethesda.Fallout4.IFallout4ModGetter> state, GSPSettings? settings = null)
        {
            initialize(settings);
            Game = new Games.Fallout4.Fallout4Game(state);
            Console.WriteLine($"GSP initialized for Fallout 4.");
        }

        /// <inheritdoc cref="initialize(GSPSettings?)" />
        /// <param name="state">Oblivion state object for session.</param>
        public static void Initialize (IPatcherState<Mutagen.Bethesda.Oblivion.IOblivionMod, Mutagen.Bethesda.Oblivion.IOblivionModGetter> state, GSPSettings? settings = null)
        {
            initialize(settings);
            Game = new Games.Oblivion.OblivionGame(state);
            Console.WriteLine($"GSP initialized for Oblivion.");
        }

        public static void Initialize (IPatcherState state, GSPSettings? settings = null)
        {
            switch (state)
            {
                case IPatcherState<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter> skyrimState:
                    Initialize(skyrimState, settings);
                    break;

                case IPatcherState<Mutagen.Bethesda.Fallout4.IFallout4Mod, Mutagen.Bethesda.Fallout4.IFallout4ModGetter> fallout4State:
                    Initialize(fallout4State, settings);
                    break;

                case IPatcherState<Mutagen.Bethesda.Oblivion.IOblivionMod, Mutagen.Bethesda.Oblivion.IOblivionModGetter> oblivionState:
                    Initialize(oblivionState, settings);
                    break;

                default:
                    throw new InvalidCastException("State is not a valid supported game state.");
            }
        }

        /// <summary>
        ///     Fully initializes the global state of the patcher. MUST be called prior to using
        ///     this class.
        /// </summary>
        /// <param name="settings">Used for testing only when settings not provided by LazySettings.</param>
        /// <exception cref="Exception">LazySettings not set and didn't provide test settings.</exception>
        private static void initialize (GSPSettings? settings = null)
        {
            Settings = (settings is null ? LazySettings?.Value : settings) ?? throw new Exception("Settings not provided.");
            Logger = new LogWriter();
        }
    }
}