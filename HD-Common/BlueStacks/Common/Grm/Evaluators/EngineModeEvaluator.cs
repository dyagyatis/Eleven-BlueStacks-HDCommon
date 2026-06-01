using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class EngineModeEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.EngineMode;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			EngineState engineState = EngineState.plus;
			if (RegistryManager.Instance.CurrentEngine == "raw")
			{
				engineState = EngineState.raw;
			}
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, engineState.ToString(), rightOperand, context);
		}
	}
}


