using System;
using System.Collections.Generic;
using System.IO;

namespace BlueStacks.Common
{
	public static class IOUtils
	{
		public static void DeleteIfExists(IEnumerable<string> filesToDelete)
		{
			if (filesToDelete != null)
			{
				foreach (string text in filesToDelete)
				{
					try
					{
						if (File.Exists(text))
						{
							File.Delete(text);
						}
					}
					catch (Exception ex)
					{
						Logger.Error("Exception while deleting file " + text + ex.ToString());
					}
				}
			}
		}

		public static long GetAvailableDiskSpaceOfDrive(string path)
		{
			return new DriveInfo(path).AvailableFreeSpace;
		}

		public static string GetPartitionNameFromPath(string path)
		{
			return new DriveInfo(path).Name;
		}

		public static long GetDirectorySize(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				return 0L;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
			long num = 0L;
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				num += fileInfo.Length;
			}
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				num += IOUtils.GetDirectorySize(directoryInfo2.FullName);
			}
			return num;
		}

		public static bool IfPathExists(string path)
		{
			return new DirectoryInfo(path).Exists || new FileInfo(path).Exists;
		}

		public static string GetFileOrFolderName(string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (directoryInfo.Exists)
			{
				return directoryInfo.Name;
			}
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				return fileInfo.Name;
			}
			throw new IOException("File or folder name does not exist");
		}

		public static bool IsDirectoryEmpty(string dir)
		{
			bool flag = true;
			if (!Directory.Exists(dir))
			{
				Logger.Info(dir + " does not exist");
				return flag;
			}
			if (Directory.GetFiles(dir).Length == 0)
			{
				Logger.Info(dir + " is empty");
			}
			else
			{
				flag = false;
			}
			foreach (string text in Directory.GetDirectories(dir))
			{
				Directory.GetFiles(text);
				if (!IOUtils.IsDirectoryEmpty(text))
				{
					flag = false;
				}
			}
			return flag;
		}

		public static readonly char[] DisallowedCharsInDirs = new char[] { '&', '<', '>', '"', '\'', '^' };
	}
}


