using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class ApkDownloadStartedEventArgs : BrowserEventArgs
	{
		public ApkDownloadStartedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


