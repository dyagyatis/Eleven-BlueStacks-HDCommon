using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	public static class InteropUtils
	{
		[DllImport("kernel32.dll")]
		public static extern long GetTickCount64();
	}
}


