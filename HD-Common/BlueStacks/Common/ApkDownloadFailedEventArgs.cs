using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class ApkDownloadFailedEventArgs : BrowserEventArgs
	{
		public ApkDownloadFailedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


