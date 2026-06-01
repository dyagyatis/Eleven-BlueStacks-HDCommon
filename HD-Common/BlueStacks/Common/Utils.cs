using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BlueStacks.Common.Grm;
using BlueStacks.Common.Interop;
using BlueStacks.VBoxUtils;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BlueStacks.Common
{
	public static class Utils
	{
		[DllImport("urlmon.dll", CharSet = CharSet.Auto)]
		private static extern uint FindMimeFromData(uint pBC, [MarshalAs(UnmanagedType.LPStr)] string pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer, uint cbSize, [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed, uint dwMimeFlags, out uint ppwzMimeOut, uint dwReserverd);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

		[DllImport("user32.dll")]
		private static extern IntPtr GetKeyboardLayout(uint thread);

		[DllImport("user32.dll")]
		private static extern int GetSystemMetrics(int smIndex);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern int SetupDiGetClassDevs(ref Guid lpGuid, IntPtr Enumerator, IntPtr hwndParent, ClassDevsFlags Flags);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern int SetupDiGetClassDevs(IntPtr guid, IntPtr Enumerator, IntPtr hwndParent, ClassDevsFlags Flags);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern int SetupDiEnumDeviceInfo(int DeviceInfoSet, int Index, ref SP_DEVINFO_DATA DeviceInfoData);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern int SetupDiEnumDeviceInterfaces(int DeviceInfoSet, int DeviceInfoData, ref Guid lpHidGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern int SetupDiGetDeviceInterfaceDetail(int DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData, IntPtr aPtr, int detailSize, ref int requiredSize, IntPtr bPtr);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern int SetupDiGetDeviceInterfaceDetail(int DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData, ref PSP_DEVICE_INTERFACE_DETAIL_DATA myPSP_DEVICE_INTERFACE_DETAIL_DATA, int detailSize, ref int requiredSize, IntPtr bPtr);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern int SetupDiGetDeviceRegistryProperty(int DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, RegPropertyType Property, IntPtr PropertyRegDataType, IntPtr PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern int SetupDiGetDeviceRegistryProperty(int DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, RegPropertyType Property, IntPtr PropertyRegDataType, ref DATA_BUFFER PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);

		public static bool IsTargetForShortcut(string shortcutPath, string targetPath)
		{
			try
			{
				if (File.Exists(shortcutPath))
				{
					string shortcutArguments = ShortcutHelper.GetShortcutArguments(shortcutPath);
					string text = ((shortcutArguments != null) ? shortcutArguments.ToLower(CultureInfo.InvariantCulture).Trim() : null);
					targetPath = ((targetPath != null) ? targetPath.ToLower(CultureInfo.InvariantCulture).Trim() : null);
					if (text.Contains(targetPath) && string.Compare(text, Path.Combine(RegistryStrings.InstallDir, targetPath), StringComparison.OrdinalIgnoreCase) == 0)
					{
						Logger.Info("{0} is a shortcut for target {1}", new object[] { shortcutPath, targetPath });
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Ignoring exception while comparing TargetForShortcut: " + ex.Message);
			}
			return false;
		}

		public static bool IsShortcutArgumentContainsPackage(string shortcutPath, string packageName)
		{
			try
			{
				if (File.Exists(shortcutPath))
				{
					ShellLink shellLink = new ShellLink();
					((IPersistFile)shellLink).Load(shortcutPath, 0);
					StringBuilder stringBuilder = new StringBuilder(1000);
					((IShellLink)shellLink).GetArguments(stringBuilder, stringBuilder.Capacity);
					if (stringBuilder.ToString().ToLower(CultureInfo.InvariantCulture).Contains((packageName != null) ? packageName.ToLower(CultureInfo.InvariantCulture) : null))
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Ignoring exception " + ex.ToString());
			}
			return false;
		}

		public static void NotifyBootFailureToParentWindow(string className, string windowName, int exitCode, string vmName)
		{
			Logger.Info("Sending BOOT_FAILURE message to class = {0}, window = {1}", new object[] { className, windowName });
			IntPtr intPtr = InteropWindow.FindWindow(className, windowName);
			try
			{
				if (intPtr == IntPtr.Zero)
				{
					Logger.Info("Unable to find window : {0}", new object[] { className });
				}
				else
				{
					uint num;
					if (vmName == "Android")
					{
						num = 0U;
					}
					else
					{
						num = (uint)int.Parse((vmName != null) ? vmName.Split(new char[] { '_' })[1] : null, CultureInfo.InvariantCulture);
					}
					Logger.Info("Sending wparam : {0} and lparam : {1}", new object[]
					{
						(uint)exitCode,
						num
					});
					InteropWindow.SendMessage(intPtr, 1037U, (IntPtr)((long)((ulong)exitCode)), (IntPtr)((long)((ulong)num)));
					Logger.Info("Sent BOOT_FAILURE message");
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format(CultureInfo.InvariantCulture, "Error Occured, Err: {0}", new object[] { ex.ToString() }));
			}
		}

		public static bool IsDesktopPC()
		{
			return Utils.GetSystemMetrics(86) == 0;
		}

		public static bool CopyRecursive(string srcPath, string dstPath)
		{
			bool flag = true;
			try
			{
				Logger.Info("Copying {0} to {1}", new object[] { srcPath, dstPath });
				if (!Directory.Exists(dstPath))
				{
					Directory.CreateDirectory(dstPath);
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(srcPath);
				foreach (FileInfo fileInfo in directoryInfo.GetFiles())
				{
					fileInfo.CopyTo(Path.Combine(dstPath, fileInfo.Name), true);
				}
				foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
				{
					if (!Utils.CopyRecursive(Path.Combine(srcPath, directoryInfo2.Name), Path.Combine(dstPath, directoryInfo2.Name)))
					{
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Ignoring exception in copy recursive src:{0} dst:{1}, exception:{2}", new object[] { srcPath, dstPath, ex.Message });
				flag = false;
			}
			return flag;
		}

		public static bool IsNumberWithinRange(int number, int lowerLimit, int upperLimit, bool includeLowerLimit = true)
		{
			if (includeLowerLimit)
			{
				return number >= lowerLimit && number < upperLimit;
			}
			return number > lowerLimit && number < upperLimit;
		}

		public static int GetAndroidVMMemory(bool defaultInstance = true)
		{
			Logger.Info("Getting Android VM Memory");
			ulong num = 1048576UL;
			int num2 = 0;
			int num3 = 600;
			int num4 = 3072;
			int num5 = 4096;
			int num6 = 5120;
			int num7 = 6144;
			int num8 = 8192;
			try
			{
				int num9 = (int)(SystemUtils.GetSystemTotalPhysicalMemory() / num);
				Logger.Info("Total RAM = {0} MB", new object[] { num9 });
				if (SystemUtils.IsOs64Bit())
				{
					if (defaultInstance)
					{
						if (num9 < num4)
						{
							num2 = num3;
						}
						else if (Utils.IsNumberWithinRange(num9, num4, num5, true))
						{
							num2 = 900;
						}
						else if (Utils.IsNumberWithinRange(num9, num5, num6, true))
						{
							num2 = 1200;
						}
						else if (Utils.IsNumberWithinRange(num9, num6, num7, true))
						{
							num2 = 1500;
						}
						else if (Utils.IsNumberWithinRange(num9, num7, num8, true))
						{
							num2 = 1800;
						}
						else
						{
							num2 = 2048;
						}
					}
					else if (num9 > 4000)
					{
						num2 = 1024;
					}
					else
					{
						num2 = 800;
					}
				}
				else if (num9 < num4 || !defaultInstance)
				{
					num2 = num3;
				}
				else
				{
					num2 = 900;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to check physical memory. Err: " + ex.ToString());
				num2 = 1200;
			}
			Logger.Info("Using RAM: {0}MB", new object[] { num2 });
			return num2;
		}

		public static void KillComServerSafe()
		{
			Logger.Info("KillComServerSafe()");
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_Process WHERE Name = 'BstkSVC.exe'")))
			{
				if (managementObjectSearcher.Get().Count != 0)
				{
					Logger.Info("Found BstkSVC. Waiting for it to exit automatically...");
					Thread.Sleep(5000);
				}
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					string text = "Considering ";
					object obj = managementObject["ProcessId"];
					string text2 = ((obj != null) ? obj.ToString() : null);
					string text3 = " -> ";
					object obj2 = managementObject["ExecutablePath"];
					Logger.Info(text + text2 + text3 + ((obj2 != null) ? obj2.ToString() : null));
					Process processById = Process.GetProcessById((int)((uint)managementObject["ProcessId"]));
					string text4 = (string)managementObject["ExecutablePath"];
					if (!string.IsNullOrEmpty(text4))
					{
						string text5 = Directory.GetParent(text4).ToString();
						string installDir = RegistryStrings.InstallDir;
						if (string.Compare(Path.GetFullPath(installDir).TrimEnd(new char[] { '\\' }), Path.GetFullPath(text5).TrimEnd(new char[] { '\\' }), StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							Logger.Info("process BstkSVC not killed since the process Dir:{0} and Ignore Dir:{1} are same", new object[] { text5, installDir });
							continue;
						}
					}
					Logger.Info("Trying to kill BstkSvc PID " + processById.Id.ToString());
					processById.Kill();
					if (!processById.WaitForExit(10000))
					{
						Logger.Info("Timeout waiting for process to die");
					}
				}
			}
		}

		public static bool CheckIfGuestReady(string vmName, int retries)
		{
			if (!Utils.sIsGuestReady.ContainsKey(vmName))
			{
				Utils.sIsGuestReady.Add(vmName, false);
			}
			if (!Utils.sIsGuestReady[vmName] && retries > 0)
			{
				while (retries > 0)
				{
					retries--;
					try
					{
						if (JObject.Parse(HTTPUtils.SendRequestToGuest("checkIfGuestReady", null, vmName, 1000, null, false, 1, 0, "bgp64"))["result"].ToString().Equals("ok", StringComparison.OrdinalIgnoreCase))
						{
							Logger.Info("Guest is ready");
							Utils.sIsGuestReady[vmName] = true;
							return Utils.sIsGuestReady[vmName];
						}
						Thread.Sleep(1000);
					}
					catch (Exception)
					{
						Thread.Sleep(1000);
					}
				}
				Logger.Error("Guest is not ready now after all retries");
			}
			return Utils.sIsGuestReady[vmName];
		}

		public static List<string> GetRunningInstancesList()
		{
			List<string> list = new List<string>();
			foreach (string text in RegistryManager.Instance.VmList)
			{
				if (Utils.IsAndroidPlayerRunning(text, "bgp64"))
				{
					list.Add(text);
				}
			}
			return list;
		}

		private static bool CheckIfAndroidService(string serviceName)
		{
			return Regex.IsMatch(serviceName, "[bB]st[hH]d(Plus{1})?Android(_\\d+)?Svc");
		}

		public static string GetUserAgent(string oem = "bgp64")
		{
			if (string.IsNullOrEmpty(oem))
			{
				oem = "bgp64";
			}
			string text = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2} gzip", new object[]
			{
				"BlueStacks",
				"4.220.0.4001",
				RegistryManager.RegistryManagers[oem].UserGuid
			});
			Logger.Debug("UserAgent = " + text);
			byte[] bytes = Encoding.Default.GetBytes(text);
			return Encoding.UTF8.GetString(bytes);
		}

		public static Process StartHiddenFrontend(string vmName, string oem = "bgp64")
		{
			if (string.Equals(oem, "dmm", StringComparison.InvariantCultureIgnoreCase))
			{
				string text = vmName + " -h";
				return ProcessUtils.StartExe(RegistryManager.Instance.PartnerExePath, text, false);
			}
			Process process;
			try
			{
				string text2 = Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, "HD-Player.exe");
				process = new Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.FileName = text2;
				process.StartInfo.Arguments = vmName + " -h";
				Logger.Info("Sending vmName for vm calling {0}", new object[] { vmName });
				Logger.Info("Utils: Starting hidden Frontend");
				process.Start();
			}
			catch (Exception ex)
			{
				process = null;
				Logger.Error("Error starting process" + ex.ToString());
			}
			return process;
		}

		public static Process StartFrontend(string vmName)
		{
			string text = Path.Combine(RegistryStrings.InstallDir, "HD-RunApp.exe");
			Process process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = text;
			process.StartInfo.Arguments = "-vmname:" + vmName;
			Logger.Info("Utils: Starting Frontend");
			process.Start();
			return process;
		}

		public static string GetMD5HashFromFile(string fileName)
		{
			try
			{
				return new _MD5
				{
					ValueAsFile = fileName
				}.FingerPrint.ToLower(CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				Logger.Error("Error in creating md5 hash: " + ex.ToString());
			}
			return string.Empty;
		}

		public static bool IsCheckSumValid(string md5Compare, string filePath)
		{
			Logger.Info("Checking for valid checksum");
			string md5HashFromFile = Utils.GetMD5HashFromFile(filePath);
			Logger.Info("Computed MD5: " + md5HashFromFile);
			return string.Equals(md5Compare, md5HashFromFile, StringComparison.OrdinalIgnoreCase);
		}

		public static string GetSystemFontName()
		{
			string text;
			try
			{
				using (new Font("Arial", 8f, FontStyle.Regular, GraphicsUnit.Point, 0))
				{
					text = "Arial";
				}
			}
			catch (Exception)
			{
				using (Label label = new Label())
				{
					try
					{
						using (new Font(label.Font.Name, 8f, FontStyle.Regular, GraphicsUnit.Point, 0))
						{
						}
					}
					catch (Exception)
					{
						if (Oem.Instance.IsMessageBoxToBeDisplayed)
						{
							MessageBox.Show("Failed to load Font set.", string.Format(CultureInfo.InvariantCulture, "{0} instance failed.", new object[] { Strings.ProductDisplayName }), MessageBoxButtons.OK, MessageBoxIcon.Hand);
						}
						Environment.Exit(-1);
					}
					text = label.Font.Name;
				}
			}
			return text;
		}

		public static bool IsBlueStacksInstalled()
		{
			return !string.IsNullOrEmpty(RegistryManager.Instance.Version);
		}

		public static string GetLogoFile()
		{
			return Path.Combine(RegistryStrings.InstallDir, "ProductLogo.ico");
		}

		public static void AddUploadTextToImage(string inputImage, string outputImage)
		{
			global::System.Drawing.Image image = global::System.Drawing.Image.FromFile(inputImage);
			int width = image.Width;
			int num = image.Height + 100;
			using (Bitmap bitmap = new Bitmap(width, num))
			{
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
				global::System.Drawing.Image image2 = global::System.Drawing.Image.FromFile(Utils.GetLogoFile());
				graphics.DrawImage(image2, new Rectangle(65, image.Height, 40, 40), new Rectangle(80, 0, image2.Width, 40), GraphicsUnit.Pixel);
				using (SolidBrush solidBrush = new SolidBrush(Color.White))
				{
					float num2 = (float)image.Width;
					float num3 = 80f;
					RectangleF rectangleF = new RectangleF(120f, (float)(image.Height + 7), num2, num3);
					using (Pen pen = new Pen(Color.Black))
					{
						graphics.DrawRectangle(pen, 120f, (float)image.Height, num2, num3);
						string snapShotShareString = Oem.Instance.SnapShotShareString;
						using (Font font = new Font("Arial", 14f))
						{
							graphics.DrawString(snapShotShareString, font, solidBrush, rectangleF);
							graphics.Save();
							image.Dispose();
							bitmap.Save(outputImage, ImageFormat.Jpeg);
						}
					}
				}
			}
		}

		public static CmdRes RunCmd(string prog, string args, string outPath)
		{
			try
			{
				return Utils.RunCmdInternal(prog, args, outPath, true, false);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
			return new CmdRes();
		}

		public static string RunCmdNoLog(string prog, string args, int timeout)
		{
			string output2;
			using (Process process = new Process())
			{
				new CmdRes();
				string output = "";
				process.StartInfo.FileName = prog;
				process.StartInfo.Arguments = args;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.OutputDataReceived += delegate(object obj, DataReceivedEventArgs line)
				{
					string text = line.Data;
					if (text != null && !string.IsNullOrEmpty(text = text.Trim()))
					{
						output = output + text + "\n";
					}
				};
				process.Start();
				process.BeginOutputReadLine();
				process.WaitForExit(timeout);
				output2 = output;
			}
			return output2;
		}

		private static CmdRes RunCmdInternal(string prog, string args, string outPath, bool enableLog, bool append = false)
		{
			CmdRes res4;
			using (StreamWriter writer = (string.IsNullOrEmpty(outPath) ? null : new StreamWriter(outPath, append)))
			{
				using (Process proc = new Process())
				{
					Logger.Info("Running Command");
					Logger.Info("    prog: " + prog);
					Logger.Info("    args: " + args);
					Logger.Info("    out:  " + outPath);
					CmdRes res = new CmdRes();
					proc.StartInfo.FileName = prog;
					proc.StartInfo.Arguments = args;
					proc.StartInfo.UseShellExecute = false;
					proc.StartInfo.CreateNoWindow = true;
					proc.StartInfo.RedirectStandardInput = true;
					proc.StartInfo.RedirectStandardOutput = true;
					proc.StartInfo.RedirectStandardError = true;
					proc.OutputDataReceived += delegate(object obj, DataReceivedEventArgs line)
					{
						if (outPath != null)
						{
							writer.WriteLine(line.Data);
						}
						string text = line.Data;
						if (text != null && !string.IsNullOrEmpty(text = text.Trim()))
						{
							if (enableLog)
							{
								Logger.Info(proc.Id.ToString() + " OUT: " + text);
							}
							CmdRes res2 = res;
							res2.StdOut = res2.StdOut + text + "\n";
						}
					};
					proc.ErrorDataReceived += delegate(object obj, DataReceivedEventArgs line)
					{
						if (outPath != null)
						{
							writer.WriteLine(line.Data);
						}
						if (enableLog)
						{
							Logger.Error(proc.Id.ToString() + " ERR: " + line.Data);
						}
						CmdRes res3 = res;
						res3.StdErr = res3.StdErr + line.Data + "\n";
					};
					proc.Start();
					proc.BeginOutputReadLine();
					proc.BeginErrorReadLine();
					proc.WaitForExit();
					res.ExitCode = proc.ExitCode;
					if (enableLog)
					{
						Logger.Info(proc.Id.ToString() + " ExitCode: " + proc.ExitCode.ToString());
					}
					if (outPath != null)
					{
						writer.Close();
					}
					res4 = res;
				}
			}
			return res4;
		}

		public static void RunCmdAsync(string prog, string args)
		{
			try
			{
				Utils.RunCmdAsyncInternal(prog, args);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}

		private static void RunCmdAsyncInternal(string prog, string args)
		{
			Process process = new Process();
			Logger.Info("Running Command Async");
			Logger.Info("    prog: " + prog);
			Logger.Info("    args: " + args);
			process.StartInfo.FileName = prog;
			process.StartInfo.Arguments = args;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
		}

		public static string GetPartnerExecutablePath()
		{
			string text = RegistryManager.Instance.PartnerExePath;
			if (string.IsNullOrEmpty(text))
			{
				text = Path.Combine(RegistryStrings.InstallDir, "BlueStacks.exe");
			}
			return text;
		}

		public static Process StartPartnerExe(string vm = "Android")
		{
			Process process = new Process();
			process.StartInfo.FileName = Utils.GetPartnerExecutablePath();
			process.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-vmName={0}", new object[] { vm });
			process.StartInfo.UseShellExecute = false;
			process.Start();
			return process;
		}

		public static bool RestartBlueStacks()
		{
			MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Application restart required to use internet on {0}", new object[] { Strings.ProductDisplayName }), Strings.ProductDisplayName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			return true;
		}

		public static void GetGuestWidthAndHeight(int sWidth, int sHeight, out int width, out int height)
		{
			if (Oem.Instance.OEM.Equals("yoozoo", StringComparison.OrdinalIgnoreCase))
			{
				width = 1280;
				height = 720;
				return;
			}
			if (sWidth > 1920 && sHeight > 1080)
			{
				width = 1920;
				height = 1080;
				return;
			}
			if (sWidth < 1280 && sHeight < 720)
			{
				width = 1280;
				height = 720;
				return;
			}
			width = sWidth;
			height = sHeight;
		}

		public static void GetWindowWidthAndHeight(out int width, out int height)
		{
			int width2 = Screen.PrimaryScreen.Bounds.Width;
			int height2 = Screen.PrimaryScreen.Bounds.Height;
			if (width2 > 1920 && height2 > 1080)
			{
				width = 1920;
				height = 1080;
				return;
			}
			if (width2 > 1600 && height2 > 900)
			{
				width = 1600;
				height = 900;
				return;
			}
			if (width2 > 1280 && height2 > 720)
			{
				width = 1280;
				height = 720;
				return;
			}
			width = 960;
			height = 540;
		}

		public static void AddMessagingSupport(out Dictionary<string, string[]> oemWindowMapper)
		{
			oemWindowMapper = new Dictionary<string, string[]>();
			if (!string.IsNullOrEmpty(Oem.Instance.MsgWindowClassName) || !string.IsNullOrEmpty(Oem.Instance.MsgWindowTitle))
			{
				string[] array = new string[]
				{
					Oem.Instance.MsgWindowClassName,
					Oem.Instance.MsgWindowTitle
				};
				oemWindowMapper.Add(Oem.Instance.OEM, array);
			}
		}

		public static bool IsUIProcessAlive(string vmName, string oem = "bgp64")
		{
			return ProcessUtils.IsAlreadyRunning(Strings.GetPlayerLockName(vmName, oem)) || ProcessUtils.IsAlreadyRunning(Strings.GetBlueStacksUILockNameOem(oem));
		}

		public static bool IsAllUIProcessAlive(string vmName)
		{
			return ProcessUtils.IsAlreadyRunning(Strings.GetPlayerLockName(vmName, "bgp64")) && ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lockbgp64");
		}

		public static bool IsAndroidPlayerRunning(string vmName, string oem = "bgp64")
		{
			return ProcessUtils.IsAlreadyRunning(Strings.GetPlayerLockName(vmName, oem));
		}

		public static bool IsFileNullOrMissing(string file)
		{
			if (!File.Exists(file))
			{
				Logger.Info(file + " does not exist");
				return true;
			}
			if (new FileInfo(file).Length == 0L)
			{
				Logger.Info(file + " is null");
				return true;
			}
			return false;
		}

		public static string GetUserGUID()
		{
			string text = null;
			string text2 = "Software\\\\BlueStacks";
			Logger.Info("Checking if guid present in HKCU");
			RegistryKey registryKey2;
			RegistryKey registryKey = (registryKey2 = Registry.CurrentUser.OpenSubKey(text2));
			try
			{
				if (registryKey != null)
				{
					text = (string)registryKey.GetValue("USER_GUID", null);
					if (text != null)
					{
						Logger.Info("Detected GUID in HKCU: " + text);
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
			Logger.Info("Checking if guid present in HKLM");
			registryKey = (registryKey2 = Registry.LocalMachine.OpenSubKey(text2));
			try
			{
				if (registryKey != null)
				{
					text = (string)registryKey.GetValue("USER_GUID", null);
					if (text != null)
					{
						Logger.Info("Detected User GUID in HKLM: " + text);
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
				Logger.Info("Checking if guid present in %temp%");
				string environmentVariable = Environment.GetEnvironmentVariable("TEMP");
				Logger.Info("%TEMP% = " + environmentVariable);
				string text3 = Path.Combine(environmentVariable, "Bst_Guid_Backup");
				if (File.Exists(text3))
				{
					string text4 = File.ReadAllText(text3);
					if (!string.IsNullOrEmpty(text4))
					{
						text = text4;
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

		private static string GetOldPCode()
		{
			string text = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_PCode_Backup");
			string text2 = "";
			if (File.Exists(text))
			{
				text2 = File.ReadAllText(text);
				if (!string.IsNullOrEmpty(text2))
				{
					Logger.Info(string.Format(CultureInfo.InvariantCulture, "Old PCode = {0}", new object[] { text2 }));
				}
				try
				{
					File.Delete(text);
				}
				catch (Exception ex)
				{
					Logger.Info(string.Format(CultureInfo.InvariantCulture, "Ignoring Error Occured, Err: {0}", new object[] { ex.ToString() }));
				}
			}
			return text2;
		}

		private static bool IsCACodeValid(string caCode)
		{
			string[] array = new string[] { "4", "20", "5", "14", "8", "2", "9", "36" };
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Compare(array[i], caCode, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return false;
				}
			}
			return true;
		}

		private static string GetOldCaCode()
		{
			string text = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_CaCode_Backup");
			string text2 = "";
			if (File.Exists(text))
			{
				Logger.Info("the ca code temp file exists");
				text2 = File.ReadAllText(text);
				if (!string.IsNullOrEmpty(text2))
				{
					Logger.Info(string.Format(CultureInfo.InvariantCulture, "Old CaCode = {0}", new object[] { text2 }));
				}
				try
				{
					File.Delete(text);
				}
				catch (Exception ex)
				{
					Logger.Warning(string.Format(CultureInfo.InvariantCulture, "Error Occured, Err: {0}", new object[] { ex.ToString() }));
				}
			}
			if (!Utils.IsCACodeValid(text2))
			{
				text2 = "";
			}
			return text2;
		}

		private static string GetOldCaSelector()
		{
			string text = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_CaSelector_Backup");
			string text2 = "";
			if (File.Exists(text))
			{
				Logger.Info("the ca selector temp file exists");
				text2 = File.ReadAllText(text);
				if (!string.IsNullOrEmpty(text2))
				{
					Logger.Info(string.Format(CultureInfo.InvariantCulture, "Old CaSelector = {0}", new object[] { text2 }));
				}
				try
				{
					File.Delete(text);
				}
				catch (Exception ex)
				{
					Logger.Warning(string.Format(CultureInfo.InvariantCulture, "Error Occured, Err: {0}", new object[] { ex.ToString() }));
				}
			}
			return text2;
		}

		private static string GetRandomPCode()
		{
			string[] array = new string[] { "madw", "mtox", "optr", "pxln", "ofpn", "snpe", "segn", "ptxg" };
			int num = new Random().Next(array.Length);
			return array[num];
		}

		public static JObject JSonResponseFromCloud(string locale, string vmName, string campaignHash, string guid)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				RegistryManager.Instance.Host,
				"api/getcacode"
			});
			if (string.IsNullOrEmpty(guid))
			{
				guid = RegistryManager.Instance.UserGuid;
				campaignHash = RegistryManager.Instance.CampaignMD5;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "locale", locale },
				{ "guid", guid },
				{ "campaign_hash", campaignHash }
			};
			string text2 = "";
			try
			{
				text2 = BstHttpClient.Post(text, dictionary, null, false, vmName, 0, 1, 0, false, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("An error occured while fetching info from cloud...Err : " + ex.ToString());
			}
			Logger.Info("Got resp: " + text2);
			return JObject.Parse(text2);
		}

		public static void GetCodesAndCountryInfo(out string code, out string pcode, out string country, out string caSelector, out string noChangesDroidG, out string pcodeFromCloud, out bool isCacodeValid, out string DNS, out string DNS2, out string abivalue, out string memAllocator, string locale, string upgradeDetected, string vmName, string campaignHash = "", string guid = "")
		{
			code = "";
			pcode = "";
			country = "";
			caSelector = "";
			abivalue = "15";
			memAllocator = string.Empty;
			noChangesDroidG = "";
			pcodeFromCloud = "";
			DNS = "";
			DNS2 = "";
			isCacodeValid = false;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software");
			string text = (string)registryKey.GetValue("BstTestCA", "");
			string text2 = (string)registryKey.GetValue("BstTestPCode", "");
			string text3 = (string)registryKey.GetValue("BstTestCaSelector", "");
			string text4 = (string)registryKey.GetValue("BstTestNoChangesDroidG", "");
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				Logger.Info("Using test CA/P codes");
				code = text;
				pcode = text2;
				caSelector = text3;
				noChangesDroidG = text4;
			}
			else
			{
				string oldPCode = Utils.GetOldPCode();
				string oldCaCode = Utils.GetOldCaCode();
				string oldCaSelector = Utils.GetOldCaSelector();
				if (!string.IsNullOrEmpty(oldCaCode))
				{
					Logger.Info("The CaCode taken from temp file");
					code = oldCaCode;
					pcode = oldPCode;
					caSelector = oldCaSelector;
					if (!Oem.Instance.IsLoadCACodeFromCloud)
					{
						goto IL_04D4;
					}
					Logger.Info("noChangesDroidG requested from cloud");
					try
					{
						JObject jobject = Utils.JSonResponseFromCloud(locale, vmName, campaignHash, guid);
						if (jobject["success"].ToString().Trim() == "true")
						{
							if (jobject.ToString().Contains("p_code"))
							{
								pcodeFromCloud = jobject["p_code"].ToString().Trim();
							}
							if (jobject.ToString().Contains("no_changes_droidg"))
							{
								noChangesDroidG = jobject["no_changes_droidg"].ToString().Trim();
							}
							if (string.IsNullOrEmpty(caSelector) && string.IsNullOrEmpty(upgradeDetected) && jobject.ToString().Contains("ca_selector"))
							{
								caSelector = jobject["ca_selector"].ToString().Trim();
							}
							if (jobject.ContainsKey("is_valid_code"))
							{
								isCacodeValid = jobject["is_valid_code"].ToObject<bool>();
							}
							if (jobject.ContainsKey("dns"))
							{
								DNS = jobject["dns"].ToObject<string>();
							}
							if (jobject.ContainsKey("dns2"))
							{
								DNS = jobject["dns2"].ToObject<string>();
							}
							if (jobject.ContainsKey("abi_value"))
							{
								abivalue = jobject["abi_value"].ToObject<string>();
							}
							if (jobject.ContainsKey("malloc_value"))
							{
								memAllocator = jobject["malloc_value"].ToObject<string>();
							}
						}
						goto IL_04D4;
					}
					catch (Exception ex)
					{
						Logger.Error(ex.Message);
						goto IL_04D4;
					}
				}
				if (Oem.Instance.IsLoadCACodeFromCloud)
				{
					try
					{
						JObject jobject2 = Utils.JSonResponseFromCloud(locale, vmName, campaignHash, guid);
						if (jobject2["success"].ToString().Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase))
						{
							code = jobject2["code"].ToString().Trim();
							if (jobject2.ToString().Contains("p_code"))
							{
								pcodeFromCloud = jobject2["p_code"].ToString().Trim();
							}
							if (string.IsNullOrEmpty(upgradeDetected))
							{
								pcode = pcodeFromCloud;
							}
							else
							{
								pcode = "";
							}
							if (jobject2.ToString().Contains("ca_selector"))
							{
								caSelector = jobject2["ca_selector"].ToString().Trim();
							}
							if (jobject2.ToString().Contains("no_changes_droidg"))
							{
								noChangesDroidG = jobject2["no_changes_droidg"].ToString().Trim();
							}
							if (jobject2.ContainsKey("is_valid_code"))
							{
								isCacodeValid = jobject2["is_valid_code"].ToObject<bool>();
							}
							if (jobject2.ContainsKey("abi_value"))
							{
								abivalue = jobject2["abi_value"].ToObject<string>();
							}
							if (jobject2.ContainsKey("malloc_value"))
							{
								memAllocator = jobject2["malloc_value"].ToObject<string>();
							}
						}
						else
						{
							pcode = "ofpn";
							code = "840";
							caSelector = "se_310260";
							Logger.Info("Setting default pcode = {0} cacode = {1} caselector = {2} ", new object[] { pcode, code, caSelector });
						}
						goto IL_04D4;
					}
					catch (Exception ex2)
					{
						Logger.Error("Failed to get cacode, pcode etc from cloud");
						Logger.Error(ex2.Message);
						pcode = "ofpn";
						code = "840";
						caSelector = "se_310260";
						Logger.Info("Setting default pcode = {0} cacode = {1} caselector = {2} ", new object[] { pcode, code, caSelector });
						goto IL_04D4;
					}
				}
				if (string.IsNullOrEmpty(upgradeDetected))
				{
					pcode = Utils.GetRandomPCode();
				}
				else
				{
					pcode = "";
				}
				code = "156";
				Logger.Info("cacode = {0} and pcode = {1}", new object[] { code, pcode });
			}
			IL_04D4:
			if (Oem.Instance.IsCountryChina)
			{
				country = "CN";
				caSelector = "se_46000";
				return;
			}
			country = Utils.GetUserCountry(vmName);
		}

		public static bool IsAndroidFeatureBitEnabled(uint featureBit, string vmName)
		{
			try
			{
				string bootParameters = RegistryManager.Instance.Guest[vmName].BootParameters;
				uint num = 0U;
				string[] array = bootParameters.Split(new char[] { ' ' });
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[] { '=' });
					if (array2[0] == "OEMFEATURES")
					{
						num = Convert.ToUInt32(array2[1], CultureInfo.InvariantCulture);
						break;
					}
				}
				Logger.Info("the android oem feature bits are" + num.ToString(CultureInfo.InvariantCulture));
				if ((num & featureBit) == 0U)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Got error while checking for android bit, err:{0}", new object[] { ex.ToString() });
				return false;
			}
			return true;
		}

		public static void SetImeSelectedInReg(string imeSelected, string vmName)
		{
			RegistryManager.Instance.Guest[vmName].ImeSelected = imeSelected;
		}

		public static bool IsLatinImeSelected(string vmName)
		{
			string text = RegistryManager.Instance.Guest[vmName].ImeSelected;
			if (text.Equals("com.android.inputmethod.latin/.LatinIME", StringComparison.CurrentCultureIgnoreCase))
			{
				Logger.Info("LatinIme is selected");
				return true;
			}
			if (string.IsNullOrEmpty(text))
			{
				try
				{
					Logger.Info("IME selected in registry is null, query currentImeId");
					string text2 = HTTPUtils.SendRequestToGuest("getCurrentIMEID", null, vmName, 5000, null, false, 1, 0, "bgp64");
					Logger.Debug("Response: {0}", new object[] { text2 });
					text = JObject.Parse(text2)["currentIme"].ToString();
					Logger.Info("The currentIme: {0}", new object[] { text });
					if (text.Equals("com.android.inputmethod.latin/.LatinIME", StringComparison.CurrentCultureIgnoreCase))
					{
						Utils.SetImeSelectedInReg(text, vmName);
						return true;
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Got exception in checking CurrentImeSelected, ex : {0}", new object[] { ex.ToString() });
				}
				return false;
			}
			return false;
		}

		public static bool IsForcePcImeForLang(string locale)
		{
			if (locale != null && locale.Equals("vi-VN", StringComparison.OrdinalIgnoreCase))
			{
				Logger.Info("the system locale is vi-vn, using pcime workflow");
				return true;
			}
			return false;
		}

		public static bool IsEastAsianLanguage(string lang)
		{
			return new List<string> { "zh-CN", "ja-JP", "ko-KR" }.Contains(lang);
		}

		public static bool WaitForSyncConfig(string vmName)
		{
			int i = 240;
			while (i > 0)
			{
				i--;
				if (RegistryManager.Instance.Guest[vmName].ConfigSynced != 0)
				{
					Logger.Info("Config is synced now");
					return true;
				}
				Logger.Info("Config not sycned, wait 1 second and try again");
				Thread.Sleep(1000);
			}
			return false;
		}

		public static bool WaitForFrontendPingResponse(string vmName)
		{
			Logger.Info("In method WaitForFrontendPingResponse for vmName: " + vmName);
			int i = 50;
			while (i > 0)
			{
				i--;
				try
				{
					string text = HTTPUtils.SendRequestToEngine("pingVm", null, vmName, 1000, null, false, 1, 0, "");
					Logger.Debug("Response: " + text);
					if ((JArray.Parse(text)[0] as JObject)["success"].ToObject<bool>())
					{
						Logger.Info("Frontend server running");
						return true;
					}
					Thread.Sleep(1000);
				}
				catch (Exception)
				{
					Thread.Sleep(1000);
				}
			}
			Logger.Error("Frontend server not running after {0} retries", new object[] { i });
			return false;
		}

		public static bool WaitForAgentPingResponse(string vmName, string oem = "bgp64")
		{
			Logger.Info("In WaitForAgentPingResponse");
			int i = 50;
			while (i > 0)
			{
				i--;
				try
				{
					if ((JArray.Parse(HTTPUtils.SendRequestToAgent("ping", null, vmName, 1000, null, false, 1, 0, oem, false))[0] as JObject)["success"].ToObject<bool>())
					{
						Logger.Info("Agent server running");
						return true;
					}
					Thread.Sleep(200);
				}
				catch (Exception)
				{
					Thread.Sleep(200);
					if (i <= 40 && !ProcessUtils.IsLockInUse("Global\\BlueStacks_HDAgent_Lockbgp64"))
					{
						return false;
					}
				}
			}
			Logger.Info("Agent server not running after {0} retries", new object[] { i });
			return false;
		}

		public static bool WaitForBootComplete(string vmName, string oem = "bgp64")
		{
			return Utils.WaitForBootComplete(vmName, 180, oem);
		}

		public static bool WaitForBootComplete(string vmName, int retries, string oem = "bgp64")
		{
			if (!Utils.OemVmLockNamedata.ContainsKey(vmName + "_" + oem))
			{
				Utils.OemVmLockNamedata.Add(vmName + "_" + oem, new object());
			}
			if (!Utils.sIsGuestBooted.ContainsKey(vmName + "_" + oem))
			{
				Utils.sIsGuestBooted.Add(vmName + "_" + oem, false);
			}
			object obj = Utils.OemVmLockNamedata[vmName + "_" + oem];
			lock (obj)
			{
				Logger.Info("Checking if guest booted or not for {0} retries", new object[] { retries });
				while (retries > 0)
				{
					retries--;
					if (Utils.IsGuestBooted(vmName, oem))
					{
						return true;
					}
					Thread.Sleep(1000);
				}
				Logger.Info("Guest not booted after {0} retries", new object[] { retries });
			}
			return false;
		}

		public static bool IsSharedFolderMounted(string vmName)
		{
			try
			{
				if (!Utils.sIsSharedFolderMounted.ContainsKey(vmName))
				{
					Utils.sIsSharedFolderMounted.Add(vmName, false);
				}
				if (!Utils.sIsSharedFolderMounted[vmName] && JObject.Parse(HTTPUtils.SendRequestToGuest("isSharedFolderMounted", null, vmName, 1000, null, false, 1, 0, "bgp64"))["result"].ToString().Equals("ok", StringComparison.InvariantCultureIgnoreCase))
				{
					Utils.sIsSharedFolderMounted[vmName] = true;
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.Info("shared folder not mounted yet." + ex.Message);
			}
			return Utils.sIsSharedFolderMounted[vmName];
		}

		public static bool SetCustomAppSize(string vmName, string package, ScreenMode mode)
		{
			string text = "";
			try
			{
				JObject jobject = new JObject
				{
					{ "package_name", package },
					{
						"screen_mode",
						mode.ToString()
					}
				};
				Dictionary<string, string> dictionary = new Dictionary<string, string> { 
				{
					"d",
					jobject.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0])
				} };
				text = HTTPUtils.SendRequestToGuest("setcustomappsize", dictionary, vmName, 1000, null, false, 1, 0, "bgp64");
				if (JObject.Parse(text)["result"].ToString().Equals("ok", StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Concat(new string[]
				{
					"Error in sending setCustomAppSize to android response: ",
					text,
					" ",
					Environment.NewLine,
					" message: ",
					ex.Message
				}));
			}
			return false;
		}

		public static bool IsGuestBooted(string vmName, string oem = "bgp64")
		{
			try
			{
				if (!Utils.sIsGuestBooted.ContainsKey(vmName + "_" + oem))
				{
					Utils.sIsGuestBooted.Add(vmName + "_" + oem, false);
				}
				if (!Utils.sIsGuestBooted[vmName + "_" + oem])
				{
					HTTPUtils.SendRequestToGuest("ping", null, vmName, 1000, null, false, 1, 0, oem);
					Utils.sIsGuestBooted[vmName + "_" + oem] = true;
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Guest not booted yet." + ex.Message);
			}
			return Utils.sIsGuestBooted[vmName + "_" + oem];
		}

		public static void ExtractImages(string targetDir, string resourceName)
		{
			try
			{
				Directory.Delete(targetDir, true);
			}
			catch (Exception)
			{
			}
			if (!Directory.Exists(targetDir))
			{
				Directory.CreateDirectory(targetDir);
			}
			ResourceManager resourceManager;
			try
			{
				resourceManager = new ResourceManager(resourceName, Assembly.GetExecutingAssembly());
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to extract resources. err: " + ex.ToString());
				return;
			}
			global::System.Drawing.Image image = (global::System.Drawing.Image)resourceManager.GetObject("bg", CultureInfo.InvariantCulture);
			image.Save(Path.Combine(targetDir, "bg.jpg"), ImageFormat.Jpeg);
			bool flag = true;
			try
			{
				image = (global::System.Drawing.Image)resourceManager.GetObject("HomeScreen", CultureInfo.InvariantCulture);
				image.Save(Path.Combine(targetDir, "HomeScreen.jpg"), ImageFormat.Jpeg);
			}
			catch (Exception)
			{
				flag = false;
			}
			try
			{
				image = (global::System.Drawing.Image)resourceManager.GetObject("ThankYouImage", CultureInfo.InvariantCulture);
				image.Save(Path.Combine(targetDir, "ThankYouImage.jpg"), ImageFormat.Jpeg);
			}
			catch (Exception)
			{
			}
			int num = 0;
			try
			{
				for (;;)
				{
					num++;
					image = (global::System.Drawing.Image)resourceManager.GetObject("SetupImage" + Convert.ToString(num, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
					image.Save(Path.Combine(targetDir, "SetupImage" + Convert.ToString(num, CultureInfo.InvariantCulture) + ".jpg"), ImageFormat.Jpeg);
					if (!flag && num == 1)
					{
						image.Save(Path.Combine(targetDir, "HomeScreen.jpg"), ImageFormat.Jpeg);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static string DownloadIcon(string package, string directory = "", bool isReDownload = false)
		{
			string text = "https://cloud.bluestacks.com/app/icon?pkg={0}&fallback=false";
			string text2 = string.Format(CultureInfo.InvariantCulture, text, new object[] { package });
			string text3 = package + ".png";
			return Utils.TinyDownloader(text2, text3, directory, isReDownload);
		}

		public static string TinyDownloader(string url, string fileNameWithExtension, string directory = "", bool isReDownload = false)
		{
			string text = string.Empty;
			try
			{
				if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(fileNameWithExtension))
				{
					string text2 = Regex.Replace(fileNameWithExtension, "[\\x22\\\\\\/:*?|<>]", " ");
					if (string.IsNullOrEmpty(directory))
					{
						directory = RegistryStrings.GadgetDir;
					}
					text = Path.Combine(directory, text2);
					if (!Directory.Exists(Directory.GetParent(text).FullName))
					{
						Directory.CreateDirectory(Directory.GetParent(text).FullName);
					}
					if (!File.Exists(text) || isReDownload)
					{
						using (WebClient webClient = new WebClient())
						{
							webClient.DownloadFile(url, text);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Cannot download icon file" + ex.ToString());
			}
			return text;
		}

		public static string GetDNS2Value(string oem)
		{
			string text = "8.8.8.8";
			if (string.Compare(oem, "tc_dt", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "china", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "china_api", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "ucweb_dt", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "4399", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "anquicafe", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(oem, "yy_dt", StringComparison.OrdinalIgnoreCase) == 0)
			{
				text = "114.114.114.114";
			}
			return text;
		}

		public static bool IsInstallOrUpgradeRequired()
		{
			if (!Utils.IsBlueStacksInstalled())
			{
				return true;
			}
			string version = RegistryManager.Instance.Version;
			if (string.IsNullOrEmpty(version))
			{
				return true;
			}
			string text = version.Substring(0, version.LastIndexOf('.')) + ".0";
			string text2 = "4.220.0.4001".Substring(0, "4.220.0.4001".LastIndexOf('.')) + ".0";
			System.Version installedVersion = new System.Version(text);
			System.Version newVersion = new System.Version(text2);
			Logger.Info("Installed Version: {0}, new version: {1}", new object[] { version, "4.220.0.4001" });
			if (newVersion > installedVersion)
			{
				Logger.Info("IMP: lower version: {0} is already installed. Forcing upgrade.", new object[] { version });
				return true;
			}
			return false;
		}

		public static void SendBrowserVersionStats(string version, string vmName)
		{
			Thread thread = new Thread(new ThreadStart(delegate
			{
				try
				{
					string userGuid = RegistryManager.Instance.UserGuid;
					string text = "https://bluestacks-cloud.appspot.com/stats/ieversionstats";
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{ "ie_ver", version },
						{ "guid", userGuid },
						{ "prod_ver", "4.220.0.4001" }
					};
					Logger.Info("Sending browser version Stats");
					string text2 = BstHttpClient.Post(text, dictionary, null, false, vmName, 0, 1, 0, false, "bgp64");
					Logger.Info("Got browser version stat response: {0}", new object[] { text2 });
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to send app stats. error: " + ex.ToString());
				}
			}))
			{
				IsBackground = true
			};
			thread.Start();
		}

		public static bool IsRemoteFilePresent(string url)
		{
			bool flag = true;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "Head";
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse.StatusCode == HttpStatusCode.NotFound)
				{
					flag = false;
				}
				httpWebResponse.Close();
			}
			catch (Exception ex)
			{
				flag = false;
				Logger.Error("Could not make http request: " + ex.ToString());
			}
			return flag;
		}

		public static string ConvertToIco(string imagePath, string iconsDir)
		{
			Logger.Info("Converting {0}", new object[] { imagePath });
			string fileName = Path.GetFileName(imagePath);
			int num = fileName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
			string text = fileName.Substring(0, num) + ".ico";
			string text2 = Path.Combine(iconsDir, text);
			IconHelper.ConvertToIcon(imagePath, text2, 256, false);
			return text2;
		}

		public static void ResizeImage(string imagePath)
		{
			bool flag = false;
			using (FileStream fileStream = File.OpenRead(imagePath))
			{
				using (global::System.Drawing.Image image = global::System.Drawing.Image.FromStream(fileStream))
				{
					int num = image.Width;
					int num2 = image.Height;
					if (num >= 256)
					{
						int num3 = 256;
						num2 = (int)((float)num2 / ((float)num / (float)num3));
						num = num3;
						flag = true;
					}
					if (num2 >= 256)
					{
						int num4 = 256;
						num = (int)((float)num / ((float)num2 / (float)num4));
						num2 = num4;
						flag = true;
					}
					if (num % 8 != 0)
					{
						num -= num % 8;
						flag = true;
					}
					if (num2 % 8 != 0)
					{
						num2 -= num2 % 8;
						flag = true;
					}
					if (flag)
					{
						using (global::System.Drawing.Image image2 = new Bitmap(num, num2))
						{
							Graphics graphics = Graphics.FromImage(image2);
							graphics.SmoothingMode = SmoothingMode.AntiAlias;
							graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
							graphics.DrawImage(image, 0, 0, image2.Width, image2.Height);
							File.Delete(imagePath);
							image2.Save(imagePath);
						}
					}
				}
			}
		}

		public static int GetSystemHeight()
		{
			return Utils.GetSystemMetrics(1);
		}

		public static int GetSystemWidth()
		{
			return Utils.GetSystemMetrics(0);
		}

		public static int GetBstCommandProcessorPort(string vmName)
		{
			return RegistryManager.Instance.Guest[vmName].BstAndroidPort;
		}

		public static bool IsHomeApp(string appInfo)
		{
			return appInfo == null || appInfo.IndexOf("com.bluestacks.appmart", StringComparison.OrdinalIgnoreCase) != -1 || (appInfo == null || appInfo.IndexOf("com.android.launcher2", StringComparison.OrdinalIgnoreCase) != -1) || (appInfo == null || appInfo.IndexOf("com.uncube.launcher", StringComparison.OrdinalIgnoreCase) != -1) || (appInfo == null || appInfo.IndexOf("com.bluestacks.gamepophome", StringComparison.OrdinalIgnoreCase) != -1);
		}

		public static bool IsValidEmail(string email)
		{
			return new Regex("^(([^<>()[\\]\\\\.,;:\\s@\\\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\\\"]+)*)|(\\\".+\\\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$").IsMatch(email);
		}

		public static string GetFileURI(string path)
		{
			return new Uri(path).AbsoluteUri;
		}

		public static string PostToBstCmdProcessorAfterServiceStart(string path, Dictionary<string, string> data, string vmName, bool isLaunchUI = true)
		{
			string text = null;
			if (!Utils.IsAllUIProcessAlive(vmName) && isLaunchUI)
			{
				Logger.Info("Starting Frontend in hidden mode.");
				Utils.StartHiddenFrontend(vmName, "bgp64");
			}
			int num = 300;
			if (!Utils.CheckIfGuestReady(vmName, num))
			{
				return new JObject
				{
					{ "result", "error" },
					{ "reason", "Guest boot failed" }
				}.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0]);
			}
			try
			{
				text = HTTPUtils.SendRequestToGuest(path, data, vmName, 0, null, false, 1, 0, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in PostAfterServiceStart");
				Logger.Error(ex.Message);
			}
			return text;
		}

		public static string GetToBstCmdProcessorAfterServiceStart(string path, string vmName)
		{
			string text = null;
			if (!Utils.IsUIProcessAlive(vmName, "bgp64"))
			{
				Logger.Info("Starting Frontend in hidden mode.");
				using (Process process = Utils.StartHiddenFrontend(vmName, "bgp64"))
				{
					process.WaitForExit(60000);
				}
			}
			try
			{
				text = HTTPUtils.SendRequestToGuest(path, null, vmName, 0, null, false, 1, 0, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetToBstCmdProcessorAfterServiceStart");
				Logger.Error(ex.Message);
			}
			return text;
		}

		public static bool IsAppInstalled(string package, string vmName, out string version)
		{
			version = "";
			string text;
			return Utils.IsAppInstalled(package, vmName, out version, out text, true);
		}

		public static bool IsAppInstalled(string package, string vmName, out string version, out string failReason, bool isLaunchUI = true)
		{
			Logger.Info("Utils: IsAppInstalled Called for package {0}", new object[] { package });
			version = "";
			failReason = "App not installed";
			bool flag = false;
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "package", package } };
				string text = Utils.PostToBstCmdProcessorAfterServiceStart("isPackageInstalled", dictionary, vmName, isLaunchUI);
				Logger.Info("Got response: {0}", new object[] { text });
				if (string.IsNullOrEmpty(text))
				{
					failReason = "The Api failed to get a response";
				}
				else
				{
					JObject jobject = JObject.Parse(text);
					string text2 = jobject["result"].ToString().Trim();
					if (string.Compare(text2, "ok", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
						version = jobject["version"].ToString().Trim();
					}
					else if (string.Compare(text2, "error", StringComparison.OrdinalIgnoreCase) == 0)
					{
						failReason = jobject["reason"].ToString().Trim();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format(CultureInfo.InvariantCulture, "Error Occured, Err: {0}", new object[] { ex.ToString() }));
				failReason = ex.Message;
			}
			Logger.Info("Installed = {0}", new object[] { flag });
			return flag;
		}

		private static string FilterSystemApps(JArray packages, out bool isSamsungStorePresent)
		{
			isSamsungStorePresent = false;
			JArray jarray = new JArray();
			foreach (JObject jobject in packages.Children<JObject>())
			{
				if (jobject["package"].ToString().Trim().Equals("com.sec.android.app.samsungapps", StringComparison.Ordinal))
				{
					isSamsungStorePresent = true;
				}
				if (string.Compare(jobject["systemapp"].ToString().Trim(), "0", StringComparison.OrdinalIgnoreCase) == 0)
				{
					bool flag = true;
					for (int i = 0; i < Utils.sListIgnoredApps.Count; i++)
					{
						if (string.Compare(jobject["package"].ToString().Trim(), Utils.sListIgnoredApps[i], StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						JObject jobject2 = new JObject
						{
							{
								"package",
								jobject["package"].ToString().Trim()
							},
							{
								"version",
								jobject["version"].ToString().Trim()
							},
							{
								"appname",
								jobject["appname"].ToString().Trim()
							},
							{
								"gl3required",
								jobject["gl3required"].ToString().Trim()
							}
						};
						jarray.Add(jobject2);
					}
				}
			}
			return jarray.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0]);
		}

		public static string GetInstalledPackages(string vmName, out string failReason, out bool isSamsungStorePresent, int count = 0)
		{
			Logger.Info("Utils: GetInstalledPackages Called for VM: {0}", new object[] { vmName });
			failReason = "Unable to get list of installed apps";
			isSamsungStorePresent = false;
			string text = "";
			try
			{
				string toBstCmdProcessorAfterServiceStart = Utils.GetToBstCmdProcessorAfterServiceStart("installedPackages", vmName);
				Logger.Info("Got response: {0}", new object[] { toBstCmdProcessorAfterServiceStart });
				if (string.IsNullOrEmpty(toBstCmdProcessorAfterServiceStart))
				{
					failReason = "The Api failed to get a response";
				}
				else
				{
					JObject jobject = JObject.Parse(toBstCmdProcessorAfterServiceStart);
					string text2 = jobject["result"].ToString().Trim();
					if (string.Compare(text2, "ok", StringComparison.OrdinalIgnoreCase) == 0)
					{
						failReason = "";
						text = Utils.FilterSystemApps(jobject["installed_packages"] as JArray, out isSamsungStorePresent);
						Logger.Info("Filtered results: {0}", new object[] { text });
					}
					else if (string.Compare(text2, "error", StringComparison.OrdinalIgnoreCase) == 0)
					{
						failReason = jobject["reason"].ToString().Trim();
						if (string.Compare(failReason, "system not ready", StringComparison.OrdinalIgnoreCase) == 0 && count < 6)
						{
							Thread.Sleep(500);
							return Utils.GetInstalledPackages(vmName, out failReason, out isSamsungStorePresent, count++);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format(CultureInfo.InvariantCulture, "Error Occurred, Err: {0}", new object[] { ex.ToString() }));
				failReason = ex.Message;
			}
			return text;
		}

		public static string GetInstalledPackagesFromAppsJSon(string vmName)
		{
			try
			{
				List<string> installedAppsList = JsonParser.GetInstalledAppsList(vmName);
				return string.Join(",", installedAppsList.ToArray());
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't get installed app list. Ex: {0}", new object[] { ex });
			}
			return string.Empty;
		}

		public static AppInfo GetPackageDetails(string vmName, string package, bool videoPresent, out string failReason)
		{
			AppInfo appInfo = null;
			try
			{
				string text = Utils.PostToBstCmdProcessorAfterServiceStart("getPackageDetails", new Dictionary<string, string> { { "package", package } }, vmName, true);
				if (string.IsNullOrEmpty(text))
				{
					failReason = "The api failed to get a response";
				}
				else
				{
					JObject jobject = JObject.Parse(text);
					if (string.Compare(jobject["result"].ToString().Trim(), "ok", StringComparison.OrdinalIgnoreCase) == 0)
					{
						failReason = "";
						JArray jarray = JArray.Parse(jobject["activities"].ToString());
						appInfo = new AppInfo(jobject["name"].ToString().Trim(), jarray[0]["img"].ToString().Trim(), jobject["package"].ToString().Trim(), (jarray[0] as JObject)["activity"].ToString().Trim(), "0", "no", jobject["version"].ToString().Trim(), jobject["gl3required"].ToObject<bool>(), videoPresent, jobject["versionName"].ToString().Trim(), false);
					}
					else
					{
						failReason = "The api failed to get a response";
					}
				}
			}
			catch (Exception ex)
			{
				failReason = ex.Message;
			}
			return appInfo;
		}

		public static void SyncAppJson(string vmName)
		{
			Logger.Info("In SyncAppJson");
			if (Utils.sIsSyncAppJsonComplete)
			{
				return;
			}
			try
			{
				string text;
				bool flag;
				string installedPackages = Utils.GetInstalledPackages(vmName, out text, out flag, 0);
				if (flag && !RegistryManager.Instance.IsSamsungStorePresent)
				{
					RegistryManager.Instance.IsSamsungStorePresent = true;
					HTTPUtils.SendRequestToClient("reloadPromotions", null, vmName, 0, null, false, 1, 0, "bgp64");
				}
				if (string.IsNullOrEmpty(text))
				{
					JArray jarray = JArray.Parse(installedPackages);
					JsonParser jsonParser = new JsonParser(vmName);
					AppInfo[] appList = jsonParser.GetAppList();
					List<AppInfo> list = appList.ToList<AppInfo>();
					bool flag2 = false;
					bool flag3 = false;
					using (IEnumerator<JObject> enumerator = jarray.Children<JObject>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							JObject installedAppsJsonObj = enumerator.Current;
							string package2 = installedAppsJsonObj["package"].ToString().Trim();
							if (jsonParser.GetAppInfoFromPackageName(package2) != null)
							{
								if (!string.Equals(JsonParser.GetGl3RequirementFromPackage(appList, package2).ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture)
									.Trim(), installedAppsJsonObj["gl3required"].ToString().ToLower(CultureInfo.InvariantCulture).Trim(), StringComparison.OrdinalIgnoreCase))
								{
									flag2 = true;
									list.Where((AppInfo x) => x.Package == package2).FirstOrDefault<AppInfo>().Gl3Required = installedAppsJsonObj["gl3required"].ToObject<bool>();
								}
								flag3 = JsonParser.GetVideoPresentRequirementFromPackage(appList, package2);
								AppInfo appInfo3 = list.Where((AppInfo x) => x.Package == package2).FirstOrDefault<AppInfo>();
								try
								{
									appInfo3.VideoPresent = installedAppsJsonObj["videopresent"].ToObject<bool>();
								}
								catch
								{
								}
							}
							if (!appList.Any((AppInfo _) => string.Compare(_.Package.Trim(), installedAppsJsonObj["package"].ToString().Trim(), StringComparison.OrdinalIgnoreCase) == 0))
							{
								flag2 = true;
								AppInfo packageDetails = Utils.GetPackageDetails(vmName, installedAppsJsonObj["package"].ToString().Trim(), flag3, out text);
								if (packageDetails != null)
								{
									list.Add(packageDetails);
								}
							}
						}
					}
					if (jarray.Count != list.Count || flag2)
					{
						List<string> list2 = new List<string>();
						using (List<AppInfo>.Enumerator enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								AppInfo appInfo = enumerator2.Current;
								if (!jarray.Children<JObject>().Any((JObject _) => string.Compare(_["package"].ToString().Trim(), appInfo.Package.Trim(), StringComparison.OrdinalIgnoreCase) == 0))
								{
									list2.Add(appInfo.Package);
									flag2 = true;
								}
							}
						}
						using (List<string>.Enumerator enumerator3 = list2.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								string package = enumerator3.Current;
								list.RemoveAll((AppInfo _) => _.Package == package);
							}
						}
						if (appList.Length != list.Count)
						{
							flag2 = true;
							list = new List<AppInfo>(jarray.Count);
							foreach (JObject jobject in jarray.Children<JObject>())
							{
								AppInfo packageDetails2 = Utils.GetPackageDetails(vmName, jobject["package"].ToString().Trim(), false, out text);
								if (packageDetails2 != null)
								{
									list.Add(packageDetails2);
								}
							}
							Logger.Info("Updating App Json from apps received from android. Details: " + installedPackages);
						}
					}
					foreach (AppInfo appInfo2 in list)
					{
						bool flag4 = Utils.CheckGamepadCompatible(appInfo2.Package);
						if (appInfo2.IsGamepadCompatible != flag4)
						{
							appInfo2.IsGamepadCompatible = flag4;
							flag2 = true;
						}
					}
					if (flag2)
					{
						jsonParser.WriteJson(list.ToArray());
						try
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							HTTPUtils.SendRequestToClient("appJsonChanged", dictionary, vmName, 0, null, false, 1, 0, "bgp64");
						}
						catch (Exception ex)
						{
							Logger.Error("Exception while sending appsync update to client: " + ex.ToString());
						}
					}
					Utils.sIsSyncAppJsonComplete = true;
				}
			}
			catch (Exception ex2)
			{
				Logger.Warning(string.Format(CultureInfo.InvariantCulture, "Unable to sync app.json file for vm:{0}. " + ex2.ToString(), new object[] { vmName }));
			}
		}

		public static bool CheckGamepadCompatible(string packageName)
		{
			try
			{
				string inputmapperFile = Utils.GetInputmapperFile(packageName);
				bool flag = false;
				if (!string.IsNullOrEmpty(inputmapperFile))
				{
					string text = "";
					using (Mutex mutex = new Mutex(false, "BlueStacks_CfgAccess"))
					{
						if (mutex.WaitOne())
						{
							try
							{
								text = File.ReadAllText(inputmapperFile);
								flag = true;
							}
							catch (Exception ex)
							{
								Logger.Error(string.Format("Failed to read cfg file... filepath: {0} Err : {1}", inputmapperFile, ex));
							}
							finally
							{
								mutex.ReleaseMutex();
							}
						}
					}
					if (flag)
					{
						foreach (string text2 in Constants.ImapGamepadEvents)
						{
							if (text.Contains(text2))
							{
								return true;
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception in CheckGamepadCompatible: " + ex2.ToString());
			}
			return false;
		}

		public static string GetInputmapperFile(string packageName = "")
		{
			string text = string.Empty;
			try
			{
				if (File.Exists(Utils.GetInputmapperUserFilePath(packageName)))
				{
					text = Utils.GetInputmapperUserFilePath(packageName);
				}
				else if (File.Exists(Utils.GetInputmapperDefaultFilePath(packageName)))
				{
					text = Utils.GetInputmapperDefaultFilePath(packageName);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Excpetion in GetInputMapper: " + ex.ToString());
			}
			return text;
		}

		public static string GetInputmapperUserFilePath(string packageName)
		{
			return Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), packageName + ".cfg");
		}

		public static string GetInputmapperDefaultFilePath(string packageName)
		{
			return Path.Combine(RegistryStrings.InputMapperFolder, packageName + ".cfg");
		}

		public static bool UnsupportedProcessor()
		{
			try
			{
				Logger.Info("Checking if Processor Unsupported");
				string[] array = new string[] { "AMD64 Family 21 Model 16 Stepping 1 AuthenticAMD" };
				string text = Path.Combine(Path.GetTempPath(), "SystemInfo.txt");
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				Utils.RunCmd("SystemInfo", null, text);
				string text2 = File.ReadAllText(text);
				foreach (string text3 in array)
				{
					if (text2.IndexOf(text3, StringComparison.OrdinalIgnoreCase) != -1)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in Checking if Processor Unsupported : {0}", new object[] { ex.ToString() });
			}
			return false;
		}

		public static bool ReserveHTTPPorts()
		{
			bool flag = false;
			try
			{
				string text = new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString();
				string text2 = "netsh.exe";
				int num = 2861;
				int num2 = 2971;
				Logger.Info("Reserving ports {0} - {1}", new object[] { num, num2 });
				Logger.Info("---------------------------------------------------------------");
				bool flag2 = false;
				for (int i = num; i < num2; i++)
				{
					try
					{
						RunCommand.RunCmd(text2, string.Format(CultureInfo.InvariantCulture, "http add urlacl url=http://*:{0}/ User=\\\"" + text + "\"", new object[] { i }), flag2, flag2, false, 0);
					}
					catch (Exception ex)
					{
						Logger.Error(string.Format(CultureInfo.InvariantCulture, "Error occured, Err: {0}", new object[] { ex.ToString() }));
					}
					flag2 = i % 10 == 0;
				}
				flag = true;
			}
			catch (Exception ex2)
			{
				Logger.Error("Error in reserving HTTP ports: {0}", new object[] { ex2.ToString() });
				flag = false;
			}
			Logger.Info("---------------------------------------------------------------");
			return flag;
		}

		public static void RestartService(string serviceName, int timeoutMilliseconds)
		{
			Logger.Info("Restarting {0} service", new object[] { serviceName });
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				try
				{
					int tickCount = Environment.TickCount;
					TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)timeoutMilliseconds);
					serviceController.Stop();
					serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeSpan);
					int tickCount2 = Environment.TickCount;
					timeSpan = TimeSpan.FromMilliseconds((double)(timeoutMilliseconds - (tickCount2 - tickCount)));
					serviceController.Start();
					serviceController.WaitForStatus(ServiceControllerStatus.Running, timeSpan);
				}
				catch (Exception ex)
				{
					Logger.Error("Error in restarting service " + ex.ToString());
				}
			}
		}

		public static bool CheckOpenGlSupport(out int glRenderMode, out string glVendor, out string glRenderer, out string glVersion, string blueStacksProgramFiles)
		{
			Logger.Info("In CheckSupportedGlRenderMode");
			glRenderMode = 4;
			glVersion = "";
			glRenderer = "";
			glVendor = "";
			Logger.Info("Running glcheck from folder : " + blueStacksProgramFiles);
			Logger.Info("Checking for glRenderMode 1");
			if (Utils.GetGraphicsInfo(Path.Combine(blueStacksProgramFiles, "HD-GLCheck.exe"), "1", out glVendor, out glRenderer, out glVersion) == 0)
			{
				glRenderMode = 1;
				return true;
			}
			Logger.Info("Opengl not supported.");
			return false;
		}

		public static int GetCurrentGraphicsInfo(string args, out string glVendor, out string glRenderer, out string glVersion)
		{
			return Utils.GetGraphicsInfo(Path.Combine(RegistryStrings.InstallDir, "HD-GLCheck.exe"), args, out glVendor, out glRenderer, out glVersion, true);
		}

		public static int GetGraphicsInfo(string prog, string args, out string glVendor, out string glRenderer, out string glVersion)
		{
			return Utils.GetGraphicsInfo(prog, args, out glVendor, out glRenderer, out glVersion, true);
		}

		public static int GetGraphicsInfo(string prog, string args, out string glVendor, out string glRenderer, out string glVersion, bool enableLogging)
		{
			Logger.Info("Will run " + prog + " with args " + args);
			string vendor = "";
			string renderer = "";
			string version = "";
			glVendor = vendor;
			glRenderer = renderer;
			glVersion = version;
			int num = -1;
			Environment.GetEnvironmentVariable("TEMP");
			try
			{
				using (Process proc = new Process())
				{
					proc.StartInfo.FileName = prog;
					proc.StartInfo.Arguments = args;
					proc.StartInfo.UseShellExecute = false;
					proc.StartInfo.CreateNoWindow = true;
					proc.StartInfo.RedirectStandardOutput = true;
					proc.OutputDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
					{
						try
						{
							string text = ((outLine.Data != null) ? outLine.Data : "");
							if (enableLogging)
							{
								Logger.Info(proc.Id.ToString() + " OUT: " + text);
							}
							if (text.Contains("GL_VENDOR ="))
							{
								int num3 = text.IndexOf('=');
								vendor = text.Substring(num3 + 1).Trim();
								vendor = vendor.Replace(";", ";;");
							}
							if (text.Contains("GL_RENDERER ="))
							{
								int num3 = text.IndexOf('=');
								renderer = text.Substring(num3 + 1).Trim();
								renderer = renderer.Replace(";", ";;");
							}
							if (text.Contains("GL_VERSION ="))
							{
								int num3 = text.IndexOf('=');
								version = text.Substring(num3 + 1).Trim();
								version = version.Replace(";", ";;");
							}
						}
						catch (Exception ex2)
						{
							Logger.Error("A crash occured in the GLCheck delegate");
							Logger.Error(ex2.ToString());
						}
					};
					proc.Start();
					proc.BeginOutputReadLine();
					int num2 = 10000;
					bool flag = proc.WaitForExit(num2);
					glVendor = vendor;
					glRenderer = renderer;
					glVersion = version;
					if (flag)
					{
						Logger.Info(proc.Id.ToString() + " EXIT: " + proc.ExitCode.ToString());
						num = proc.ExitCode;
					}
					else
					{
						Logger.Error("Process killed after timeout: {0}s", new object[] { num2 / 1000 });
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some error while running graphics check. Ex: {0}", new object[] { ex });
			}
			return num;
		}

		public static int CheckSsse3Info(string prog, out string ssse3Supported)
		{
			Logger.Info("Will run " + prog);
			int num = -1;
			string ssse3value = "";
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = prog;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.RedirectStandardError = true;
					Countdown countDown = new Countdown(1);
					process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
					{
						if (outLine.Data != null)
						{
							try
							{
								string data = outLine.Data;
								if (data.Contains("value ="))
								{
									int num3 = data.IndexOf('=');
									ssse3value = data.Substring(num3 + 1).Trim();
								}
							}
							catch (Exception ex2)
							{
								Console.WriteLine("A crash occured in check cpu info delegate");
								Console.WriteLine(ex2.ToString());
							}
							Logger.Info(Path.GetFileName(prog) + ": " + outLine.Data);
							return;
						}
						countDown.Signal();
					};
					process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
					{
						if (outLine.Data != null)
						{
							Logger.Error(Path.GetFileName(prog) + ": " + outLine.Data);
							return;
						}
						countDown.Signal();
					};
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					int num2 = 10000;
					bool flag = process.WaitForExit(num2);
					countDown.Wait();
					if (flag)
					{
						Logger.Info(process.Id.ToString() + " EXIT: " + process.ExitCode.ToString());
						num = process.ExitCode;
					}
					else
					{
						Logger.Error("Process killed after timeout: {0}s", new object[] { num2 / 1000 });
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some error while running graphics check. Ex: {0}", new object[] { ex });
			}
			if (ssse3value == "1" || string.IsNullOrEmpty(ssse3value))
			{
				ssse3Supported = "1";
			}
			else
			{
				ssse3Supported = "0";
			}
			return num;
		}

		public static bool CheckTwoCameraPresentOnDevice(ref bool bBothCamera)
		{
			bool flag;
			try
			{
				Guid guid = new Guid("{53F56307-B6BF-11D0-94F2-00A0C91EFB8B}");
				int num = Utils.SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, ClassDevsFlags.DIGCF_PRESENT | ClassDevsFlags.DIGCF_ALLCLASSES);
				int num2 = -1;
				int num3 = 0;
				int num4 = 0;
				while (num2 != 0)
				{
					SP_DEVINFO_DATA sp_DEVINFO_DATA = default(SP_DEVINFO_DATA);
					sp_DEVINFO_DATA.cbSize = Marshal.SizeOf(sp_DEVINFO_DATA);
					num2 = Utils.SetupDiEnumDeviceInfo(num, num3, ref sp_DEVINFO_DATA);
					if (num2 == 1 && Utils.GetRegistryProperty(num, ref sp_DEVINFO_DATA, RegPropertyType.SPDRP_CLASSGUID).Equals("{6bdd1fc6-810f-11d0-bec7-08002be2092f}", StringComparison.OrdinalIgnoreCase))
					{
						num4++;
						if (num4 == 2)
						{
							bBothCamera = true;
						}
					}
					num3++;
					if (bBothCamera)
					{
						Logger.Info("Both Camera present on Device");
						break;
					}
				}
				flag = true;
			}
			catch (Exception ex)
			{
				flag = false;
				Logger.Info("Exception when trying to check Camera present on Device");
				Logger.Info(ex.ToString());
			}
			return flag;
		}

		private static string GetRegistryProperty(int PnPHandle, ref SP_DEVINFO_DATA DeviceInfoData, RegPropertyType Property)
		{
			int num = 0;
			DATA_BUFFER data_BUFFER = default(DATA_BUFFER);
			Utils.SetupDiGetDeviceRegistryProperty(PnPHandle, ref DeviceInfoData, Property, IntPtr.Zero, ref data_BUFFER, 1024, ref num);
			return data_BUFFER.Buffer;
		}

		public static int CallApkInstaller(string apkPath, bool isSilentInstall)
		{
			return Utils.CallApkInstaller(apkPath, isSilentInstall, null);
		}

		public static int CallApkInstaller(string apkPath, bool isSilentInstall, string vmName)
		{
			Logger.Info("Installing apk :{0} vm name :{1} ", new object[] { apkPath, vmName });
			if (vmName == null)
			{
				vmName = "Android";
			}
			int num = -1;
			try
			{
				string installDir = RegistryStrings.InstallDir;
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
				{
					processStartInfo.FileName = Path.Combine(installDir, "HD-XapkHandler.exe");
					if (isSilentInstall)
					{
						processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-xapk \"{0}\" -s -vmname {1}", new object[] { apkPath, vmName });
					}
					else
					{
						processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-xapk \"{0}\" -vmname {1}", new object[] { apkPath, vmName });
					}
				}
				else
				{
					processStartInfo.FileName = Path.Combine(installDir, "HD-ApkHandler.exe");
					if (isSilentInstall)
					{
						processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-apk \"{0}\" -s -vmname {1}", new object[] { apkPath, vmName });
					}
					else
					{
						processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-apk \"{0}\" -vmname {1}", new object[] { apkPath, vmName });
					}
				}
				processStartInfo.UseShellExecute = false;
				processStartInfo.CreateNoWindow = true;
				Logger.Info("Console: installer path {0}", new object[] { processStartInfo.FileName });
				Process process = Process.Start(processStartInfo);
				process.WaitForExit();
				num = process.ExitCode;
				Logger.Info("Console: apk installer exit code: {0}", new object[] { process.ExitCode });
			}
			catch (Exception ex)
			{
				Logger.Info("Error Installing Apk : " + ex.ToString());
			}
			return num;
		}

		public static string GetInstallStatsUrl()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				RegistryManager.Instance.Host,
				"stats/bsinstallstats"
			});
		}

		public static Dictionary<string, string> GetUserData()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string version = RegistryManager.Instance.Version;
			string registeredEmail = RegistryManager.Instance.RegisteredEmail;
			if (!string.IsNullOrEmpty(registeredEmail))
			{
				dictionary.Add("email", registeredEmail);
			}
			long num = DateTime.UtcNow.Ticks - 621355968000000000L;
			string text = (num / 10000000L).ToString(CultureInfo.InvariantCulture);
			dictionary.Add("user_time", text);
			return dictionary;
		}

		public static bool IsForegroundApplication()
		{
			bool flag = false;
			IntPtr foregroundWindow = InteropWindow.GetForegroundWindow();
			if (foregroundWindow != IntPtr.Zero)
			{
				uint num = 0U;
				InteropWindow.GetWindowThreadProcessId(foregroundWindow, ref num);
				if ((ulong)num == (ulong)((long)Process.GetCurrentProcess().Id))
				{
					flag = true;
				}
			}
			return flag;
		}

		public static bool CheckWritePermissionForFolder(string DirectoryPath)
		{
			if (string.IsNullOrEmpty(DirectoryPath))
			{
				return false;
			}
			bool flag;
			try
			{
				using (File.Create(Path.Combine(DirectoryPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
				{
				}
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static void UpdateRegistry(string registryKey, string name, object value, RegistryValueKind kind)
		{
			try
			{
				RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(registryKey, true);
				registryKey2.SetValue(name, value, kind);
				registryKey2.Close();
				registryKey2.Flush();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception occured in UpdateRegistry " + ex.ToString());
				throw;
			}
		}

		public static Icon GetApplicationIcon()
		{
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				return new Icon(Path.Combine(RegistryStrings.InstallDir, "app_icon.ico"));
			}
			string productIconCompletePath = RegistryStrings.ProductIconCompletePath;
			if (File.Exists(productIconCompletePath))
			{
				return new Icon(productIconCompletePath);
			}
			return Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}

		public static bool IsHDPlusDebugMode()
		{
			return RegistryManager.Instance.PlusDebug != 0;
		}

		public static int GetGMStreamWindowWidth()
		{
			return 320 * SystemUtils.GetDPI() / 96;
		}

		public static void SetCurrentEngineStateAndGlTransportValue(EngineState state, string vmName)
		{
			Logger.Info("Setting CurrentEngineState: " + state.ToString());
			RegistryManager.Instance.CurrentEngine = state.ToString();
			string bootParameters = RegistryManager.Instance.Guest[vmName].BootParameters;
			string[] array = bootParameters.Split(new char[] { ' ' });
			string text = "";
			string text2 = "GlTransport";
			int num;
			if (state == EngineState.legacy)
			{
				num = RegistryManager.Instance.GlLegacyTransportConfig;
			}
			else
			{
				num = RegistryManager.Instance.GlPlusTransportConfig;
			}
			Logger.Info("setting GlValue to {0}", new object[] { num });
			if (bootParameters.IndexOf(text2, StringComparison.OrdinalIgnoreCase) == -1)
			{
				text = string.Concat(new string[]
				{
					bootParameters,
					" ",
					text2,
					"=",
					num.ToString()
				});
			}
			else
			{
				foreach (string text3 in array)
				{
					if (text3.IndexOf(text2, StringComparison.OrdinalIgnoreCase) != -1)
					{
						if (!string.IsNullOrEmpty(text))
						{
							text += " ";
						}
						text = text + text2 + "=" + num.ToString();
					}
					else
					{
						if (!string.IsNullOrEmpty(text))
						{
							text += " ";
						}
						text += text3;
					}
				}
			}
			RegistryManager.Instance.Guest[vmName].BootParameters = text;
		}

		public static bool RegisterComExe(string path, bool register)
		{
			bool flag;
			try
			{
				flag = Utils.RunCmd(path, register ? "/RegServer" : "/UnregServer", null).ExitCode == 0;
			}
			catch (Exception ex)
			{
				Logger.Error("Command runner raised an exception: " + ex.ToString());
				flag = false;
			}
			return flag;
		}

		public static string GetCurrentKeyboardLayout()
		{
			string text;
			try
			{
				text = new CultureInfo(Utils.GetKeyboardLayout(Utils.GetWindowThreadProcessId(Utils.GetForegroundWindow(), IntPtr.Zero)).ToInt32() & 65535).Name;
			}
			catch
			{
				text = "en-US";
			}
			return text;
		}

		public static bool IsEngineRaw()
		{
			bool flag = false;
			try
			{
				if (JObject.Parse(RegistryManager.Instance.DeviceCaps)["engine_enabled"].ToString().Trim() == EngineState.raw.ToString())
				{
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error Occured, Err: " + ex.ToString());
			}
			Logger.Info("Engine mode Raw: " + flag.ToString());
			return flag;
		}

		public static string GetCampaignName()
		{
			string text = "";
			try
			{
				string campaignJson = RegistryManager.Instance.CampaignJson;
				if (string.IsNullOrEmpty(campaignJson))
				{
					return text;
				}
				JObject jobject = JObject.Parse(campaignJson);
				if (jobject != null)
				{
					text = jobject["campaign_name"].ToString();
				}
			}
			catch (Exception)
			{
				Logger.Warning("Failed to get campaign name.");
			}
			return text;
		}

		public static string GetUserCountry(string vmName)
		{
			string text2;
			try
			{
				string text = BstHttpClient.Get(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
				{
					RegistryManager.Instance.Host,
					"api/getcountryforip"
				}), null, false, vmName, 0, 1, 0, false, "bgp64");
				Logger.Info("Got resp: " + text);
				text2 = JObject.Parse(text)["country"].ToString().Trim();
			}
			catch (Exception ex)
			{
				Logger.Error(ex.Message);
				text2 = "";
			}
			return text2;
		}

		public static void KillComServer()
		{
			Logger.Info("In KillComServer");
			string fullPath = Path.GetFullPath(RegistryStrings.InstallDir + "\\");
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_Process WHERE Name = 'BstkSVC.exe'")))
			{
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					string text = "Considering ";
					object obj = managementObject["ProcessId"];
					string text2 = ((obj != null) ? obj.ToString() : null);
					string text3 = " -> ";
					object obj2 = managementObject["ExecutablePath"];
					Logger.Info(text + text2 + text3 + ((obj2 != null) ? obj2.ToString() : null));
					if (string.Compare(Path.GetFullPath(Path.GetDirectoryName((string)managementObject["ExecutablePath"]) + "\\"), fullPath, StringComparison.OrdinalIgnoreCase) == 0)
					{
						Process processById = Process.GetProcessById((int)((uint)managementObject["ProcessId"]));
						Logger.Info("Trying to kill PID " + processById.Id.ToString());
						processById.Kill();
						if (!processById.WaitForExit(10000))
						{
							Logger.Info("Timeout waiting for process to die");
						}
					}
				}
			}
		}

		public static void StopClientInstanceAsync(string vmName)
		{
			try
			{
				Logger.Info("Will send request stopInstance to " + vmName);
				List<string> list = new List<string> { vmName };
				if (string.IsNullOrEmpty(vmName))
				{
					list = RegistryManager.Instance.VmList.ToList<string>();
				}
				foreach (string text in list)
				{
					try
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string> { { "vmName", text } };
						HTTPUtils.SendRequestToClientAsync("stopInstance", dictionary, text, 0, null, false, 1, 0, "bgp64");
					}
					catch (Exception ex)
					{
						Logger.Warning("Exception in closing client for vm: {0} --> {1}", new object[] { text, ex.Message });
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception in closing any frontend: " + ex2.ToString());
			}
		}

		public static void StopFrontend(string vmName, bool isWaitForPlayerClosing = true)
		{
			try
			{
				Logger.Info("Will send request shutdown" + vmName);
				List<string> list = new List<string> { vmName };
				if (string.IsNullOrEmpty(vmName))
				{
					list = RegistryManager.Instance.VmList.ToList<string>();
				}
				foreach (string text in list)
				{
					try
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string> { { "vmName", text } };
						bool flag;
						using (Mutex mutex = new Mutex(true, Strings.GetPlayerLockName(text, "bgp64"), out flag))
						{
							if (!flag)
							{
								HTTPUtils.SendRequestToEngineAsync("shutdown", dictionary, text, 0, null, false, 1, 0);
								if (isWaitForPlayerClosing)
								{
									try
									{
										if (!mutex.WaitOne(60000))
										{
											HTTPUtils.SendRequestToEngine("forceShutdown", null, "Android", 0, null, false, 1, 0, "");
										}
									}
									catch (AbandonedMutexException ex)
									{
										Logger.Info("Player closed: " + ex.Message);
									}
									catch (Exception ex2)
									{
										Logger.Error("Could not check if player is running." + ex2.Message);
									}
								}
							}
						}
					}
					catch (Exception ex3)
					{
						Logger.Warning("Exception in closing any frontend for vm = " + text + " -->" + ex3.ToString());
					}
				}
			}
			catch (Exception ex4)
			{
				Logger.Error("Exception in closing any frontend: " + ex4.ToString());
			}
		}

		public static bool CheckIfAndroidBstkExistAndValid(string vmName)
		{
			Logger.Info("Checking if android bstk exist and valid");
			string text = Path.Combine(Path.Combine(RegistryStrings.DataDir, vmName), vmName + ".bstk");
			if (File.Exists(text))
			{
				if (new FileInfo(text).Length == 0L)
				{
					return false;
				}
				try
				{
					new XDocument();
					XDocument.Load(text);
					return true;
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in parsing bstk file" + ex.ToString());
					return false;
				}
				return false;
			}
			return false;
		}

		public static void CreateBstkFileFromPrev(string vmName)
		{
			Logger.Info("Creating Bstk file from Bstk-Prev file");
			string text = Path.Combine(RegistryStrings.DataDir, vmName);
			string text2 = Path.Combine(text, vmName + ".bstk");
			string text3 = Path.Combine(text, vmName + ".bstk-prev");
			if (!File.Exists(text3))
			{
				Logger.Info("android.bstk-prev file not exist");
				return;
			}
			File.Copy(text3, text2, true);
		}

		public static bool IsFirstVersionHigher(string firstVersion, string secondVersion)
		{
			string[] array = ((firstVersion != null) ? firstVersion.Split(new char[] { '.' }) : null);
			string[] array2 = ((secondVersion != null) ? secondVersion.Split(new char[] { '.' }) : null);
			bool flag = false;
			int i = 0;
			int num = Math.Min(array.Length, array2.Length);
			while (i < num)
			{
				long num2 = Convert.ToInt64(array[i], CultureInfo.InvariantCulture);
				long num3 = Convert.ToInt64(array2[i], CultureInfo.InvariantCulture);
				long num4 = num2 - num3;
				if (num4 > 0L)
				{
					flag = true;
					break;
				}
				if (num4 < 0L)
				{
					break;
				}
				i++;
			}
			if (!flag && i < array.Length && i == array2.Length)
			{
				flag = true;
			}
			return flag;
		}

		public static bool IsRunningInstanceClashWithAnotherInstance(string procName)
		{
			string installDir = RegistryStrings.InstallDir;
			string clientInstallDir = RegistryManager.Instance.ClientInstallDir;
			if (string.IsNullOrEmpty(installDir) && string.IsNullOrEmpty(clientInstallDir))
			{
				return false;
			}
			procName = ((procName != null) ? procName.Replace(".exe", "") : null);
			List<string> applicationPath = GetProcessExecutionPath.GetApplicationPath(Process.GetProcessesByName(procName));
			Logger.Debug("Number of running instances for the process {0} are {1} ", new object[] { procName, applicationPath.Count });
			foreach (string text in applicationPath)
			{
				try
				{
					string directoryName = Path.GetDirectoryName(text);
					if (!directoryName.Equals(installDir.TrimEnd(new char[] { '\\' }), StringComparison.InvariantCultureIgnoreCase) && !directoryName.Equals(clientInstallDir.TrimEnd(new char[] { '\\' }), StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		public static int GetVideoControllersNum()
		{
			int num = 0;
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
				{
					ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
					num = managementObjectCollection.Count;
					Logger.Info("Win32_VideoController query count: ", new object[] { num });
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						foreach (PropertyData propertyData in ((ManagementObject)managementBaseObject).Properties)
						{
							string name = propertyData.Name;
							if (name != null)
							{
								if (!(name == "Description"))
								{
									if (!(name == "DriverVersion"))
									{
										if (name == "DriverDate")
										{
											Logger.Info("DriverDate: {0}", new object[] { ManagementDateTimeConverter.ToDateTime(propertyData.Value.ToString()).ToUniversalTime().ToString("yyyy-MM-dd HH-mm-ss", DateTimeFormatInfo.InvariantInfo) });
										}
									}
									else
									{
										Logger.Info("DriverVersion: {0}", new object[] { propertyData.Value });
									}
								}
								else
								{
									Logger.Info("Description (Name): {0}", new object[] { propertyData.Value });
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while runninq query. Ex: ", new object[] { ex });
				Logger.Info("Ignoring and continuing...");
			}
			return num;
		}

		public static void ParseGLVersion(string glVersion, out double version)
		{
			try
			{
				string text;
				if (glVersion != null && glVersion.StartsWith("OpenGL", StringComparison.OrdinalIgnoreCase))
				{
					text = glVersion.Split(new char[] { '(' })[0].Trim();
					text = text.Split(new char[] { 'S' })[1].Trim();
				}
				else
				{
					text = glVersion.Split(new char[] { ' ' })[0].Trim();
					string[] array = text.Split(new char[] { '.' });
					text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
					{
						array[0],
						array[1]
					});
				}
				version = double.Parse(text, CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't parse for GL3 string: {0}", new object[] { glVersion });
				Logger.Error(ex.ToString());
				version = 0.0;
			}
		}

		public static string GetUpdatedBootParamsString(string var, string val, string oldBootParams)
		{
			Logger.Info("Attempting to update bootparam for {0}={1}", new object[] { var, val });
			bool flag = false;
			if (string.IsNullOrEmpty(val))
			{
				flag = true;
			}
			List<string[]> list;
			if (oldBootParams == null)
			{
				list = null;
			}
			else
			{
				list = (from part in oldBootParams.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
					select part.Split(new char[] { '=' }) into x
					where x.Length == 1
					select x).ToList<string[]>();
			}
			List<string[]> list2 = list;
			Dictionary<string, string> dictionary;
			if (oldBootParams == null)
			{
				dictionary = null;
			}
			else
			{
				dictionary = (from part in oldBootParams.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
					select part.Split(new char[] { '=' }) into x
					where x.Length == 2
					select x).ToDictionary((string[] split) => split[0], (string[] split) => split[1]);
			}
			Dictionary<string, string> dictionary2 = dictionary;
			if (flag)
			{
				string[] array = new string[] { var };
				bool flag2 = false;
				using (List<string[]>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Contains(var))
						{
							flag2 = true;
						}
					}
				}
				if (!flag2)
				{
					list2.Add(array);
					Logger.Info("BootParams added for {0}", new object[] { var, val });
				}
				else
				{
					Logger.Info("BootParam already present");
				}
			}
			else
			{
				dictionary2[var] = val;
				Logger.Info("BootParam added/updated");
			}
			List<string> list3 = dictionary2.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value).ToList<string>();
			list3.AddRange(list2.SelectMany((string[] x) => x));
			return string.Join(" ", list3.ToArray());
		}

		private static string GetServiceImagePath(string svcName)
		{
			string text = "SYSTEM\\CurrentControlSet\\Services\\" + svcName;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text))
			{
				if (registryKey != null)
				{
					return Environment.ExpandEnvironmentVariables(registryKey.GetValue("ImagePath", "").ToString());
				}
			}
			return "";
		}

		public static bool IsRunningInstanceClashWithService(string[] servicePrefixes, out ServiceController runningSvc)
		{
			Logger.Info("In IsRunningInstanceClashWithService");
			runningSvc = null;
			ServiceController[] devices = ServiceController.GetDevices();
			List<ServiceController> list = new List<ServiceController>();
			if (servicePrefixes != null)
			{
				foreach (ServiceController serviceController in devices)
				{
					foreach (string text in servicePrefixes)
					{
						if (serviceController.ServiceName.Contains(text))
						{
							list.Add(serviceController);
						}
					}
				}
			}
			string text2 = RegistryStrings.InstallDir.TrimEnd(new char[] { '\\' });
			foreach (ServiceController serviceController2 in list)
			{
				string text3 = Path.GetDirectoryName(Utils.GetServiceImagePath(serviceController2.ServiceName));
				text3 = text3.Substring(4, text3.Length - 4);
				if (!string.Equals(text2, text3, StringComparison.InvariantCultureIgnoreCase) && serviceController2.Status == ServiceControllerStatus.Running)
				{
					runningSvc = serviceController2;
					return true;
				}
			}
			return false;
		}

		public static double RoundUp(double input, int places)
		{
			double num = Math.Pow(10.0, Convert.ToDouble(places));
			return Math.Ceiling(input * num) / num;
		}

		public static void UpdateBlueStacksSizeToRegistryASync()
		{
			using (BackgroundWorker backgroundWorker = new BackgroundWorker())
			{
				backgroundWorker.DoWork += Utils.UpdateBlueStacksSizeToRegistry;
				backgroundWorker.RunWorkerAsync();
			}
		}

		private static void UpdateBlueStacksSizeToRegistry(object sender, DoWorkEventArgs e)
		{
			try
			{
				string text = string.Empty;
				if (SystemUtils.IsOs64Bit())
				{
					text = string.Format(CultureInfo.InvariantCulture, "{0}\\BlueStacks{1}", new object[]
					{
						"SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall",
						Strings.GetOemTag()
					});
				}
				else
				{
					text = string.Format(CultureInfo.InvariantCulture, "{0}\\BlueStacks{1}", new object[]
					{
						"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall",
						Strings.GetOemTag()
					});
				}
				long num = 0L;
				foreach (string text2 in RegistryManager.Instance.VmList.ToList<string>())
				{
					string text3 = Path.Combine(RegistryStrings.DataDir, text2);
					if (Directory.Exists(text3))
					{
						num += IOUtils.GetDirectorySize(text3);
					}
				}
				num /= 1048576L;
				num += 1000L;
				int num2 = Convert.ToInt32(num);
				Logger.Info("Updating {0}MB BlueStacks size to registry", new object[] { num2 });
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text, true))
				{
					if (registryKey != null)
					{
						registryKey.SetValue("EstimatedSize", num2 * 1024, RegistryValueKind.DWord);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Couldn't update size to registry, ignoring error. Ex: {0}", new object[] { ex.Message });
			}
		}

		public static object GetRegistryHKLMValue(string regPath, string key, object defaultValue)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regPath))
				{
					if (registryKey != null)
					{
						return registryKey.GetValue(key, defaultValue);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in getting the reistry value " + ex.Message);
			}
			return defaultValue;
		}

		public static object GetRegistryHKCUValue(string regPath, string key, object defaultValue)
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(regPath))
				{
					if (registryKey != null)
					{
						return registryKey.GetValue(key, defaultValue);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in getting the HKCU reistry value " + ex.Message);
			}
			return defaultValue;
		}

		public static void BackUpGuid(string userGUID)
		{
			try
			{
				StreamWriter streamWriter = new StreamWriter(Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_Guid_Backup"));
				streamWriter.Write(userGUID);
				streamWriter.Close();
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to backup guid...ignoring...printing exception");
				Logger.Error(ex.ToString());
			}
		}

		public static void SetAttributesNormal(DirectoryInfo dir)
		{
			foreach (DirectoryInfo directoryInfo in (dir != null) ? dir.GetDirectories("*", SearchOption.AllDirectories) : null)
			{
				Utils.SetAttributesNormal(directoryInfo);
				directoryInfo.Attributes = FileAttributes.Normal;
			}
			FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Attributes = FileAttributes.Normal;
			}
		}

		public static string GetString(string currentValue, string defaultValue)
		{
			if (string.IsNullOrEmpty(currentValue))
			{
				return defaultValue;
			}
			return currentValue;
		}

		public static int GetInt(int currentValue, int defaultValue)
		{
			if (currentValue == 0)
			{
				return defaultValue;
			}
			return currentValue;
		}

		public static ulong GeneratePseudoRandomNumber()
		{
			DateTime now = DateTime.Now;
			return (ulong)(((long)now.Month * 1000000000L + (long)now.DayOfWeek * 100000000L + (long)now.Day * 1000000L + (long)now.Hour * 1000L + (long)now.Minute * 100L + (long)now.Second) * (long)now.Millisecond);
		}

		public static string CreateRandomBstSharedFolder(string bstSharedFolder)
		{
			string text3;
			try
			{
				ulong num = Utils.GeneratePseudoRandomNumber();
				string text;
				string text2;
				for (;;)
				{
					text = string.Format(CultureInfo.InvariantCulture, "Bst_{0}", new object[] { Convert.ToString(num, CultureInfo.InvariantCulture) });
					text2 = Path.Combine(bstSharedFolder, text);
					if (!Directory.Exists(text2))
					{
						break;
					}
					num = Utils.GeneratePseudoRandomNumber();
				}
				Directory.CreateDirectory(text2);
				text3 = text;
			}
			catch (Exception ex)
			{
				Logger.Info("Failed to create random shared folder... Err : " + ex.ToString());
				throw new Exception("Failed to create Bst Shared Folder");
			}
			return text3;
		}

		public static string GetValueInBootParams(string name, string vmName, string bootparam = "", string oem = "bgp64")
		{
			if (oem == null)
			{
				oem = "bgp64";
			}
			string text = string.Empty;
			string text2 = bootparam;
			if (string.IsNullOrEmpty(text2))
			{
				if (oem != "bgp64")
				{
					text2 = RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters;
				}
				else
				{
					text2 = RegistryManager.Instance.Guest[vmName].BootParameters;
				}
			}
			Dictionary<string, string> dictionary = (from part in text2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				select part.Split(new char[] { '=' }) into x
				where x.Length == 2
				select x).ToDictionary((string[] split) => split[0], (string[] split) => split[1]);
			if (dictionary.ContainsKey(name))
			{
				text = dictionary[name];
			}
			return text;
		}

		public static string RemoveKeyFromBootParam(string key, string bootParam)
		{
			if (bootParam == null)
			{
				return "";
			}
			Dictionary<string, string> dictionary = (from part in bootParam.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				select part.Split(new char[] { '=' }) into x
				where x.Length == 2
				select x).ToDictionary((string[] split) => split[0], (string[] split) => split[1]);
			if (dictionary.ContainsKey(key))
			{
				dictionary.Remove(key);
			}
			return string.Join(" ", dictionary.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value).ToArray<string>());
		}

		public static void UpdateValueInBootParams(string name, string value, string vmName, bool addIfNotPresent = true, string oem = "bgp64")
		{
			if (oem == null)
			{
				oem = "bgp64";
			}
			string text;
			if (oem != "bgp64")
			{
				text = RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters;
			}
			else
			{
				text = RegistryManager.Instance.Guest[vmName].BootParameters;
			}
			Dictionary<string, string> dictionary = (from part in text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				select part.Split(new char[] { '=' }) into x
				where x.Length == 2
				select x).ToDictionary((string[] split) => split[0], (string[] split) => split[1]);
			if (dictionary.ContainsKey(name))
			{
				dictionary[name] = value;
			}
			else if (addIfNotPresent)
			{
				dictionary.Add(name, value);
			}
			List<string> list = dictionary.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value).ToList<string>();
			string text2 = string.Join(" ", list.ToArray());
			if (oem != "bgp64")
			{
				RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters = text2;
				return;
			}
			RegistryManager.Instance.Guest[vmName].BootParameters = text2;
		}

		public static string GetDisplayName(string vmName, string oem = "bgp64")
		{
			if (oem == "bgp64")
			{
				if ("Android".Equals(vmName, StringComparison.OrdinalIgnoreCase))
				{
					return Strings.ProductTopBarDisplayName;
				}
				if (!RegistryManager.Instance.Guest.ContainsKey(vmName))
				{
					if (vmName == null)
					{
						return null;
					}
					return vmName.Replace("Android_", Strings.ProductDisplayName + " ");
				}
				else
				{
					if (!string.IsNullOrEmpty(RegistryManager.Instance.Guest[vmName].DisplayName))
					{
						return RegistryManager.Instance.Guest[vmName].DisplayName;
					}
					if (vmName == null)
					{
						return null;
					}
					return vmName.Replace("Android_", Strings.ProductDisplayName + " ");
				}
			}
			else if (!RegistryManager.RegistryManagers[oem].Guest.ContainsKey(vmName))
			{
				if (vmName == null)
				{
					return null;
				}
				return vmName.Replace("Android_", Strings.ProductDisplayName + " ");
			}
			else
			{
				if (!string.IsNullOrEmpty(RegistryManager.RegistryManagers[oem].Guest[vmName].DisplayName))
				{
					return RegistryManager.RegistryManagers[oem].Guest[vmName].DisplayName;
				}
				if (vmName == null)
				{
					return null;
				}
				return vmName.Replace("Android_", Strings.ProductDisplayName + " ");
			}
		}

		public static bool IsAnyItemEmptyInStringList(List<string> strList)
		{
			if (strList != null)
			{
				using (List<string>.Enumerator enumerator = strList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (string.IsNullOrEmpty(enumerator.Current))
						{
							return false;
						}
					}
				}
				return true;
			}
			return true;
		}

		public static VirtualBox GetVirtualBoxSerialisedObejct(string filePath)
		{
			try
			{
				Utils.SaveFileInUnicode(filePath);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to save file in unicode encoding... Err : " + ex.ToString());
			}
			Utils.ReplaceOldVirtualBoxNamespaceWithNew(filePath);
			XmlReader xmlReader = XmlReader.Create(File.OpenRead(filePath));
			VirtualBox virtualBox = (VirtualBox)new XmlSerializer(typeof(VirtualBox)).Deserialize(xmlReader);
			xmlReader.Close();
			return virtualBox;
		}

		public static void ReplaceOldVirtualBoxNamespaceWithNew(string filePath)
		{
			Logger.Info("In ReplaceOldVirtualBoxNamespaceWithNew");
			string text = File.ReadAllText(filePath);
			string text2 = "http://www.innotek.de/VirtualBox-settings";
			string text3 = "http://www.virtualbox.org/";
			if (text.Contains(text2))
			{
				text = text.Replace(text2, text3);
				File.WriteAllText(filePath, text);
			}
		}

		private static void SaveFileInUnicode(string filePath)
		{
			string text = File.ReadAllText(filePath);
			using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.Unicode))
			{
				streamWriter.Write(text);
				streamWriter.Flush();
				streamWriter.Close();
			}
		}

		internal static Dictionary<string, string> AddCommonData(Dictionary<string, string> data)
		{
			if (!data.ContainsKey("install_id"))
			{
				data.Add("install_id", RegistryManager.Instance.InstallID);
			}
			if (!data.ContainsKey("launcher_version"))
			{
				data.Add("launcher_version", RegistryManager.Instance.WebAppVersion);
			}
			return data;
		}

		public static string GetVmIdFromVmName(string vmName)
		{
			if (vmName == "Android")
			{
				return "0";
			}
			if (vmName == null)
			{
				return null;
			}
			return vmName.Split(new char[] { '_' })[1];
		}

		public static string GetAppRunAppJsonArg(string appName, string pkgName)
		{
			JObject jobject = new JObject
			{
				{ "app_icon_url", "" },
				{ "app_name", appName },
				{ "app_url", "" },
				{ "app_pkg", pkgName }
			};
			return string.Format(CultureInfo.InvariantCulture, "-json \"{0}\"", new object[] { jobject.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0]).Replace("\"", "\\\"") });
		}

		public static void DeleteFiles(List<string> listOfFiles)
		{
			if (listOfFiles != null)
			{
				foreach (string text in listOfFiles)
				{
					if (!Utils.DeleteFile(text))
					{
						Logger.Warning("Couldn't delete file: {0}", new object[] { text });
					}
				}
			}
		}

		public static bool DeleteFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				try
				{
					File.Delete(filePath);
				}
				catch
				{
					return false;
				}
				return true;
			}
			return true;
		}

		public static void RemoveGamingRelatedFiles()
		{
			Logger.Info("Removing gaming related files");
			Utils.DeleteFiles(new List<string> { Path.Combine(RegistryManager.Instance.ClientInstallDir, "game_config.json") });
		}

		public static void UpgradeGamingRegistriesToFull()
		{
			Logger.Info("Setting registries for full edition");
			RegistryManager.Instance.InstallationType = InstallationTypes.FullEdition;
			RegistryManager.Instance.InstallerPkgName = string.Empty;
		}

		public static void UpgradeToFullVersionAndCreateBstShortcut(bool updateUninstallEntryToo = false)
		{
			Logger.Info("Upgrading to full version of BlueStacks");
			Utils.UpgradeGamingRegistriesToFull();
			Utils.RemoveGamingRelatedFiles();
			Utils.RemoveAdminRelatedRegistryAndFiles(updateUninstallEntryToo);
			CommonInstallUtils.CreateDesktopAndStartMenuShortcuts(Strings.ProductDisplayName, RegistryStrings.ProductIconCompletePath, Path.Combine(RegistryManager.Instance.ClientInstallDir, "BlueStacks.exe"), "", "", "");
			Utils.SHChangeNotify(134217728, 4096, IntPtr.Zero, IntPtr.Zero);
		}

		private static void RemoveAdminRelatedRegistryAndFiles(bool updateUninstallEntryToo)
		{
			Logger.Info("Removing admin related things");
			List<string> list = new List<string>
			{
				Path.Combine(RegistryStrings.InstallDir, "game_config.json"),
				Path.Combine(RegistryStrings.InstallDir, "app_icon.ico")
			};
			string[] files = Directory.GetFiles(RegistryStrings.InstallDir, "gameinstaller_*.png", SearchOption.TopDirectoryOnly);
			list.AddRange(files);
			string text = Path.Combine(Path.GetTempPath(), "RemoveGamingFiles.bat");
			using (StreamWriter streamWriter = new StreamWriter(text))
			{
				if (updateUninstallEntryToo)
				{
					Logger.Info("Exporting uninstall registry");
					int num = Utils.ExportUninstallEntry("\\BlueStacks" + Strings.GetOemTag(), Strings.UninstallRegistryExportedFilePath);
					Logger.Info("Exporting result: {0}", new object[] { num });
					Utils.UpdateUninstallRegistryFileForFullEdition(Strings.UninstallRegistryExportedFilePath);
					streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "REG IMPORT \"{0}\"", new object[] { Strings.UninstallRegistryExportedFilePath }));
				}
				foreach (string text2 in list)
				{
					streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "DEL /F /Q \"{0}\"", new object[] { text2 }));
				}
				streamWriter.Close();
			}
			Logger.Info("Executing: {0}", new object[] { text });
			Process process = new Process();
			process.StartInfo.Verb = "runas";
			process.StartInfo.FileName = text;
			process.StartInfo.WorkingDirectory = Path.GetTempPath();
			process.StartInfo.CreateNoWindow = true;
			try
			{
				process.Start();
				process.WaitForExit();
			}
			catch (Win32Exception ex)
			{
				Logger.Error("User cancelled UAC: " + ex.Message);
			}
			catch (Exception ex2)
			{
				Logger.Error("An error occured while executing the batch script: " + ex2.ToString());
			}
			finally
			{
				process.Dispose();
			}
			Logger.Info("All done!");
		}

		public static void UpdateUninstallEntryForFullEdition()
		{
			try
			{
				Logger.Info("Exporting uninstall registry");
				string uninstallRegistryExportedFilePath = Strings.UninstallRegistryExportedFilePath;
				int num = Utils.ExportUninstallEntry("\\BlueStacks" + Strings.GetOemTag(), uninstallRegistryExportedFilePath);
				Logger.Info("Exporting result: {0}", new object[] { num });
				Utils.UpdateUninstallRegistryFileForFullEdition(uninstallRegistryExportedFilePath);
				num = Utils.ImportRegistryFile(uninstallRegistryExportedFilePath, true);
				Logger.Info("Importing result: {0}", new object[] { num });
			}
			catch (Exception ex)
			{
				Logger.Warning("Couldn't update uninstall entry");
				Logger.Warning(ex.ToString());
			}
		}

		private static void UpdateUninstallRegistryFileForFullEdition(string regFilePath)
		{
			Logger.Info("Updating exported file for full version");
			List<string> list = new List<string>();
			using (StreamReader streamReader = new StreamReader(regFilePath))
			{
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					if (text.Contains("DisplayName"))
					{
						text = string.Format(CultureInfo.InvariantCulture, "\"DisplayName\"=\"{0}\"", new object[] { Oem.Instance.ControlPanelDisplayName });
					}
					else if (text.Contains("DisplayIcon"))
					{
						text = string.Format(CultureInfo.InvariantCulture, "\"DisplayIcon\"=\"{0}\"", new object[] { RegistryStrings.ProductIconCompletePath });
					}
					else if (text.Contains("Publisher"))
					{
						text = string.Format(CultureInfo.InvariantCulture, "\"Publisher\"=\"{0}\"", new object[] { "BlueStack Systems, Inc." });
					}
					list.Add(text);
				}
				streamReader.Close();
			}
			using (StreamWriter streamWriter = new StreamWriter(regFilePath))
			{
				foreach (string text2 in list)
				{
					streamWriter.WriteLine(text2);
				}
				streamWriter.Close();
			}
		}

		public static int ExportUninstallEntry(string keyName, string destFilePath)
		{
			string text = "Reg.exe";
			string text2 = string.Format(CultureInfo.InvariantCulture, "EXPORT HKLM\\{0}{1} \"{2}\"", new object[] { "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", keyName, destFilePath });
			if (File.Exists(destFilePath))
			{
				File.Delete(destFilePath);
			}
			return RunCommand.RunCmd(text, text2, true, true, false, 0).ExitCode;
		}

		public static int ImportRegistryFile(string regFilePath, bool requireAdminProc)
		{
			string text = "Reg.exe";
			string text2 = string.Format(CultureInfo.InvariantCulture, "IMPORT \"{0}\"", new object[] { regFilePath });
			return RunCommand.RunCmd(text, text2, true, true, requireAdminProc, 0).ExitCode;
		}

		public static string[] FixDuplicate7zArgs(string[] args)
		{
			string[] array;
			if (args == null || args.Length != 0)
			{
				if (args.Length == 1)
				{
					args[0] = args[0].Remove(args[0].Length / 2);
					array = args;
				}
				else
				{
					int num = args.Length / 2 + 1;
					array = new string[num];
					if (args[args.Length / 2].EndsWith(args[0], StringComparison.OrdinalIgnoreCase))
					{
						for (int i = 0; i <= num - 1; i++)
						{
							array[i] = args[i];
						}
						array[num - 1] = array[num - 1].Remove(array[num - 1].LastIndexOf(args[0], StringComparison.OrdinalIgnoreCase));
					}
				}
			}
			else
			{
				array = args;
			}
			return array;
		}

		public static string GetMultiInstanceEventName(string vmName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[] { "BstClient", vmName });
		}

		public static int GetVmIdToCreate(string oem = "bgp64")
		{
			int num = RegistryManager.Instance.VmId;
			if (oem == null)
			{
				oem = "bgp64";
			}
			if (oem != "bgp64")
			{
				num = RegistryManager.RegistryManagers[oem].VmId;
			}
			for (;;)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Android_{0}", new object[] { num });
				if (oem == "bgp64")
				{
					if (!Directory.Exists(Path.Combine(RegistryStrings.DataDir, text)))
					{
						break;
					}
					num++;
					Logger.Info("Incrementing vmId: {0}", new object[] { num });
				}
				else
				{
					if (!Directory.Exists(Path.Combine(RegistryManager.RegistryManagers[oem].EngineDataDir, text)))
					{
						break;
					}
					num++;
					Logger.Info("Incrementing vmId: {0}", new object[] { num });
				}
			}
			if (oem != "bgp64")
			{
				RegistryManager.RegistryManagers[oem].VmId = RegistryManager.RegistryManagers[oem].VmId + 1;
			}
			else
			{
				RegistryManager.Instance.VmId = RegistryManager.Instance.VmId + 1;
			}
			return num;
		}

		public static JsonSerializerSettings GetSerializerSettings()
		{
			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
			jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
			jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
			jsonSerializerSettings.Converters.Add(new StringEnumConverter());
			jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
			jsonSerializerSettings.Error = (EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>)Delegate.Combine(jsonSerializerSettings.Error, new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(Utils.JsonSerializer_Error));
			jsonSerializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
			return jsonSerializerSettings;
		}

		private static void JsonSerializer_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
		{
			e.ErrorContext.Handled = true;
			Logger.Error("Error loading JSON " + e.ErrorContext.Path + Environment.NewLine + e.ErrorContext.Error.ToString());
		}

		public static void OpenUrl(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch (Win32Exception)
			{
				try
				{
					Process.Start("IExplore.exe", url);
				}
				catch (Exception ex)
				{
					Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex.ToString());
				}
			}
			catch (Exception ex2)
			{
				Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex2.ToString());
			}
		}

		public static string GetDpiFromBootParameters(string bootParameterString)
		{
			string[] array = ((bootParameterString != null) ? bootParameterString.Split(new char[] { ' ' }) : null);
			string text = null;
			foreach (string text2 in array)
			{
				if (text2.StartsWith("DPI=", StringComparison.OrdinalIgnoreCase))
				{
					text = text2.Split(new char[] { '=' })[1];
					break;
				}
			}
			if (text == null)
			{
				text = "240";
			}
			return text;
		}

		public static void SetDPIInBootParameters(string bootParameterString, string updatedValue, string vmName, string oem = "bgp64")
		{
			string[] array = ((bootParameterString != null) ? bootParameterString.Split(new char[] { ' ' }) : null);
			string text = null;
			foreach (string text2 in array)
			{
				if (text2.StartsWith("DPI=", StringComparison.OrdinalIgnoreCase))
				{
					text = text2.Split(new char[] { '=' })[0];
					string text3 = text2.Split(new char[] { '=' })[1];
					if (text3 != updatedValue)
					{
						string text4 = string.Format(CultureInfo.InvariantCulture, "DPI={0}", new object[] { updatedValue });
						string text5 = string.Format(CultureInfo.InvariantCulture, "DPI={0}", new object[] { text3 });
						string text6 = bootParameterString.Replace(text5, text4);
						if (oem != "bgp64")
						{
							RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters = text6;
						}
						else
						{
							RegistryManager.Instance.Guest[vmName].BootParameters = text6;
						}
					}
				}
			}
			if (text == null)
			{
				string text7 = string.Format(CultureInfo.InvariantCulture, "DPI={0}", new object[] { updatedValue });
				string text8 = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { bootParameterString, text7 });
				if (oem != "bgp64")
				{
					RegistryManager.RegistryManagers[oem].Guest[vmName].BootParameters = text8;
					return;
				}
				RegistryManager.Instance.Guest[vmName].BootParameters = text8;
			}
		}

		public static IntPtr BringToFront(string windowName)
		{
			Logger.Info("Window name is = " + windowName);
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = InteropWindow.BringWindowToFront(windowName, false, true);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in bringing existing window to the foreground Err : " + ex.ToString());
			}
			return intPtr;
		}

		public static void SendChangeFPSToInstanceASync(string vmname, int newFps = 2147483647)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				bool flag = Interlocked.CompareExchange(ref Utils.s_isFpsChangeRunning, 1, 0) != 0;
				if (!flag)
				{
					try
					{
						VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "setfpsvalue?fps={0}", new object[] { (newFps == int.MaxValue) ? RegistryManager.Instance.Guest[vmname].FPS : newFps }), vmname);
					}
					catch (Exception ex)
					{
						string text = "Exception in SendChangeFPSToInstanceASync. Error: ";
						Exception ex2 = ex;
						Logger.Warning(text + ((ex2 != null) ? ex2.ToString() : null));
					}
					finally
					{
						Utils.s_isFpsChangeRunning = 0;
					}
				}
			});
		}

		public static void SendShowFPSToInstanceASync(string vmname, int isShowFPS)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { 
			{
				"isshowfps",
				isShowFPS.ToString(CultureInfo.InvariantCulture)
			} };
			HTTPUtils.SendRequestToEngineAsync("showFPS", dictionary, vmname, 0, null, true, 1, 0);
		}

		public static bool CheckMultiInstallBeforeRunQuitMultiInstall()
		{
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software");
				int num = 0;
				foreach (string text in registryKey.GetSubKeyNames())
				{
					if (text.StartsWith("BlueStacks", StringComparison.OrdinalIgnoreCase) && !text.StartsWith("BlueStacksGP", StringComparison.OrdinalIgnoreCase) && !text.StartsWith("BlueStacksInstaller", StringComparison.OrdinalIgnoreCase))
					{
						num++;
					}
				}
				if (num >= 2)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.Info("error at CheckMultiInstallBeforeRunQuitMultiInstall" + ex.ToString());
			}
			return false;
		}

		public static bool PingPartner(string oem, string vmName)
		{
			try
			{
				if (!string.IsNullOrEmpty(oem) && BstHttpClient.Get(string.Format(CultureInfo.InvariantCulture, "http://127.0.0.1:{0}/ping", new object[] { RegistryManager.RegistryManagers[oem].PartnerServerPort }), null, false, vmName, 0, 1, 0, false, "bgp64").Contains("success"))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to ping partner server. Exc: " + ex.ToString());
			}
			return false;
		}

		public static void WriteAgentPortInFile(int port)
		{
			Thread thread = new Thread(new ThreadStart(delegate
			{
				int i = 5;
				while (i > 0)
				{
					try
					{
						Utils.WriteToFile(Path.Combine(RegistryManager.Instance.UserDefinedDir, "bst_params.txt"), string.Format(CultureInfo.InvariantCulture, "agentserverport={0}", new object[] { port }), "agentserverport");
						break;
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to write agent port to bst_params.txt. Ex: " + ex.ToString());
					}
					Logger.Info("retrying..." + i.ToString());
					i--;
					Thread.Sleep(500);
				}
			}))
			{
				IsBackground = true
			};
			thread.Start();
		}

		private static void WriteToFile(string path, string text, string searchText = "")
		{
			bool flag = true;
			List<string> list = new List<string>();
			if (File.Exists(path))
			{
				foreach (string text2 in File.ReadAllLines(path))
				{
					if (text2.Contains("="))
					{
						if (text2.Contains(searchText))
						{
							list.Add(text);
							flag = false;
						}
						else
						{
							list.Add(text2);
						}
					}
				}
			}
			if (flag)
			{
				using (TextWriter textWriter = new StreamWriter(path, true))
				{
					textWriter.WriteLine(text);
					textWriter.Flush();
					return;
				}
			}
			using (TextWriter textWriter2 = new StreamWriter(path, false))
			{
				foreach (string text3 in list)
				{
					textWriter2.WriteLine(text3);
				}
				textWriter2.Flush();
			}
		}

		public static int RunQuitMultiInstall()
		{
			string installDir = RegistryStrings.InstallDir;
			string text = "HD-QuitMultiInstall.exe";
			int num;
			try
			{
				string text2 = Path.Combine(installDir, text);
				Process process = new Process();
				process.StartInfo.Arguments = "-in";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.FileName = text2;
				Logger.Info("Complete path to QuitMultiInstall: " + text2);
				if (Environment.OSVersion.Version.Major <= 5)
				{
					process.StartInfo.Verb = "runas";
				}
				Logger.Info("Utils: Starting QuitMultiInstall with -in");
				process.Start();
				process.WaitForExit();
				num = process.ExitCode;
			}
			catch (Exception ex)
			{
				Logger.Error("An error occured: {0}", new object[] { ex });
				Process process2 = new Process();
				process2.StartInfo.Arguments = "-in";
				process2.StartInfo.UseShellExecute = false;
				process2.StartInfo.CreateNoWindow = true;
				process2.StartInfo.FileName = text;
				process2.StartInfo.WorkingDirectory = installDir;
				Logger.Info("Running {0} with WorkingDir {1}", new object[] { text, installDir });
				if (Environment.OSVersion.Version.Major <= 5)
				{
					process2.StartInfo.Verb = "runas";
				}
				Logger.Info("Utils: Starting QuitMultiInstall with -in");
				process2.Start();
				process2.WaitForExit();
				num = process2.ExitCode;
			}
			return num;
		}

		public static bool WaitForBGPClientPing(int retries = 40)
		{
			while (retries > 0)
			{
				try
				{
					if (JArray.Parse(HTTPUtils.SendRequestToClient("ping", null, "Android", 1000, null, false, 1, 0, "bgp64"))[0]["success"].ToString().Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.Debug("got ping response from client");
						return true;
					}
				}
				catch
				{
				}
				retries--;
				Thread.Sleep(500);
			}
			return false;
		}

		public static IWin32Window GetIWin32Window(IntPtr handle)
		{
			return new OldWindow(handle);
		}

		public static string GetPackageNameFromAPK(string apkFile)
		{
			string text = null;
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
					process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "hd-aapt.exe");
					process.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "dump badging \"{0}\"", new object[] { apkFile });
					process.Start();
					string text2 = process.StandardOutput.ReadToEnd();
					process.WaitForExit();
					text = new Regex("package:\\sname='(.+?)'").Match(text2).Groups[1].Value.ToString(CultureInfo.InvariantCulture);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error getting apk name: {0}", new object[] { ex.Message });
			}
			return text;
		}

		public static string GetFileAssemblyVersion(string path)
		{
			string text = string.Empty;
			if (File.Exists(path))
			{
				try
				{
					text = FileVersionInfo.GetVersionInfo(path).FileVersion;
				}
				catch (Exception ex)
				{
					Logger.Error("Error in parsing file version information: {0}", new object[] { ex.Message });
				}
			}
			return text;
		}

		public static string GetHelperInstalledPath()
		{
			return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Helper"), "BlueStacksHelper.exe");
		}

		public static string GetHelperTaskDetailsJSon()
		{
			try
			{
				CmdRes taskQueryCommandOutput = TaskScheduler.GetTaskQueryCommandOutput("BlueStacksHelper");
				JObject jobject = new JObject();
				string[] array = taskQueryCommandOutput.StdOut.Split(new char[] { '\n' });
				string[] array2 = taskQueryCommandOutput.StdErr.Split(new char[] { '\n' });
				JObject jobject2 = new JObject();
				int num = 1;
				foreach (string text in array)
				{
					if (!string.IsNullOrEmpty(text))
					{
						jobject2.Add(string.Format(CultureInfo.InvariantCulture, "line{0}", new object[] { num }), text);
						num++;
					}
				}
				jobject.Add("stdout", jobject2.ToString());
				jobject2 = new JObject();
				num = 1;
				foreach (string text2 in array2)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						jobject2.Add(string.Format(CultureInfo.InvariantCulture, "line{0}", new object[] { num }), text2);
						num++;
					}
				}
				jobject.Add("stderr", jobject2.ToString());
				return jobject.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Some error while creating json of the QueryTask: {0}", new object[] { ex });
			}
			return "";
		}

		public static bool HasOneDayPassed(DateTime srcTime)
		{
			try
			{
				double totalMinutes = (DateTime.Now - srcTime).TotalMinutes;
				double num = 1440.0;
				if (totalMinutes > num)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Couldn't check if the req time has passed. Ex: {0}", new object[] { ex.Message });
			}
			return false;
		}

		public static string GetAndroidIDFromAndroid(string vmName)
		{
			string text = string.Empty;
			try
			{
				JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getAndroidID", null, vmName, 0, null, false, 1, 0, "bgp64"));
				if (jobject["result"].ToString() == "ok")
				{
					text = jobject["androidID"].ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in getting Android ID: {0}", new object[] { ex.ToString() });
			}
			return text;
		}

		public static string GetGoogleAdIDFromAndroid(string vmName)
		{
			string text = string.Empty;
			try
			{
				JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getGoogleAdID", null, vmName, 0, null, false, 1, 0, "bgp64"));
				if (jobject["result"].ToString() == "ok")
				{
					text = jobject["googleadid"].ToString();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in getting googleAd ID: {0}", new object[] { ex.ToString() });
			}
			return text;
		}

		public static void SetGoogleAdIdAndAndroidIdFromAndroid(string vmName)
		{
			Thread thread = new Thread(new ThreadStart(delegate
			{
				try
				{
					string googleAdIDFromAndroid = Utils.GetGoogleAdIDFromAndroid(vmName);
					if (!string.IsNullOrEmpty(googleAdIDFromAndroid))
					{
						RegistryManager.Instance.Guest[vmName].GoogleAId = UUID.Base64Encode(googleAdIDFromAndroid);
					}
					string androidIDFromAndroid = Utils.GetAndroidIDFromAndroid(vmName);
					if (!string.IsNullOrEmpty(androidIDFromAndroid))
					{
						RegistryManager.Instance.Guest[vmName].AndroidId = UUID.Base64Encode(androidIDFromAndroid);
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception while getting ids from android: {0}", new object[] { ex.ToString() });
				}
			}))
			{
				IsBackground = true
			};
			thread.Start();
		}

		public static string GetGoogleAdIdfromRegistry(string vmName)
		{
			string text = string.Empty;
			try
			{
				text = UUID.Base64Decode(RegistryManager.Instance.Guest[vmName].GoogleAId);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in decoding GoogleAid: {0}", new object[] { ex.ToString() });
			}
			return StringUtils.GetControlCharFreeString(text);
		}

		public static string GetAndroidIdfromRegistry(string vmName)
		{
			string text = string.Empty;
			try
			{
				text = UUID.Base64Decode(RegistryManager.Instance.Guest[vmName].AndroidId);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in decoding AndroidID ID: {0}", new object[] { ex.ToString() });
			}
			return StringUtils.GetControlCharFreeString(text);
		}

		public static void CreateMD5HashOfRootVdi()
		{
			Thread thread = new Thread(new ThreadStart(delegate
			{
				try
				{
					string blockDevice0Path = RegistryManager.Instance.DefaultGuest.BlockDevice0Path;
					RegistryManager.Instance.RootVdiMd5Hash = Utils.GetMD5HashFromFile(blockDevice0Path);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception while checking md5 hash of root.vdi: {0}", new object[] { ex.ToString() });
				}
			}))
			{
				IsBackground = true
			};
			thread.Start();
		}

		public static int GetMaxVmIdFromVmList(string[] vmList)
		{
			int num = 0;
			try
			{
				if (vmList != null)
				{
					foreach (string text in vmList)
					{
						if (!(text == "Android"))
						{
							int num2;
							int.TryParse(text.Split(new char[] { '_' })[1], out num2);
							if (num < num2)
							{
								num = num2;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in getting max VmId to create err:", new object[] { ex.ToString() });
			}
			return num + 1;
		}

		public static int GetRecommendedVCPUCount(bool isDefaultVm)
		{
			int num = 2;
			if (!isDefaultVm)
			{
				num = 1;
			}
			return num;
		}

		public static void SetTimeZoneInGuest(string vmName)
		{
			string text = TimeZone.CurrentTimeZone.StandardName;
			TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
			string text2 = utcOffset.ToString();
			if (text2[0] != '-')
			{
				text2 = string.Format(CultureInfo.InvariantCulture, "GMT+{0}", new object[] { text2 });
			}
			else
			{
				text2 = string.Format(CultureInfo.InvariantCulture, "GMT{0}", new object[] { text2 });
			}
			string text3 = TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now).ToString(CultureInfo.InvariantCulture);
			string text4 = SystemUtils.GetSysInfo("Select DaylightBias from Win32_TimeZone");
			string text5;
			if (text3.Equals("True", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(text4))
			{
				text5 = utcOffset.Add(new TimeSpan(0, Convert.ToInt32(text4, CultureInfo.InvariantCulture), 0)).ToString();
				if (text5[0] != '-')
				{
					text5 = string.Format(CultureInfo.InvariantCulture, "GMT+{0}", new object[] { text5 });
				}
				else
				{
					text5 = string.Format(CultureInfo.InvariantCulture, "GMT{0}", new object[] { text5 });
				}
			}
			else
			{
				text5 = text2;
			}
			if (Features.IsFeatureEnabled(4194304UL))
			{
				text5 = "GMT+08:00:00";
				text3 = "False";
				text4 = "0";
				text2 = "GMT+08:00:00";
				text = "??????";
			}
			else if ("bgp64".Equals("dmm", StringComparison.OrdinalIgnoreCase))
			{
				text5 = "GMT+09:00:00";
				text3 = "False";
				text4 = "0";
				text2 = "GMT+09:00:00";
				text = "??????";
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "baseUtcOffset", text5 },
				{ "isDaylightSavingTime", text3 },
				{ "daylightBias", text4 },
				{ "utcOffset", text2 },
				{ "standardName", text }
			};
			JObject jobject;
			VmCmdHandler.SendRequest("settz", dictionary, vmName, out jobject);
		}

		public static void RunHDQuit(bool isWaitForExit = false, bool isFromClient = false, bool overrideIgnoreAgent = false)
		{
			try
			{
				Logger.Info("Quit bluestacks called with args: {0}, {1}", new object[] { isWaitForExit, isFromClient });
				string text = Path.Combine(RegistryStrings.InstallDir, "HD-Quit.exe");
				using (Process process = new Process())
				{
					process.StartInfo.FileName = text;
					if (!FeatureManager.Instance.IsCustomUIForDMM && !overrideIgnoreAgent)
					{
						process.StartInfo.Arguments = "-ignoreAgent";
					}
					if (isFromClient)
					{
						ProcessStartInfo startInfo = process.StartInfo;
						startInfo.Arguments += " -isFromClient";
					}
					Logger.Debug("Quit Aguments = " + process.StartInfo.Arguments);
					process.Start();
					if (isWaitForExit)
					{
						process.WaitForExit();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Quit bluestacks failed: " + ex.ToString());
			}
		}

		public static JToken ExtractInfoFromXapk(string zipFilePath)
		{
			JToken jtoken = null;
			string text = Path.Combine(Path.GetTempPath(), Path.GetFileName(zipFilePath));
			if (File.Exists(Path.Combine(text, "manifest.json")))
			{
				jtoken = JToken.Parse(File.ReadAllText(Path.Combine(text, "manifest.json")));
			}
			return jtoken;
		}

		public static bool CheckIfDeviceProfileChanged(JObject mCurrentDeviceProfile, JObject mChangedDeviceProfile)
		{
			if (mCurrentDeviceProfile != null && mChangedDeviceProfile != null)
			{
				if (!string.Equals(mCurrentDeviceProfile["pcode"].ToString(), mChangedDeviceProfile["pcode"].ToString(), StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (!string.Equals(mCurrentDeviceProfile["caSelector"].ToString(), mChangedDeviceProfile["caSelector"].ToString(), StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (string.Equals(mCurrentDeviceProfile["pcode"].ToString(), "custom", StringComparison.OrdinalIgnoreCase) && (!string.Equals(mCurrentDeviceProfile["model"].ToString(), mChangedDeviceProfile["model"].ToString(), StringComparison.OrdinalIgnoreCase) || !string.Equals(mCurrentDeviceProfile["brand"].ToString(), mChangedDeviceProfile["brand"].ToString(), StringComparison.OrdinalIgnoreCase) || !string.Equals(mCurrentDeviceProfile["manufacturer"].ToString(), mChangedDeviceProfile["manufacturer"].ToString(), StringComparison.OrdinalIgnoreCase)))
				{
					return true;
				}
			}
			return false;
		}

		public static bool CheckForInternetConnection()
		{
			bool flag;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					using (webClient.OpenRead("http://connectivitycheck.gstatic.com/generate_204"))
					{
						flag = true;
					}
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static string[] AddVmNameInArgsIfNotPresent(string[] args)
		{
			if (!Utils.CheckIfVmNamePassedToArgs(args))
			{
				string text = Utils.CheckIfAnyVmRunning();
				List<string> list = args.ToList<string>();
				if (!string.IsNullOrEmpty(text))
				{
					list.Add("-vmname");
					list.Add(text);
				}
				args = list.ToArray();
			}
			return args;
		}

		private static string CheckIfAnyVmRunning()
		{
			foreach (object obj in RegistryManager.Instance.VmList)
			{
				string text = obj as string;
				if (Utils.CheckIfGuestReady(text, 1))
				{
					return text;
				}
			}
			return null;
		}

		private static bool CheckIfVmNamePassedToArgs(string[] args)
		{
			IEnumerable<string> enumerable = args.ToList<string>();
			IList<string> list = new List<string> { "--vmname", "-vmname", "vmname" };
			return enumerable.Intersect(list).Any<string>();
		}

		public static void SetAstcOption(string vmname, ASTCOption astcOption, string oem = "bgp64")
		{
			if (oem == null)
			{
				oem = "bgp64";
			}
			if (oem != "bgp64")
			{
				RegistryManager.RegistryManagers[oem].Guest[vmname].ASTCOption = astcOption;
			}
			else
			{
				RegistryManager.Instance.Guest[vmname].ASTCOption = astcOption;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text = "AstcOption";
			int num = (int)astcOption;
			dictionary.Add(text, num.ToString(CultureInfo.InvariantCulture));
			Dictionary<string, string> dictionary2 = dictionary;
			HTTPUtils.SendRequestToEngineAsync("setAstcOption", dictionary2, vmname, 0, null, true, 1, 0);
			if (oem != "bgp64")
			{
				Stats.SendMiscellaneousStatsAsync("ASTCOption", RegistryManager.RegistryManagers[oem].UserGuid, RegistryManager.RegistryManagers[oem].ClientVersion, "ASTCOption", astcOption.ToString(), null, null, null, null, vmname, 0);
				return;
			}
			Stats.SendMiscellaneousStatsAsync("ASTCOption", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "ASTCOption", astcOption.ToString(), null, null, null, null, vmname, 0);
		}

		public static void KillCurrentOemProcessByName(string[] nameList, string clientInstallDir = null)
		{
			if (nameList != null)
			{
				if (clientInstallDir == null)
				{
					clientInstallDir = string.Empty;
				}
				for (int i = 0; i < nameList.Length; i++)
				{
					Utils.KillCurrentOemProcessByName(nameList[i], clientInstallDir);
				}
			}
		}

		public static void KillCurrentOemProcessByName(string procName, string clientInstallDir = null)
		{
			Process[] processesByName = Process.GetProcessesByName(procName);
			string installDir = RegistryStrings.InstallDir;
			if (string.IsNullOrEmpty(clientInstallDir))
			{
				clientInstallDir = RegistryManager.Instance.ClientInstallDir;
			}
			foreach (Process process in processesByName)
			{
				try
				{
					string directoryName = Path.GetDirectoryName(GetProcessExecutionPath.GetApplicationPathFromProcess(process));
					if (directoryName.Equals(installDir.TrimEnd(new char[] { '\\' }), StringComparison.InvariantCultureIgnoreCase) || directoryName.Equals(clientInstallDir.TrimEnd(new char[] { '\\' }), StringComparison.InvariantCultureIgnoreCase) || directoryName.Equals(RegistryStrings.ObsDir, StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.Debug("Attempting to kill: {0}", new object[] { process.ProcessName });
						process.Kill();
						if (!process.WaitForExit(5000))
						{
							Logger.Info("Timeout waiting for process {0} to die", new object[] { process.ProcessName });
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in killing process " + ex.Message);
				}
			}
		}

		public static void EnableDisableApp(string appPackage, bool enable, string vmName)
		{
			try
			{
				JObject jobject = new JObject
				{
					{ "packagename", appPackage },
					{
						"enable",
						enable.ToString(CultureInfo.InvariantCulture)
					}
				};
				Dictionary<string, string> dictionary = new Dictionary<string, string> { 
				{
					"d",
					jobject.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0])
				} };
				HTTPUtils.SendRequestToGuestAsync("setapplicationstate", dictionary, vmName, 0, null, false, 1, 0);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in EnableDisableApp :" + ex.ToString());
			}
		}

		public static string GetInstalledAppDataFromAllVms()
		{
			string[] vmList = RegistryManager.Instance.VmList;
			string text = string.Empty;
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (JsonWriter jsonWriter = new JsonTextWriter(new StringWriter(stringBuilder))
				{
					Formatting = Newtonsoft.Json.Formatting.Indented
				})
				{
					jsonWriter.WriteStartObject();
					foreach (string text2 in vmList)
					{
						jsonWriter.WritePropertyName("vm" + Utils.GetVmIdFromVmName(text2));
						jsonWriter.WriteStartObject();
						jsonWriter.WritePropertyName("google_aid");
						jsonWriter.WriteValue(Utils.GetGoogleAdIdfromRegistry(text2));
						jsonWriter.WritePropertyName("android_id");
						jsonWriter.WriteValue(Utils.GetAndroidIdfromRegistry(text2));
						jsonWriter.WritePropertyName("installed_apps");
						jsonWriter.WriteStartObject();
						foreach (AppInfo appInfo in new JsonParser(text2).GetAppList().ToList<AppInfo>())
						{
							string package = appInfo.Package;
							string name = appInfo.Name;
							jsonWriter.WritePropertyName(package);
							jsonWriter.WriteValue(name);
						}
						jsonWriter.WriteEndObject();
						jsonWriter.WriteEndObject();
					}
					jsonWriter.WriteEndObject();
					text = stringBuilder.ToString();
					Logger.Debug("json string of all apps : " + stringBuilder.ToString());
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in getting all installed apps from all Vms: {0}", new object[] { ex.ToString() });
			}
			return text;
		}

		public static int GetTimeFromEpoch()
		{
			return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public static Dictionary<string, string> GetBootParamsDictFromBootParamString(string bootParams)
		{
			try
			{
				if (string.IsNullOrEmpty(bootParams))
				{
					return null;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (string text in bootParams.Split(new char[] { ' ' }).ToList<string>())
				{
					dictionary.Add(text.Split(new char[] { '=' })[0], text.Split(new char[] { '=' })[1]);
				}
				return dictionary;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in getting bootParamsDict err: " + ex.ToString());
			}
			return null;
		}

		public static GlMode GetGlModeForVm(string vmName)
		{
			int glRenderMode = RegistryManager.Instance.Guest[vmName].GlRenderMode;
			int glMode = RegistryManager.Instance.Guest[vmName].GlMode;
			GlMode glMode2;
			if (glRenderMode == 1 && glMode == 1)
			{
				glMode2 = GlMode.PGA_GL;
			}
			else if (glRenderMode == 1 && glMode == 2)
			{
				glMode2 = GlMode.AGA_GL;
			}
			else if (glMode == 1)
			{
				glMode2 = GlMode.PGA_DX;
			}
			else
			{
				if (glMode != 2)
				{
					throw new Exception("Could not determine the engine mode for current build.");
				}
				glMode2 = GlMode.AGA_DX;
			}
			return glMode2;
		}

		private const int SM_TABLETPC = 86;

		public const int BTV_RIGHT_PANEL_WIDTH = 320;

		private static bool sIsSyncAppJsonComplete = false;

		private const int SM_CXSCREEN = 0;

		private const int SM_CYSCREEN = 1;

		private static Dictionary<string, object> OemVmLockNamedata = new Dictionary<string, object>();

		public static readonly Dictionary<string, bool> sIsGuestBooted = new Dictionary<string, bool>();

		public static readonly Dictionary<string, bool> sIsGuestReady = new Dictionary<string, bool>();

		public static readonly Dictionary<string, bool> sIsSharedFolderMounted = new Dictionary<string, bool>();

		private const int PROC_KILL_TIMEOUT = 10000;

		private const int COMSERVER_EXIT_TIMEOUT = 5000;

		public static readonly List<string> sListIgnoredApps = new List<string>
		{
			"tv.gamepop.home", "com.pop.store", "com.pop.store51", "com.bluestacks.s2p5105", "mpi.v23", "com.google.android.gms", "com.google.android.gsf.login", "com.android.deskclock", "me.onemobile.android", "me.onemobile.lite.android",
			"android.rk.RockVideoPlayer.RockVideoPlayer", "com.bluestacks.chartapp", "com.bluestacks.setupapp", "com.android.gallery3d", "com.bluestacks.keymappingtool", "com.baidu.appsearch", "com.bluestacks.s2p", "com.bluestacks.windowsfilemanager", "com.android.quicksearchbox", "com.bluestacks.setup",
			"com.bluestacks.appsettings", "mpi.v23", "com.bluestacks.setup", "com.bluestacks.gamepophome", "com.bluestacks.appfinder", "com.android.providers.downloads.ui"
		};

		private static int s_isFpsChangeRunning;
	}
}


