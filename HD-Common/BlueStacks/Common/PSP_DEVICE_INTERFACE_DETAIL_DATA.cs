using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	public struct PSP_DEVICE_INTERFACE_DETAIL_DATA
	{
		public int cbSize { readonly get; set; }

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string DevicePath;
	}
}


