using System;
using System.Collections.Generic;
using System.IO;

namespace BlueStacks.Common.Decoding
{
	internal abstract class GifBlock
	{
		internal static GifBlock ReadBlock(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
		{
			int num = stream.ReadByte();
			if (num < 0)
			{
				throw GifHelpers.UnexpectedEndOfStreamException();
			}
			if (num == 33)
			{
				return GifExtension.ReadExtension(stream, controlExtensions, metadataOnly);
			}
			if (num == 44)
			{
				return GifFrame.ReadFrame(stream, controlExtensions, metadataOnly);
			}
			if (num != 59)
			{
				throw GifHelpers.UnknownBlockTypeException(num);
			}
			return GifTrailer.ReadTrailer();
		}

		internal abstract GifBlockKind Kind { get; }
	}
}


