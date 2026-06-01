using System;
using System.IO;
using Microsoft.Win32;

namespace BlueStacks.Common
{
	public static class MonitorLocator
	{
		public static void Publish(string vmName, uint vmId)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(MonitorLocator.REG_PATH, true);
			foreach (string text in registryKey.GetValueNames())
			{
				if (registryKey.GetValueKind(text) != RegistryValueKind.DWord)
				{
					registryKey.DeleteValue(text);
				}
				else
				{
					uint num = (uint)((int)registryKey.GetValue(text, 0));
					if (vmId == num)
					{
						registryKey.DeleteValue(text);
					}
				}
			}
			registryKey.SetValue(vmName, vmId, RegistryValueKind.DWord);
		}

		public static string[] Fetch()
		{
			return Registry.LocalMachine.OpenSubKey(MonitorLocator.REG_PATH, true).GetValueNames();
		}

		public static uint Lookup(string vmName)
		{
			return (uint)((int)Registry.LocalMachine.OpenSubKey(MonitorLocator.REG_PATH).GetValue(vmName, 0));
		}

		private static readonly string REG_PATH = Path.Combine(RegistryManager.Instance.BaseKeyPath, "Monitors");
	}
}


