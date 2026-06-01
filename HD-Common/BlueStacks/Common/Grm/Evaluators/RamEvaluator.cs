using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class RamEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.Ram;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			int memory = RegistryManager.Instance.Guest[context.VmName].Memory;
			return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, memory, rightOperand, context);
		}
	}
}


