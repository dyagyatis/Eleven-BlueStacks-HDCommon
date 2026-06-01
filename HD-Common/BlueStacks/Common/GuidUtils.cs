using System;
using System.IO;
using Microsoft.Win32;

namespace BlueStacks.Common
{
	public static class GuidUtils
	{
		public static string ReuseOrGenerateMachineId()
		{
			string text = "";
			try
			{
				string blueStacksMachineId = GuidUtils.GetBlueStacksMachineId();
				if (!string.IsNullOrEmpty(blueStacksMachineId))
				{
					return blueStacksMachineId;
				}
				text = Guid.NewGuid().ToString();
				GuidUtils.SetBlueStacksMachineId(text);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't generate/find Machine ID. Ex: {0}", new object[] { ex.Message });
			}
			return text;
		}

		public static string ReuseOrGenerateVersionId()
		{
			string text = "";
			try
			{
				string blueStacksVersionId = GuidUtils.GetBlueStacksVersionId();
				if (!string.IsNullOrEmpty(blueStacksVersionId))
				{
					return blueStacksVersionId;
				}
				text = Guid.NewGuid().ToString();
				GuidUtils.SetBlueStacksVersionId(text);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't generate/find Version ID. Ex: {0}", new object[] { ex.Message });
			}
			return text;
		}

		public static string GetBlueStacksMachineId()
		{
			if (string.IsNullOrEmpty(GuidUtils.sBlueStacksMachineId))
			{
				GuidUtils.sBlueStacksMachineId = StringUtils.GetControlCharFreeString(GuidUtils.GetIdFromRegistryOrFile("MachineID").Trim());
			}
			return GuidUtils.sBlueStacksMachineId;
		}

		public static bool SetBlueStacksMachineId(string newId)
		{
			return GuidUtils.SetIdInRegistryAndFile("MachineID", newId);
		}

		public static string GetBlueStacksVersionId()
		{
			if (string.IsNullOrEmpty(GuidUtils.sBlueStacksVersionId))
			{
				GuidUtils.sBlueStacksVersionId = StringUtils.GetControlCharFreeString(GuidUtils.GetIdFromRegistryOrFile("VersionMachineId_4.220.0.4001").Trim());
			}
			return GuidUtils.sBlueStacksVersionId;
		}

		public static bool SetBlueStacksVersionId(string newId)
		{
			return GuidUtils.SetIdInRegistryAndFile("VersionMachineId_4.220.0.4001", newId);
		}

		public static string GetIdFromRegistryOrFile(string id)
		{
			string text = "";
			text = (string)RegistryUtils.GetRegistryValue("Software\\BlueStacksInstaller", id, "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			try
			{
				string text2 = Path.Combine(new DirectoryInfo(ShortcutHelper.CommonDesktopPath).Parent.FullName, "BlueStacks");
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				string text3 = Path.Combine(text2, id);
				if (File.Exists(text3))
				{
					string text4 = File.ReadAllText(text3);
					if (!string.IsNullOrEmpty(text4))
					{
						text = text4;
					}
				}
			}
			catch
			{
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\BlueStacksInstaller");
			if (registryKey != null)
			{
				text = (string)registryKey.GetValue(id, "");
			}
			return text;
		}

		public static bool SetIdInRegistryAndFile(string id, string value)
		{
			bool flag = false;
			value = ((value != null) ? value.Trim() : null);
			flag = RegistryUtils.SetRegistryValue("Software\\BlueStacksInstaller", id, value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			try
			{
				string text = Path.Combine(new DirectoryInfo(ShortcutHelper.CommonDesktopPath).Parent.FullName, "BlueStacks");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string text2 = Path.Combine(text, id);
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
				File.WriteAllText(text2, value);
				flag = true;
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to write in in file. Error: " + ex.Message);
			}
			try
			{
				Registry.CurrentUser.CreateSubKey("Software\\BlueStacksInstaller").SetValue(id, value);
				flag = true;
			}
			catch (Exception ex2)
			{
				Logger.Warning("Failed to write id in HKCU. Error: " + ex2.Message);
			}
			return flag;
		}

		private static string sBlueStacksMachineId;

		private static string sBlueStacksVersionId;
	}
}


