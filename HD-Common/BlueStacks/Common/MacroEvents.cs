using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	[Serializable]
	public class MacroEvents
	{
		[JsonProperty("Timestamp", NullValueHandling = NullValueHandling.Ignore)]
		public long Timestamp { get; set; }

		[JsonExtensionData]
		public IDictionary<string, object> ExtraData { get; set; }
	}
}


