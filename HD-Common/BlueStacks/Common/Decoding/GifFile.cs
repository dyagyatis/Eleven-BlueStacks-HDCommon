using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlueStacks.Common.Decoding
{
	internal class GifFile
	{
		public GifHeader Header { get; private set; }

		public GifColor[] GlobalColorTable { get; set; }

		public IList<GifFrame> Frames { get; set; }

		public IList<GifExtension> Extensions { get; set; }

		public ushort RepeatCount { get; set; }

		private GifFile()
		{
		}

		internal static GifFile ReadGifFile(Stream stream, bool metadataOnly)
		{
			GifFile gifFile = new GifFile();
			gifFile.Read(stream, metadataOnly);
			return gifFile;
		}

		private void Read(Stream stream, bool metadataOnly)
		{
			this.Header = GifHeader.ReadHeader(stream);
			if (this.Header.LogicalScreenDescriptor.HasGlobalColorTable)
			{
				this.GlobalColorTable = GifHelpers.ReadColorTable(stream, this.Header.LogicalScreenDescriptor.GlobalColorTableSize);
			}
			this.ReadFrames(stream, metadataOnly);
			GifApplicationExtension gifApplicationExtension = this.Extensions.OfType<GifApplicationExtension>().FirstOrDefault(new Func<GifApplicationExtension, bool>(GifHelpers.IsNetscapeExtension));
			if (gifApplicationExtension != null)
			{
				this.RepeatCount = GifHelpers.GetRepeatCount(gifApplicationExtension);
				return;
			}
			this.RepeatCount = 1;
		}

		private void ReadFrames(Stream stream, bool metadataOnly)
		{
			List<GifFrame> list = new List<GifFrame>();
			List<GifExtension> list2 = new List<GifExtension>();
			List<GifExtension> list3 = new List<GifExtension>();
			for (;;)
			{
				GifBlock gifBlock = GifBlock.ReadBlock(stream, list2, metadataOnly);
				if (gifBlock.Kind == GifBlockKind.GraphicRendering)
				{
					list2 = new List<GifExtension>();
				}
				if (gifBlock is GifFrame)
				{
					list.Add((GifFrame)gifBlock);
				}
				else
				{
					GifExtension gifExtension = gifBlock as GifExtension;
					if (gifExtension != null)
					{
						GifBlockKind kind = gifExtension.Kind;
						if (kind != GifBlockKind.Control)
						{
							if (kind == GifBlockKind.SpecialPurpose)
							{
								list3.Add(gifExtension);
							}
						}
						else
						{
							list2.Add(gifExtension);
						}
					}
					else if (gifBlock is GifTrailer)
					{
						break;
					}
				}
			}
			this.Frames = list.AsReadOnly();
			this.Extensions = list3.AsReadOnly();
		}
	}
}


