using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Common;
using Common.JsonConverters;

using GenericSynthesisPatcher.Games.Universal.Action;
using GenericSynthesisPatcher.Games.Universal.Json.Converters;
using GenericSynthesisPatcher.Helpers;

using Loqui;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;

using Newtonsoft.Json;

using Noggog;

namespace GenericSynthesisPatcher.Games.Universal
{
    /// <summary>
    ///     Base class for all games supported by Generic Synthesis Patcher.
    /// </summary>
    /// <param name="recordTypeMappings"></param>
    /// <param name="recordPropertyMappings"></param>
    /// <param name="loadOrder"></param>
    public abstract class BaseGame (LoadOrder<IModListingGetter> loadOrder)
    {
        private readonly HashSet<PropertyAliasMapping> propertyAliases = [];

        private ReadOnlyDictionary<string, ILoquiRegistration>? _recordTypes;

        public HashSet<Type> IgnoreSubPropertiesOnTypes { get; protected set; } =
                    [
                typeof(P2Double),
                typeof(P2Float),
                typeof(P2Int),
                typeof(P2Int16),
                typeof(P2UInt8),
                typeof(P3Double),
                typeof(P3Float),
                typeof(P3Int),
                typeof(P3Int16),
                typeof(P3UInt16),
                typeof(P3UInt8),
                typeof(AssetLink<>),
                typeof(ExtendedList<>),
                typeof(FormLink<>),
                typeof(FormLinkNullable<>),
                typeof(IFormLink<>),
                typeof(IFormLinkNullable<>),
                typeof(string),
                typeof(TranslatedString),
            ];

        public LoadOrder<IModListingGetter> LoadOrder { get; } = loadOrder;

        public ReadOnlyDictionary<string, ILoquiRegistration> RecordTypes
        {
            get
            {
                if (_recordTypes is null)
                {
                    var recordTypes = new Dictionary<string, ILoquiRegistration>(StringComparer.OrdinalIgnoreCase);

                    foreach (var recordType in getRecordTypes())
                    {
                        if (!TranslationMaskFactory.TryGetTranslationMaskType(recordType, out _))
                            continue;

                        _ = recordTypes.TryAdd(recordType.Name, recordType);
                        if (recordType.GetType().GetField("TriggeringRecordType")?.GetValue(recordType) is RecordType type)
                            _ = recordTypes.TryAdd(type.Type, recordType);
                    }

                    _recordTypes = recordTypes.AsReadOnly();
                }

                return _recordTypes;
            }
        }

        public JsonSerializerSettings SerializerSettings { get; } = new()
        {
            Converters =
            [
                new ColorConverter(),
                new FormKeyConverter(),
                new ModKeyConverter(),
                new NoggogPxConverter(),
                new OperationsConverter(),
                new PercentConverter(),
                new RecordTypeConverter(),
                new TranslationMaskConverter()
            ],
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
        };

        public abstract IPatcherState State { get; }

        private Dictionary<PropertyKey, PropertyAction?> PropertyMappings { get; } = [];

        private Dictionary<Type, IRecordAction?> TypeMappingsExact { get; } = new()
        {
            // Convertible Actions
            { typeof(bool), ConvertibleAction<bool>.Instance },
            { typeof(byte), ConvertibleAction<byte>.Instance },
            { typeof(char), ConvertibleAction<char>.Instance },
            { typeof(float), ConvertibleAction<float>.Instance },
            { typeof(int), ConvertibleAction<int>.Instance },
            { typeof(sbyte), ConvertibleAction<sbyte>.Instance },
            { typeof(short), ConvertibleAction<short>.Instance },
            { typeof(string), ConvertibleAction<string>.Instance  },
            { typeof(uint), ConvertibleAction<uint>.Instance },
            { typeof(ushort), ConvertibleAction<ushort>.Instance },

            // Basic Actions
            { typeof(System.Drawing.Color), BasicAction<System.Drawing.Color>.Instance },
            { typeof(MemorySlice<byte>), MemorySliceByteAction.Instance },
            { typeof(P2Double), BasicAction<P2Double>.Instance },
            { typeof(P2Float), BasicAction<P2Float>.Instance },
            { typeof(P2Int), BasicAction<P2Int>.Instance },
            { typeof(P2Int16), BasicAction<P2Int16>.Instance  },
            { typeof(P2UInt8), BasicAction<P2UInt8>.Instance },
            { typeof(P3Double), BasicAction<P3Double>.Instance },
            { typeof(P3Float), BasicAction<P3Float>.Instance },
            { typeof(P3Int), BasicAction<P3Int>.Instance },
            { typeof(P3Int16), BasicAction<P3Int16>.Instance },
            { typeof(P3UInt16), BasicAction<P3UInt16>.Instance },
            { typeof(P3UInt8), BasicAction<P3UInt8>.Instance },
            { typeof(Percent), BasicAction<Percent>.Instance },
        };

        public IEnumerable<ILoquiRegistration> AllRecordTypes () => new HashSet<ILoquiRegistration>(RecordTypes.Values);

        /// <summary>
        ///     Get the action for a given property name in a record type. propertyName will be
        ///     searched for on the record in the following order:
        ///     <list type="number">
        ///         <item>Exact match</item>
        ///         <item>
        ///             <see cref="GetRealPropertyName(Type, string)" /> to resolve possible alias
        ///         </item>
        ///         <item>Case insensitive search for property name.</item>
        ///     </list>
        /// </summary>
        /// <param name="recordType">Record Type property belongs to</param>
        /// <param name="propertyName">Name of property.</param>
        /// <returns>Record action class if found else null.</returns>
        public PropertyAction GetAction (ILoquiRegistration recordType, string propertyName)
        {
            var key = new PropertyKey(recordType, propertyName);
            if (PropertyMappings.TryGetValue(key, out var value) && value is not null)
                return value.Value;

            IRecordAction? action = null;
            if (TryGetProperties(recordType, propertyName, out var properties, out propertyName, StringComparison.OrdinalIgnoreCase))
            {
                var property = properties[^1];
                action = discoverAction(property.PropertyType);

                if (action is null && properties.Length == 1)
                {
                    if (TranslationMaskFactory.TryCreate(recordType, false, [propertyName], out _))
                        action = DeepCopyInAction.Instance;
                }
            }

            var pa = new PropertyAction(recordType, properties, propertyName, action);
            PropertyMappings[key] = pa;
            return pa;
        }

        /// <summary>
        ///     Resolve a possible alias of a property name for a given record type. This is called
        ///     by <see cref="GetAction(ILoquiRegistration, string)" /> to resolve the property name
        ///     if it is an alias.
        /// </summary>
        /// <param name="type">Record type property being accessed references.</param>
        /// <param name="name">
        ///     Property name that was provided to check if it is a valid alias.
        /// </param>
        /// <returns>Proper property name or null</returns>
        public string? GetRealPropertyName (ILoquiRegistration recordType, string name)
            => propertyAliases.TryGetValue(new PropertyAliasMapping(recordType.ClassType, name, null), out var alias)
            || propertyAliases.TryGetValue(new PropertyAliasMapping(null, name, null), out alias)
            ? alias.RealPropertyName
            : null;

        public abstract IEnumerable<IModContext<IMajorRecordGetter>> GetRecords (ILoquiRegistration recordType);

        public ILoquiRegistration? GetRecordType (string name) => RecordTypes.TryGetValue(name, out var recordType) ? recordType : null;

        /// <summary>
        ///     Get the action for a given property name in a record type.
        /// </summary>
        /// <param name="recordType">
        ///     Static Registration of major record type being accessed.
        /// </param>
        /// <param name="propertyName">Name of property</param>
        /// <param name="stringComparison">
        ///     String comparison to use when searching for property. Exact match always checked for
        ///     first.
        /// </param>
        /// <returns>Record action class if found else null.</returns>
        public bool TryGetProperties (ILoquiRegistration recordType, string propertyName, out PropertyInfo[] properties, out string correctedPropertyName, StringComparison stringComparison = StringComparison.Ordinal)
        {
            var parent = recordType.ClassType;
            string[] propertyNames = propertyName.Split('.');
            int depth = propertyNames.Length;
            int last = depth-1;

            bool valid = true;
            properties = new PropertyInfo[depth];
            string[] name = new string[depth];

            for (int i = 0; i < depth; i++)
            {
                if (!valid)
                {
                    name[i] = propertyNames[i];
                    continue;
                }

                if (!tryGetProperty(parent, propertyNames[i], out var property, stringComparison))
                {
                    if (i == 0)
                    {
                        string? rpn = Global.Game.GetRealPropertyName(recordType, propertyNames[i]);
                        if (rpn is not null)
                        {
                            propertyNames[0] = rpn;

                            // We recall this method as is possible real property name contains a
                            // sub-property which means total depth changes
                            return TryGetProperties(recordType, string.Join('.', propertyNames), out properties, out correctedPropertyName, stringComparison);
                        }
                    }

                    valid = false;
                    name[i] = propertyNames[i];
                    continue;
                }

                if ((i < last) && IgnoreSubPropertiesOnTypes.Contains(property.PropertyType))
                    valid = false;

                properties[i] = property;
                name[i] = property.Name;
                parent = property.PropertyType;
            }

            correctedPropertyName = string.Join('.', name);
            return valid;
        }

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        protected void AddAlias (ILoquiRegistration? recordType, string propertyName, string? realPropertyName)
        {
            if (recordType is not null)
            {
                if (propertyAliases.TryGetValue(new PropertyAliasMapping(null, propertyName, null), out var pam))
                {
                    if (string.Equals(pam.RealPropertyName, realPropertyName, StringComparison.Ordinal))
                        throw new Exception($"Invalid alias that matches null entry. Type: {recordType.Name}. Alias: {propertyName}. Real Name: {realPropertyName}");
                }
            }

            _ = propertyAliases.Add(new PropertyAliasMapping(recordType?.ClassType, propertyName, realPropertyName));
        }

        /// <summary>
        ///     Allow implementing classes to pre-load fixed matches
        /// </summary>
        /// <param name="type">Type action is for. Can not be a generic type definition.</param>
        /// <param name="action">Record action instance.</param>
        /// <exception cref="ArgumentException"></exception>
        protected void addExactMatch (Type type, IRecordAction? action)
        {
            if (type.IsGenericTypeDefinition)
                throw new ArgumentException("Cannot add exact match for a generic type definition.", nameof(type));

            if (TypeMappingsExact.ContainsKey(type))
                Global.Logger.Log(0, $"Exact match for {type.GetClassName()} already exists, overwriting with {action?.GetType().GetClassName() ?? "null"}", logLevel: Microsoft.Extensions.Logging.LogLevel.Trace);

            TypeMappingsExact[type] = action;
        }

        /// <summary>
        ///     Get the action for a given property type
        /// </summary>
        /// <param name="propertyType">Type of the property you need action for.</param>
        /// <returns>Record action class if found else null.</returns>
        protected IRecordAction? discoverAction (Type propertyType)
        {
            propertyType = propertyType.RemoveNullable();

            // Enums and Flags
            if (propertyType.IsEnum)
                return propertyType.GetCustomAttributes(typeof(FlagsAttribute), true).FirstOrDefault() is null ? EnumsAction.Instance : FlagsAction.Instance;

            if (TypeMappingsExact.TryGetValue(propertyType, out var mappedAction))
                return mappedAction;

            var action = discoverAction(propertyType.Explode(3));

            TypeMappingsExact[propertyType] = action; // Cache the discovered type
            return action;
        }

        /// <summary>
        ///     Discover the action type for a given type exploded into 1-3 parts. This is called
        ///     from the base discoverAction method.
        /// </summary>
        /// <param name="explodedType">
        ///     Array of the exploded type (MaxDepth=3).
        ///     <see cref="Common.Extensions.Explode(Type, int)" />
        /// </param>
        /// <returns></returns>
        protected virtual IRecordAction? discoverAction (Type[] explodedType)
        {
            switch (explodedType.Length)
            {
                case 1:
                    if (explodedType[0].IsAssignableTo(typeof(ITranslatedStringGetter)))
                        return ConvertibleAction<string>.Instance;

                    // Unknown type, return null
                    return null;

                case 2:
                    if (explodedType[0].IsAssignableTo(typeof(IFormLink<>)) ||
                        explodedType[0].IsAssignableTo(typeof(IFormLinkGetter<>)) ||
                        explodedType[0].IsAssignableTo(typeof(IFormLinkNullable<>)) ||
                        explodedType[0].IsAssignableTo(typeof(IFormLinkNullableGetter<>))
                        )
                    {
                        return typeof(FormLinkAction<>).MakeGenericType([explodedType[1]]).GetSingletonInstance<IRecordAction>();
                    }

                    // Unknown type, return null
                    return null;

                case 3:
                    if (explodedType[0].IsAssignableTo(typeof(ExtendedList<>)))
                    {
                        if (explodedType[1].IsAssignableTo(typeof(IFormLink<>)) ||
                            explodedType[1].IsAssignableTo(typeof(IFormLinkGetter<>)) ||
                            explodedType[1].IsAssignableTo(typeof(IFormLinkNullable<>)) ||
                            explodedType[1].IsAssignableTo(typeof(IFormLinkNullableGetter<>))
                            )
                        {
                            return typeof(FormLinksAction<>).MakeGenericType([explodedType[2]]).GetSingletonInstance<IRecordAction>();
                        }
                    }

                    // Unknown type, return null
                    return null;

                default:

                    // Unknown type, return null
                    return null;
            }
        }

        protected abstract IEnumerable<ILoquiRegistration> getRecordTypes ();

        private static bool tryGetProperty (Type type, string propertyName, [NotNullWhen(true)] out PropertyInfo? property, StringComparison stringComparison = StringComparison.Ordinal)
        {
            property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property is not null)
                return true;

            if (stringComparison != StringComparison.Ordinal)
            {
                foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (p.Name.Equals(propertyName, stringComparison))
                    {
                        property = p;
                        return true;
                    }
                }
            }

            property = null;
            return false;
        }

        /// <summary>
        ///     Used has key for caching property inputs, to actual property action.
        /// </summary>
        /// <param name="recordType">Record property is for.</param>
        /// <param name="propertyName">Property name as provided by user input.</param>
        private readonly struct PropertyKey (ILoquiRegistration recordType, string propertyName) : IEquatable<PropertyKey>
        {
            public string PropertyName { get; } = propertyName;
            public ILoquiRegistration RecordType { get; } = recordType;

            public static bool operator != (PropertyKey left, PropertyKey right) => !(left == right);

            public static bool operator == (PropertyKey left, PropertyKey right) => left.Equals(right);

            public override bool Equals ([NotNullWhen(true)] object? obj) => obj is PropertyKey op && Equals(op);

            public bool Equals (PropertyKey o) => RecordType == o.RecordType && PropertyName.Equals(o.PropertyName, StringComparison.Ordinal);

            public override readonly int GetHashCode () => HashCode.Combine(RecordType, PropertyName);
        }
    }
}