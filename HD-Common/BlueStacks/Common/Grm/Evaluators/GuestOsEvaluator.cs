using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class GuestOsEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.GuestOs;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			GuestOS guestOS = GuestOS.Nougat;
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, guestOS.ToString(), rightOperand, context);
		}
	}
}


