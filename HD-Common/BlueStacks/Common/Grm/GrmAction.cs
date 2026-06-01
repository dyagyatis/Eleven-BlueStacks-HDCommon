using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common.Grm
{
	public class GrmAction
	{
		[JsonProperty(PropertyName = "actionType")]
		public string ActionType { get; set; } = string.Empty;

		[JsonProperty(PropertyName = "actionDictionary")]
		public Dictionary<string, string> ActionDictionary { get; set; } = new Dictionary<string, string>();
	}
}


