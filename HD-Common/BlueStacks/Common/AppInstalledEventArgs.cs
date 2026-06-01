using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class AppInstalledEventArgs : BrowserEventArgs
	{
		public AppInstalledEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


