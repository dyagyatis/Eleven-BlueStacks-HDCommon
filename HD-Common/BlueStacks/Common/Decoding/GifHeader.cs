using System;
using System.IO;

namespace BlueStacks.Common.Decoding
{
	internal class GifHeader : GifBlock
	{
		public string Signature { get; private set; }

		public string Version { get; private set; }

		public GifLogicalScreenDescriptor LogicalScreenDescriptor { get; private set; }

		private GifHeader()
		{
		}

		internal override GifBlockKind Kind
		{
			get
			{
				return GifBlockKind.Other;
			}
		}

		internal static GifHeader ReadHeader(Stream stream)
		{
			GifHeader gifHeader = new GifHeader();
			gifHeader.Read(stream);
			return gifHeader;
		}

		private void Read(Stream stream)
		{
			this.Signature = GifHelpers.ReadString(stream, 3);
			if (this.Signature != "GIF")
			{
				throw GifHelpers.InvalidSignatureException(this.Signature);
			}
			this.Version = GifHelpers.ReadString(stream, 3);
			if (this.Version != "87a" && this.Version != "89a")
			{
				throw GifHelpers.UnsupportedVersionException(this.Version);
			}
			this.LogicalScreenDescriptor = GifLogicalScreenDescriptor.ReadLogicalScreenDescriptor(stream);
		}
	}
}


