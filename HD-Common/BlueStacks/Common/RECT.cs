using System;

namespace BlueStacks.Common
{
	[Serializable]
	public struct RECT
	{
		public int Left { readonly get; set; }

		public int Top { readonly get; set; }

		public int Right { readonly get; set; }

		public int Bottom { readonly get; set; }

		public RECT(int left, int top, int right, int bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	}
}


