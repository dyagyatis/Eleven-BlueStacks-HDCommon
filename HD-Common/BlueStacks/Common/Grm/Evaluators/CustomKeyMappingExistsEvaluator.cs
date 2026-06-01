using System;
using System.IO;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class CustomKeyMappingExistsEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.CustomKeyMappingExists;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			bool flag = File.Exists(Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), context.PackageName + ".cfg"));
			return GrmComparer<bool>.Evaluate(this.EvaluatorForOperandType, grmOperator, flag, rightOperand, context);
		}
	}
}


