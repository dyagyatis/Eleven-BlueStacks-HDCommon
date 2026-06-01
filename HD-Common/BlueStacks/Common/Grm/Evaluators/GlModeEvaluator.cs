using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class GlModeEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.GlMode;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			GlMode glModeForVm = Utils.GetGlModeForVm(context.VmName);
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, glModeForVm.ToString(), rightOperand, context);
		}
	}
}


