using System;

namespace BlueStacks.Common
{
	public struct MONITORINFO
	{
		public int cbSize { readonly get; set; }

		public RECT rcMonitor { readonly get; set; }

		public RECT rcWork { readonly get; set; }

		public uint dwFlags { readonly get; set; }
	}
}


