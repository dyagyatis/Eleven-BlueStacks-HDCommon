using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct WIN32_FIND_DATAW
	{
		public uint dwFileAttributes { readonly get; set; }

		public long ftCreationTime { readonly get; set; }

		public long ftLastAccessTime { readonly get; set; }

		public long ftLastWriteTime { readonly get; set; }

		public uint nFileSizeHigh { readonly get; set; }

		public uint nFileSizeLow { readonly get; set; }

		public uint dwReserved0 { readonly get; set; }

		public uint dwReserved1 { readonly get; set; }

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string cFileName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string cAlternateFileName;
	}
}


