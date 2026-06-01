using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	public static class ProcessDetails
	{
		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CreateToolhelp32Snapshot([In] uint dwFlags, [In] uint th32ProcessID);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool Process32First([In] IntPtr hSnapshot, ref ProcessDetails.PROCESSENTRY32 lppe);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool Process32Next([In] IntPtr hSnapshot, ref ProcessDetails.PROCESSENTRY32 lppe);

		[DllImport("kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle([In] IntPtr hObject);

		public static int? GetParentProcessId(int pid)
		{
			Process parentProcess = ProcessDetails.GetParentProcess(pid);
			if (parentProcess == null)
			{
				return null;
			}
			return new int?(parentProcess.Id);
		}

		public static Process GetParentProcess(int pid)
		{
			Process process = null;
			int id = Process.GetCurrentProcess().Id;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				ProcessDetails.PROCESSENTRY32 processentry = new ProcessDetails.PROCESSENTRY32
				{
					dwSize = (uint)Marshal.SizeOf(typeof(ProcessDetails.PROCESSENTRY32))
				};
				intPtr = ProcessDetails.CreateToolhelp32Snapshot(2U, 0U);
				if (!ProcessDetails.Process32First(intPtr, ref processentry))
				{
					throw new ApplicationException(string.Format(CultureInfo.InvariantCulture, "Failed with win32 error code {0}", new object[] { Marshal.GetLastWin32Error() }));
				}
				while ((long)pid != (long)((ulong)processentry.th32ProcessID))
				{
					if (!ProcessDetails.Process32Next(intPtr, ref processentry))
					{
						return process;
					}
				}
				process = Process.GetProcessById((int)processentry.th32ParentProcessID);
			}
			catch (Exception ex)
			{
				Logger.Error("Can't get the process.", new object[] { ex.ToString() });
			}
			finally
			{
				ProcessDetails.CloseHandle(intPtr);
			}
			return process;
		}

		public static string CurrentProcessParentFileName
		{
			get
			{
				return Path.GetFileName(ProcessDetails.CurrentProcessParentFullPath);
			}
		}

		public static string CurrentProcessParentFullPath
		{
			get
			{
				return ProcessDetails.CurrentProcessParent.MainModule.FileName;
			}
		}

		public static Process CurrentProcessParent
		{
			get
			{
				return ProcessDetails.GetParentProcess(Process.GetCurrentProcess().Id);
			}
		}

		public static int? CurrentProcessParentId
		{
			get
			{
				return ProcessDetails.GetParentProcessId(Process.GetCurrentProcess().Id);
			}
		}

		public static int CurrentProcessId
		{
			get
			{
				return Process.GetCurrentProcess().Id;
			}
		}

		public static int? GetNthParentPid(int pid, int order)
		{
			int? parentProcessId = new int?(pid);
			while (order > 0 && parentProcessId != null)
			{
				parentProcessId = ProcessDetails.GetParentProcessId(parentProcessId.Value);
				order--;
			}
			return parentProcessId;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct PROCESSENTRY32
		{
			private const int MAX_PATH = 260;

			internal uint dwSize;

			internal uint cntUsage;

			internal uint th32ProcessID;

			internal IntPtr th32DefaultHeapID;

			internal uint th32ModuleID;

			internal uint cntThreads;

			internal uint th32ParentProcessID;

			internal int pcPriClassBase;

			internal uint dwFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string szExeFile;
		}

		[Flags]
		private enum SnapshotFlags : uint
		{
			HeapList = 1U,
			Process = 2U,
			Thread = 4U,
			Module = 8U,
			Module32 = 16U,
			Inherit = 2147483648U,
			All = 31U,
			NoHeaps = 1073741824U
		}
	}
}


