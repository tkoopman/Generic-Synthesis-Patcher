using System;

using GenericSynthesisPatcher.Json.Converters;

using Mutagen.Bethesda.Plugins;

using Newtonsoft.Json;

namespace GenericSynthesisPatcher.Json.Data
{
	[JsonConverter(typeof(FilterFormLinksConverter))]
	public class FilterFormLinks
	{
		[JsonIgnore]
		public FormKey FormKey { get; private set; }

		[JsonIgnore]
		public bool Neg {  get; private set; }

        public FilterFormLinks ( string input )
		{
			Neg = input.StartsWith ( '-' );

			if (Neg || input.StartsWith('+'))
                input = input[1..];

			FormKey = FormKey.Factory (FormKeyConverter.FixFormKey(input));
		}
    }
}
