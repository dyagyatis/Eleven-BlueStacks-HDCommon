using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;

namespace BlueStacks.Common
{
	public static class RegistryUtils
	{
		[DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegRenameKey(UIntPtr hKey, [MarshalAs(UnmanagedType.LPWStr)] string oldname, [MarshalAs(UnmanagedType.LPWStr)] string newname);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegOpenKeyEx(UIntPtr hKey, string subKey, int ulOptions, RegSAM samDesired, out UIntPtr hkResult);

		public static RegistryKey InitKeyWithSecurityCheck(string keyName)
		{
			if (!SystemUtils.IsAdministrator())
			{
				return Registry.LocalMachine.OpenSubKey(keyName);
			}
			return Registry.LocalMachine.CreateSubKey(keyName);
		}

		public static RegistryKey InitKey(string keyName)
		{
			RegistryKey registryKey;
			try
			{
				registryKey = Registry.LocalMachine.CreateSubKey(keyName);
			}
			catch
			{
				registryKey = null;
			}
			return registryKey;
		}

		public static void DeleteKey(string hklmPath, bool throwOnError = true)
		{
			try
			{
				Registry.LocalMachine.DeleteSubKeyTree(hklmPath);
			}
			catch
			{
				if (throwOnError)
				{
					throw;
				}
			}
		}

		public static object GetRegistryValue(string registryPath, string key, object defaultValue, RegistryKeyKind registryKind = RegistryKeyKind.HKEY_LOCAL_MACHINE)
		{
			RegistryKey registryKey = null;
			object obj = defaultValue;
			if (registryKind != RegistryKeyKind.HKEY_LOCAL_MACHINE)
			{
				if (registryKind == RegistryKeyKind.HKEY_CURRENT_USER)
				{
					registryKey = Registry.CurrentUser.OpenSubKey(registryPath);
				}
			}
			else
			{
				registryKey = Registry.LocalMachine.OpenSubKey(registryPath);
			}
			if (registryKey != null)
			{
				obj = registryKey.GetValue(key, defaultValue);
				registryKey.Close();
			}
			return obj;
		}

		public static bool SetRegistryValue(string registryPath, string key, object value, RegistryValueKind type, RegistryKeyKind registryKind = RegistryKeyKind.HKEY_LOCAL_MACHINE)
		{
			RegistryKey registryKey = null;
			bool flag = true;
			try
			{
				if (registryKind != RegistryKeyKind.HKEY_LOCAL_MACHINE)
				{
					if (registryKind == RegistryKeyKind.HKEY_CURRENT_USER)
					{
						registryKey = Registry.CurrentUser.CreateSubKey(registryPath);
					}
				}
				else
				{
					registryKey = Registry.LocalMachine.CreateSubKey(registryPath);
				}
				if (registryKey != null)
				{
					registryKey.SetValue(key, value, type);
				}
			}
			catch
			{
				Logger.Warning("Failed to set registry value at {0} for {1}:{2}", new object[] { registryPath, key, value });
				flag = false;
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			return flag;
		}

		public static int RenameKey(string basePath, string oldName, string newName, bool deleteNewIfExist)
		{
			if (deleteNewIfExist)
			{
				try
				{
					Registry.LocalMachine.DeleteSubKeyTree(Path.Combine(basePath, newName));
				}
				catch (Exception ex)
				{
					Logger.Warning("Couldn't delete new subkeytree: {0}\\{1}, ex: {2}", new object[] { basePath, newName, ex.Message });
				}
			}
			UIntPtr uintPtr;
			int num = RegistryUtils.RegOpenKeyEx(RegistryUtils.HKEY_LOCAL_MACHINE, basePath, 0, RegSAM.Write, out uintPtr);
			if (num == 0)
			{
				num = RegistryUtils.RegRenameKey(uintPtr, oldName, newName);
			}
			return num;
		}

		public static void GrantAllAccessPermission(RegistryKey rk)
		{
			object obj = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)) as NTAccount;
			RegistrySecurity registrySecurity = new RegistrySecurity();
			RegistryAccessRule registryAccessRule = new RegistryAccessRule(obj.ToString(), RegistryRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
			registrySecurity.AddAccessRule(registryAccessRule);
			if (rk != null)
			{
				rk.SetAccessControl(registrySecurity);
			}
		}

		public static void MoveUnifiedInstallerRegistryFromWow64()
		{
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Strings.RegistryBaseKeyPath);
				if (registryKey == null || string.IsNullOrEmpty((string)registryKey.GetValue("Version", null)))
				{
					RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("Software", true);
					RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("Software\\WOW6432Node\\BlueStacks" + Strings.GetOemTag());
					if (registryKey3 != null)
					{
						RegistryKey registryKey4 = registryKey2.CreateSubKey("BlueStacks" + Strings.GetOemTag());
						RegistryUtils.RecurseCopyKey(registryKey3, registryKey4);
						registryKey2.DeleteSubKeyTree("WOW6432Node\\BlueStacks" + Strings.GetOemTag());
						RegistryUtils.GrantAllAccessPermission(registryKey4);
					}
				}
			}
			catch
			{
			}
		}

		private static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
		{
			foreach (string text in sourceKey.GetValueNames())
			{
				object value = sourceKey.GetValue(text);
				RegistryValueKind valueKind = sourceKey.GetValueKind(text);
				destinationKey.SetValue(text, value, valueKind);
			}
			foreach (string text2 in sourceKey.GetSubKeyNames())
			{
				RegistryKey registryKey = sourceKey.OpenSubKey(text2);
				RegistryKey registryKey2 = destinationKey.CreateSubKey(text2);
				RegistryUtils.RecurseCopyKey(registryKey, registryKey2);
			}
		}

		public static readonly UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(2147483650U);

		public static readonly UIntPtr HKEY_CURRENT_USER = new UIntPtr(2147483649U);
	}
}


