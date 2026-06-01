using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common.Grm
{
	public class GrmRule
	{
		[JsonProperty(PropertyName = "expressions")]
		public List<GrmExpression> Expressions { get; set; } = new List<GrmExpression>();
	}
}


