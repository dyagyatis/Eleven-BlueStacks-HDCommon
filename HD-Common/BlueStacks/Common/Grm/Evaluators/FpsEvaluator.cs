using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class FpsEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.Fps;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			int fps = RegistryManager.Instance.Guest[context.VmName].FPS;
			return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, fps, rightOperand, context);
		}
	}
}


