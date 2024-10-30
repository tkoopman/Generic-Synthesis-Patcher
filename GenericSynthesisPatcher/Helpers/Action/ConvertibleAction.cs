using System.Text.RegularExpressions;

using GenericSynthesisPatcher.Json.Operations;

using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace GenericSynthesisPatcher.Helpers.Action
{
    public class ConvertibleAction<T> : BasicAction<T> where T : IConvertible
    {
        public new static readonly ConvertibleAction<T> Instance = new();
        private const int ClassLogCode = 0x10;

        private ConvertibleAction () : base()
        {
        }

        public override bool CanMatch () => true;

        public override bool MatchesRule (ProcessingKeys proKeys)
        {
            var values = proKeys.GetMatchValueAs<List<ListOperation<T>>>();

            if (!values.SafeAny())
                return true;

            if (!Mod.GetProperty<T>(proKeys.Record, proKeys.Property.PropertyName, out var curValue) || curValue == null)
                return !values.Any(k => k.Operation != ListLogic.NOT);

            foreach (var v in values)
            {
                if (v is string str && str.StartsWith('/') && str.EndsWith('/') && curValue is string curStr)
                {
                    if (new Regex(str.Trim('/'), RegexOptions.IgnoreCase).IsMatch(curStr))
                        return v.Operation != ListLogic.NOT;
                }
                else if (curValue.Equals(v.Value))
                {
                    if (v.Operation == ListLogic.NOT)
                        return v.Operation != ListLogic.NOT;
                }
            }

            return !values.Any(k => k.Operation != ListLogic.NOT);
        }
    }
}