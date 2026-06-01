using System;
using System.IO;

namespace BlueStacks.Common.Decoding
{
	internal class GifGraphicControlExtension : GifExtension
	{
		public int BlockSize { get; private set; }

		public int DisposalMethod { get; private set; }

		public bool UserInput { get; private set; }

		public bool HasTransparency { get; private set; }

		public int Delay { get; private set; }

		public int TransparencyIndex { get; private set; }

		private GifGraphicControlExtension()
		{
		}

		internal override GifBlockKind Kind
		{
			get
			{
				return GifBlockKind.Control;
			}
		}

		internal static GifGraphicControlExtension ReadGraphicsControl(Stream stream)
		{
			GifGraphicControlExtension gifGraphicControlExtension = new GifGraphicControlExtension();
			gifGraphicControlExtension.Read(stream);
			return gifGraphicControlExtension;
		}

		private void Read(Stream stream)
		{
			byte[] array = new byte[6];
			stream.ReadAll(array, 0, array.Length);
			this.BlockSize = (int)array[0];
			if (this.BlockSize != 4)
			{
				throw GifHelpers.InvalidBlockSizeException("Graphic Control Extension", 4, this.BlockSize);
			}
			byte b = array[1];
			this.DisposalMethod = (b & 28) >> 2;
			this.UserInput = (b & 2) > 0;
			this.HasTransparency = (b & 1) > 0;
			this.Delay = (int)(BitConverter.ToUInt16(array, 2) * 10);
			this.TransparencyIndex = (int)array[4];
		}

		internal const int ExtensionLabel = 249;
	}
}


