using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using BlueStacks.Common;

public class Manifest
{
	public Manifest(string filePath)
	{
		this.m_FileParts = new List<FilePart>();
		this.m_FilePath = filePath;
	}

	public bool Check()
	{
		int num = 0;
		using (List<FilePart>.Enumerator enumerator = this.m_FileParts.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.Check())
				{
					Logger.Error("Check failed for part " + num.ToString());
					return false;
				}
				num++;
			}
		}
		return true;
	}

	public void Build()
	{
		using (StreamReader streamReader = new StreamReader(File.OpenRead(this.m_FilePath)))
		{
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				string[] array = text.Split(new char[] { ' ' });
				string text2 = array[0];
				long num = Convert.ToInt64(array[1], CultureInfo.InvariantCulture);
				string text3 = array[2];
				string text4 = Path.Combine(Path.GetDirectoryName(this.m_FilePath), text2);
				FilePart filePart = new FilePart(text2, num, text3, text4);
				if (filePart.Check())
				{
					filePart.DownloadedSize = filePart.Size;
				}
				this.m_FileParts.Add(filePart);
				this.FileSize += num;
			}
		}
	}

	public void Dump()
	{
		foreach (FilePart filePart in this.m_FileParts)
		{
			Logger.Info("{0} {1} {2}", new object[] { filePart.Name, filePart.Size, filePart.SHA1 });
		}
	}

	public long Count
	{
		get
		{
			return (long)this.m_FileParts.Count;
		}
	}

	public FilePart this[int i]
	{
		get
		{
			return this.m_FileParts[i];
		}
	}

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool FlushFileBuffers(IntPtr handle);

	public string MakeFile()
	{
		int num = 16384;
		byte[] array = new byte[num];
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.m_FilePath);
		string text = Path.Combine(Path.GetDirectoryName(this.m_FilePath), fileNameWithoutExtension);
		using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.None))
		{
			foreach (FilePart filePart in this.m_FileParts)
			{
				using (Stream stream = new FileStream(filePart.Path, FileMode.Open, FileAccess.Read))
				{
					int num2;
					while ((num2 = stream.Read(array, 0, num)) > 0)
					{
						fileStream.Write(array, 0, num2);
					}
				}
			}
			fileStream.Flush();
			if (!Manifest.FlushFileBuffers(fileStream.Handle))
			{
				throw new SystemException("Win32 FlushFileBuffers failed for " + text, new Win32Exception(Marshal.GetLastWin32Error()));
			}
		}
		return text;
	}

	public void DeleteFileParts()
	{
		foreach (FilePart filePart in this.m_FileParts)
		{
			File.Delete(filePart.Path);
		}
	}

	public void DeleteManifest()
	{
		File.Delete(this.m_FilePath);
	}

	public long DownloadedSize
	{
		get
		{
			long num = 0L;
			foreach (FilePart filePart in this.m_FileParts)
			{
				num += filePart.DownloadedSize;
			}
			return num;
		}
	}

	public long FileSize { get; private set; }

	public float PercentDownloaded()
	{
		return (float)Math.Round((double)this.DownloadedSize * 100.0 / (double)this.FileSize, 1);
	}

	private List<FilePart> m_FileParts;

	private string m_FilePath;
}


