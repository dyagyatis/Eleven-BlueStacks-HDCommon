using System;

namespace BlueStacks.Common
{
	public class StringEventArgs : EventArgs
	{
		public string Str { get; set; } = string.Empty;

		public StringEventArgs(string str)
		{
			this.Str = str;
		}
	}
}


