using System;
using System.Collections.Generic;

namespace BlueStacks.Common.Grm.Evaluators
{
	internal class GpuEvaluator : IRequirementEvaluator
	{
		public GrmOperand EvaluatorForOperandType
		{
			get
			{
				return GrmOperand.Gpu;
			}
		}

		public bool Evaluate(GrmRuleSetContext context, GrmOperator grmOperator, string rightOperand)
		{
			if (!GpuEvaluator.mVmNameGpu.ContainsKey(context.VmName) || string.IsNullOrEmpty(GpuEvaluator.mVmNameGpu[context.VmName]))
			{
				string text;
				string text2;
				string text3;
				Utils.GetCurrentGraphicsInfo(RegistryManager.Instance.Guest[context.VmName].GlRenderMode.ToString() + " " + RegistryManager.Instance.Guest[context.VmName].GlMode.ToString(), out text, out text2, out text3);
				string text4 = text + " " + text2;
				GpuEvaluator.mVmNameGpu[context.VmName] = text4;
				Logger.Info("GpuEvaluator " + text4);
			}
			return GrmComparer<string>.Evaluate(this.EvaluatorForOperandType, grmOperator, GpuEvaluator.mVmNameGpu[context.VmName], rightOperand, context);
		}

		private static Dictionary<string, string> mVmNameGpu = new Dictionary<string, string>();
	}
}


