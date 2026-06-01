using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BlueStacks.Common
{
	public static class StringExtensions
	{
		public static bool IsNullOrWhiteSpace(this string value)
		{
			if (value == null)
			{
				return true;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsSubPathOf(this string baseDirPath, string path)
		{
			string fullPath = Path.GetFullPath((path != null) ? path.Replace('/', '\\').WithEnding("\\") : null);
			string fullPath2 = Path.GetFullPath((baseDirPath != null) ? baseDirPath.Replace('/', '\\').WithEnding("\\") : null);
			return fullPath.StartsWith(fullPath2, StringComparison.OrdinalIgnoreCase);
		}

		public static string WithEnding(this string str, string ending)
		{
			if (str == null)
			{
				return ending;
			}
			int num = 0;
			string text;
			for (;;)
			{
				int num2 = num;
				int? num3 = ((ending != null) ? new int?(ending.Length) : null);
				if (!((num2 <= num3.GetValueOrDefault()) & (num3 != null)))
				{
					return str;
				}
				text = str + ending.Right(num);
				if (text.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
				{
					break;
				}
				num++;
			}
			return text;
		}

		public static string Right(this string value, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
			}
			if (length >= value.Length)
			{
				return value;
			}
			return value.Substring(value.Length - length);
		}

		public static bool IsValidFileName(this string value)
		{
			if (value == null)
			{
				return false;
			}
			value = value.Trim();
			return !string.IsNullOrEmpty(value) && value.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
		}

		public static bool GetValidFileName(this string value, out string fileName)
		{
			fileName = value;
			if (string.IsNullOrEmpty(fileName))
			{
				return false;
			}
			fileName = fileName.Trim();
			if (string.IsNullOrEmpty(fileName))
			{
				return false;
			}
			for (int i = fileName.IndexOfAny(Path.GetInvalidFileNameChars()); i >= 0; i = fileName.IndexOfAny(Path.GetInvalidFileNameChars()))
			{
				fileName = fileName.Remove(i, 1);
			}
			return !string.IsNullOrEmpty(fileName);
		}

		public static string CombinePathWith(this string path1, string path2)
		{
			return Path.Combine(path1, path2);
		}

		public static bool IsValidPath(string path)
		{
			if (!new Regex("^[a-zA-Z]:\\\\$").IsMatch((path != null) ? path.Substring(0, 3) : null))
			{
				return false;
			}
			string text = new string(Path.GetInvalidPathChars());
			text += ":/?*\"";
			if (new Regex("[" + Regex.Escape(text) + "]").IsMatch(path.Substring(3, path.Length - 3)))
			{
				return false;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetFullPath(path));
			if (!directoryInfo.Exists)
			{
				try
				{
					directoryInfo.Create();
				}
				catch
				{
					return false;
				}
				return true;
			}
			return true;
		}

		public static T ToEnum<T>(this string value, bool ignoreCase = true)
		{
			return (T)((object)Enum.Parse(typeof(T), value, ignoreCase));
		}

		public static string TrimStart(this string target, string trimString)
		{
			if (string.IsNullOrEmpty(trimString))
			{
				return target;
			}
			string text = target;
			if (target != null)
			{
				while (text.StartsWith(trimString, StringComparison.InvariantCultureIgnoreCase))
				{
					text = text.Substring(trimString.Length);
				}
			}
			return text;
		}
	}
}


