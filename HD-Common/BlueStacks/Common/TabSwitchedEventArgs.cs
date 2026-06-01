using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class TabSwitchedEventArgs : BrowserEventArgs
	{
		public TabSwitchedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


