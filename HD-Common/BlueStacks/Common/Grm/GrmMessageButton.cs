using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common.Grm
{
	public class GrmMessageButton
	{
		[JsonProperty(PropertyName = "buttonColor")]
		public string ButtonColor { get; set; } = "Blue";

		[JsonProperty(PropertyName = "buttonStringKey")]
		public string ButtonStringKey { get; set; } = string.Empty;

		[JsonProperty(PropertyName = "actions")]
		public List<GrmAction> Actions { get; set; } = new List<GrmAction>();
	}
}


