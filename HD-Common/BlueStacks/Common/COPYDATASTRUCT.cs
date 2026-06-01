using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	public struct COPYDATASTRUCT
	{
		public IntPtr dwData { readonly get; set; }

		public int cbData { readonly get; set; }

		public IntPtr lpData { readonly get; set; }

		public static COPYDATASTRUCT CreateForString(int dwData, string value, bool _ = false)
		{
			return new COPYDATASTRUCT
			{
				dwData = (IntPtr)dwData,
				cbData = ((value != null) ? value.Length : 1) * 2,
				lpData = Marshal.StringToHGlobalUni(value)
			};
		}
	}
}


