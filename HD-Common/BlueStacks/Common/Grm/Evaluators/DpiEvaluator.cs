using System;
using System.Globalization;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class DpiEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.Dpi;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			string dpiFromBootParameters = Utils.GetDpiFromBootParameters(RegistryManager.Instance.Guest[context.VmName].BootParameters);
			return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, Convert.ToInt32(dpiFromBootParameters, CultureInfo.InvariantCulture), rightOperand, context);
		}
	}
}


