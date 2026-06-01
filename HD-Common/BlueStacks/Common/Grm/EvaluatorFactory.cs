using System;
using BlueStacks.Common.Grm.Evaluators;

namespace BlueStacks.Common.Grm
{
	internal class EvaluatorFactory
	{
		public static IRequirementEvaluator CreateandReturnEvaluator(GrmOperand operand)
		{
			switch (operand)
			{
			case GrmOperand.AppVersionCode:
				return new AppVersionEvaluator();
			case GrmOperand.ProductVersion:
				return new ProductVersionEvaluator();
			case GrmOperand.Geo:
				return new GeoEvaluator();
			case GrmOperand.Gpu:
				return new GpuEvaluator();
			case GrmOperand.Ram:
				return new RamEvaluator();
			case GrmOperand.PhysicalRam:
				return new PhysicalRamEvaluator();
			case GrmOperand.GlMode:
				return new GlModeEvaluator();
			case GrmOperand.EngineMode:
				return new EngineModeEvaluator();
			case GrmOperand.Is64Bit:
				return new Is64BitEvaluator();
			case GrmOperand.CpuCoresAllocated:
				return new CpuCoresAllocatedEvaluator();
			case GrmOperand.PhysicalCoresAvailable:
				return new PhysicalCoresAvailableEvaluator();
			case GrmOperand.Dpi:
				return new DpiEvaluator();
			case GrmOperand.Fps:
				return new FpsEvaluator();
			case GrmOperand.Resolution:
				return new ResolutionEvaluator();
			case GrmOperand.GuestOs:
				return new GuestOsEvaluator();
			case GrmOperand.Oem:
				return new OemEvaluator();
			case GrmOperand.InstalledOems:
				return new InstalledOemEvaluator();
			case GrmOperand.CustomKeyMappingExists:
				return new CustomKeyMappingExistsEvaluator();
			case GrmOperand.RegistryKeyValue:
				return new RegistryKeyValueEvaluator();
			case GrmOperand.BootParam:
				return new BootParamEvaluator();
			case GrmOperand.DeviceProfile:
				return new DeviceProfileEvaluator();
			case GrmOperand.ASTCTexture:
				return new ASTCTextureEvaluator();
			case GrmOperand.ABIMode:
				return new ABIModeEvaluator();
			default:
				return null;
			}
		}
	}
}


