using System;
using System.Globalization;
using Microsoft.VisualBasic.Devices;

namespace BlueStacks.Common.Grm.Evaluators
{
	public class PhysicalRamEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.PhysicalRam;
			}
		}

		public static string RAM
		{
			get
			{
				int num = 0;
				try
				{
					num = (int)(new ComputerInfo().TotalPhysicalMemory / 1048576UL);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception when finding ram");
					Logger.Error(ex.ToString());
				}
				return num.ToString(CultureInfo.InvariantCulture);
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			int num;
			int.TryParse(PhysicalRamEvaluator.RAM, out num);
			int num2 = (int)((double)num * 0.5);
			if (num2 >= 4096)
			{
				num2 = 4096;
			}
			if (RegistryManager.Instance.CurrentEngine == "raw" && num2 >= 3072)
			{
				num2 = 3072;
			}
			return GrmComparer<int>.Evaluate(this.EvaluatorForOperandType, grmOperator, num2, rightOperand, context);
		}
	}
}


