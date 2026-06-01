using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	public struct DATA_BUFFER
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string Buffer;
	}
}


