using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.VisualBasic.Devices;

namespace BlueStacks.Common
{
	public static class SystemUtils
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool IsWow64Process(IntPtr proc, ref bool isWow);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

		public static bool IsOSWinXP()
		{
			return Environment.OSVersion.Version.Major == 5;
		}

		public static bool IsOSVista()
		{
			return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0;
		}

		public static bool IsOSWin7()
		{
			return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
		}

		public static bool IsOSWin8()
		{
			return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2;
		}

		public static bool IsOSWin81()
		{
			return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 3;
		}

		private static bool IsOSWin10()
		{
			return ((string)RegistryUtils.GetRegistryValue("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", "", RegistryKeyKind.HKEY_LOCAL_MACHINE)).Contains("Windows 10");
		}

		public static int GetOSArchitecture()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine);
			if (!string.IsNullOrEmpty(environmentVariable) && string.Compare(environmentVariable, 0, "x86", 0, 3, StringComparison.OrdinalIgnoreCase) != 0)
			{
				return 64;
			}
			return 32;
		}

		public static bool GetOSInfo(out string osName, out string servicePack, out string osArch)
		{
			osName = "";
			servicePack = "";
			osArch = "";
			OperatingSystem osversion = Environment.OSVersion;
			Version version = osversion.Version;
			if (osversion.Platform == PlatformID.Win32Windows)
			{
				int minor = version.Minor;
				if (minor != 0)
				{
					if (minor != 10)
					{
						if (minor == 90)
						{
							osName = "Me";
						}
					}
					else if (version.Revision.ToString(CultureInfo.InvariantCulture) == "2222A")
					{
						osName = "98SE";
					}
					else
					{
						osName = "98";
					}
				}
				else
				{
					osName = "95";
				}
			}
			else if (osversion.Platform == PlatformID.Win32NT)
			{
				switch (version.Major)
				{
				case 3:
					osName = "NT 3.51";
					break;
				case 4:
					osName = "NT 4.0";
					break;
				case 5:
					if (version.Minor == 0)
					{
						osName = "2000";
					}
					else
					{
						osName = "XP";
					}
					break;
				case 6:
					if (version.Minor == 0)
					{
						osName = "Vista";
					}
					else if (version.Minor == 1)
					{
						osName = "7";
					}
					else if (version.Minor == 2)
					{
						osName = "8";
					}
					else if (version.Minor == 3)
					{
						osName = "8.1";
					}
					break;
				case 10:
					osName = "10";
					break;
				}
			}
			string text = osName;
			if (!string.IsNullOrEmpty(text))
			{
				text = "Windows " + text;
				if (!string.IsNullOrEmpty(osversion.ServicePack))
				{
					servicePack = osversion.ServicePack.Substring(osversion.ServicePack.LastIndexOf(' ') + 1);
					text = text + " " + osversion.ServicePack;
				}
				osArch = SystemUtils.GetOSArchitecture().ToString(CultureInfo.InvariantCulture) + "-bit";
				text = text + " " + osArch;
				Logger.Info("Operating system details: " + text);
				return true;
			}
			return false;
		}

		public static ulong GetSystemTotalPhysicalMemory()
		{
			ulong num = 0UL;
			try
			{
				num = ulong.Parse(new ComputerInfo().TotalPhysicalMemory.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't get TotalPhysicalMemory, Ex: {0}", new object[] { ex.Message });
			}
			return num;
		}

		public static bool IsOs64Bit()
		{
			return IntPtr.Size == 8 || (IntPtr.Size == 4 && SystemUtils.Is32BitProcessOn64BitProcessor());
		}

		private static bool Is32BitProcessOn64BitProcessor()
		{
			bool flag = false;
			SystemUtils.IsWow64Process(Process.GetCurrentProcess().Handle, ref flag);
			return flag;
		}

		public static DateTime FromUnixEpochToLocal(long secs)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return dateTime.AddSeconds((double)secs).ToLocalTime();
		}

		public static int CurrentDPI
		{
			get
			{
				if (SystemUtils.currentDPI.Equals(-2147483648))
				{
					SystemUtils.currentDPI = SystemUtils.GetDPI();
				}
				return SystemUtils.currentDPI;
			}
		}

		public static int GetDPI()
		{
			Logger.Info("Getting DPI");
			IntPtr hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
			int num = SystemUtils.GetDeviceCaps(hdc, 88);
			int deviceCaps = SystemUtils.GetDeviceCaps(hdc, 10);
			int deviceCaps2 = SystemUtils.GetDeviceCaps(hdc, 117);
			float num2 = (float)deviceCaps / (float)deviceCaps2;
			num = (int)((float)num * num2);
			Logger.Info("DPI = {0}", new object[] { num });
			return num;
		}

		public static bool IsAdministrator()
		{
			bool flag = false;
			try
			{
				WindowsIdentity current = WindowsIdentity.GetCurrent();
				if (current == null)
				{
					return false;
				}
				flag = new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch
			{
			}
			return flag;
		}

		public static string GetSysInfo(string query)
		{
			int num = 0;
			string text = "";
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementBaseObject managementBaseObject2 = (ManagementObject)managementBaseObject;
						num++;
						foreach (PropertyData propertyData in managementBaseObject2.Properties)
						{
							text = text + propertyData.Value.ToString() + "\n";
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in getting sysinfo err:" + ex.ToString());
			}
			return text.Trim();
		}

		public const int DEFAULT_DPI = 96;

		private static int currentDPI = int.MinValue;

		public enum DeviceCap
		{
			LOGPIXELSX = 88,
			LOGPIXELSY = 90,
			VERTRES = 10,
			DESKTOPVERTRES = 117
		}
	}
}


