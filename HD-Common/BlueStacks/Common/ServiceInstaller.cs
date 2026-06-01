using System;
using System.Diagnostics;
using System.IO;

namespace BlueStacks.Common
{
	public static class ServiceInstaller
	{
		public static int ReinstallService()
		{
			string text = "-reinstall -oem bgp64";
			Process process = ProcessUtils.StartExe(ServiceInstaller.BinaryName, text, true);
			process.WaitForExit();
			return process.ExitCode;
		}

		private static string BinaryName = Path.Combine(RegistryStrings.InstallDir, "HD-ServiceInstaller.exe");
	}
}


