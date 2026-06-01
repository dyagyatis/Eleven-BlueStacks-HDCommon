using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

internal class SplitFile
{
	public static void Split(string path, int size, SplitFile.ProgressCb progressCb)
	{
		byte[] array = new byte[16384];
		using (Stream stream = File.OpenRead(path))
		{
			int num = 0;
			string.Format(CultureInfo.InvariantCulture, "{0}.manifest", new object[] { path });
			while (stream.Position < stream.Length)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "{0}_part_{1}", new object[] { path, num });
				using (Stream stream2 = File.Create(text))
				{
					int num2;
					for (int i = size; i > 0; i -= num2)
					{
						num2 = stream.Read(array, 0, Math.Min(i, 16384));
						if (num2 == 0)
						{
							break;
						}
						stream2.Write(array, 0, num2);
					}
				}
				string text2 = null;
				using (Stream stream3 = File.OpenRead(text))
				{
					string text3 = SplitFile.CheckSum(stream3);
					long length = stream3.Length;
					text2 = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[]
					{
						Path.GetFileName(text),
						length,
						text3
					});
				}
				progressCb(text2);
				num++;
			}
		}
	}

	public static string CheckSum(Stream stream)
	{
		string text;
		using (SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
		{
			byte[] array = sha1CryptoServiceProvider.ComputeHash(stream);
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			foreach (byte b in array)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			text = stringBuilder.ToString();
		}
		return text;
	}

	public delegate void ProgressCb(string manifest);
}


