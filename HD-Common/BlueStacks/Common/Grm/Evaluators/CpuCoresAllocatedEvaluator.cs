using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class CpuCoresAllocatedEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.CpuCoresAllocated;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			int vcpus = RegistryManager.Instance.Guest[context.VmName].VCPUs;
			return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, vcpus, rightOperand, context);
		}
	}
}


