using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlueStacks.Common.Decoding
{
	internal class GifPlainTextExtension : GifExtension
	{
		public int BlockSize { get; private set; }

		public int Left { get; private set; }

		public int Top { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int CellWidth { get; private set; }

		public int CellHeight { get; private set; }

		public int ForegroundColorIndex { get; private set; }

		public int BackgroundColorIndex { get; private set; }

		public string Text { get; private set; }

		public IList<GifExtension> Extensions { get; private set; }

		private GifPlainTextExtension()
		{
		}

		internal override GifBlockKind Kind
		{
			get
			{
				return GifBlockKind.GraphicRendering;
			}
		}

		internal static GifPlainTextExtension ReadPlainText(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
		{
			GifPlainTextExtension gifPlainTextExtension = new GifPlainTextExtension();
			gifPlainTextExtension.Read(stream, controlExtensions, metadataOnly);
			return gifPlainTextExtension;
		}

		private void Read(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
		{
			byte[] array = new byte[13];
			stream.ReadAll(array, 0, array.Length);
			this.BlockSize = (int)array[0];
			if (this.BlockSize != 12)
			{
				throw GifHelpers.InvalidBlockSizeException("Plain Text Extension", 12, this.BlockSize);
			}
			this.Left = (int)BitConverter.ToUInt16(array, 1);
			this.Top = (int)BitConverter.ToUInt16(array, 3);
			this.Width = (int)BitConverter.ToUInt16(array, 5);
			this.Height = (int)BitConverter.ToUInt16(array, 7);
			this.CellWidth = (int)array[9];
			this.CellHeight = (int)array[10];
			this.ForegroundColorIndex = (int)array[11];
			this.BackgroundColorIndex = (int)array[12];
			byte[] array2 = GifHelpers.ReadDataBlocks(stream, metadataOnly);
			this.Text = Encoding.ASCII.GetString(array2);
			this.Extensions = controlExtensions.ToList<GifExtension>().AsReadOnly();
		}

		internal const int ExtensionLabel = 1;
	}
}


