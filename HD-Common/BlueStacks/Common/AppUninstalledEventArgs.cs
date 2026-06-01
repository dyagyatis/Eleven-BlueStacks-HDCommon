using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class AppUninstalledEventArgs : BrowserEventArgs
	{
		public AppUninstalledEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


