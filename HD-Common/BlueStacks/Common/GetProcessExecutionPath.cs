using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.Common
{
	public static class GetProcessExecutionPath
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool QueryFullProcessImageName(IntPtr hwnd, int flags, [Out] StringBuilder buffer, out int size);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr OpenProcess(int flags, bool handle, UIntPtr procId);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CloseHandle(IntPtr handle);

		public static List<string> GetApplicationPath(Process[] procList)
		{
			List<string> list = new List<string>();
			if (procList != null)
			{
				foreach (Process process in procList)
				{
					try
					{
						string applicationPathFromProcess = GetProcessExecutionPath.GetApplicationPathFromProcess(process);
						if (!string.IsNullOrEmpty(applicationPathFromProcess))
						{
							list.Add(applicationPathFromProcess);
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return list;
		}

		public static string GetApplicationPathFromProcess(Process proc)
		{
			try
			{
				if (!SystemUtils.IsOSWinXP())
				{
					return GetProcessExecutionPath.GetExecutablePathAboveVista(new UIntPtr((uint)((proc != null) ? new int?(proc.Id) : null).Value));
				}
				if (SystemUtils.IsAdministrator())
				{
					return (proc != null) ? proc.MainModule.FileName.ToString(CultureInfo.InvariantCulture) : null;
				}
			}
			catch
			{
			}
			return string.Empty;
		}

		public static string GetExecutablePathAboveVista(UIntPtr dwProcessId)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			IntPtr intPtr = GetProcessExecutionPath.OpenProcess(4096, false, dwProcessId);
			if (intPtr != IntPtr.Zero)
			{
				try
				{
					int capacity = stringBuilder.Capacity;
					if (GetProcessExecutionPath.QueryFullProcessImageName(intPtr, 0, stringBuilder, out capacity))
					{
						return stringBuilder.ToString();
					}
				}
				finally
				{
					GetProcessExecutionPath.CloseHandle(intPtr);
				}
			}
			return string.Empty;
		}
	}
}


