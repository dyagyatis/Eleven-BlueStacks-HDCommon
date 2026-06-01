using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	public static class MemoryMappedFile
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, UIntPtr dwNumberOfBytesToMap);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);

		public static int GetNCSoftAgentPort(string SharedMemoryName, uint NumBytes)
		{
			IntPtr intPtr = MemoryMappedFile.OpenFileMapping(983071U, false, SharedMemoryName);
			if (IntPtr.Zero == intPtr)
			{
				Logger.Error("Shared Memory Handle not found. Last Error : " + Marshal.GetLastWin32Error().ToString());
				return -1;
			}
			IntPtr intPtr2 = MemoryMappedFile.MapViewOfFile(intPtr, 983071U, 0U, 0U, new UIntPtr(NumBytes));
			if (intPtr2 == IntPtr.Zero)
			{
				Logger.Error("Cannot map view of file. Last Error : " + Marshal.GetLastWin32Error().ToString());
				return -1;
			}
			int num = -1;
			try
			{
				num = Marshal.ReadInt32(intPtr2);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to read memory as int32");
				Logger.Error(ex.ToString());
			}
			if (IntPtr.Zero != intPtr)
			{
				MemoryMappedFile.CloseHandle(intPtr);
				intPtr = IntPtr.Zero;
			}
			intPtr2 = IntPtr.Zero;
			return num;
		}

		private const uint STANDARD_RIGHTS_REQUIRED = 983040U;

		private const uint SECTION_QUERY = 1U;

		private const uint SECTION_MAP_WRITE = 2U;

		private const uint SECTION_MAP_READ = 4U;

		private const uint SECTION_MAP_EXECUTE = 8U;

		private const uint SECTION_EXTEND_SIZE = 16U;

		private const uint SECTION_ALL_ACCESS = 983071U;

		private const uint FILE_MAP_ALL_ACCESS = 983071U;
	}
}


