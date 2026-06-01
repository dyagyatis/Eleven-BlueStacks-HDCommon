using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BlueStacks.Common
{
	public static class IconHelper
	{
		public static bool ConvertToIcon(Stream input, Stream output, int size = 16, bool preserveAspectRatio = false)
		{
			Bitmap bitmap = (Bitmap)Image.FromStream(input);
			if (bitmap == null)
			{
				return false;
			}
			float num = (float)size;
			float num2 = (float)size;
			if (preserveAspectRatio)
			{
				if (bitmap.Width > bitmap.Height)
				{
					num2 = (float)bitmap.Height / (float)bitmap.Width * (float)size;
				}
				else
				{
					num = (float)bitmap.Width / (float)bitmap.Height * (float)size;
				}
			}
			bool flag;
			using (Bitmap bitmap2 = new Bitmap(bitmap, new Size((int)num, (int)num2)))
			{
				if (bitmap2 == null)
				{
					flag = false;
				}
				else
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						bitmap2.Save(memoryStream, ImageFormat.Png);
						using (BinaryWriter binaryWriter = new BinaryWriter(output))
						{
							if (output == null || binaryWriter == null)
							{
								return false;
							}
							binaryWriter.Write(0);
							binaryWriter.Write(0);
							binaryWriter.Write(1);
							binaryWriter.Write(1);
							binaryWriter.Write((byte)num);
							binaryWriter.Write((byte)num2);
							binaryWriter.Write(0);
							binaryWriter.Write(0);
							binaryWriter.Write(0);
							binaryWriter.Write(32);
							binaryWriter.Write((int)memoryStream.Length);
							binaryWriter.Write(22);
							binaryWriter.Write(memoryStream.ToArray());
							binaryWriter.Flush();
							binaryWriter.Close();
						}
					}
					flag = true;
				}
			}
			return flag;
		}

		public static bool ConvertToIcon(string inputPath, string outputPath, int size = 16, bool preserveAspectRatio = false)
		{
			bool flag;
			using (FileStream fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
			{
				using (FileStream fileStream2 = new FileStream(outputPath, FileMode.OpenOrCreate))
				{
					flag = IconHelper.ConvertToIcon(fileStream, fileStream2, size, preserveAspectRatio);
				}
			}
			return flag;
		}

		public static byte[] ConvertToIcon(Image image, bool preserveAspectRatio = false)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				if (image != null)
				{
					image.Save(memoryStream, ImageFormat.Png);
				}
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					int width = image.Size.Width;
					if (!IconHelper.ConvertToIcon(memoryStream, memoryStream2, width, preserveAspectRatio))
					{
						array = null;
					}
					else
					{
						array = memoryStream2.ToArray();
					}
				}
			}
			return array;
		}
	}
}


