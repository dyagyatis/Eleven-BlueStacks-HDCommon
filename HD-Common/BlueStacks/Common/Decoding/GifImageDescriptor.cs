using System;
using System.IO;

namespace BlueStacks.Common.Decoding
{
	internal class GifImageDescriptor
	{
		public int Left { get; private set; }

		public int Top { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public bool HasLocalColorTable { get; private set; }

		public bool Interlace { get; private set; }

		public bool IsLocalColorTableSorted { get; private set; }

		public int LocalColorTableSize { get; private set; }

		private GifImageDescriptor()
		{
		}

		internal static GifImageDescriptor ReadImageDescriptor(Stream stream)
		{
			GifImageDescriptor gifImageDescriptor = new GifImageDescriptor();
			gifImageDescriptor.Read(stream);
			return gifImageDescriptor;
		}

		private void Read(Stream stream)
		{
			byte[] array = new byte[9];
			stream.ReadAll(array, 0, array.Length);
			this.Left = (int)BitConverter.ToUInt16(array, 0);
			this.Top = (int)BitConverter.ToUInt16(array, 2);
			this.Width = (int)BitConverter.ToUInt16(array, 4);
			this.Height = (int)BitConverter.ToUInt16(array, 6);
			byte b = array[8];
			this.HasLocalColorTable = (b & 128) > 0;
			this.Interlace = (b & 64) > 0;
			this.IsLocalColorTableSorted = (b & 32) > 0;
			this.LocalColorTableSize = 1 << (int)((b & 7) + 1);
		}
	}
}


