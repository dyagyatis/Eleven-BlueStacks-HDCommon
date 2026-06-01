using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class OemInstallStartedEventArgs : BrowserEventArgs
	{
		public OemInstallStartedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


