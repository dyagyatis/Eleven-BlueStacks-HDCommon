using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common.Grm
{
	public class GrmMessageWindow
	{
		[JsonProperty(PropertyName = "messageType")]
		public string MessageType { get; set; } = "None";

		[JsonProperty(PropertyName = "headerStringKey")]
		public string HeaderStringKey { get; set; } = string.Empty;

		[JsonProperty(PropertyName = "messageStringKey")]
		public string MessageStringKey { get; set; } = string.Empty;

		[JsonProperty(PropertyName = "dontShowOption")]
		public bool DontShowOption { get; set; }

		[JsonProperty(PropertyName = "buttons")]
		public List<GrmMessageButton> Buttons { get; set; } = new List<GrmMessageButton>();
	}
}


