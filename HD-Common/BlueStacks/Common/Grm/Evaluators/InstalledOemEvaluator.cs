using System;
using System.Collections.Generic;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class InstalledOemEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.InstalledOems;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			List<string> allInstalledOemList = InstalledOem.AllInstalledOemList;
			return GrmComparer<List<string>>.Evaluate(this.EvaluatorForOperandType, grmOperator, allInstalledOemList, rightOperand, context);
		}
	}
}


