using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class GeoEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.Geo;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			string geo = RegistryManager.Instance.Geo;
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, geo, rightOperand, context);
		}
	}
}


