using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class OemInstallFailedEventArgs : BrowserEventArgs
	{
		public OemInstallFailedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


