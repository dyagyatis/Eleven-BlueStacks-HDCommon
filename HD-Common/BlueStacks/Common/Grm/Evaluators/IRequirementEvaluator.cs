using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal interface IRequirementEvaluator
	{
		GrmOperand EvaluatorForOperandType { get; }

		bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand);
	}
}


