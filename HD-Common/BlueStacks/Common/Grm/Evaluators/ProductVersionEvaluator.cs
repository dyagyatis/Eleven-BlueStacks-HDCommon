using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class ProductVersionEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.ProductVersion;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			Version version = new Version(RegistryManager.Instance.Version);
			return GrmComparer<Version>.Evaluate(this.EvaluatorForOperandType, grmOperator, version, rightOperand, context);
		}
	}
}


