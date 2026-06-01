using System;
using System.Drawing;

namespace BlueStacks.Common
{
	public struct COMPOSITIONFORM
	{
		public int dwStyle { readonly get; set; }

		public POINT ptCurrentPos { readonly get; set; }

		public RECT rcArea { readonly get; set; }
	}
}


