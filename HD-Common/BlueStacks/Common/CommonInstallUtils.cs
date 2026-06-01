using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class CommonInstallUtils
	{
		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, [Out] StringBuilder pszPath);

		[DllImport("HD-LibraryHandler.dll", CharSet = CharSet.Auto)]
		private static extern int DeleteLibrary(string libraryName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process([In] IntPtr hProcess, out bool wow64Process);

		[DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegRenameKey(UIntPtr hKey, [MarshalAs(UnmanagedType.LPWStr)] string oldname, [MarshalAs(UnmanagedType.LPWStr)] string newname);

		[DllImport("HD-OpenGl-Native.dll")]
		public static extern int IsVulkanSupported();

		[DllImport("HD-OpenGl-Native.dll")]
		public static extern void PgaLoggerInit(Logger.HdLoggerCallback cb);

		private static bool InternalCheckIsWow64()
		{
			if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 6)
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					bool flag;
					if (!CommonInstallUtils.IsWow64Process(currentProcess.Handle, out flag))
					{
						return false;
					}
					return flag;
				}
				return false;
			}
			return false;
		}

		public static void KillBlueStacksProcesses(string clientInstallDir = null)
		{
			Logger.Info("Killing all BlueStacks processes");
			Utils.KillCurrentOemProcessByName(new string[]
			{
				"BlueStacks", "Keymapui", "BstkSVC", "HD-OBS", "HD-Player", "HD-Agent", "HD-Adb", "HD-RunApp", "HD-LogCollector", "HD-DataManager",
				"HD-QuitMultiInstall", "HD-MultiInstanceManager", "BlueStacksHelper"
			}, clientInstallDir);
		}

		public static string EngineInstallDir
		{
			get
			{
				return (string)Utils.GetRegistryHKLMValue(string.Format(CultureInfo.InvariantCulture, "Software\\BlueStacks{0}", new object[] { Strings.GetOemTag() }), "InstallDir", string.Empty);
			}
		}

		public static void RunHdQuit(string hdQuitPath)
		{
			try
			{
				string text = Path.Combine(hdQuitPath, "HD-Quit.exe");
				using (Process process = new Process())
				{
					process.StartInfo.FileName = text;
					process.Start();
					process.WaitForExit();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in running hd-quit err: {0}", new object[] { ex.ToString() });
			}
		}

		public static void ModifyDirectoryPermissionsForEveryone(string dir)
		{
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			try
			{
				string text = new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString();
				DirectoryInfo directoryInfo = new DirectoryInfo(dir);
				DirectorySecurity accessControl = directoryInfo.GetAccessControl();
				accessControl.AddAccessRule(new FileSystemAccessRule(text, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
				directoryInfo.SetAccessControl(accessControl);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to set permissions. err: " + ex.ToString());
			}
			try
			{
				if (!SystemUtils.IsOSWinXP())
				{
					foreach (string text2 in Directory.GetFiles(dir))
					{
						string text3 = new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString();
						FileInfo fileInfo = new FileInfo(text2);
						FileSecurity accessControl2 = fileInfo.GetAccessControl();
						accessControl2.AddAccessRule(new FileSystemAccessRule(text3, FileSystemRights.FullControl, AccessControlType.Allow));
						fileInfo.SetAccessControl(accessControl2);
					}
				}
				string[] array = Directory.GetDirectories(dir);
				for (int i = 0; i < array.Length; i++)
				{
					CommonInstallUtils.ModifyDirectoryPermissionsForEveryone(array[i]);
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Failed to set permissions. err: " + ex2.ToString());
			}
		}

		public static bool MoveDirectory(string srcDir, string dstDir)
		{
			Logger.Info("Moving directory {0} to {1}", new object[] { srcDir, dstDir });
			try
			{
				if (Directory.Exists(dstDir))
				{
					Directory.Delete(dstDir, true);
				}
				Directory.Move(srcDir, dstDir);
			}
			catch (Exception ex)
			{
				Logger.Info("------------ FOR DEV TRACKING--------------- Moving failed");
				Logger.Info("Caught exception when moving directory {0} to {1} err :{2}", new object[]
				{
					srcDir,
					dstDir,
					ex.ToString()
				});
				if (!Directory.Exists(dstDir))
				{
					Directory.CreateDirectory(dstDir);
				}
				foreach (string text in Directory.GetFiles(srcDir))
				{
					FileInfo fileInfo = new FileInfo(text);
					string text2 = Path.Combine(dstDir, fileInfo.Name);
					try
					{
						if (File.Exists(text2))
						{
							File.SetAttributes(text, FileAttributes.Normal);
							File.Delete(text2);
						}
						File.Move(text, text2);
					}
					catch (Exception ex2)
					{
						Logger.Info("Exception in file move {0} to {1}. Copying instead.. ex:{2}", new object[]
						{
							text,
							text2,
							ex2.ToString()
						});
						try
						{
							File.Copy(text, text2, true);
						}
						catch (Exception ex3)
						{
							Logger.Error("Exception in file copy: THIS WILL RESULT IN DEPLOYMENT FAILURE" + ex3.ToString());
							return false;
						}
					}
				}
				string[] array = Directory.GetDirectories(srcDir);
				for (int i = 0; i < array.Length; i++)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(array[i]);
					string text3 = Path.Combine(dstDir, directoryInfo.Name);
					string text4 = Path.Combine(srcDir, directoryInfo.Name);
					if (!CommonInstallUtils.MoveDirectory(text4, text3))
					{
						Logger.Warning("Returing false in directory move for {0} to {1}", new object[] { text4, text3 });
						return false;
					}
				}
				try
				{
					Directory.Delete(srcDir);
				}
				catch (Exception ex4)
				{
					Logger.Info("Ignoring exception when trying to delete srcDir at the end err:{0}", new object[] { ex4.ToString() });
				}
			}
			return true;
		}

		public static string GetFolderPath(int CSIDL)
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			CommonInstallUtils.SHGetFolderPath(IntPtr.Zero, CSIDL, IntPtr.Zero, 0U, stringBuilder);
			return stringBuilder.ToString();
		}

		public static void DeleteLegacyShortcuts()
		{
			Logger.Info("Deleting legacy shortcuts");
			ShortcutHelper.DeleteDesktopShortcut("Start BlueStacks.lnk");
			ShortcutHelper.DeleteStartMenuShortcut("Start BlueStacks.lnk");
			ShortcutHelper.DeleteStartMenuShortcut("Programs\\BlueStacks\\Start BlueStacks.lnk");
			if (!string.IsNullOrEmpty(Oem.Instance.DesktopShortcutFileName))
			{
				ShortcutHelper.DeleteDesktopShortcut(Oem.Instance.DesktopShortcutFileName);
				ShortcutHelper.DeleteStartMenuShortcut(Oem.Instance.DesktopShortcutFileName);
			}
			if (Oem.Instance.CreateMultiInstanceManagerIcon)
			{
				ShortcutHelper.DeleteDesktopShortcut(Oem.Instance.MultiInstanceManagerShortcutFileName);
				ShortcutHelper.DeleteStartMenuShortcut(Oem.Instance.MultiInstanceManagerShortcutFileName);
			}
			try
			{
				string text = Path.Combine(CommonInstallUtils.GetFolderPath(25), Oem.Instance.DesktopShortcutFileName);
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
			catch
			{
			}
		}

		public static bool CheckIfSDCardPresent()
		{
			return File.Exists(Path.Combine(Path.Combine(RegistryStrings.DataDir, "Android"), "SDCard.vdi"));
		}

		public static void DeleteOldShortcuts()
		{
			try
			{
				CommonInstallUtils.DeleteLegacyShortcuts();
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to delete old shortcuts. Err: " + ex.ToString());
			}
		}

		public static void CreateDesktopAndStartMenuShortcuts(string shortcutName, string iconPath, string targetBinPath, string args = "", string description = "", string package = "")
		{
			try
			{
				if (Oem.Instance.IsCreateDesktopAndStartMenuShortcut)
				{
					if (!string.IsNullOrEmpty(shortcutName))
					{
						if (string.IsNullOrEmpty(description))
						{
							description = shortcutName;
						}
						ShortcutHelper.CreateCommonDesktopShortcut(shortcutName, iconPath, targetBinPath, args, description, package);
						ShortcutHelper.CreateCommonStartMenuShortcut(shortcutName, iconPath, targetBinPath, args, shortcutName, package);
						if (!File.Exists(Path.Combine(ShortcutHelper.CommonDesktopPath, shortcutName.Replace(".lnk", "") + ".lnk")))
						{
							Logger.Info("Failed to make common desktop shortcut...atempting user desktop");
							ShortcutHelper.CreateDesktopShortcut(shortcutName, iconPath, targetBinPath, args, description, package);
						}
						Utils.SHChangeNotify(134217728, 4096, IntPtr.Zero, IntPtr.Zero);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to create BlueStacks shortcuts. ex: " + ex.ToString());
			}
		}

		public static void UpdateOldAppDesktopShortcutTarget(string oldInstallDir, string newInstallDir)
		{
			try
			{
				foreach (string text in Directory.GetFiles(ShortcutHelper.sDesktopPath, "*.lnk", SearchOption.AllDirectories))
				{
					try
					{
						if (File.Exists(text) && ShortcutHelper.GetShortcutArguments(text).TrimEnd(new char[] { '\\' }).ToLower(CultureInfo.InvariantCulture)
							.Equals(Path.Combine(oldInstallDir, "HD-RunApp.exe").ToLower(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
						{
							string text2 = ShortcutHelper.GetShortcutIconLocation(text);
							if (text2.ToLower(CultureInfo.InvariantCulture).Contains("library\\icons"))
							{
								text2 = text2.Replace("Library\\Icons", "Gadget");
								ShortcutHelper.UpdateTargetPathAndIcon(text, Path.Combine(newInstallDir, "HD-RunApp.exe"), text2);
							}
						}
					}
					catch
					{
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to fix app shortcut target");
				Logger.Error(ex.ToString());
			}
		}

		public static void CreatePartnerRegistryEntry(string clientInstallDir)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Software\\BlueStacks{0}\\Config", new object[] { Strings.GetOemTag() });
			Logger.Info("CreatePartnerRegistryEntry start");
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text, true))
			{
				if (registryKey != null)
				{
					registryKey.SetValue("PartnerExePath", Path.Combine(clientInstallDir, "BlueStacks.exe"));
				}
				else
				{
					Logger.Info("Not writing partner exe path , registry not exists");
				}
			}
		}

		public static bool RecreateAddRemoveRegistry(string pfDir, string iconPath, string displayName, string publisher)
		{
			try
			{
				string text = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\BlueStacks" + Strings.GetOemTag();
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(text))
				{
					registryKey.SetValue("DisplayName", displayName);
					registryKey.SetValue("DisplayVersion", "4.220.0.4001");
					registryKey.SetValue("DisplayIcon", iconPath);
					registryKey.SetValue("EstimatedSize", 2096128, RegistryValueKind.DWord);
					registryKey.SetValue("InstallDate", string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMdd}", new object[] { DateTime.Now }));
					registryKey.SetValue("NoModify", "1", RegistryValueKind.DWord);
					registryKey.SetValue("NoRepair", "1", RegistryValueKind.DWord);
					registryKey.SetValue("Publisher", publisher);
					registryKey.SetValue("UninstallString", Path.Combine(pfDir, "BlueStacksUninstaller.exe -tmp"));
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.Info("Error in creating ControlPanel registry: {0}", new object[] { ex.ToString() });
			}
			return false;
		}

		public static List<string> GetUninstallCurrentVersionSubKey(string keyToSearch)
		{
			List<string> list = new List<string>();
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", true))
			{
				foreach (string text in registryKey.GetSubKeyNames())
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
					{
						string text2 = (string)registryKey2.GetValue("DisplayName");
						if (text2 != null && text2.Equals(keyToSearch, StringComparison.OrdinalIgnoreCase))
						{
							list.Add(text);
						}
					}
				}
			}
			return list;
		}

		public static CmdRes ExtractZip(string zipFilePath, string extractDirectory)
		{
			string text = Path.Combine(Directory.GetCurrentDirectory(), "7zr.exe");
			if (!Directory.Exists(extractDirectory))
			{
				Directory.CreateDirectory(extractDirectory);
			}
			string text2 = string.Format(CultureInfo.InvariantCulture, "x \"{0}\" -o\"{1}\" -aoa", new object[] { zipFilePath, extractDirectory });
			return RunCommand.RunCmd(text, text2, false, true, false, 0);
		}

		public static void CopyLogFileToLogDir(string logFilePath, string destFileName)
		{
			try
			{
				string text = Path.Combine(RegistryManager.Instance.LogDir, destFileName);
				File.Copy(logFilePath, text, true);
			}
			catch (Exception ex)
			{
				Logger.Error("Got exception when copying isntaller logs in log dir ex :{0}", new object[] { ex.ToString() });
			}
		}

		public static void DeleteDirectory(List<string> directories, bool throwError = false)
		{
			if (directories != null)
			{
				foreach (string text in directories)
				{
					try
					{
						CommonInstallUtils.DeleteDirectory(text);
					}
					catch (Exception ex)
					{
						Logger.Warning("Failed to delete directory {0}, ignoring", new object[] { text });
						Logger.Warning(ex.Message);
						if (throwError)
						{
							throw;
						}
					}
				}
			}
		}

		public static void DeleteDirectory(string targetDir)
		{
			if (string.IsNullOrEmpty(targetDir) || CommonInstallUtils.sDisallowedDeletionStrings.Any((string str) => str.Equals(targetDir, StringComparison.CurrentCultureIgnoreCase)))
			{
				Logger.Warning("A hazardous DirectoryDelete for '{0}' was invoked. Ignoring the call", new object[] { targetDir });
				return;
			}
			try
			{
				Logger.Info("Deleting directory : " + targetDir);
				Directory.Delete(targetDir, true);
			}
			catch (DirectoryNotFoundException)
			{
				Logger.Warning("Could not delete {0} as it does not exist", new object[] { targetDir });
			}
			catch (Exception ex)
			{
				Logger.Warning("Got exception when deleting {0}, err:{1}", new object[]
				{
					targetDir,
					ex.ToString()
				});
				Logger.Info("------------- FOR DEV TRACKING --------------");
				foreach (string text in Directory.GetFiles(targetDir))
				{
					File.SetAttributes(text, FileAttributes.Normal);
					File.Delete(text);
				}
				string[] array = Directory.GetDirectories(targetDir);
				for (int i = 0; i < array.Length; i++)
				{
					CommonInstallUtils.DeleteDirectory(array[i]);
				}
				try
				{
					Directory.Delete(targetDir, true);
				}
				catch (IOException)
				{
					Thread.Sleep(100);
					try
					{
						Directory.Delete(targetDir, true);
					}
					catch
					{
					}
				}
				catch (UnauthorizedAccessException)
				{
					Thread.Sleep(100);
					try
					{
						Directory.Delete(targetDir, true);
					}
					catch
					{
					}
				}
			}
		}

		public static void GetScreenResolution(out int windowWidth, out int windowHeight, out int guestWidth, out int guestHeight)
		{
			double num = 1.7777777777777777;
			double num2;
			double num3;
			double num4;
			WpfUtils.GetDefaultSize(out num2, out num3, out num4, num, true);
			windowWidth = (int)(num2 - 17.0) / 16 * 16;
			windowHeight = windowWidth / 16 * 9;
			Utils.GetGuestWidthAndHeight(windowWidth, windowHeight, out guestWidth, out guestHeight);
			if (Oem.Instance.IsPixelParityToBeIgnored)
			{
				Utils.GetWindowWidthAndHeight(out guestWidth, out guestHeight);
			}
			Logger.Info("Resolution values: guestWidth: {0} guestHeight: {1} widowWidth: {2} windowHeight: {3}", new object[] { guestWidth, guestHeight, windowWidth, windowHeight });
		}

		public static void CreateWebUrlScheme(string clientInstallDir)
		{
			try
			{
				string bgpkeyName = Strings.BGPKeyName;
				using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(bgpkeyName))
				{
					registryKey.SetValue("", "BlueStacks Web Url Scheme");
					registryKey.SetValue("URL Protocol", "");
				}
				using (RegistryKey registryKey2 = Registry.ClassesRoot.CreateSubKey(bgpkeyName + "\\DefaultIcon"))
				{
					registryKey2.SetValue("", Path.Combine(clientInstallDir, "ProductLogo.ico"));
				}
				using (RegistryKey registryKey3 = Registry.ClassesRoot.CreateSubKey(bgpkeyName + "\\Shell"))
				{
					registryKey3.SetValue("", "open");
				}
				using (RegistryKey registryKey4 = Registry.ClassesRoot.CreateSubKey(bgpkeyName + "\\Shell\\open"))
				{
					registryKey4.SetValue("", "Open BlueStacks Game Platform");
				}
				using (RegistryKey registryKey5 = Registry.ClassesRoot.CreateSubKey(bgpkeyName + "\\Shell\\open\\command"))
				{
					string text = Path.Combine(clientInstallDir, "Bluestacks.exe");
					string text2 = string.Format(CultureInfo.InvariantCulture, "\"{0}\" %1", new object[] { text });
					registryKey5.SetValue("", text2);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to create web url scheme. Err: " + ex.ToString());
			}
		}

		public static void CreateInstallApkScheme(string installDir, string baseKeyName, string targetBinary)
		{
			Logger.Info("CreateInstallApkScheme start");
			try
			{
				using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(baseKeyName))
				{
					registryKey.SetValue("", string.Format(CultureInfo.InvariantCulture, "{0} Android Package File", new object[] { Strings.ProductDisplayName }));
				}
				using (RegistryKey registryKey2 = Registry.ClassesRoot.CreateSubKey(baseKeyName + "\\DefaultIcon"))
				{
					registryKey2.SetValue("", RegistryStrings.ProductIconCompletePath);
				}
				using (RegistryKey registryKey3 = Registry.ClassesRoot.CreateSubKey(baseKeyName + "\\Shell"))
				{
					registryKey3.SetValue("", "open");
				}
				using (RegistryKey registryKey4 = Registry.ClassesRoot.CreateSubKey(baseKeyName + "\\Shell\\open"))
				{
					registryKey4.SetValue("", string.Format(CultureInfo.InvariantCulture, "Open with {0} APK Installer", new object[] { Strings.ProductDisplayName }));
				}
				using (RegistryKey registryKey5 = Registry.ClassesRoot.CreateSubKey(baseKeyName + "\\Shell\\open\\command"))
				{
					string text = Path.Combine(installDir, targetBinary);
					string text2 = string.Format(CultureInfo.InvariantCulture, "{0} \"%1\"", new object[] { text });
					registryKey5.SetValue("", text2);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Some error while setting APKHandler extention association, ex: " + ex.Message);
			}
			try
			{
				if (baseKeyName != null && baseKeyName.Equals("BlueStacks.Apk", StringComparison.OrdinalIgnoreCase))
				{
					using (RegistryKey registryKey6 = Registry.ClassesRoot.CreateSubKey(".apk"))
					{
						registryKey6.SetValue("", baseKeyName, RegistryValueKind.String);
						goto IL_01D8;
					}
				}
				using (RegistryKey registryKey7 = Registry.ClassesRoot.CreateSubKey(".xapk"))
				{
					registryKey7.SetValue("", baseKeyName, RegistryValueKind.String);
				}
				IL_01D8:;
			}
			catch (Exception ex2)
			{
				Logger.Warning("Some error while setting main .apk extention association, ex: " + ex2.Message);
			}
			Logger.Info("CreateInstallApkScheme end");
		}

		public static void DeleteInstallApkScheme(string installDir, string baseKeyName)
		{
			Logger.Info("DeleteInstallApkScheme start");
			try
			{
				string text = "junkPath";
				using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(baseKeyName + "\\Shell\\open\\command"))
				{
					text = (string)registryKey.GetValue("");
				}
				text = text.Trim(new char[] { '"' });
				installDir = ((installDir != null) ? installDir.Trim(new char[] { '"' }) : null);
				if (text.StartsWith(installDir, StringComparison.OrdinalIgnoreCase))
				{
					Registry.ClassesRoot.DeleteSubKeyTree(baseKeyName);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to delete apk handler registry. Ex : " + ex.ToString());
			}
			Logger.Info("DeleteInstallApkScheme end");
		}

		public static void LogAllServiceNames()
		{
			try
			{
				Logger.Info("Installed kernel level services:-");
				foreach (ServiceController serviceController in ServiceController.GetDevices())
				{
					Logger.Info("ServiceName: {0},\tDisplayName: {1}, status: {2}", new object[] { serviceController.ServiceName, serviceController.DisplayName, serviceController.Status });
				}
				Logger.Info("Installed services:-");
				foreach (ServiceController serviceController2 in ServiceController.GetServices())
				{
					Logger.Info("ServiceName: {0},\tDisplayName: {1}, status: {2}", new object[] { serviceController2.ServiceName, serviceController2.DisplayName, serviceController2.Status });
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Some error occured while logging services, ex: " + ex.Message);
			}
		}

		public static void CheckForActiveHandles(string installerExtractedPath)
		{
			Logger.Info("Checking for active Handles");
			try
			{
				RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Sysinternals\\Handle", true);
				if (registryKey == null)
				{
					Registry.CurrentUser.CreateSubKey("Software\\Sysinternals\\Handle");
				}
				else
				{
					registryKey.Close();
				}
				using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Software\\Sysinternals\\Handle", true))
				{
					registryKey2.SetValue("EulaAccepted", 1, RegistryValueKind.DWord);
					Logger.Info("Accepted");
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Couldn't access registry, ex: {0}", new object[] { ex.Message });
			}
			try
			{
				string text = Path.Combine(installerExtractedPath, "HD-Handle.exe");
				string text2 = "bluestacks";
				RunCommand.RunCmd(text, text2, true, true, false, 0);
			}
			catch (Exception ex2)
			{
				Logger.Warning("Couldn't check for active handles, ex: {0}", new object[] { ex2.Message });
			}
		}

		public static void RemoveAndAddFirewallRule(string ruleName, string binPath)
		{
			CommonInstallUtils.RemoveFirewallRule(ruleName);
			CommonInstallUtils.AddFirewallRule(ruleName, binPath);
		}

		public static void AddFirewallRule(string ruleName, string binPath)
		{
			try
			{
				string text = "netsh.exe";
				string text2 = string.Format(CultureInfo.InvariantCulture, "advfirewall firewall add rule name=\"{0}\" dir=in action=allow program=\"{1}\" enable=yes", new object[] { ruleName, binPath });
				RunCommand.RunCmd(text, text2, false, true, false, 0);
			}
			catch (Exception ex)
			{
				Logger.Error("Error in adding firewall rule", new object[] { ex.ToString() });
			}
		}

		public static void RemoveFirewallRule(string ruleName)
		{
			try
			{
				string text = "netsh.exe";
				string text2 = string.Format(CultureInfo.InvariantCulture, "advfirewall firewall delete rule name=\"{0}\"", new object[] { ruleName });
				RunCommand.RunCmd(text, text2, false, true, false, 0);
			}
			catch (Exception ex)
			{
				Logger.Error("{0} Firewall rule dont exist {1}", new object[]
				{
					ruleName,
					ex.ToString()
				});
			}
		}

		public static bool CreateHardLinkForFile(string existingFilePath, string newFilePath)
		{
			try
			{
				Logger.Info("Creating link from " + existingFilePath + " to " + newFilePath);
				if (!CommonInstallUtils.CreateHardLink(newFilePath, existingFilePath, IntPtr.Zero))
				{
					Logger.Error("Failed to create hard link: " + Marshal.GetLastWin32Error().ToString());
					if (File.Exists(existingFilePath))
					{
						Logger.Error("Copying the file instead");
						File.Copy(existingFilePath, newFilePath, true);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Failed in creating hard link, Ex: " + ex.ToString());
			}
			return false;
		}

		public static bool CreateHardLinkForDirectory(string existingDirPath, string newDirPath)
		{
			try
			{
				Logger.Info("Creating link from " + existingDirPath + " to " + newDirPath);
				if (!Directory.Exists(newDirPath))
				{
					Directory.CreateDirectory(newDirPath);
				}
				foreach (FileInfo fileInfo in new DirectoryInfo(existingDirPath).GetFiles())
				{
					string fullName = fileInfo.FullName;
					string text = Path.Combine(newDirPath, fileInfo.Name);
					if (!CommonInstallUtils.CreateHardLinkForFile(fullName, text))
					{
						Logger.Error("Failed to create hard link for file : " + fileInfo.FullName);
						return false;
					}
				}
				foreach (DirectoryInfo directoryInfo in new DirectoryInfo(existingDirPath).GetDirectories())
				{
					if (!CommonInstallUtils.CreateHardLinkForDirectory(Path.Combine(existingDirPath, directoryInfo.Name), Path.Combine(newDirPath, directoryInfo.Name)))
					{
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to create hard link for directory, Err: " + ex.ToString());
			}
			return false;
		}

		public static bool IsDiskFull(Exception ex)
		{
			int num = Marshal.GetHRForException(ex) & 65535;
			Logger.Info("Win32 error code: " + num.ToString());
			Logger.Info("Disk full error codes are: {0} and {1}", new object[] { 112, 39 });
			return num == 39 || num == 112;
		}

		public static bool StopAndUninstallService(string svcName, int timeoutSeconds = 15, bool isKernelDriver = false)
		{
			Logger.Info("Uninstalling service {0}", new object[] { svcName });
			bool flag;
			try
			{
				CommonInstallUtils.StopService(svcName, timeoutSeconds);
				CommonInstallUtils.UninstallService(svcName, isKernelDriver);
				flag = true;
			}
			catch (Exception ex)
			{
				Logger.Warning("Ignoring exception when uninstalling service {0} ex : {1}", new object[]
				{
					svcName,
					ex.ToString()
				});
				flag = false;
			}
			return flag;
		}

		private static void StopService(string serviceName, int timeoutSeconds = 15)
		{
			Logger.Info("Stopping service {0} with timeout {1}s", new object[] { serviceName, timeoutSeconds });
			using (ServiceController serviceController = new ServiceController(serviceName))
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)timeoutSeconds);
				if (serviceController.Status == ServiceControllerStatus.Stopped || serviceController.Status == ServiceControllerStatus.StopPending)
				{
					serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeSpan);
					Logger.Info("Service {0} stopped", new object[] { serviceName });
				}
				else
				{
					try
					{
						Logger.Info("Service is running , stopping the service {0}", new object[] { serviceName });
						serviceController.Stop();
						serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeSpan);
						Logger.Info("Service stopped successfully");
					}
					catch (global::System.ServiceProcess.TimeoutException)
					{
						Logger.Error("Timed out while waiting for service to stop");
						throw;
					}
					catch (Exception ex)
					{
						Logger.Error("Got exception stopping service {0}, Err: {1}", new object[]
						{
							serviceName,
							ex.ToString()
						});
						throw;
					}
				}
			}
		}

		private static void UninstallService(string name, bool isKernelDriverService = false)
		{
			ServiceManager.UninstallService(name, isKernelDriverService);
		}

		public static bool DoesRegistryHKLMKeyExist(string keyPath)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyPath);
			bool flag = registryKey != null;
			if (flag)
			{
				registryKey.Close();
			}
			return flag;
		}

		public static string ConvertIntToEnumString(int enumCode)
		{
			return Enum.GetName(typeof(InstallerCodes), (InstallerCodes)enumCode);
		}

		public static string GetCurrentLocale()
		{
			string text;
			try
			{
				text = Thread.CurrentThread.CurrentCulture.Name;
			}
			catch
			{
				text = "en-US";
			}
			return text;
		}

		public static void StopBlueStacksIfUpgrade(bool isUpgrade, List<string> svcNames, string clientInstallDir, out bool isServiceStopped)
		{
			isServiceStopped = true;
			if (isUpgrade)
			{
				CommonInstallUtils.KillBlueStacksProcesses(clientInstallDir);
				CommonInstallUtils.RunHdQuit(CommonInstallUtils.EngineInstallDir);
				if (svcNames != null)
				{
					foreach (string text in svcNames)
					{
						ServiceManager.StopService(text, false);
						isServiceStopped = isServiceStopped && CommonInstallUtils.CheckIfServiceStopped(text);
					}
				}
			}
		}

		private static bool CheckIfServiceStopped(string svcName)
		{
			try
			{
				Logger.Info("Checking if {0} is stopped", new object[] { svcName });
				ServiceController[] devices = ServiceController.GetDevices();
				int i = 0;
				while (i < devices.Length)
				{
					ServiceController serviceController = devices[i];
					if (serviceController.ServiceName == svcName)
					{
						ServiceControllerStatus status = serviceController.Status;
						Logger.Info("Service status of {0} is {1}", new object[] { svcName, status });
						if (status != ServiceControllerStatus.Stopped)
						{
							Logger.Warning("Service is not stopped, returning false");
							return false;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("An error occured while checking if svc is stopped, ex: " + ex.ToString());
			}
			return true;
		}

		public static bool IsUpgradePossible(string clientKeyPath)
		{
			Logger.Info("Checking if upgrade possible");
			bool flag = false;
			Version version = new Version("3.52.66.1905");
			Version version2 = new Version("2.52.66.8704");
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(clientKeyPath))
			{
				string text = string.Empty;
				string text2 = string.Empty;
				if (registryKey != null)
				{
					text = (string)registryKey.GetValue("ClientVersion", "");
					if (string.IsNullOrEmpty(text))
					{
						text2 = (string)registryKey.GetValue("Version", "");
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					Version version3 = new Version(text);
					Logger.Info("Previous client version is {0}", new object[] { text });
					if (version3 >= version)
					{
						flag = true;
					}
				}
				else if (!string.IsNullOrEmpty(text2))
				{
					Version version4 = new Version(text2);
					Logger.Info("Previous engine version is {0}", new object[] { text2 });
					if (version4 >= version2)
					{
						flag = true;
					}
				}
				else
				{
					Logger.Info("ClientVersion as well as Version registry not found so setting isUpgradePossible to false");
					flag = false;
				}
			}
			Logger.Info("IsUpgradePossible: {0}", new object[] { flag });
			return flag;
		}

		public static string GenerateUniqueInstallId()
		{
			Logger.Info("Generating install ID");
			try
			{
				string text = Guid.NewGuid().ToString();
				Logger.Info("GeneratedID: " + text);
				return text;
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to generate unique install id");
				Logger.Warning(ex.ToString());
			}
			return string.Empty;
		}

		public static void LogAllDirs(List<string> listOfDirs)
		{
			Logger.Info("-----------------------------------------------------");
			Logger.Info("Logging all dirs");
			foreach (string text in listOfDirs.Distinct<string>().ToList<string>())
			{
				CommonInstallUtils.LogDir(text, false);
			}
			Logger.Info("-----------------------------------------------------");
		}

		public static void LogDir(string dir, bool onlyDirs = false)
		{
			try
			{
				CommonInstallUtils.DumpDirListing(dir, onlyDirs);
			}
			catch (Exception ex)
			{
				Logger.Warning("Couldn't log dir {0}, ignoring exception: {1}", new object[] { dir, ex.Message });
			}
		}

		private static void DumpDirListing(string dir, bool onlyDirs = false)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "/c dir \"{0}\" /s", new object[] { dir });
			if (onlyDirs)
			{
				text = string.Format(CultureInfo.InvariantCulture, "/c dir \"{0}\" /ad /s", new object[] { dir });
			}
			RunCommand.RunCmd("cmd", text, true, true, false, 0);
		}

		public static bool CheckForOldConfigFiles(string dataDir)
		{
			try
			{
				string text = Path.Combine(dataDir, "UserData\\InputMapper\\UserFiles");
				if (Directory.Exists(text))
				{
					foreach (FileInfo fileInfo in new DirectoryInfo(text).GetFiles("*.cfg"))
					{
						if (fileInfo.Length != 0L)
						{
							return true;
						}
						Logger.Warning("Zero length config file found, ignoring: {0}", new object[] { fileInfo.Name });
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some error in checking for old Config Files: " + ex.Message);
			}
			return false;
		}

		public static bool CheckIfValidJsonFile(string jsonFileName)
		{
			try
			{
				Logger.Info("Checking if {0} is a valid json file", new object[] { jsonFileName });
				JArray.Parse(File.ReadAllText(jsonFileName));
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("{0} may be corrupt. Ex: {1}", new object[] { jsonFileName, ex.Message });
			}
			return false;
		}

		public static string ZipLogsAndRegistry(string logsDir, string currentInstallerLogPath)
		{
			Logger.Info("In ZipLogsAndRegistry");
			string text5;
			try
			{
				string text = CommonInstallUtils.CreateStagingDir();
				string text2 = Path.Combine(text, "Logs");
				if (Directory.Exists(logsDir))
				{
					Directory.CreateDirectory(text2);
					CommonInstallUtils.CopyFiles(logsDir, text2);
					CommonInstallUtils.ExportBluestacksRegistry(text, "RegHKLM.txt");
					string text3 = Path.Combine(text, Path.GetFileName(currentInstallerLogPath));
					File.Copy(currentInstallerLogPath, text3);
					string text4 = Path.Combine(Path.GetTempPath(), "Installer.zip");
					CommonInstallUtils.CreateZipFile(text, text4);
					Directory.Delete(text2, true);
					text5 = text4;
				}
				else
				{
					text5 = null;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in creating zip file " + ex.ToString());
				text5 = null;
			}
			return text5;
		}

		private static void CreateZipFile(string zipFolderTempPath, string zipFilePath)
		{
			try
			{
				string text = Path.Combine(Directory.GetCurrentDirectory(), "7zr.exe");
				string text2 = string.Format(CultureInfo.InvariantCulture, "a {0} -m0=LZMA:a=2 {1}\\*", new object[] { zipFilePath, zipFolderTempPath });
				if (File.Exists(zipFilePath))
				{
					File.Delete(zipFilePath);
				}
				Utils.RunCmd(text, text2, null);
			}
			catch (Exception ex)
			{
				Logger.Error("Error in creating Zip " + ex.ToString());
			}
		}

		private static void ExportBluestacksRegistry(string destination, string fileName)
		{
			try
			{
				string text = string.Format(CultureInfo.InvariantCulture, "EXPORT HKLM\\{0} \"{1}\"", new object[]
				{
					Strings.RegistryBaseKeyPath,
					Path.Combine(destination, fileName)
				});
				Utils.RunCmd("reg.exe", text, null);
			}
			catch
			{
			}
		}

		private static void CopyFiles(string src, string dest)
		{
			foreach (string text in Directory.GetFiles(src))
			{
				string fileName = Path.GetFileName(text);
				string text2 = Path.Combine(dest, fileName);
				File.Copy(text, text2);
			}
		}

		private static string CreateStagingDir()
		{
			string randomFileName = Path.GetRandomFileName();
			string text = Path.Combine(Path.GetTempPath(), randomFileName);
			if (Directory.Exists(text))
			{
				Directory.Delete(text);
			}
			Directory.CreateDirectory(text);
			return text;
		}

		public static Dictionary<string, string> GetSupportedGLModes(string glCheckDir)
		{
			Logger.Info("Checking for supported GL Modes");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> list = new List<string>();
			try
			{
				foreach (string text in new List<string> { "1 1", "4 1", "1 2", "4 2" })
				{
					if (CommonInstallUtils.RunGLCheck(glCheckDir, text) == 0)
					{
						list.Add(text);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("An error occured while checking for supported GLModes");
				Logger.Error(ex.ToString());
			}
			dictionary["supported_glmodes"] = string.Join(",", list.ToArray());
			Logger.Info("Supported GL Modes: " + string.Join(",", list.ToArray()));
			return dictionary;
		}

		private static int RunGLCheck(string glCheckDir, string args)
		{
			try
			{
				return RunCommand.RunCmd(Path.Combine(glCheckDir, "HD-GLCheck.exe"), args, true, true, false, 10000).ExitCode;
			}
			catch (Exception ex)
			{
				Logger.Warning("An error occured while running glcheck, ignorning.");
				Logger.Warning(ex.Message);
			}
			return -1;
		}

		public static void CheckIfVulkanSupported()
		{
		}

		public static void WriteClientVersionInFile(string version)
		{
			int i = 5;
			while (i > 0)
			{
				try
				{
					string text = Path.Combine(RegistryManager.Instance.UserDefinedDir, "bst_params.txt");
					CommonInstallUtils.WriteToFile(text, string.Format(CultureInfo.InvariantCulture, "version={0}", new object[] { version }), "version");
					Logger.Info("version written to file successfully");
					string text2 = new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString();
					FileInfo fileInfo = new FileInfo(text);
					FileSecurity accessControl = fileInfo.GetAccessControl();
					accessControl.AddAccessRule(new FileSystemAccessRule(text2, FileSystemRights.FullControl, AccessControlType.Allow));
					fileInfo.SetAccessControl(accessControl);
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

		public static int SetupVmConfig(string installDir, string dataDir)
		{
			Logger.Info("In SetupVmConfig");
			if (!CommonInstallUtils.InstallVmConfig(installDir, dataDir))
			{
				Logger.Info("Throwing error cannot create android.bstk from in file");
				return -42;
			}
			return 0;
		}

		public static int SetupBstkGlobalConfig(string dataDir)
		{
			Logger.Info("In SetupBstkGlobalConfig");
			if (!CommonInstallUtils.InstallVirtualBoxConfig(dataDir, false))
			{
				Logger.Info("Throwing error cannot create bstkglobal from in file");
				return -41;
			}
			return 0;
		}

		public static bool InstallVirtualBoxConfig(string dataDir, bool isUpgrade = false)
		{
			Logger.Info("InstallVirtualBoxConfig()");
			string text = Path.Combine(dataDir, "Manager");
			string text2 = Path.Combine(Path.Combine(dataDir, "Manager"), "BstkGlobal.xml");
			string text3 = Path.Combine(text, "BstkGlobal.xml.in");
			string text4 = null;
			try
			{
				using (StreamReader streamReader = new StreamReader(text3))
				{
					text4 = streamReader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				string text5 = "Cannot read '";
				string text6 = text3;
				string text7 = "': ";
				Exception ex2 = ex;
				Logger.Error(text5 + text6 + text7 + ((ex2 != null) ? ex2.ToString() : null));
				return false;
			}
			string text8 = dataDir;
			if (isUpgrade)
			{
				text8 = RegistryStrings.DataDir.TrimEnd(new char[] { '\\' });
			}
			string text9 = text4;
			text9 = text9.Replace("@@USER_DEFINED_DIR@@", text8 + "\\");
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(text2))
				{
					streamWriter.Write(text9);
				}
			}
			catch (Exception ex3)
			{
				string text10 = "Cannot write '";
				string text11 = text2;
				string text12 = "': ";
				Exception ex4 = ex3;
				Logger.Error(text10 + text11 + text12 + ((ex4 != null) ? ex4.ToString() : null));
				return false;
			}
			return true;
		}

		public static bool InstallVmConfig(string installDir, string dataDir)
		{
			Logger.Info("InstallVmConfig()");
			string text = Path.Combine(dataDir, "Android");
			string text2 = Path.Combine(text, "Android.bstk");
			string text3 = Path.Combine(text, "Android.bstk.in");
			string text4 = null;
			if (File.Exists(text2))
			{
				Logger.Info("android.bstk already present returning");
				return true;
			}
			try
			{
				using (StreamReader streamReader = new StreamReader(text3))
				{
					text4 = streamReader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				string text5 = "Cannot read '";
				string text6 = text3;
				string text7 = "': ";
				Exception ex2 = ex;
				Logger.Info(text5 + text6 + text7 + ((ex2 != null) ? ex2.ToString() : null));
				return false;
			}
			string text8 = text4;
			text8 = text8.Replace("@@HD_PLUS_DEVICES_DLL_PATH@@", SecurityElement.Escape(Path.Combine(installDir, "HD-Plus-Devices.dll")));
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			if (!string.IsNullOrEmpty(folderPath))
			{
				string text9 = string.Format(CultureInfo.InvariantCulture, "<SharedFolder name=\"Documents\" hostPath=\"{0}\" writable=\"true\" autoMount=\"false\"/>", new object[] { SecurityElement.Escape(folderPath) });
				text8 = text8.Replace("@@USER_DOCUMENTS_FOLDER@@", text9);
			}
			else
			{
				text8 = text8.Replace("@@USER_DOCUMENTS_FOLDER@@", "");
			}
			string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			if (!string.IsNullOrEmpty(folderPath2))
			{
				string text10 = string.Format(CultureInfo.InvariantCulture, "<SharedFolder name=\"Pictures\" hostPath=\"{0}\" writable=\"true\" autoMount=\"false\"/>", new object[] { SecurityElement.Escape(folderPath2) });
				text8 = text8.Replace("@@USER_PICTURES_FOLDER@@", text10);
			}
			else
			{
				text8 = text8.Replace("@@USER_PICTURES_FOLDER@@", "");
			}
			text8 = text8.Replace("@@INPUT_MAPPER_FOLDER@@", SecurityElement.Escape(Path.Combine(dataDir, "UserData\\InputMapper")));
			text8 = text8.Replace("@@BST_SHARED_FOLDER@@", SecurityElement.Escape(Path.Combine(dataDir, "UserData\\SharedFolder")));
			text8 = text8.Replace("@@BST_VM_MEMORY_SIZE@@", SecurityElement.Escape(Utils.GetAndroidVMMemory(true).ToString(CultureInfo.InvariantCulture)));
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(text8);
				xmlDocument.Save(text2);
			}
			catch (Exception ex3)
			{
				string text11 = "Cannot write '";
				string text12 = text2;
				string text13 = "': ";
				Exception ex4 = ex3;
				Logger.Info(text11 + text12 + text13 + ((ex4 != null) ? ex4.ToString() : null));
				return false;
			}
			return true;
		}

		public static bool CheckSupportedGlRenderMode(out int glRenderMode, out string glVendor, out string glRenderer, out string glVersion, out GLMode glMode, List<GlProperty> glCheckOrder, List<GlProperty> fallbackGlCheckOrder)
		{
			glRenderMode = 4;
			glVersion = "";
			glRenderer = "";
			glVendor = "";
			glMode = GLMode.PGA;
			try
			{
				if (glCheckOrder != null && glCheckOrder.Count > 0)
				{
					foreach (GlProperty glProperty in glCheckOrder)
					{
						Logger.Info("gl check with args.." + glProperty.Gl_renderer.ToString() + " and " + glProperty.Gl_mode.ToString());
						if (BlueStacksGL.GLCheckInstallation(glProperty.Gl_renderer, glProperty.Gl_mode, out glVendor, out glRenderer, out glVersion) == 0)
						{
							glRenderMode = (int)glProperty.Gl_renderer;
							glMode = glProperty.Gl_mode;
							return true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				string text = "exception in getting gl check from list..";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
			if (Oem.Instance.CheckForAGAInInstaller && CommonInstallUtils.CheckSupportedGlRenderMode(GLMode.AGA, out glRenderMode, out glVendor, out glRenderer, out glVersion))
			{
				Logger.Info("AGA supported!");
				glMode = GLMode.AGA;
				return true;
			}
			if (fallbackGlCheckOrder != null && fallbackGlCheckOrder.Count > 0)
			{
				foreach (GlProperty glProperty2 in fallbackGlCheckOrder)
				{
					Logger.Info("gl check with args.." + glProperty2.Gl_renderer.ToString() + " and " + glProperty2.Gl_mode.ToString());
					if (BlueStacksGL.GLCheckInstallation(glProperty2.Gl_renderer, glProperty2.Gl_mode, out glVendor, out glRenderer, out glVersion) == 0)
					{
						glRenderMode = (int)glProperty2.Gl_renderer;
						glMode = glProperty2.Gl_mode;
						return true;
					}
				}
			}
			return false;
		}

		public static bool CheckSupportedGlRenderMode(GLMode mode, out int glRenderMode, out string glVendor, out string glRenderer, out string glVersion)
		{
			glRenderMode = 4;
			glVersion = "";
			glRenderer = "";
			glVendor = "";
			try
			{
				int num;
				if (BlueStacksGL.GLCheckInstallation(GLRenderer.OpenGL, mode, out glVendor, out glRenderer, out glVersion) == 0)
				{
					num = 0;
					glRenderMode = 1;
				}
				else if (SystemUtils.IsOs64Bit())
				{
					if (CommonInstallUtils.GpuWithDx9SupportOnly())
					{
						Logger.Info("Machine has gpu which runs on DX9 only");
						glRenderMode = 2;
						num = BlueStacksGL.GLCheckInstallation(GLRenderer.DX9, mode, out glVendor, out glRenderer, out glVersion);
					}
					else
					{
						glRenderMode = 4;
						num = BlueStacksGL.GLCheckInstallation(GLRenderer.DX11FallbackDX9, mode, out glVendor, out glRenderer, out glVersion);
						if (num != 0)
						{
							glRenderMode = 2;
							num = BlueStacksGL.GLCheckInstallation(GLRenderer.DX9, mode, out glVendor, out glRenderer, out glVersion);
						}
					}
				}
				else
				{
					glRenderMode = 2;
					num = BlueStacksGL.GLCheckInstallation(GLRenderer.DX9, mode, out glVendor, out glRenderer, out glVersion);
				}
				if (num != 0)
				{
					Logger.Info("DirectX not supported.");
					glRenderMode = -1;
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Some error occured while checking for GL. Ex: {0}", new object[] { ex });
			}
			return false;
		}

		public static bool GpuWithDx9SupportOnly()
		{
			string[] array = new string[] { "Mobile Intel(R) 4 Series Express Chipset Family", "Mobile Intel(R) 45 Express Chipset Family", "Mobile Intel(R) 965 Express Chipset Family", "Intel(R) G41 Express Chipset", "Intel(R) G45/G43 Express Chipset", "Intel(R) Q45/Q43 Express Chipset" };
			string text = "";
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration"))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						foreach (PropertyData propertyData in ((ManagementObject)managementBaseObject).Properties)
						{
							if (propertyData.Name == "Description")
							{
								text = propertyData.Value.ToString();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while runninq query. Error: ", new object[] { ex.ToString() });
			}
			Logger.Info("Graphics card: {0}", new object[] { text });
			string text2 = text.ToLower(CultureInfo.InvariantCulture);
			if (text2.Contains("intel") && text2.Contains("express chipset"))
			{
				Logger.Info("graphicsCard : {0} part of the list of graphics card to be forced to dx9", new object[] { text });
				return true;
			}
			return false;
		}

		public static bool IsProcessorIntel()
		{
			bool flag;
			try
			{
				Dictionary<string, string> dictionary = Profile.Info();
				if (dictionary != null && dictionary["Processor"].Contains("Intel"))
				{
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			catch
			{
				Logger.Error("Unable to detect for intel processor");
				flag = false;
			}
			return flag;
		}

		public static string GetPrebundledVdiUid(string file)
		{
			string text = string.Empty;
			if (!File.Exists(file))
			{
				return "";
			}
			string text2 = File.ReadAllText(file);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.XmlResolver = null;
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespaceManager.AddNamespace("bstk", "http://www.virtualbox.org/");
			string text3;
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(text2), new XmlReaderSettings
			{
				XmlResolver = null
			}))
			{
				xmlDocument.Load(xmlReader);
				foreach (object obj in xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:MediaRegistry//bstk:HardDisks//bstk:HardDisk", xmlNamespaceManager))
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Attributes["location"].Value.Equals("Prebundled.vdi", StringComparison.InvariantCultureIgnoreCase))
					{
						text = xmlNode.Attributes["uuid"].Value;
						break;
					}
				}
				text3 = text;
			}
			return text3;
		}

		private const string OpenGL_Native_DLL = "HD-OpenGl-Native.dll";

		private const int CSIDL_COMMON_DESKTOPDIRECTORY = 25;

		internal static List<string> sDisallowedDeletionStrings = new List<string>
		{
			"*",
			"\\",
			Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
			Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
			Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
		};

		private static bool is64BitProcess = IntPtr.Size == 8;

		internal static bool is64BitOperatingSystem = CommonInstallUtils.is64BitProcess || CommonInstallUtils.InternalCheckIsWow64();
	}
}


