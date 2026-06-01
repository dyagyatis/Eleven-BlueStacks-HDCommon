using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace BlueStacks.Common
{
	public static class SSDCheck
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern SafeFileHandle CreateFileW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, ref SSDCheck.STORAGE_PROPERTY_QUERY lpInBuffer, uint nInBufferSize, ref SSDCheck.DEVICE_TRIM_DESCRIPTOR lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, ref SSDCheck.VOLUME_DISK_EXTENTS lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, StringBuilder lpBuffer, uint nSize, IntPtr Arguments);

		private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
		{
			return (DeviceType << 16) | (Access << 14) | (Function << 2) | Method;
		}

		public static bool IsMediaTypeSSD(string path)
		{
			try
			{
				string pathRoot = Path.GetPathRoot(path);
				Logger.Info("Checking if media type ssd for drive: " + pathRoot);
				string text = pathRoot.TrimEnd(new char[] { '\\' }).TrimEnd(new char[] { ':' });
				if (text.Length > 1)
				{
					Logger.Info("Invalid drive letter " + text + ". returning!!");
					return false;
				}
				return SSDCheck.HasTrimEnabled("\\\\.\\PhysicalDrive" + SSDCheck.GetDiskExtents(text.ToCharArray()[0]).ToString());
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to find if media is ssd. Ex : " + ex.ToString());
			}
			return false;
		}

		private static bool HasTrimEnabled(string drive)
		{
			Logger.Info("Checking trim enabled for drive: " + drive);
			SafeFileHandle safeFileHandle = SSDCheck.CreateFileW(drive, 0U, 3U, IntPtr.Zero, 3U, 128U, IntPtr.Zero);
			if (safeFileHandle == null || safeFileHandle.IsInvalid)
			{
				string errorMessage = SSDCheck.GetErrorMessage(Marshal.GetLastWin32Error());
				Logger.Error("CreateFile failed with message: " + errorMessage);
				throw new Exception("check the error message above");
			}
			uint num = SSDCheck.CTL_CODE(45U, 1280U, 0U, 0U);
			SSDCheck.STORAGE_PROPERTY_QUERY storage_PROPERTY_QUERY = new SSDCheck.STORAGE_PROPERTY_QUERY
			{
				PropertyId = 8U,
				QueryType = 0U
			};
			SSDCheck.DEVICE_TRIM_DESCRIPTOR device_TRIM_DESCRIPTOR = default(SSDCheck.DEVICE_TRIM_DESCRIPTOR);
			uint num2;
			bool flag = SSDCheck.DeviceIoControl(safeFileHandle, num, ref storage_PROPERTY_QUERY, (uint)Marshal.SizeOf(storage_PROPERTY_QUERY), ref device_TRIM_DESCRIPTOR, (uint)Marshal.SizeOf(device_TRIM_DESCRIPTOR), out num2, IntPtr.Zero);
			if (safeFileHandle != null)
			{
				safeFileHandle.Close();
			}
			if (!flag)
			{
				string errorMessage2 = SSDCheck.GetErrorMessage(Marshal.GetLastWin32Error());
				Logger.Error("DeviceIoControl failed to query trim enabled. " + errorMessage2);
				throw new Exception("check the error message above");
			}
			bool trimEnabled = device_TRIM_DESCRIPTOR.TrimEnabled;
			Logger.Info(string.Format("Is Trim Enabled: {0}", trimEnabled));
			return trimEnabled;
		}

		private static uint GetDiskExtents(char cDrive)
		{
			if (new DriveInfo(cDrive.ToString(CultureInfo.InvariantCulture)).DriveType != DriveType.Fixed)
			{
				Logger.Info(string.Format("The drive {0} is not fixed drive.", cDrive));
			}
			string text = "\\\\.\\" + cDrive.ToString(CultureInfo.InvariantCulture) + ":";
			SafeFileHandle safeFileHandle = SSDCheck.CreateFileW(text, 0U, 3U, IntPtr.Zero, 3U, 128U, IntPtr.Zero);
			if (safeFileHandle == null || safeFileHandle.IsInvalid)
			{
				string errorMessage = SSDCheck.GetErrorMessage(Marshal.GetLastWin32Error());
				Logger.Error("CreateFile failed for " + text + ".  " + errorMessage);
				throw new Exception("check the error message above");
			}
			uint num = SSDCheck.CTL_CODE(86U, 0U, 0U, 0U);
			SSDCheck.VOLUME_DISK_EXTENTS volume_DISK_EXTENTS = default(SSDCheck.VOLUME_DISK_EXTENTS);
			uint num2;
			bool flag = SSDCheck.DeviceIoControl(safeFileHandle, num, IntPtr.Zero, 0U, ref volume_DISK_EXTENTS, (uint)Marshal.SizeOf(volume_DISK_EXTENTS), out num2, IntPtr.Zero);
			if (safeFileHandle != null)
			{
				safeFileHandle.Close();
			}
			if (!flag || volume_DISK_EXTENTS.Extents.Length != 1)
			{
				string errorMessage2 = SSDCheck.GetErrorMessage(Marshal.GetLastWin32Error());
				Logger.Error("DeviceIoControl failed to query disk extension. " + errorMessage2);
				throw new Exception("check the error message above");
			}
			uint diskNumber = volume_DISK_EXTENTS.Extents[0].DiskNumber;
			Logger.Info(string.Format("The physical drive number is: {0}", diskNumber));
			return diskNumber;
		}

		private static string GetErrorMessage(int code)
		{
			StringBuilder stringBuilder = new StringBuilder(255);
			SSDCheck.FormatMessage(4096U, IntPtr.Zero, (uint)code, 0U, stringBuilder, (uint)stringBuilder.Capacity, IntPtr.Zero);
			return stringBuilder.ToString();
		}

		private const uint FILE_SHARE_READ = 1U;

		private const uint FILE_SHARE_WRITE = 2U;

		private const uint OPEN_EXISTING = 3U;

		private const uint FILE_ATTRIBUTE_NORMAL = 128U;

		private const uint FILE_DEVICE_MASS_STORAGE = 45U;

		private const uint IOCTL_STORAGE_BASE = 45U;

		private const uint METHOD_BUFFERED = 0U;

		private const uint FILE_ANY_ACCESS = 0U;

		private const uint IOCTL_VOLUME_BASE = 86U;

		private const uint StorageDeviceTrimEnabledProperty = 8U;

		private const uint PropertyStandardQuery = 0U;

		private const uint FORMAT_MESSAGE_FROM_SYSTEM = 4096U;

		private struct STORAGE_PROPERTY_QUERY
		{
			public uint PropertyId;

			public uint QueryType;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public byte[] AdditionalParameters;
		}

		private struct DEVICE_TRIM_DESCRIPTOR
		{
			public uint Version;

			public uint Size;

			[MarshalAs(UnmanagedType.U1)]
			public bool TrimEnabled;
		}

		private struct DISK_EXTENT
		{
			public uint DiskNumber;

			public long StartingOffset;

			public long ExtentLength;
		}

		private struct VOLUME_DISK_EXTENTS
		{
			public uint NumberOfDiskExtents;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public SSDCheck.DISK_EXTENT[] Extents;
		}
	}
}


