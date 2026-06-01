using System;
using System.Globalization;

namespace BlueStacks.Common.Decoding
{
	internal struct GifColor
	{
		internal GifColor(byte r, byte g, byte b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "#{0:x2}{1:x2}{2:x2}", new object[] { this.R, this.G, this.B });
		}

		private readonly byte R;

		private readonly byte G;

		private readonly byte B;
	}
}


