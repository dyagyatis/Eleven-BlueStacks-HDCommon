using System;
using System.Collections.Generic;
using System.IO;

namespace BlueStacks.Common.Decoding
{
	internal abstract class GifExtension : GifBlock
	{
		internal static GifExtension ReadExtension(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
		{
			int num = stream.ReadByte();
			if (num < 0)
			{
				throw GifHelpers.UnexpectedEndOfStreamException();
			}
			if (num <= 249)
			{
				if (num == 1)
				{
					return GifPlainTextExtension.ReadPlainText(stream, controlExtensions, metadataOnly);
				}
				if (num == 249)
				{
					return GifGraphicControlExtension.ReadGraphicsControl(stream);
				}
			}
			else
			{
				if (num == 254)
				{
					return GifCommentExtension.ReadComment(stream);
				}
				if (num == 255)
				{
					return GifApplicationExtension.ReadApplication(stream);
				}
			}
			throw GifHelpers.UnknownExtensionTypeException(num);
		}

		internal const int ExtensionIntroducer = 33;
	}
}


