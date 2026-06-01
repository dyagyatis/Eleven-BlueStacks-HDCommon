using System;
using System.Globalization;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class AppVersionEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.AppVersionCode;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			int num = int.Parse(new JsonParser(context.VmName).GetAppInfoFromPackageName(context.PackageName).Version, CultureInfo.InvariantCulture);
			return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, num, rightOperand, context);
		}
	}
}


