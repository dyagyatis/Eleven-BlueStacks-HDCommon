using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class Is64BitEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.Is64Bit;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			bool is64BitOperatingSystem = CommonInstallUtils.is64BitOperatingSystem;
			return GrmComparer<bool>.Evaluate(this.EvaluatorForOperandType, grmOperator, is64BitOperatingSystem, rightOperand, context);
		}
	}
}


