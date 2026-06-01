using System;
using System.IO;
using System.Text;

namespace BlueStacks.Common.Decoding
{
	internal class GifCommentExtension : GifExtension
	{
		public string Text { get; private set; }

		private GifCommentExtension()
		{
		}

		internal override GifBlockKind Kind
		{
			get
			{
				return GifBlockKind.SpecialPurpose;
			}
		}

		internal static GifCommentExtension ReadComment(Stream stream)
		{
			GifCommentExtension gifCommentExtension = new GifCommentExtension();
			gifCommentExtension.Read(stream);
			return gifCommentExtension;
		}

		private void Read(Stream stream)
		{
			byte[] array = GifHelpers.ReadDataBlocks(stream, false);
			if (array != null)
			{
				this.Text = Encoding.ASCII.GetString(array);
			}
		}

		internal const int ExtensionLabel = 254;
	}
}


