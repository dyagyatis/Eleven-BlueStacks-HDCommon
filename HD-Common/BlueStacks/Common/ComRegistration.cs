using System;
using System.Diagnostics;
using System.IO;

namespace BlueStacks.Common
{
	public static class ComRegistration
	{
		public static int Register()
		{
			Logger.Info("Registering COM components");
			return ComRegistration.RunBinary("-reg");
		}

		public static int Unregister()
		{
			Logger.Info("Unregistering COM components");
			return ComRegistration.RunBinary("-unreg");
		}

		private static int RunBinary(string args)
		{
			Process process = new Process();
			process.StartInfo.FileName = ComRegistration.BIN_PATH;
			process.StartInfo.Arguments = args;
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.Verb = "runas";
			process.Start();
			process.WaitForExit();
			return process.ExitCode;
		}

		private static string BIN_PATH = Path.Combine(RegistryStrings.InstallDir, "HD-ComRegistrar.exe");
	}
}


