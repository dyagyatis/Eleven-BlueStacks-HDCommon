using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class ASTCTextureEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.ASTCTexture;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			ASTCOption astcoption = RegistryManager.Instance.Guest[context.VmName].ASTCOption;
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, astcoption.ToString(), rightOperand, context);
		}
	}
}


