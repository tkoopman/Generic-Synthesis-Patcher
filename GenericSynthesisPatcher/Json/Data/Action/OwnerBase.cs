using GenericSynthesisPatcher.Json.Converters;

using Mutagen.Bethesda.Skyrim;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data.Action
{
    [JsonConverter(typeof(OwnerBaseConverter))]
    public abstract class OwnerBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Readability")]
        public static bool Equals (OwnerBase? l, OwnerBase? r)
        {
            if (ReferenceEquals(l, r))
                return true;
            if (l == null || r == null)
                return false;

            if (l is FactionOwner lf && r is FactionOwner rf)
                return FactionOwner.Equals(lf, rf);

            if (l is NpcOwner ln && r is NpcOwner rn)
                return NpcOwner.Equals(ln, rn);

            return false;
        }

        public abstract OwnerTarget ToActionData ();
    }
}