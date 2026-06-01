using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
	public class CustomVolumeEventArgs : EventArgs
	{
		public int Volume { get; set; }

		public Dictionary<string, string> dictData { get; set; }

		public string mSelected { get; set; }

		public CustomVolumeEventArgs(int volume)
		{
			this.Volume = volume;
		}

		public CustomVolumeEventArgs(Dictionary<string, string> dict, string selected)
		{
			this.dictData = dict;
			this.mSelected = selected;
		}
	}
}


