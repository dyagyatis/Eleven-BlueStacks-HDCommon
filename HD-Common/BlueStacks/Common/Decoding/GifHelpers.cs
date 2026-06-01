using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace BlueStacks.Common.Decoding
{
	internal static class GifHelpers
	{
		public static string ReadString(Stream stream, int length)
		{
			byte[] array = new byte[length];
			stream.ReadAll(array, 0, length);
			return Encoding.ASCII.GetString(array);
		}

		public static byte[] ReadDataBlocks(Stream stream, bool discard)
		{
			MemoryStream memoryStream = (discard ? null : new MemoryStream());
			byte[] array2;
			using (memoryStream)
			{
				int num;
				while ((num = stream.ReadByte()) > 0)
				{
					byte[] array = new byte[num];
					stream.ReadAll(array, 0, num);
					if (memoryStream != null)
					{
						memoryStream.Write(array, 0, num);
					}
				}
				if (memoryStream != null)
				{
					array2 = memoryStream.ToArray();
				}
				else
				{
					array2 = null;
				}
			}
			return array2;
		}

		public static GifColor[] ReadColorTable(Stream stream, int size)
		{
			int num = 3 * size;
			byte[] array = new byte[num];
			stream.ReadAll(array, 0, num);
			GifColor[] array2 = new GifColor[size];
			for (int i = 0; i < size; i++)
			{
				byte b = array[3 * i];
				byte b2 = array[3 * i + 1];
				byte b3 = array[3 * i + 2];
				array2[i] = new GifColor(b, b2, b3);
			}
			return array2;
		}

		public static bool IsNetscapeExtension(GifApplicationExtension ext)
		{
			return ext.ApplicationIdentifier == "NETSCAPE" && Encoding.ASCII.GetString(ext.AuthenticationCode) == "2.0";
		}

		public static ushort GetRepeatCount(GifApplicationExtension ext)
		{
			if (ext.Data.Length >= 3)
			{
				return BitConverter.ToUInt16(ext.Data, 1);
			}
			return 1;
		}

		public static Exception UnexpectedEndOfStreamException()
		{
			return new GifDecoderException("Unexpected end of stream before trailer was encountered");
		}

		public static Exception UnknownBlockTypeException(int blockId)
		{
			return new GifDecoderException("Unknown block type: 0x" + blockId.ToString("x2", CultureInfo.InvariantCulture));
		}

		public static Exception UnknownExtensionTypeException(int extensionLabel)
		{
			return new GifDecoderException("Unknown extension type: 0x" + extensionLabel.ToString("x2", CultureInfo.InvariantCulture));
		}

		public static Exception InvalidBlockSizeException(string blockName, int expectedBlockSize, int actualBlockSize)
		{
			return new GifDecoderException(string.Format(CultureInfo.InvariantCulture, "Invalid block size for {0}. Expected {1}, but was {2}", new object[] { blockName, expectedBlockSize, actualBlockSize }));
		}

		public static Exception InvalidSignatureException(string signature)
		{
			return new GifDecoderException("Invalid file signature: " + signature);
		}

		public static Exception UnsupportedVersionException(string version)
		{
			return new GifDecoderException("Unsupported version: " + version);
		}

		public static void ReadAll(this Stream stream, byte[] buffer, int offset, int count)
		{
			for (int i = 0; i < count; i += stream.Read(buffer, offset + i, count - i))
			{
			}
		}
	}
}


