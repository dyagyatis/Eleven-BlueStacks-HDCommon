using System;
using System.IO;

namespace BlueStacks.Common.Decoding
{
	internal class GifImageData
	{
		public byte LzwMinimumCodeSize { get; set; }

		public byte[] CompressedData { get; set; }

		private GifImageData()
		{
		}

		internal static GifImageData ReadImageData(Stream stream, bool metadataOnly)
		{
			GifImageData gifImageData = new GifImageData();
			gifImageData.Read(stream, metadataOnly);
			return gifImageData;
		}

		private void Read(Stream stream, bool metadataOnly)
		{
			this.LzwMinimumCodeSize = (byte)stream.ReadByte();
			this.CompressedData = GifHelpers.ReadDataBlocks(stream, metadataOnly);
		}
	}
}


