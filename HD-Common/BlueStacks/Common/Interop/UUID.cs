using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace BlueStacks.Common.Interop
{
	public static class UUID
	{
		public static string GetUserGUID()
		{
			string text = null;
			string registryBaseKeyPath = Strings.RegistryBaseKeyPath;
			RegistryKey registryKey2;
			RegistryKey registryKey = (registryKey2 = Registry.CurrentUser.OpenSubKey(registryBaseKeyPath));
			try
			{
				if (registryKey != null)
				{
					text = (string)registryKey.GetValue("USER_GUID", null);
					if (text != null)
					{
						Logger.Info("TODO: Fix GUID generation. This should not happen. Detected GUID in HKCU: " + text);
						return text;
					}
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
			registryKey = (registryKey2 = Registry.LocalMachine.OpenSubKey(registryBaseKeyPath));
			try
			{
				if (registryKey != null)
				{
					text = (string)registryKey.GetValue("USER_GUID", null);
					if (text != null)
					{
						Logger.Info("TODO: Fix GUID generation. This should not happen. Detected GUID in HKLM: " + text);
						return text;
					}
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
			try
			{
				string text2 = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_Guid_Backup");
				if (File.Exists(text2))
				{
					string text3 = File.ReadAllText(text2);
					if (!string.IsNullOrEmpty(text3))
					{
						text = text3;
						Logger.Info("Detected User GUID %temp%: " + text);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
			return text;
		}

		public static string GetGuidFromBackUp()
		{
			string text = string.Empty;
			if (FeatureManager.Instance.IsGuidBackUpEnable)
			{
				text = UUID.GetUserGUID();
				if (!string.IsNullOrEmpty(text))
				{
					try
					{
						return new Guid(text).ToString();
					}
					catch
					{
						return string.Empty;
					}
				}
			}
			return string.Empty;
		}

		public static string Base64Encode(string plainText)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
		}

		public static string Base64Decode(string base64EncodedData)
		{
			byte[] array = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(array);
		}

		private static string getBluestacksID()
		{
			return "BGP";
		}

		private static RegistryKey _openSubKey(RegistryKey parentKey, string subKeyName, bool writable, UUID.RegWow64Options options)
		{
			if (parentKey == null || UUID._getRegistryKeyHandle(parentKey) == IntPtr.Zero)
			{
				return null;
			}
			int num = 131097;
			if (writable)
			{
				num = 131078;
			}
			int num2;
			if (UUID.RegOpenKeyEx(UUID._getRegistryKeyHandle(parentKey), subKeyName, 0, num | (int)options, out num2) != 0)
			{
				return null;
			}
			return UUID._pointerToRegistryKey((IntPtr)num2, writable, false);
		}

		private static IntPtr _getRegistryKeyHandle(RegistryKey registryKey)
		{
			return ((SafeHandle)typeof(RegistryKey).GetField("hkey", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(registryKey)).DangerousGetHandle();
		}

		private static RegistryKey _pointerToRegistryKey(IntPtr hKey, bool writable, bool ownsHandle)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			Type type = typeof(SafeHandleZeroOrMinusOneIsInvalid).Assembly.GetType("Microsoft.Win32.SafeHandles.SafeRegistryHandle");
			Type[] array = new Type[]
			{
				typeof(IntPtr),
				typeof(bool)
			};
			object obj = type.GetConstructor(bindingFlags, null, array, null).Invoke(new object[] { hKey, ownsHandle });
			Type typeFromHandle = typeof(RegistryKey);
			Type[] array2 = new Type[]
			{
				type,
				typeof(bool)
			};
			return (RegistryKey)typeFromHandle.GetConstructor(bindingFlags, null, array2, null).Invoke(new object[] { obj, writable });
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
		public static extern int RegOpenKeyEx(IntPtr hKey, string subKey, int ulOptions, int samDesired, out int phkResult);

		private enum RegWow64Options
		{
			None,
			KEY_WOW64_64KEY = 256,
			KEY_WOW64_32KEY = 512
		}

		private enum RegistryRights
		{
			ReadKey = 131097,
			WriteKey = 131078
		}
	}
}


