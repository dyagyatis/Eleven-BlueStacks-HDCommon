using System;
using System.IO;

namespace BlueStacks.Common.Decoding
{
	internal class GifLogicalScreenDescriptor
	{
		public int Width { get; private set; }

		public int Height { get; private set; }

		public bool HasGlobalColorTable { get; private set; }

		public int ColorResolution { get; private set; }

		public bool IsGlobalColorTableSorted { get; private set; }

		public int GlobalColorTableSize { get; private set; }

		public int BackgroundColorIndex { get; private set; }

		public double PixelAspectRatio { get; private set; }

		internal static GifLogicalScreenDescriptor ReadLogicalScreenDescriptor(Stream stream)
		{
			GifLogicalScreenDescriptor gifLogicalScreenDescriptor = new GifLogicalScreenDescriptor();
			gifLogicalScreenDescriptor.Read(stream);
			return gifLogicalScreenDescriptor;
		}

		private void Read(Stream stream)
		{
			byte[] array = new byte[7];
			stream.ReadAll(array, 0, array.Length);
			this.Width = (int)BitConverter.ToUInt16(array, 0);
			this.Height = (int)BitConverter.ToUInt16(array, 2);
			byte b = array[4];
			this.HasGlobalColorTable = (b & 128) > 0;
			this.ColorResolution = ((b & 112) >> 4) + 1;
			this.IsGlobalColorTableSorted = (b & 8) > 0;
			this.GlobalColorTableSize = 1 << (int)((b & 7) + 1);
			this.BackgroundColorIndex = (int)array[5];
			this.PixelAspectRatio = ((array[5] == 0) ? 0.0 : ((double)(15 + array[5]) / 64.0));
		}
	}
}


