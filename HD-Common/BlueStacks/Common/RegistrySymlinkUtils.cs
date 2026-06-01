using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	public static class RegistrySymlinkUtils
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool IsWow64Process(IntPtr proc, ref bool isWow);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int RegOpenKeyEx(IntPtr hKey, string lpSubKey, uint ulOptions, uint samDesired, ref IntPtr phkResult);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int RegCreateKeyEx(IntPtr hKey, string lpSubKey, uint Reserved, string lpClass, uint dwOptions, uint samDesired, IntPtr lpSecurityAttributes, ref IntPtr phkResult, ref uint lpdwDisposition);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int RegSetValueEx(IntPtr hKey, string lpValueName, uint Reserved, uint dwType, string lpData, int cbData);

		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int ZwDeleteKey(IntPtr hKey);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int RegDeleteValue(IntPtr hKey, string lpValueName);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int RegCloseKey(IntPtr hKey);

		public static bool SymlinkCreator()
		{
			if (RegistrySymlinkUtils.IsOs64Bit())
			{
				try
				{
					RegistrySymlinkUtils.RemoveRegistrySymlink();
				}
				catch (Exception)
				{
				}
				RegistrySymlinkUtils.CreateRegistrySymlink();
			}
			return true;
		}

		public static bool IsOs64Bit()
		{
			return IntPtr.Size == 8 || (IntPtr.Size == 4 && RegistrySymlinkUtils.Is32BitProcessOn64BitProcessor());
		}

		private static bool Is32BitProcessOn64BitProcessor()
		{
			bool flag = false;
			RegistrySymlinkUtils.IsWow64Process(Process.GetCurrentProcess().Handle, ref flag);
			return flag;
		}

		public static void CreateRegistrySymlink()
		{
			IntPtr intPtr = (IntPtr)(-2147483646);
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			uint num = 0U;
			int num2 = RegistrySymlinkUtils.RegOpenKeyEx(intPtr, "Software", 0U, 983359U, ref zero);
			if (num2 != 0)
			{
				throw new ApplicationException("Cannot open 64-bit HKLM\\Software", new Win32Exception(num2));
			}
			try
			{
				num2 = RegistrySymlinkUtils.RegCreateKeyEx(zero, "BlueStacks" + Strings.GetOemTag(), 0U, null, 2U, 983359U, IntPtr.Zero, ref zero2, ref num);
				if (num2 != 0)
				{
					throw new ApplicationException("Cannot create 64-bit registry", new Win32Exception(num2));
				}
				string text = "\\Registry\\Machine\\Software\\Wow6432Node\\BlueStacks" + Strings.GetOemTag();
				num2 = RegistrySymlinkUtils.RegSetValueEx(zero2, "SymbolicLinkValue", 0U, 6U, text, text.Length * 2);
				if (num2 != 0)
				{
					throw new ApplicationException("Cannot set registry symlink value for target" + text, new Win32Exception(num2));
				}
			}
			finally
			{
				RegistrySymlinkUtils.RegCloseKey(zero);
				if (zero2 != IntPtr.Zero)
				{
					RegistrySymlinkUtils.RegCloseKey(zero2);
				}
			}
		}

		public static void RemoveRegistrySymlink()
		{
			IntPtr intPtr = (IntPtr)(-2147483646);
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			int num = RegistrySymlinkUtils.RegOpenKeyEx(intPtr, "Software", 0U, 983359U, ref zero);
			if (num != 0)
			{
				throw new ApplicationException("Cannot open 64-bit HKLM\\Software", new Win32Exception(num));
			}
			try
			{
				num = RegistrySymlinkUtils.RegOpenKeyEx(zero, "BlueStacks" + Strings.GetOemTag(), 8U, 983359U, ref zero2);
				if (num != 0)
				{
					throw new ApplicationException("Cannot open 64-bit registry", new Win32Exception(num));
				}
				num = RegistrySymlinkUtils.RegDeleteValue(zero2, "SymbolicLinkValue");
				num = RegistrySymlinkUtils.ZwDeleteKey(zero2);
			}
			finally
			{
				RegistrySymlinkUtils.RegCloseKey(zero);
				if (zero2 != IntPtr.Zero)
				{
					RegistrySymlinkUtils.RegCloseKey(zero2);
				}
			}
		}

		public static bool IsSymlinkPresent()
		{
			if (RegistrySymlinkUtils.IsOs64Bit())
			{
				try
				{
					IntPtr intPtr = (IntPtr)(-2147483646);
					IntPtr zero = IntPtr.Zero;
					int num = RegistrySymlinkUtils.RegOpenKeyEx(intPtr, "Software", 0U, 257U, ref zero);
					if (num != 0)
					{
						throw new ApplicationException("Cannot open 64-bit HKLM\\Software: 0x" + num.ToString("x", CultureInfo.InvariantCulture));
					}
					num = RegistrySymlinkUtils.RegOpenKeyEx(zero, "BlueStacks" + Strings.GetOemTag(), 8U, 257U, ref zero);
					if (num != 0)
					{
						throw new ApplicationException("Cannot open 64-bit registry: 0x" + num.ToString("x", CultureInfo.InvariantCulture));
					}
					return true;
				}
				catch (Exception ex)
				{
					Logger.Warning("Some error while detecting symlink. Ex: {0}", new object[] { ex.Message });
					return false;
				}
				return false;
			}
			return false;
		}

		private const uint REG_LINK = 6U;

		private const uint HKEY_LOCAL_MACHINE = 2147483650U;

		private const uint REG_OPTION_CREATE_LINK = 2U;

		private const uint REG_OPTION_OPEN_LINK = 8U;

		private const uint KEY_ALL_ACCESS = 983103U;

		private const uint KEY_WOW64_64KEY = 256U;

		private const uint KEY_QUERY_VALUE = 1U;
	}
}


