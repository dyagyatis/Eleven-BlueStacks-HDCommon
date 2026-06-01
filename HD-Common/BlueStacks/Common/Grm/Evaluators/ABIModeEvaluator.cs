using System;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class ABIModeEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.ABIMode;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			string text = Utils.GetValueInBootParams("abivalue", context.VmName, "", "bgp64");
			if (string.IsNullOrEmpty(text))
			{
				text = ABISetting.Auto.ToString();
			}
			else if (EngineSettingBaseViewModel.Is64BitABIValuesValid())
			{
				string text2;
				if (text != null)
				{
					if (text == "7")
					{
						text2 = ABISetting.Auto.ToString();
						goto IL_0092;
					}
					if (text == "15")
					{
						text2 = ABISetting.ARM.ToString();
						goto IL_0092;
					}
				}
				text2 = ABISetting.Custom.ToString();
				IL_0092:
				text = text2;
			}
			else
			{
				string text2;
				if (text != null)
				{
					if (text == "15")
					{
						text2 = ABISetting.Auto.ToString();
						goto IL_00E9;
					}
					if (text == "4")
					{
						text2 = ABISetting.ARM.ToString();
						goto IL_00E9;
					}
				}
				text2 = ABISetting.Custom.ToString();
				IL_00E9:
				text = text2;
			}
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, text, rightOperand, context);
		}
	}
}


