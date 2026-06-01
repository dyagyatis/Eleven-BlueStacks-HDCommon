using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class OemDownloadCompletedEventArgs : BrowserEventArgs
	{
		public OemDownloadCompletedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


