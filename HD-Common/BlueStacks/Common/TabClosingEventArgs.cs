using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class TabClosingEventArgs : BrowserEventArgs
	{
		public TabClosingEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


