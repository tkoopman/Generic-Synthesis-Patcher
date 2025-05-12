using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using Common;

using GenericSynthesisPatcher.Games.Universal.Action;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Strings;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal
{
    /// <summary>
    ///     Class used for generating property mappings and documentation.
    /// </summary>
    public abstract class Genny
    {
        public Genny ()
        {
            addMapping(typeof(ITranslatedStringGetter), false, typeof(ConvertibleAction<string>));
            addMapping(typeof(Percent), true, typeof(BasicAction<Percent>));
            addMapping(typeof(Color), true, typeof(BasicAction<Color>));
            addMapping(typeof(MemorySlice<byte>), true, typeof(MemorySliceByteAction));

            addMapping([typeof(IFormLink<>), null], false, typeof(FormLinkAction<>));
            addMapping([typeof(IFormLinkGetter<>), null], false, typeof(FormLinkAction<>));
            addMapping([typeof(IFormLinkNullable<>), null], false, typeof(FormLinkAction<>));
            addMapping([typeof(IFormLinkNullableGetter<>), null], false, typeof(FormLinkAction<>));

            addMapping([typeof(ExtendedList<>), typeof(IFormLink<>), null], false, typeof(FormLinksAction<>));
            addMapping([typeof(ExtendedList<>), typeof(IFormLinkGetter<>), null], false, typeof(FormLinksAction<>));
            addMapping([typeof(ExtendedList<>), typeof(IFormLinkNullable<>), null], false, typeof(FormLinksAction<>));
            addMapping([typeof(ExtendedList<>), typeof(IFormLinkNullableGetter<>), null], false, typeof(FormLinksAction<>));
        }

        public Type[] ForceDeeperTypes { get; protected set; } =
                    [
            typeof(P2Double),
            typeof(P2Float),
            typeof(P2Int),
            typeof(P2Int16),
            typeof(P3Double),
            typeof(P3Float),
            typeof(P3Int),
            typeof(P3Int16),
            ];

        public abstract string GameName { get; }

        public Type[] IgnoreDeepScanOnTypes { get; protected set; } =
            [
            typeof(AssetLink<>),
            typeof(ExtendedList<>),
            typeof(FormLink<>),
            typeof(FormLinkNullable<>),
            typeof(IFormLink<>),
            typeof(IFormLinkNullable<>),
            typeof(string),
            typeof(TranslatedString),
            ];

        public abstract Type[] IgnoreMajorRecordGetterTypes { get; }

        public string[] IgnoreProperty { get; protected set; } =
            [
            "BodyTemplate.ActsLike44",
            "DATADataTypeState",
            "FormKey",
            "IsCompressed",
            "IsDeleted",
            "MajorRecordFlagsRaw",
            "PersistentTimestamp",
            "PersistentUnknownGroupData",
            "SubCellsTimestamp",
            "SubCellsUnknown",
            "TemporaryTimestamp",
            "TemporaryUnknownGroupData",
            "Timestamp",
            "TopCell",
            "Unknown",
            "Unknown08",
            "Unknown09",
            "Unknown0A",
            "Unknown0B",
            "Unknown0C",
            "Unknown0D",
            "Unknown0E",
            "Unknown0F",
            "Unknown1",
            "Unknown10",
            "Unknown14",
            "Unknown2",
            "Unknown3",
            "Unknown4",
            "Unknown4",
            "Unknown48",
            "Unknown49",
            "Unknown4A",
            "Unknown4B",
            "Unknown4C",
            "Unknown4D",
            "Unknown4E",
            "Unknown4F",
            "Unknown5",
            "Unknown50",
            "Unknown54",
            "Unknown6",
            "Unknown7",
            "UnknownGroupData",
            "Unused",
            "Unused2",
            "Unused3",
            "Unused4",
            "UnusedNoisemaps",
            "Version2",
            "VersionControl",
            "Versioning",
            ];

        private Dictionary<Type?[], Type> Mappings { get; } = [];

        private Dictionary<Type?[], Type> MappingsAssignableTo { get; } = [];

        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public virtual Type? GetRPMAction (Type type)
        {
            type = type.RemoveNullable();

            if (type.IsEnum)
                return type.GetCustomAttributes(typeof(FlagsAttribute), true).FirstOrDefault() is null ? typeof(EnumsAction) : typeof(FlagsAction);

            if (type.IsAssignableTo(typeof(IConvertible)) && (type.IsPrimitive || type == typeof(string)))
            {
                return type.Name switch
                {
                    nameof(Boolean) => typeof(ConvertibleAction<bool>),
                    nameof(Byte) => typeof(ConvertibleAction<byte>),
                    nameof(Char) => typeof(ConvertibleAction<char>),
                    nameof(Int16) => typeof(ConvertibleAction<short>),
                    nameof(Int32) => typeof(ConvertibleAction<int>),
                    nameof(SByte) => typeof(ConvertibleAction<sbyte>),
                    nameof(Single) => typeof(ConvertibleAction<float>),
                    nameof(String) => typeof(ConvertibleAction<string>),
                    nameof(UInt16) => typeof(ConvertibleAction<ushort>),
                    nameof(UInt32) => typeof(ConvertibleAction<uint>),
                    _ => null
                };
            }

            return checkMappings(type, true, Mappings) ?? checkMappings(type, false, MappingsAssignableTo);
        }

        public virtual string RPMPopulateFooter () => """
            #pragma warning restore format
                    }
                }
            }
            """;

        public virtual string RPMPopulateHeader (string classLine, string methodLine) => $$"""
            using System.Drawing;

            using GenericSynthesisPatcher.Games.Universal.Action;
            using GenericSynthesisPatcher.Games.{{GameName}}.Action;

            using Mutagen.Bethesda;
            using Mutagen.Bethesda.{{GameName}};
            using Mutagen.Bethesda.Synthesis;

            using Noggog;

            namespace GenericSynthesisPatcher.Games.{{GameName}}
            {
                {{classLine}}
                {
                    {{methodLine}}
                    {
            #pragma warning disable format
            """;

        /// <summary>
        ///     Adds a mapping that will be used in GetRPMAction. Type being checked will be
        ///     Exploded using length of matches as max size. You can use null if you will accept
        ///     any type in that position.
        /// </summary>
        /// <param name="match">Exact type to match</param>
        /// <param name="exact">
        ///     If true, all types must match exactly, else type.IsAssignableTo(match[x]) is used
        /// </param>
        /// <param name="assign">
        ///     IRecordAction type to assign. Can be IRecordAction&lt;&gt; in which case will create
        ///     generic from definition, using the last Type of exploded types.
        /// </param>
        /// <exception cref="InvalidCastException">
        ///     If assign is not of type IRecordAction
        /// </exception>
        protected void addMapping (Type?[] match, bool exact, Type assign)
        {
            if (assign.IsAssignableTo(typeof(IRecordAction)))
            {
                if (exact)
                    Mappings.Add(match, assign);
                else
                    MappingsAssignableTo.Add(match, assign);
            }
            else
            {
                throw new InvalidCastException("Assign type must be of type IRecordAction");
            }
        }

        protected void addMapping (Type type, bool exact, Type assign) => addMapping([type], exact, assign);

        private static Type? checkMappings (Type type, bool exact, Dictionary<Type?[], Type> mappings)
        {
            foreach (var (k, v) in mappings)
            {
                if (isMatch(type, k, exact, out var result))
                    return v.IsGenericTypeDefinition ? v.MakeGenericType([result]) : v;
            }

            return null;
        }

        private static bool isMatch (Type type, Type?[] matchTypes, bool exact, [NotNullWhen(true)] out Type? last)
        {
            last = null;

            var types = type.Explode(matchTypes.Length);
            if (types.Length != matchTypes.Length)
                return false;

            for (int x = 0; x < types.Length; x++)
            {
                if (matchTypes[x] is null)
                    continue;

                if (exact && types[x] != matchTypes[x])
                    return false;

                if (!exact && !types[x].IsAssignableTo(matchTypes[x]))
                    return false;
            }

            last = types[^1];
            return true;
        }
    }
}