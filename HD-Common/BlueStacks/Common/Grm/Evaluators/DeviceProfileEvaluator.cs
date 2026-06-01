using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class DeviceProfileEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.DeviceProfile;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			string valueInBootParams = Utils.GetValueInBootParams("pcode", context.VmName, "", "bgp64");
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, valueInBootParams, rightOperand, context);
		}
	}
}


