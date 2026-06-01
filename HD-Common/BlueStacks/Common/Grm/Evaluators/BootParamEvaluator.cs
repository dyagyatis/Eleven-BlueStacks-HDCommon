using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class BootParamEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.BootParam;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			JObject jobject = JsonConvert.DeserializeObject(context.ContextJson, Utils.GetSerializerSettings()) as JObject;
			if (jobject == null || !jobject.ContainsKey("param"))
			{
				throw new ArgumentException("BootParamEvaluator requires contextjson with param key.");
			}
			string valueInBootParams = Utils.GetValueInBootParams(jobject["param"].Value<string>(), context.VmName, "", "bgp64");
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, valueInBootParams, rightOperand, context);
		}
	}
}


