using System;
using System.IO;
using BlueStacks.Common;

public class FilePart
{
	public FilePart(string name, long size, string sha1, string path)
	{
		this.Name = name;
		this.Size = size;
		this.SHA1 = sha1;
		this.Path = path;
		this.DownloadedSize = 0L;
	}

	public string Name { get; }

	public long Size { get; }

	public string SHA1 { get; }

	public string URL(string manifestURL)
	{
		return ((manifestURL != null) ? manifestURL.Substring(0, manifestURL.LastIndexOf('/') + 1) : null) + this.Name;
	}

	public string Path { get; }

	public long DownloadedSize { get; set; }

	public bool Check()
	{
		Logger.Info("Will check " + this.Path);
		bool flag = false;
		if (!File.Exists(this.Path))
		{
			Logger.Error("File missing");
			return false;
		}
		using (Stream stream = File.OpenRead(this.Path))
		{
			if (stream.Length != this.Size)
			{
				Logger.Error("File size incorrect: " + stream.Length.ToString());
				return false;
			}
			if (SplitFile.CheckSum(stream) == this.SHA1)
			{
				this.DownloadedSize = this.Size;
				Logger.Info("File size correct");
				flag = true;
			}
		}
		return flag;
	}
}


