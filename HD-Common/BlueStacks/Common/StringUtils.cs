using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BlueStacks.Common
{
	public static class StringUtils
	{
		public static string GetControlCharFreeString(string s)
		{
			return new string(s.Where((char c) => !char.IsControl(c)).ToArray<char>());
		}

		public static string Encode(Dictionary<string, string> data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (data != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in data)
				{
					stringBuilder.AppendFormat("{0}={1}&", keyValuePair.Key, HttpUtility.UrlEncode(keyValuePair.Value));
				}
			}
			return stringBuilder.ToString().TrimEnd(new char[] { '&' });
		}
	}
}


