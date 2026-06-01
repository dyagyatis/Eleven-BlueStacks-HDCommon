using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace BlueStacks.Common
{
	public static class ImageUtils
	{
		public static BitmapImage BitmapFromPath(string path)
		{
			BitmapImage bitmapImage = null;
			if (File.Exists(path))
			{
				bitmapImage = new BitmapImage();
				try
				{
					using (FileStream fileStream = File.OpenRead(path))
					{
						bitmapImage.BeginInit();
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.StreamSource = fileStream;
						bitmapImage.EndInit();
					}
				}
				catch
				{
				}
			}
			return bitmapImage;
		}

		public static BitmapImage BitmapFromUri(string uri)
		{
			BitmapImage bitmapImage = null;
			try
			{
				bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.UriSource = new Uri(uri);
				bitmapImage.EndInit();
			}
			catch
			{
			}
			return bitmapImage;
		}

		public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
		{
			Bitmap bitmap;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new BmpBitmapEncoder
				{
					Frames = { BitmapFrame.Create(bitmapImage) }
				}.Save(memoryStream);
				bitmap = new Bitmap(memoryStream);
			}
			return bitmap;
		}

		public static BitmapImage ByteArrayToImage(byte[] dataArray)
		{
			BitmapImage bitmapImage2;
			using (MemoryStream memoryStream = new MemoryStream(dataArray))
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.EndInit();
				bitmapImage2 = bitmapImage;
			}
			return bitmapImage2;
		}
	}
}


