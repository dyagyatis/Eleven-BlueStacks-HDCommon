using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class OemEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.Oem;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, "bgp64", rightOperand, context);
		}
	}
}


