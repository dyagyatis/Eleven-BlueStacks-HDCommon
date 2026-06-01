using System;

namespace BlueStacks.Common
{
	public struct GamePad
	{
		public int X { readonly get; set; }

		public int Y { readonly get; set; }

		public int Z { readonly get; set; }

		public int Rx { readonly get; set; }

		public int Ry { readonly get; set; }

		public int Rz { readonly get; set; }

		public int Hat { readonly get; set; }

		public uint Mask { readonly get; set; }
	}
}


