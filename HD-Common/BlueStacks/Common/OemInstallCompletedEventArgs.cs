using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class OemInstallCompletedEventArgs : BrowserEventArgs
	{
		public OemInstallCompletedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


