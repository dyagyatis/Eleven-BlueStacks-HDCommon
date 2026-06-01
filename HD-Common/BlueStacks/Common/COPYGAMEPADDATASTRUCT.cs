using System;

namespace BlueStacks.Common
{
	public struct COPYGAMEPADDATASTRUCT
	{
		public IntPtr dwData { readonly get; set; }

		public int size { readonly get; set; }

		public IntPtr lpData { readonly get; set; }
	}
}


