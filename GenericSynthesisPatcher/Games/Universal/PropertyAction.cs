using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using GenericSynthesisPatcher.Games.Universal.Action;

using Loqui;

namespace GenericSynthesisPatcher.Games.Universal
{
    /// <summary>
    ///     Stores property information for sharing between classes.
    /// </summary>
    public readonly struct PropertyAction
    {
        private readonly IRecordAction? action;
        private readonly ILoquiRegistration? recordType;

        /// <param name="recordType">Record property is for.</param>
        /// <param name="propertyName">Real property name</param>
        /// <param name="action">Record action to use or null if no valid action found.</param>
        public PropertyAction (ILoquiRegistration? recordType, PropertyInfo[] properties, string propertyName, IRecordAction? action)
        {
            bool valid = properties.Length != 0 && properties[^1] is not null;

            if (action is not null && (recordType is null || !valid))
                throw new ArgumentException("Action is not null so recordType and properties must be specified.", nameof(properties));

            this.action = action;
            Properties = properties;
            PropertyName = propertyName;
            this.recordType = recordType;
        }

        public IRecordAction Action => action ?? throw new InvalidOperationException("Invalid Property so no Action");

        [MemberNotNullWhen(true, nameof(Action))]
        [MemberNotNullWhen(true, nameof(RecordType))]
        public bool IsValid => action is not null;

        public PropertyInfo[] Properties { get; }
        public string PropertyName { get; }
        public ILoquiRegistration RecordType => recordType ?? throw new InvalidOperationException("Invalid Property so no RecordType");
    }
}