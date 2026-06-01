using System;

namespace BlueStacks.Common
{
	[Serializable]
	public struct POINT
	{
		public int X { readonly get; set; }

		public int Y { readonly get; set; }

		public POINT(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}
}


