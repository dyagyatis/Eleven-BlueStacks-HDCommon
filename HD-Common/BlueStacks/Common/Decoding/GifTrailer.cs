using System;

namespace BlueStacks.Common.Decoding
{
	internal class GifTrailer : GifBlock
	{
		private GifTrailer()
		{
		}

		internal override GifBlockKind Kind
		{
			get
			{
				return GifBlockKind.Other;
			}
		}

		internal static GifTrailer ReadTrailer()
		{
			return new GifTrailer();
		}

		internal const int TrailerByte = 59;
	}
}


