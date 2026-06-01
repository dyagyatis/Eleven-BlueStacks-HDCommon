using System;

namespace BlueStacks.Common
{
	[Serializable]
	public struct WINDOWPLACEMENT
	{
		public int length { readonly get; set; }

		public int flags { readonly get; set; }

		public int showCmd { readonly get; set; }

		public POINT minPosition { readonly get; set; }

		public POINT maxPosition { readonly get; set; }

		public RECT normalPosition { readonly get; set; }
	}
}


