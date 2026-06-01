using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common.Grm
{
	public class GrmRuleSet
	{
		public string RuleId { get; set; }

		public string Description { get; set; }

		[JsonProperty(PropertyName = "rules")]
		public List<GrmRule> Rules { get; set; } = new List<GrmRule>();

		[JsonProperty(PropertyName = "messageWindow")]
		public GrmMessageWindow MessageWindow { get; set; }
	}
}


