using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Common
{
	public static class ProcessUtils
	{
		public static Dictionary<string, string> LockToProcessMap
		{
			get
			{
				return new Dictionary<string, string>
				{
					{ "Global\\BlueStacks_Installer_Lockbgp64", "Installer" },
					{ "Global\\BlueStacks_MicroInstaller_Lockbgp64", "MicroInstaller" },
					{ "Global\\BlueStacks_Uninstaller_Lockbgp64", "Uninstaller" }
				};
			}
		}

		public static bool FindProcessByName(string name)
		{
			return Process.GetProcessesByName(name).Length != 0;
		}

		public static void KillProcessByName(string name)
		{
			foreach (Process process in Process.GetProcessesByName(name))
			{
				try
				{
					Logger.Debug("Attempting to kill: {0}", new object[] { process.ProcessName });
					process.Kill();
					if (!process.WaitForExit(5000))
					{
						Logger.Info("Timeout waiting for process {0} to die", new object[] { process.ProcessName });
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in killing process " + ex.Message);
				}
			}
		}

		public static void KillProcessesByName(string[] nameList)
		{
			if (nameList != null)
			{
				for (int i = 0; i < nameList.Length; i++)
				{
					ProcessUtils.KillProcessByName(nameList[i]);
				}
			}
		}

		public static Process GetProcessObject(string exePath, string args, bool isAdmin = false)
		{
			Process process = new Process();
			process.StartInfo.Arguments = args;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = exePath;
			if (isAdmin)
			{
				process.StartInfo.Verb = "runas";
				process.StartInfo.UseShellExecute = true;
			}
			return process;
		}

		public static bool IsProcessAlive(int pid)
		{
			bool flag = false;
			try
			{
				Process.GetProcessById(pid);
				flag = true;
			}
			catch (ArgumentException)
			{
			}
			return flag;
		}

		public static bool IsLockInUse(string lockName)
		{
			return ProcessUtils.IsLockInUse(lockName, true);
		}

		[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
		public static bool IsLockInUse(string lockName, bool printLog)
		{
			Mutex mutex;
			if (ProcessUtils.CheckAlreadyRunningAndTakeLock(lockName, out mutex))
			{
				if (printLog)
				{
					Logger.Info(lockName + " running.");
				}
				return true;
			}
			if (mutex != null)
			{
				mutex.Close();
				mutex = null;
			}
			return false;
		}

		public static bool IsAnyInstallerProcesRunning(out string runningProcName)
		{
			runningProcName = null;
			foreach (string text in ProcessUtils.LockToProcessMap.Keys)
			{
				if (ProcessUtils.IsAlreadyRunning(text))
				{
					runningProcName = ProcessUtils.LockToProcessMap[text];
					return true;
				}
			}
			return false;
		}

		[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
		public static bool IsAlreadyRunning(string name)
		{
			Mutex mutex;
			if (!ProcessUtils.CheckAlreadyRunningAndTakeLock(name, out mutex))
			{
				if (mutex != null)
				{
					mutex.Close();
				}
				return false;
			}
			return true;
		}

		public static bool CheckAlreadyRunningAndTakeLock(string name, out Mutex lck)
		{
			bool flag;
			try
			{
				lck = new Mutex(true, name, out flag);
			}
			catch (AbandonedMutexException ex)
			{
				lck = null;
				Logger.Warning("Abandoned mutex : " + name + ".  " + ex.ToString());
				return false;
			}
			catch (UnauthorizedAccessException ex2)
			{
				lck = null;
				Logger.Warning("UnauthorisedAccess on mutex : " + name + ".  " + ex2.ToString());
				return true;
			}
			if (!flag)
			{
				lck.Close();
				lck = null;
			}
			return !flag;
		}

		public static void KillProcessByNameIgnoreDirectory(string name, string IgnoreDirectory)
		{
			foreach (Process process in Process.GetProcessesByName(name))
			{
				string text = "";
				try
				{
					text = process.MainModule.FileName;
				}
				catch (Win32Exception ex)
				{
					Logger.Error("Got the excpetion {0}", new object[] { ex.Message });
					Logger.Info("Giving the exit code to start as admin");
					Environment.Exit(2);
				}
				catch (Exception ex2)
				{
					Logger.Error("Got exception: err {0}", new object[] { ex2.ToString() });
				}
				string text2 = Directory.GetParent(text).ToString();
				Logger.Debug("The Process Dir is {0}", new object[] { text2 });
				if (text2.Equals(IgnoreDirectory, StringComparison.CurrentCultureIgnoreCase))
				{
					Logger.Debug("Process:{0} not killed since the process sir:{1} and ignore dir:{2} are the same", new object[] { process.ProcessName, text2, IgnoreDirectory });
				}
				else
				{
					Logger.Info("Killing PID " + process.Id.ToString() + " -> " + process.ProcessName);
					try
					{
						process.Kill();
					}
					catch (Exception ex3)
					{
						Logger.Error(ex3.ToString());
						goto IL_0117;
					}
					if (!process.WaitForExit(5000))
					{
						Logger.Info("Timeout waiting for process to die");
					}
				}
				IL_0117:;
			}
		}

		public static void LogParentProcessDetails()
		{
			try
			{
				Process currentProcessParent = ProcessDetails.CurrentProcessParent;
				if (currentProcessParent == null)
				{
					Logger.Info("Unable to retrieve information about invoking process");
				}
				else
				{
					Logger.Info("Invoking Process Details: (Name: {0}, Pid: {1})", new object[] { currentProcessParent.ProcessName, currentProcessParent.Id });
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Unable to get parent process details, Err: {0}", new object[] { ex.ToString() });
			}
		}

		public static void LogProcessContextDetails()
		{
			Logger.Info("PID {0}, CLR version {0}", new object[]
			{
				Process.GetCurrentProcess().Id,
				Environment.Version
			});
			Logger.Info("IsAdministrator: {0}", new object[] { SystemUtils.IsAdministrator() });
		}

		public static Process StartExe(string exePath, string args, bool isAdmin = false)
		{
			Process process = new Process();
			process.StartInfo.Arguments = args;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = exePath;
			if (isAdmin)
			{
				process.StartInfo.Verb = "runas";
				process.StartInfo.UseShellExecute = true;
			}
			Logger.Info("Utils: Starting Process : " + exePath);
			process.Start();
			return process;
		}

		public static void ExecuteProcessUnElevated(string process, string args, string currentDirectory = "")
		{
			ProcessUtils.IShellWindows shellWindows = (ProcessUtils.IShellWindows)new ProcessUtils.CShellWindows();
			object obj = 0;
			object obj2 = new object();
			int num;
			ProcessUtils.IServiceProvider serviceProvider = (ProcessUtils.IServiceProvider)shellWindows.FindWindowSW(ref obj, ref obj2, 8, out num, 1);
			Guid sid_STopLevelBrowser = ProcessUtils.SID_STopLevelBrowser;
			Guid guid = typeof(ProcessUtils.IShellBrowser).GUID;
			ProcessUtils.IShellBrowser shellBrowser = (ProcessUtils.IShellBrowser)serviceProvider.QueryService(ref sid_STopLevelBrowser, ref guid);
			Guid guid2 = typeof(ProcessUtils.IDispatch).GUID;
			((ProcessUtils.IShellDispatch2)((ProcessUtils.IShellFolderViewDual)shellBrowser.QueryActiveShellView().GetItemObject(0U, ref guid2)).Application).ShellExecute(process, args, currentDirectory, string.Empty, 1);
		}

		private const int CSIDL_Desktop = 0;

		private const int SWC_DESKTOP = 8;

		private const int SWFO_NEEDDISPATCH = 1;

		private const int SW_SHOWNORMAL = 1;

		private const int SVGIO_BACKGROUND = 0;

		private static readonly Guid SID_STopLevelBrowser = new Guid("4C96BE40-915C-11CF-99D3-00AA004AE837");

		[Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39")]
		[ClassInterface(ClassInterfaceType.None)]
		[ComImport]
		private class CShellWindows
		{
		}

		[Guid("85CB6900-4D95-11CF-960C-0080C7F4EE85")]
		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		[ComImport]
		private interface IShellWindows
		{
			[return: MarshalAs(UnmanagedType.IDispatch)]
			object FindWindowSW([MarshalAs(UnmanagedType.Struct)] ref object pvarloc, [MarshalAs(UnmanagedType.Struct)] ref object pvarlocRoot, int swClass, out int pHWND, int swfwOptions);
		}

		[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		private interface IServiceProvider
		{
			[return: MarshalAs(UnmanagedType.Interface)]
			object QueryService(ref Guid guidService, ref Guid riid);
		}

		[Guid("000214E2-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		private interface IShellBrowser
		{
			void VTableGap01();

			void VTableGap02();

			void VTableGap03();

			void VTableGap04();

			void VTableGap05();

			void VTableGap06();

			void VTableGap07();

			void VTableGap08();

			void VTableGap09();

			void VTableGap10();

			void VTableGap11();

			void VTableGap12();

			ProcessUtils.IShellView QueryActiveShellView();
		}

		[Guid("000214E3-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		private interface IShellView
		{
			void VTableGap01();

			void VTableGap02();

			void VTableGap03();

			void VTableGap04();

			void VTableGap05();

			void VTableGap06();

			void VTableGap07();

			void VTableGap08();

			void VTableGap09();

			void VTableGap10();

			void VTableGap11();

			void VTableGap12();

			[return: MarshalAs(UnmanagedType.Interface)]
			object GetItemObject(uint aspectOfView, ref Guid riid);
		}

		[Guid("00020400-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		[ComImport]
		private interface IDispatch
		{
		}

		[Guid("E7A1AF80-4D96-11CF-960C-0080C7F4EE85")]
		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		[ComImport]
		private interface IShellFolderViewDual
		{
			object Application
			{
				[return: MarshalAs(UnmanagedType.IDispatch)]
				get;
			}
		}

		[Guid("A4C6892C-3BA9-11D2-9DEA-00C04FB16162")]
		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		[ComImport]
		public interface IShellDispatch2
		{
			void ShellExecute([MarshalAs(UnmanagedType.BStr)] string File, [MarshalAs(UnmanagedType.Struct)] object vArgs, [MarshalAs(UnmanagedType.Struct)] object vDir, [MarshalAs(UnmanagedType.Struct)] object vOperation, [MarshalAs(UnmanagedType.Struct)] object vShow);
		}
	}
}


