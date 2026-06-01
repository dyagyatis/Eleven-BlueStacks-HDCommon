using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class PhysicalCoresAvailableEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.PhysicalCoresAvailable;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			int num = 1;
			if (RegistryManager.Instance.CurrentEngine != "raw")
			{
				num = ((Environment.ProcessorCount > 8) ? 8 : Environment.ProcessorCount);
			}
			return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, num, rightOperand, context);
		}
	}
}


