using System.Reflection;

using Loqui;

using Microsoft.Extensions.Logging;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class DeepCopyAction<T> : BasicAction<T>
        where T : ILoquiObject
    {
        public new static readonly DeepCopyAction<T> Instance = new();
        private const int ClassLogCode = 0x1C;

        private DeepCopyAction () : base()
        {
        }

        protected override int Fill (ProcessingKeys proKeys, T? curValue, T? newValue)
        {
            if (Equals(curValue, newValue))
                return 0;

            if (typeof(T).GetProperty("StaticRegistration")?.GetValue(null) is not ILoquiRegistration reg)
            {
                Global.Logger.Log(ClassLogCode, "Static Registration for Deep Copy not found", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            var skyrim = Assembly.GetAssembly(typeof(Mutagen.Bethesda.Skyrim.AcousticSpaceMixIn));
            var mixIn = skyrim?.GetType($"{reg.ClassType}MixIn");

            var deepCopyIn = mixIn?.GetMethod("DeepCopyIn", BindingFlags.Public | BindingFlags.Static, [reg.SetterType, reg.GetterType]);
            if (deepCopyIn == null)
            {
                Global.Logger.Log(ClassLogCode, "No DeepCopyIn method found", logLevel: LogLevel.Error, propertyName: proKeys.Property.PropertyName);
                return -1;
            }

            if (!Mod.TryGetPropertyForEditing<T>(proKeys.GetPatchRecord(), proKeys.Property.PropertyName, out var editValue))
                return -1;

            _ = deepCopyIn.Invoke(null, [editValue, newValue]);
            Global.DebugLogger?.Log(ClassLogCode, "Changed via deep copy.", propertyName: proKeys.Property.PropertyName);
            return 1;
        }
    }
}