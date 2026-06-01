using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class OemDownloadStartedEventArgs : BrowserEventArgs
	{
		public OemDownloadStartedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


