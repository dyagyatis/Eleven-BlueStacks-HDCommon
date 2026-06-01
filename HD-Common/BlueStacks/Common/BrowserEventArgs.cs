using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class BrowserEventArgs : EventArgs
	{
		public BrowserControlTags ClientTag { get; private set; }

		public string mVmName { get; private set; } = string.Empty;

		public JObject ExtraData { get; private set; }

		public BrowserEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
		{
			this.ClientTag = tag;
			this.mVmName = vmName;
			this.ExtraData = extraData;
		}
	}
}


