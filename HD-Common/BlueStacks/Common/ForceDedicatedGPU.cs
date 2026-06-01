using System;
using System.IO;

namespace BlueStacks.Common
{
	public static class ForceDedicatedGPU
	{
		public static bool ToggleDedicatedGPU(bool enable, string binPath = null)
		{
			try
			{
				if (binPath == null)
				{
					binPath = Path.Combine(RegistryStrings.InstallDir, "HD-ForceGPU.exe");
				}
				string text = (enable ? "1" : "0");
				return RunCommand.RunCmd(binPath, text, true, true, false, 0).ExitCode == 0;
			}
			catch (Exception ex)
			{
				Logger.Error("An error occured while running {0}, Ex: {1}", new object[] { binPath, ex });
			}
			return false;
		}

		private const string ENABLE_ARG = "1";

		private const string DISABLE_ARG = "0";
	}
}


