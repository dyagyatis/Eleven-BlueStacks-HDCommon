using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class OemDownloadFailedEventArgs : BrowserEventArgs
	{
		public OemDownloadFailedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


