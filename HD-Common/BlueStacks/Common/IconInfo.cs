using System;

namespace BlueStacks.Common
{
	public struct IconInfo
	{
		public bool fIcon { readonly get; set; }

		public int xHotspot { readonly get; set; }

		public int yHotspot { readonly get; set; }

		public IntPtr hbmMask { readonly get; set; }

		public IntPtr hbmColor { readonly get; set; }
	}
}


