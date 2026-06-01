using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

namespace BlueStacks.Common
{
	public static class Profile
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool IsWow64Process(IntPtr proc, ref bool isWow);

		public static string GlVendor { get; set; } = "";

		public static string GlRenderer { get; set; } = "";

		public static string GlVersion { get; set; } = "";

		private static bool IsOs64Bit()
		{
			return IntPtr.Size == 8 || (IntPtr.Size == 4 && Profile.Is32BitProcessOn64BitProcessor());
		}

		private static bool Is32BitProcessOn64BitProcessor()
		{
			bool flag = false;
			Profile.IsWow64Process(Process.GetCurrentProcess().Handle, ref flag);
			return flag;
		}

		public static Dictionary<string, string> Info()
		{
			if (Profile.s_Info != null)
			{
				return Profile.s_Info;
			}
			string text = "Android";
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"ProcessorId",
					SystemUtils.GetSysInfo("Select processorID from Win32_Processor")
				},
				{
					"Processor",
					Profile.CPU
				}
			};
			string sysInfo = SystemUtils.GetSysInfo("Select NumberOfLogicalProcessors from Win32_Processor");
			Logger.Info("the length of numOfProcessor string is {0}", new object[] { sysInfo.Length.ToString(CultureInfo.InvariantCulture) });
			dictionary.Add("NumberOfProcessors", sysInfo);
			dictionary.Add("GPU", Profile.GPU);
			dictionary.Add("GPUDriver", SystemUtils.GetSysInfo("Select DriverVersion from Win32_VideoController"));
			dictionary.Add("OS", Profile.OS);
			string text2 = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
			{
				Environment.OSVersion.Version.Major,
				Environment.OSVersion.Version.Minor
			});
			dictionary.Add("OSVersion", text2);
			dictionary.Add("RAM", Profile.RAM);
			try
			{
				string version = RegistryManager.Instance.Version;
				dictionary.Add("BlueStacksVersion", version);
			}
			catch
			{
			}
			int num;
			try
			{
				num = RegistryManager.Instance.Guest[text].GlMode;
			}
			catch
			{
				num = -1;
			}
			dictionary.Add("GlMode", num.ToString(CultureInfo.InvariantCulture));
			int num2;
			try
			{
				num2 = RegistryManager.Instance.Guest[text].GlRenderMode;
			}
			catch
			{
				num2 = -1;
			}
			dictionary.Add("GlRenderMode", num2.ToString(CultureInfo.InvariantCulture));
			string text3 = "";
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\OEMInformation");
				string text4 = (string)registryKey.GetValue("Manufacturer", "");
				string text5 = (string)registryKey.GetValue("Model", "");
				text3 = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { text4, text5 });
			}
			catch
			{
			}
			dictionary.Add("OEMInfo", text3);
			int num3 = Screen.PrimaryScreen.Bounds.Width;
			int num4 = Screen.PrimaryScreen.Bounds.Height;
			dictionary.Add("ScreenResolution", num3.ToString(CultureInfo.InvariantCulture) + "x" + num4.ToString(CultureInfo.InvariantCulture));
			try
			{
				num3 = RegistryManager.Instance.Guest[text].WindowWidth;
				num4 = RegistryManager.Instance.Guest[text].WindowHeight;
				dictionary.Add("BlueStacksResolution", num3.ToString(CultureInfo.InvariantCulture) + "x" + num4.ToString(CultureInfo.InvariantCulture));
			}
			catch
			{
			}
			string text6 = "";
			try
			{
				RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP");
				foreach (string text7 in registryKey2.GetSubKeyNames())
				{
					if (text7.StartsWith("v", StringComparison.OrdinalIgnoreCase))
					{
						RegistryKey registryKey3 = registryKey2.OpenSubKey(text7);
						if (registryKey3.GetValue("Install") != null && (int)registryKey3.GetValue("Install") == 1)
						{
							text6 = (string)registryKey3.GetValue("Version");
						}
						if (text7 == "v4")
						{
							RegistryKey registryKey4 = registryKey3.OpenSubKey("Client");
							if (registryKey4 != null && (int)registryKey4.GetValue("Install") == 1)
							{
								text6 = (string)registryKey4.GetValue("Version") + " Client";
							}
							registryKey4 = registryKey3.OpenSubKey("Full");
							if (registryKey4 != null && (int)registryKey4.GetValue("Install") == 1)
							{
								text6 = (string)registryKey4.GetValue("Version") + " Full";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Got exception when checking dot net version,err: {0}", new object[] { ex.ToString() });
			}
			dictionary.Add("DotNetVersion", text6);
			if (Profile.IsOs64Bit())
			{
				dictionary.Add("OSVERSIONTYPE", "64 bit");
			}
			else
			{
				dictionary.Add("OSVERSIONTYPE", "32 bit");
			}
			Profile.s_Info = dictionary;
			return Profile.s_Info;
		}

		public static Dictionary<string, string> InfoForGraphicsDriverCheck()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"os_version",
					SystemUtils.GetSysInfo("Select Caption from Win32_OperatingSystem")
				},
				{
					"os_arch",
					SystemUtils.GetSysInfo("Select OSArchitecture from Win32_OperatingSystem")
				},
				{
					"processor_vendor",
					SystemUtils.GetSysInfo("Select Manufacturer from Win32_Processor")
				},
				{
					"processor",
					SystemUtils.GetSysInfo("Select Name from Win32_Processor")
				}
			};
			string text = SystemUtils.GetSysInfo("Select Caption from Win32_VideoController");
			string text2 = "";
			string[] array = text.Split(new string[]
			{
				Environment.NewLine,
				"\r\n",
				"\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			if (!string.IsNullOrEmpty(text))
			{
				foreach (string text3 in array)
				{
					text2 = text2 + text3.Substring(0, text3.IndexOf(" ", StringComparison.OrdinalIgnoreCase)) + "\r\n";
				}
				text2 = text2.Trim();
			}
			string text4 = SystemUtils.GetSysInfo("Select DriverVersion from Win32_VideoController");
			string text5 = SystemUtils.GetSysInfo("Select DriverDate from Win32_VideoController");
			string[] array3 = text2.Split(new string[]
			{
				Environment.NewLine,
				"\r\n",
				"\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array4 = text4.Split(new string[]
			{
				Environment.NewLine,
				"\r\n",
				"\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array5 = text5.Split(new string[]
			{
				Environment.NewLine,
				"\r\n",
				"\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] == Profile.GlRenderer || Profile.GlVendor.Contains(array3[j]))
				{
					text = array[j];
					text2 = array3[j];
					text4 = array4[j];
					text5 = array5[j];
					break;
				}
			}
			dictionary.Add("gpu", text);
			dictionary.Add("gpu_vendor", text2);
			dictionary.Add("driver_version", text4);
			dictionary.Add("driver_date", text5);
			string text6 = "";
			RegistryKey registryKey2;
			RegistryKey registryKey = (registryKey2 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\OEMInformation"));
			try
			{
				if (registryKey != null)
				{
					text6 = (string)registryKey.GetValue("Manufacturer", "");
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
			dictionary.Add("oem_manufacturer", text6);
			string text7 = "";
			registryKey = (registryKey2 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\OEMInformation"));
			try
			{
				if (registryKey != null)
				{
					text7 = (string)registryKey.GetValue("Model", "");
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
			dictionary.Add("oem_model", text7);
			dictionary.Add("bst_oem", RegistryManager.Instance.Oem);
			return dictionary;
		}

		private static string ToUpper(string id)
		{
			return id.ToUpperInvariant();
		}

		public static ulong TotalPhysicalMemory
		{
			get
			{
				if (Profile.mTotalPhysicalMemory == 0UL)
				{
					try
					{
						Profile.mTotalPhysicalMemory = ulong.Parse(new ComputerInfo().TotalPhysicalMemory.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					}
					catch (Exception ex)
					{
						Logger.Error("Couldn't get TotalPhysicalMemory, Ex: {0}", new object[] { ex.Message });
					}
				}
				return Profile.mTotalPhysicalMemory;
			}
		}

		public static int NumberOfLogicalProcessors
		{
			get
			{
				if (Profile.mNumberOfLogicalProcessors == 0)
				{
					try
					{
						int.TryParse(SystemUtils.GetSysInfo("Select NumberOfLogicalProcessors from Win32_Processor"), out Profile.mNumberOfLogicalProcessors);
					}
					catch (Exception ex)
					{
						Logger.Error("Couldn't get NumberOfLogicalProcessors, Ex: {0}", new object[] { ex.Message });
					}
				}
				return Profile.mNumberOfLogicalProcessors;
			}
		}

		public static string RAM
		{
			get
			{
				int num = 0;
				try
				{
					num = (int)(Convert.ToUInt64(SystemUtils.GetSysInfo("Select TotalPhysicalMemory from Win32_ComputerSystem"), CultureInfo.InvariantCulture) / 1048576UL);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception when finding ram");
					Logger.Error(ex.ToString());
				}
				return num.ToString(CultureInfo.InvariantCulture);
			}
		}

		public static string CPU
		{
			get
			{
				return SystemUtils.GetSysInfo("Select Name from Win32_Processor");
			}
		}

		public static string GPU
		{
			get
			{
				return SystemUtils.GetSysInfo("Select Caption from Win32_VideoController");
			}
		}

		public static string OS
		{
			get
			{
				if (string.IsNullOrEmpty(Profile.sOS))
				{
					Profile.sOS = SystemUtils.GetSysInfo("Select Caption from Win32_OperatingSystem");
				}
				return Profile.sOS;
			}
		}

		private static Dictionary<string, string> s_Info;

		private static ulong mTotalPhysicalMemory = 0UL;

		private static int mNumberOfLogicalProcessors = 0;

		private static string sOS = "";
	}
}


