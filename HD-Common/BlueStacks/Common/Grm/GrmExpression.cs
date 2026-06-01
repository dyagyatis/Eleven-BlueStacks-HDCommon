using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common.Grm
{
	public class GrmExpression
	{
		[JsonProperty(PropertyName = "leftOperand")]
		public string LeftOperand { get; set; } = string.Empty;

		[JsonProperty(PropertyName = "operator")]
		public string Operator { get; set; } = string.Empty;

		[JsonProperty(PropertyName = "rightOperand")]
		public string RightOperand { get; set; } = string.Empty;

		[JsonProperty(PropertyName = "contextJson")]
		public string ContextJson { get; set; } = string.Empty;

		public bool EvaluateExpression(GrmRuleSetContext context)
		{
			bool flag;
			try
			{
				if (context != null)
				{
					context.ContextJson = this.ContextJson;
				}
				GrmOperand grmOperand = (GrmOperand)Enum.Parse(typeof(GrmOperand), this.LeftOperand, true);
				GrmOperator grmOperator = (GrmOperator)Enum.Parse(typeof(GrmOperator), this.Operator, true);
				flag = EvaluatorFactory.CreateandReturnEvaluator(grmOperand).Evaluate(context, grmOperator, this.RightOperand);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while parsing operand for grmrule. operand: {0} operator: {1} rulesetid:{2} exception: {3}", new object[]
				{
					this.LeftOperand,
					this.Operator,
					(context != null) ? context.RuleSetId : null,
					ex.Message
				});
				if (!GrmExpression._rulesetsWithException.Contains((context != null) ? context.RuleSetId : null))
				{
					GrmExpression._rulesetsWithException.Add((context != null) ? context.RuleSetId : null);
					Stats.SendMiscellaneousStatsAsync("grm_evaluation_error", RegistryManager.Instance.UserGuid, (context != null) ? context.RuleSetId : null, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "bgp64", (context != null) ? context.PackageName : null, ex.Message, null, (context != null) ? context.VmName : null, 0);
				}
				flag = false;
			}
			return flag;
		}

		private static List<string> _rulesetsWithException = new List<string>();
	}
}


