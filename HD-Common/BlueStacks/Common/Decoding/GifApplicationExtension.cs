using System;
using System.IO;
using System.Text;

namespace BlueStacks.Common.Decoding
{
	internal class GifApplicationExtension : GifExtension
	{
		public int BlockSize { get; private set; }

		public string ApplicationIdentifier { get; private set; }

		public byte[] AuthenticationCode { get; private set; }

		public byte[] Data { get; private set; }

		private GifApplicationExtension()
		{
		}

		internal override GifBlockKind Kind
		{
			get
			{
				return GifBlockKind.SpecialPurpose;
			}
		}

		internal static GifApplicationExtension ReadApplication(Stream stream)
		{
			GifApplicationExtension gifApplicationExtension = new GifApplicationExtension();
			gifApplicationExtension.Read(stream);
			return gifApplicationExtension;
		}

		private void Read(Stream stream)
		{
			byte[] array = new byte[12];
			stream.ReadAll(array, 0, array.Length);
			this.BlockSize = (int)array[0];
			if (this.BlockSize != 11)
			{
				throw GifHelpers.InvalidBlockSizeException("Application Extension", 11, this.BlockSize);
			}
			this.ApplicationIdentifier = Encoding.ASCII.GetString(array, 1, 8);
			byte[] array2 = new byte[3];
			Array.Copy(array, 9, array2, 0, 3);
			this.AuthenticationCode = array2;
			this.Data = GifHelpers.ReadDataBlocks(stream, false);
		}

		internal const int ExtensionLabel = 255;
	}
}


