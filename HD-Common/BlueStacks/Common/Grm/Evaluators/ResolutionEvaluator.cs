using System;
using System.Globalization;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class ResolutionEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.Resolution;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			int guestWidth = RegistryManager.Instance.Guest[context.VmName].GuestWidth;
			int guestHeight = RegistryManager.Instance.Guest[context.VmName].GuestHeight;
			string text = guestWidth.ToString(CultureInfo.InvariantCulture) + "x" + guestHeight.ToString(CultureInfo.InvariantCulture);
			rightOperand = rightOperand.Replace(" ", string.Empty);
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, text, rightOperand, context);
		}
	}
}


