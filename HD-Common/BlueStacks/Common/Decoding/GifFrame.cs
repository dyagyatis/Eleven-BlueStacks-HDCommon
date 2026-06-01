using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlueStacks.Common.Decoding
{
	internal class GifFrame : GifBlock
	{
		public GifImageDescriptor Descriptor { get; private set; }

		public GifColor[] LocalColorTable { get; private set; }

		public IList<GifExtension> Extensions { get; private set; }

		public GifImageData ImageData { get; private set; }

		private GifFrame()
		{
		}

		internal override GifBlockKind Kind
		{
			get
			{
				return GifBlockKind.GraphicRendering;
			}
		}

		internal static GifFrame ReadFrame(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
		{
			GifFrame gifFrame = new GifFrame();
			gifFrame.Read(stream, controlExtensions, metadataOnly);
			return gifFrame;
		}

		private void Read(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
		{
			this.Descriptor = GifImageDescriptor.ReadImageDescriptor(stream);
			if (this.Descriptor.HasLocalColorTable)
			{
				this.LocalColorTable = GifHelpers.ReadColorTable(stream, this.Descriptor.LocalColorTableSize);
			}
			this.ImageData = GifImageData.ReadImageData(stream, metadataOnly);
			this.Extensions = controlExtensions.ToList<GifExtension>().AsReadOnly();
		}

		internal const int ImageSeparator = 44;
	}
}


