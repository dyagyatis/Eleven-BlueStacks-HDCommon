using System;
using System.Globalization;

namespace BlueStacks.Common
{
	public class Digest
	{
		public uint A { get; set; }

		public uint B { get; set; }

		public uint C { get; set; }

		public uint D { get; set; }

		public Digest()
		{
			this.A = 1732584193U;
			this.B = 4023233417U;
			this.C = 2562383102U;
			this.D = 271733878U;
		}

		public string GetString()
		{
			return Digest.ReverseByte(this.A).ToString("X8", CultureInfo.InvariantCulture) + Digest.ReverseByte(this.B).ToString("X8", CultureInfo.InvariantCulture) + Digest.ReverseByte(this.C).ToString("X8", CultureInfo.InvariantCulture) + Digest.ReverseByte(this.D).ToString("X8", CultureInfo.InvariantCulture);
		}

		private static uint ReverseByte(uint uiNumber)
		{
			return ((uiNumber & 255U) << 24) | (uiNumber >> 24) | ((uiNumber & 16711680U) >> 8) | ((uiNumber & 65280U) << 8);
		}
	}
}


